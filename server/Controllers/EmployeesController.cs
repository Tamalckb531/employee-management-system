using EmployeeManagement.DTOs;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var result = await _employeeService.GetEmployeeByIdAsync(id);
        if (result == null) return NotFound();
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        var result = await _employeeService.CreateEmployeeAsync(dto);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var result = await _employeeService.UpdateEmployeeAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await _employeeService.DeleteEmployeeAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
