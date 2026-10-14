// src/services/designerService.js
// All calls to the Designer part of the API. The token is added by src/api.js.
import api from '../api'

export const getAvailable = () => api.get('/Designer/available').then((r) => r.data)

export const getMine = () => api.get('/Designer/mine').then((r) => r.data)

export const getDesignerProject = (id) =>
  api.get(`/Designer/projects/${id}`).then((r) => r.data)

export const takeProject = (id) =>
  api.post(`/Designer/projects/${id}/take`).then((r) => r.data)

export const listDesignerAttachments = (id) =>
  api.get(`/Designer/projects/${id}/attachments`).then((r) => r.data)

// Files need the login token, so they are fetched with axios and opened as a blob
export async function openDesignerAttachment(projectId, attachmentId) {
  const res = await api.get(
    `/Designer/projects/${projectId}/attachments/${attachmentId}/file`,
    { responseType: 'blob' }
  )
  const url = URL.createObjectURL(res.data)
  window.open(url, '_blank')
  setTimeout(() => URL.revokeObjectURL(url), 60000)
}