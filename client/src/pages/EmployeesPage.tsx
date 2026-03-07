import AdminButton from "../components/EmployeeComp/AdminButton"
import EmployeeTable from "../components/EmployeeComp/EmployeeTable"

const EmployeesPage = () => {
  return (
    <div className="flex flex-col p-5 gap-5">
        <AdminButton/>      
        <EmployeeTable/>      
    </div>
  )
}

export default EmployeesPage