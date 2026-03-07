using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.DTOs;

public class AdminLoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Passkey { get; set; } = string.Empty;
}

public class AdminLoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
