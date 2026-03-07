namespace EmployeeManagement.DTOs;

public class EmployeeListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public SpouseDto? Spouse { get; set; }
    public List<ChildDto> Children { get; set; } = new();
}

public class SpouseDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

public class ChildDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}
