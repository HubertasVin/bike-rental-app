import axios from 'axios'
import { authService } from './auth-service'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

// Create axios instance with base configuration
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add request interceptor to include auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = authService.getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// Add response interceptor to handle token expiration
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      authService.clearToken()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export interface CreateReportDTO {
  type: string
  description: string
  bikeId?: string
  userId?: string
}

export interface ReportDTO {
  id: string
  type: string
  description: string
  bikeId?: string
  rentalId?: string
  userId: string
  createdAt: string
  status: string
}

export const reportService = {
  // Create a new report
  async createReport(reportData: CreateReportDTO): Promise<{ data: ReportDTO }> {
    const response = await apiClient.post('/api/Report', reportData)
    return response
  },

  // Get all reports
  async getAllReports(): Promise<{ data: ReportDTO[] }> {
    const response = await apiClient.get('/api/Report')
    return response
  },

  // Get report by ID
  async getReportById(id: string): Promise<{ data: ReportDTO }> {
    const response = await apiClient.get(`/api/Report/${id}`)
    return response
  },

  // Get reports by bike ID
  async getReportsByBikeId(bikeId: string): Promise<{ data: ReportDTO[] }> {
    const response = await apiClient.get(`/api/Report/bike/${bikeId}`)
    return response
  },

  // Get reports by user ID
  async getReportsByUserId(userId: string): Promise<{ data: ReportDTO[] }> {
    const response = await apiClient.get(`/api/Report/user/${userId}`)
    return response
  },

  // Update report
  async updateReport(
    id: string,
    reportData: Partial<CreateReportDTO>,
  ): Promise<{ data: ReportDTO }> {
    const response = await apiClient.put(`/api/Report/${id}`, reportData)
    return response
  },

  // Delete report
  async deleteReport(id: string): Promise<void> {
    await apiClient.delete(`/api/Report/${id}`)
  },
}
