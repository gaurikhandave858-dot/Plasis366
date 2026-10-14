// src/pages/AdminDashboard.jsx
// The studio's overview for Admin and Manager: real numbers from the database.
// Clicking a card or a status shows those projects in the table below.
import { useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import StatusBadge from '../components/StatusBadge'
import {
  getAdminDashboard,
  getAdminDesigners,
  getAdminProjects,
  reassignProject,
} from '../services/adminService'
import './dashboard.css'
import '../components/proposals.css'
import './admin.css'

const ORDER = [
  'Draft', 'Submitted', 'Under Review', 'Design In Progress', 'Design Submitted',
  'Revision Requested', 'Approved', 'Completed',
]

function greeting() {
  const h = new Date().getHours()
  return h < 12 ? 'Good morning' : h < 17 ? 'Good afternoon' : 'Good evening'
}

function timeAgo(iso) {
  const mins = Math.floor((Date.now() - new Date(iso)) / 60000)
  if (mins < 60) return `${Math.max(mins, 1)} min ago`
  const hrs = Math.floor(mins / 60)
  if (hrs < 24) return `${hrs} hour${hrs > 1 ? 's' : ''} ago`
  const days = Math.floor(hrs / 24)
  if (days < 30) return `${days} day${days > 1 ? 's' : ''} ago`
  return new Date(iso).toLocaleDateString()
}

// A designer can be swapped while one of these statuses is on the project
const REASSIGNABLE = ['Under Review', 'Design In Progress', 'Design Submitted', 'Revision Requested', 'Approved']

// A filter is { kind: 'all' } | { kind: 'waiting' } | { kind: 'status', value: 'Approved' }
const keyOf = (f) => (f ? (f.kind === 'status' ? `status:${f.value}` : f.kind) : '')
const paramsOf = (f) =>
  f.kind === 'waiting' ? { waiting: true } : f.kind === 'status' ? { status: f.value } : {}
const titleOf = (f) =>
  !f ? 'Recent activity'
    : f.kind === 'all' ? 'All projects'
    : f.kind === 'waiting' ? 'Waiting for a designer'
    : f.value

function AdminDashboard() {
  const user = JSON.parse(localStorage.getItem('user') || 'null')
  const isAdmin = (user?.roles ?? []).includes('Admin')

  const [data, setData] = useState(null)       // null = still loading
  const [error, setError] = useState('')
  const [filter, setFilter] = useState(null)   // null = the 10 most recent projects
  const [list, setList] = useState({ key: '', rows: [] })
  const tableRef = useRef(null)
  const [reloadKey, setReloadKey] = useState(0)

  // Reassigning a project to another designer
  const [reassigning, setReassigning] = useState(null)   // the project row being changed
  const [designers, setDesigners] = useState([])
  const [pick, setPick] = useState('')
  const [saving, setSaving] = useState(false)
  const [notice, setNotice] = useState('')

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        const d = await getAdminDashboard()
        if (!cancelled) setData(d)
      } catch (err) {
        console.error('Admin dashboard load failed', err)
        if (!cancelled) setError('Could not load the dashboard. Is the API running?')
      }
    }

    load()
    return () => { cancelled = true }
  }, [reloadKey])

  // Load the project list whenever a card or status is selected
  useEffect(() => {
    if (!filter) return
    let cancelled = false
    const key = keyOf(filter)

    async function loadList() {
      try {
        const rows = await getAdminProjects(paramsOf(filter))
        if (!cancelled) setList({ key, rows })
      } catch {
        if (!cancelled) {
          setError('Could not load those projects.')
          setList({ key, rows: [] })
        }
      }
    }

    loadList()
    return () => { cancelled = true }
  }, [filter, reloadKey])

  const startReassign = async (row) => {
    setError('')
    setNotice('')
    setPick('')
    setReassigning(row)
    try {
      setDesigners(await getAdminDesigners())
    } catch {
      setError('Could not load the designers.')
    }
  }

  const confirmReassign = async () => {
    if (!pick) return setError('Please choose a designer.')
    setSaving(true)
    setError('')
    try {
      const res = await reassignProject(reassigning.projectId, Number(pick))
      setNotice(`${reassigning.projectCode} now belongs to ${res.designerName}. The designer and the customer were told.`)
      setReassigning(null)
      setReloadKey((k) => k + 1)
    } catch (err) {
      setError(err.response?.data?.message || 'Could not reassign the project.')
    } finally {
      setSaving(false)
    }
  }

  const choose = (next) => {
    const same = filter && keyOf(filter) === keyOf(next)
    setError('')
    setFilter(same ? null : next)
    if (!same) {
      // Bring the table into view, because it can be below the cards
      setTimeout(() => tableRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' }), 50)
    }
  }

  const loading = data === null && !error
  const num = (v) => (loading || v == null ? '–' : v)
  const isActive = (f) => filter && keyOf(filter) === keyOf(f)

  // Known statuses first, in flow order; anything unexpected goes at the end
  const byStatus = [...(data?.byStatus ?? [])].sort((a, b) => {
    const ia = ORDER.indexOf(a.status)
    const ib = ORDER.indexOf(b.status)
    return (ia === -1 ? 99 : ia) - (ib === -1 ? 99 : ib)
  })
  const total = Math.max(1, byStatus.reduce((sum, s) => sum + s.count, 0))

  const listLoading = filter && list.key !== keyOf(filter)
  const rows = filter ? (listLoading ? [] : list.rows) : (data?.recent ?? [])

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">{greeting()}</p>
          <h1>Welcome, {user?.firstName ?? 'there'}</h1>
          <p className="pd-hero-sub">
            See how your studio is doing, and manage who works in it.
          </p>
        </div>
        <Link to="/admin/users" className="btn-c" style={{ textDecoration: 'none' }}>
          {isAdmin ? 'Manage users' : 'View users'}
        </Link>
      </section>

      {notice && <p className="np-notice">{notice}</p>}
      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

      <section className="pd-stats">
        <button
          type="button"
          className={`pd-stat pd-stat-btn ${isActive({ kind: 'all' }) ? 'active' : ''}`}
          aria-pressed={Boolean(isActive({ kind: 'all' }))}
          onClick={() => choose({ kind: 'all' })}
        >
          <div>
            <div className="pd-stat-value">{num(data?.totalProjects)}</div>
            <div className="pd-stat-label">Projects</div>
          </div>
        </button>

        <button
          type="button"
          className={`pd-stat pd-stat-btn ${isActive({ kind: 'waiting' }) ? 'active' : ''}`}
          aria-pressed={Boolean(isActive({ kind: 'waiting' }))}
          onClick={() => choose({ kind: 'waiting' })}
        >
          <div>
            <div className="pd-stat-value">{num(data?.waiting)}</div>
            <div className="pd-stat-label">Waiting for a designer</div>
          </div>
        </button>

        <Link to="/admin/users?role=Designer" className="pd-stat pd-stat-btn" style={{ textDecoration: 'none' }}>
          <div>
            <div className="pd-stat-value">{num(data?.designers)}</div>
            <div className="pd-stat-label">Designers</div>
          </div>
        </Link>

        <Link to="/admin/users?role=Customer" className="pd-stat pd-stat-btn" style={{ textDecoration: 'none' }}>
          <div>
            <div className="pd-stat-value">{num(data?.customers)}</div>
            <div className="pd-stat-label">Customers</div>
          </div>
        </Link>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Projects by status</h2></div>
        {byStatus.length === 0 ? (
          <p className="pd-muted" style={{ margin: 0 }}>
            {loading ? 'Loading…' : 'No projects yet.'}
          </p>
        ) : (
          <div className="ad-tiles">
            {byStatus.map((s) => {
              const f = { kind: 'status', value: s.status }
              return (
                <button
                  type="button"
                  key={s.status}
                  className={`ad-tile ad-tile-btn ${isActive(f) ? 'active' : ''}`}
                  aria-pressed={Boolean(isActive(f))}
                  onClick={() => choose(f)}
                >
                  <StatusBadge status={s.status} />
                  <div className="ad-tile-count">{s.count}</div>
                  <div className="ad-tile-share">
                    {Math.round((s.count / total) * 100)}% of all projects
                  </div>
                  <div className="ad-bar"><span style={{ width: `${(s.count / total) * 100}%` }} /></div>
                </button>
              )
            })}
          </div>
        )}
      </section>

      <section className="pd-card" ref={tableRef}>
        <div className="pd-card-head">
          <h2>{titleOf(filter)} ({listLoading ? '…' : rows.length})</h2>
          {filter && (
            <button type="button" className="pd-act" onClick={() => setFilter(null)}>
              Show recent activity
            </button>
          )}
        </div>
        {reassigning && (
          <div className="pr-item" style={{ marginBottom: '1rem' }}>
            <strong>Give {reassigning.projectCode} to another designer</strong>
            <div className="dw-field">
              <label htmlFor="ad-pick">New designer</label>
              <select
                id="ad-pick"
                value={pick}
                onChange={(e) => setPick(e.target.value)}
                style={{ font: 'inherit', color: 'inherit', background: 'transparent', padding: '0.55rem', borderRadius: 6, border: '1px solid rgba(128,128,128,0.45)' }}
              >
                <option value="">Choose a designer…</option>
                {designers
                  .filter((d) => d.userId !== reassigning.designerUserId)
                  .map((d) => (
                    <option key={d.userId} value={d.userId}>
                      {d.name} ({d.openProjects} open project{d.openProjects === 1 ? '' : 's'})
                    </option>
                  ))}
              </select>
              <span className="dw-hint">
                The new designer, the old designer and the customer will each get a notification.
              </span>
            </div>
            <div className="pr-actions">
              <button type="button" className="btn-c" disabled={saving} onClick={confirmReassign}>
                {saving ? 'Saving…' : 'Reassign'}
              </button>
              <button type="button" className="pd-act" onClick={() => setReassigning(null)}>
                Cancel
              </button>
            </div>
          </div>
        )}

        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Customer</th>
                <th>Designer</th>
                <th>Status</th>
                <th>Updated</th>
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.projectId}>
                  <td className="pd-strong">
                    {r.projectName}
                    <div className="pd-muted">{r.projectCode}</div>
                  </td>
                  <td>{r.customerName || '—'}</td>
                  <td>{r.designerName || 'Not assigned'}</td>
                  <td><StatusBadge status={r.status} /></td>
                  <td className="pd-muted">{timeAgo(r.lastUpdated)}</td>
                  {isAdmin && (
                    <td>
                      <div className="pd-actions">
                        {r.designerUserId && REASSIGNABLE.includes(r.status) ? (
                          <button type="button" className="pd-act" onClick={() => startReassign(r)}>
                            Reassign
                          </button>
                        ) : (
                          <span className="pd-muted">—</span>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
              {rows.length === 0 && (
                <tr>
                  <td colSpan={isAdmin ? 6 : 5} className="pd-muted">
                    {loading || listLoading ? 'Loading…' : 'No projects here.'}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  )
}

export default AdminDashboard