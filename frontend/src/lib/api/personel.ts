import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type { Personel, CreatePersonelDto, UpdatePersonelDto } from '@/types/personel'

export const personelApi = {
  getAll: (params?: { search?: string; isActive?: boolean; page?: number; pageSize?: number }) =>
    siteApi.get<PaginatedResult<Personel>>('/api/personel', { params }),

  create: (data: CreatePersonelDto) =>
    siteApi.post<{ id: string }>('/api/personel', data),

  update: (id: string, data: UpdatePersonelDto) =>
    siteApi.put(`/api/personel/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/personel/${id}`),
}
