import { siteApi } from './client'
import { UnitSummary, UnitDetail, CreateUnitDto, UpdateUnitDto } from '@/types/unit'

export const unitsApi = {
  getAll: (buildingId?: string) =>
    siteApi.get<UnitSummary[]>('/api/units', { params: buildingId ? { buildingId } : undefined }),

  getById: (id: string) =>
    siteApi.get<UnitDetail>(`/api/units/${id}`),

  create: (data: CreateUnitDto) =>
    siteApi.post<{ id: string }>('/api/units', data),

  update: (id: string, data: UpdateUnitDto) =>
    siteApi.put(`/api/units/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/units/${id}`),
}
