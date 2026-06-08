import { loginApi } from './client'

export interface PlanDto {
  id: string
  name: string
  description?: string
  price: number
  isActive: boolean
  moduleNames: string[]
  createdAt: string
}

export interface ModuleDto {
  id: string
  name: string
  displayName: string
  description?: string
  isActive: boolean
}

export interface CreatePlanDto {
  name: string
  description?: string
  price: number
  isActive: boolean
}

export interface UpdatePlanDto {
  name: string
  description?: string
  price: number
  isActive: boolean
}

export interface SetPlanModuleDto {
  moduleId: string
  isIncluded: boolean
}

export const adminPlansApi = {
  getAll: () => loginApi.get<PlanDto[]>('/api/admin/plans'),
  getById: (id: string) => loginApi.get<PlanDto>(`/api/admin/plans/${id}`),
  create: (data: CreatePlanDto) => loginApi.post<{ id: string }>('/api/admin/plans', data),
  update: (id: string, data: UpdatePlanDto) => loginApi.put(`/api/admin/plans/${id}`, data),
  setModule: (id: string, data: SetPlanModuleDto) => loginApi.patch(`/api/admin/plans/${id}/modules`, data),
  getModules: () => loginApi.get<ModuleDto[]>('/api/admin/modules'),
}
