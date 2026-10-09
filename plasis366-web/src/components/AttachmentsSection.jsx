import { useRef, useState } from 'react'
import './attachments.css'

const ALLOWED = ['.jpg', '.jpeg', '.png', '.webp', '.pdf']
const MAX_SIZE = 10 * 1024 * 1024
const MAX_FILES = 30

function formatSize(bytes) {
  if (bytes < 1024 * 1024) return `${Math.max(1, Math.round(bytes / 1024))} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

function AttachmentsSection({
  existing = [],        // files already saved on the server
  pending = [],         // files chosen but not uploaded yet
  onPendingChange,      // (newList) => void
  onRemoveExisting,     // (attachment) => void
  onOpen,               // (attachment) => void
  readOnly = false,
}) {
  const inputRef = useRef(null)
  const [drag, setDrag] = useState(false)
  const [error, setError] = useState('')

  const addFiles = (fileList) => {
    const next = [...pending]
    const problems = []

    for (const f of Array.from(fileList)) {
      const dot = f.name.lastIndexOf('.')
      const ext = dot >= 0 ? f.name.slice(dot).toLowerCase() : ''

      if (!ALLOWED.includes(ext)) {
        problems.push(`${f.name}: only JPG, PNG, WEBP and PDF files are allowed.`)
      } else if (f.size === 0) {
        problems.push(`${f.name} is empty.`)
      } else if (f.size > MAX_SIZE) {
        problems.push(`${f.name} is larger than 10 MB.`)
      } else if (existing.length + next.length >= MAX_FILES) {
        problems.push(`A project can have up to ${MAX_FILES} files.`)
      } else if (!next.some((p) => p.name === f.name && p.size === f.size)) {
        next.push(f)
      }
    }

    onPendingChange(next)
    setError(problems.join(' '))
    if (inputRef.current) inputRef.current.value = ''
  }

  const removePending = (index) =>
    onPendingChange(pending.filter((_, i) => i !== index))

  const isEmpty = existing.length === 0 && pending.length === 0

  return (
    <div className="att">
      {!readOnly && (
        <div
          className={`att-drop ${drag ? 'drag' : ''}`}
          role="button"
          tabIndex={0}
          onClick={() => inputRef.current?.click()}
          onKeyDown={(e) => {
            if (e.key === 'Enter' || e.key === ' ') {
              e.preventDefault()
              inputRef.current?.click()
            }
          }}
          onDragOver={(e) => { e.preventDefault(); setDrag(true) }}
          onDragLeave={() => setDrag(false)}
          onDrop={(e) => {
            e.preventDefault()
            setDrag(false)
            addFiles(e.dataTransfer.files)
          }}
        >
          <svg className="ico att-up" viewBox="0 0 24 24" aria-hidden="true">
            <path d="M12 16V4M7 9l5-5 5 5M4 20h16" />
          </svg>
          <p><strong>Choose files</strong> or drag them here</p>
          <small>Property photos, floor plan, inspiration images · JPG, PNG, WEBP or PDF · up to 10 MB each</small>
          <input
            ref={inputRef}
            type="file"
            multiple
            hidden
            accept=".jpg,.jpeg,.png,.webp,.pdf"
            onChange={(e) => addFiles(e.target.files)}
          />
        </div>
      )}

      {error && <div className="err">{error}</div>}

      {isEmpty ? (
        <p className="sub att-empty">No files yet.</p>
      ) : (
        <ul className="att-list">
          {existing.map((a) => (
            <li key={a.attachmentId} className="att-item">
              <svg className="ico" viewBox="0 0 24 24" aria-hidden="true">
                <path d="M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8zM14 3v5h5" />
              </svg>
              <button type="button" className="att-name" onClick={() => onOpen(a)}>
                {a.fileName}
              </button>
              <span className="att-size">{formatSize(a.fileSize)}</span>
              {!readOnly && (
                <button type="button" className="att-remove" onClick={() => onRemoveExisting(a)}>
                  Remove
                </button>
              )}
            </li>
          ))}

          {pending.map((f, i) => (
            <li key={`${f.name}-${i}`} className="att-item att-new">
              <svg className="ico" viewBox="0 0 24 24" aria-hidden="true">
                <path d="M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8zM14 3v5h5" />
              </svg>
              <span className="att-name-static">{f.name}</span>
              <span className="att-size">{formatSize(f.size)}</span>
              <span className="att-tag">Uploads when you save</span>
              <button type="button" className="att-remove" onClick={() => removePending(i)}>
                Remove
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default AttachmentsSection