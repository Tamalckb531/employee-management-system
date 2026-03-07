# Project Specification: Employee Registry System

## Project Context

This project is an **Employee Registry System** built as part of a technical assessment. The system manages employees and their family information and allows searching, exporting reports, and role-based data management. The goal is to demonstrate clean architecture, correct database design, and working functionality.

---

## Technology Stack

### Backend

* **Framework:** ASP.NET Core Web API
* **ORM:** Entity Framework Core
* **Database:** PostgreSQL
* **Auth:** JWT Authentication

### Frontend

* **Library:** React
* **Styling:** Tailwind CSS
* **State Management:** Zustand
* **Icons:** Lucide Icons

---

# Containerization & Development Environment

The project must support a **fully containerized development environment using Docker**.

Claude must maintain a **single root-level `docker-compose.yml` file** that orchestrates the entire system.

The goal is that the entire project can run using **one command only**.

Example command:

docker compose up --build

This command must automatically:

1. Start a PostgreSQL database container
2. Initialize the database
3. Run EF Core migrations
4. Seed the initial employee data
5. Start the ASP.NET Core backend server
6. Start the React frontend
7. Connect frontend → backend
8. Connect backend → database

No local PostgreSQL installation should be required.

---

# Docker Services

The docker-compose setup must include:

postgres  
backend  
frontend  

Postgres container:
- Official PostgreSQL image
- Persistent volume for database data
- Environment variables for database name, user, and password

Backend container:
- Builds the ASP.NET Core API
- Runs migrations automatically on startup
- Connects to the Postgres container

Frontend container:
- Runs the React development server
- Connects to the backend API

---

# Docker Development Workflow

The expected development workflow is:

1. Clone repository
2. Run:

docker compose up --build

3. The system automatically starts:

PostgreSQL  
Backend API  
Frontend app  

4. The frontend becomes accessible in the browser.

No manual database setup should be required.

---

# Docker Requirements

Claude must ensure:

- PostgreSQL runs only inside Docker
- Backend connects using the docker service name
- Environment variables are used for configuration
- Database migrations run automatically on startup
- Seed data is inserted automatically
- Containers restart correctly if needed

The goal is to make the entire project **reproducible with a single command**.

---

# Important Docker Rule

Claude must always keep the docker setup **simple and minimal**.

Avoid:

- overly complex networking
- unnecessary services
- development tooling containers

The docker-compose file should remain **clean and easy to understand**.

...

## Core System Requirements

The system must support:

* Employee management
* Family relationships
* Global search
* PDF exports
* Role-based access control

---

## Employee Data Model

### Employee

* Id
* Image
* Name
* NID
* Phone
* Department
* BasicSalary

### Spouse

* Id
* Image
* Name
* NID
* EmployeeId

### Child

* Id
* Image
* Name
* DateOfBirth
* EmployeeId

---

## Database Relationships

Each employee has:

* One spouse
* Multiple children

**Relationship Structure:**
`Employee ├── Spouse (One-to-One) └── Children (One-to-Many)`

---

## Data Constraints

The following validations must be enforced:

### NID

* Must be unique
* Must contain 10 or 17 digits

### Phone

* Must match Bangladesh phone format
* Example: `+8801XXXXXXXXX` or `01XXXXXXXXX`

---

## User Roles

Two roles exist in the system:

1. **Viewer:**
* Read-only access
* Can view employees, search, and export PDFs


2. **Admin:**
* Full CRUD access (Create, Update, Delete)
* Can manage spouse and children data



---

## Authentication System

Authentication must use **JWT tokens**.

**Flow:**

1. User clicks "Login as Admin".
2. A dialog asks for an admin passkey.
3. Backend validates the passkey and returns a JWT token.
4. The frontend stores the token.
5. Admin routes and operations (POST, PUT, DELETE) require JWT verification.

---

## Search Requirements

The system must implement global search matching:

* Name
* NID
* Department

**Rules:**

* Case insensitive
* Fast query performance
* Debounced input on the frontend (`GET /employees?search=value`)

---

## PDF Export Features

1. **Employee Table Export:** Export the current filtered employee list as a PDF.
2. **Employee CV Export:** Export a single employee profile including spouse and children information.

---

## Backend Architecture Rules

Follow a clean backend structure:

* **Controllers:** Handle HTTP requests, validate inputs, and call services.
* **Services:** Contain business logic and operations like PDF generation.
* **Models:** Represent database entities.
* **DTOs:** Used for API request/response objects to prevent exposing database models directly.
* **DbContext:** Manages database connections and entity relationships.

### Database Initialization

* Include seed data for **10 employees**.
* Implemented using EF Core migrations.

---

## Frontend Architecture Rules

Modular and professional structure:
`src ├── components ├── hooks ├── pages ├── store ├── services └── utils`

### Guidelines

* Reusable UI parts must be in `/components`.
* Business logic should live inside custom hooks.
* Global state must use **Zustand**.
* Icons must use **Lucide Icons**.

---

## Frontend Design Guidelines

The UI must follow professional design engineering practices:

* **Style:** Minimalist and professional.
* **Palette:** Cream/warm neutral background with dark gray primary text.
* **Details:** Consistent spacing, clean typography, soft border radius, and subtle shadows.
* **Avoid:** Random gradients and visual clutter.

---

## Evaluation Criteria

| Category | Weight | Focus Areas |
| --- | --- | --- |
| **Code Quality** | 30% | Clean architecture, naming conventions, DTO usage. |
| **Functionality** | 30% | PDF exports, fast search, debounced input. |
| **Database Design** | 20% | Relational mapping, Employee ↔ Family structure. |
| **SRS Clarity** | 20% | Professional formatting, logical documentation. |

---

## Edge Case Handling

* Duplicate NID or invalid NID length.
* Invalid phone format.
* Employees without a spouse or children.
* Deleting employees with existing family records (Cascading vs. Restricted).

---

## Documentation Requirements

Maintain a root-level `SRS_Document.md` containing:

* **System Scope:** What the software does and does not do.
* **ERD:** Visual or text-based representation.
* **Edge Cases:** Handling of duplicates and invalid formats.
* **Assumptions:** e.g., "An employee can only have one spouse."

---