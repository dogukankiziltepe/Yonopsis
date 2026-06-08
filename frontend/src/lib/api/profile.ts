import { siteApi } from './client'

export interface ProfileDto {
  id: string
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string
  nationalId?: string
  gender?: number
  isActive: boolean
  createdAt: string
}

export interface UpdateProfileDto {
  firstName: string
  lastName: string
  phoneNumber?: string
  nationalId?: string
  gender?: number
}

export const profileApi = {
  get: () => siteApi.get<ProfileDto>('/api/auth/profile'),
  update: (data: UpdateProfileDto) => siteApi.put('/api/auth/profile', data),
}
