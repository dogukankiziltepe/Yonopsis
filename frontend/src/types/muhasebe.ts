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
