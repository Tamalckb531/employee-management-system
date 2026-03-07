` 
You have the context of the EmployeeManagement project and the SeedData.cs file.

Task: Create a new API to fetch employee data for a frontend data table with infinite scroll.

Requirements:

API endpoint: GET /api/employees.

Returns first 50 employees by default. Should support pagination or throttling for infinite scroll (frontend will pass page or offset).

Response: An array of employee objects containing:

Employee Name, Image

Spouse Name, Image (if exists)

Children array with each child's Name and Image (if exists)

Implement a service layer for business logic.

Include controller, service, and DTOs as needed.

Include unit tests covering:

Default 50 results

Pagination/offset

Employees with no spouse

Employees with no children

Empty database scenario

Invalid page or offset values

Generate an SRS.md file entry for this API in the assessment format, including:

API Name

Endpoint

Request parameters

Response format

Description

Error handling

Do not implement global search or filters yet.

Make sure the code is correct, clean, and follows EF Core best practices.

Output:

EmployeesController.cs

EmployeeService.cs

DTOs (if needed)

Unit test file with all edge cases

SRS.md entry

Keep all other project structure and data intact. Do not modify existing models or seed data.
`