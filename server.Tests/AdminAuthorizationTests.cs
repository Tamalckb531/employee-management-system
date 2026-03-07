using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Tests;

public class AdminAuthorizationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    private const string JwtKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
    private const string JwtIssuer = "EmployeeManagement";
    private const string JwtAudience = "EmployeeManagementClient";

    public AdminAuthorizationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("AuthTestDb_" + Guid.NewGuid()));
                });
            });
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    private static string GenerateToken(string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("AdminId", "1"),
            new Claim(ClaimTypes.Name, "Admin"),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static CreateEmployeeDto SampleDto() => new()
    {
        Name = "Auth Test",
        Image = "https://example.com/img.jpg",
        Gender = "Male",
        Phone = "+8801712340000",
        NID = "9999999999",
        Department = "Engineering",
        BasicSalary = 50000
    };

    [Fact]
    public async Task CreateEmployee_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/employees", SampleDto());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_InvalidToken_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "invalid.token.value");

        var response = await client.PostAsJsonAsync("/api/employees", SampleDto());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_NonAdminRole_Returns403()
    {
        var client = _factory.CreateClient();
        var token = GenerateToken("User");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/employees", SampleDto());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
