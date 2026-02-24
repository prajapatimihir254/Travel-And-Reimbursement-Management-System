using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace BizTravel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        //to connect with database
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {   
            if(HttpContext.Session.GetString("UserRole") != "Accountant")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        private IActionResult RedirectToRole(string role)
        {
            if (role == "Admin") return RedirectToAction("Index", "Admin");
            if (role == "Manager") return RedirectToAction("Index", "Manager");
            if (role == "Accountant") return RedirectToAction("Index", "Accountant");
            if (role == "Employee") return RedirectToAction("Dashboard", "Employee");

            return RedirectToAction("Index", "Home");
        }

        //fix the brozers back button
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(string role)
        {
            if(HttpContext.Session.GetString("UserRole") != null)
            {
                return RedirectToRole(HttpContext.Session.GetString("UserRole"));   
            }
            //view set to viewbeg for the while use login page (same color login)
            ViewBag.SelectedRole = role;
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password are required.";
                return View();
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                
                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    //check the sql dummy deta from sql
                    if (user.Password == password || passwordHasher.VerifyHashedPassword(user,user.Password,password) == PasswordVerificationResult.Success)
                    {
                        HttpContext.Session.SetString("UserEmail", user.Email);
                        HttpContext.Session.SetString("EmployeeID", user.EmployeeID);
                        HttpContext.Session.SetString("Username", user.Fullname);
                        HttpContext.Session.SetString("UserRole",user.Role);

                        //Redirect With role name 
                        return user.Role switch
                        {
                            "Admin" => RedirectToAction("Index", "Admin"),
                            "Manager" => RedirectToAction("Index", "Manager"),
                            "Accountant" => RedirectToAction("Index", "Accountant"),
                            "Employee" => RedirectToAction("Dashboard", "Employee")
                        };
                    } 
                }
                ViewBag.Error = "Invalid Login";
                return View();
                
            }
            catch (Exception)
            {
                ViewBag.Error = "Incorrect Password Or Email";
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index","Home");
        }
        
    }
}
