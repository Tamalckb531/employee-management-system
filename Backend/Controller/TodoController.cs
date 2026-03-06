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

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return Ok();
    }
}