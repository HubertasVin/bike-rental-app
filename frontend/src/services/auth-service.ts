import { jwtDecode } from 'jwt-decode'

const TOKEN_KEY = 'auth_token'

interface JwtPayload {
  exp: number
  email: string
  name?: string
  sub?: string
  [key: string]: any
}

const setToken = (token: string): void => {
  localStorage.setItem(TOKEN_KEY, token)
}

const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY)
}

const clearToken = (): void => {
  localStorage.removeItem(TOKEN_KEY)
}

const isTokenValid = (): boolean => {
  const token = getToken()
  if (!token) return false

  try {
    const decoded = jwtDecode<JwtPayload>(token)
    // Check if token is expired
    const currentTime = Date.now() / 1000
    return decoded.exp > currentTime
  } catch (error) {
    return false
  }
}

const getUserEmail = (): string | null => {
  const token = getToken()
  if (!token) return null

  try {
    const decoded = jwtDecode<JwtPayload>(token)
    return decoded.email || null
  } catch (error) {
    return null
  }
}

const getUserName = (): string | null => {
  const token = getToken()
  if (!token) return null

  try {
    const decoded = jwtDecode<JwtPayload>(token)
    return decoded.name || decoded.sub || null
  } catch (error) {
    return null
  }
}

export const authService = {
  setToken,
  getToken,
  clearToken,
  isTokenValid,
  getUserEmail,
  getUserName,
}
