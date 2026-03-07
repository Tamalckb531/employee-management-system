const EmployeeDetailsPage = () => {
  // Sample data
  const employee = {
    name: "Hasan Mahmud",
    phone: "+8801712345678",
    image: "https://randomuser.me/api/portraits/men/1.jpg",
    nid: "1234567890",
    gender: "Male",
    department: "Engineering",
    basicSalary: "45,000 BDT",
    spouse: {
      name: "Moushumi Akter",
      nid: "9876543210",
      gender: "Female",
      image: "https://randomuser.me/api/portraits/women/1.jpg",
    },
    children: [
      {
        name: "Rafiq Hasan",
        dob: "2018-01-01",
        gender: "Male",
        image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq",
      },
      {
        name: "Sara Hasan",
        dob: "2020-05-10",
        gender: "Female",
        image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Sara",
      },
    ],
  };

  return (
    <div className="w-full h-full p-5 mt-20 flex items-center justify-center">
      <div className="overflow-x-auto max-w-350 rounded-lg border shadow-xl border-gray-200 bg-[#FFFFFC] p-5 space-y-6">
        
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
            <button className="border border-slate-400 text-slate-600 px-4 py-2 rounded-md text-sm font-medium hover:bg-gray-100 hover:text-black">
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
              <span className="font-semibold">Department:</span> {employee.department}
            </div>
            <div>
              <span className="font-semibold">Basic Salary:</span> {employee.basicSalary}
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
                  <span className="font-semibold">Name:</span> {employee.spouse.name}
                </div>
                <div>
                  <span className="font-semibold">NID:</span> {employee.spouse.nid}
                </div>
                <div>
                  <span className="font-semibold">Gender:</span> {employee.spouse.gender}
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
                <div key={index} className="flex items-center justify-around gap-6 text-lg">
                  <img
                    src={child.image}
                    alt={child.name}
                    className="w-20 h-20 rounded-full object-cover"
                  />
                    <div>
                      <span className="font-semibold">Name:</span> {child.name}
                    </div>
                    <div>
                      <span className="font-semibold">Date of Birth:</span> {child.dob}
                    </div>
                    <div>
                      <span className="font-semibold">Gender:</span> {child.gender}
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