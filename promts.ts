` 
Implement an API endpoint to fetch full details of a single employee for the employee details page.

Endpoint
GET /api/employees/{id}
Behavior

When the frontend clicks an employee row in the table, it will send the employee ID to this endpoint.

Backend flow:

Receive id

Find the employee in the database

Load related data:

Spouse

Children

Return the complete employee information

Use EF Core Include() to fetch relations efficiently.

Response DTO

Use the existing EmployeeDto structure which now includes:

Name

Image

Gender

Phone

NID

Department

BasicSalary

Spouse

Name

Image

Children[]

Name

Image

Return null or 404 if the employee does not exist.

Architecture

Follow the same architecture already used:

Controller

Service layer

DTO reuse

Files to update/create:

EmployeesController.cs

EmployeeService.cs

Add a method similar to:

GetEmployeeByIdAsync(int id)
Edge Cases

Handle the following scenarios:

Employee exists with spouse and children

Employee exists without spouse

Employee exists without children

Employee exists with no relations

Employee ID does not exist

Invalid ID (negative or zero)

Unit Tests

Create unit tests covering:

Employee exists and returns full details

Employee with spouse returns populated SpouseDto

Employee without spouse returns null

Employee with children returns populated ChildDto list

Employee without children returns empty list

Non-existent employee ID returns null or 404

Negative ID handled correctly

Returned DTO contains correct mapped fields (name, image, gender, NID, phone, department, basicSalary, spouse, children)

Test Documentation

Update the file:

Server.Tests/TEST_CASE.md

Add a new section titled:

## 3. Employee Service — Get Employee By Id

Follow the same table format already used in that file and include all test cases with Status = Passed.

SRS Documentation

Update SRS.md and add documentation for this API following the same format used for previous APIs.

Include:

API Name

Endpoint

Description

Path Parameter

Example Request

Example Response

Error Handling

Important

Do not modify existing models

Reuse existing DTOs

Follow existing coding style

Ensure the API returns a single EmployeeDto object
`