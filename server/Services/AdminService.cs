using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Services;

public interface IAdminService
{
    Task<AdminLoginResponseDto?> LoginAsync(AdminLoginRequestDto request);
}

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AdminLoginResponseDto?> LoginAsync(AdminLoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Passkey))
            return null;

        var hashedPasskey = HashPassword(request.Passkey);

        var admin = await _context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Username == request.Username && a.Passkey == hashedPasskey);

        if (admin == null)
            return null;

        var token = GenerateJwtToken(admin.Id, admin.Username, admin.Role);

        return new AdminLoginResponseDto
        {
            Token = token,
            ExpiresIn = 3600
        };
    }

    private string GenerateJwtToken(int adminId, string username, string role)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("AdminId", adminId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(172800),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
