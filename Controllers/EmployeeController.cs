using BizTravel.Data;
using BizTravel.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
        public async Task<IActionResult> RaiseClaim(TravelRequest request, List<IFormFile> billfiles)
        {
            // Session se email aur default status set karein
            request.EmployeeEmail = HttpContext.Session.GetString("UserEmail");
            request.Status = "Pending";

            if (billfiles != null && billfiles.Count > 0)
            {
                decimal totalScannedAmount = 0;
                List<string> savedFilePaths = new List<string>(); // Saare paths yahan jama honge

                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                foreach (var file in billfiles)
                {
                    // Unique file name generate karein
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // File ko physically save karein
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Database ke liye path list mein add karein
                    savedFilePaths.Add("/uploads/" + uniqueFileName);

                    // --- OCR Scanning Start ---
                    decimal scannedAmount = 0;
                    try
                    {
                        using (var engine = new Tesseract.TesseractEngine(@"./tessdata", "eng", Tesseract.EngineMode.Default))
                        {
                            using (var img = Tesseract.Pix.LoadFromFile(filePath))
                            {
                                using (var page = engine.Process(img))
                                {
                                    string text = page.GetText();
                                    System.Diagnostics.Debug.WriteLine("OCR Extracted Text: " + text);
                                    scannedAmount = ExtractAmountFromText(text);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("OCR Error: " + ex.Message);
                        scannedAmount = 0;
                    }
                    // --- OCR End ---

                    totalScannedAmount += scannedAmount;
                }

                // --- MAIN FIXES START HERE ---

                // 1. Saare paths ko semicolon (;) se join karke ek single string banayein
                request.BillFilePath = string.Join(";", savedFilePaths);

                // 2. Final total amount set karein
                request.EstimatedAmount = totalScannedAmount;

                // 3. Ek hi baar mein poora data TravelRequest table mein save karein
                _context.TravelRequest.Add(request);
                await _context.SaveChangesAsync();

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Please upload at least one bill.";
            return View(request);
        }

        private decimal ExtractAmountFromText(string text)
        {
            List<decimal> allFoundAmounts = new List<decimal>();
            try
            {
                var Lines = text.Split('\n');
                foreach (var line in Lines)
                {
                    //remove symbols
                    string cleanLine = line.Replace("₹", "").Replace(",", "").Replace(":", " ").Trim();
                    var words = cleanLine.Split(' ');

                    foreach (var word in words)
                    {
                        if (decimal.TryParse(word, out decimal result))
                        {
                            allFoundAmounts.Add(result);
                        }
                    }
                }
                //find biggest amount
                if(allFoundAmounts.Count > 0)
                {
                    return allFoundAmounts.Max();
                }
            }

            catch { }
            return 0;
        }
    }
}
