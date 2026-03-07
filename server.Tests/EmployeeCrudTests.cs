using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Tests;

public class EmployeeCrudTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeService _service;

    public EmployeeCrudTests()
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

    private static CreateEmployeeDto SampleCreateDto(
        string nid = "1234567890",
        string phone = "+8801312345678",
        bool withSpouse = false,
        int childCount = 0)
    {
        var dto = new CreateEmployeeDto
        {
            Name = "Test Employee",
            Image = "https://example.com/img.jpg",
            Gender = "Male",
            Phone = phone,
            NID = nid,
            Department = "Engineering",
            BasicSalary = 50000m
        };

        if (withSpouse)
        {
            dto.Spouse = new CreateSpouseDto
            {
                Name = "Test Spouse",
                Image = "https://example.com/spouse.jpg",
                Gender = "Female",
                NID = "9876543210"
            };
        }

        if (childCount > 0)
        {
            dto.Children = new List<CreateChildDto>();
            for (int i = 0; i < childCount; i++)
            {
                dto.Children.Add(new CreateChildDto
                {
                    Name = $"Child {i}",
                    Image = $"https://example.com/child{i}.jpg",
                    Gender = i % 2 == 0 ? "Male" : "Female",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(2018 + i, 1, 1), DateTimeKind.Utc)
                });
            }
        }

        return dto;
    }

    // ─── CREATE ───

    [Fact]
    public async Task CreateEmployee_WithSpouseAndChildren_ReturnsFullDto()
    {
        var dto = SampleCreateDto(withSpouse: true, childCount: 2);

        var result = await _service.CreateEmployeeAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Test Employee", result.Name);
        Assert.Equal("Engineering", result.Department);
        Assert.Equal(50000m, result.BasicSalary);
        Assert.NotNull(result.Spouse);
        Assert.Equal("Test Spouse", result.Spouse!.Name);
        Assert.Equal("Female", result.Spouse.Gender);
        Assert.Equal("9876543210", result.Spouse.NID);
        Assert.Equal(2, result.Children.Count);
        Assert.Equal("Child 0", result.Children[0].Name);
        Assert.Equal("Male", result.Children[0].Gender);
        Assert.Equal("Child 1", result.Children[1].Name);
        Assert.Equal("Female", result.Children[1].Gender);
    }

    [Fact]
    public async Task CreateEmployee_WithSpouseOnly_NoChildren()
    {
        var dto = SampleCreateDto(withSpouse: true, childCount: 0);

        var result = await _service.CreateEmployeeAsync(dto);

        Assert.NotNull(result);
        Assert.NotNull(result.Spouse);
        Assert.Equal("Test Spouse", result.Spouse!.Name);
        Assert.Empty(result.Children);
    }

    [Fact]
    public async Task CreateEmployee_WithChildrenOnly_NoSpouse()
    {
        var dto = SampleCreateDto(withSpouse: false, childCount: 2);

        var result = await _service.CreateEmployeeAsync(dto);

        Assert.NotNull(result);
        Assert.Null(result.Spouse);
        Assert.Equal(2, result.Children.Count);
    }

    [Fact]
    public async Task CreateEmployee_WithoutRelations_ReturnsBasicDto()
    {
        var dto = SampleCreateDto(withSpouse: false, childCount: 0);

        var result = await _service.CreateEmployeeAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Test Employee", result.Name);
        Assert.Equal("Male", result.Gender);
        Assert.Equal("1234567890", result.NID);
        Assert.Equal("+8801312345678", result.Phone);
        Assert.Null(result.Spouse);
        Assert.Empty(result.Children);
    }

    // ─── UPDATE ───

    [Fact]
    public async Task UpdateEmployee_Success_ReturnsUpdatedDto()
    {
        var createDto = SampleCreateDto();
        var created = await _service.CreateEmployeeAsync(createDto);

        var updateDto = new UpdateEmployeeDto
        {
            Name = "Updated Name",
            Image = "https://example.com/updated.jpg",
            Gender = "Male",
            Phone = "+8801312345678",
            NID = "1234567890",
            Department = "Finance",
            BasicSalary = 60000m
        };

        var result = await _service.UpdateEmployeeAsync(created.Id, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result!.Name);
        Assert.Equal("Finance", result.Department);
        Assert.Equal(60000m, result.BasicSalary);
    }

    [Fact]
    public async Task UpdateEmployee_AddAndUpdateSpouse()
    {
        // Create without spouse
        var createDto = SampleCreateDto(withSpouse: false);
        var created = await _service.CreateEmployeeAsync(createDto);
        Assert.Null(created.Spouse);

        // Update: add spouse
        var updateDto = new UpdateEmployeeDto
        {
            Name = "Test Employee",
            Image = "https://example.com/img.jpg",
            Gender = "Male",
            Phone = "+8801312345678",
            NID = "1234567890",
            Department = "Engineering",
            BasicSalary = 50000m,
            Spouse = new CreateSpouseDto
            {
                Name = "New Spouse",
                Image = "https://example.com/spouse.jpg",
                Gender = "Female",
                NID = "1111111111"
            }
        };

        var result = await _service.UpdateEmployeeAsync(created.Id, updateDto);

        Assert.NotNull(result);
        Assert.NotNull(result!.Spouse);
        Assert.Equal("New Spouse", result.Spouse!.Name);

        // Update: remove spouse by setting null
        var removeSpouseDto = new UpdateEmployeeDto
        {
            Name = "Test Employee",
            Image = "https://example.com/img.jpg",
            Gender = "Male",
            Phone = "+8801312345678",
            NID = "1234567890",
            Department = "Engineering",
            BasicSalary = 50000m,
            Spouse = null
        };

        var result2 = await _service.UpdateEmployeeAsync(created.Id, removeSpouseDto);
        Assert.NotNull(result2);
        Assert.Null(result2!.Spouse);
    }

    [Fact]
    public async Task UpdateEmployee_ReplaceChildren()
    {
        var createDto = SampleCreateDto(childCount: 2);
        var created = await _service.CreateEmployeeAsync(createDto);
        Assert.Equal(2, created.Children.Count);

        var updateDto = new UpdateEmployeeDto
        {
            Name = "Test Employee",
            Image = "https://example.com/img.jpg",
            Gender = "Male",
            Phone = "+8801312345678",
            NID = "1234567890",
            Department = "Engineering",
            BasicSalary = 50000m,
            Children = new List<CreateChildDto>
            {
                new()
                {
                    Name = "New Child",
                    Image = "https://example.com/newchild.jpg",
                    Gender = "Female",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(2022, 6, 15), DateTimeKind.Utc)
                }
            }
        };

        var result = await _service.UpdateEmployeeAsync(created.Id, updateDto);

        Assert.NotNull(result);
        Assert.Single(result!.Children);
        Assert.Equal("New Child", result.Children[0].Name);
        Assert.Equal("Female", result.Children[0].Gender);
    }

    [Fact]
    public async Task UpdateEmployee_NotFound_ReturnsNull()
    {
        var updateDto = new UpdateEmployeeDto
        {
            Name = "Ghost",
            Image = "https://example.com/img.jpg",
            Gender = "Male",
            Phone = "+8801312345678",
            NID = "0000000000",
            Department = "None",
            BasicSalary = 0
        };

        var result = await _service.UpdateEmployeeAsync(999, updateDto);

        Assert.Null(result);
    }

    // ─── DELETE ───

    [Fact]
    public async Task DeleteEmployee_Existing_ReturnsTrue()
    {
        var createDto = SampleCreateDto(withSpouse: true, childCount: 1);
        var created = await _service.CreateEmployeeAsync(createDto);

        var deleted = await _service.DeleteEmployeeAsync(created.Id);

        Assert.True(deleted);

        // Verify employee is gone
        var fetched = await _service.GetEmployeeByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task DeleteEmployee_NotFound_ReturnsFalse()
    {
        var result = await _service.DeleteEmployeeAsync(999);

        Assert.False(result);
    }
}
