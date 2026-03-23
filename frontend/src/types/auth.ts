export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken?: string
  accessTokenExpires?: string
  userId: string
  firstName: string
  lastName: string
  email: string
  isSuperAdmin: boolean
  mustChangePassword: boolean
}

export interface SelectSiteRequest {
  siteId: string
  userType: string
}

export interface SelectSiteResponse {
  accessToken: string
  refreshToken?: string
  accessTokenExpires?: string
  siteId: string
  siteName: string
  userType: string
  roleTypeId?: string
  roleName?: string
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

export interface UserClaims {
  userId: string
  email: string
  firstName: string
  lastName: string
  isSuperAdmin: boolean
  mustChangePassword?: boolean
  // Site-specific fields (set after site selection)
  siteId?: string
  siteName?: string
  userType?: string
  roleTypeId?: string
  roleName?: string
}

export interface UserSiteApplicationDto {
  siteId: string
  siteName: string
}

export interface UserSiteListDto {
  siteId: string
  siteName: string
  siteLogo?: string
  userTypes: string[]
  isSubscriptionActive: boolean
  subscriptionEndDate?: string
  pendingApplications: UserSiteApplicationDto[]
}

export interface MySitesResponse {
  sites: UserSiteListDto[]
}
