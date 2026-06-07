export interface UnitTypeDto {
  id: string
  siteId: string
  name: string
  isActive: boolean
  createdAt: string
}

export interface CreateUnitTypeDto {
  name: string
}

export interface UpdateUnitTypeDto {
  name: string
  isActive: boolean
}
