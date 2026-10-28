// src/auth.js
// Small helpers for the login that the login page stored in the browser.

export function getUser() {
  try {
    return JSON.parse(localStorage.getItem('user') || 'null')
  } catch {
    return null
  }
}

// The token is three parts separated by dots; the middle part holds the expiry time (exp, in seconds)
export function tokenExpired(token) {
  try {
    const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')))
    return payload.exp ? payload.exp * 1000 <= Date.now() : false
  } catch {
    return true   // a token we cannot read is treated as not valid
  }
}

export function clearSession() {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
}

// Logged in means there is a user, and a token that has not expired
export function hasValidSession() {
  const token = localStorage.getItem('token')
  return Boolean(token && getUser() && !tokenExpired(token))
}