// ─── E-Posta Şablonu ───────────────────────────────────────────────────────
export interface EpostaSablonu {
  id: string
  siteId: string
  ad: string
  konu: string
  icerikHtml: string
  icerikText?: string
  kategori?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateEpostaSablonuDto {
  ad: string
  konu: string
  icerikHtml: string
  icerikText?: string
  kategori?: string
}

export interface UpdateEpostaSablonuDto {
  ad: string
  konu: string
  icerikHtml: string
  icerikText?: string
  kategori?: string
  isActive: boolean
}

// ─── SMS Şablonu ───────────────────────────────────────────────────────────
export interface SmsSablonu {
  id: string
  siteId: string
  ad: string
  icerik: string
  kategori?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateSmsSablonuDto {
  ad: string
  icerik: string
  kategori?: string
}

export interface UpdateSmsSablonuDto {
  ad: string
  icerik: string
  kategori?: string
  isActive: boolean
}

// ─── Mobil Bildirim Şablonu ────────────────────────────────────────────────
export interface MobilBildirimSablonu {
  id: string
  siteId: string
  ad: string
  baslik: string
  icerik: string
  kategori?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateMobilBildirimSablonuDto {
  ad: string
  baslik: string
  icerik: string
  kategori?: string
}

export interface UpdateMobilBildirimSablonuDto {
  ad: string
  baslik: string
  icerik: string
  kategori?: string
  isActive: boolean
}

// ─── Otomatik Bildirim ─────────────────────────────────────────────────────
export enum OtomatikBildirimOlay {
  AidatVadesi = 1,
  AidatOdendi = 2,
  BorcMakbuzu = 3,
  TahsilatMakbuzu = 4,
  ZiyaretciGiris = 5,
  AracGiris = 6,
  IsEmriOlusturuldu = 7,
  IsEmriTamamlandi = 8,
  Duyuru = 9,
  Diger = 10,
}

export const OtomatikBildirimOlayLabel: Record<OtomatikBildirimOlay, string> = {
  [OtomatikBildirimOlay.AidatVadesi]:       'Aidat Vadesi',
  [OtomatikBildirimOlay.AidatOdendi]:       'Aidat Ödendi',
  [OtomatikBildirimOlay.BorcMakbuzu]:       'Borç Makbuzu',
  [OtomatikBildirimOlay.TahsilatMakbuzu]:   'Tahsilat Makbuzu',
  [OtomatikBildirimOlay.ZiyaretciGiris]:    'Ziyaretçi Girişi',
  [OtomatikBildirimOlay.AracGiris]:         'Araç Girişi',
  [OtomatikBildirimOlay.IsEmriOlusturuldu]: 'İş Emri Oluşturuldu',
  [OtomatikBildirimOlay.IsEmriTamamlandi]:  'İş Emri Tamamlandı',
  [OtomatikBildirimOlay.Duyuru]:            'Duyuru',
  [OtomatikBildirimOlay.Diger]:             'Diğer',
}

export interface OtomatikBildirim {
  id: string
  siteId: string
  olayTipi: OtomatikBildirimOlay
  epostaAktif: boolean
  smsAktif: boolean
  mobilAktif: boolean
  epostaSablonuId?: string
  smsSablonuId?: string
  mobilSablonuId?: string
  epostaSablonuAd?: string
  smsSablonuAd?: string
  mobilSablonuAd?: string
  isActive: boolean
}

export interface UpsertOtomatikBildirimDto {
  olayTipi: OtomatikBildirimOlay
  epostaAktif: boolean
  smsAktif: boolean
  mobilAktif: boolean
  epostaSablonuId?: string
  smsSablonuId?: string
  mobilSablonuId?: string
  isActive: boolean
}

// ─── Telefon Rehberi ───────────────────────────────────────────────────────
export interface TelefonRehberi {
  id: string
  siteId: string
  ad: string
  unvan?: string
  telefon: string
  dahili?: string
  email?: string
  departman?: string
  aciklama?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateTelefonRehberiDto {
  ad: string
  unvan?: string
  telefon: string
  dahili?: string
  email?: string
  departman?: string
  aciklama?: string
}

export interface UpdateTelefonRehberiDto {
  ad: string
  unvan?: string
  telefon: string
  dahili?: string
  email?: string
  departman?: string
  aciklama?: string
  isActive: boolean
}
