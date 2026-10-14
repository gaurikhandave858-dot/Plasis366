import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getNotifications, markAllRead, markRead } from '../services/notificationService'
import './notificationbell.css'

function timeAgo(iso) {
  const mins = Math.floor((Date.now() - new Date(iso)) / 60000)
  if (mins < 60) return `${Math.max(mins, 1)} min ago`
  const hrs = Math.floor(mins / 60)
  if (hrs < 24) return `${hrs} hour${hrs > 1 ? 's' : ''} ago`
  const days = Math.floor(hrs / 24)
  if (days < 30) return `${days} day${days > 1 ? 's' : ''} ago`
  return new Date(iso).toLocaleDateString()
}

function NotificationBell() {
  const navigate = useNavigate()
  const wrapRef = useRef(null)

  const [data, setData] = useState({ unread: 0, items: [] })
  const [open, setOpen] = useState(false)

  const user = JSON.parse(localStorage.getItem('user') || 'null')
  const isDesigner = (user?.roles ?? []).includes('Designer')

  // Check for new notifications now, and then once a minute
  useEffect(() => {
    let cancelled = false

    const check = () =>
      getNotifications()
        .then((d) => {
          if (!cancelled) setData(d)
        })
        .catch(() => {})   // a failed check is not worth bothering the user about

    check()
    const timer = setInterval(check, 60000)
    return () => {
      cancelled = true
      clearInterval(timer)
    }
  }, [])

  // Clicking outside, or pressing Escape, closes the list
  useEffect(() => {
    if (!open) return
    const onClick = (e) => {
      if (wrapRef.current && !wrapRef.current.contains(e.target)) setOpen(false)
    }
    const onKey = (e) => {
      if (e.key === 'Escape') setOpen(false)
    }
    document.addEventListener('mousedown', onClick)
    document.addEventListener('keydown', onKey)
    return () => {
      document.removeEventListener('mousedown', onClick)
      document.removeEventListener('keydown', onKey)
    }
  }, [open])

  const toggle = async () => {
    const next = !open
    setOpen(next)
    if (next) {
      try {
        setData(await getNotifications())   // fresh list every time it is opened
      } catch {
        /* keep what we have */
      }
    }
  }

  const openItem = async (n) => {
    setOpen(false)
    if (!n.isRead) {
      try {
        await markRead(n.notificationId)
        setData((d) => ({
          unread: Math.max(0, d.unread - 1),
          items: d.items.map((x) => (x.notificationId === n.notificationId ? { ...x, isRead: true } : x)),
        }))
      } catch {
        /* still open the project */
      }
    }
    if (n.projectId) {
      navigate(isDesigner ? `/designer/projects/${n.projectId}` : `/projects/${n.projectId}`)
    }
  }

  const readAll = async () => {
    try {
      await markAllRead()
      setData((d) => ({ unread: 0, items: d.items.map((x) => ({ ...x, isRead: true })) }))
    } catch {
      /* ignore */
    }
  }

  return (
    <div className="nb-wrap" ref={wrapRef}>
      <button
        type="button"
        className="nb-bell"
        onClick={toggle}
        aria-label={`Notifications, ${data.unread} unread`}
        aria-expanded={open}
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor"
             strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
          <path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9" />
          <path d="M13.7 21a2 2 0 0 1-3.4 0" />
        </svg>
        {data.unread > 0 && <span className="nb-badge">{data.unread > 9 ? '9+' : data.unread}</span>}
      </button>

      {open && (
        <div className="nb-panel" role="dialog" aria-label="Notifications">
          <div className="nb-head">
            <strong>Notifications</strong>
            {data.unread > 0 && (
              <button type="button" className="nb-link" onClick={readAll}>
                Mark all as read
              </button>
            )}
          </div>

          {data.items.length === 0 ? (
            <p className="nb-empty">You're all caught up.</p>
          ) : (
            <ul className="nb-list">
              {data.items.map((n) => (
                <li key={n.notificationId}>
                  <button
                    type="button"
                    className={`nb-item ${n.isRead ? '' : 'unread'}`}
                    onClick={() => openItem(n)}
                  >
                    <span className="nb-title">{n.title}</span>
                    <span className="nb-msg">{n.message}</span>
                    <span className="nb-time">{timeAgo(n.created)}</span>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  )
}

export default NotificationBell