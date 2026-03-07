# Test Results

## 1. Employee Service — Get Employees (Paginated)

Tests for `GET /api/employees` endpoint logic. Validates pagination behavior, input clamping, empty states, and correct DTO mapping of employee, spouse, and children data.

| # | Test | Status |
|---|------|--------|
| 1 | Returns default 50 results when more exist, `hasMore: true` | Passed |
| 2 | Returns all results when less than page size, `hasMore: false` | Passed |
| 3 | Second page returns remaining records | Passed |
| 4 | Page beyond last page returns empty data | Passed |
| 5 | Empty database returns empty list, `totalCount: 0` | Passed |
| 6 | Negative page value clamped to 1 | Passed |
| 7 | Zero page value clamped to 1 | Passed |
| 8 | Negative pageSize clamped to default (50) | Passed |
| 9 | Excessive pageSize clamped to max (100) | Passed |
| 10 | Employee with spouse returns populated SpouseDto | Passed |
| 11 | Employee without spouse returns `null` | Passed |
| 12 | Employee with children returns populated ChildDto array | Passed |
| 13 | Employee without children returns empty array | Passed |
| 14 | DTO maps correct fields (name, image, gender, NID, phone, department, basicSalary) | Passed |
| 15 | Results are ordered by ID ascending | Passed |

**Total: 15 passed, 0 failed**

## 2. Employee Service — Search Employees

Tests for `GET /api/employees/search` endpoint logic. Validates case-insensitive search by name, NID, and department, infinite scroll with offset/limit, edge cases for empty queries, missing relations, and invalid parameters.

| # | Test | Status |
|---|------|--------|
| 1 | Search by name returns matching employees | Passed |
| 2 | Search by NID returns matching employees | Passed |
| 3 | Search by department returns matching employees | Passed |
| 4 | Search is case-insensitive (lower, upper, mixed) | Passed |
| 5 | Infinite scroll — first batch (offset=0, limit=50) | Passed |
| 6 | Infinite scroll — second batch (offset=50, limit=50) | Passed |
| 7 | Infinite scroll — final batch (offset=100, limit=50) | Passed |
| 8 | Infinite scroll — results ordered by ID ascending | Passed |
| 9 | Empty query returns empty array | Passed |
| 10 | Whitespace-only query returns empty array | Passed |
| 11 | No matching results returns empty array | Passed |
| 12 | Employee without spouse returns `null` spouse | Passed |
| 13 | Employee without children returns empty children array | Passed |
| 14 | Employee with children returns populated children | Passed |
| 15 | Negative offset clamped to 0 | Passed |
| 16 | Negative limit clamped to default (50) | Passed |
| 17 | Offset beyond available results returns empty array | Passed |
| 18 | Returns correct DTO fields (name, image, gender, NID, phone, department, basicSalary, spouse, children) | Passed |

**Total: 18 passed, 0 failed**

## 3. Employee Service — Get Employee By Id

Tests for `GET /api/employees/{id}` endpoint logic. Validates fetching a single employee with full details including spouse (name, image, gender, NID) and children (name, image, gender, dateOfBirth), handling of missing relations, non-existent IDs, and correct DTO mapping.

| # | Test | Status |
|---|------|--------|
| 1 | Existing employee returns full details (spouse + children) | Passed |
| 2 | Employee with spouse returns populated SpouseFullDto (name, image, gender, NID) | Passed |
| 3 | Employee without spouse returns `null` | Passed |
| 4 | Employee with children returns populated ChildFullDto list (name, image, gender, dateOfBirth) | Passed |
| 5 | Employee without children returns empty list | Passed |
| 6 | Non-existent employee ID returns `null` | Passed |
| 7 | Negative ID returns `null` | Passed |
| 8 | Zero ID returns `null` | Passed |
| 9 | Returns correct DTO fields (id, name, image, gender, NID, phone, department, basicSalary, spouse with gender/NID, children with gender/dateOfBirth) | Passed |

**Total: 9 passed, 0 failed**
