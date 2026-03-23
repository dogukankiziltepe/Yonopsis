export interface ApiResult<T> {
  isSuccess: boolean
  value: T | null
  error: string | null
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export interface ApiError {
  status: number
  message: string
  errors?: Record<string, string[]>
}

export enum PermissionLevel {
  Unauthorized = 0,
  ReadOnly = 1,
  ReadAndCreate = 2,
  FullAccess = 3,
}
