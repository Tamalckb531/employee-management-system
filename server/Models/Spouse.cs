using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models;

public class Spouse
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

    // Foreign key
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}
