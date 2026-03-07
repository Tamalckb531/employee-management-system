import { useRef } from "react";
import { Pencil, Trash2, Eye } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useEmployees } from "../../hooks/useEmployees";
import { useAdminStore } from "../../store/adminStore";
import { exportPdf } from "../../lib/exportPdf";

const departmentStyles: Record<string, { color: string; bg: string }> = {
  Engineering: { color: "#6843E9", bg: "#F4F3FF" },
  Finance: { color: "#2B77C1", bg: "#F0F9FF" },
  HR: { color: "#3538CD", bg: "#EEF4FF" },
  Marketing: { color: "#C4326A", bg: "#FFF1F6" },
  Sales: { color: "#B54708", bg: "#FFF6ED" },
  Operations: { color: "#027A48", bg: "#ECFDF3" },
  Support: { color: "#7A2E0E", bg: "#FFF4ED" },
};

const EmployeeTable = () => {
  const navigate = useNavigate();
  const { employees, loading, handleSearch, tableRef, query } = useEmployees();
  const isAdmin = useAdminStore((s) => s.isAdmin);
  const printRef = useRef<HTMLDivElement>(null);

  return (
    <div className="w-full h-full p-5 flex items-center justify-center">
      <div ref={printRef} className="overflow-x-auto max-w-350 rounded-lg border shadow-xl border-gray-200 bg-[#FFFFFC]">
        {/* Header */}
        <div className="flex items-center justify-between flex-wrap gap-4 p-4">
          <button
            onClick={() => exportPdf(printRef.current, "employee-table")}
            className="border border-slate-400 text-slate-600 px-4 py-2 rounded-md text-sm font-medium hover:bg-gray-100 hover:text-black"
          >
            Export table as pdf
          </button>

          <div className="relative">
            <div className="absolute inset-y-0 inset-s-0 flex items-center ps-3 pointer-events-none">
              <svg
                className="w-4 h-4 text-body"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                fill="none"
                viewBox="0 0 24 24"
              >
                <path
                  stroke="currentColor"
                  strokeLinecap="round"
                  strokeWidth="2"
                  d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"
                />
              </svg>
            </div>
            <input
              type="text"
              value={query}
              onChange={(e) => handleSearch(e.target.value)}
              className="block w-full max-w-96 ps-9 pe-3 py-2 border border-slate-400 text-heading text-sm rounded-xl shadow-xs placeholder:text-body outline-none"
              placeholder="Search"
            />
          </div>
        </div>

        {/* Table */}
        <div ref={tableRef} className="max-h-[75vh] overflow-y-auto">
          <table className="min-w-300 text-sm text-left">
            <thead className="border-y border-slate-400 bg-gray-50 sticky top-0 z-10">
              <tr>
                <th className="px-6 py-3 font-semibold">Employee</th>
                <th className="px-6 py-3 font-semibold">NID</th>
                <th className="px-6 py-3 font-semibold">Phone</th>
                <th className="px-6 py-3 font-semibold">Department</th>
                <th className="px-6 py-3 font-semibold">Salary</th>
                <th className="px-6 py-3 font-semibold">Family</th>
                {isAdmin && (
                  <th className="px-6 py-3 font-semibold">Actions</th>
                )}
              </tr>
            </thead>

            <tbody>
              {employees.map((emp) => {
                const dept = departmentStyles[emp.department] ?? {
                  color: "#6B7280",
                  bg: "#F3F4F6",
                };

                return (
                  <tr key={emp.id} className="hover:bg-gray-50">
                    {/* Employee */}
                    <td
                      className="px-6 py-4 flex items-center gap-3 cursor-pointer"
                      onClick={() => navigate(`/employee/${emp.id}`)}
                    >
                      <img
                        src={emp.image}
                        alt={emp.name}
                        className="w-10 h-10 rounded-full"
                      />
                      <div>
                        <div className="font-semibold">{emp.name}</div>
                      </div>
                    </td>

                    {/* NID */}
                    <td className="px-6 py-4">{emp.nid}</td>

                    {/* Phone */}
                    <td className="px-6 py-4">{emp.phone}</td>

                    {/* Department */}
                    <td className="px-6 py-4">
                      <span
                        className="px-3 py-1 rounded-full text-xs font-medium border"
                        style={{
                          color: dept.color,
                          background: dept.bg,
                          borderColor: dept.color,
                        }}
                      >
                        {emp.department}
                      </span>
                    </td>

                    {/* Salary */}
                    <td className="px-6 py-4 font-medium">
                      {emp.basicSalary.toLocaleString()} BDT
                    </td>

                    {/* Family */}
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2">
                        {emp.spouse && (
                          <img
                            src={emp.spouse.image}
                            alt={emp.spouse.name}
                            title={emp.spouse.name}
                            className="w-8 h-8 rounded-full"
                          />
                        )}
                        {emp.children.length > 0 && (
                          <span className="text-xs text-gray-500">
                            {emp.children.length} child
                            {emp.children.length > 1 ? "ren" : ""}
                          </span>
                        )}
                        {!emp.spouse && emp.children.length === 0 && "-"}
                      </div>
                    </td>

                    {/* Actions */}
                    {isAdmin && (
                      <td className="px-6 py-4">
                        <div className="flex gap-3 items-center">
                          <Eye
                            onClick={() => navigate(`/employee/${emp.id}`)}
                            className="w-5 h-5 cursor-pointer text-black hover:text-green-500"
                          />
                          <Pencil
                            onClick={() => navigate("/form")}
                            className="w-5 h-5 cursor-pointer text-black hover:text-blue-400"
                          />
                          <Trash2 className="w-5 h-5 cursor-pointer text-black hover:text-red-400" />
                        </div>
                      </td>
                    )}
                  </tr>
                );
              })}
            </tbody>
          </table>

          {loading && (
            <div className="flex justify-center py-4">
              <div className="w-6 h-6 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default EmployeeTable;
