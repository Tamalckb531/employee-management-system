# Software Requirements Specification (SRS)
## Employee Registry System

**Version:** 1.0
**Date:** March 2026

---

## 1. System Scope

The Employee Registry System is a full-stack web application for managing employee records within an organization. It provides:

- **Employee CRUD** — Create, read, update, and delete employee profiles including personal details, department, and salary.
- **Family Records** — Track spouse and children information linked to each employee.
- **Admin Authentication** — Role-based access via a hardcoded admin account.
- **Search & Filtering** — Search employees by name, NID, or department with case-insensitive matching.
- **PDF Export** — Generate PDF reports for employee data (planned).

### Tech Stack

| Layer     | Technology                              |
|-----------|-----------------------------------------|
| Frontend  | React, TypeScript, Vite, TailwindCSS    |
| Backend   | ASP.NET Core Web API (.NET 8)           |
| Database  | PostgreSQL                              |
| ORM       | Entity Framework Core                   |
| Infra     | Docker, Docker Compose (planned)        |

---

## 2. Database Design

### 2.1 Entities

**Admin**
| Field    | Type         | Constraints        |
|----------|--------------|--------------------|
| Id       | int          | PK, auto-increment |
| Username | string(50)   | Required, unique   |
| Passkey  | string       | Required, hashed   |
| Role     | string(20)   | Required           |

**Employee**
| Field       | Type          | Constraints                               |
|-------------|---------------|-------------------------------------------|
| Id          | int           | PK, auto-increment                        |
| Image       | string        | Required, URL                              |
| Gender      | string        | Required                                   |
| Name        | string(100)   | Required                                   |
| NID         | string        | Required, unique, 10 or 17 digits          |
| Phone       | string        | Required, unique, BD format (+880/01...)    |
| Department  | string(100)   | Required                                   |
| BasicSalary | decimal(18,2) | Required                                   |

**Spouse**
| Field      | Type        | Constraints                      |
|------------|-------------|----------------------------------|
| Id         | int         | PK, auto-increment               |
| Image      | string      | Required, URL                     |
| Gender     | string      | Required                          |
| Name       | string(100) | Required                          |
| NID        | string      | Required, unique, 10 or 17 digits |
| EmployeeId | int         | FK → Employee, unique (1:1)       |

**Child**
| Field       | Type        | Constraints                |
|-------------|-------------|----------------------------|
| Id          | int         | PK, auto-increment         |
| Image       | string      | Required, URL               |
| Gender      | string      | Required                    |
| Name        | string(100) | Required                    |
| DateOfBirth | DateTime    | Required                    |
| EmployeeId  | int         | FK → Employee (1:many)      |

### 2.2 Entity Relationship Diagram

```
┌──────────────────┐
│      Admin       │
├──────────────────┤
│ Id (PK)          │
│ Username (UQ)    │
│ Passkey          │
│ Role             │
└──────────────────┘

┌──────────────────┐       1:1       ┌──────────────────┐
│    Employee      │────────────────▶│     Spouse       │
├──────────────────┤                 ├──────────────────┤
│ Id (PK)          │                 │ Id (PK)          │
│ Image            │                 │ Image            │
│ Gender           │                 │ Gender           │
│ Name             │                 │ Name             │
│ NID (UQ)         │                 │ NID (UQ)         │
│ Phone (UQ)       │                 │ EmployeeId (FK)  │
│ Department       │                 └──────────────────┘
│ BasicSalary      │
└────────┬─────────┘
         │
         │ 1:N
         ▼
┌──────────────────┐
│      Child       │
├──────────────────┤
│ Id (PK)          │
│ Image            │
│ Gender           │
│ Name             │
│ DateOfBirth      │
│ EmployeeId (FK)  │
└──────────────────┘
```

### 2.3 Relationship Rules

