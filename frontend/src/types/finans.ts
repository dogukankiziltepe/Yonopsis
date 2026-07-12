// ── Enums ───────────────────────────────────────────────────────────────────
export enum OdemeTipi { Nakit = 1, KrediKarti = 2, HavaleEft = 3, Diger = 4 }
export enum BankaHareketiDurum { Bekleyen = 0, Eslestis = 1 }
export enum FaturaTipi { Gelir = 0, Gider = 1 }
export enum FaturaOdemeDurumu { Odenmedi = 0, Odendi = 1 }

export const FaturaOdemeDurumuLabel: Record<FaturaOdemeDurumu, string> = {
  [FaturaOdemeDurumu.Odenmedi]: 'Ödenmedi',
  [FaturaOdemeDurumu.Odendi]: 'Ödendi',
}

export const OdemeTipiLabel: Record<OdemeTipi, string> = {
  [OdemeTipi.Nakit]: 'Cash',
  [OdemeTipi.KrediKarti]: 'Credit Card',
  [OdemeTipi.HavaleEft]: 'Bank Transfer / EFT',
  [OdemeTipi.Diger]: 'Other',
}

export const BankaHareketiDurumLabel: Record<BankaHareketiDurum, string> = {
  [BankaHareketiDurum.Bekleyen]: 'Pending',
  [BankaHareketiDurum.Eslestis]: 'Matched',
}

// ── Borç Makbuzu ─────────────────────────────────────────────────────────────
export interface BorcMakbuzu {
  id: string
  evrakNo: string
  islemTarihi: string
  donem?: string
  sonOdemeTarihi?: string
  unitId?: string
  unitDoorNumber?: string
  borcluAdi?: string
  gelirTanimiAdi?: string
  tutar: number
  gecikmeTutari: number
  odenenTutar: number
  kalanTutar: number
  aciklama?: string
  createdAt: string
}
export interface CreateBorcMakbuzuDto {
  donem?: string
  sonOdemeTarihi?: string
  unitId?: string
  borcluAdi?: string
  gelirTanimiId?: string
  tutar: number
  aciklama?: string
}
export interface UpdateBorcMakbuzuDto extends CreateBorcMakbuzuDto {
  gecikmeTutari: number
  odenenTutar: number
}

// ── Tahsilat Makbuzu ─────────────────────────────────────────────────────────
export interface TahsilatMakbuzu {
  id: string
  evrakNo: string
  islemTarihi: string
  borcluAdi?: string
  kasaBankaId?: string
  kasaBankaAdi?: string
  borcMakbuzuId?: string
  borcMakbuzuEvrakNo?: string
  odemeTutari: number
  odemeTipi: OdemeTipi
  aciklama?: string
  createdAt: string
}
export interface CreateTahsilatMakbuzuDto {
  borcluAdi?: string
  kasaBankaId?: string
  borcMakbuzuId?: string
  odemeTutari: number
  odemeTipi: OdemeTipi
  aciklama?: string
}
export type UpdateTahsilatMakbuzuDto = CreateTahsilatMakbuzuDto

// ── Fatura ───────────────────────────────────────────────────────────────────
export interface Fatura {
  id: string
  tip: FaturaTipi
  evrakNo: string
  islemTarihi: string
  faturaTarihi: string
  cariAdi: string
  gelirTanimiId?: string
  gelirTanimiAdi?: string
  giderTanimiId?: string
  giderTanimiAdi?: string
  toplamTutar: number
  aciklama?: string
  sonOdemeTarihi?: string
  odemeDurumu: FaturaOdemeDurumu
  createdAt: string
}
export interface CreateFaturaDto {
  tip: FaturaTipi
  faturaTarihi: string
  cariAdi: string
  gelirTanimiId?: string
  giderTanimiId?: string
  toplamTutar: number
  aciklama?: string
  sonOdemeTarihi?: string
}
export interface UpdateFaturaDto {
  faturaTarihi: string
  cariAdi: string
  gelirTanimiId?: string
  giderTanimiId?: string
  toplamTutar: number
  aciklama?: string
  sonOdemeTarihi?: string
  odemeDurumu?: FaturaOdemeDurumu
}

// ── Banka Hareketi ───────────────────────────────────────────────────────────
export interface BankaHareketi {
  id: string
  kasaBankaId: string
  kasaBankaAdi: string
  tarih: string
  aciklama: string
  referansNo?: string
  tutar: number
  durum: BankaHareketiDurum
  eslestirmeId?: string
  createdAt: string
}
export interface CreateBankaHareketiDto {
  kasaBankaId: string
  tarih: string
  aciklama: string
  referansNo?: string
  tutar: number
}
export interface UpdateBankaHareketiDto extends CreateBankaHareketiDto {
  durum: BankaHareketiDurum
  eslestirmeId?: string
}
