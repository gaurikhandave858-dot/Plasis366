import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import StatusBadge from '../components/StatusBadge'
import api from '../api'
import { deleteProject } from '../services/projectService'
import './dashboard.css'

const FLOW = [
  'Draft', 'Submitted', 'Under Review', 'Design In Progress',
  'Design Submitted', 'Customer Review', 'Completed',
]

// Customer view: text only. The numbers and rows come from the API.
const CUSTOMER = {
  subtitle: 'Track your interior design projects in one place.',
  action: { label: '+ New Project', to: '/projects/new' },
  tableTitle: 'My projects',
  partyLabel: 'Designer',
}

// Designer view: still sample data for now
const DESIGNER = {
  subtitle: 'Review requirements and move your projects forward.',
  action: null,
  stats: [
    { label: 'Assigned projects', value: 6 },
    { label: 'Pending reviews', value: 2 },
    { label: 'Designs in progress', value: 3 },
    { label: 'Completed', value: 1 },
  ],
  tableTitle: 'Assigned projects',
  partyLabel: 'Customer',
  rows: [
    { name: 'Modern 2BHK Living Room', property: 'Flat, Pune', party: 'Aarav Mehta', status: 'Design In Progress', updated: '2 days ago' },
    { name: 'Master Bedroom Makeover', property: 'Villa, Mumbai', party: 'Neha Kulkarni', status: 'Under Review', updated: '1 day ago' },
    { name: 'Kids Room Design', property: 'Flat, Nashik', party: 'Rohan Patil', status: 'Customer Review', updated: '4 days ago' },
  ],
}

const EMPTY = {
  stats: { total: 0, drafts: 0, designInProgress: 0, completed: 0 },
  recent: [],
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

function Dashboard() {
  const user = JSON.parse(localStorage.getItem('user') || 'null')
  const roles = user?.roles ?? []
  const isStaff =
    roles.includes('Designer') || roles.includes('Manager') || roles.includes('Admin')

  const [live, setLive] = useState(EMPTY)        // stat numbers
  const [projects, setProjects] = useState(null) // full project list (null = still loading)
  const [error, setError] = useState('')
  const [toDelete, setToDelete] = useState(null)
  const [deleting, setDeleting] = useState(false)

  const load = useCallback(
    () =>
      Promise.all([api.get('/MyProjects/dashboard'), api.get('/MyProjects')])
        .then(([dash, list]) => {
          setLive(dash.data)
          setProjects(list.data)
        })
        .catch((err) => {
          console.error('Dashboard load failed', err)
          setError('Could not load your projects. Is the API running?')
          setProjects((p) => p ?? [])
        }),
    []
  )

  useEffect(() => {
    if (isStaff) return
    load()
  }, [isStaff, load])

  // Escape closes the delete confirmation
  useEffect(() => {
    if (!toDelete) return
    const onKey = (e) => {
      if (e.key === 'Escape' && !deleting) setToDelete(null)
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [toDelete, deleting])

  const confirmDelete = async () => {
    setDeleting(true)
    setError('')
    try {
      await deleteProject(toDelete.id)
      await load()   // refresh the table and the stat cards
    } catch (err) {
      setError(err.response?.data?.message || 'Could not delete the project.')
    } finally {
      setDeleting(false)
      setToDelete(null)
    }
  }

  const loading = !isStaff && projects === null

  const view = isStaff
    ? { ...DESIGNER, showActions: false }
    : {
        ...CUSTOMER,
        showActions: true,
        stats: [
          { label: 'My projects', value: live.stats.total },
          { label: 'Drafts', value: live.stats.drafts },
          { label: 'Design in progress', value: live.stats.designInProgress },
          { label: 'Completed', value: live.stats.completed },
        ],
        rows: (projects ?? []).map((r) => ({
          id: r.projectId,
          name: r.name,
          property: [r.propertyType, r.city].filter(Boolean).join(', ') || '—',
          party: r.designer || 'Not assigned yet',
          status: r.status,
          updated: timeAgo(r.updated),
        })),
      }

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">{greeting()}</p>
          <h1>Welcome, {user?.firstName ?? 'there'}</h1>
          <p className="pd-hero-sub">{view.subtitle}</p>
        </div>
        {view.action && (
          <Link className="pd-hero-btn" to={view.action.to}>
            {view.action.label}
          </Link>
        )}
      </section>

      <section className="pd-stats">
        {view.stats.map((s) => (
          <div className="pd-stat" key={s.label}>
            <div>
              <div className="pd-stat-value">{s.value}</div>
              <div className="pd-stat-label">{s.label}</div>
            </div>
          </div>
        ))}
      </section>

      <section className="pd-card">
        <div className="pd-card-head">
          <h2>{view.tableTitle}</h2>
        </div>

        {error && <p className="err" style={{ marginTop: 0 }}>{error}</p>}

        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Project</th>
                <th>Property</th>
                <th>{view.partyLabel}</th>
                <th>Status</th>
                <th>Updated</th>
                {view.showActions && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {view.rows.map((r) => (
                <tr key={r.id ?? r.name}>
                  <td className="pd-strong">{r.name}</td>
                  <td>{r.property}</td>
                  <td>{r.party}</td>
                  <td><StatusBadge status={r.status} /></td>
                  <td className="pd-muted">{r.updated}</td>
                  {view.showActions && (
                    <td>
                      <div className="pd-actions">
                        {r.status === 'Draft' ? (
                          <>
                            <Link className="pd-act" to={`/projects/${r.id}/edit`}>Edit</Link>
                            <button type="button" className="pd-act danger" onClick={() => setToDelete(r)}>
                              Delete
                            </button>
                          </>
                        ) : (
                          <Link className="pd-act" to={`/projects/${r.id}`}>View</Link>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
              {view.rows.length === 0 && (
                <tr>
                  <td colSpan={view.showActions ? 6 : 5} className="pd-muted">
                    {loading ? 'Loading your projects…' : 'No projects yet.'}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head">
          <h2>Project journey</h2>
        </div>
        <div className="pd-flow">
          {FLOW.map((step, i) => (
            <div className="pd-flow-step" key={step}>
              <span className="pd-flow-dot">{i + 1}</span>
              <span className="pd-flow-text">{step}</span>
            </div>
          ))}
        </div>
      </section>

      {toDelete && (
        <div className="pd-modal-back" onClick={() => !deleting && setToDelete(null)}>
          <div
            className="pd-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="del-title"
            onClick={(e) => e.stopPropagation()}
          >
            <h3 id="del-title">Delete this draft?</h3>
            <p>
              "{toDelete.name}" and its uploaded files will be removed. This can't be undone from the app.
            </p>
            <div className="pd-modal-btns">
              <button type="button" className="pd-act" onClick={() => setToDelete(null)} disabled={deleting}>
                Cancel
              </button>
              <button type="button" className="btn-c danger" onClick={confirmDelete} disabled={deleting}>
                {deleting ? 'Deleting…' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default Dashboard