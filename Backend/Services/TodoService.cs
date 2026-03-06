using Backend.Models;

namespace Backend.Services;

public class TodoService
{
    private static readonly List<Todo> todos = [];

    public List<Todo> GetAll()
    {
        return todos;
    }

    public Todo Add(Todo todo)
    {
        todo.Id = todos.Count + 1;
        todos.Add(todo);
        return todo;
    }

    public Todo? Update(int id, Todo updatedTodo)
    {
        var existing = todos.FirstOrDefault(x => x.Id == id);
        if (existing != null)
        {
            existing.Title = updatedTodo.Title;
            existing.Completed = updatedTodo.Completed;
        }
        return existing;
    }

    public void Delete(int id)
    {
        var t = todos.FirstOrDefault(x => x.Id == id);
        if (t != null)
            todos.Remove(t);
    }
}