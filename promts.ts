` 
Implement a Global Search API for the EmployeeManagement backend.
The frontend already has debouncing, so do not implement debouncing on the backend.

Requirements

Endpoint

GET /api/employees/search

Query parameters

query (string) → search keyword

offset (int, default = 0)

limit (int, default = 50)

Search behavior

The search must be case-insensitive and match employees by:

Name

NID

Department

Example:

searching "engineering" should return employees in the Engineering department

searching "hasan" should return employees whose name contains Hasan

searching "123" should match NID containing 123

Use EF Core LINQ with proper indexing-friendly queries.

Infinite Scroll Compatibility

This API must support infinite scrolling, same as the existing GetAll endpoint.

Rules:

default limit = 50

frontend will increase offset to fetch next results

results must be ordered consistently (e.g. by Name or Id)

Skip(offset).Take(limit) must be used

Example:

Search "Engineering" returns 120 employees

offset=0 → first 50

offset=50 → next 50

offset=100 → final 20

The API must behave correctly for this scenario.

Response Format

Return an array of employees containing:

Name

Image

Spouse

Name

Image

Children[]

Name

Image

Same DTO structure as the GetAll endpoint so the frontend can reuse the same table component.

Architecture

Follow the existing architecture:

Controller

Service layer

DTOs (reuse if possible)

Files to implement:

EmployeesController.cs (search endpoint)

EmployeeService.cs (search logic)

Edge Cases to Handle

empty search query → return empty array

no matching employees → return empty array

employees without spouse

employees without children

offset larger than available results

negative offset or limit

Unit Tests

Create test cases covering:

Search by Name

Search by NID

Search by Department

Case-insensitive search

Infinite scroll behavior (offset + limit)

Empty search query

No matching results

Employee with no spouse

Employee with no children

Invalid offset/limit values

Documentation

Update the SRS.md file with this API using the same format already used for the GetAll endpoint.

Include:

API Name

Endpoint

Description

Query Parameters

Example Request

Example Response

Error Handling

Important

Do not modify existing models or SeedData

Reuse existing DTOs if possible

Ensure performance is good for large datasets

Ensure infinite scroll works exactly like the GetAll API
`