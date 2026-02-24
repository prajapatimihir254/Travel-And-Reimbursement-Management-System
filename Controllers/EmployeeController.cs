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
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            //data fetch from session
            var empId = HttpContext.Session.GetString("EmployeeID");
            var empName = HttpContext.Session.GetString("Username");

            //using viewbeg send data to the view
            ViewBag.EmpID = empId;
            ViewBag.FullName = empName;

            //database logic
            var claims = _context.TravelRequest
                        .Where(x => x.EmployeeEmail == userEmail && x.Status == "Pending")
                        .OrderByDescending(x => x.TravelDate)
                        .ToList();
            return View(claims);
        }
        public IActionResult TravelHistory()
        {
            //var empId = HttpContext.Session.GetString("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            //data fetch from session
            var empId = HttpContext.Session.GetString("EmployeeID");
            var empName = HttpContext.Session.GetString("Username");

            // Isme saari requests aayengi (Pending, Approved, Settled, Rejected)
            var allRequests = _context.TravelRequest
                                      .Where(x => x.EmployeeEmail == userEmail)
                                      .OrderByDescending(x => x.RequestId)
                                      .ToList();
            return View(allRequests);
        }

        //Get: Showing The Request Form
        public IActionResult RaiseClaim()
        {
            return View();
        }
        
        //Post: For Saving The Data
        [HttpPost]
        public async Task<IActionResult> RaiseClaim(TravelRequest request,IFormFile billfile)
        {
            //check the folder (wwwrote/uploads)
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            //Unique file name 
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + billfile.FileName;
            string filePath = Path.Combine(uploadFolder,uniqueFileName);

            //save the file
            using (var fileStrem = new FileStream(filePath, FileMode.Create))
            {
                await billfile.CopyToAsync(fileStrem);
            }
            //save file to database
            request.BillFilePath = "/uploads/" + uniqueFileName;

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
