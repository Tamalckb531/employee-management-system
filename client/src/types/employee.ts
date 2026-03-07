export interface Employee {
  id: number;
  name: string;
  image: string;
  gender: string;
  nid: string;
  phone: string;
  department: string;
  basicSalary: number;
  spouse?: {
    name: string;
    image: string;
  };
  children: {
    name: string;
    image: string;
  }[];
}

export interface EmployeeListResponse {
  data: Employee[];
  page: number;
  pageSize: number;
  totalCount: number;
  hasMore: boolean;
}

export interface EmployeeSearchResponse {
  data: Employee[];
  hasMore: boolean;
}
