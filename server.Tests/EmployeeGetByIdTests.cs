using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests;

public class EmployeeGetByIdTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeGetByIdTests()
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

    [Fact]
    public async Task GetById_ExistingEmployee_ReturnsFullDetails()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: true, childCount: 2));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Employee 1", result!.Name);
        Assert.NotNull(result.Spouse);
        Assert.Equal(2, result.Children.Count);
    }

    [Fact]
    public async Task GetById_WithSpouse_ReturnsPopulatedSpouseDto()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: true));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.NotNull(result!.Spouse);
        Assert.Equal("Spouse 1", result.Spouse!.Name);
        Assert.Contains("portraits/women/1", result.Spouse.Image);
        Assert.Equal("Female", result.Spouse.Gender);
        Assert.Equal("0000001001", result.Spouse.NID);
    }

    [Fact]
    public async Task GetById_WithoutSpouse_ReturnsNull()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: false));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Null(result!.Spouse);
    }

    [Fact]
    public async Task GetById_WithChildren_ReturnsPopulatedChildDtoList()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: true, childCount: 3));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Children.Count);
        Assert.Equal("Child 1_0", result.Children[0].Name);
        Assert.Equal("Male", result.Children[0].Gender);
        Assert.Equal(DateTime.SpecifyKind(new DateTime(2015, 1, 1), DateTimeKind.Utc), result.Children[0].DateOfBirth);
        Assert.Equal("Female", result.Children[1].Gender);
    }

    [Fact]
    public async Task GetById_WithoutChildren_ReturnsEmptyList()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: true, childCount: 0));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result!.Children);
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNull()
    {
        _context.Employees.Add(CreateEmployee(1));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_NegativeId_ReturnsNull()
    {
        _context.Employees.Add(CreateEmployee(1));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(-1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ZeroId_ReturnsNull()
    {
        _context.Employees.Add(CreateEmployee(1));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(0);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ReturnsCorrectDtoFields()
    {
        _context.Employees.Add(CreateEmployee(1, withSpouse: true, childCount: 1));
        await _context.SaveChangesAsync();

        var result = await _service.GetEmployeeByIdAsync(1);

        Assert.NotNull(result);
        var dto = result!;
        Assert.Equal(1, dto.Id);
        Assert.Equal("Employee 1", dto.Name);
        Assert.Contains("portraits/men/1", dto.Image);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("0000000001", dto.NID);
        Assert.Equal("+8801700000001", dto.Phone);
        Assert.Equal("Engineering", dto.Department);
        Assert.Equal(41000m, dto.BasicSalary);
        Assert.NotNull(dto.Spouse);
        Assert.Equal("Spouse 1", dto.Spouse!.Name);
        Assert.Contains("portraits/women/1", dto.Spouse.Image);
        Assert.Equal("Female", dto.Spouse.Gender);
        Assert.Equal("0000001001", dto.Spouse.NID);
        Assert.Single(dto.Children);
        Assert.Equal("Child 1_0", dto.Children[0].Name);
        Assert.Equal("Male", dto.Children[0].Gender);
        Assert.Equal(DateTime.SpecifyKind(new DateTime(2015, 1, 1), DateTimeKind.Utc), dto.Children[0].DateOfBirth);
    }
}
