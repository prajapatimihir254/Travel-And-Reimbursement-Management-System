using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BizTravel.Models
{
    public class PaymentDetails
    {
        [Key]
        public int PaymentId { get; set; }
        
        public int RequestId { get; set; }

        [Required]
        public string PaymentMode { get; set; }

        [Required]
        public string TransactionId { get; set; }
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [ForeignKey("RequestId")]
        public virtual TravelRequest TravelRequest { get; set; }
    }
}
