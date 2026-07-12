import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  ZiyaretciGirisCikis, CreateZiyaretciGirisCikisDto, UpdateZiyaretciGirisCikisDto,
  AracGirisCikis, CreateAracGirisCikisDto, UpdateAracGirisCikisDto,
  Olay, CreateOlayDto, UpdateOlayDto,
  KayipEsya, CreateKayipEsyaDto, UpdateKayipEsyaDto,
} from '@/types/guvenlik'
import type { OlayDurum, KayipEsyaDurum } from '@/types/guvenlik'

export const ziyaretciGirisCikisApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<ZiyaretciGirisCikis>>('/api/ziyaretci-giris-cikis', { params: { page, pageSize, search } }),
  create: (data: CreateZiyaretciGirisCikisDto) =>
    siteApi.post<{ id: string }>('/api/ziyaretci-giris-cikis', data),
  update: (id: string, data: UpdateZiyaretciGirisCikisDto) =>
    siteApi.put(`/api/ziyaretci-giris-cikis/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/ziyaretci-giris-cikis/${id}`),
}

export const aracGirisCikisApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<AracGirisCikis>>('/api/arac-giris-cikis', { params: { page, pageSize, search } }),
  create: (data: CreateAracGirisCikisDto) =>
    siteApi.post<{ id: string }>('/api/arac-giris-cikis', data),
  update: (id: string, data: UpdateAracGirisCikisDto) =>
    siteApi.put(`/api/arac-giris-cikis/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/arac-giris-cikis/${id}`),
}

export const olaylarApi = {
  getAll: (page = 1, pageSize = 20, search?: string, durum?: OlayDurum) =>
    siteApi.get<PaginatedResult<Olay>>('/api/olaylar', { params: { page, pageSize, search, durum } }),
  create: (data: CreateOlayDto) =>
    siteApi.post<{ id: string }>('/api/olaylar', data),
  update: (id: string, data: UpdateOlayDto) =>
    siteApi.put(`/api/olaylar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/olaylar/${id}`),
}

export const kayipEsyaApi = {
  getAll: (page = 1, pageSize = 20, search?: string, durum?: KayipEsyaDurum) =>
    siteApi.get<PaginatedResult<KayipEsya>>('/api/kayip-esya', { params: { page, pageSize, search, durum } }),
  create: (data: CreateKayipEsyaDto) =>
    siteApi.post<{ id: string }>('/api/kayip-esya', data),
  update: (id: string, data: UpdateKayipEsyaDto) =>
    siteApi.put(`/api/kayip-esya/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/kayip-esya/${id}`),
}
