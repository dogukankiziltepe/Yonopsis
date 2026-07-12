import { siteApi } from './client'
import { UnitSummary, UnitDetail, CreateUnitDto, UpdateUnitDto } from '@/types/unit'
import { UnitFullDetailDto } from '@/types/unitDetail'

export const unitsApi = {
  getAll: (buildingId?: string) =>
    siteApi.get<UnitSummary[]>('/api/units', { params: buildingId ? { buildingId } : undefined }),

  getById: (id: string) =>
    siteApi.get<UnitDetail>(`/api/units/${id}`),

  getFullDetail: (id: string) =>
    siteApi.get<UnitFullDetailDto>(`/api/units/${id}/detail`),

  create: (data: CreateUnitDto) =>
    siteApi.post<{ id: string }>('/api/units', data),

  update: (id: string, data: UpdateUnitDto) =>
    siteApi.put(`/api/units/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/units/${id}`),

  getMyUnitsAsOwner: () =>
    siteApi.get<UnitSummary[]>('/api/my-unit/as-owner'),

  getMyUnitAsTenant: () =>
    siteApi.get<UnitDetail | null>('/api/my-unit/as-tenant'),

  assignOwner: (id: string, userId: string) =>
    siteApi.patch(`/api/units/${id}/assign-owner`, { userId }),

  assignTenant: (id: string, userId: string) =>
    siteApi.patch(`/api/units/${id}/assign-tenant`, { userId }),

  unassignOwner: (id: string) =>
    siteApi.patch(`/api/units/${id}/unassign-owner`),

  unassignTenant: (id: string) =>
    siteApi.patch(`/api/units/${id}/unassign-tenant`),
}