- **Employee → Spouse**: One-to-one. Each employee has at most one spouse. Deleting an employee cascades to their spouse.
- **Employee → Children**: One-to-many. An employee can have zero or more children. Deleting an employee cascades to their children.
- **Admin**: Standalone entity with no foreign key relationships. Seeded with a hardcoded account.

---

## 3. Assumptions

1. Each employee has at most one spouse record in the system.
2. NID is either 10 digits (old format) or 17 digits (smart NID format) — both formats are valid in Bangladesh.
3. Phone numbers follow Bangladesh mobile format: starts with `+880` or `0`, followed by `1[3-9]` and 8 more digits.
4. The admin account is hardcoded during database seeding — there is no admin registration flow.
5. Employee images are stored as external URLs, not uploaded files.
6. BasicSalary is stored in BDT (Bangladeshi Taka).
7. The system is single-tenant (one organization).

---

## 4. Edge Cases

| Scenario | Handling |
|----------|----------|
| Duplicate NID on employee creation | Rejected — NID has a unique database constraint |
| Duplicate phone number | Rejected — Phone has a unique database constraint |
| NID with wrong digit count (not 10 or 17) | Rejected at model validation via regex |
| Invalid phone format | Rejected at model validation via regex |
| Employee without a spouse | Allowed — Spouse is nullable on the Employee entity |
| Employee with no children | Allowed — Children collection defaults to empty |
| Deleting an employee with family records | Cascade delete removes spouse and all children |
| Multiple admins | Not supported by design — single seeded admin |
| Admin passkey storage | SHA-256 hashed before storage |

---

## 5. API Specifications

### 5.1 Get Employees (Paginated)

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Get Employees                                       |
| **Endpoint**      | `GET /api/employees`                                |
| **Method**        | GET                                                 |
| **Description**   | Returns a paginated list of employees with their spouse and children info, designed for infinite scroll on the frontend. |

**Request Parameters (Query String)**

| Parameter  | Type | Default | Description                              |
|------------|------|---------|------------------------------------------|
| `page`     | int  | 1       | Page number (1-based). Clamped to 1 if < 1. |
| `pageSize` | int  | 50      | Items per page. Clamped to 50 if < 1, max 100. |

**Response Format (200 OK)**

```json
{
  "data": [
    {
      "id": 1,
      "name": "Hasan Mahmud",
      "image": "https://randomuser.me/api/portraits/men/1.jpg",
      "gender": "Male",
      "nid": "1234567890",
      "phone": "+8801712345678",
      "department": "Engineering",
      "basicSalary": 45000.00,
      "spouse": {
        "name": "Moushumi Akter",
        "image": "https://randomuser.me/api/portraits/women/1.jpg"
      },
      "children": [
        {
          "name": "Rafiq Hasan",
          "image": "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq"
        }
      ]
    }
  ],
  "page": 1,
  "pageSize": 50,
  "totalCount": 10,
  "hasMore": false
}
```

**Response Fields**

| Field        | Type               | Description                                  |
|--------------|--------------------|----------------------------------------------|
| `data`       | EmployeeListDto[]  | Array of employee objects for current page    |
| `page`       | int                | Current page number                           |
| `pageSize`   | int                | Number of items per page                      |
| `totalCount` | int                | Total employees in database                   |
| `hasMore`    | bool               | Whether more pages exist after current page   |

**Employee DTO Fields**

| Field         | Type          | Description                                |
|---------------|---------------|--------------------------------------------|
| `id`          | int           | Employee ID                                |
| `name`        | string        | Employee full name                         |
| `image`       | string        | Employee image URL                         |
| `gender`      | string        | Employee gender                            |
| `nid`         | string        | National ID (10 or 17 digits)              |
| `phone`       | string        | Phone number (BD format)                   |
| `department`  | string        | Department name                            |
| `basicSalary` | decimal       | Basic salary in BDT                        |
| `spouse`      | SpouseDto?    | Spouse info, or `null` if no spouse        |
| `children`    | ChildDto[]    | Array of children, empty if none           |

