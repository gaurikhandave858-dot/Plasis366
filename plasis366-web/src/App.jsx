import { Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import MainLayout from './layouts/MainLayout'
import Dashboard from './pages/Dashboard'
import NewProject from './pages/NewProject'
import DesignerProject from './pages/DesignerProject'
import AdminCustomer from './pages/AdminCustomer'
import AdminUsers from './pages/AdminUsers'

function App() {
  return (
    <Routes>

      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      <Route element={<MainLayout />}>
        <Route path="/dashboard" element={<Dashboard />} />

        {/* Customer */}
        <Route path="/projects/new" element={<NewProject />} />
        <Route path="/projects/:id" element={<NewProject />} />
        <Route path="/projects/:id/edit" element={<NewProject />} />

        {/* Designer */}
        <Route path="/designer/projects/:id" element={<DesignerProject />} />
        <Route path="/admin/users" element={<AdminUsers />} />
        <Route path="/admin/customers/:userId" element={<AdminCustomer />} />
      </Route>

    </Routes>
  )
}

export default App