import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import AuthLayout from '../components/AuthLayout'
import { Field } from '../components/ui'
import api from '../api'

function Register() {
  const navigate = useNavigate()

  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [role, setRole] = useState('Customer')

  const submit = async (e) => {
    e.preventDefault()
    setError('')

    const d = new FormData(e.target)

    if (d.get('password').length < 6) {
      return setError('Password must be at least 6 characters.')
    }

    if (d.get('password') !== d.get('confirmPassword')) {
      return setError('Passwords do not match.')
    }

    setLoading(true)

    try {
      await api.post('/Auth/register', {
        role: role,
        firstName: d.get('firstName'),
        lastName: d.get('lastName'),
        email: d.get('email'),
        phoneNumber: d.get('phone'),
        address: d.get('address'),
        password: d.get('password'),
      })

      navigate('/login')
    } catch (err) {
      setError(
        err.response?.data?.message ||
        'Could not reach the server. Is the API running?'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <AuthLayout
      tagline={
        <>
          Create your account.
          <br />
          Start designing your space.
        </>
      }
    >
      <h2>Create account</h2>

      <p className="sub">It only takes a minute.</p>

      <form onSubmit={submit}>

        {/* Account Type */}
        <div className="mb-3">
          <label className="form-label">
            Register as
          </label>

          <div className="two">

            <button
              type="button"
              className={`role-option ${role === 'Customer' ? 'selected' : ''}`}
              onClick={() => setRole('Customer')}
            >
              Customer
            </button>

            <button
              type="button"
              className={`role-option ${role === 'Designer' ? 'selected' : ''}`}
              onClick={() => setRole('Designer')}
            >
              Designer
            </button>

          </div>
        </div>

        <div className="two">
          <Field
            id="firstName"
            label="First name"
            icon="user"
            placeholder="First name"
            required
          />

          <Field
            id="lastName"
            label="Last name"
            icon="user"
            placeholder="Last name"
            required
          />
        </div>

        <Field
          id="email"
          label="Email"
          icon="mail"
          type="email"
          placeholder="you@example.com"
          required
        />

        <div className="two">
          <Field
            id="phone"
            label="Phone"
            icon="phone"
            type="tel"
            placeholder="Phone number"
          />

          <Field
            id="address"
            label="Address"
            icon="home"
            placeholder="City or address"
            required
          />
        </div>

        <div className="two">
          <Field
            id="password"
            label="Password"
            icon="lock"
            type="password"
            placeholder="Password"
            required
          />

          <Field
            id="confirmPassword"
            label="Confirm"
            icon="lock"
            type="password"
            placeholder="Confirm password"
            required
          />
        </div>

        {error && (
          <div className="err mb-3">
            {error}
          </div>
        )}

        <button
          className="btn-c w-100"
          type="submit"
          disabled={loading}
        >
          {loading ? 'Creating account…' : 'Create account'}
        </button>

      </form>

      <p className="alt">
        Already have an account?{' '}
        <Link to="/login">Log in</Link>
      </p>

    </AuthLayout>
  )
}

export default Register