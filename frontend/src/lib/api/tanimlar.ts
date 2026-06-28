import { siteApi } from './client'
import {
  GelirGrubu, CreateGelirGrubuDto, UpdateGelirGrubuDto,
  GiderGrubu, CreateGiderGrubuDto, UpdateGiderGrubuDto,
  GelirTanimi, CreateGelirTanimiDto, UpdateGelirTanimiDto,
  GiderTanimi, CreateGiderTanimiDto, UpdateGiderTanimiDto,
  KasaBanka, CreateKasaBankaDto, UpdateKasaBankaDto,
  Tesis, CreateTesisDto, UpdateTesisDto,
} from '@/types/tanimlar'

export const gelirGruplariApi = {
  getAll: () => siteApi.get<GelirGrubu[]>('/api/gelir-gruplari'),
  create: (data: CreateGelirGrubuDto) => siteApi.post<{ id: string }>('/api/gelir-gruplari', data),
  update: (id: string, data: UpdateGelirGrubuDto) => siteApi.put(`/api/gelir-gruplari/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/gelir-gruplari/${id}`),
}

export const giderGruplariApi = {
  getAll: () => siteApi.get<GiderGrubu[]>('/api/gider-gruplari'),
  create: (data: CreateGiderGrubuDto) => siteApi.post<{ id: string }>('/api/gider-gruplari', data),
  update: (id: string, data: UpdateGiderGrubuDto) => siteApi.put(`/api/gider-gruplari/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/gider-gruplari/${id}`),
}

export const gelirTanimlariApi = {
  getAll: () => siteApi.get<GelirTanimi[]>('/api/gelir-tanimlari'),
  create: (data: CreateGelirTanimiDto) => siteApi.post<{ id: string }>('/api/gelir-tanimlari', data),
  update: (id: string, data: UpdateGelirTanimiDto) => siteApi.put(`/api/gelir-tanimlari/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/gelir-tanimlari/${id}`),
}

export const giderTanimlariApi = {
  getAll: () => siteApi.get<GiderTanimi[]>('/api/gider-tanimlari'),
  create: (data: CreateGiderTanimiDto) => siteApi.post<{ id: string }>('/api/gider-tanimlari', data),
  update: (id: string, data: UpdateGiderTanimiDto) => siteApi.put(`/api/gider-tanimlari/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/gider-tanimlari/${id}`),
}

export const kasaBankaApi = {
  getAll: () => siteApi.get<KasaBanka[]>('/api/kasa-banka'),
  create: (data: CreateKasaBankaDto) => siteApi.post<{ id: string }>('/api/kasa-banka', data),
  update: (id: string, data: UpdateKasaBankaDto) => siteApi.put(`/api/kasa-banka/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/kasa-banka/${id}`),
}

export const tesislerApi = {
  getAll: () => siteApi.get<Tesis[]>('/api/tesisler'),
  create: (data: CreateTesisDto) => siteApi.post<{ id: string }>('/api/tesisler', data),
  update: (id: string, data: UpdateTesisDto) => siteApi.put(`/api/tesisler/${id}`, data),
  delete: (id: string) => siteApi.delete(`/api/tesisler/${id}`),
}
