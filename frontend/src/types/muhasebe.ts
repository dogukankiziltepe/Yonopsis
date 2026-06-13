// Muhasebe modülü tip tanımları. Enum değerleri backend ile birebir (numeric).

export enum HesapTipi {
  Tekduzen = 0,
  Cari = 1,
}

export enum HesapKategorisi {
  Aktif = 0,
  Pasif = 1,
  Gelir = 2,
  Gider = 3,
  Maliyet = 4,
  Nazim = 5,
}

export enum NormalBakiye {
  Borc = 0,
  Alacak = 1,
}

export enum CariTuru {
  Kiraci = 0,
  EvSahibi = 1,
  Tedarikci = 2,
  Personel = 3,
  Diger = 4,
}

export const hesapKategorisiLabel: Record<HesapKategorisi, string> = {
  [HesapKategorisi.Aktif]: 'Aktif',
  [HesapKategorisi.Pasif]: 'Pasif',
  [HesapKategorisi.Gelir]: 'Gelir',
  [HesapKategorisi.Gider]: 'Gider',
  [HesapKategorisi.Maliyet]: 'Maliyet',
  [HesapKategorisi.Nazim]: 'Nazım',
}

export const normalBakiyeLabel: Record<NormalBakiye, string> = {
  [NormalBakiye.Borc]: 'Borç',
  [NormalBakiye.Alacak]: 'Alacak',
}

export const cariTuruLabel: Record<CariTuru, string> = {
  [CariTuru.Kiraci]: 'Kiracı',
  [CariTuru.EvSahibi]: 'Ev Sahibi',
  [CariTuru.Tedarikci]: 'Tedarikçi',
  [CariTuru.Personel]: 'Personel',
  [CariTuru.Diger]: 'Diğer',
}

export interface HesapNode {
  id: string
  hesapKodu: string
  hesapAdi: string
  hesapTipi: HesapTipi
  hesapKategorisi: HesapKategorisi
  normalBakiye: NormalBakiye
  seviye: number
  fisKesilebilirMi: boolean
  aktifMi: boolean
  cariTuru?: CariTuru | null
  children: HesapNode[]
}

export interface HesapListItem {
  id: string
  hesapKodu: string
  hesapAdi: string
  hesapTipi: HesapTipi
  hesapKategorisi: HesapKategorisi
  normalBakiye: NormalBakiye
  seviye: number
  parentId?: string | null
  fisKesilebilirMi: boolean
  aktifMi: boolean
  cariTuru?: CariTuru | null
  personId?: string | null
}

export interface HesapDetail {
  id: string
  hesapKodu: string
  hesapAdi: string
  hesapTipi: HesapTipi
  hesapKategorisi: HesapKategorisi
  normalBakiye: NormalBakiye
  seviye: number
  parentId?: string | null
  parentHesapKodu?: string | null
  fisKesilebilirMi: boolean
  aktifMi: boolean
  cariTuru?: CariTuru | null
  personId?: string | null
  giderTuruId?: string | null
  aciklama?: string | null
}

export interface CreateHesapDto {
  hesapKodu: string
  hesapAdi: string
  hesapKategorisi: HesapKategorisi
  normalBakiye: NormalBakiye
  parentId?: string | null
  fisKesilebilirMi: boolean
  aciklama?: string
}

export interface UpdateHesapDto {
  hesapAdi: string
  hesapKategorisi: HesapKategorisi
  normalBakiye: NormalBakiye
  fisKesilebilirMi: boolean
  aciklama?: string
}

export interface CreateCariHesapDto {
  cariTuru: CariTuru
  hesapAdi: string
  personId?: string
  aciklama?: string
}

// ---- Fiş / Dönem ----

export enum FisTuru {
  Acilis = 0,
  Mahsup = 1,
  Tahsil = 2,
  Tediye = 3,
  Kapanis = 4,
}

export enum FisDurumu {
  Taslak = 0,
  Onaylandi = 1,
  Iptal = 2,
}

export const fisTuruLabel: Record<FisTuru, string> = {
  [FisTuru.Acilis]: 'Açılış',
  [FisTuru.Mahsup]: 'Mahsup',
  [FisTuru.Tahsil]: 'Tahsil',
  [FisTuru.Tediye]: 'Tediye',
  [FisTuru.Kapanis]: 'Kapanış',
}

export const fisDurumuLabel: Record<FisDurumu, string> = {
  [FisDurumu.Taslak]: 'Taslak',
  [FisDurumu.Onaylandi]: 'Entegre',
  [FisDurumu.Iptal]: 'İptal',
}

export interface FisDetaySatir {
  hesapId: string
  borcTutar: number
  alacakTutar: number
  aciklama?: string
  belgeNo?: string
}

export interface CreateFisDto {
  fisTuru: FisTuru
  fisTarihi: string // YYYY-MM-DD
  aciklama: string
  detaylar: FisDetaySatir[]
}

export type UpdateFisDto = CreateFisDto

export interface FisListItem {
  id: string
  fisNo: string
  yevmiyeNo?: number | null
  fisTarihi: string
  fisTuru: FisTuru
  aciklama: string
  durum: FisDurumu
  toplamBorc: number
  toplamAlacak: number
  satirSayisi: number
}

export interface FisDetaySatirView {
  id: string
  siraNo: number
  hesapId: string
  hesapKodu: string
  hesapAdi: string
  borcTutar: number
  alacakTutar: number
  aciklama?: string | null
  belgeNo?: string | null
}

export interface FisDetay {
  id: string
  donemId: string
  fisNo: string
  yevmiyeNo?: number | null
  fisTarihi: string
  fisTuru: FisTuru
  aciklama: string
  durum: FisDurumu
  toplamBorc: number
  toplamAlacak: number
  satirSayisi: number
  detaylar: FisDetaySatirView[]
}

export interface FisDetaylarItem {
  detayId: string
  fisId: string
  fisNo: string
  yevmiyeNo?: number | null
  fisTarihi: string
  durum: FisDurumu
  siraNo: number
  hesapId: string
  hesapKodu: string
  hesapAdi: string
  borcTutar: number
  alacakTutar: number
  aciklama?: string | null
  belgeNo?: string | null
}

export interface Donem {
  id: string
  yil: number
  ad: string
  baslangicTarihi: string
  bitisTarihi: string
  durum: number
  sonYevmiyeNo: number
}
