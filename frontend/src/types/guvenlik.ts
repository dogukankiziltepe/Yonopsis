// ── Enums ────────────────────────────────────────────────────────────────────
export enum AracTipi { Otomobil = 1, Kamyon = 2, Motosiklet = 3, Minibus = 4, Diger = 5 }
export enum OlayTipi { Hirsizlik = 1, Vandalizm = 2, Kaza = 3, Kavga = 4, Yangin = 5, Diger = 6 }
export enum OlayDurum { Acik = 0, Inceleniyor = 1, Kapandi = 2 }
export enum KayipEsyaDurum { Beklemede = 0, Bulundu = 1, TeslimEdildi = 2 }

export const AracTipiLabel: Record<AracTipi, string> = {
  [AracTipi.Otomobil]: 'Car',
  [AracTipi.Kamyon]: 'Truck',
  [AracTipi.Motosiklet]: 'Motorcycle',
  [AracTipi.Minibus]: 'Minibus',
  [AracTipi.Diger]: 'Other',
}

export const OlayTipiLabel: Record<OlayTipi, string> = {
  [OlayTipi.Hirsizlik]: 'Theft',
  [OlayTipi.Vandalizm]: 'Vandalism',
  [OlayTipi.Kaza]: 'Accident',
  [OlayTipi.Kavga]: 'Fight',
  [OlayTipi.Yangin]: 'Fire',
  [OlayTipi.Diger]: 'Other',
}

export const OlayDurumLabel: Record<OlayDurum, string> = {
  [OlayDurum.Acik]: 'Open',
  [OlayDurum.Inceleniyor]: 'Under Review',
  [OlayDurum.Kapandi]: 'Closed',
}

export const KayipEsyaDurumLabel: Record<KayipEsyaDurum, string> = {
  [KayipEsyaDurum.Beklemede]: 'Awaiting',
  [KayipEsyaDurum.Bulundu]: 'Found',
  [KayipEsyaDurum.TeslimEdildi]: 'Returned',
}

// ── Ziyaretçi Giriş Çıkış ────────────────────────────────────────────────────
export interface ZiyaretciGirisCikis {
  id: string
  gelensAdi: string
  geldigiKisi?: string
  unitId?: string
  unitDoorNumber?: string
  ziyaretAmaci?: string
  girisSaati: string
  cikisSaati?: string
  plaka?: string
  aciklama?: string
  createdAt: string
}

export interface CreateZiyaretciGirisCikisDto {
  gelensAdi: string
  geldigiKisi?: string
  unitId?: string
  ziyaretAmaci?: string
  girisSaati: string
  plaka?: string
  aciklama?: string
}

export interface UpdateZiyaretciGirisCikisDto extends CreateZiyaretciGirisCikisDto {
  cikisSaati?: string
}

// ── Araç Giriş Çıkış ─────────────────────────────────────────────────────────
export interface AracGirisCikis {
  id: string
  plaka: string
  suruculAdi?: string
  unitId?: string
  unitDoorNumber?: string
  aracTipi?: AracTipi
  girisSaati: string
  cikisSaati?: string
  aciklama?: string
  createdAt: string
}

export interface CreateAracGirisCikisDto {
  plaka: string
  suruculAdi?: string
  unitId?: string
  aracTipi?: AracTipi
  girisSaati: string
  aciklama?: string
}

export interface UpdateAracGirisCikisDto extends CreateAracGirisCikisDto {
  cikisSaati?: string
}

// ── Olaylar ───────────────────────────────────────────────────────────────────
export interface Olay {
  id: string
  baslik: string
  aciklama: string
  olayTarihi: string
  tip: OlayTipi
  konum?: string
  unitId?: string
  unitDoorNumber?: string
  durum: OlayDurum
  createdAt: string
}

export interface CreateOlayDto {
  baslik: string
  aciklama: string
  olayTarihi: string
  tip: OlayTipi
  konum?: string
  unitId?: string
}

export interface UpdateOlayDto extends CreateOlayDto {
  durum: OlayDurum
}

// ── Kayıp Eşya ────────────────────────────────────────────────────────────────
export interface KayipEsya {
  id: string
  esyaAdi: string
  aciklama?: string
  bulunanYer?: string
  bulunanTarih: string
  sahipAdi?: string
  sahipIletisim?: string
  durum: KayipEsyaDurum
  createdAt: string
}

export interface CreateKayipEsyaDto {
  esyaAdi: string
  aciklama?: string
  bulunanYer?: string
  bulunanTarih: string
  sahipAdi?: string
  sahipIletisim?: string
}

export interface UpdateKayipEsyaDto extends CreateKayipEsyaDto {
  durum: KayipEsyaDurum
}
