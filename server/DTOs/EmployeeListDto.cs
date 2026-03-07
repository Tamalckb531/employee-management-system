namespace EmployeeManagement.DTOs;

public class EmployeeListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string NID { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public SpouseDto? Spouse { get; set; }
    public List<ChildDto> Children { get; set; } = new();
}
public class EmployeeListFullDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string NID { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public SpouseFullDto? Spouse { get; set; }
    public List<ChildFullDto> Children { get; set; } = new();
}

public class SpouseDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}
public class SpouseFullDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string NID { get; set; } = string.Empty;
}

public class ChildDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

public class ChildFullDto
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}
