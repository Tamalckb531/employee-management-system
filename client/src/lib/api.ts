import type { Employee, EmployeeListResponse, EmployeeSearchResponse } from "../types/employee";

export async function fetchEmployees(
  page: number,
  pageSize: number
): Promise<EmployeeListResponse> {
  const res = await fetch(
    `/api/employees?page=${page}&pageSize=${pageSize}`
  );
  if (!res.ok) throw new Error("Failed to fetch employees");
  return res.json();
}

export async function searchEmployees(
  query: string,
  offset: number,
  limit: number
): Promise<EmployeeSearchResponse> {
  const res = await fetch(
    `/api/employees/search?query=${encodeURIComponent(query)}&offset=${offset}&limit=${limit}`
  );
  if (!res.ok) throw new Error("Failed to search employees");
  const data: Employee[] = await res.json();
  return { data, hasMore: data.length >= limit };
}
