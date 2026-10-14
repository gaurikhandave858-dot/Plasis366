// src/components/ProposalList.jsx
// Shows the proposals of a project, newest first. Used on the designer's page now,
// and on the customer's page later (the customer's buttons go in through `actions`).
import { openProposalFile } from '../services/proposalService'
import './proposals.css'

const money = (v) => (v == null ? '—' : `₹${Number(v).toLocaleString('en-IN')}`)

function sizeText(bytes) {
  if (bytes == null) return ''
  return bytes < 1024 * 1024
    ? `${Math.max(1, Math.round(bytes / 1024))} KB`
    : `${(bytes / 1024 / 1024).toFixed(1)} MB`
}

function ProposalList({ items, onError, actions }) {
  const open = async (file) => {
    try {
      await openProposalFile(file.id)
    } catch {
      if (onError) onError('Could not open the file.')
    }
  }

  if (!items || items.length === 0) return null

  return (
    <div>
      {items.map((p, index) => (
        <article key={p.id} className={`pr-item ${index === 0 ? 'latest' : ''}`}>
          <div className="pr-head">
            <h3 className="pr-title">
              Version {p.version} · {p.title}
            </h3>
            <span className="pr-tag">{p.status}</span>
          </div>

          <div className="pr-facts">
            <div>
              <div className="dp-label">Estimated cost</div>
              <div className="dp-value">{money(p.estimatedCost)}</div>
            </div>
            <div>
              <div className="dp-label">Timeline</div>
              <div className="dp-value">
                {p.estimatedDays != null ? `${p.estimatedDays} day${p.estimatedDays > 1 ? 's' : ''}` : '—'}
              </div>
            </div>
            <div>
              <div className="dp-label">Submitted</div>
              <div className="dp-value">
                {p.submittedDate ? new Date(p.submittedDate).toLocaleDateString() : '—'}
              </div>
            </div>
          </div>

          {p.description && <p className="pr-note">{p.description}</p>}

          {p.files.length > 0 && (
            <ul className="dp-files">
              {p.files.map((f) => (
                <li key={f.id}>
                  <button type="button" className="pd-act" onClick={() => open(f)}>
                    Open
                  </button>
                  <span className="dp-file-name">{f.fileName}</span>
                  <span className="pd-muted">{sizeText(f.fileSize)}</span>
                </li>
              ))}
            </ul>
          )}

          {p.customerFeedback && (
            <div className="pr-feedback">
              <strong>Customer feedback</strong>
              <div>{p.customerFeedback}</div>
            </div>
          )}

          {actions && actions(p, index === 0)}
        </article>
      ))}
    </div>
  )
}

export default ProposalList