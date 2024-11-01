using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter Name Property")]
        public required string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary Must Be Positive Value")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Enter Location Property")]
        public required string Location { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Please Enter in Email Format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Enter Department Property")]
        public required string Department { get; set; }

        [Required(ErrorMessage = "Enter Qualification Property")]
        public required string Qualification { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
