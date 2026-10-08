export default function AuthLayout({ tagline, children }) {
  return (
    <div className="auth">
      <aside className="auth-brand">
       <h1 className="brand-title">Plasis366</h1>
        <p className="tagline">{tagline}</p>
      </aside>
      <main className="auth-main"><div className="auth-card">{children}</div></main>
    </div>
  )
}