**Error Handling**

| Scenario                     | Behavior                                          |
|------------------------------|---------------------------------------------------|
| `page < 1`                  | Clamped to 1                                      |
| `pageSize < 1`              | Clamped to default (50)                           |
| `pageSize > 100`            | Clamped to max (100)                              |
| Page beyond total pages      | Returns empty `data` array with correct `totalCount` |
| Empty database               | Returns empty `data` array, `totalCount: 0`       |
| Database error               | Returns 500 Internal Server Error                 |

### 5.2 Search Employees

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Search Employees                                    |
| **Endpoint**      | `GET /api/employees/search`                         |
| **Method**        | GET                                                 |
| **Description**   | Searches employees by name, NID, or department with case-insensitive matching. Supports infinite scroll via offset/limit pagination. Returns the same DTO structure as the GetAll endpoint. |

**Request Parameters (Query String)**

| Parameter | Type   | Default | Description                                              |
|-----------|--------|---------|----------------------------------------------------------|
| `query`   | string | `""`    | Search keyword. Matches against Name, NID, and Department. Empty/whitespace returns empty array. |
| `offset`  | int    | 0       | Number of results to skip. Clamped to 0 if negative.     |
| `limit`   | int    | 50      | Maximum number of results to return. Clamped to 50 if < 1. |

**Example Request**

```
GET /api/employees/search?query=engineering&offset=0&limit=50
GET /api/employees/search?query=hasan&offset=0&limit=50
GET /api/employees/search?query=123&offset=50&limit=50
```

**Response Format (200 OK)**

```json
[
  {
    "id": 1,
    "name": "Hasan Mahmud",
    "image": "https://randomuser.me/api/portraits/men/1.jpg",
    "gender": "Male",
    "nid": "1234567890",
    "phone": "+8801712345678",
    "department": "Engineering",
    "basicSalary": 45000.00,
    "spouse": {
      "name": "Moushumi Akter",
      "image": "https://randomuser.me/api/portraits/women/1.jpg"
    },
    "children": [
      {
        "name": "Rafiq Hasan",
        "image": "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq"
      }
    ]
  }
]
```

**Response Fields**

Returns an array of `EmployeeListDto` objects (same structure as the GetAll endpoint).

| Field         | Type          | Description                                |
|---------------|---------------|--------------------------------------------|
| `id`          | int           | Employee ID                                |
| `name`        | string        | Employee full name                         |
| `image`       | string        | Employee image URL                         |
| `gender`      | string        | Employee gender                            |
| `nid`         | string        | National ID (10 or 17 digits)              |
| `phone`       | string        | Phone number (BD format)                   |
| `department`  | string        | Department name                            |
| `basicSalary` | decimal       | Basic salary in BDT                        |
| `spouse`      | SpouseDto?    | Spouse info, or `null` if no spouse        |
| `children`    | ChildDto[]    | Array of children, empty if none           |

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Empty or whitespace query       | Returns empty array `[]`                      |
| No matching employees           | Returns empty array `[]`                      |
| `offset < 0`                   | Clamped to 0                                  |
| `limit < 1`                    | Clamped to default (50)                       |
| Offset beyond available results | Returns empty array `[]`                      |
| Employee without spouse         | `spouse` field is `null`                      |
| Employee without children       | `children` field is empty array `[]`          |
| Database error                  | Returns 500 Internal Server Error             |

**Infinite Scroll Behavior**

The API is designed for infinite scrolling. The frontend increases `offset` to fetch the next batch:

| Scenario: "Engineering" matches 120 employees | Result          |
|-----------------------------------------------|-----------------|
| `offset=0, limit=50`                          | First 50 results  |
| `offset=50, limit=50`                         | Next 50 results   |
| `offset=100, limit=50`                        | Final 20 results  |

