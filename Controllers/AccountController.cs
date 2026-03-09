using Azure.Core;
using BizTravel.Data;
using BizTravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices.WindowsRuntime;
namespace BizTravel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BizTravel.Models.EmailSender _emailSender;

        //to connect with database
        public AccountController(ApplicationDbContext context,BizTravel.Models.EmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            //finding the email in database
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null) 
            {
                //6-digit otp generation
                string otp = new Random().Next(100000,999999).ToString();
                user.ResetOTP = otp;

                //current Time Saving
                user.OTPGenratedTime = DateTime.Now;

                _context.SaveChanges();

                string subject = "Your Otp For Password Reset";
                string message = $"<h3>Hello,</h3><p>Your Otp For To Reset Password Is:<b>{otp}</b></p>";
                                 
                await _emailSender.SendEmailAsync(email, subject, message);

                //redirect to the otp sender page
                return RedirectToAction("VerifyOtp", new { email = email });
                //ViewBag.Message = "Password Has Been Sent To Your Email Address";
            }
                ViewBag.Error = "Eamil Address Not Found";
                return View();
        }

        [HttpGet]
        public IActionResult VerifyOTP(string email)
        {
            ViewBag.Email = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string email, string otp, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            //Genration Time From The Session
            var generatedAtStr = HttpContext.Session.GetString("OTPGeneratedAt");

            if (string.IsNullOrEmpty(generatedAtStr))
            {
                ViewBag.Error = "Session Expired. Please Request a New Otp.";
                return View();
            }
            
            DateTime generatedAt = DateTime.Parse(generatedAtStr);
            
            if(DateTime.Now > generatedAt.AddMinutes(2))
            {
                ViewBag.Error = "OTP Expired! Please Try Again.";
                ViewBag.Email = email;
                return View();
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
            {
                ViewBag.Error = "Password must be 8 characters long";
                ViewBag.Email = email;
                return View();
            }

            if (user != null)
            {
                if(user.ResetOTP != otp)
                {
                    ViewBag.Error = "Wrong Otp! Please Check Your Email And Try Again";
                    ViewBag.Email = email;
                    return View();
                }

                user.Password = newPassword; //update password
                user.ResetOTP = null; //clear otp for the safety
                _context.SaveChanges();

                return RedirectToAction("Login", new { msg = "Password Reset Successfully" });
            }
            ViewBag.Error = "User not found";
            return View();
        }
    }
}
