import { siteApi } from './client'
import { Building, CreateBuildingDto, UpdateBuildingDto } from '@/types/building'

export const buildingsApi = {
  getAll: () =>
    siteApi.get<Building[]>('/api/buildings'),

  getById: (id: string) =>
    siteApi.get<Building>(`/api/buildings/${id}`),

  create: (data: CreateBuildingDto) =>
    siteApi.post<{ id: string }>('/api/buildings', data),

  update: (id: string, data: UpdateBuildingDto) =>
    siteApi.put(`/api/buildings/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/buildings/${id}`),
}
