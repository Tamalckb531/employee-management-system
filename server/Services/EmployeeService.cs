using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services;

public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeListDto>> GetEmployeesAsync(int page, int pageSize);
    Task<List<EmployeeListDto>> SearchEmployeesAsync(string query, int offset, int limit);
    Task<EmployeeListFullDto?> GetEmployeeByIdAsync(int id);
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
}
