using BizTravel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BizTravel.Controllers
{
    public class AccountantController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountantController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //check the user is logged in or user is Accountant or not
            if(HttpContext.Session.GetString("UserRole") != "Accountant")
            {
                return RedirectToAction("Login", "Account");
            }
            //for showing the arrpove request and settlement
            var approveRequest = _context.TravelRequest.Where(r => r.Status == "Approved").ToList();
            return View(approveRequest);
        }
        [HttpPost]
        public IActionResult SettleClaim(int requestId)
        {
            var request = _context.TravelRequest.Find(requestId);
            if (request != null)
            {
                request.Status = "Settled"; //when the payment is settled
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
