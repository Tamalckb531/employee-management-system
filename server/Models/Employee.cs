using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models;

public class Employee
{
    public int Id { get; set; }

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\d{10}$|^\d{17}$", ErrorMessage = "NID must be 10 or 17 digits")]
    public string NID { get; set; } = string.Empty;

    [Required, RegularExpression(@"^(\+880|0)1[3-9]\d{8}$", ErrorMessage = "Invalid Bangladesh phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public decimal BasicSalary { get; set; }

    // Navigation properties
    public Spouse? Spouse { get; set; }
    public List<Child> Children { get; set; } = new();
}
