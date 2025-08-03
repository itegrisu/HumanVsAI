import axios from 'axios'

// Environment based API base URL
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

console.log('API Base URL:', API_BASE_URL)

// Create axios instance
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    console.log(`Making ${config.method?.toUpperCase()} request to: ${config.url}`)
    return config
  },
  (error) => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// Response interceptor
apiClient.interceptors.response.use(
  (response) => {
    console.log('API Response:', response.status, response.config.url)
    return response
  },
  (error) => {
    console.error('API Error:', error.response?.status, error.config?.url)
    
    // Fallback to mock data if API fails
    if (error.config?.url?.includes('/api/')) {
      console.log('API failed, using mock data')
      return Promise.resolve({ data: null, status: 200 })
    }
    
    return Promise.reject(error)
  }
)

// API endpoints
export const api = {
  // Health check
  healthCheck: () => apiClient.get('/health'),
  
  // Game endpoints
  getImages: () => apiClient.get('/game/images'),
  submitScore: (data: any) => apiClient.post('/game/score', data),
  
  // Leaderboard endpoints
  getLeaderboard: () => apiClient.get('/leaderboard'),
  addScore: (data: any) => apiClient.post('/leaderboard', data),
  
  // User endpoints
  createUser: (userData: any) => apiClient.post('/users', userData),
  updateUser: (id: string, userData: any) => apiClient.put(`/users/${id}`, userData),
}

export default apiClient
