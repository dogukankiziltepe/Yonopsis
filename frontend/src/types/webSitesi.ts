// ─── Fotoğraf Galerisi ─────────────────────────────────────────────────────
export interface FotografGalerisi {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  imageUrl: string
  sira: number
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateFotografGalerisiDto {
  baslik: string
  aciklama?: string
  imageUrl: string
  sira: number
}

export interface UpdateFotografGalerisiDto {
  baslik: string
  aciklama?: string
  imageUrl: string
  sira: number
  isActive: boolean
}

// ─── Anket ─────────────────────────────────────────────────────────────────
export enum AnketDurum {
  Taslak = 0,
  Aktif = 1,
  Kapandi = 2,
}

export const AnketDurumLabel: Record<AnketDurum, string> = {
  [AnketDurum.Taslak]:  'Taslak',
  [AnketDurum.Aktif]:   'Aktif',
  [AnketDurum.Kapandi]: 'Kapandı',
}

export interface Anket {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  baslangicTarihi?: string
  bitisTarihi?: string
  durum: AnketDurum
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateAnketDto {
  baslik: string
  aciklama?: string
  baslangicTarihi?: string
  bitisTarihi?: string
  durum: AnketDurum
}

export interface UpdateAnketDto {
  baslik: string
  aciklama?: string
  baslangicTarihi?: string
  bitisTarihi?: string
  durum: AnketDurum
  isActive: boolean
}

// ─── Ana Sayfa Ayarı ───────────────────────────────────────────────────────
export interface AnaSayfaAyar {
  id?: string
  siteId?: string
  siteAdi?: string
  slogan?: string
  kisaAciklama?: string
  iletisimTelefon?: string
  iletisimEmail?: string
  adres?: string
  logoUrl?: string
  kapakFotoUrl?: string
}

export interface UpdateAnaSayfaAyarDto {
  siteAdi?: string
  slogan?: string
  kisaAciklama?: string
  iletisimTelefon?: string
  iletisimEmail?: string
  adres?: string
  logoUrl?: string
  kapakFotoUrl?: string
}

// ─── Site Teması ───────────────────────────────────────────────────────────
export interface SiteTemasi {
  id?: string
  siteId?: string
  primaryColor?: string
  secondaryColor?: string
  accentColor?: string
  logoUrl?: string
  faviconUrl?: string
  fontFamily?: string
}

export interface UpdateSiteTemasDto {
  primaryColor?: string
  secondaryColor?: string
  accentColor?: string
  logoUrl?: string
  faviconUrl?: string
  fontFamily?: string
}
