import { UnitStatus, UnitDirection } from './unit'
import { PetType } from './personDetail'
import { UserType } from './person'

export interface UnitCoreDto {
  id: string
  doorNumber: string
  code?: string
  buildingId: string
  buildingName?: string
  unitTypeId?: string
  unitTypeName?: string
  grossArea?: number
  netArea?: number
  landShare?: number
  floor?: string
  status: UnitStatus
  monthlyFee?: number
  parkingCount?: number
  direction?: UnitDirection
  internet?: string
  hasDask: boolean
}

export interface UnitDetailInfoDto {
  code2?: string
  code3?: string
  deliveryDate?: string
  description?: string
}

export interface UnitRelatedPersonDto {
  historyId: string
  personUserId: string
  role: UserType
  fullName: string
  sharePercentage?: number
  petType?: PetType
  petDetail?: string
  entryDate: string
  exitDate?: string
  contactPerson?: string
  notes?: string
  balance: number
}

export interface UnitVehicleDto {
  id: string
  plate: string
  ownerFullName?: string
  brand?: string
  model?: string
  color?: string
  isActive: boolean
}

export interface UnitFullDetailDto {
  core: UnitCoreDto
  detail: UnitDetailInfoDto
  relatedPersons: UnitRelatedPersonDto[]
  vehicles: UnitVehicleDto[]
}
