import { siteApi } from './client'
import { AccessCardSummaryDto, CreateAccessCardDto, UpdateAccessCardDto } from '@/types/accessCard'
import { PaginatedResult } from '@/types/api'

export const accessCardsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<AccessCardSummaryDto>>('/api/access-cards', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<AccessCardSummaryDto>(`/api/access-cards/${id}`),

  create: (data: CreateAccessCardDto) =>
    siteApi.post<{ id: string }>('/api/access-cards', data),

  update: (id: string, data: UpdateAccessCardDto) =>
    siteApi.put(`/api/access-cards/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/access-cards/${id}`),
}
