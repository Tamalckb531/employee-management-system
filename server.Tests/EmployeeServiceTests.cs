using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests;

public class EmployeeServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new EmployeeService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private Employee CreateEmployee(int index, bool withSpouse = true, int childCount = 0)
    {
        var employee = new Employee
        {
            Image = $"https://randomuser.me/api/portraits/men/{index}.jpg",
            Gender = "Male",
            Name = $"Employee {index}",
            NID = $"{index:D10}",
            Phone = $"+88017{index:D8}",
            Department = "Engineering",
            BasicSalary = 40000m + index * 1000
        };

        if (withSpouse)
        {
            employee.Spouse = new Spouse
            {
                Image = $"https://randomuser.me/api/portraits/women/{index}.jpg",
                Gender = "Female",
                Name = $"Spouse {index}",
                NID = $"{(index + 1000):D10}"
            };
        }

        for (int i = 0; i < childCount; i++)
        {
            employee.Children.Add(new Child
            {
                Image = $"https://api.dicebear.com/7.x/adventurer/svg?seed=Child{index}_{i}",
                Gender = i % 2 == 0 ? "Male" : "Female",
                Name = $"Child {index}_{i}",
                DateOfBirth = DateTime.SpecifyKind(new DateTime(2015 + i, 1, 1), DateTimeKind.Utc)
            });
        }

        return employee;
    }

    private async Task SeedEmployees(int count, bool withSpouse = true, int childCount = 0)
    {
        for (int i = 1; i <= count; i++)
        {
            _context.Employees.Add(CreateEmployee(i, withSpouse, childCount));
        }
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetEmployees_ReturnsDefault50_WhenMoreExist()
    {
        await SeedEmployees(60);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Equal(50, result.Data.Count);
        Assert.Equal(60, result.TotalCount);
        Assert.True(result.HasMore);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task GetEmployees_ReturnsAllWhenLessThanPageSize()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Equal(5, result.Data.Count);
        Assert.Equal(5, result.TotalCount);
        Assert.False(result.HasMore);
    }

    [Fact]
    public async Task GetEmployees_Pagination_SecondPageReturnsRemaining()
    {
        await SeedEmployees(60);

        var result = await _service.GetEmployeesAsync(2, 50);

        Assert.Equal(10, result.Data.Count);
        Assert.Equal(60, result.TotalCount);
        Assert.False(result.HasMore);
        Assert.Equal(2, result.Page);
    }

    [Fact]
    public async Task GetEmployees_Pagination_BeyondLastPage_ReturnsEmpty()
    {
        await SeedEmployees(10);

        var result = await _service.GetEmployeesAsync(5, 50);

        Assert.Empty(result.Data);
        Assert.Equal(10, result.TotalCount);
        Assert.False(result.HasMore);
    }

    [Fact]
    public async Task GetEmployees_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
        Assert.False(result.HasMore);
    }

    [Fact]
    public async Task GetEmployees_InvalidPage_ClampsToOne()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(-1, 50);

        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetEmployees_ZeroPage_ClampsToOne()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(0, 50);

        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetEmployees_InvalidPageSize_ClampsToDefault()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(1, -10);

        Assert.Equal(50, result.PageSize);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetEmployees_ExcessivePageSize_ClampsToMax()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(1, 500);

        Assert.Equal(100, result.PageSize);
    }

    [Fact]
    public async Task GetEmployees_IncludesSpouseData()
    {
        await SeedEmployees(1, withSpouse: true);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Single(result.Data);
        Assert.NotNull(result.Data[0].Spouse);
        Assert.Equal("Spouse 1", result.Data[0].Spouse!.Name);
    }

    [Fact]
    public async Task GetEmployees_NoSpouse_ReturnsNull()
    {
        await SeedEmployees(1, withSpouse: false);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Single(result.Data);
        Assert.Null(result.Data[0].Spouse);
    }

    [Fact]
    public async Task GetEmployees_IncludesChildrenData()
    {
        await SeedEmployees(1, withSpouse: true, childCount: 3);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Single(result.Data);
        Assert.Equal(3, result.Data[0].Children.Count);
        Assert.Equal("Child 1_0", result.Data[0].Children[0].Name);
    }

    [Fact]
    public async Task GetEmployees_NoChildren_ReturnsEmptyList()
    {
        await SeedEmployees(1, withSpouse: true, childCount: 0);

        var result = await _service.GetEmployeesAsync(1, 50);

        Assert.Single(result.Data);
        Assert.Empty(result.Data[0].Children);
    }

    [Fact]
    public async Task GetEmployees_ReturnsCorrectDtoFields()
    {
        await SeedEmployees(1, withSpouse: true, childCount: 1);

        var result = await _service.GetEmployeesAsync(1, 50);
        var dto = result.Data[0];

        Assert.Equal("Employee 1", dto.Name);
        Assert.Contains("portraits/men/1", dto.Image);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("0000000001", dto.NID);
        Assert.Equal("+8801700000001", dto.Phone);
        Assert.Equal("Engineering", dto.Department);
        Assert.Equal(41000m, dto.BasicSalary);
    }

    [Fact]
    public async Task GetEmployees_OrderedById()
    {
        await SeedEmployees(5);

        var result = await _service.GetEmployeesAsync(1, 50);

        for (int i = 1; i < result.Data.Count; i++)
        {
            Assert.True(result.Data[i].Id > result.Data[i - 1].Id);
        }
    }
}
