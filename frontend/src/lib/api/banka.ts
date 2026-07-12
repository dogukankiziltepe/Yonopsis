import { siteApi } from './client'
import type {
  Banka, BankaSubesiPickerItem, CreateBankaDto, UpdateBankaDto,
  CreateBankaSubesiDto, UpdateBankaSubesiDto,
} from '@/types/banka'

export const bankaApi = {
  getAll: () => siteApi.get<Banka[]>('/api/bankalar'),
  getSubeler: (params?: { bankaId?: string; search?: string }) =>
    siteApi.get<BankaSubesiPickerItem[]>('/api/bankalar/subeler', { params }),
  create: (data: CreateBankaDto) => siteApi.post<{ id: string }>('/api/bankalar', data),
  update: (id: string, data: UpdateBankaDto) => siteApi.put(`/api/bankalar/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/bankalar/${id}`),
  createSube: (bankaId: string, data: CreateBankaSubesiDto) =>
    siteApi.post<{ id: string }>(`/api/bankalar/${bankaId}/subeler`, data),
  updateSube: (id: string, data: UpdateBankaSubesiDto) =>
    siteApi.put(`/api/bankalar/subeler/${id}`, data),
  deleteSube: (id: string) => siteApi.delete(`/api/bankalar/subeler/${id}`),
}
