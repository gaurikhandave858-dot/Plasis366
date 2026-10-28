import { Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import MainLayout from './layouts/MainLayout'
import RequireRole from './components/RequireRole'
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

        {/* Any logged-in user. The page itself shows the right dashboard for their role. */}
        <Route element={<RequireRole />}>
          <Route path="/dashboard" element={<Dashboard />} />
        </Route>

        {/* Customers only */}
        <Route element={<RequireRole roles={['Customer']} />}>
          <Route path="/projects/new" element={<NewProject />} />
          <Route path="/projects/:id" element={<NewProject />} />
          <Route path="/projects/:id/edit" element={<NewProject />} />
        </Route>

        {/* Designers only */}
        <Route element={<RequireRole roles={['Designer']} />}>
          <Route path="/designer/projects/:id" element={<DesignerProject />} />
        </Route>

        {/* Admin and Manager only */}
        <Route element={<RequireRole roles={['Admin', 'Manager']} />}>
          <Route path="/admin/users" element={<AdminUsers />} />
          <Route path="/admin/customers/:userId" element={<AdminCustomer />} />
        </Route>

      </Route>

      {/* Anything else */}
      <Route path="*" element={<Navigate to="/dashboard" replace />} />

    </Routes>
  )
}

export default App