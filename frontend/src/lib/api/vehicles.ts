import { siteApi } from './client'
import { VehicleSummaryDto, CreateVehicleDto, UpdateVehicleDto, AssignVehicleToUnitDto, ChangeVehicleOwnerDto } from '@/types/vehicle'
import { PaginatedResult } from '@/types/api'

export const vehiclesApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<VehicleSummaryDto>>('/api/vehicles', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<VehicleSummaryDto>(`/api/vehicles/${id}`),

  getByUnit: (unitId: string) =>
    siteApi.get<VehicleSummaryDto[]>(`/api/vehicles/by-unit/${unitId}`),

  create: (data: CreateVehicleDto) =>
    siteApi.post<{ id: string }>('/api/vehicles', data),

  update: (id: string, data: UpdateVehicleDto) =>
    siteApi.put(`/api/vehicles/${id}`, data),

  assignUnit: (id: string, data: AssignVehicleToUnitDto) =>
    siteApi.patch(`/api/vehicles/${id}/assign-unit`, data),

  changeOwner: (id: string, data: ChangeVehicleOwnerDto) =>
    siteApi.patch(`/api/vehicles/${id}/owner`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/vehicles/${id}`),
}
