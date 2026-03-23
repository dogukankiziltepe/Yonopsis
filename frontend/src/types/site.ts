export enum DbMode {
  Shared = 1,
  Dedicated = 2,
}

export interface SiteSummaryDto {
  id: string
  name: string
  city?: string
  district?: string
  isActive: boolean
  dbMode: DbMode
  blockCount: number
  unitCount: number
  createdAt: string
}

export interface SiteDetailDto {
  id: string
  name: string
  address?: string
  district?: string
  city?: string
  postalCode?: string
  phone?: string
  email?: string
  taxOffice?: string
  taxNumber?: string
  dbMode: DbMode
  isActive: boolean
  blockCount: number
  unitCount: number
  createdAt: string
  updatedAt: string
}

export interface CreateSiteDto {
  name: string
  dbMode: DbMode
  adminFirstName: string
  adminLastName: string
  adminEmail: string
  adminPhone?: string
  address?: string
  district?: string
  city?: string
  postalCode?: string
  phone?: string
  email?: string
  taxOffice?: string
  taxNumber?: string
}

export interface UpdateSiteDto {
  name: string
  address?: string
  district?: string
  city?: string
  postalCode?: string
  phone?: string
  email?: string
  taxOffice?: string
  taxNumber?: string
  isActive: boolean
}
