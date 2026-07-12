// Shared type for simple group/definition entities
export interface SimpleDefinition {
  id: string
  name: string
  description?: string
  isActive: boolean
  order: number
  createdAt: string
}

// Gelir Grubu (Income Group)
export type GelirGrubu = SimpleDefinition
export interface CreateGelirGrubuDto { name: string; description?: string; order: number }
export interface UpdateGelirGrubuDto { name: string; description?: string; isActive: boolean; order: number }

// Gider Grubu (Expense Group)
export type GiderGrubu = SimpleDefinition
export interface CreateGiderGrubuDto { name: string; description?: string; order: number }
export interface UpdateGiderGrubuDto { name: string; description?: string; isActive: boolean; order: number }

// Gelir Tanimi (Income Definition)
export interface GelirTanimi extends SimpleDefinition {
  gelirGrubuId?: string
  gelirGrubuName?: string
}
export interface CreateGelirTanimiDto { name: string; description?: string; gelirGrubuId?: string; order: number }
export interface UpdateGelirTanimiDto { name: string; description?: string; gelirGrubuId?: string; isActive: boolean; order: number }

// Gider Tanimi (Expense Definition)
export type DagitimSekli = 0 | 1 | 2 | 3
export const DagitimSekliLabel: Record<DagitimSekli, string> = {
  0: 'Eşit Dağıtım', 1: 'Arsa Payına Göre', 2: 'Metrekareye Göre', 3: 'Manuel Dağıtım',
}

// CariTuru burada muhasebe.ts'deki enum ile birebir aynı (0-4); Borçlandırılacak Kişi seçiminde kullanılır.
export type BorclandirilacakKisiTuru = 0 | 1 | 2 | 3 | 4

export interface GiderTanimi extends SimpleDefinition {
  giderKodu: string
  giderGrubuId?: string
  giderGrubuName?: string
  dagitimSekli?: DagitimSekli
  bosDairelereDagit: boolean
  kdv?: number
  borclandirilacakKisi?: BorclandirilacakKisiTuru
  muhasebeKodu?: string
}
export interface CreateGiderTanimiDto {
  giderKodu: string
  name: string
  description?: string
  giderGrubuId?: string
  dagitimSekli?: DagitimSekli
  bosDairelereDagit: boolean
  kdv?: number
  borclandirilacakKisi?: BorclandirilacakKisiTuru
  muhasebeKodu?: string
  order: number
}
export interface UpdateGiderTanimiDto {
  giderKodu: string
  name: string
  description?: string
  giderGrubuId?: string
  dagitimSekli?: DagitimSekli
  bosDairelereDagit: boolean
  kdv?: number
  borclandirilacakKisi?: BorclandirilacakKisiTuru
  muhasebeKodu?: string
  isActive: boolean
  order: number
}

// KasaBanka (Cash/Bank)
export enum KasaBankaTipi { Kasa = 1, Banka = 2 }
export interface KasaBanka {
  id: string
  name: string
  tip: KasaBankaTipi
  bankaAdi?: string
  subeAdi?: string
  hesapNo?: string
  iban?: string
  isActive: boolean
  createdAt: string
}
export interface CreateKasaBankaDto { name: string; tip: KasaBankaTipi; bankaAdi?: string; subeAdi?: string; hesapNo?: string; iban?: string }
export interface UpdateKasaBankaDto { name: string; tip: KasaBankaTipi; bankaAdi?: string; subeAdi?: string; hesapNo?: string; iban?: string; isActive: boolean }

// Tesis (Facility)
export interface Tesis extends SimpleDefinition {
  kapasite?: number
}
export interface CreateTesisDto { name: string; description?: string; kapasite?: number; order: number }
export interface UpdateTesisDto { name: string; description?: string; kapasite?: number; isActive: boolean; order: number }
