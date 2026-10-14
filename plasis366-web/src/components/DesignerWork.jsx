// src/components/DesignerWork.jsx
// The designer's work area on a project they have taken: start the design,
// submit a proposal, and mark an approved project as completed.
import { useState } from 'react'
import { completeProject, startDesign, submitProposal } from '../services/proposalService'
import './proposals.css'

const MAX_FILES = 10
const MAX_SIZE = 10 * 1024 * 1024
const ALLOWED = ['.jpg', '.jpeg', '.png', '.webp', '.pdf']

const extOf = (name) => {
  const i = name.lastIndexOf('.')
  return i >= 0 ? name.slice(i).toLowerCase() : ''
}

function DesignerWork({ project, hasProposals, onChanged, onMessage }) {
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [cost, setCost] = useState('')
  const [days, setDays] = useState('')
  const [files, setFiles] = useState([])

  const run = async (action, doneMessage) => {
    setBusy(true)
    setError('')
    try {
      await action()
      onMessage(doneMessage)
      onChanged()
    } catch (err) {
      setError(err.response?.data?.message || 'Something went wrong. Please try again.')
      onChanged()
    } finally {
      setBusy(false)
    }
  }

  const chooseFiles = (e) => {
    const picked = Array.from(e.target.files || [])
    e.target.value = ''
    setError('')

    const next = [...files, ...picked]
    if (next.length > MAX_FILES) {
      setError(`You can attach up to ${MAX_FILES} files.`)
      return
    }
    for (const f of picked) {
      if (!ALLOWED.includes(extOf(f.name))) {
        setError(`${f.name}: only JPG, PNG, WEBP and PDF files are allowed.`)
        return
      }
      if (f.size > MAX_SIZE) {
        setError(`${f.name} is larger than 10 MB.`)
        return
      }
    }
    setFiles(next)
  }

  const submit = (e) => {
    e.preventDefault()
    setError('')

    if (!title.trim()) return setError('Please give the proposal a title.')
    if (title.trim().length > 200) return setError('The title can be at most 200 characters.')
    if (!description.trim()) return setError('Please add a note for the customer.')
    if (description.trim().length > 4000) return setError('The note can be at most 4000 characters.')

    const c = Number(cost)
    if (cost === '' || Number.isNaN(c) || c < 0) return setError('Please enter a valid estimated cost.')
    const d = Number(days)
    if (days === '' || !Number.isInteger(d) || d < 1 || d > 3650) {
      return setError('Estimated days must be a whole number between 1 and 3650.')
    }
    if (files.length === 0) return setError('Please attach at least one file.')

    const data = new FormData()
    data.append('title', title.trim())
    data.append('description', description.trim())
    data.append('estimatedCost', String(c))
    data.append('estimatedDays', String(d))
    files.forEach((f) => data.append('files', f))

    run(async () => {
      await submitProposal(project.projectId, data)
      setTitle('')
      setDescription('')
      setCost('')
      setDays('')
      setFiles([])
    }, 'Your proposal was sent to the customer.')
  }

  const status = project.status

  // Under Review or Revision Requested: the designer starts (or restarts) the work
  if (status === 'Under Review' || status === 'Revision Requested') {
    const again = status === 'Revision Requested'
    return (
      <section className="pd-card dw-wait">
        <div>
          <h2>{again ? 'The customer asked for changes' : 'Ready to start?'}</h2>
          <p className="pd-muted" style={{ margin: 0 }}>
            {again
              ? 'Read the feedback below, then start working on a new version.'
              : 'Read the project details below. When you are ready, start the design.'}
          </p>
          {error && <p className="err" style={{ margin: '0.5rem 0 0' }}>{error}</p>}
        </div>
        <button
          type="button"
          className="btn-c"
          disabled={busy}
          onClick={() => run(() => startDesign(project.projectId), 'The design is now in progress.')}
        >
          {busy ? 'Starting…' : again ? 'Start new version' : 'Start design'}
        </button>
      </section>
    )
  }

  if (status === 'Design In Progress') {
    return (
      <section className="pd-card">
        <div className="pd-card-head"><h2>Submit your proposal</h2></div>
        <form className="dw-form" onSubmit={submit} noValidate>
          <div className="dw-field">
            <label htmlFor="dw-title">Title</label>
            <input
              id="dw-title"
              type="text"
              value={title}
              maxLength={200}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="For example: Modern living room concept"
            />
          </div>

          <div className="dw-row">
            <div className="dw-field">
              <label htmlFor="dw-cost">Estimated cost (₹)</label>
              <input
                id="dw-cost"
                type="number"
                min="0"
                step="any"
                value={cost}
                onChange={(e) => setCost(e.target.value)}
              />
            </div>
            <div className="dw-field">
              <label htmlFor="dw-days">Timeline (days)</label>
              <input
                id="dw-days"
                type="number"
                min="1"
                max="3650"
                step="1"
                value={days}
                onChange={(e) => setDays(e.target.value)}
              />
            </div>
          </div>

          <div className="dw-field">
            <label htmlFor="dw-note">Note for the customer</label>
            <textarea
              id="dw-note"
              value={description}
              maxLength={4000}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Explain your idea, materials, colours and what the estimate covers."
            />
            <span className="dw-hint">{description.length} / 4000</span>
          </div>

          <div className="dw-field">
            <label htmlFor="dw-files">Files</label>
            <input
              id="dw-files"
              type="file"
              multiple
              accept=".jpg,.jpeg,.png,.webp,.pdf"
              onChange={chooseFiles}
            />
            <span className="dw-hint">
              Up to {MAX_FILES} files (JPG, PNG, WEBP or PDF), 10 MB each.
            </span>
            {files.length > 0 && (
              <ul className="dw-chosen">
                {files.map((f, i) => (
                  <li key={`${f.name}-${i}`}>
                    <span className="dp-file-name">{f.name}</span>
                    <button
                      type="button"
                      className="pd-act"
                      onClick={() => setFiles(files.filter((_, n) => n !== i))}
                    >
                      Remove
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </div>

          {error && <p className="err" style={{ margin: 0 }}>{error}</p>}

          <div>
            <button type="submit" className="btn-c" disabled={busy}>
              {busy ? 'Sending…' : 'Submit to customer'}
            </button>
          </div>
        </form>
      </section>
    )
  }

  if (status === 'Design Submitted') {
    return (
      <section className="pd-card">
        <h2 style={{ margin: '0 0 0.25rem' }}>Waiting for the customer</h2>
        <p className="pd-muted" style={{ margin: 0 }}>
          Your proposal has been sent. You will get a notification when the customer answers.
        </p>
      </section>
    )
  }

  if (status === 'Approved') {
    return (
      <section className="pd-card dw-wait">
        <div>
          <h2>The customer approved your proposal</h2>
          <p className="pd-muted" style={{ margin: 0 }}>
            When the work is finished, mark the project as completed.
          </p>
          {error && <p className="err" style={{ margin: '0.5rem 0 0' }}>{error}</p>}
        </div>
        <button
          type="button"
          className="btn-c"
          disabled={busy}
          onClick={() => {
            if (!window.confirm('Mark this project as completed?')) return
            run(() => completeProject(project.projectId), 'The project is marked as completed.')
          }}
        >
          {busy ? 'Saving…' : 'Mark as completed'}
        </button>
      </section>
    )
  }

  if (status === 'Completed' && hasProposals) {
    return (
      <section className="pd-card">
        <p className="pd-muted" style={{ margin: 0 }}>This project is completed.</p>
      </section>
    )
  }

  return null
}

export default DesignerWork