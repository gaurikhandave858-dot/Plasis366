// src/components/ProposalSection.jsx
// The customer's view of the designer's proposals on their own project:
// read the proposal, open its files, then approve it or ask for a revision.
import { useEffect, useState } from 'react'
import ProposalList from './ProposalList'
import { approveProposal, getProposals, requestRevision } from '../services/proposalService'
import './proposals.css'

// Before these statuses there is nothing to show yet
const HAS_PROPOSALS = ['Design In Progress', 'Design Submitted', 'Revision Requested', 'Approved', 'Completed']

function ProposalSection({ projectId, status, onAnswered }) {
  const [items, setItems] = useState([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const [revising, setRevising] = useState(false)
  const [comment, setComment] = useState('')

  // Loads again whenever the project status changes (for example after answering)
  useEffect(() => {
    let cancelled = false

    async function load() {
      if (!HAS_PROPOSALS.includes(status)) return
      try {
        const data = await getProposals(projectId)
        if (!cancelled) setItems(data.items)
      } catch {
        if (!cancelled) setError('Could not load the design proposals.')
      }
    }

    load()
    return () => { cancelled = true }
  }, [projectId, status])

  const answer = async (action, newStatus) => {
    setBusy(true)
    setError('')
    try {
      await action()
      setRevising(false)
      setComment('')
      onAnswered(newStatus)
    } catch (err) {
      setError(err.response?.data?.message || 'Could not send your answer. Please try again.')
    } finally {
      setBusy(false)
    }
  }

  const approve = (p) => {
    if (!window.confirm('Approve this proposal? The designer will be told right away.')) return
    answer(() => approveProposal(p.id), 'Approved')
  }

  const sendRevision = (p) => {
    const text = comment.trim()
    if (!text) return setError('Please tell the designer what you would like changed.')
    if (text.length > 2000) return setError('The comment can be at most 2000 characters.')
    answer(() => requestRevision(p.id, text), 'Revision Requested')
  }

  // Buttons are shown only under the newest proposal, and only while it waits for an answer
  const actions = (p, isLatest) => {
    if (!isLatest || p.status !== 'Submitted' || status !== 'Design Submitted') return null

    if (!revising) {
      return (
        <div className="pr-actions">
          <button type="button" className="btn-c" disabled={busy} onClick={() => approve(p)}>
            {busy ? 'Saving…' : 'Approve'}
          </button>
          <button
            type="button"
            className="pd-act"
            disabled={busy}
            onClick={() => { setRevising(true); setError('') }}
          >
            Ask for changes
          </button>
        </div>
      )
    }

    return (
      <div className="dw-field">
        <label htmlFor="pr-comment">What would you like changed?</label>
        <textarea
          id="pr-comment"
          value={comment}
          maxLength={2000}
          onChange={(e) => setComment(e.target.value)}
          placeholder="Be specific, so the designer can fix it in one go."
        />
        <span className="dw-hint">{comment.length} / 2000</span>
        <div className="pr-actions">
          <button type="button" className="btn-c" disabled={busy} onClick={() => sendRevision(p)}>
            {busy ? 'Sending…' : 'Send to designer'}
          </button>
          <button
            type="button"
            className="pd-act"
            disabled={busy}
            onClick={() => { setRevising(false); setError('') }}
          >
            Cancel
          </button>
        </div>
      </div>
    )
  }

  if (items.length === 0 && !error) return null

  return (
    <section className="pd-card">
      <div className="pd-card-head"><h2>Design proposals ({items.length})</h2></div>
      {status === 'Design Submitted' && (
        <p className="np-notice" style={{ marginTop: 0 }}>
          Your designer has sent a proposal. Please review it and approve it or ask for changes.
        </p>
      )}
      {error && <p className="err" style={{ margin: '0 0 0.8rem' }}>{error}</p>}
      <ProposalList items={items} onError={setError} actions={actions} />
    </section>
  )
}

export default ProposalSection