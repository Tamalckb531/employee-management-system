using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services;

public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeListDto>> GetEmployeesAsync(int page, int pageSize);
    Task<List<EmployeeListDto>> SearchEmployeesAsync(string query, int offset, int limit);
    Task<EmployeeListFullDto?> GetEmployeeByIdAsync(int id);
    Task<EmployeeListFullDto> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<EmployeeListFullDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
    Task<bool> DeleteEmployeeAsync(int id);
}

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 50;
    private const int DefaultLimit = 50;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<EmployeeListDto>> GetEmployeesAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        var totalCount = await _context.Employees.CountAsync();

        var employees = await _context.Employees
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(e => e.Spouse)
            .Include(e => e.Children)
            .Select(e => new EmployeeListDto
            {
                Id = e.Id,
                Name = e.Name,
                Image = e.Image,
                Gender = e.Gender,
                NID = e.NID,
                Phone = e.Phone,
                Department = e.Department,
                BasicSalary = e.BasicSalary,
                Spouse = e.Spouse != null ? new SpouseDto
                {
                    Name = e.Spouse.Name,
                    Image = e.Spouse.Image
                } : null,
                Children = e.Children.Select(c => new ChildDto
                {
                    Name = c.Name,
                    Image = c.Image
                }).ToList()
            })
            .ToListAsync();

        return new PaginatedResponse<EmployeeListDto>
        {
            Data = employees,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = (page * pageSize) < totalCount
        };
    }

    public async Task<List<EmployeeListDto>> SearchEmployeesAsync(string query, int offset, int limit)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<EmployeeListDto>();

        if (offset < 0) offset = 0;
        if (limit < 1) limit = DefaultLimit;

        var searchTerm = query.Trim().ToLower();

        return await _context.Employees
            .AsNoTracking()
            .Where(e =>
                e.Name.ToLower().Contains(searchTerm) ||
                e.NID.ToLower().Contains(searchTerm) ||
                e.Department.ToLower().Contains(searchTerm))
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .Select(e => new EmployeeListDto
            {
                Id = e.Id,
                Name = e.Name,
                Image = e.Image,
                Gender = e.Gender,
                NID = e.NID,
                Phone = e.Phone,
                Department = e.Department,
                BasicSalary = e.BasicSalary,
                Spouse = e.Spouse != null ? new SpouseDto
                {
                    Name = e.Spouse.Name,
                    Image = e.Spouse.Image
                } : null,
                Children = e.Children.Select(c => new ChildDto
                {
                    Name = c.Name,
                    Image = c.Image
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<EmployeeListFullDto?> GetEmployeeByIdAsync(int id)
    {
        if (id < 1) return null;

        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeListFullDto
            {
                Id = e.Id,
                Name = e.Name,
                Image = e.Image,
                Gender = e.Gender,
                NID = e.NID,
                Phone = e.Phone,
                Department = e.Department,
                BasicSalary = e.BasicSalary,
                Spouse = e.Spouse != null ? new SpouseFullDto
                {
                    Name = e.Spouse.Name,
                    Image = e.Spouse.Image,
                    Gender = e.Spouse.Gender,
                    NID = e.Spouse.NID,
                } : null,
                Children = e.Children.Select(c => new ChildFullDto
                {
                    Name = c.Name,
                    Image = c.Image,
                    Gender = c.Gender,
                    DateOfBirth = c.DateOfBirth,
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EmployeeListFullDto> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            Name = dto.Name,
            Image = dto.Image,
            Gender = dto.Gender,
            Phone = dto.Phone,
            NID = dto.NID,
            Department = dto.Department,
            BasicSalary = dto.BasicSalary
        };

        if (dto.Spouse != null)
        {
            employee.Spouse = new Spouse
            {
                Name = dto.Spouse.Name,
                Image = dto.Spouse.Image,
                Gender = dto.Spouse.Gender,
                NID = dto.Spouse.NID
            };
        }

        if (dto.Children is { Count: > 0 })
        {
            foreach (var childDto in dto.Children)
            {
                employee.Children.Add(new Child
                {
                    Name = childDto.Name,
                    Image = childDto.Image,
                    Gender = childDto.Gender,
                    DateOfBirth = childDto.DateOfBirth
                });
            }
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return (await GetEmployeeByIdAsync(employee.Id))!;
    }

    public async Task<EmployeeListFullDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .Include(e => e.Spouse)
            .Include(e => e.Children)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return null;

        employee.Name = dto.Name;
        employee.Image = dto.Image;
        employee.Gender = dto.Gender;
        employee.Phone = dto.Phone;
        employee.NID = dto.NID;
        employee.Department = dto.Department;
        employee.BasicSalary = dto.BasicSalary;

        // Handle spouse: upsert or remove
        if (dto.Spouse != null)
        {
            if (employee.Spouse != null)
            {
                employee.Spouse.Name = dto.Spouse.Name;
                employee.Spouse.Image = dto.Spouse.Image;
                employee.Spouse.Gender = dto.Spouse.Gender;
                employee.Spouse.NID = dto.Spouse.NID;
            }
            else
            {
                employee.Spouse = new Spouse
                {
                    Name = dto.Spouse.Name,
                    Image = dto.Spouse.Image,
                    Gender = dto.Spouse.Gender,
                    NID = dto.Spouse.NID
                };
            }
        }
        else if (employee.Spouse != null)
        {
            _context.Spouses.Remove(employee.Spouse);
        }

        // Handle children: replace entire list
        var existingChildren = employee.Children.ToList();
        _context.Children.RemoveRange(existingChildren);
        employee.Children.Clear();

        if (dto.Children is { Count: > 0 })
        {
            foreach (var childDto in dto.Children)
            {
                employee.Children.Add(new Child
                {
                    Name = childDto.Name,
                    Image = childDto.Image,
                    Gender = childDto.Gender,
                    DateOfBirth = childDto.DateOfBirth
                });
            }
        }

        await _context.SaveChangesAsync();

        return (await GetEmployeeByIdAsync(employee.Id))!;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}
