import { siteApi } from './client'
import { VehicleSummaryDto, CreateVehicleDto, UpdateVehicleDto } from '@/types/vehicle'

export const vehiclesApi = {
  getAll: () =>
    siteApi.get<VehicleSummaryDto[]>('/api/vehicles'),

  getById: (id: string) =>
    siteApi.get<VehicleSummaryDto>(`/api/vehicles/${id}`),

  create: (data: CreateVehicleDto) =>
    siteApi.post<{ id: string }>('/api/vehicles', data),

  update: (id: string, data: UpdateVehicleDto) =>
    siteApi.put(`/api/vehicles/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/vehicles/${id}`),
}
