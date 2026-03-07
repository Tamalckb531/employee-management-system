using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50
    )
    {
        var result = await _employeeService.GetEmployeesAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchEmployees(
        [FromQuery] string query = "",
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 50
    )
    {
        var result = await _employeeService.SearchEmployeesAsync(query, offset, limit);
        return Ok(result);
    }
}
