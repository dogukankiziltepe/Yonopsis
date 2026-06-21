export interface VehicleSummaryDto {
  id: string
  siteId: string
  unitId: string | null
  unitDoorNumber: string | null
  ownerUserId: string | null
  plate: string
  brand?: string
  model?: string
  color?: string
  year?: number
  isActive: boolean
  createdAt: string
}

export interface CreateVehicleDto {
  unitId: string
  ownerUserId?: string
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

export interface AssignVehicleToUnitDto {
  unitId: string
  ownerUserId?: string
}

export interface ChangeVehicleOwnerDto {
  ownerUserId: string | null
}
