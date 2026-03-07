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
- **Search & Filtering** — Search employees by name, NID, department, or phone (planned).
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
      "department": "Engineering",
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

| Field       | Type        | Description                                |
|-------------|-------------|--------------------------------------------|
| `id`        | int         | Employee ID                                |
| `name`      | string      | Employee full name                         |
| `image`     | string      | Employee image URL                         |
| `department`| string      | Department name                            |
| `spouse`    | SpouseDto?  | Spouse info, or `null` if no spouse        |
| `children`  | ChildDto[]  | Array of children, empty if none           |

**Error Handling**

| Scenario                     | Behavior                                          |
|------------------------------|---------------------------------------------------|
| `page < 1`                  | Clamped to 1                                      |
| `pageSize < 1`              | Clamped to default (50)                           |
| `pageSize > 100`            | Clamped to max (100)                              |
| Page beyond total pages      | Returns empty `data` array with correct `totalCount` |
| Empty database               | Returns empty `data` array, `totalCount: 0`       |
| Database error               | Returns 500 Internal Server Error                 |

---

## 6. Future Scope (Planned)

- Employee search by name, NID, department, phone
- PDF report generation for individual/all employees
- Docker Compose for full-stack local development
- Image upload to cloud storage (replacing URL-based images)
