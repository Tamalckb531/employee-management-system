using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.DTOs;

public class CreateEmployeeDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string NID { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public decimal BasicSalary { get; set; }

    public CreateSpouseDto? Spouse { get; set; }
    public List<CreateChildDto>? Children { get; set; }
}

public class UpdateEmployeeDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string NID { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public decimal BasicSalary { get; set; }

    public CreateSpouseDto? Spouse { get; set; }
    public List<CreateChildDto>? Children { get; set; }
}

public class CreateSpouseDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string NID { get; set; } = string.Empty;
}

public class CreateChildDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Image { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }
}
