export type ImportType = 'buildings' | 'units' | 'users'

export interface ImportRowResult {
  rowIndex: number
  data: Record<string, string | null>
  isValid: boolean
  errors: string[]
}

export interface ImportPreview {
  totalRows: number
  validRows: number
  invalidRows: number
  rows: ImportRowResult[]
}

export interface ImportResult {
  savedRows: number
  skippedRows: number
  errors: string[]
}
