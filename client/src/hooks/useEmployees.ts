import { useCallback, useEffect, useRef } from "react";
import { useEmployeeStore } from "../store/employeeStore";
import { fetchEmployees, searchEmployees } from "../lib/api";
import { useDebounce } from "./useDebounce";
import { useThrottle } from "./useThrottle";

const PAGE_SIZE = 20;

export function useEmployees() {
  const { employees, loading, query, setQuery, resetEmployees } =
    useEmployeeStore();

  const tableRef = useRef<HTMLDivElement>(null);
  const debouncedQuery = useDebounce(query, 300);
  const isFetchingRef = useRef(false);
  const debouncedQueryRef = useRef(debouncedQuery);
  debouncedQueryRef.current = debouncedQuery;

  // Stable loadMore — reads all state from store directly
  const loadMore = useCallback(async () => {
    const { hasMore } = useEmployeeStore.getState();
    if (isFetchingRef.current || !hasMore) return;
    isFetchingRef.current = true;

    const { setLoading, appendEmployees, setHasMore, incrementPage } =
      useEmployeeStore.getState();
    setLoading(true);

    try {
      const q = debouncedQueryRef.current;
      if (q) {
        const offset = useEmployeeStore.getState().employees.length;
        const res = await searchEmployees(q, offset, PAGE_SIZE);
        appendEmployees(res.data);
        setHasMore(res.hasMore);
      } else {
        const currentPage = useEmployeeStore.getState().page;
        const res = await fetchEmployees(currentPage, PAGE_SIZE);
        appendEmployees(res.data);
        setHasMore(res.hasMore);
        incrementPage();
      }
    } catch (err) {
      console.error("Failed to load employees:", err);
      setHasMore(false);
    } finally {
      setLoading(false);
      isFetchingRef.current = false;
    }
  }, []);

  // Initial load + re-fetch when debounced search query changes
  useEffect(() => {
    resetEmployees();
    isFetchingRef.current = false;
    loadMore();
  }, [debouncedQuery, resetEmployees, loadMore]);

  // Infinite scroll
  const handleScroll = useCallback(() => {
    const el = tableRef.current;
    if (!el) return;
    const { loading, hasMore } = useEmployeeStore.getState();
    if (loading || !hasMore) return;

    const { scrollTop, scrollHeight, clientHeight } = el;
    if (scrollTop + clientHeight >= scrollHeight - 100) {
      loadMore();
    }
  }, [loadMore]);

  const throttledScroll = useThrottle(handleScroll, 200);

  useEffect(() => {
    const el = tableRef.current;
    if (!el) return;

    el.addEventListener("scroll", throttledScroll);
    return () => el.removeEventListener("scroll", throttledScroll);
  }, [throttledScroll]);

  const handleSearch = useCallback(
    (value: string) => {
      setQuery(value);
    },
    [setQuery]
  );

  return {
    employees,
    loading,
    handleSearch,
    tableRef,
    query,
  };
}
