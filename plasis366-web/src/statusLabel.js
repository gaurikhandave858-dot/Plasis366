// src/statusLabel.js
// What people read on screen. The stored status names stay the same; only the words change.
export const statusLabel = (s) => (s === 'Revision Requested' ? 'Changes requested' : s)