### 5.3 Get Employee By Id

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Get Employee By Id                                  |
| **Endpoint**      | `GET /api/employees/{id}`                           |
| **Method**        | GET                                                 |
| **Description**   | Returns the full details of a single employee including spouse (with gender and NID) and children (with gender and date of birth) data. Used by the frontend when clicking an employee row to view their details page. |

**Path Parameter**

| Parameter | Type | Description                        |
|-----------|------|------------------------------------|
| `id`      | int  | The employee ID to look up.        |

**Example Request**

```
GET /api/employees/1
GET /api/employees/5
```

**Response Format (200 OK)**

```json
{
  "id": 1,
  "name": "Hasan Mahmud",
  "image": "https://randomuser.me/api/portraits/men/1.jpg",
  "gender": "Male",
  "nid": "1234567890",
  "phone": "+8801712345678",
  "department": "Engineering",
  "basicSalary": 45000.00,
  "spouse": {
    "name": "Moushumi Akter",
    "image": "https://randomuser.me/api/portraits/women/1.jpg",
    "gender": "Female",
    "nid": "9876543210"
  },
  "children": [
    {
      "name": "Rafiq Hasan",
      "image": "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq",
      "gender": "Male",
      "dateOfBirth": "2018-01-01T00:00:00Z"
    }
  ]
}
```

**Response Fields**

Returns a single `EmployeeListFullDto` object with extended spouse and children details.

| Field         | Type             | Description                                |
|---------------|------------------|--------------------------------------------|
| `id`          | int              | Employee ID                                |
| `name`        | string           | Employee full name                         |
| `image`       | string           | Employee image URL                         |
| `gender`      | string           | Employee gender                            |
| `nid`         | string           | National ID (10 or 17 digits)              |
| `phone`       | string           | Phone number (BD format)                   |
| `department`  | string           | Department name                            |
| `basicSalary` | decimal          | Basic salary in BDT                        |
| `spouse`      | SpouseFullDto?   | Spouse info with gender and NID, or `null` if no spouse |
| `children`    | ChildFullDto[]   | Array of children with gender and dateOfBirth, empty if none |

**SpouseFullDto Fields**

| Field    | Type   | Description                   |
|----------|--------|-------------------------------|
| `name`   | string | Spouse full name              |
| `image`  | string | Spouse image URL              |
| `gender` | string | Spouse gender                 |
| `nid`    | string | Spouse National ID            |

**ChildFullDto Fields**

| Field         | Type     | Description                  |
|---------------|----------|------------------------------|
| `name`        | string   | Child full name              |
| `image`       | string   | Child image URL              |
| `gender`      | string   | Child gender                 |
| `dateOfBirth` | DateTime | Child date of birth (UTC)    |

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Employee found                  | Returns `200 OK` with employee object         |
| Employee ID does not exist      | Returns `404 Not Found`                       |
| ID is negative or zero          | Returns `404 Not Found`                       |
| Employee without spouse         | `spouse` field is `null`                      |
| Employee without children       | `children` field is empty array `[]`          |
| Database error                  | Returns 500 Internal Server Error             |

### 5.4 Admin Login

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Admin Login                                         |
| **Endpoint**      | `POST /api/admin/login`                             |
| **Method**        | POST                                                |
| **Description**   | Authenticates an admin user using username and passkey. The passkey is hashed using SHA256 and compared against the stored hash. On success, returns a JWT token containing AdminId, Username, and Role claims. The token can be used for future admin-protected operations (create, update, delete employees). |

**Request Body (JSON)**

| Field      | Type   | Required | Description                    |
|------------|--------|----------|--------------------------------|
| `username` | string | Yes      | Admin username                 |
| `passkey`  | string | Yes      | Admin password (plain text)    |

**Example Request**

```
POST /api/admin/login
Content-Type: application/json

{
  "username": "Admin",
  "passkey": "123456"
}
```

**Response Format (200 OK)**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

**Response Fields**

