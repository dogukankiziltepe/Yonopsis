import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  AnaSayac, CreateAnaSayacDto, UpdateAnaSayacDto,
  DaireSayac, CreateDaireSayacDto, UpdateDaireSayacDto,
  SayacOkuma, CreateSayacOkumaDto, UpdateSayacOkumaDto,
  BirimFiyat, CreateBirimFiyatDto, UpdateBirimFiyatDto,
} from '@/types/sayac'
import type { SayacTipi } from '@/types/sayac'

export const anaSayaclarApi = {
  getAll: (page = 1, pageSize = 50, search?: string, tip?: SayacTipi) =>
    siteApi.get<PaginatedResult<AnaSayac>>('/api/ana-sayaclar', { params: { page, pageSize, search, tip } }),
  getAllList: () =>
    siteApi.get<AnaSayac[]>('/api/ana-sayaclar/all'),
  create: (data: CreateAnaSayacDto) =>
    siteApi.post<{ id: string }>('/api/ana-sayaclar', data),
  update: (id: string, data: UpdateAnaSayacDto) =>
    siteApi.put(`/api/ana-sayaclar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/ana-sayaclar/${id}`),
}

export const daireSayaclarApi = {
  getAll: (page = 1, pageSize = 50, search?: string, anaSayacId?: string, tip?: SayacTipi) =>
    siteApi.get<PaginatedResult<DaireSayac>>('/api/daire-sayaclar', { params: { page, pageSize, search, anaSayacId, tip } }),
  create: (data: CreateDaireSayacDto) =>
    siteApi.post<{ id: string }>('/api/daire-sayaclar', data),
  update: (id: string, data: UpdateDaireSayacDto) =>
    siteApi.put(`/api/daire-sayaclar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/daire-sayaclar/${id}`),
}

export const sayacOkumalarApi = {
  getAll: (page = 1, pageSize = 30, anaSayacId?: string, daireSayacId?: string) =>
    siteApi.get<PaginatedResult<SayacOkuma>>('/api/sayac-okumalar', { params: { page, pageSize, anaSayacId, daireSayacId } }),
  create: (data: CreateSayacOkumaDto) =>
    siteApi.post<{ id: string }>('/api/sayac-okumalar', data),
  update: (id: string, data: UpdateSayacOkumaDto) =>
    siteApi.put(`/api/sayac-okumalar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/sayac-okumalar/${id}`),
}

export const birimFiyatlarApi = {
  getAll: (tip?: SayacTipi) =>
    siteApi.get<BirimFiyat[]>('/api/birim-fiyatlar', { params: { tip } }),
  create: (data: CreateBirimFiyatDto) =>
    siteApi.post<{ id: string }>('/api/birim-fiyatlar', data),
  update: (id: string, data: UpdateBirimFiyatDto) =>
    siteApi.put(`/api/birim-fiyatlar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/birim-fiyatlar/${id}`),
}
