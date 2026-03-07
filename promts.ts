`You are a senior full-stack developer and software architect. Your task is to implement the **Database Design phase** of an Employee Registry System. The project structure is already initialized:

/client → React + Tailwind + Zustand
/server → ASP.NET Core Web API + EF Core + PostgreSQL

Docker will be used for PostgreSQL and full stack startup using a single docker-compose later.

Now I want to start with the **Database Design** phase.

Your tasks:

1. Design the core database entities based on the system requirements:

Employee
Spouse
Child
Admin

Relationships:
- One Employee → One Spouse
- One Employee → Many Children

Admin fields:
- Id
- Username
- Passkey (hashed Ideally)
- Role (Admin)

Employee fields:
- Id
- Image
- Gender
- Name
- NID
- Phone
- Department
- BasicSalary

Spouse fields:
- Id
- Image
- Gender
- Name
- NID
- EmployeeId

Child fields:
- Id
- Image
- Gender
- Name
- DateOfBirth
- EmployeeId

Requirements:
- NID must be unique
- NID must be 10 or 17 digits
- Phone must follow Bangladesh phone format
- Proper relational mapping using EF Core
- Use clean naming conventions
- Prepare models that are suitable for migrations later
(Admin should be hardcoded while seeding the database)

Implementation tasks:

1. Create Entity Models inside '/server/Models'
2. Create 'AppDbContext' inside '/server/Data'
3. Configure relationships using Fluent API if needed
4. Prepare the project for EF Core migrations
5. Create a simple 'SeedData' class that inserts 10 example employees with spouse/children (With realistic Bangladeshi name Like Hasan Tanvir Rahim Karim Moushumi Farhana Tamal Sadia Arif Nusrat etc. Please make sure that you are using opposite gender as spouse for each time every time. And also add children. Make varieties on the amount of them. use https://randomuser.me/api/portraits/{Gender}/{id}.jpg for creating images of Employee and spouse and use https://api.dicebear.com/7.x/adventurer/svg?seed={Name} to create images of children)
6. Hard code the Admin in seed data using the Username:Admin, Passkey:123456 and Role:Admin

IMPORTANT:
You must also start creating the **root level file 'SRS_Document.md'**.

Update the SRS with:

- System Scope
- Database Design Explanation
- Entity Relationship Diagram (text diagram is fine)
- Assumptions
- Edge Cases

Remember the project evaluation matrix:

Code Quality → Clean architecture, DTO usage
Functionality → Search and PDF exports later
Database Design → Correct relational mapping
SRS Clarity → Professional documentation

Do not overengineer.
Keep architecture clean and readable.

After finishing:
- Show folder structure
- Show the models
- Show DbContext
- Show SeedData
- Show the updated SRS section

`