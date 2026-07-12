import { siteApi } from './client'
import type { PaginatedResult } from '@/types/api'
import type { Personel, CreatePersonelDto } from '@/types/personel'
import type {
  UpdatePersonelDto, PersonelFullDetailDto, UpdatePersonelKimlikDto,
  UpdatePersonelMuhasebeEntegrasyonDto, PersonelIzinTuru,
} from '@/types/personelDetail'

export const personelApi = {
  getAll: (params?: { search?: string; isActive?: boolean; page?: number; pageSize?: number }) =>
    siteApi.get<PaginatedResult<Personel>>('/api/personel', { params }),

  getFullDetail: (id: string) =>
    siteApi.get<PersonelFullDetailDto>(`/api/personel/${id}/detail`),

  create: (data: CreatePersonelDto) =>
    siteApi.post<{ id: string }>('/api/personel', data),

  update: (id: string, data: UpdatePersonelDto) =>
    siteApi.put(`/api/personel/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/personel/${id}`),

  updateKimlik: (id: string, data: UpdatePersonelKimlikDto) =>
    siteApi.put(`/api/personel/${id}/kimlik`, data),

  updateMuhasebeEntegrasyon: (id: string, data: UpdatePersonelMuhasebeEntegrasyonDto) =>
    siteApi.put(`/api/personel/${id}/muhasebe-entegrasyon`, data),

  addTelefon: (id: string, data: { phoneNumber: string; label?: string | null }) =>
    siteApi.post<{ id: string }>(`/api/personel/${id}/telefonlar`, data),
  updateTelefon: (telefonId: string, data: { phoneNumber: string; label?: string | null }) =>
    siteApi.put(`/api/personel/telefonlar/${telefonId}`, data),
  deleteTelefon: (telefonId: string) =>
    siteApi.delete(`/api/personel/telefonlar/${telefonId}`),

  addAcilDurumKisi: (id: string, data: { adSoyad: string; yakinlik?: string | null; telefon?: string | null }) =>
    siteApi.post<{ id: string }>(`/api/personel/${id}/acil-durum-kisileri`, data),
  updateAcilDurumKisi: (kisiId: string, data: { adSoyad: string; yakinlik?: string | null; telefon?: string | null }) =>
    siteApi.put(`/api/personel/acil-durum-kisileri/${kisiId}`, data),
  deleteAcilDurumKisi: (kisiId: string) =>
    siteApi.delete(`/api/personel/acil-durum-kisileri/${kisiId}`),

  addEgitim: (id: string, data: {
    egitiminKonusu: string; egitmen?: string | null; egitimYeri?: string | null
    baslamaTarihi?: string | null; bitisTarihi?: string | null; toplamSaat?: number | null
  }) => siteApi.post<{ id: string }>(`/api/personel/${id}/egitimler`, data),
  updateEgitim: (egitimId: string, data: {
    egitiminKonusu: string; egitmen?: string | null; egitimYeri?: string | null
    baslamaTarihi?: string | null; bitisTarihi?: string | null; toplamSaat?: number | null
  }) => siteApi.put(`/api/personel/egitimler/${egitimId}`, data),
  deleteEgitim: (egitimId: string) =>
    siteApi.delete(`/api/personel/egitimler/${egitimId}`),

  addIzin: (id: string, data: {
    baslangicTarihi: string; bitisTarihi: string; izinTuru: PersonelIzinTuru; aciklama?: string | null
  }) => siteApi.post<{ id: string }>(`/api/personel/${id}/izinler`, data),
  updateIzin: (izinId: string, data: {
    baslangicTarihi: string; bitisTarihi: string; izinTuru: PersonelIzinTuru; aciklama?: string | null
  }) => siteApi.put(`/api/personel/izinler/${izinId}`, data),
  deleteIzin: (izinId: string) =>
    siteApi.delete(`/api/personel/izinler/${izinId}`),
}
