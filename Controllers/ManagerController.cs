using BizTravel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BizTravel.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        //connect with database 
        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //check the user is logged in or user is manager or not
            if(HttpContext.Session.GetString("UserRole") != "Manager")
            {
                return RedirectToAction("Login","Account");
            }
            //show all request
            //var allRequests = _context.TravelRequest.OrderByDescending(x => x.RequestId).ToList();
            //return View(allRequests);

            //show only pending request
            var PendingRequests = _context.TravelRequest.Where(r => r.Status == "Pending").ToList();
            return View(PendingRequests);
        }
        [HttpPost]
        public IActionResult UpdateStatus(int requestId,string newStatus,string reason)
        {
            var request = _context.TravelRequest.Find(requestId);
            if (request != null)
            {
                //request.Status = newStatus; //status for aprove or reject the request
                if(newStatus == "Approved")
                {
                    request.Status = "Approved";
                    //request.RejectionReason = reason;
                }
                else if (newStatus == "Rejected")
                {
                    request.Status = "Rejected";
                    request.RejectionReason = reason;
                }
                    _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Approve(int requestId)
        {
            var request = _context.TravelRequest.Find();
            if (request != null)
            {
                request.Status = "Approved";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult TravelHistory()
        {
            var allHistory = _context.TravelRequest.OrderByDescending(r => r.RequestId).ToList();
            return View(allHistory);
        }
        //public IActionResult Reject(int requestId,string reason)
        //{
        //    var request = _context.TravelRequest.Find(requestId);
        //    if (request != null) 
        //    {
        //        request.Status = "Rejected";
        //        request.RejectionReason = reason; //save the comments
        //        _context.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}
