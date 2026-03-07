using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models;

public class Child
{
    public int Id { get; set; }

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    // Foreign key
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}
