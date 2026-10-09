import api from '../api'

export const getProject = (id) =>
  api.get(`/MyProjects/${id}`).then((r) => r.data)

export const createProject = (data) =>
  api.post('/MyProjects', data).then((r) => r.data)

export const updateProject = (id, data) =>
  api.put(`/MyProjects/${id}`, data).then((r) => r.data)

export const deleteProject = (id) =>
  api.delete(`/MyProjects/${id}`).then((r) => r.data)

export const listAttachments = (id) =>
  api.get(`/MyProjects/${id}/attachments`).then((r) => r.data)

export const removeAttachment = (projectId, attachmentId) =>
  api.delete(`/MyProjects/${projectId}/attachments/${attachmentId}`).then((r) => r.data)

// The API accepts at most 10 files per request, so we send them in batches of 10
export async function uploadAttachments(projectId, files) {
  for (let i = 0; i < files.length; i += 10) {
    const form = new FormData()
    files.slice(i, i + 10).forEach((f) => form.append('files', f))
    await api.post(`/MyProjects/${projectId}/attachments`, form)
  }
}

// Files are private and need the login token, so a plain link would not work.
// We download the file through the API, then show it in a new tab.
export async function openAttachment(projectId, attachmentId) {
  const tab = window.open('', '_blank')
  try {
    const res = await api.get(
      `/MyProjects/${projectId}/attachments/${attachmentId}/file`,
      { responseType: 'blob' }
    )
    const url = URL.createObjectURL(res.data)
    tab.location = url
    setTimeout(() => URL.revokeObjectURL(url), 60000)
  } catch (err) {
    tab?.close()
    throw err
  }
}
   export const submitProject = (id) => api.post(`/MyProjects/${id}/submit`);