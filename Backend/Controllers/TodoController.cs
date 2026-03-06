using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly TodoService _service;

    public TodoController(TodoService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetTodos()
    {
        return Ok(_service.GetAll());
    }

    [HttpPost]
    public IActionResult CreateTodo([FromBody] Todo todo)
    {
        var result = _service.Add(todo);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTodo(int id, [FromBody] Todo todo)
    {
        var updated = _service.Update(id, todo);

        if (updated == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return Ok();
    }
}