import { useEffect, useState } from "react";
import { fetchEmployeeById } from "../lib/api";
import type { EmployeeDetail } from "../types/employee";

export function useEmployee(id: number) {
  const [employee, setEmployee] = useState<EmployeeDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    fetchEmployeeById(id)
      .then((data) => {
        if (!cancelled) setEmployee(data);
      })
      .catch((err) => {
        if (!cancelled) setError(err.message);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [id]);

  return { employee, loading, error };
}
