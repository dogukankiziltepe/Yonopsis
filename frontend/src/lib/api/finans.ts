import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  BorcMakbuzu, CreateBorcMakbuzuDto, UpdateBorcMakbuzuDto,
  TahsilatMakbuzu, CreateTahsilatMakbuzuDto, UpdateTahsilatMakbuzuDto,
  Fatura, CreateFaturaDto, UpdateFaturaDto,
  BankaHareketi, CreateBankaHareketiDto, UpdateBankaHareketiDto, BankaHareketiDurum,
  GelirTanimiSecim, TopluBorclandirmaTemplateRequest,
  TopluBorclandirmaPreview, TopluBorclandirmaConfirmRequest, TopluBorclandirmaSonuc,
  CariHesapPicker,
  OdemeMakbuzu, CreateOdemeMakbuzuDto, UpdateOdemeMakbuzuDto,
  GelirTahsilatMakbuzu, CreateGelirTahsilatMakbuzuDto, UpdateGelirTahsilatMakbuzuDto,
  DevirBakiye, CreateDevirBakiyeDto, UpdateDevirBakiyeDto,
  IadeMakbuzu, CreateIadeMakbuzuDto, UpdateIadeMakbuzuDto,
  KisiFinansalDurumSatiri, KisiFinansalDetay,
  BankaHareketiImportPreview, BankaHareketiImportConfirmRequest, BankaHareketiImportSonuc,
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
  getAll: (page = 1, pageSize = 20, kasaBankaId?: string, durum?: BankaHareketiDurum) =>
    siteApi.get<PaginatedResult<BankaHareketi>>('/api/banka-hareketleri', { params: { page, pageSize, kasaBankaId, durum } }),
  create: (data: CreateBankaHareketiDto) =>
    siteApi.post<{ id: string }>('/api/banka-hareketleri', data),
  update: (id: string, data: UpdateBankaHareketiDto) =>
    siteApi.put(`/api/banka-hareketleri/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/banka-hareketleri/${id}`),

  excelImport: {
    downloadTemplate: () =>
      siteApi.get('/api/banka-hareketleri/excel-import/template', { responseType: 'blob' }),
    preview: (file: File) => {
      const form = new FormData()
      form.append('file', file)
      return siteApi.post<BankaHareketiImportPreview>('/api/banka-hareketleri/excel-import/preview', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
    },
    confirm: (kasaBankaId: string, data: BankaHareketiImportConfirmRequest) =>
      siteApi.post<BankaHareketiImportSonuc>('/api/banka-hareketleri/excel-import/confirm', data, { params: { kasaBankaId } }),
  },
}

export const cariHesaplariApi = {
  getAll: (search?: string) =>
    siteApi.get<CariHesapPicker[]>('/api/cari-hesaplar', { params: { search } }),
}

export const odemeMakbuzlariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<OdemeMakbuzu>>('/api/odeme-makbuzlari', { params: { page, pageSize, search } }),
  create: (data: CreateOdemeMakbuzuDto) =>
    siteApi.post<{ id: string }>('/api/odeme-makbuzlari', data),
  update: (id: string, data: UpdateOdemeMakbuzuDto) =>
    siteApi.put(`/api/odeme-makbuzlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/odeme-makbuzlari/${id}`),
}

export const gelirTahsilatMakbuzlariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<GelirTahsilatMakbuzu>>('/api/gelir-tahsilat-makbuzlari', { params: { page, pageSize, search } }),
  create: (data: CreateGelirTahsilatMakbuzuDto) =>
    siteApi.post<{ id: string }>('/api/gelir-tahsilat-makbuzlari', data),
  update: (id: string, data: UpdateGelirTahsilatMakbuzuDto) =>
    siteApi.put(`/api/gelir-tahsilat-makbuzlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/gelir-tahsilat-makbuzlari/${id}`),
}

export const devirBakiyeleriApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<DevirBakiye>>('/api/devir-bakiyeleri', { params: { page, pageSize, search } }),
  create: (data: CreateDevirBakiyeDto) =>
    siteApi.post<{ id: string }>('/api/devir-bakiyeleri', data),
  update: (id: string, data: UpdateDevirBakiyeDto) =>
    siteApi.put(`/api/devir-bakiyeleri/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/devir-bakiyeleri/${id}`),
}

export const iadeMakbuzlariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<IadeMakbuzu>>('/api/iade-makbuzlari', { params: { page, pageSize, search } }),
  create: (data: CreateIadeMakbuzuDto) =>
    siteApi.post<{ id: string }>('/api/iade-makbuzlari', data),
  update: (id: string, data: UpdateIadeMakbuzuDto) =>
    siteApi.put(`/api/iade-makbuzlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/iade-makbuzlari/${id}`),
}

export const kisilerFinansalDurumApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<KisiFinansalDurumSatiri>>('/api/kisiler-finansal-durum', { params: { page, pageSize, search } }),
  getDetay: (personUserId: string) =>
    siteApi.get<KisiFinansalDetay>(`/api/kisiler-finansal-durum/${personUserId}/detay`),
}
