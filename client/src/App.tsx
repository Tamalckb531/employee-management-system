import { Routes, Route } from "react-router-dom"

import EmployeesPage from "./pages/EmployeesPage"
import EmployeeDetailsPage from "./pages/EmployeeDetailsPage"
import EmployeeFormPage from "./pages/EmployeeFormPage"
import NotFoundPage from "./pages/NotFoundPage"

function App() {
  return (
    <Routes>
      <Route path="/" element={<EmployeesPage />} />
      <Route path="/employee/:id" element={<EmployeeDetailsPage />} />
      <Route path="/form" element={<EmployeeFormPage />} />
       {/* fallback */}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}

export default App