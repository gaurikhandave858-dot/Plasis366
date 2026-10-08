import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import AuthLayout from '../components/AuthLayout'
import { Field } from '../components/ui'
import api from '../api'

function Login() {
  const navigate = useNavigate()

  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [loading, setLoading] = useState(false)

  const submit = async (e) => {
    e.preventDefault()

    setError('')
    setSuccess('')

    const d = new FormData(e.target)

    setLoading(true)

    try {
      const res = await api.post('/Auth/login', {
        email: d.get('email'),
        password: d.get('password'),
      })

      localStorage.setItem('token', res.data.token)
      localStorage.setItem('user', JSON.stringify(res.data))

      setSuccess(`Welcome back, ${res.data.firstName}!`)

      navigate('/dashboard')

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
    <AuthLayout tagline={<>Design your space.<br />Create your vision.</>}>
      <h2>Welcome back</h2>
      <p className="sub">Log in to continue your projects.</p>

      <form onSubmit={submit}>
        <Field
          id="email"
          label="Email"
          icon="mail"
          type="email"
          placeholder="you@example.com"
          required
        />

        <Field
          id="password"
          label="Password"
          icon="lock"
          type="password"
          placeholder="Your password"
          required
        />

        {error && <div className="err mb-3">{error}</div>}

        {success && <div className="ok mb-3">{success}</div>}

        <button
          className="btn-c w-100"
          type="submit"
          disabled={loading}
        >
          {loading ? 'Logging in…' : 'Log in'}
        </button>
      </form>

      <p className="alt">
        New here? <Link to="/register">Create an account</Link>
      </p>
    </AuthLayout>
  )
}

export default Login