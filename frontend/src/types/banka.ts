export interface BankaSubesi {
  id: string
  bankaId: string
  subeAdi: string
  subeKodu?: string | null
  isActive: boolean
}

export interface Banka {
  id: string
  name: string
  isActive: boolean
  subeler: BankaSubesi[]
}

export interface BankaSubesiPickerItem {
  id: string
  bankaAdi: string
  subeAdi: string
  subeKodu?: string | null
}

export interface CreateBankaDto { name: string }
export interface UpdateBankaDto { name: string; isActive: boolean }
export interface CreateBankaSubesiDto { subeAdi: string; subeKodu?: string }
export interface UpdateBankaSubesiDto { subeAdi: string; subeKodu?: string; isActive: boolean }
