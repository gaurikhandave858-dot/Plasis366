// src/components/StatusTimeline.jsx
// "Where is my project": every step it went through, oldest first, newest highlighted.
import { useEffect, useState } from 'react'
import { getProjectHistory } from '../services/historyService'
import { statusLabel } from '../statusLabel'
import './timeline.css'

// Every line of the history is shown to the customer too, so keep wording plain
function when(iso) {
  const d = new Date(iso)
  return `${d.toLocaleDateString()} · ${d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`
}

// status is passed in so the timeline loads again when the project moves on
function StatusTimeline({ projectId, status }) {
  const [items, setItems] = useState(null)   // null = still loading
  const [error, setError] = useState('')

  useEffect(() => {
    let cancelled = false

    async function load() {
      try {
        const list = await getProjectHistory(projectId)
        if (!cancelled) setItems(list)
      } catch {
        if (!cancelled) {
          setError('Could not load the history.')
          setItems((i) => i ?? [])
        }
      }
    }

    load()
    return () => { cancelled = true }
  }, [projectId, status])

  return (
    <section className="pd-card">
      <div className="pd-card-head"><h2>Project history</h2></div>
      {error && <p className="err" style={{ margin: 0 }}>{error}</p>}
      {items === null && <p className="pd-muted" style={{ margin: 0 }}>Loading…</p>}
      {items && items.length === 0 && !error && (
        <p className="pd-muted" style={{ margin: 0 }}>Nothing has happened yet.</p>
      )}
      {items && items.length > 0 && (
        <ol className="tl">
          {items.map((h, i) => (
            <li key={i} className={`tl-item ${i === items.length - 1 ? 'latest' : ''}`}>
              <div className="tl-title">{statusLabel(h.status)}</div>
              {h.remarks && <div className="tl-note">{h.remarks}</div>}
              <div className="tl-when">{when(h.when)}</div>
            </li>
          ))}
        </ol>
      )}
    </section>
  )
}

export default StatusTimeline