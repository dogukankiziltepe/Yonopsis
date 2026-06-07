import { siteApi } from './client'
import { VehicleSummaryDto, CreateVehicleDto, UpdateVehicleDto } from '@/types/vehicle'
import { PaginatedResult } from '@/types/api'

export const vehiclesApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<VehicleSummaryDto>>('/api/vehicles', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<VehicleSummaryDto>(`/api/vehicles/${id}`),

  create: (data: CreateVehicleDto) =>
    siteApi.post<{ id: string }>('/api/vehicles', data),

  update: (id: string, data: UpdateVehicleDto) =>
    siteApi.put(`/api/vehicles/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/vehicles/${id}`),
}
