import { siteApi } from './client'
import {
  CariTuru,
  CreateCariHesapDto,
  CreateHesapDto,
  HesapDetail,
  HesapListItem,
  HesapNode,
  UpdateHesapDto,
} from '@/types/muhasebe'

export const muhasebeApi = {
  // Hesap Planı
  getTree: (sadeceAktif = false) =>
    siteApi.get<HesapNode[]>('/api/muhasebe/hesap-plani/tree', { params: { sadeceAktif } }),

  getHesaplar: (params?: {
    cariTuru?: CariTuru
    fisKesilebilir?: boolean
    aktif?: boolean
    search?: string
  }) => siteApi.get<HesapListItem[]>('/api/muhasebe/hesaplar', { params }),

  getHesap: (id: string) =>
    siteApi.get<HesapDetail>(`/api/muhasebe/hesaplar/${id}`),

  createHesap: (data: CreateHesapDto) =>
    siteApi.post<{ id: string }>('/api/muhasebe/hesaplar', data),

  updateHesap: (id: string, data: UpdateHesapDto) =>
    siteApi.put(`/api/muhasebe/hesaplar/${id}`, data),

  toggleAktif: (id: string, aktif: boolean) =>
    siteApi.patch(`/api/muhasebe/hesaplar/${id}/aktif`, null, { params: { aktif } }),

  // Cari Hesaplar
  getCariHesaplar: (params?: { cariTuru?: CariTuru; search?: string }) =>
    siteApi.get<HesapListItem[]>('/api/muhasebe/cari-hesaplar', { params }),

  createCariHesap: (data: CreateCariHesapDto) =>
    siteApi.post<{ id: string }>('/api/muhasebe/cari-hesaplar', data),
}
