export interface Personel {
  id: string
  siteId: string
  name: string
  title?: string | null
  phone?: string | null
  email?: string | null
  department?: string | null
  startDate?: string | null
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
}

export interface CreatePersonelDto {
  name: string
  title?: string | null
  phone?: string | null
  email?: string | null
  department?: string | null
  startDate?: string | null
}

export interface UpdatePersonelDto {
  name: string
  title?: string | null
  phone?: string | null
  email?: string | null
  department?: string | null
  startDate?: string | null
  isActive: boolean
}
