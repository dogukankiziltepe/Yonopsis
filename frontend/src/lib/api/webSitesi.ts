import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  FotografGalerisi, CreateFotografGalerisiDto, UpdateFotografGalerisiDto,
  Anket, CreateAnketDto, UpdateAnketDto,
  AnaSayfaAyar, UpdateAnaSayfaAyarDto,
  SiteTemasi, UpdateSiteTemasDto,
} from '@/types/webSitesi'
import type { AnketDurum } from '@/types/webSitesi'

export const fotografGalerisiApi = {
  getAll: (page = 1, pageSize = 50, search?: string) =>
    siteApi.get<PaginatedResult<FotografGalerisi>>('/api/fotograf-galerisi', { params: { page, pageSize, search } }),
  create: (data: CreateFotografGalerisiDto) =>
    siteApi.post<{ id: string }>('/api/fotograf-galerisi', data),
  update: (id: string, data: UpdateFotografGalerisiDto) =>
    siteApi.put(`/api/fotograf-galerisi/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/fotograf-galerisi/${id}`),
}

export const anketlerApi = {
  getAll: (page = 1, pageSize = 50, search?: string, durum?: AnketDurum) =>
    siteApi.get<PaginatedResult<Anket>>('/api/anketler', { params: { page, pageSize, search, durum } }),
  create: (data: CreateAnketDto) =>
    siteApi.post<{ id: string }>('/api/anketler', data),
  update: (id: string, data: UpdateAnketDto) =>
    siteApi.put(`/api/anketler/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/anketler/${id}`),
}

export const webSiteAyarlariApi = {
  getAnaSayfa: () =>
    siteApi.get<AnaSayfaAyar>('/api/web-site-ayarlari/ana-sayfa'),
  updateAnaSayfa: (data: UpdateAnaSayfaAyarDto) =>
    siteApi.put('/api/web-site-ayarlari/ana-sayfa', data),
  getTema: () =>
    siteApi.get<SiteTemasi>('/api/web-site-ayarlari/tema'),
  updateTema: (data: UpdateSiteTemasDto) =>
    siteApi.put('/api/web-site-ayarlari/tema', data),
}
