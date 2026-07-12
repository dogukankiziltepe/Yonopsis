// ─── Ajanda Etkinlik ───────────────────────────────────────────────────────
export interface AjandaEtkinlik {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  baslangicTarihi: string
  bitisTarihi?: string
  konum?: string
  renk?: string
  tumGun: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateAjandaEtkinlikDto {
  baslik: string
  aciklama?: string
  baslangicTarihi: string
  bitisTarihi?: string
  konum?: string
  renk?: string
  tumGun: boolean
}

export interface UpdateAjandaEtkinlikDto {
  baslik: string
  aciklama?: string
  baslangicTarihi: string
  bitisTarihi?: string
  konum?: string
  renk?: string
  tumGun: boolean
  isActive: boolean
}

// ─── Toplantı ──────────────────────────────────────────────────────────────
export enum ToplamtiDurum {
  Planlandilar = 0,
  Tamamlandi = 1,
  Iptal = 2,
}

export const ToplamtiDurumLabel: Record<ToplamtiDurum, string> = {
  [ToplamtiDurum.Planlandilar]: 'Planlandı',
  [ToplamtiDurum.Tamamlandi]:   'Tamamlandı',
  [ToplamtiDurum.Iptal]:        'İptal',
}

export interface Toplanti {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  gundem?: string
  toplamtiTarihi: string
  konum?: string
  durum: ToplamtiDurum
  katilimcilar?: string
  kararlar?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateToplantiDto {
  baslik: string
  aciklama?: string
  gundem?: string
  toplamtiTarihi: string
  konum?: string
  durum: ToplamtiDurum
  katilimcilar?: string
  kararlar?: string
}

export interface UpdateToplantiDto {
  baslik: string
  aciklama?: string
  gundem?: string
  toplamtiTarihi: string
  konum?: string
  durum: ToplamtiDurum
  katilimcilar?: string
  kararlar?: string
  isActive: boolean
}

// ─── Teklif ────────────────────────────────────────────────────────────────
export enum TeklifDurum {
  Beklemede = 0,
  Onaylandi = 1,
  Reddedildi = 2,
  Iptal = 3,
}

export const TeklifDurumLabel: Record<TeklifDurum, string> = {
  [TeklifDurum.Beklemede]:  'Beklemede',
  [TeklifDurum.Onaylandi]:  'Onaylandı',
  [TeklifDurum.Reddedildi]: 'Reddedildi',
  [TeklifDurum.Iptal]:      'İptal',
}

export interface Teklif {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  tedarikciAdi?: string
  tutar?: number
  teklifTarihi: string
  gecerlilikTarihi?: string
  durum: TeklifDurum
  notlar?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateTeklifDto {
  baslik: string
  aciklama?: string
  tedarikciAdi?: string
  tutar?: number
  teklifTarihi: string
  gecerlilikTarihi?: string
  durum: TeklifDurum
  notlar?: string
}

export interface UpdateTeklifDto {
  baslik: string
  aciklama?: string
  tedarikciAdi?: string
  tutar?: number
  teklifTarihi: string
  gecerlilikTarihi?: string
  durum: TeklifDurum
  notlar?: string
  isActive: boolean
}

// ─── Yapılacak İş ──────────────────────────────────────────────────────────
export enum YapilacakIsDurum {
  Beklemede = 0,
  Devam = 1,
  Tamamlandi = 2,
}

export const YapilacakIsDurumLabel: Record<YapilacakIsDurum, string> = {
  [YapilacakIsDurum.Beklemede]:  'Beklemede',
  [YapilacakIsDurum.Devam]:      'Devam Ediyor',
  [YapilacakIsDurum.Tamamlandi]: 'Tamamlandı',
}

export enum YapilacakIsOncelik {
  Dusuk = 1,
  Normal = 2,
  Yuksek = 3,
}

export const YapilacakIsOncelikLabel: Record<YapilacakIsOncelik, string> = {
  [YapilacakIsOncelik.Dusuk]:  'Düşük',
  [YapilacakIsOncelik.Normal]: 'Normal',
  [YapilacakIsOncelik.Yuksek]: 'Yüksek',
}

export interface YapilacakIs {
  id: string
  siteId: string
  baslik: string
  aciklama?: string
  atananKisi?: string
  oncelik: YapilacakIsOncelik
  tamamlanmaTarihi?: string
  durum: YapilacakIsDurum
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateYapilacakIsDto {
  baslik: string
  aciklama?: string
  atananKisi?: string
  oncelik: YapilacakIsOncelik
  tamamlanmaTarihi?: string
  durum: YapilacakIsDurum
}

export interface UpdateYapilacakIsDto {
  baslik: string
  aciklama?: string
  atananKisi?: string
  oncelik: YapilacakIsOncelik
  tamamlanmaTarihi?: string
  durum: YapilacakIsDurum
  isActive: boolean
}
