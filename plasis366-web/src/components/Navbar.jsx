import { Link } from 'react-router-dom'

function Navbar() {
  return (
    <nav className="navbar navbar-expand-lg">
      <div className="container">

        <Link to="/" className="logo">
          Plasis366
        </Link>

        <div className="d-flex align-items-center gap-4">

          <Link to="/" className="nav-link">
            Home
          </Link>

          <Link to="/login" className="nav-link">
            Login
          </Link>

          <Link to="/register" className="btn-c">
            Register
          </Link>

        </div>

      </div>
    </nav>
  )
}

export default Navbar