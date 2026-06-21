import { siteApi } from './client'
import { AidatKalemi, CreateAidatKalemiDto, UpdateAidatKalemiDto } from '@/types/aidatKalemi'

export const aidatKalemleriApi = {
  getAll: () =>
    siteApi.get<AidatKalemi[]>('/api/aidat-kalemleri'),

  create: (data: CreateAidatKalemiDto) =>
    siteApi.post<{ id: string }>('/api/aidat-kalemleri', data),

  update: (id: string, data: UpdateAidatKalemiDto) =>
    siteApi.put(`/api/aidat-kalemleri/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/aidat-kalemleri/${id}`),
}
