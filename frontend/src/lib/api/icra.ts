import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  Avukat, SaveAvukatDto, AvukatSecim,
  TakipAdayFiltre, TakipAdayi, TakipBaslatDto,
  TakipListItem, TakipDetay, UpdateTakipDto, IcrayaVerDto, TakipDurumu,
  IcraDosyasiListItem, IcraDosyasiDetay, UpdateIcraDosyasiDto,
  IcraRaporFiltre, IcraRaporSatiri,
} from '@/types/icra'

export const avukatlarApi = {
  getAll: (search?: string) =>
    siteApi.get<Avukat[]>('/api/avukatlar', { params: { search } }),
  create: (data: SaveAvukatDto) =>
    siteApi.post<{ id: string }>('/api/avukatlar', data),
  update: (id: string, data: SaveAvukatDto) =>
    siteApi.put(`/api/avukatlar/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/avukatlar/${id}`),
}

export const takibeGonderApi = {
  getAdaylar: (filtre: TakipAdayFiltre) =>
    siteApi.get<TakipAdayi[]>('/api/takibe-gonder/adaylar', { params: filtre }),
  baslat: (data: TakipBaslatDto) =>
    siteApi.post<number>('/api/takibe-gonder/baslat', data),
}

export const icraTakipleriApi = {
  getAll: (page = 1, pageSize = 20, search?: string, durum?: TakipDurumu) =>
    siteApi.get<PaginatedResult<TakipListItem>>('/api/icra-takipleri', { params: { page, pageSize, search, durum } }),
  getById: (id: string) =>
    siteApi.get<TakipDetay>(`/api/icra-takipleri/${id}`),
  update: (id: string, data: UpdateTakipDto) =>
    siteApi.put(`/api/icra-takipleri/${id}`, data),
  icrayaVer: (id: string, data: IcrayaVerDto) =>
    siteApi.post<{ id: string }>(`/api/icra-takipleri/${id}/icraya-ver`, data),
  getAvukatlar: () =>
    siteApi.get<AvukatSecim[]>('/api/icra-takipleri/avukatlar'),
}

export const icraDosyalariApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<IcraDosyasiListItem>>('/api/icra-dosyalari', { params: { page, pageSize, search } }),
  getById: (id: string) =>
    siteApi.get<IcraDosyasiDetay>(`/api/icra-dosyalari/${id}`),
  update: (id: string, data: UpdateIcraDosyasiDto) =>
    siteApi.put(`/api/icra-dosyalari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/icra-dosyalari/${id}`),
  getAvukatlar: () =>
    siteApi.get<AvukatSecim[]>('/api/icra-dosyalari/avukatlar'),
}

export const icraRaporuApi = {
  get: (filtre: IcraRaporFiltre) =>
    siteApi.get<IcraRaporSatiri[]>('/api/icra-raporu', { params: filtre }),
  getAvukatlar: () =>
    siteApi.get<AvukatSecim[]>('/api/icra-raporu/avukatlar'),
}
