import axios from 'axios'
import { useAuthStore } from '@/lib/store/auth.store'

const baseURL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5241'

// Login token instance — used for /login, /my-sites, /select, /change-password
export const loginApi = axios.create({ baseURL, withCredentials: true })

// Site token instance — used for all business endpoints
export const siteApi = axios.create({ baseURL, withCredentials: true })

// Attach login token to requests
loginApi.interceptors.request.use((config) => {
  const token = useAuthStore.getState().loginToken
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// Attach site token to requests
siteApi.interceptors.request.use((config) => {
  const token = useAuthStore.getState().siteToken
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// Refresh queue for concurrent 401s
let isRefreshing = false
let refreshQueue: Array<(token: string) => void> = []

const processQueue = (token: string) => {
  refreshQueue.forEach((cb) => cb(token))
  refreshQueue = []
}

siteApi.interceptors.response.use(
  (res) => res,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error)
    }

    if (isRefreshing) {
      return new Promise((resolve) => {
        refreshQueue.push((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`
          resolve(siteApi(originalRequest))
        })
      })
    }

    originalRequest._retry = true
    isRefreshing = true

    try {
      const { data } = await axios.post(
        `${baseURL}/api/auth/refresh`,
        {},
        { withCredentials: true }
      )
      const currentUser = useAuthStore.getState().user
      useAuthStore.getState().setSiteToken(data.accessToken, {
        siteId: currentUser?.siteId ?? '',
        siteName: currentUser?.siteName,
        userType: currentUser?.userType ?? '',
        roleTypeId: currentUser?.roleTypeId,
        roleName: currentUser?.roleName,
      })
      processQueue(data.accessToken)
      originalRequest.headers.Authorization = `Bearer ${data.accessToken}`

      // Refresh sonrası pages listesini güncelle
      import('@/lib/api/pages').then(({ getMyPages }) => {
        getMyPages().then((res) => {
          if (res.isSuccess && res.value) {
            useAuthStore.getState().setPages(res.value)
          }
        })
      })

      return siteApi(originalRequest)
    } catch (refreshError) {
      useAuthStore.getState().clearTokens()
      if (typeof window !== 'undefined') {
        window.location.href = '/login'
      }
      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  }
)
