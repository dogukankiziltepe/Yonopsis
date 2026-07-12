// ── Enums ────────────────────────────────────────────────────────────────────
export enum SayacTipi { Elektrik = 1, Su = 2, Dogalgaz = 3, Diger = 4 }

export const SayacTipiLabel: Record<SayacTipi, string> = {
  [SayacTipi.Elektrik]: 'Electricity',
  [SayacTipi.Su]: 'Water',
  [SayacTipi.Dogalgaz]: 'Natural Gas',
  [SayacTipi.Diger]: 'Other',
}

// ── AnaSayac ──────────────────────────────────────────────────────────────────
export interface AnaSayac {
  id: string
  ad: string
  tip: SayacTipi
  seriNo?: string
  marka?: string
  takimTarihi?: string
  aciklama?: string
  isActive: boolean
  createdAt: string
}

export interface CreateAnaSayacDto {
  ad: string
  tip: SayacTipi
  seriNo?: string
  marka?: string
  takimTarihi?: string
  aciklama?: string
}

export interface UpdateAnaSayacDto extends CreateAnaSayacDto {
  isActive: boolean
}

// ── DaireSayac ────────────────────────────────────────────────────────────────
export interface DaireSayac {
  id: string
  unitId: string
  unitDoorNumber?: string
  anaSayacId: string
  anaSayacAdi?: string
  tip: SayacTipi
  seriNo?: string
  marka?: string
  takimTarihi?: string
  aciklama?: string
  isActive: boolean
  createdAt: string
}

export interface CreateDaireSayacDto {
  unitId: string
  anaSayacId: string
  tip: SayacTipi
  seriNo?: string
  marka?: string
  takimTarihi?: string
  aciklama?: string
}

export interface UpdateDaireSayacDto extends CreateDaireSayacDto {
  isActive: boolean
}

// ── SayacOkuma ────────────────────────────────────────────────────────────────
export interface SayacOkuma {
  id: string
  anaSayacId?: string
  anaSayacAdi?: string
  daireSayacId?: string
  unitDoorNumber?: string
  okumaTarihi: string
  oncekiEndeks: number
  sonEndeks: number
  tuketim: number
  aciklama?: string
  createdAt: string
}

export interface CreateSayacOkumaDto {
  anaSayacId?: string
  daireSayacId?: string
  okumaTarihi: string
  oncekiEndeks: number
  sonEndeks: number
  aciklama?: string
}

export interface UpdateSayacOkumaDto {
  okumaTarihi: string
  oncekiEndeks: number
  sonEndeks: number
  aciklama?: string
}

// ── BirimFiyat ────────────────────────────────────────────────────────────────
export interface BirimFiyat {
  id: string
  tip: SayacTipi
  fiyat: number
  birim?: string
  baslangicTarihi: string
  bitisTarihi?: string
  aciklama?: string
  createdAt: string
}

export interface CreateBirimFiyatDto {
  tip: SayacTipi
  fiyat: number
  birim?: string
  baslangicTarihi: string
  bitisTarihi?: string
  aciklama?: string
}

export interface UpdateBirimFiyatDto extends CreateBirimFiyatDto {}
