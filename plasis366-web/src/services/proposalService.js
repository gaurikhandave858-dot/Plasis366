// src/services/proposalService.js
// Calls to the Proposals part of the API. The login token is added by src/api.js.
import api from '../api'

export const getProposals = (projectId) =>
  api.get(`/Proposals/projects/${projectId}`).then((r) => r.data)

// ----- designer -----
export const startDesign = (projectId) =>
  api.post(`/Proposals/projects/${projectId}/start`).then((r) => r.data)

// formData holds: title, description, estimatedCost, estimatedDays and the files
export const submitProposal = (projectId, formData) =>
  api.post(`/Proposals/projects/${projectId}`, formData).then((r) => r.data)

export const completeProject = (projectId) =>
  api.post(`/Proposals/projects/${projectId}/complete`).then((r) => r.data)

// ----- customer -----
export const approveProposal = (proposalId) =>
  api.post(`/Proposals/${proposalId}/approve`).then((r) => r.data)

export const requestRevision = (proposalId, comment) =>
  api.post(`/Proposals/${proposalId}/revision`, { comment }).then((r) => r.data)

// ----- both -----
// Files need the login token, so they are fetched with axios and opened as a blob
export async function openProposalFile(fileId) {
  const res = await api.get(`/Proposals/files/${fileId}`, { responseType: 'blob' })
  const url = URL.createObjectURL(res.data)
  window.open(url, '_blank')
  setTimeout(() => URL.revokeObjectURL(url), 60000)
}