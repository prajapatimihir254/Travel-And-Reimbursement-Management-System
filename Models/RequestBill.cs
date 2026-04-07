using System.ComponentModel.DataAnnotations;
namespace BizTravel.Models
{
    public class RequestBill
    {
        [Key] 
        public int BillId { get; set; }

        public int RequestId { get; set; }
        public string FilePath { get; set; }
        public decimal ScannedAmount { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
