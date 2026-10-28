// src/services/historyService.js
import api from '../api'

export const getProjectHistory = (projectId) =>
  api.get(`/ProjectHistory/${projectId}`).then((r) => r.data)