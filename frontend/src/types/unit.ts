export interface Unit {
  id: string
  siteId: string
  buildingId: string
  buildingName?: string
  number: string
  floor?: number
  type?: string
  ownerName?: string
  renterName?: string
  createdAt: string
  updatedAt: string
}

export interface CreateUnitDto {
  buildingId: string
  number: string
  floor?: number
  type?: string
}

export interface UpdateUnitDto {
  number: string
  floor?: number
  type?: string
}
