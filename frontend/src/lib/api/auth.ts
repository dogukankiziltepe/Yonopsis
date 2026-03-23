import { loginApi, siteApi } from './client'
import {
  LoginRequest,
  LoginResponse,
  SelectSiteRequest,
  SelectSiteResponse,
  ChangePasswordRequest,
  MySitesResponse,
  UserSiteListDto,
} from '@/types/auth'

export const authApi = {
  login: (data: LoginRequest) =>
    loginApi.post<LoginResponse>('/api/auth/login', data),

  logout: () =>
    loginApi.post('/api/auth/logout'),

  refresh: () =>
    loginApi.post<LoginResponse>('/api/auth/refresh', {}),

  changePassword: (data: ChangePasswordRequest) =>
    loginApi.post('/api/auth/change-password', data),

  me: () =>
    loginApi.get('/api/auth/me'),

  mySites: () =>
    loginApi.get<UserSiteListDto[]>('/api/siteselection/my-sites'),

  selectSite: (data: SelectSiteRequest) =>
    loginApi.post<SelectSiteResponse>('/api/siteselection/select', data),
}
