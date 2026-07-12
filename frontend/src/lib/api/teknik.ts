import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  Departman, CreateDepartmanDto, UpdateDepartmanDto,
  TalepTipi, CreateTalepTipiDto, UpdateTalepTipiDto,
  OrtakAlan, CreateOrtakAlanDto, UpdateOrtakAlanDto,
  IsEmri, CreateIsEmriDto, UpdateIsEmriDto,
} from '@/types/teknik'
import type { IsEmriDurum } from '@/types/teknik'

export const departmanlarApi = {
  getAll: () => siteApi.get<Departman[]>('/api/departmanlar'),
  create: (data: CreateDepartmanDto) => siteApi.post<{ id: string }>('/api/departmanlar', data),
  update: (id: string, data: UpdateDepartmanDto) => siteApi.put(`/api/departmanlar/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/departmanlar/${id}`),
}

export const talepTipleriApi = {
  getAll: () => siteApi.get<TalepTipi[]>('/api/talep-tipleri'),
  create: (data: CreateTalepTipiDto) => siteApi.post<{ id: string }>('/api/talep-tipleri', data),
  update: (id: string, data: UpdateTalepTipiDto) => siteApi.put(`/api/talep-tipleri/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/talep-tipleri/${id}`),
}

export const ortakAlanlarApi = {
  getAll: () => siteApi.get<OrtakAlan[]>('/api/ortak-alanlar'),
  create: (data: CreateOrtakAlanDto) => siteApi.post<{ id: string }>('/api/ortak-alanlar', data),
  update: (id: string, data: UpdateOrtakAlanDto) => siteApi.put(`/api/ortak-alanlar/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/ortak-alanlar/${id}`),
}

export const isEmirleriApi = {
  getAll: (page = 1, pageSize = 20, search?: string, durum?: IsEmriDurum, departmanId?: string) =>
    siteApi.get<PaginatedResult<IsEmri>>('/api/is-emirleri', { params: { page, pageSize, search, durum, departmanId } }),
  getPano: () =>
    siteApi.get<Record<string, IsEmri[]>>('/api/is-emirleri/pano'),
  create: (data: CreateIsEmriDto) =>
    siteApi.post<{ id: string }>('/api/is-emirleri', data),
  update: (id: string, data: UpdateIsEmriDto) =>
    siteApi.put(`/api/is-emirleri/${id}`, data),
  updateDurum: (id: string, durum: IsEmriDurum) =>
    siteApi.patch(`/api/is-emirleri/${id}/durum`, durum),
  delete: (id: string) =>
    siteApi.delete(`/api/is-emirleri/${id}`),
}
