using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Tests;

public class AdminLoginTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AdminService _service;

    private const string JwtKey = "TestSecretKeyThatIsAtLeast32CharactersLong!!";
    private const string JwtIssuer = "TestIssuer";
    private const string JwtAudience = "TestAudience";

    public AdminLoginTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience
            })
            .Build();

        _service = new AdminService(_context, configuration);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private async Task SeedAdmin(string username = "Admin", string passkey = "123456", string role = "Admin")
    {
        _context.Admins.Add(new Admin
        {
            Username = username,
            Passkey = HashPassword(passkey),
            Role = role
        });
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "Admin",
            Passkey = "123456"
        });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));
        Assert.Equal(3600, result.ExpiresIn);
    }

    [Fact]
    public async Task Login_InvalidUsername_ReturnsNull()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "WrongUser",
            Passkey = "123456"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_InvalidPasskey_ReturnsNull()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "Admin",
            Passkey = "wrongpassword"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_EmptyUsername_ReturnsNull()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "",
            Passkey = "123456"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_EmptyPasskey_ReturnsNull()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "Admin",
            Passkey = ""
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_TokenContainsExpectedClaims()
    {
        await SeedAdmin();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "Admin",
            Passkey = "123456"
        });

        Assert.NotNull(result);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result!.Token);

        Assert.Equal("1", token.Claims.First(c => c.Type == "AdminId").Value);
        Assert.Equal("Admin", token.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal("Admin", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal(JwtIssuer, token.Issuer);
        Assert.Contains(JwtAudience, token.Audiences);
    }

    [Fact]
    public async Task Login_PasswordHashingMatchesSeedDataLogic()
    {
        var password = "123456";
        var expectedHash = HashPassword(password);

        _context.Admins.Add(new Admin
        {
            Username = "Admin",
            Passkey = expectedHash,
            Role = "Admin"
        });
        await _context.SaveChangesAsync();

        var result = await _service.LoginAsync(new AdminLoginRequestDto
        {
            Username = "Admin",
            Passkey = password
        });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result!.Token));
    }
}
