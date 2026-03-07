using EmployeeManagement.Data;
using EmployeeManagement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services;

public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeListDto>> GetEmployeesAsync(int page, int pageSize);
}

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 50;

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
                Department = e.Department,
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
}
