export interface Building {
  id: string
  siteId: string
  name: string
  unitCount?: number
  createdAt: string
  updatedAt: string
}

export interface CreateBuildingDto {
  name: string
}

export interface UpdateBuildingDto {
  name: string
}
