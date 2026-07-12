import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type {
  EpostaSablonu, CreateEpostaSablonuDto, UpdateEpostaSablonuDto,
  SmsSablonu, CreateSmsSablonuDto, UpdateSmsSablonuDto,
  MobilBildirimSablonu, CreateMobilBildirimSablonuDto, UpdateMobilBildirimSablonuDto,
  OtomatikBildirim, UpsertOtomatikBildirimDto,
  TelefonRehberi, CreateTelefonRehberiDto, UpdateTelefonRehberiDto,
} from '@/types/iletisimYonetim'
import type { OtomatikBildirimOlay } from '@/types/iletisimYonetim'

export const epostaSablonlariApi = {
  getAll: (page = 1, pageSize = 50, search?: string) =>
    siteApi.get<PaginatedResult<EpostaSablonu>>('/api/eposta-sablonlari', { params: { page, pageSize, search } }),
  create: (data: CreateEpostaSablonuDto) =>
    siteApi.post<{ id: string }>('/api/eposta-sablonlari', data),
  update: (id: string, data: UpdateEpostaSablonuDto) =>
    siteApi.put(`/api/eposta-sablonlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/eposta-sablonlari/${id}`),
}

export const smsSablonlariApi = {
  getAll: (page = 1, pageSize = 50, search?: string) =>
    siteApi.get<PaginatedResult<SmsSablonu>>('/api/sms-sablonlari', { params: { page, pageSize, search } }),
  create: (data: CreateSmsSablonuDto) =>
    siteApi.post<{ id: string }>('/api/sms-sablonlari', data),
  update: (id: string, data: UpdateSmsSablonuDto) =>
    siteApi.put(`/api/sms-sablonlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/sms-sablonlari/${id}`),
}

export const mobilBildirimSablonlariApi = {
  getAll: (page = 1, pageSize = 50, search?: string) =>
    siteApi.get<PaginatedResult<MobilBildirimSablonu>>('/api/mobil-bildirim-sablonlari', { params: { page, pageSize, search } }),
  create: (data: CreateMobilBildirimSablonuDto) =>
    siteApi.post<{ id: string }>('/api/mobil-bildirim-sablonlari', data),
  update: (id: string, data: UpdateMobilBildirimSablonuDto) =>
    siteApi.put(`/api/mobil-bildirim-sablonlari/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/mobil-bildirim-sablonlari/${id}`),
}

export const otomatikBildirimlerApi = {
  getAll: () =>
    siteApi.get<OtomatikBildirim[]>('/api/otomatik-bildirimler'),
  upsert: (data: UpsertOtomatikBildirimDto) =>
    siteApi.put('/api/otomatik-bildirimler', data),
}

export const telefonRehberiApi = {
  getAll: (page = 1, pageSize = 50, search?: string) =>
    siteApi.get<PaginatedResult<TelefonRehberi>>('/api/telefon-rehberi', { params: { page, pageSize, search } }),
  create: (data: CreateTelefonRehberiDto) =>
    siteApi.post<{ id: string }>('/api/telefon-rehberi', data),
  update: (id: string, data: UpdateTelefonRehberiDto) =>
    siteApi.put(`/api/telefon-rehberi/${id}`, data),
  delete: (id: string) =>
    siteApi.delete(`/api/telefon-rehberi/${id}`),
}
