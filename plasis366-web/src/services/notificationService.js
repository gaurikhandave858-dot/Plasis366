// src/services/notificationService.js
// Calls to the Notifications part of the API. The login token is added by src/api.js.
import api from '../api'

// { unread: number, items: [{ notificationId, projectId, title, message, isRead, created }] }
export const getNotifications = () => api.get('/Notifications').then((r) => r.data)

export const markRead = (id) => api.post(`/Notifications/${id}/read`).then((r) => r.data)

export const markAllRead = () => api.post('/Notifications/read-all').then((r) => r.data)