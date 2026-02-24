namespace BizTravel.Models
{
    public class AdminDashboardVM
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int SettledRequests { get; set; }
        public int RejectedRequests { get; set; }
        public decimal TotalSettledAmount { get; set; }

        //for the showing userlist 
        public List<ApplicationUser> UserList { get; set; }
        public List<MonthlyExpenseVM> MonthlyExpenses { get; set; }
    }
    public class MonthlyExpenseVM
    {
        public string MonthName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
