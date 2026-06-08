import { siteApi } from './client'
import {
  RoleTypeDto,
  RoleTypePermissionsDto,
  CreateRoleTypeDto,
  UpdateRoleTypeDto,
  SetRolePermissionDto,
} from '@/types/roleType'

export const roleTypesApi = {
  getAll: () =>
    siteApi.get<RoleTypeDto[]>('/api/role-types'),

  getPermissions: (id: string) =>
    siteApi.get<RoleTypePermissionsDto>(`/api/role-types/${id}/permissions`),

  create: (data: CreateRoleTypeDto) =>
    siteApi.post<{ id: string }>('/api/role-types', data),

  update: (id: string, data: UpdateRoleTypeDto) =>
    siteApi.put(`/api/role-types/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/role-types/${id}`),

  setPermission: (id: string, data: SetRolePermissionDto) =>
    siteApi.patch(`/api/role-types/${id}/permissions`, data),
}
