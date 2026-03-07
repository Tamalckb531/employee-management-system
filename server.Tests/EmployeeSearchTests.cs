using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests;

public class EmployeeSearchTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeSearchTests()
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

    private Employee CreateEmployee(
        int index,
        string? name = null,
        string? nid = null,
        string? department = null,
        bool withSpouse = true,
        int childCount = 0)
    {
        var employee = new Employee
        {
            Image = $"https://randomuser.me/api/portraits/men/{index}.jpg",
            Gender = "Male",
            Name = name ?? $"Employee {index}",
            NID = nid ?? $"{index:D10}",
            Phone = $"+88017{index:D8}",
            Department = department ?? "Engineering",
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

    // --- Search by Name ---

    [Fact]
    public async Task Search_ByName_ReturnsMatchingEmployees()
    {
        _context.Employees.AddRange(
            CreateEmployee(1, name: "Hasan Mahmud"),
            CreateEmployee(2, name: "Tanvir Ahmed"),
            CreateEmployee(3, name: "Hasan Khan"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, 50);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Contains("Hasan", e.Name));
    }

    // --- Search by NID ---

    [Fact]
    public async Task Search_ByNID_ReturnsMatchingEmployees()
    {
        _context.Employees.AddRange(
            CreateEmployee(1, nid: "1234567890"),
            CreateEmployee(2, nid: "9876543210"),
            CreateEmployee(3, nid: "1112345678"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("12345", 0, 50);

        Assert.Equal(2, result.Count);
    }

    // --- Search by Department ---

    [Fact]
    public async Task Search_ByDepartment_ReturnsMatchingEmployees()
    {
        _context.Employees.AddRange(
            CreateEmployee(1, department: "Engineering"),
            CreateEmployee(2, department: "Finance"),
            CreateEmployee(3, department: "Engineering"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Engineering", 0, 50);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("Engineering", e.Department));
    }

    // --- Case-insensitive search ---

    [Fact]
    public async Task Search_IsCaseInsensitive()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud", department: "Engineering"));
        await _context.SaveChangesAsync();

        var lowerResult = await _service.SearchEmployeesAsync("hasan", 0, 50);
        var upperResult = await _service.SearchEmployeesAsync("HASAN", 0, 50);
        var mixedResult = await _service.SearchEmployeesAsync("hAsAn", 0, 50);

        Assert.Single(lowerResult);
        Assert.Single(upperResult);
        Assert.Single(mixedResult);
    }

    // --- Infinite scroll (offset + limit) ---

    [Fact]
    public async Task Search_InfiniteScroll_FirstBatch()
    {
        for (int i = 1; i <= 120; i++)
            _context.Employees.Add(CreateEmployee(i, department: "Engineering"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Engineering", 0, 50);

        Assert.Equal(50, result.Count);
    }

    [Fact]
    public async Task Search_InfiniteScroll_SecondBatch()
    {
        for (int i = 1; i <= 120; i++)
            _context.Employees.Add(CreateEmployee(i, department: "Engineering"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Engineering", 50, 50);

        Assert.Equal(50, result.Count);
    }

    [Fact]
    public async Task Search_InfiniteScroll_FinalBatch()
    {
        for (int i = 1; i <= 120; i++)
            _context.Employees.Add(CreateEmployee(i, department: "Engineering"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Engineering", 100, 50);

        Assert.Equal(20, result.Count);
    }

    [Fact]
    public async Task Search_InfiniteScroll_ResultsOrderedById()
    {
        for (int i = 1; i <= 10; i++)
            _context.Employees.Add(CreateEmployee(i, department: "Engineering"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Engineering", 0, 50);

        for (int i = 1; i < result.Count; i++)
            Assert.True(result[i].Id > result[i - 1].Id);
    }

    // --- Empty search query ---

    [Fact]
    public async Task Search_EmptyQuery_ReturnsEmptyArray()
    {
        _context.Employees.Add(CreateEmployee(1));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("", 0, 50);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Search_WhitespaceQuery_ReturnsEmptyArray()
    {
        _context.Employees.Add(CreateEmployee(1));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("   ", 0, 50);

        Assert.Empty(result);
    }

    // --- No matching results ---

    [Fact]
    public async Task Search_NoMatch_ReturnsEmptyArray()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("NonExistent", 0, 50);

        Assert.Empty(result);
    }

    // --- Employee with no spouse ---

    [Fact]
    public async Task Search_EmployeeWithNoSpouse_ReturnsNullSpouse()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud", withSpouse: false));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, 50);

        Assert.Single(result);
        Assert.Null(result[0].Spouse);
    }

    // --- Employee with no children ---

    [Fact]
    public async Task Search_EmployeeWithNoChildren_ReturnsEmptyChildrenArray()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud", childCount: 0));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, 50);

        Assert.Single(result);
        Assert.Empty(result[0].Children);
    }

    [Fact]
    public async Task Search_EmployeeWithChildren_ReturnsChildren()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud", childCount: 3));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, 50);

        Assert.Single(result);
        Assert.Equal(3, result[0].Children.Count);
    }

    // --- Invalid offset/limit ---

    [Fact]
    public async Task Search_NegativeOffset_ClampsToZero()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", -5, 50);

        Assert.Single(result);
    }

    [Fact]
    public async Task Search_NegativeLimit_ClampsToDefault()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, -10);

        Assert.Single(result);
    }

    [Fact]
    public async Task Search_OffsetBeyondResults_ReturnsEmptyArray()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud"));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 100, 50);

        Assert.Empty(result);
    }

    // --- DTO mapping ---

    [Fact]
    public async Task Search_ReturnsCorrectDtoFields()
    {
        _context.Employees.Add(CreateEmployee(1, name: "Hasan Mahmud", department: "Engineering", withSpouse: true, childCount: 1));
        await _context.SaveChangesAsync();

        var result = await _service.SearchEmployeesAsync("Hasan", 0, 50);

        Assert.Single(result);
        var dto = result[0];
        Assert.Equal("Hasan Mahmud", dto.Name);
        Assert.Contains("portraits/men/1", dto.Image);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("0000000001", dto.NID);
        Assert.Equal("+8801700000001", dto.Phone);
        Assert.Equal("Engineering", dto.Department);
        Assert.Equal(41000m, dto.BasicSalary);
        Assert.NotNull(dto.Spouse);
        Assert.Equal("Spouse 1", dto.Spouse!.Name);
        Assert.Single(dto.Children);
        Assert.Equal("Child 1_0", dto.Children[0].Name);
    }
}
