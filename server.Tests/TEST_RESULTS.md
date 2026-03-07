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
| 14 | DTO maps correct fields (name, image, department) | Passed |
| 15 | Results are ordered by ID ascending | Passed |

**Total: 15 passed, 0 failed**
