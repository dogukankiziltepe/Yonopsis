export type UnitStatus = 0 | 1 | 2  // Bos=0, Dolu=1, Kiralik=2
export type UnitDirection = 0 | 1 | 2 | 3 | 4  // Kuzey=0, Guney=1, Dogu=2, Bati=3, Bilinmiyor=4

export const UnitStatusLabel: Record<UnitStatus, string> = {
  0: 'Boş',
  1: 'Dolu',
  2: 'Kiralık',
}

export const UnitDirectionLabel: Record<UnitDirection, string> = {
  0: 'Kuzey',
  1: 'Güney',
  2: 'Doğu',
  3: 'Batı',
  4: 'Bilinmiyor',
}

export interface UnitSummary {
  id: string
  buildingId: string
  buildingName?: string
  unitTypeId?: string
  unitTypeName?: string
  doorNumber: string
  code?: string
  floor?: string
  status: UnitStatus
  monthlyFee?: number
  hasDask: boolean
  createdAt: string
}

export interface UnitDetail {
  id: string
  siteId: string
  buildingId: string
  buildingName?: string
  unitTypeId?: string
  unitTypeName?: string
  doorNumber: string
  code?: string
  floor?: string
  grossArea?: number
  netArea?: number
  landShare?: number
  status: UnitStatus
  monthlyFee?: number
  parkingCount?: number
  direction?: UnitDirection
  internet?: string
  hasDask: boolean
  ownerUserId?: string
  ownerFullName?: string
  tenantUserId?: string
  tenantFullName?: string
  description?: string
  code2?: string
  code3?: string
  deliveryDate?: string
  createdAt: string
  updatedAt: string
}

export interface CreateUnitDto {
  buildingId: string
  unitTypeId?: string
  doorNumber: string
  code?: string
  floor?: string
  grossArea?: number
  netArea?: number
  landShare?: number
  status: UnitStatus
  monthlyFee?: number
  parkingCount?: number
  direction?: UnitDirection
  internet?: string
  hasDask: boolean
  description?: string
  code2?: string
  code3?: string
  deliveryDate?: string
}

export interface UpdateUnitDto extends CreateUnitDto {}
