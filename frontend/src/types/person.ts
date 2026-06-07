import { UnitSummary } from './unit'

export type UserType = 1 | 2 | 3 | 4 | 5  // SuperAdmin=1, Owner=2, Renter=3, Management=4, Admin=5
export type UserSiteStatus = 0 | 1 | 2     // Pending=0, Approved=1, Rejected=2

export const UserTypeLabel: Record<UserType, string> = {
  1: 'Süper Admin',
  2: 'Mal Sahibi',
  3: 'Kiracı',
  4: 'Yönetim',
  5: 'Admin',
}

export const UserSiteStatusLabel: Record<UserSiteStatus, string> = {
  0: 'Beklemede',
  1: 'Onaylı',
  2: 'Reddedildi',
}

export interface PersonDto {
  userId: string
  userSiteId: string
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string
  userType?: UserType
  roleTypeId?: string
  roleName?: string
  status: UserSiteStatus
  isActive: boolean
}

export interface PersonDetailDto extends PersonDto {
  nationalId?: string
  gender?: number
  units: UnitSummary[]
}

export interface InvitePersonDto {
  email: string
  firstName: string
  lastName: string
  phoneNumber?: string
  userType: UserType
  roleTypeId?: string
}

export interface UpdatePersonDto {
  userType?: UserType
  roleTypeId?: string
  status?: UserSiteStatus
}
