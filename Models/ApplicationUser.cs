    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizTravel.Models
{
    public class ApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //for auto-increment the empid
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name Is Required")]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "Name should be between 3 or 100 characters")]
        public string Fullname { get; set; } = "";

        public string? EmployeeID { get; set; }
        
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Department Is mandatory")]
        public string? Department { get; set; }

        //for the strong password 
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must have 1 Uppercase,1 Lowercase,1 Number,1 Special Character")]
        public string Password { get; set; } = "";


        [Required(ErrorMessage = "Role Is Required")]
        public string? Role { get; set; } //Admin,Employee,Manager,Accountant
    }
}
