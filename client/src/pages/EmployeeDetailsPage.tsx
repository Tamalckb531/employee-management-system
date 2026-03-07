import { useParams } from "react-router-dom";
import { useRef } from "react";
import { useEmployee } from "../hooks/useEmployee";
import { exportPdf } from "../lib/exportPdf";

const EmployeeDetailsPage = () => {
  const { id } = useParams<{ id: string }>();
  const { employee, loading, error } = useEmployee(Number(id));
  const contentRef = useRef<HTMLDivElement>(null);

  if (loading) {
    return (
      <div className="w-full h-screen flex items-center justify-center">
        <div className="w-8 h-8 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (error || !employee) {
    return (
      <div className="w-full h-screen flex items-center justify-center">
        <p className="text-red-500 text-lg">{error ?? "Employee not found"}</p>
      </div>
    );
  }

  return (
    <div className="w-full h-full p-5 mt-20 flex items-center justify-center">
      <div
        ref={contentRef}
        className="overflow-x-auto max-w-350 rounded-lg border shadow-xl border-gray-200 bg-[#FFFFFC] p-5 space-y-6"
      >
        {/* Employee Section */}
        <div className="space-y-4 w-300">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <img
                src={employee.image}
                alt={employee.name}
                className="w-32 h-32 rounded-full object-cover"
              />
              <div className="flex flex-col">
                <span className="text-3xl font-bold">{employee.name}</span>
                <span className="text-lg text-slate-500">{employee.phone}</span>
              </div>
            </div>
            <button
              onClick={() => exportPdf(contentRef.current, `${employee.name}-details`)}
              className="border border-slate-400 text-slate-600 px-4 py-2 rounded-md text-sm font-medium hover:bg-gray-100 hover:text-black"
            >
              Export as pdf
            </button>
          </div>
          <div className="grid grid-cols-2 gap-4 text-lg">
            <div>
              <span className="font-semibold">NID:</span> {employee.nid}
            </div>
            <div>
              <span className="font-semibold">Gender:</span> {employee.gender}
            </div>
            <div>
              <span className="font-semibold">Department:</span>{" "}
              {employee.department}
            </div>
            <div>
              <span className="font-semibold">Basic Salary:</span>{" "}
              {employee.basicSalary.toLocaleString()} BDT
            </div>
          </div>
          <div className="border-b border-gray-300 w-1/2 mx-auto"></div>
        </div>

        {/* Spouse Section */}
        {employee.spouse && (
          <div className="space-y-4">
            <h2 className="text-center text-xl font-bold">Spouse</h2>
            <div className="flex items-center justify-around gap-6 text-lg">
              <img
                src={employee.spouse.image}
                alt={employee.spouse.name}
                className="w-20 h-20 rounded-full object-cover"
              />
              <div>
                <span className="font-semibold">Name:</span>{" "}
                {employee.spouse.name}
              </div>
              <div>
                <span className="font-semibold">NID:</span>{" "}
                {employee.spouse.nid}
              </div>
              <div>
                <span className="font-semibold">Gender:</span>{" "}
                {employee.spouse.gender}
              </div>
            </div>
            <div className="border-b border-gray-300 w-1/2 mx-auto"></div>
          </div>
        )}

        {/* Children Section */}
        {employee.children && employee.children.length > 0 && (
          <div className="space-y-4">
            <h2 className="text-center text-xl font-bold">Children</h2>
            <div className="flex flex-col gap-3">
              {employee.children.map((child, index) => (
                <div
                  key={index}
                  className="flex items-center justify-around gap-6 text-lg"
                >
                  <img
                    src={child.image}
                    alt={child.name}
                    className="w-20 h-20 rounded-full object-cover"
                  />
                  <div>
                    <span className="font-semibold">Name:</span> {child.name}
                  </div>
                  <div>
                    <span className="font-semibold">Date of Birth:</span>{" "}
                    {new Date(child.dateOfBirth).toLocaleDateString()}
                  </div>
                  <div>
                    <span className="font-semibold">Gender:</span>{" "}
                    {child.gender}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default EmployeeDetailsPage;
