using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace BizTravel.Models
{
    public class TravelRequest
    {
        [Key]
        public int RequestId { get; set; }
        public string? EmployeeEmail { get; set; }

        [Required(ErrorMessage = "Travel Date Is Required")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }

        [Required(ErrorMessage = "Purpose Is Required")]
        public string? Purpose { get; set; }

        [Required(ErrorMessage = "Please Enter The Estimated Amount")]
        [Range(100,10000000,ErrorMessage = "Amount Must Be Between 100 and 10,00,000")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal EstimatedAmount { get; set; }
        public string Status { get; set; } = "Pending";


        [Required(ErrorMessage = "Please Select The Country")]
        public string Country { get; set; } = "India";

        [Required(ErrorMessage = "Please Select The State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Select The City")]
        public string City { get; set; }
    }
}
