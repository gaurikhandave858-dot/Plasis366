// src/pages/AdminCustomer.jsx
// One customer, seen by the admin: contact details and their projects (read-only).
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import StatusBadge from '../components/StatusBadge'
import { activateUser, deactivateUser, getAdminCustomer } from '../services/adminService'
import './dashboard.css'
import './designerproject.css'

const show = (v) => (v === null || v === undefined || v === '' ? '—' : v)

function timeAgo(iso) {
  const mins = Math.floor((Date.now() - new Date(iso)) / 60000)
  if (mins < 60) return `${Math.max(mins, 1)} min ago`
  const hrs = Math.floor(mins / 60)
  if (hrs < 24) return `${hrs} hour${hrs > 1 ? 's' : ''} ago`
  const days = Math.floor(hrs / 24)
  if (days < 30) return `${days} day${days > 1 ? 's' : ''} ago`
  return new Date(iso).toLocaleDateString()
}

function AdminCustomer() {
  const { userId } = useParams()
  const me = JSON.parse(localStorage.getItem('user') || 'null')
  const iAmAdmin = (me?.roles ?? []).includes('Admin')

  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')
  const [busy, setBusy] = useState(false)
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        const d = await getAdminCustomer(userId)
        if (!cancelled) setData(d)
      } catch (err) {
        if (!cancelled) {
          setData(null)
          setError(err.response?.status === 404 ? 'Customer not found.' : 'Could not load the customer.')
        }
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => { cancelled = true }
  }, [userId, reloadKey])

  const toggle = async () => {
    const off = data.user.isActive
    const text = off
      ? `Switch off ${data.user.name}? They will not be able to log in until you switch them back on.`
      : `Switch ${data.user.name} back on?`
    if (!window.confirm(text)) return

    setBusy(true)
    setError('')
    setNotice('')
    try {
      await (off ? deactivateUser(userId) : activateUser(userId))
      setNotice(off ? 'The account is switched off.' : 'The account is active again.')
      setReloadKey((k) => k + 1)
    } catch (err) {
      setError(err.response?.data?.message || 'Could not change the account.')
    } finally {
      setBusy(false)
    }
  }

  if (loading) return <div className="pd"><p className="sub">Loading…</p></div>

  if (!data) {
    return (
      <div className="pd">
        <p className="err">{error || 'Customer not found.'}</p>
        <Link to="/admin/users">Back to users</Link>
      </div>
    )
  }

  const { user, address, projects } = data

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">Customer</p>
          <h1>{user.name}</h1>
          <p className="pd-hero-sub">
            {user.isActive ? 'Active account.' : 'This account is switched off. They cannot log in.'}
          </p>
        </div>
        {iAmAdmin && (
          <button type="button" className="btn-c" disabled={busy} onClick={toggle}>
            {busy ? 'Saving…' : user.isActive ? 'Switch off account' : 'Switch on account'}
          </button>
        )}
      </section>

      {notice && <p className="np-notice">{notice}</p>}
      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

      <section className="pd-card">
        <div className="pd-card-head"><h2>Details</h2></div>
        <div className="dp-grid">
          <div className="dp-item"><div className="dp-label">Email</div><div className="dp-value">{show(user.email)}</div></div>
          <div className="dp-item"><div className="dp-label">Phone</div><div className="dp-value">{show(user.phone)}</div></div>
          <div className="dp-item">
            <div className="dp-label">Joined</div>
            <div className="dp-value">{user.joined ? new Date(user.joined).toLocaleDateString() : '—'}</div>
          </div>
          <div className="dp-item full"><div className="dp-label">Address</div><div className="dp-value">{show(address)}</div></div>
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Projects ({projects.length})</h2></div>
        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Designer</th>
                <th>Status</th>
                <th>Updated</th>
              </tr>
            </thead>
            <tbody>
              {projects.map((p) => (
                <tr key={p.projectId}>
                  <td className="pd-strong">
                    {p.projectName}
                    <div className="pd-muted">{p.projectCode}</div>
                  </td>
                  <td>{p.designerName || 'Not assigned'}</td>
                  <td><StatusBadge status={p.status} /></td>
                  <td className="pd-muted">{timeAgo(p.lastUpdated)}</td>
                </tr>
              ))}
              {projects.length === 0 && (
                <tr><td colSpan={4} className="pd-muted">This customer has no projects yet.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <div className="np-actions">
        <Link to="/admin/users" className="np-cancel">Back to users</Link>
      </div>
    </div>
  )
}

export default AdminCustomer