| Field       | Type   | Description                                    |
|-------------|--------|------------------------------------------------|
| `token`     | string | JWT token for authenticating future requests   |
| `expiresIn` | int    | Token validity duration in seconds (3600 = 1 hour) |

**JWT Token Claims**

| Claim      | Description                          |
|------------|--------------------------------------|
| `AdminId`  | The admin's database ID              |
| `Name`     | The admin's username                 |
| `Role`     | The admin's role (e.g., "Admin")     |

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Valid credentials               | Returns `200 OK` with JWT token               |
| Invalid username                | Returns `401 Unauthorized`                    |
| Invalid passkey                 | Returns `401 Unauthorized`                    |
| Empty username                  | Returns `401 Unauthorized`                    |
| Empty passkey                   | Returns `401 Unauthorized`                    |
| Database error                  | Returns 500 Internal Server Error             |

**JWT Configuration**

JWT settings are stored in `appsettings.json`:

| Setting    | Description                              |
|------------|------------------------------------------|
| `Key`      | Secret key for signing tokens (min 32 chars) |
| `Issuer`   | Token issuer identifier                  |
| `Audience` | Token audience identifier                |

The JWT middleware is configured to validate issuer, audience, lifetime, and signing key. Future admin-only endpoints can be protected using `[Authorize(Roles = "Admin")]`.

### 5.5 Create Employee

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Create Employee                                     |
| **Endpoint**      | `POST /api/employees`                               |
| **Method**        | POST                                                |
| **Auth**          | `Authorization: Bearer <jwt_token>` (Role: Admin)   |
| **Description**   | Creates a new employee with optional spouse and children. Requires a valid admin JWT token. Returns the full created employee DTO. |

**Request Body (JSON)**

| Field         | Type              | Required | Description                        |
|---------------|-------------------|----------|------------------------------------|
| `name`        | string            | Yes      | Employee full name                 |
| `image`       | string            | Yes      | Employee image URL                 |
| `gender`      | string            | Yes      | Employee gender                    |
| `phone`       | string            | Yes      | Phone number (BD format)           |
| `nid`         | string            | Yes      | National ID (10 or 17 digits)      |
| `department`  | string            | Yes      | Department name                    |
| `basicSalary` | decimal           | Yes      | Basic salary in BDT                |
| `spouse`      | CreateSpouseDto?  | No       | Spouse data, or `null`             |
| `children`    | CreateChildDto[]? | No       | Array of children, or `null`       |

**CreateSpouseDto Fields**

| Field    | Type   | Required | Description        |
|----------|--------|----------|--------------------|
| `name`   | string | Yes      | Spouse full name   |
| `image`  | string | Yes      | Spouse image URL   |
| `gender` | string | Yes      | Spouse gender      |
| `nid`    | string | Yes      | Spouse National ID |

**CreateChildDto Fields**

| Field         | Type     | Required | Description           |
|---------------|----------|----------|-----------------------|
| `name`        | string   | Yes      | Child full name       |
| `image`       | string   | Yes      | Child image URL       |
| `gender`      | string   | Yes      | Child gender          |
| `dateOfBirth` | DateTime | Yes      | Child date of birth   |

**Example Request**

```
POST /api/employees
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Kamal Ahmed",
  "image": "https://example.com/kamal.jpg",
  "gender": "Male",
  "phone": "+8801712345000",
  "nid": "1111111111",
  "department": "Engineering",
  "basicSalary": 55000.00,
  "spouse": {
    "name": "Fatema Ahmed",
    "image": "https://example.com/fatema.jpg",
    "gender": "Female",
    "nid": "2222222222"
  },
  "children": [
    {
      "name": "Rahim Ahmed",
      "image": "https://example.com/rahim.jpg",
      "gender": "Male",
      "dateOfBirth": "2018-05-10T00:00:00Z"
    }
  ]
}
```

**Response Format (201 Created)**

