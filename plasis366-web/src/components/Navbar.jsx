import { Link, useNavigate } from 'react-router-dom'
import NotificationBell from './NotificationBell'

function Navbar() {
  const navigate = useNavigate()

  // Logged in means the login page stored a token and the user's details
  const user = JSON.parse(localStorage.getItem('user') || 'null')
  const loggedIn = Boolean(user && localStorage.getItem('token'))

  // Notifications belong to the people who work on projects: customers and designers.
  // An admin or manager only oversees the studio, so they get no bell.
  const roles = user?.roles ?? []
  const showBell = roles.includes('Customer') || roles.includes('Designer')

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    navigate('/login', { replace: true })
  }

  return (
    <nav className="navbar navbar-expand-lg">
      <div className="container">

        <Link to={loggedIn ? '/dashboard' : '/'} className="logo">
          Plasis366
        </Link>

        <div className="d-flex align-items-center gap-4">

          {loggedIn ? (
            <>
              {showBell && <NotificationBell />}

              <button type="button" className="btn-c" onClick={logout}>
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/" className="nav-link">
                Home
              </Link>

              <Link to="/login" className="nav-link">
                Login
              </Link>

              <Link to="/register" className="btn-c">
                Register
              </Link>
            </>
          )}

        </div>

      </div>
    </nav>
  )
}

export default Navbar