// ── Enums ────────────────────────────────────────────────────────────────────
export enum IsEmriOncelik { Dusuk = 1, Normal = 2, Yuksek = 3, Kritik = 4 }
export enum IsEmriDurum { YeniTalep = 0, Atandi = 1, Devam = 2, Tamamlandi = 3, Iptal = 4 }

export const IsEmriOncelikLabel: Record<IsEmriOncelik, string> = {
  [IsEmriOncelik.Dusuk]: 'Low',
  [IsEmriOncelik.Normal]: 'Normal',
  [IsEmriOncelik.Yuksek]: 'High',
  [IsEmriOncelik.Kritik]: 'Critical',
}

export const IsEmriDurumLabel: Record<IsEmriDurum, string> = {
  [IsEmriDurum.YeniTalep]: 'New Request',
  [IsEmriDurum.Atandi]: 'Assigned',
  [IsEmriDurum.Devam]: 'In Progress',
  [IsEmriDurum.Tamamlandi]: 'Completed',
  [IsEmriDurum.Iptal]: 'Cancelled',
}

// ── Departman ────────────────────────────────────────────────────────────────
export interface Departman {
  id: string
  ad: string
  aciklama?: string
  isActive: boolean
  createdAt: string
}

export interface CreateDepartmanDto {
  ad: string
  aciklama?: string
}

export interface UpdateDepartmanDto {
  ad: string
  aciklama?: string
  isActive: boolean
}

// ── TalepTipi ────────────────────────────────────────────────────────────────
export interface TalepTipi {
  id: string
  ad: string
  aciklama?: string
  isActive: boolean
  createdAt: string
}

export interface CreateTalepTipiDto {
  ad: string
  aciklama?: string
}

export interface UpdateTalepTipiDto {
  ad: string
  aciklama?: string
  isActive: boolean
}

// ── OrtakAlan ────────────────────────────────────────────────────────────────
export interface OrtakAlan {
  id: string
  ad: string
  aciklama?: string
  konum?: string
  isActive: boolean
  createdAt: string
}

export interface CreateOrtakAlanDto {
  ad: string
  aciklama?: string
  konum?: string
}

export interface UpdateOrtakAlanDto {
  ad: string
  aciklama?: string
  konum?: string
  isActive: boolean
}

// ── IsEmri ───────────────────────────────────────────────────────────────────
export interface IsEmri {
  id: string
  baslik: string
  aciklama?: string
  talepTipiId?: string
  talepTipiAdi?: string
  departmanId?: string
  departmanAdi?: string
  ortakAlanId?: string
  ortakAlanAdi?: string
  unitId?: string
  unitDoorNumber?: string
  oncelik: IsEmriOncelik
  durum: IsEmriDurum
  atananKisiId?: string
  atananKisiAdi?: string
  islemBaslangic?: string
  islemBitis?: string
  notlar?: string
  createdAt: string
}

export interface CreateIsEmriDto {
  baslik: string
  aciklama?: string
  talepTipiId?: string
  departmanId?: string
  ortakAlanId?: string
  unitId?: string
  oncelik: IsEmriOncelik
  atananKisiAdi?: string
  islemBaslangic?: string
  notlar?: string
}

export interface UpdateIsEmriDto extends CreateIsEmriDto {
  durum: IsEmriDurum
  islemBitis?: string
}
