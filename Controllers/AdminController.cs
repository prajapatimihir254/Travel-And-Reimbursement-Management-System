using Microsoft.AspNetCore.Mvc;
using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            //4 cards at dashboard
            var stats = new AdminDashboardVM();

            //Total claim count and status wise count
            stats.TotalRequests = _context.TravelRequest.Count();
            stats.PendingRequests = _context.TravelRequest.Count(x => x.Status == "Pending");
            stats.SettledRequests = _context.TravelRequest.Count(x => x.Status == "Settled");
            stats.RejectedRequests = _context.TravelRequest.Count(x => x.Status == "Rejected");

            //total amount spend(settle claim's Total)
            stats.TotalSettledAmount = _context.TravelRequest
                                   .Where(x => x.Status == "Settled")
                                   .Sum(x => (decimal?)x.FinalAmount) ?? 0m;

            //Monthly Expense Logic
            stats.MonthlyExpenses = _context.TravelRequest
                .Where(x => x.Status == "Settled")
                .GroupBy(x => new {x.TravelDate.Month, x.TravelDate.Year })
                .Select(g => new MonthlyExpenseVM
                {
                    //for fetch the month name
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    TotalAmount = g.Sum(x => x.FinalAmount)
                })
                .ToList();
            
            stats.UserList = _context.Users.ToList(); 
            
            
            //admin check
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")) ||
                HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            // not save the page at browzer
            Response.Headers["Cache-Control"] = "no-cache,no-store,must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            var allUsers = _context.Users.ToList();
            return View(stats);
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

        public IActionResult DownloadReport()
        {
            //4 cards at dashboard
            var stats = new AdminDashboardVM();

            //Total claim count and status wise count
            stats.TotalRequests = _context.TravelRequest.Count();
            stats.PendingRequests = _context.TravelRequest.Count(x => x.Status == "Pending");
            stats.SettledRequests = _context.TravelRequest.Count(x => x.Status == "Settled");
            stats.RejectedRequests = _context.TravelRequest.Count(x => x.Status == "Rejected");

            //total amount spend(settle claim's Total)
            stats.TotalSettledAmount = _context.TravelRequest
                                   .Where(x => x.Status == "Settled")
                                   .Sum(x => (decimal?)x.FinalAmount) ?? 0m;
            stats.UserList = _context.Users.ToList();

            //pdf view return
            return new ViewAsPdf("Report", stats)
            {
                FileName = "Travel_Report.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation  = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--print-media-type --no-background",
            };
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
