import { siteApi } from './client'
import { Building, CreateBuildingDto, UpdateBuildingDto } from '@/types/building'
import { PaginatedResult } from '@/types/api'

export const buildingsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<Building>>('/api/buildings', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<Building>(`/api/buildings/${id}`),

  create: (data: CreateBuildingDto) =>
    siteApi.post<{ id: string }>('/api/buildings', data),

  update: (id: string, data: UpdateBuildingDto) =>
    siteApi.put(`/api/buildings/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/buildings/${id}`),
}
