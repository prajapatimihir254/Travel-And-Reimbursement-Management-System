using Microsoft.AspNetCore.Mvc;
using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ClosedXML.Excel;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Charts;

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
            var activeCount = _context.Users.Count(u => u.Role == "Employee" && u.IsActive == true);
            var inactiveCount = _context.Users.Count(u => u.Role == "Employee" && u.IsActive == false);


            //send data to the view
            ViewBag.ActiveEmployee = activeCount;
            ViewBag.InactiveEmployee = inactiveCount;

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

        [HttpPost]
        public IActionResult ToggleUserStatus(int userId)
        {
            var user = _context.Users.Find(userId);
            if(user != null)
            {
                user.IsActive = !user.IsActive;    
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
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
        public IActionResult DownloadExcelReport()
        {
            //fetch data from database 
            var data = _context.TravelRequest.ToList();

            //create new workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Travel Reports");
                var currentRow = 1;

                //set headers for the worksheet
                worksheet.Cell(currentRow, 1).Value = "Request ID";
                worksheet.Cell(currentRow, 2).Value = "Employee Email";
                worksheet.Cell(currentRow, 3).Value = "Destination";
                worksheet.Cell(currentRow, 4).Value = "Purpose";
                worksheet.Cell(currentRow, 5).Value = "Amount";
                worksheet.Cell(currentRow, 6).Value = "Status";

                //Header style
                var headerrange = worksheet.Range("A1:F1");
                headerrange.Style.Font.Bold = true;
                headerrange.Style.Fill.BackgroundColor = XLColor.BabyBlue;

                //fill data rows
                foreach (var item in data) 
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.RequestId;
                    worksheet.Cell(currentRow, 2).Value = item.EmployeeEmail;
                    worksheet.Cell(currentRow, 3).Value = item.City + " " + item.State;
                    worksheet.Cell(currentRow, 4).Value = item.Purpose;
                    worksheet.Cell(currentRow, 5).Value = item.EstimatedAmount;
                    worksheet.Cell(currentRow, 6).Value = item.Status;
                }
                //column auto-adjust 
                worksheet.Columns().AdjustToContents();

                //file download(stream)
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "TravelReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                }
            }

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
