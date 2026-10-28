// src/components/RequireRole.jsx
// Wrap routes with this to keep the wrong people out of a page.
//   <Route element={<RequireRole />}> ...             any logged-in user
//   <Route element={<RequireRole roles={['Designer']} />}> ...   only designers
import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { clearSession, getUser, hasValidSession } from '../auth'

function RequireRole({ roles }) {
  const location = useLocation()

  // Not logged in, or the token has run out: back to the login page
  if (!hasValidSession()) {
    clearSession()
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  // Logged in, but this page is for another role: back to their own dashboard
  const myRoles = getUser()?.roles ?? []
  if (roles && !roles.some((r) => myRoles.includes(r))) {
    return <Navigate to="/dashboard" replace />
  }

  return <Outlet />
}

export default RequireRole