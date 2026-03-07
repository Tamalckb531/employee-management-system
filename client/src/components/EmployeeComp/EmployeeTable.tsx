import { Pencil, Trash2 } from "lucide-react";
import { useNavigate } from "react-router-dom";

const departmentStyles: Record<string, { color: string; bg: string }> = {
  Engineering: { color: "#6843E9", bg: "#F4F3FF" },
  Finance: { color: "#2B77C1", bg: "#F0F9FF" },
  HR: { color: "#3538CD", bg: "#EEF4FF" },
  Marketing: { color: "#C4326A", bg: "#FFF1F6" },
  Sales: { color: "#B54708", bg: "#FFF6ED" },
  Operations: { color: "#027A48", bg: "#ECFDF3" },
  Support: { color: "#7A2E0E", bg: "#FFF4ED" }
}

const employees = [
  {
    id: 1,
    name: "Hasan Mahmud",
    image: "https://randomuser.me/api/portraits/men/1.jpg",
    gender: "Male",
    nid: "1234567890",
    phone: "+8801712345678",
    department: "Engineering",
    basicSalary: 55000,
    spouse: {
      name: "Moushumi Akter",
      image: "https://randomuser.me/api/portraits/women/1.jpg"
    },
    children: [
      { name: "Nadia Hasan", image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Nadia" },
      { name: "Rafiq Hasan", image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Rafiq" },
      { name: "Lamia Hasan", image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Lamia" }
    ]
  },
  {
    id: 2,
    name: "Tanvir Ahmed",
    image: "https://randomuser.me/api/portraits/men/2.jpg",
    gender: "Male",
    nid: "2345678901",
    phone: "+8801812345678",
    department: "Finance",
    basicSalary: 48000,
    spouse: {
      name: "Sadia Rahman",
      image: "https://randomuser.me/api/portraits/women/2.jpg"
    },
    children: [
      { name: "Ayan Tanvir", image: "https://api.dicebear.com/7.x/adventurer/svg?seed=Ayan" }
    ]
  },
  {
    id: 3,
    name: "Farhan Islam",
    image: "https://randomuser.me/api/portraits/men/3.jpg",
    gender: "Male",
    nid: "3456789012",
    phone: "+8801912345678",
    department: "Marketing",
    basicSalary: 46000,
    spouse: {
      name: "Nusrat Jahan",
      image: "https://randomuser.me/api/portraits/women/3.jpg"
    },
    children: []
  },
  {
    id: 4,
    name: "Shihab Uddin",
    image: "https://randomuser.me/api/portraits/men/4.jpg",
    gender: "Male",
    nid: "4567890123",
    phone: "+8801612345678",
    department: "Sales",
    basicSalary: 50000,
    spouse: {
      name: "Jannat Ara",
      image: "https://randomuser.me/api/portraits/women/4.jpg"
    },
    children: []
  },
  {
    id: 5,
    name: "Rafiq Hasan",
    image: "https://randomuser.me/api/portraits/men/5.jpg",
    gender: "Male",
    nid: "5678901234",
    phone: "+8801512345678",
    department: "HR",
    basicSalary: 42000,
    spouse: null,
    children: []
  },
  {
    id: 6,
    name: "Jahid Karim",
    image: "https://randomuser.me/api/portraits/men/6.jpg",
    gender: "Male",
    nid: "6789012345",
    phone: "+8801412345678",
    department: "Operations",
    basicSalary: 47000,
    spouse: null,
    children: []
  },
  {
    id: 7,
    name: "Tasnim Akter",
    image: "https://randomuser.me/api/portraits/women/7.jpg",
    gender: "Female",
    nid: "7890123456",
    phone: "+8801312345678",
    department: "Support",
    basicSalary: 39000,
    spouse: null,
    children: []
  },
  {
    id: 8,
    name: "Sabbir Rahman",
    image: "https://randomuser.me/api/portraits/men/8.jpg",
    gender: "Male",
    nid: "8901234567",
    phone: "+8801212345678",
    department: "Engineering",
    basicSalary: 58000,
    spouse: null,
    children: []
  },
  {
    id: 9,
    name: "Mahin Chowdhury",
    image: "https://randomuser.me/api/portraits/men/9.jpg",
    gender: "Male",
    nid: "9012345678",
    phone: "+8801112345678",
    department: "Finance",
    basicSalary: 51000,
    spouse: null,
    children: []
  },
  {
    id: 10,
    name: "Nabila Haque",
    image: "https://randomuser.me/api/portraits/women/10.jpg",
    gender: "Female",
    nid: "0123456789",
    phone: "+8801012345678",
    department: "Marketing",
    basicSalary: 53000,
    spouse: null,
    children: []
  }
]

const EmployeeTable = () => {
    const navigate = useNavigate();
    return (
      <div className=" w-full h-full p-5 flex items-center justify-center">        
        <div className="overflow-x-auto max-w-350 rounded-lg border shadow-xl border-gray-200 bg-[#FFFFFC]">

        {/* Header */}
        <div className="flex items-center justify-between flex-wrap gap-4 p-4">

            <button className="border border-slate-400 text-slate-600 px-4 py-2 rounded-md text-sm font-medium hover:bg-gray-100 hover:text-black">
            Export table as pdf
            </button>

            
        <div className="relative">
            <div className="absolute inset-y-0 inset-s-0 flex items-center ps-3 pointer-events-none">
                <svg className="w-4 h-4 text-body" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24"><path stroke="currentColor" stroke-linecap="round" stroke-width="2" d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/></svg>
            </div>
            <input type="text" id="input-group-1" className="block w-full max-w-96 ps-9 pe-3 py-2 border border-slate-400 text-heading text-sm rounded-xl shadow-xs placeholder:text-body outline-none" placeholder="Search"/>
        </div>

        </div>

        {/* Table */}
        <table className=" min-w-300 text-sm text-left">

            <thead className="border-y border-slate-400 bg-gray-50">
            <tr>
                <th className="px-6 py-3 font-semibold">Name</th>
                <th className="px-6 py-3 font-semibold">NID</th>
                <th className="px-6 py-3 font-semibold">Department</th>
                <th className="px-6 py-3 font-semibold">Basic Salary</th>
                <th className="px-6 py-3 font-semibold">Spouse</th>
                <th className="px-6 py-3 font-semibold">Children</th>
                <th className="px-6 py-3 font-semibold">Action</th>
            </tr>
            </thead>

            <tbody>

            {employees.map((emp) => {

                const dept = departmentStyles[emp.department]

                return (
                <tr key={emp.id} onClick={()=>navigate(`/employee/${emp.id}`)} className=" hover:bg-gray-50 cursor-pointer">

                    {/* Name */}
                    <td className="px-6 py-4 flex items-center gap-3">

                    <img
                        src={emp.image}
                        className="w-10 h-10 rounded-full"
                    />

                    <div>
                        <div className="font-semibold">{emp.name}</div>
                        <div className="text-gray-500 text-sm">{emp.phone}</div>
                    </div>

                    </td>

                    {/* NID */}
                    <td className="px-6 py-4">{emp.nid}</td>

                    {/* Department */}
                    <td className="px-6 py-4">

                    <span
                        className="px-3 py-1 rounded-full text-xs font-medium border"
                        style={{
                        color: dept.color,
                        background: dept.bg,
                        borderColor: dept.color
                        }}
                    >
                        {emp.department}
                    </span>

                    </td>

                    {/* Salary */}
                    <td className="px-6 py-4 font-medium">
                    {emp.basicSalary.toLocaleString()} BDT
                    </td>

                    {/* Spouse */}
                    <td className="px-6 py-4">

                    {emp.spouse ? (
                        <div className="flex items-center gap-2">

                        <img
                            src={emp.spouse.image}
                            className="w-8 h-8 rounded-full"
                        />

                        <span>{emp.spouse.name}</span>

                        </div>
                    ) : "-"}

                    </td>

                    {/* Children */}
                    <td className="px-6 py-4">

                    <div className="flex -space-x-2">

                        {emp.children.slice(0,2).map((child, index) => (
                        <img
                            key={index}
                            src={child.image}
                            title={child.name}
                            className="w-8 h-8 rounded-full border border-slate-400"
                        />
                        ))}

                        {emp.children.length > 2 && (
                        <div className="w-8 h-8 rounded-full bg-gray-200 flex items-center justify-center text-xs font-medium border">
                            +{emp.children.length - 2}
                        </div>
                        )}

                    </div>

                    </td>

                        {/* Actions */}
                    <div className=" flex items-start justify-center">

                        <td className="flex gap-3 items-center justify-center ">

                            <Pencil className="w-5 h-5 cursor-pointer text-black  hover:text-blue-400"/>

                            <Trash2 className="w-5 h-5 cursor-pointer text-black hover:text-red-400"/>

                        </td>
                    </div>

                </tr>
                )
            })}

            </tbody>
        </table>
        </div>
      </div>
  )
}

export default EmployeeTable

