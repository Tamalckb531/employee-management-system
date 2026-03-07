import React, { useState } from "react";

const EmployeeFormPage = () => {
  const [children, setChildren] = useState([
    { name: "", dob: "", gender: "", image: "" },
  ]);

  const addChild = () => {
    setChildren([...children, { name: "", dob: "", gender: "", image: "" }]);
  };

  const removeChild = (index: number) => {
    const newChildren = [...children];
    newChildren.splice(index, 1);
    setChildren(newChildren);
  };

  return (
    <div className="w-full min-h-screen p-5 flex justify-center">
      <div className="w-full max-w-3xl space-y-6">
        
        {/* Employee Section */}
        <div className="p-5 border rounded-lg shadow-sm bg-[#FFFFFC] space-y-4">
          <h2 className="text-xl font-bold text-center">Employee</h2>
          <div className="grid grid-cols-2 gap-4">
            <input type="text" placeholder="Name" className="border p-2 rounded" />
            <input type="text" placeholder="Image URL" className="border p-2 rounded" />
            <input type="text" placeholder="Gender" className="border p-2 rounded" />
            <input type="text" placeholder="NID" className="border p-2 rounded" />
            <input type="text" placeholder="Phone" className="border p-2 rounded" />
            <input type="text" placeholder="Department" className="border p-2 rounded" />
            <input type="text" placeholder="Basic Salary" className="border p-2 rounded" />
          </div>
        </div>

        {/* Spouse Section */}
        <div className="p-5 border rounded-lg shadow-sm bg-[#FFFFFC] space-y-4">
          <h2 className="text-xl font-bold text-center">Spouse</h2>
          <div className="grid grid-cols-2 gap-4">
            <input type="text" placeholder="Name" className="border p-2 rounded" />
            <input type="text" placeholder="Image URL" className="border p-2 rounded" />
            <input type="text" placeholder="Gender" className="border p-2 rounded" />
            <input type="text" placeholder="NID" className="border p-2 rounded" />
          </div>
        </div>

        {/* Children Section */}
        <div className="p-5 border rounded-lg shadow-sm bg-[#FFFFFC] space-y-4">
          <h2 className="text-xl font-bold text-center">Children</h2>
          <div className="space-y-4">
            {children.map((child, index) => (
              <div key={index} className="grid grid-cols-2 gap-4 items-end">
                <input type="text" placeholder="Name" className="border p-2 rounded" />
                <input type="text" placeholder="Image URL" className="border p-2 rounded" />
                <input type="text" placeholder="Gender" className="border p-2 rounded" />
                <input type="date" placeholder="Date of Birth" className="border p-2 rounded" />
                <button
                  type="button"
                  onClick={() => removeChild(index)}
                  className="col-span-2 px-3 py-1 bg-red-500 text-white rounded hover:bg-red-600"
                >
                  Remove Child
                </button>
              </div>
            ))}
          </div>
          <button
            type="button"
            onClick={addChild}
            className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600"
          >
            Add Child
          </button>
        </div>

        {/* Submit Button */}
        <div className="text-center">
          <button className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
            Submit
          </button>
        </div>

      </div>
    </div>
  );
};

export default EmployeeFormPage;