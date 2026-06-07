import { siteApi } from './client'
import { UnitTypeDto, CreateUnitTypeDto, UpdateUnitTypeDto } from '@/types/unitType'

export const unitTypesApi = {
  getAll: () =>
    siteApi.get<UnitTypeDto[]>('/api/unit-types'),

  getById: (id: string) =>
    siteApi.get<UnitTypeDto>(`/api/unit-types/${id}`),

  create: (data: CreateUnitTypeDto) =>
    siteApi.post<{ id: string }>('/api/unit-types', data),

  update: (id: string, data: UpdateUnitTypeDto) =>
    siteApi.put(`/api/unit-types/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/unit-types/${id}`),
}
