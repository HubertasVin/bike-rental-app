import axios from 'axios'
import type { AxiosInstance } from 'axios'

// Create axios instance with hardcoded base URL
const apiClient: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5270',
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

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

// Example of how to add more specific API methods
// const login = (credentials: { username: string, password: string }) => {
//   return apiClient.post('/api/auth/login', credentials)
// }

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
}
