import axios from 'axios'
import type { AxiosInstance } from 'axios'
import { authService } from './auth-service'

// Create axios instance with hardcoded base URL
const apiClient: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5270',
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

// Add request interceptor for auth headers
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

// Add response interceptor for handling auth errors
apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      // Handle unauthorized access, just clear the token
      authService.clearToken()
    }
    return Promise.reject(error)
  },
)

// General API methods
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

// Specific API endpoints
const getHelloWorld = () => {
  return apiClient.get('/api/test')
}

const getWeatherForecast = () => {
  return apiClient.get('/weatherforecast')
}

// Authentication endpoints
const login = (credentials: { email: string; password: string }) => {
  return apiClient.post('/api/user/login', credentials)
}

const register = (userData: { name: string; email: string; password: string }) => {
  return apiClient.post('/api/user/register', userData)
}

const logout = () => {
  authService.clearToken()
  // Return a resolved promise to maintain function signature
  return Promise.resolve({ status: 200, data: { success: true } })
}

// Get zones endpoint
const getZones = () => {
  return apiClient.get('/api/zone')
}

export const api = {
  // General methods
  client: apiClient,
  fetch,
  post,
  put,
  delete: remove,

  // Specific endpoints
  getHelloWorld,
  getWeatherForecast,
  getZones,

  // Auth endpoints
  login,
  register,
  logout,
}
