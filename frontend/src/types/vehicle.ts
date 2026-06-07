export interface VehicleSummaryDto {
  id: string
  siteId: string
  userId: string
  plate: string
  brand?: string
  model?: string
  color?: string
  year?: number
  isActive: boolean
  createdAt: string
}

export interface CreateVehicleDto {
  userId: string
  plate: string
  brand?: string
  model?: string
  color?: string
  year?: number
}

export interface UpdateVehicleDto {
  plate: string
  brand?: string
  model?: string
  color?: string
  year?: number
  isActive: boolean
}
