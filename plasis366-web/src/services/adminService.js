// src/services/adminService.js
// Calls to the Admin part of the API. The login token is added by src/api.js.
import api from '../api'

export const getAdminDashboard = () => api.get('/Admin/dashboard').then((r) => r.data)

// filter: { status } or { waiting: true } or {} for every project
export const getAdminProjects = (filter = {}) =>
  api.get('/Admin/projects', { params: filter }).then((r) => r.data)

export const getAdminUsers = () => api.get('/Admin/users').then((r) => r.data)

export const getAdminCustomer = (userId) =>
  api.get(`/Admin/customers/${userId}`).then((r) => r.data)

export const addDesigner = (data) => api.post('/Admin/designers', data).then((r) => r.data)

// Switch a designer's or customer's login off or on
export const deactivateUser = (userId) =>
  api.post(`/Admin/users/${userId}/deactivate`).then((r) => r.data)

export const activateUser = (userId) =>
  api.post(`/Admin/users/${userId}/activate`).then((r) => r.data)

export const getAdminDesigners = () => api.get('/Admin/designers').then((r) => r.data)

export const reassignProject = (projectId, designerUserId) =>
  api.post(`/Admin/projects/${projectId}/reassign`, { designerUserId }).then((r) => r.data)