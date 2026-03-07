using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models;

public class Admin
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Passkey { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Role { get; set; } = "Admin";
}
