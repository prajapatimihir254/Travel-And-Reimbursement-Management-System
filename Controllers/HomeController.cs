using System.Diagnostics;
using BizTravel.Models;
using Microsoft.AspNetCore.Mvc;

namespace BizTravel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //check the user is looged in or not ?
            var role = HttpContext.Session.GetString("UserRole");

            if (role == "Employee")
            {
                // Note: Employee ka action 'Dashboard' hai (jo humne pehle set kiya tha)
                return RedirectToAction("Dashboard", "Employee");
            }
            else if (role == "Manager")
            {
                return RedirectToAction("Index", "Manager");
            }
            else if (role == "Accountant")
            {
                return RedirectToAction("Index", "Accountant");
            }
            else if (role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
