import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type { Rezervasyon, CreateRezervasyonDto, UpdateRezervasyonDto, RezervasyonDurum } from '@/types/rezervasyon'

export const rezervasyonApi = {
  getAll: (params?: {
    tesisId?: string
    from?: string
    to?: string
    durum?: RezervasyonDurum
    page?: number
    pageSize?: number
  }) => siteApi.get<PaginatedResult<Rezervasyon>>('/api/rezervasyonlar', { params }),

  create: (data: CreateRezervasyonDto) =>
    siteApi.post<{ id: string }>('/api/rezervasyonlar', data),

  update: (id: string, data: UpdateRezervasyonDto) =>
    siteApi.put(`/api/rezervasyonlar/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/rezervasyonlar/${id}`),
}