Returns the created `EmployeeListFullDto` object (same structure as Get Employee By Id).

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Valid admin token                | Returns `201 Created` with employee object    |
| Missing JWT token               | Returns `401 Unauthorized`                    |
| Invalid JWT token               | Returns `401 Unauthorized`                    |
| Non-admin role                  | Returns `403 Forbidden`                       |
| Duplicate NID                   | Returns `500` (unique constraint violation)   |
| Duplicate phone                 | Returns `500` (unique constraint violation)   |
| Database error                  | Returns `500 Internal Server Error`           |

### 5.6 Update Employee

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Update Employee                                     |
| **Endpoint**      | `PUT /api/employees/{id}`                           |
| **Method**        | PUT                                                 |
| **Auth**          | `Authorization: Bearer <jwt_token>` (Role: Admin)   |
| **Description**   | Updates an existing employee's fields, spouse, and children. If spouse is provided, it is created or updated. If spouse is `null`, the existing spouse is removed. Children are replaced entirely with the provided list. Requires a valid admin JWT token. |

**Path Parameter**

| Parameter | Type | Description                        |
|-----------|------|------------------------------------|
| `id`      | int  | The employee ID to update.         |

**Request Body (JSON)**

Same structure as Create Employee request body (`UpdateEmployeeDto`).

**Example Request**

```
PUT /api/employees/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Kamal Ahmed (Updated)",
  "image": "https://example.com/kamal-new.jpg",
  "gender": "Male",
  "phone": "+8801712345000",
  "nid": "1111111111",
  "department": "Finance",
  "basicSalary": 60000.00,
  "spouse": null,
  "children": []
}
```

**Response Format (200 OK)**

Returns the updated `EmployeeListFullDto` object.

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Employee found and updated      | Returns `200 OK` with updated employee object |
| Employee ID does not exist      | Returns `404 Not Found`                       |
| Missing JWT token               | Returns `401 Unauthorized`                    |
| Invalid JWT token               | Returns `401 Unauthorized`                    |
| Non-admin role                  | Returns `403 Forbidden`                       |
| Database error                  | Returns `500 Internal Server Error`           |

**Spouse Update Behavior**

| Scenario                             | Action                          |
|--------------------------------------|---------------------------------|
| `spouse` provided, employee has none | New spouse created              |
| `spouse` provided, employee has one  | Existing spouse updated         |
| `spouse` is `null`, employee has one | Existing spouse deleted          |
| `spouse` is `null`, employee has none| No action                       |

**Children Update Behavior**

All existing children are removed and replaced with the provided list. If `children` is `null` or empty, all children are removed.

### 5.7 Delete Employee

| Field             | Detail                                              |
|-------------------|-----------------------------------------------------|
| **API Name**      | Delete Employee                                     |
| **Endpoint**      | `DELETE /api/employees/{id}`                        |
| **Method**        | DELETE                                              |
| **Auth**          | `Authorization: Bearer <jwt_token>` (Role: Admin)   |
| **Description**   | Deletes an employee by ID. Cascade deletes the associated spouse and all children. Requires a valid admin JWT token. |

**Path Parameter**

| Parameter | Type | Description                        |
|-----------|------|------------------------------------|
| `id`      | int  | The employee ID to delete.         |

**Example Request**

```
DELETE /api/employees/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response Format (204 No Content)**

Returns no body on successful deletion.

**Error Handling**

| Scenario                        | Behavior                                      |
|---------------------------------|-----------------------------------------------|
| Employee found and deleted      | Returns `204 No Content`                      |
| Employee ID does not exist      | Returns `404 Not Found`                       |
| Missing JWT token               | Returns `401 Unauthorized`                    |
| Invalid JWT token               | Returns `401 Unauthorized`                    |
| Non-admin role                  | Returns `403 Forbidden`                       |
| Database error                  | Returns `500 Internal Server Error`           |

---

## 6. Future Scope (Planned)

- PDF report generation for individual/all employees
- Docker Compose for full-stack local development
- Image upload to cloud storage (replacing URL-based images)
