import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import StatusBadge from '../components/StatusBadge'
import DesignerWork from '../components/DesignerWork'
import ProposalList from '../components/ProposalList'
import {
  getDesignerProject,
  listDesignerAttachments,
  openDesignerAttachment,
  takeProject,
} from '../services/designerService'
import { getProposals } from '../services/proposalService'
import './dashboard.css'
import './designerproject.css'

const show = (v) => (v === null || v === undefined || v === '' ? '—' : v)
const money = (v) => (v == null ? null : `₹${Number(v).toLocaleString('en-IN')}`)

function budgetText(min, max) {
  if (min != null && max != null) return `${money(min)} – ${money(max)}`
  if (max != null) return `Up to ${money(max)}`
  if (min != null) return `From ${money(min)}`
  return '—'
}

function sizeText(bytes) {
  if (bytes == null) return ''
  return bytes < 1024 * 1024
    ? `${Math.max(1, Math.round(bytes / 1024))} KB`
    : `${(bytes / 1024 / 1024).toFixed(1)} MB`
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

function Item({ label, value, full }) {
  return (
    <div className={`dp-item ${full ? 'full' : ''}`}>
      <div className="dp-label">{label}</div>
      <div className="dp-value">{show(value)}</div>
    </div>
  )
}

function DesignerProject() {
  const { id } = useParams()

  const [project, setProject] = useState(null)
  const [files, setFiles] = useState([])
  const [proposals, setProposals] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')
  const [taking, setTaking] = useState(false)

  // Changing this number makes the page load the project again (used after taking it)
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let cancelled = false

    async function init() {
      try {
        const p = await getDesignerProject(id)
        const f = await listDesignerAttachments(id)
        // Only the assigned designer has proposals to show
        const pr = p.isMine ? await getProposals(id) : { items: [] }
        if (cancelled) return
        setProject(p)
        setFiles(f)
        setProposals(pr.items)
      } catch (err) {
        if (cancelled) return
        setProject(null)
        setError(
          err.response?.status === 404
            ? 'This project was not found, or another designer has already taken it.'
            : 'Could not load the project. Is the API running?'
        )
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    init()
    return () => { cancelled = true }
  }, [id, reloadKey])

  const take = async () => {
    if (!window.confirm('Take this project? It will move to your list and no other designer will be able to pick it up.')) {
      return
    }
    setTaking(true)
    setError('')
    try {
      await takeProject(id)
      setNotice('You have taken this project. It is now under review.')
      setReloadKey((k) => k + 1)
    } catch (err) {
      setError(err.response?.data?.message || 'Could not take the project.')
      setReloadKey((k) => k + 1)
    } finally {
      setTaking(false)
    }
  }

  const openFile = async (f) => {
    try {
      await openDesignerAttachment(id, f.attachmentId)
    } catch {
      setError('Could not open the file.')
    }
  }

  if (loading) {
    return <div className="pd"><p className="sub">Loading…</p></div>
  }

  if (!project) {
    return (
      <div className="pd">
        <p className="err">{error || 'Project not found.'}</p>
        <Link to="/dashboard">Back to dashboard</Link>
      </div>
    )
  }

  const canTake = !project.isMine && project.status === 'Submitted'
  const r = project.requirements || {}

  return (
    <div className="pd">
      <section className="pd-hero">
        <div>
          <p className="pd-hero-small">{project.projectCode}</p>
          <h1>{project.projectName}</h1>
          <p className="pd-hero-sub">
            For {show(project.customerName)} ·{' '}
            {project.isMine ? 'This project is assigned to you.' : 'Waiting for a designer.'}
          </p>
        </div>
        <StatusBadge status={project.status} />
      </section>

      {notice && <p className="np-notice">{notice}</p>}
      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

      {project.isMine && project.lastCustomerUpdate && (
        <section className="dp-update">
          <strong>Updated by the customer</strong>
          <span>
            {project.lastCustomerUpdate.text.replace('Customer updated: ', 'Changed: ')}.{' '}
            <span className="pd-muted">{timeAgo(project.lastCustomerUpdate.when)}</span>
          </span>
        </section>
      )}

      {canTake && (
        <section className="pd-card dp-take">
          <div>
            <h2>Interested in this project?</h2>
            <p className="pd-muted" style={{ margin: 0 }}>
              Review the details below, then take it. Once taken, it is yours and no other designer can pick it up.
            </p>
          </div>
          <button type="button" className="btn-c" onClick={take} disabled={taking}>
            {taking ? 'Taking…' : 'Take this project'}
          </button>
        </section>
      )}

      {project.isMine && (
        <DesignerWork
          project={project}
          hasProposals={proposals.length > 0}
          onMessage={setNotice}
          onChanged={() => setReloadKey((k) => k + 1)}
        />
      )}

      {proposals.length > 0 && (
        <section className="pd-card">
          <div className="pd-card-head"><h2>Your proposals ({proposals.length})</h2></div>
          <ProposalList items={proposals} onError={setError} />
        </section>
      )}

      <section className="pd-card">
        <div className="pd-card-head"><h2>Project</h2></div>
        <div className="dp-grid">
          <Item label="Customer" value={project.customerName} />
          <Item label="Budget" value={budgetText(project.budgetMin, project.budgetMax)} />
          <Item
            label="Expected start"
            value={project.expectedStartDate ? new Date(project.expectedStartDate).toLocaleDateString() : null}
          />
          <Item label="Description" value={project.description} full />
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Property</h2></div>
        <div className="dp-grid">
          <Item label="Type" value={project.propertyType} />
          <Item label="Name" value={project.propertyName} />
          <Item label="Total area" value={project.totalArea != null ? `${project.totalArea} sq ft` : null} />
          <Item label="Floors" value={project.numberOfFloors} />
          <Item label="Address" value={project.address} full />
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Rooms ({project.rooms.length})</h2></div>
        <div className="pd-table-wrap">
          <table className="pd-table">
            <thead>
              <tr>
                <th>Type</th>
                <th>Name</th>
                <th>Length (ft)</th>
                <th>Width (ft)</th>
                <th>Height (ft)</th>
              </tr>
            </thead>
            <tbody>
              {project.rooms.map((room) => (
                <tr key={room.roomId}>
                  <td className="pd-strong">{room.roomType}</td>
                  <td>{show(room.roomName)}</td>
                  <td>{show(room.length)}</td>
                  <td>{show(room.width)}</td>
                  <td>{show(room.height)}</td>
                </tr>
              ))}
              {project.rooms.length === 0 && (
                <tr><td colSpan={5} className="pd-muted">No rooms were added.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Design requirements</h2></div>
        <div className="dp-grid">
          <Item label="Preferred style" value={r.preferredStyle} />
          <Item label="Preferred colors" value={r.preferredColors} />
          <Item label="Furniture" value={r.furnitureRequirements} full />
          <Item label="Storage" value={r.storageRequirements} />
          <Item label="Lighting" value={r.lightingRequirements} />
          <Item label="Special requirements" value={r.specialRequirements} full />
          <Item label="Additional notes" value={r.additionalNotes} full />
        </div>
      </section>

      <section className="pd-card">
        <div className="pd-card-head"><h2>Customer files ({files.length})</h2></div>
        {files.length === 0 ? (
          <p className="pd-muted" style={{ margin: 0 }}>The customer did not attach any files.</p>
        ) : (
          <ul className="dp-files">
            {files.map((f) => (
              <li key={f.attachmentId}>
                <button type="button" className="pd-act" onClick={() => openFile(f)}>
                  Open
                </button>
                <span className="dp-file-name">{f.fileName}</span>
                <span className="pd-muted">{sizeText(f.fileSize)}</span>
              </li>
            ))}
          </ul>
        )}
      </section>

      <div className="np-actions">
        <Link to="/dashboard" className="np-cancel">Back to dashboard</Link>
      </div>
    </div>
  )
}

export default DesignerProject