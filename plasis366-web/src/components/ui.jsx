import { useState } from 'react'
import { Link } from 'react-router-dom'

const paths = {
  mail: <><path d="M3 6h18v12H3z" /><path d="m3 7 9 6 9-6" /></>,
  lock: <><rect x="5" y="11" width="14" height="9" rx="2" /><path d="M8 11V8a4 4 0 0 1 8 0v3" /></>,
  user: <><circle cx="12" cy="8" r="4" /><path d="M4 21c0-4 4-6 8-6s8 2 8 6" /></>,
  phone: <path d="M5 4h4l2 5-2.5 1.5a11 11 0 0 0 5 5L15 13l5 2v4a2 2 0 0 1-2 2A16 16 0 0 1 3 6a2 2 0 0 1 2-2z" />,
  home: <><path d="M3 11l9-8 9 8" /><path d="M5 10v10h14V10" /></>,
  at: <><circle cx="12" cy="12" r="4" /><path d="M16 8v5a3 3 0 0 0 6 0v-1a10 10 0 1 0-4 8" /></>,
  eye: <><path d="M2 12s4-7 10-7 10 7 10 7-4 7-10 7S2 12 2 12z" /><circle cx="12" cy="12" r="3" /></>,
  eyeoff: <><path d="M3 3l18 18" /><path d="M10.6 6.1A9.7 9.7 0 0 1 12 5c6 0 10 7 10 7a17 17 0 0 1-3.2 3.9M6.6 6.7A17 17 0 0 0 2 12s4 7 10 7a9.6 9.6 0 0 0 4-.9" /></>,
}

export const Icon = ({ name }) => (
  <svg className="ico" viewBox="0 0 24 24" aria-hidden="true">{paths[name]}</svg>
)

export const Logo = () => (
  <Link to="/" className="logo"><span className="dot" />Plasis<b>366</b></Link>
)

export function Field({ label, icon, error, type = 'text', id, ...rest }) {
  const [show, setShow] = useState(false)
  const pw = type === 'password'
  return (
    <div className="field">
      <label htmlFor={id}>{label}</label>
      <div className="inp">
        <Icon name={icon} />
        <input id={id} name={id} type={pw && show ? 'text' : type} {...rest} />
        {pw && (
          <button type="button" className="eye" onClick={() => setShow(!show)} aria-label={show ? 'Hide password' : 'Show password'}>
            <Icon name={show ? 'eyeoff' : 'eye'} />
          </button>
        )}
      </div>
      {error && <div className="err">{error}</div>}
    </div>
  )
}

export function RoomArt({ wall = '#f3e3cf', floor = '#d9b98f', sofa = '#c8734f', accent = '#8fa58a', className }) {
  return (
    <svg viewBox="0 0 300 190" className={className} role="img" aria-label="Room illustration">
      <rect width="300" height="190" fill={wall} />
      <rect y="140" width="300" height="50" fill={floor} />
      <rect x="40" y="28" width="70" height="62" rx="6" fill="#fffdf9" opacity=".85" />
      <path d="M75 28v62M40 59h70" stroke={wall} strokeWidth="3" />
      <rect x="56" y="80" width="118" height="30" rx="12" fill={sofa} opacity=".7" />
      <rect x="40" y="96" width="150" height="46" rx="14" fill={sofa} />
      <rect x="30" y="108" width="26" height="42" rx="10" fill={sofa} opacity=".85" />
      <rect x="174" y="108" width="26" height="42" rx="10" fill={sofa} opacity=".85" />
      <path d="M226 150v-48" stroke="#6b4f3a" strokeWidth="3" />
      <path d="M212 100a14 14 0 0 1 28 0z" fill="#f6d9a8" />
      <rect x="252" y="126" width="22" height="24" rx="4" fill="#b98a63" />
      <circle cx="263" cy="112" r="14" fill={accent} />
      <circle cx="254" cy="119" r="9" fill={accent} opacity=".8" />
    </svg>
  )
}