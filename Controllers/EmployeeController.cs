using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BizTravel.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        //to conncet with database
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole")!= "Employee")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Dashboard()
        {
            //data fetch from session
            var empId = HttpContext.Session.GetString("EmployeeID");
            var empName = HttpContext.Session.GetString("Username");

            //using viewbeg send data to the view
            ViewBag.EmpID = empId;
            ViewBag.FullName = empName;

            //database logic
            var claims = _context.TravelRequest
                        .Where(x => x.EmployeeEmail == HttpContext.Session.GetString("UserEmail"))
                        .ToList();
            return View(claims);

            //string email = HttpContext.Session.GetString("UserEmail");
            ////get the employee request
            //var myRequests = _context.TravelRequest.Where(r => r.EmployeeEmail == email).ToList();
            //return View(myRequests);
        }

        //Get: Showing The Request Form
        public IActionResult RaiseClaim()
        {
            return View();
        }
        
        //Post: For Saving The Data
        [HttpPost]
        public IActionResult RaiseClaim(TravelRequest request)
        {
            //session: taking email from the login
            request.EmployeeEmail = HttpContext.Session.GetString("UserEmail");
            request.Status = "Pending";

            if(ModelState.IsValid)
            {
                _context.TravelRequest.Add(request);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(request);
        }
    }
}
