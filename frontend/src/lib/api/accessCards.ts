import { siteApi } from './client'
import { AccessCardSummaryDto, CreateAccessCardDto, UpdateAccessCardDto } from '@/types/accessCard'

export const accessCardsApi = {
  getAll: () =>
    siteApi.get<AccessCardSummaryDto[]>('/api/access-cards'),

  getById: (id: string) =>
    siteApi.get<AccessCardSummaryDto>(`/api/access-cards/${id}`),

  create: (data: CreateAccessCardDto) =>
    siteApi.post<{ id: string }>('/api/access-cards', data),

  update: (id: string, data: UpdateAccessCardDto) =>
    siteApi.put(`/api/access-cards/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/access-cards/${id}`),
}
