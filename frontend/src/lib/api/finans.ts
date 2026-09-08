import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  BorcMakbuzu, CreateBorcMakbuzuDto, UpdateBorcMakbuzuDto,
  TahsilatMakbuzu, CreateTahsilatMakbuzuDto, UpdateTahsilatMakbuzuDto,
  Fatura, CreateFaturaDto, UpdateFaturaDto,
  BankaHareketi, CreateBankaHareketiDto, UpdateBankaHareketiDto,
  GelirTanimiSecim, TopluBorclandirmaTemplateRequest,
  TopluBorclandirmaPreview, TopluBorclandirmaConfirmRequest, TopluBorclandirmaSonuc,
} from '@/types/finans'

export const borcMakbuzlariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<BorcMakbuzu>>('/api/borc-makbuzlari', { params: { page, pageSize, search } }),
  create: (data: CreateBorcMakbuzuDto) =>
    siteApi.post<{ id: string }>('/api/borc-makbuzlari', data),
  update: (id: string, data: UpdateBorcMakbuzuDto) =>
    siteApi.put(`/api/borc-makbuzlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/borc-makbuzlari/${id}`),

  topluBorclandirma: {
    getAktifGelirTanimlari: () =>
      siteApi.get<GelirTanimiSecim[]>('/api/borc-makbuzlari/toplu-borclandirma/gelir-tanimlari-aktif'),
    downloadTemplate: (data: TopluBorclandirmaTemplateRequest) =>
      siteApi.post('/api/borc-makbuzlari/toplu-borclandirma/template', data, { responseType: 'blob' }),
    preview: (file: File) => {
      const form = new FormData()
      form.append('file', file)
      return siteApi.post<TopluBorclandirmaPreview>('/api/borc-makbuzlari/toplu-borclandirma/preview', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
    },
    confirm: (data: TopluBorclandirmaConfirmRequest) =>
      siteApi.post<TopluBorclandirmaSonuc>('/api/borc-makbuzlari/toplu-borclandirma/confirm', data),
  },
}

export const tahsilatMakbuzlariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<TahsilatMakbuzu>>('/api/tahsilat-makbuzlari', { params: { page, pageSize, search } }),
  create: (data: CreateTahsilatMakbuzuDto) =>
    siteApi.post<{ id: string }>('/api/tahsilat-makbuzlari', data),
  update: (id: string, data: UpdateTahsilatMakbuzuDto) =>
    siteApi.put(`/api/tahsilat-makbuzlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/tahsilat-makbuzlari/${id}`),
}

export const gelirFaturalariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<Fatura>>('/api/faturalar/gelir', { params: { page, pageSize, search } }),
  create: (data: CreateFaturaDto) =>
    siteApi.post<{ id: string }>('/api/faturalar', data),
  update: (id: string, data: UpdateFaturaDto) =>
    siteApi.put(`/api/faturalar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/faturalar/${id}`),
}

export const giderFaturalariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<Fatura>>('/api/faturalar/gider', { params: { page, pageSize, search } }),
  create: (data: CreateFaturaDto) =>
    siteApi.post<{ id: string }>('/api/faturalar', data),
  update: (id: string, data: UpdateFaturaDto) =>
    siteApi.put(`/api/faturalar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/faturalar/${id}`),
}

export const bankaHareketleriApi = {
  getAll: (page = 1, pageSize = 20, kasaBankaId?: string) =>
    siteApi.get<PaginatedResult<BankaHareketi>>('/api/banka-hareketleri', { params: { page, pageSize, kasaBankaId } }),
  create: (data: CreateBankaHareketiDto) =>
    siteApi.post<{ id: string }>('/api/banka-hareketleri', data),
  update: (id: string, data: UpdateBankaHareketiDto) =>
    siteApi.put(`/api/banka-hareketleri/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/banka-hareketleri/${id}`),
}
