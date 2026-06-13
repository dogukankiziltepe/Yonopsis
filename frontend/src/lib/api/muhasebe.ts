import { siteApi } from './client'
import { PaginatedResult } from '@/types/api'
import {
  CariBakiye,
  CariTuru,
  CreateCariHesapDto,
  CreateFisDto,
  CreateHesapDto,
  Defter,
  Donem,
  FisDetay,
  FisDetaylarItem,
  FisDurumu,
  FisListItem,
  FisTuru,
  HesapDetail,
  HesapListItem,
  HesapNode,
  Mizan,
  UpdateFisDto,
  UpdateHesapDto,
  YevmiyeDefteri,
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

  // Dönemler
  getDonemler: () => siteApi.get<Donem[]>('/api/muhasebe/donemler'),
  getAktifDonem: () => siteApi.get<Donem | null>('/api/muhasebe/donemler/aktif'),
  createDonem: (data: { yil: number; ad?: string }) =>
    siteApi.post<{ id: string }>('/api/muhasebe/donemler', data),

  // Fişler
  getFisler: (params?: {
    from?: string
    to?: string
    tur?: FisTuru
    durum?: FisDurumu
    q?: string
    page?: number
    pageSize?: number
  }) => siteApi.get<PaginatedResult<FisListItem>>('/api/muhasebe/fisler', { params }),

  getFis: (id: string) => siteApi.get<FisDetay>(`/api/muhasebe/fisler/${id}`),

  createFis: (data: CreateFisDto) =>
    siteApi.post<{ id: string }>('/api/muhasebe/fisler', data),

  updateFis: (id: string, data: UpdateFisDto) =>
    siteApi.put(`/api/muhasebe/fisler/${id}`, data),

  onaylaFis: (id: string) => siteApi.post(`/api/muhasebe/fisler/${id}/onayla`),

  iptalFis: (id: string) => siteApi.post(`/api/muhasebe/fisler/${id}/iptal`),

  deleteFis: (id: string) => siteApi.delete(`/api/muhasebe/fisler/${id}`),

  getFisDetaylari: (params?: {
    hesapId?: string
    from?: string
    to?: string
    durum?: FisDurumu
    page?: number
    pageSize?: number
  }) => siteApi.get<PaginatedResult<FisDetaylarItem>>('/api/muhasebe/fis-detaylari', { params }),

  // Defterler & Raporlar
  getYevmiye: (from?: string, to?: string) =>
    siteApi.get<YevmiyeDefteri>('/api/muhasebe/defterler/yevmiye', { params: { from, to } }),
  getKebir: (hesapId: string, from?: string, to?: string) =>
    siteApi.get<Defter>('/api/muhasebe/defterler/kebir', { params: { hesapId, from, to } }),
  getMuavin: (hesapId: string, from?: string, to?: string) =>
    siteApi.get<Defter>('/api/muhasebe/defterler/muavin', { params: { hesapId, from, to } }),
  getMizan: (from?: string, to?: string) =>
    siteApi.get<Mizan>('/api/muhasebe/raporlar/mizan', { params: { from, to } }),
  getCariEkstre: (hesapId: string, from?: string, to?: string) =>
    siteApi.get<Defter>('/api/muhasebe/raporlar/cari-ekstre', { params: { hesapId, from, to } }),
  getBorcAlacak: (cariTuru?: CariTuru) =>
    siteApi.get<CariBakiye[]>('/api/muhasebe/raporlar/borc-alacak', { params: { cariTuru } }),
}
