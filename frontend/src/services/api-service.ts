import axios from 'axios'
import type { AxiosInstance } from 'axios'
import { authService } from './auth-service'

const apiClient: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5270',
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use(
  (config) => {
    const token = authService.getToken()
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      authService.clearToken()
    }
    return Promise.reject(error)
  },
)

const fetch = (endpoint: string) => {
  return apiClient.get(endpoint)
}

const post = (endpoint: string, data: any) => {
  return apiClient.post(endpoint, data)
}

const put = (endpoint: string, data: any) => {
  return apiClient.put(endpoint, data)
}

const remove = (endpoint: string) => {
  return apiClient.delete(endpoint)
}

const getHelloWorld = () => {
  return apiClient.get('/api/test')
}

const getWeatherForecast = () => {
  return apiClient.get('/weatherforecast')
}

const login = (credentials: { email: string; password: string }) => {
  return apiClient.post('/api/user/login', credentials)
}

const register = (userData: { name: string; email: string; password: string }) => {
  return apiClient.post('/api/user/register', userData)
}

const getZones = () => {
  return apiClient.get('/api/zone')
}

const getActiveReservation = () => {
  return apiClient.get('/api/Reservation/active')
}

const checkReservationIsFree = (reservationId: string) => {
  return apiClient.get(`/api/Reservation/${reservationId}/is-free`)
}

const getReservationCost = (reservationId: string) => {
  return apiClient.get(`/api/Reservation/${reservationId}/cost`)
}

const getReservationDuration = (reservationId: string) => {
  return apiClient.get(`/api/Reservation/${reservationId}/duration`)
}

const createReservation = (bikeId: string) => {
  return apiClient.post('/api/Reservation', { bikeId })
}

const cancelReservation = (reservationId: string) => {
  return apiClient.post(`/api/Reservation/${reservationId}/cancel`)
}

const startRental = (reservationId: string) => {
  return apiClient.post(`/api/Rental/reservation/${reservationId}`)
}

const getRentalCost = (rentalId: string) => {
  return apiClient.get(`/api/Rental/${rentalId}/cost`)
}

const lockBike = (rentalId: string) => {
  return apiClient.post(`/api/Rental/${rentalId}/lock`)
}

const unlockBike = (rentalId: string) => {
  return apiClient.post(`/api/Rental/${rentalId}/unlock`)
}

const endRental = (rentalId: string, zoneId: string) => {
  return apiClient.post(`/api/Rental/${rentalId}/end?zoneId=${zoneId}`)
}

const getActiveRental = () => {
  return apiClient.get('/api/Rental/active')
}

const reportProblem = () => {
  // Placeholder for reporting problems - would typically send to support endpoint
  return Promise.resolve({ data: 'Problem reported successfully' })
}

const createReport = (reportData: {
  type: string
  description: string
  bikeId?: string
  rentalId?: string
}) => {
  return apiClient.post('/api/Report', reportData)
}

const getAllReports = () => {
  return apiClient.get('/api/Report')
}

const getReportById = (id: string) => {
  return apiClient.get(`/api/Report/${id}`)
}

const getReportsByBikeId = (bikeId: string) => {
  return apiClient.get(`/api/Report/bike/${bikeId}`)
}

const getReportsByUserId = (userId: string) => {
  return apiClient.get(`/api/Report/user/${userId}`)
}

const updateReport = (id: string, reportData: any) => {
  return apiClient.put(`/api/Report/${id}`, reportData)
}

const deleteReport = (id: string) => {
  return apiClient.delete(`/api/Report/${id}`)
}

export const api = {
  client: apiClient,
  fetch,
  post,
  put,
  delete: remove,

  getHelloWorld,
  getWeatherForecast,
  getZones,

  login,
  register,

  getActiveReservation,
  checkReservationIsFree,
  getReservationCost,
  getReservationDuration,
  createReservation,
  cancelReservation,

  startRental,
  getRentalCost,
  lockBike,
  unlockBike,
  endRental,
  getActiveRental,

  reportProblem,
  createReport,
  getAllReports,
  getReportById,
  getReportsByBikeId,
  getReportsByUserId,
  updateReport,
  deleteReport,
}
