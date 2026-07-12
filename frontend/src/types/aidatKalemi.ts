export interface AidatKalemi {
  id: string
  siteId: string
  name: string
  description?: string
  isActive: boolean
  order: number
  createdAt: string
}

export interface CreateAidatKalemiDto {
  name: string
  description?: string
  order: number
}

export interface UpdateAidatKalemiDto {
  name: string
  description?: string
  isActive: boolean
  order: number
}

export interface AidatImportRow {
  unitId: string
  displayName: string
  amounts: { kalemId: string; amount: number }[]
}

export interface BulkCreateAidatPaymentsDto {
  dueDate: string
  items: { unitId: string; aidatKalemId: string; amount: number }[]
}
