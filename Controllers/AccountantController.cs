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
            //var allRequest = _context.TravelRequest.OrderByDescending(x => x.RequestId).ToList();
            //return View(allRequest);

            //show only the approved request
            var ApprovedRequests = _context.TravelRequest.Where(r => r.Status == "Approved").ToList();
            return View(ApprovedRequests);
        }
        [HttpPost]
        public IActionResult SettleClaim(int requestId,decimal finalAmount)
        {
            var request = _context.TravelRequest.Find(requestId);
            if (request != null)
            {
                request.Status = "Settled"; //when the payment is settled
                request.FinalAmount = finalAmount; //save final amount
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateBillAmount(int id,decimal finalAmount)
        {
            var claim = _context.TravelRequest.Find(id);
            if(claim != null)
            {
                claim.EstimatedAmount = finalAmount; // amount update
                claim.Status = "Settled";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SettleRequest(int requestId)
        {
            var request = _context.TravelRequest.Find(requestId);
            if(request != null)
            {
                //set the manager's approval request
                request.Status = "Settled";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult TravelHistory()
        {
            var allHistory = _context.TravelRequest.OrderByDescending(r => r.RequestId).ToList();
            return View(allHistory);
        }
    }
}
