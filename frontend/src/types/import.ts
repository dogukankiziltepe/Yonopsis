export type ImportType = 'buildings' | 'units' | 'users'

export interface BuildingImportRowData {
  buildingName: string | null
  totalFloors: number | null
  address: string | null
  description: string | null
}

export interface UnitImportRowData {
  buildingName: string | null
  floorNumber: number | null
  unitNumber: string | null
  unitType: string | null
  squareMeters: number | null
  description: string | null
}

export interface UserImportRowData {
  firstName: string | null
  lastName: string | null
  email: string | null
  phone: string | null
  unitNumber: string | null
  buildingName: string | null
  role: string | null
}

export interface ImportPreviewRow {
  rowIndex: number
  data: Record<string, unknown>
  isValid: boolean
  errors: string[]
}

export interface ImportPreviewResult {
  totalRows: number
  validRows: number
  invalidRows: number
  rows: ImportPreviewRow[]
}

export interface ImportConfirmResult {
  savedCount: number
  skippedCount: number
  errors: string[]
}
