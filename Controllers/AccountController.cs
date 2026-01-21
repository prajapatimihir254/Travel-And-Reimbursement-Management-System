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
        //public IActionResult Registration()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Registration(ApplicationUser model) 
        //{
        //    //find the last id from the database
        //    var lastUser = _context.Users.OrderByDescending(u => u.Id).FirstOrDefault();

        //    //if there is no empid in database or last empid + 1 = next id
        //    int nextID = (lastUser == null) ? 1 : lastUser.Id + 1;

        //    //formating the empid
        //    ViewBag.NextEmployeeID = "EMP" + nextID.ToString("D3");

        //    //if (ModelState.IsValid)
        //    //{
        //    //    ModelState.AddModelError("EmployeeID", "This EmployeeID is already register.");
        //    //    //return View(model);
        //    //    _context.Users.Add(model);
        //    //    _context.SaveChanges();
        //    //    return RedirectToAction("Login");
        //    //}

        //    if (ModelState.IsValid)
        //    {
        //        //for doing password hash
        //        var passwordHasher = new PasswordHasher<ApplicationUser>();
        //        model.Password = passwordHasher.HashPassword(model, model.Password);

        //        _context.Users.Add(model);
        //        _context.SaveChanges();
        //        return RedirectToAction("Login");
        //    }

        //    // Debugging: Check karne ke liye ki data aa raha hai ya nahi
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Users.Add(model);
        //            _context.SaveChanges();

        //            TempData["SuccessMessage"] = "Registration Successful!";
        //            return RedirectToAction("Registration");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Agar Database mein save karte waqt error aaye
        //            ViewBag.Error = "Database Error: " + ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        // Agar validation fail ho jaye (jaise koi field khali reh gayi ho)
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        //        ViewBag.Error = "Validation Failed: " + string.Join(", ", errors);
        //    }

        //    return View(model);
        //}

        public IActionResult Index()
        {   
            if(HttpContext.Session.GetString("UserRole") != "Accountant")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Login()
        {
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
                        HttpContext.Session.SetString("EmployeeID", user.EmployeeID);
                        HttpContext.Session.SetString("Username", user.Fullname);
                        HttpContext.Session.SetString("UserRole",user.Role);

                        //Redirect With role name 
                        return user.Role switch
                        {
                            "Admin" => RedirectToAction("Index", "Admin"),
                            "Manager" => RedirectToAction("Index", "Manager"),
                            "Accountant" => RedirectToAction("Index", "Accountant"),
                            _ => RedirectToAction("Dashboard", "Employee")
                        };
                    } 
                }
                ViewBag.Error = "Invalid Login";
                return View();
                
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login","Account");
        }
        
    }
}
