import { siteApi } from './client'
import { Unit, CreateUnitDto, UpdateUnitDto } from '@/types/unit'
import { ApiResult } from '@/types/api'

export const unitsApi = {
  getAll: (buildingId?: string) =>
    siteApi.get<ApiResult<Unit[]>>('/api/units', { params: buildingId ? { buildingId } : undefined }),

  getById: (id: string) =>
    siteApi.get<ApiResult<Unit>>(`/api/units/${id}`),

  create: (data: CreateUnitDto) =>
    siteApi.post<ApiResult<{ id: string }>>('/api/units', data),

  update: (id: string, data: UpdateUnitDto) =>
    siteApi.put<ApiResult<null>>(`/api/units/${id}`, data),

  delete: (id: string) =>
    siteApi.delete<ApiResult<null>>(`/api/units/${id}`),
}
