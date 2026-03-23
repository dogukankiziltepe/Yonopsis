import { siteApi } from './client'
import { Building, CreateBuildingDto, UpdateBuildingDto } from '@/types/building'
import { ApiResult } from '@/types/api'

export const buildingsApi = {
  getAll: () =>
    siteApi.get<ApiResult<Building[]>>('/api/buildings'),

  getById: (id: string) =>
    siteApi.get<ApiResult<Building>>(`/api/buildings/${id}`),

  create: (data: CreateBuildingDto) =>
    siteApi.post<ApiResult<{ id: string }>>('/api/buildings', data),

  update: (id: string, data: UpdateBuildingDto) =>
    siteApi.put<ApiResult<null>>(`/api/buildings/${id}`, data),

  delete: (id: string) =>
    siteApi.delete<ApiResult<null>>(`/api/buildings/${id}`),
}
