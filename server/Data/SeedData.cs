using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (context.Database.IsRelational())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        if (await context.Employees.AnyAsync())
            return;

        // Seed Admin
        var admin = new Admin
        {
            Username = "Admin",
            Passkey = HashPassword("123456"),
            Role = "Admin"
        };
        context.Admins.Add(admin);

        // Seed Employees
        var employees = GetEmployees();
        context.Employees.AddRange(employees);

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static List<Employee> GetEmployees()
    {
        return new List<Employee>
        {
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/1.jpg",
                Gender = "Male",
                Name = "Hasan Mahmud",
                NID = "1234567890",
                Phone = "+8801712345678",
                Department = "Engineering",
                BasicSalary = 55000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/1.jpg",
                    Gender = "Female",
                    Name = "Moushumi Akter",
                    NID = "1234567891"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq",
                        Gender = "Male",
                        Name = "Rafiq Hasan",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2015, 3, 12), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Nadia",
                        Gender = "Female",
                        Name = "Nadia Hasan",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2018, 7, 25), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/2.jpg",
                Gender = "Male",
                Name = "Tanvir Ahmed",
                NID = "2345678901",
                Phone = "+8801812345678",
                Department = "Finance",
                BasicSalary = 48000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/2.jpg",
                    Gender = "Female",
                    Name = "Sadia Rahman",
                    NID = "2345678902"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Ayan",
                        Gender = "Male",
                        Name = "Ayan Tanvir",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2019, 1, 8), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/women/3.jpg",
                Gender = "Female",
                Name = "Farhana Yasmin",
                NID = "3456789012",
                Phone = "+8801912345678",
                Department = "Human Resources",
                BasicSalary = 45000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/men/3.jpg",
                    Gender = "Male",
                    Name = "Rahim Uddin",
                    NID = "3456789013"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Tasnim",
                        Gender = "Female",
                        Name = "Tasnim Farhana",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2016, 11, 3), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Farhan",
                        Gender = "Male",
                        Name = "Farhan Rahim",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2019, 5, 20), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Lamia",
                        Gender = "Female",
                        Name = "Lamia Rahim",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2022, 2, 14), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/4.jpg",
                Gender = "Male",
                Name = "Karim Sheikh",
                NID = "4567890123",
                Phone = "+8801312345678",
                Department = "Marketing",
                BasicSalary = 52000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/4.jpg",
                    Gender = "Female",
                    Name = "Nusrat Jahan",
                    NID = "4567890124"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Mahir",
                        Gender = "Male",
                        Name = "Mahir Karim",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2020, 9, 15), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/5.jpg",
                Gender = "Male",
                Name = "Tamal Bhowmik",
                NID = "5678901234",
                Phone = "+8801512345678",
                Department = "Engineering",
                BasicSalary = 60000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/5.jpg",
                    Gender = "Female",
                    Name = "Sharmin Sultana",
                    NID = "5678901235"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Arham",
                        Gender = "Male",
                        Name = "Arham Tamal",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2017, 6, 30), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Anika",
                        Gender = "Female",
                        Name = "Anika Tamal",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2021, 4, 10), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/women/6.jpg",
                Gender = "Female",
                Name = "Nusrat Begum",
                NID = "6789012345",
                Phone = "+8801612345678",
                Department = "Accounts",
                BasicSalary = 42000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/men/6.jpg",
                    Gender = "Male",
                    Name = "Arif Hossain",
                    NID = "6789012346"
                },
                Children = new List<Child>()
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/7.jpg",
                Gender = "Male",
                Name = "Arif Khan",
                NID = "7890123456",
                Phone = "+8801712345679",
                Department = "IT Support",
                BasicSalary = 38000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/7.jpg",
                    Gender = "Female",
                    Name = "Fatema Khatun",
                    NID = "7890123457"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Samin",
                        Gender = "Male",
                        Name = "Samin Arif",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2014, 8, 22), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Sabrina",
                        Gender = "Female",
                        Name = "Sabrina Arif",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2017, 12, 5), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Saif",
                        Gender = "Male",
                        Name = "Saif Arif",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2021, 3, 18), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/women/8.jpg",
                Gender = "Female",
                Name = "Sadia Islam",
                NID = "8901234567",
                Phone = "+8801812345679",
                Department = "Administration",
                BasicSalary = 47000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/men/8.jpg",
                    Gender = "Male",
                    Name = "Shakil Ahmed",
                    NID = "8901234568"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Riyad",
                        Gender = "Male",
                        Name = "Riyad Shakil",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2018, 10, 2), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/9.jpg",
                Gender = "Male",
                Name = "Rahim Mia",
                NID = "9012345678",
                Phone = "+8801912345679",
                Department = "Logistics",
                BasicSalary = 35000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/9.jpg",
                    Gender = "Female",
                    Name = "Halima Begum",
                    NID = "9012345679"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Maliha",
                        Gender = "Female",
                        Name = "Maliha Rahim",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2013, 4, 17), DateTimeKind.Utc)
                    },
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Mehedi",
                        Gender = "Male",
                        Name = "Mehedi Rahim",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2016, 9, 28), DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Image = "https://randomuser.me/api/portraits/men/10.jpg",
                Gender = "Male",
                Name = "Imran Hossain",
                NID = "1012345678",
                Phone = "+8801312345679",
                Department = "Engineering",
                BasicSalary = 58000m,
                Spouse = new Spouse
                {
                    Image = "https://randomuser.me/api/portraits/women/10.jpg",
                    Gender = "Female",
                    Name = "Rabeya Sultana",
                    NID = "1012345670"
                },
                Children = new List<Child>
                {
                    new()
                    {
                        Image = "https://api.dicebear.com/7.x/adventurer/svg?seed=Ishan",
                        Gender = "Male",
                        Name = "Ishan Imran",
                        DateOfBirth = DateTime.SpecifyKind(new DateTime(2020, 7, 11), DateTimeKind.Utc)
                    }
                }
            }
        };
    }
}
