import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  AjandaEtkinlik, CreateAjandaEtkinlikDto, UpdateAjandaEtkinlikDto,
  Toplanti, CreateToplantiDto, UpdateToplantiDto,
  Teklif, CreateTeklifDto, UpdateTeklifDto,
  YapilacakIs, CreateYapilacakIsDto, UpdateYapilacakIsDto,
} from '@/types/siteYonetim'
import type { ToplamtiDurum, TeklifDurum, YapilacakIsDurum } from '@/types/siteYonetim'

export const ajandaApi = {
  getAll: (page = 1, pageSize = 50, from?: string, to?: string) =>
    siteApi.get<PaginatedResult<AjandaEtkinlik>>('/api/ajanda', { params: { page, pageSize, from, to } }),
  create: (data: CreateAjandaEtkinlikDto) =>
    siteApi.post<{ id: string }>('/api/ajanda', data),
  update: (id: string, data: UpdateAjandaEtkinlikDto) =>
    siteApi.put(`/api/ajanda/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/ajanda/${id}`),
}

export const toplantilarApi = {
  getAll: (page = 1, pageSize = 50, search?: string, durum?: ToplamtiDurum) =>
    siteApi.get<PaginatedResult<Toplanti>>('/api/toplantilar', { params: { page, pageSize, search, durum } }),
  create: (data: CreateToplantiDto) =>
    siteApi.post<{ id: string }>('/api/toplantilar', data),
  update: (id: string, data: UpdateToplantiDto) =>
    siteApi.put(`/api/toplantilar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/toplantilar/${id}`),
}

export const tekliflerApi = {
  getAll: (page = 1, pageSize = 50, search?: string, durum?: TeklifDurum) =>
    siteApi.get<PaginatedResult<Teklif>>('/api/teklifler', { params: { page, pageSize, search, durum } }),
  create: (data: CreateTeklifDto) =>
    siteApi.post<{ id: string }>('/api/teklifler', data),
  update: (id: string, data: UpdateTeklifDto) =>
    siteApi.put(`/api/teklifler/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/teklifler/${id}`),
}

export const yapilacakIslerApi = {
  getAll: (page = 1, pageSize = 50, search?: string, durum?: YapilacakIsDurum) =>
    siteApi.get<PaginatedResult<YapilacakIs>>('/api/yapilacak-isler', { params: { page, pageSize, search, durum } }),
  create: (data: CreateYapilacakIsDto) =>
    siteApi.post<{ id: string }>('/api/yapilacak-isler', data),
  update: (id: string, data: UpdateYapilacakIsDto) =>
    siteApi.put(`/api/yapilacak-isler/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/yapilacak-isler/${id}`),
}
