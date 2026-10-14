// src/pages/AdminUsers.jsx
// Everyone in the studio. An Admin adds designers here and can switch a designer's login off.
import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import {
  activateUser,
  addDesigner,
  deactivateUser,
  getAdminUsers,
} from '../services/adminService'
import './dashboard.css'
import '../components/proposals.css'
import './admin.css'

const TABS = [
  { key: 'all', label: 'Everyone' },
  { key: 'Customer', label: 'Customers' },
  { key: 'Designer', label: 'Designers' },
  { key: 'staff', label: 'Admin & Manager' },
]

const emptyForm = { firstName: '', lastName: '', email: '', phoneNumber: '', password: '' }

// The API sends either { message } or a list of field errors
function apiMessage(err, fallback) {
  const data = err.response?.data
  if (data?.message) return data.message
  if (data?.errors) {
    const first = Object.values(data.errors).flat()[0]
    if (first) return first
  }
  return fallback
}

function AdminUsers() {
  const me = JSON.parse(localStorage.getItem('user') || 'null')
  const iAmAdmin = (me?.roles ?? []).includes('Admin')

  const [users, setUsers] = useState(null)   // null = still loading
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')
  const [busyId, setBusyId] = useState(null)
  // /admin/users?role=Designer opens straight on that tab
  const [params] = useSearchParams()
  const [tab, setTab] = useState(
    TABS.some((t) => t.key === params.get('role')) ? params.get('role') : 'all'
  )
  const [search, setSearch] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  const [adding, setAdding] = useState(false)
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        const list = await getAdminUsers()
        if (!cancelled) setUsers(list)
      } catch {
        if (!cancelled) {
          setError('Could not load the users.')
          setUsers((u) => u ?? [])
        }
      }
    }

    load()
    return () => { cancelled = true }
  }, [reloadKey])

  const setField = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }))

  const submitDesigner = async (e) => {
    e.preventDefault()
    setFormError('')

    if (!form.firstName.trim() || !form.lastName.trim()) return setFormError('Please enter the first and last name.')
    if (!/^\S+@\S+\.\S+$/.test(form.email.trim())) return setFormError('Please enter a valid email address.')
    if (form.password.length < 8 || !/[A-Za-z]/.test(form.password) || !/\d/.test(form.password)) {
      return setFormError('The password must be at least 8 characters with a letter and a number.')
    }

    setSaving(true)
    try {
      const created = await addDesigner({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
        phoneNumber: form.phoneNumber.trim() || null,
        password: form.password,
      })
      setNotice(`${created.name} was added as a designer. Share the email and password with them.`)
      setForm(emptyForm)
      setAdding(false)
      setReloadKey((k) => k + 1)
    } catch (err) {
      setFormError(apiMessage(err, 'Could not add the designer.'))
    } finally {
      setSaving(false)
    }
  }

  const toggle = async (u) => {
    const off = u.isActive
    const text = off
      ? `Switch off ${u.name}? They will not be able to log in until you switch them back on.`
      : `Switch ${u.name} back on?`
    if (!window.confirm(text)) return

    setBusyId(u.userId)
    setError('')
    setNotice('')
    try {
      await (off ? deactivateUser(u.userId) : activateUser(u.userId))
      setNotice(`${u.name} is now ${off ? 'switched off' : 'active again'}.`)
      setReloadKey((k) => k + 1)
    } catch (err) {
      setError(apiMessage(err, 'Could not change this designer.'))
    } finally {
      setBusyId(null)
    }
  }

  const isStaff = (roles) => roles.includes('Admin') || roles.includes('Manager')

  const list = (users ?? []).filter((u) => {
    if (tab === 'staff' ? !isStaff(u.roles) : tab !== 'all' && !u.roles.includes(tab)) return false
    const q = search.trim().toLowerCase()
    return !q || u.name.toLowerCase().includes(q) || u.email.toLowerCase().includes(q)
  })

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">Admin</p>
          <h1>Users</h1>
          <p className="pd-hero-sub">
            Customers sign up on their own. Designers are added by you, because they work for your studio.
          </p>
        </div>
        {iAmAdmin && !adding && (
          <button type="button" className="btn-c" onClick={() => { setAdding(true); setNotice('') }}>
            Add designer
          </button>
        )}
      </section>

      {notice && <p className="np-notice">{notice}</p>}
      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

      {adding && (
        <section className="pd-card">
          <div className="pd-card-head"><h2>Add a designer</h2></div>
          <form className="dw-form" onSubmit={submitDesigner} noValidate>
            <div className="dw-row">
              <div className="dw-field">
                <label htmlFor="ad-first">First name</label>
                <input id="ad-first" type="text" value={form.firstName} maxLength={100} onChange={setField('firstName')} />
              </div>
              <div className="dw-field">
                <label htmlFor="ad-last">Last name</label>
                <input id="ad-last" type="text" value={form.lastName} maxLength={100} onChange={setField('lastName')} />
              </div>
            </div>
            <div className="dw-row">
              <div className="dw-field">
                <label htmlFor="ad-email">Email (they log in with this)</label>
                <input id="ad-email" type="text" value={form.email} maxLength={100} onChange={setField('email')} />
              </div>
              <div className="dw-field">
                <label htmlFor="ad-phone">Phone (optional)</label>
                <input id="ad-phone" type="text" value={form.phoneNumber} maxLength={15} onChange={setField('phoneNumber')} />
              </div>
            </div>
            <div className="dw-field">
              <label htmlFor="ad-pass">Starting password</label>
              <input id="ad-pass" type="text" value={form.password} maxLength={72} onChange={setField('password')} />
              <span className="dw-hint">At least 8 characters, with a letter and a number. Share it with the designer.</span>
            </div>

            {formError && <p className="err" style={{ margin: 0 }}>{formError}</p>}

            <div className="pr-actions">
              <button type="submit" className="btn-c" disabled={saving}>
                {saving ? 'Adding…' : 'Add designer'}
              </button>
              <button
                type="button"
                className="pd-act"
                onClick={() => { setAdding(false); setForm(emptyForm); setFormError('') }}
              >
                Cancel
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="pd-card">
        <div className="ad-tools">
          <div className="ad-tabs">
            {TABS.map((t) => (
              <button
                type="button"
                key={t.key}
                className={`ad-tab ${tab === t.key ? 'active' : ''}`}
                onClick={() => setTab(t.key)}
              >
                {t.label}
              </button>
            ))}
          </div>
          <input
            type="search"
            placeholder="Search name or email"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Status</th>
                <th>Joined</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {list.map((u) => (
                <tr key={u.userId}>
                  <td className="pd-strong">
                    {u.roles.includes('Customer') ? (
                      <Link to={`/admin/customers/${u.userId}`}>{u.name}</Link>
                    ) : (
                      u.name
                    )}
                  </td>
                  <td>{u.email}</td>
                  <td>{u.roles.join(', ') || '—'}</td>
                  <td>{u.isActive ? 'Active' : 'Switched off'}</td>
                  <td className="pd-muted">{u.joined ? new Date(u.joined).toLocaleDateString() : '—'}</td>
                  <td>
                    <div className="pd-actions">
                      {u.roles.includes('Customer') && (
                        <Link className="pd-act" to={`/admin/customers/${u.userId}`}>View</Link>
                      )}
                      {iAmAdmin && u.roles.includes('Designer') && (
                        <button
                          type="button"
                          className="pd-act"
                          disabled={busyId === u.userId}
                          onClick={() => toggle(u)}
                        >
                          {u.isActive ? 'Switch off' : 'Switch on'}
                        </button>
                      )}
                      {!u.roles.includes('Customer') && !(iAmAdmin && u.roles.includes('Designer')) && (
                        <span className="pd-muted">—</span>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
              {list.length === 0 && (
                <tr>
                  <td colSpan={6} className="pd-muted">
                    {users === null ? 'Loading users…' : 'No users match.'}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <div className="np-actions">
        <Link to="/dashboard" className="np-cancel">Back to dashboard</Link>
      </div>
    </div>
  )
}

export default AdminUsers