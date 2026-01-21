using Microsoft.AspNetCore.Mvc;
using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Identity;

namespace BizTravel.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            //check the user is logged in or user is admin or not
            //view all user at dashboard
            //if (HttpContext.Session.GetString("UserRole") != "Admin")
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //var allUsers = _context.Users.ToList(); //List of all user
            //return View(allUsers);
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")) || 
                HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login","Account");
            }
            // not save the page at browzer
            Response.Headers["Cache-Control"] = "no-cache,no-store,must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            var allUsers = _context.Users.ToList();
            return View(allUsers);
        }

        //creating the new users(GET)
        public IActionResult CreateUser()
        {
            return View();
        }

        //save the new user(GET)
        [HttpPost]
        public IActionResult CreateUser(ApplicationUser newUser)
        {
            //check that filed should not be empty
            if (string.IsNullOrEmpty(newUser.Role)) newUser.Role = "Employee";

            // ID generate karna taaki SQL mein NULL na jaye
            int count = _context.Users.Count() + 1;
            newUser.EmployeeID = "EMP" + count.ToString("D3");

            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                newUser.Password = passwordHasher.HashPassword(newUser, newUser.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            foreach (var modelstate in ModelState.Values)
            {
                foreach (var error in modelstate.Errors)
                {
                    System.Diagnostics.Debug.WriteLine("Validation Error:" + error.ErrorMessage);
                }
            }
            return View(newUser);
        }

        public IActionResult Logout()
        {
            //for clean the session
            HttpContext.Session.Clear();

            foreach(var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Index,Home");
        }
    }
}
