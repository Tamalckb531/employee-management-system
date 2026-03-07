import { create } from "zustand";
import type { Employee } from "../types/employee";

interface EmployeeState {
  employees: Employee[];
  page: number;
  hasMore: boolean;
  loading: boolean;
  query: string;
  setQuery: (query: string) => void;
  resetEmployees: () => void;
  appendEmployees: (employees: Employee[]) => void;
  setLoading: (loading: boolean) => void;
  setHasMore: (hasMore: boolean) => void;
  incrementPage: () => void;
}

export const useEmployeeStore = create<EmployeeState>((set) => ({
  employees: [],
  page: 1,
  hasMore: true,
  loading: false,
  query: "",
  setQuery: (query) => set({ query }),
  resetEmployees: () => set({ employees: [], page: 1, hasMore: true }),
  appendEmployees: (newEmployees) =>
    set((state) => ({ employees: [...state.employees, ...newEmployees] })),
  setLoading: (loading) => set({ loading }),
  setHasMore: (hasMore) => set({ hasMore }),
  incrementPage: () => set((state) => ({ page: state.page + 1 })),
}));
