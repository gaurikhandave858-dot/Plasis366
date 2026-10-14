import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import StatusBadge from '../components/StatusBadge'
import { getAvailable, getMine } from '../services/designerService'
import './dashboard.css'

// Each card is a list. "Available" comes from the studio pool, the rest are the designer's own projects.
const GROUPS = {
  available: { label: 'Available', title: 'Available projects', hint: 'Submitted projects waiting for a designer.' },
  progress: { label: 'In progress', title: 'In progress', statuses: ['Under Review', 'Design In Progress'] },
  withCustomer: {
    label: 'With customer',
    title: 'With customer',
    statuses: ['Design Submitted', 'Customer Review', 'Revision Requested'],
  },
  completed: { label: 'Completed', title: 'Completed', statuses: ['Approved', 'Completed'] },
}

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

function budgetText(min, max) {
  const f = (v) => `₹${Number(v).toLocaleString('en-IN')}`
  if (min != null && max != null) return `${f(min)} – ${f(max)}`
  if (max != null) return `up to ${f(max)}`
  if (min != null) return `from ${f(min)}`
  return '—'
}

function DesignerDashboard() {
  const user = JSON.parse(localStorage.getItem('user') || 'null')

  const [available, setAvailable] = useState(null)   // null = still loading
  const [mine, setMine] = useState(null)
  const [group, setGroup] = useState('available')    // which card is selected
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([getAvailable(), getMine()])
      .then(([a, m]) => {
        setAvailable(a)
        setMine(m)
      })
      .catch((err) => {
        console.error('Designer dashboard load failed', err)
        setError('Could not load your projects. Is the API running?')
        setAvailable((a) => a ?? [])
        setMine((m) => m ?? [])
      })
  }, [])

  const loading = available === null || mine === null
  const av = available ?? []
  const my = mine ?? []

  const inGroup = (key) => my.filter((r) => GROUPS[key].statuses.includes(r.status))

  const stats = [
    { key: 'available', value: av.length },
    { key: 'progress', value: inGroup('progress').length },
    { key: 'withCustomer', value: inGroup('withCustomer').length },
    { key: 'completed', value: inGroup('completed').length },
  ]

  // null group = every project the designer has taken
  const rows = group === 'available' ? av : group ? inGroup(group) : my
  const title = group ? GROUPS[group].title : 'All my projects'

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">{greeting()}</p>
          <h1>Welcome, {user?.firstName ?? 'there'}</h1>
          <p className="pd-hero-sub">
            Pick up new projects, review requirements and move your designs forward.
          </p>
        </div>
      </section>

      <section className="pd-stats">
        {stats.map((s) => {
          const active = group === s.key
          return (
            <button
              type="button"
              key={s.key}
              className={`pd-stat pd-stat-btn ${active ? 'active' : ''}`}
              aria-pressed={active}
              onClick={() => setGroup(active ? null : s.key)}
            >
              <div>
                <div className="pd-stat-value">{loading ? '–' : s.value}</div>
                <div className="pd-stat-label">{GROUPS[s.key].label}</div>
              </div>
            </button>
          )
        })}
      </section>

      <section className="pd-card">
        <div className="pd-card-head">
          <h2>{title} ({rows.length})</h2>
          {group && (
            <button type="button" className="pd-act" onClick={() => setGroup(null)}>
              All my projects
            </button>
          )}
        </div>

        {group === 'available' && (
          <p className="pd-muted" style={{ marginTop: 0 }}>{GROUPS.available.hint}</p>
        )}
        {error && <p className="err" style={{ marginTop: 0 }}>{error}</p>}

        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Customer</th>
                <th>Property</th>
                <th>Budget</th>
                <th>Status</th>
                <th>Updated</th>
                <th>Actions</th>
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
                  <td>{r.propertyType || '—'}</td>
                  <td>{budgetText(r.budgetMin, r.budgetMax)}</td>
                  <td><StatusBadge status={r.status} /></td>
                  <td className="pd-muted">{timeAgo(r.lastUpdated)}</td>
                  <td>
                    <div className="pd-actions">
                      <Link className="pd-act" to={`/designer/projects/${r.projectId}`}>
                        {group === 'available' ? 'View & take' : 'Open'}
                      </Link>
                    </div>
                  </td>
                </tr>
              ))}
              {rows.length === 0 && (
                <tr>
                  <td colSpan={7} className="pd-muted">
                    {loading
                      ? 'Loading projects…'
                      : group === 'available'
                        ? 'No projects are waiting for a designer right now.'
                        : 'Nothing here yet.'}
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

export default DesignerDashboard