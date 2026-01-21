using BizTravel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.ObjectModelRemoting;

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
            var pendingRequests = _context.TravelRequest.Where(r => r.Status == "Pending").ToList();
            return View(pendingRequests);
        }
        [HttpPost]
        public IActionResult UpdateStatus(int requestId,string newStatus)
        {
            var request = _context.TravelRequest.FirstOrDefault(r => r.RequestId == requestId);
            if (request != null)
            {
                request.Status = newStatus; //status for aprove or reject the request
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
