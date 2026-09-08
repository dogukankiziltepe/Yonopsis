import type { UserType } from './person'

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
  borcluUserId?: string
  borcluAdSoyad?: string
  borcluRol?: UserType
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
  borcluUserId?: string
  borcluRol?: UserType
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
  borcluUserId?: string
  borcluAdSoyad?: string
  kasaBankaId?: string
  kasaBankaAdi?: string
  borcMakbuzuId?: string
  borcMakbuzuEvrakNo?: string
  borcMakbuzuKalanTutar?: number
  odemeTutari: number
  odemeTipi: OdemeTipi
  aciklama?: string
  createdAt: string
}
export interface CreateTahsilatMakbuzuDto {
  borcluUserId?: string
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

// ── Toplu Borçlandırma ───────────────────────────────────────────────────────
export interface GelirTanimiSecim {
  id: string
  name: string
}

export interface TopluBorclandirmaTemplateRequest {
  gelirTanimiIds: string[]
  borcluRolTercihi: UserType // 2=Sahip, 3=Kiracı
  donem?: string
  sonOdemeTarihi?: string
}

export interface TopluBorclandirmaPreviewItem {
  unitId: string
  unitDoorNumber: string
  buildingName?: string
  gelirTanimiId: string
  gelirTanimiAdi: string
  tutar: number
  aciklama?: string
  borcluUserId?: string
  borcluAdSoyad?: string
  borcluRol?: UserType
  mukerrer: boolean
  uyarilar: string[]
}

export interface TopluBorclandirmaPreview {
  toplamKombinasyon: number
  toplamTutar: number
  mukerrerSayisi: number
  donem?: string
  sonOdemeTarihi?: string
  items: TopluBorclandirmaPreviewItem[]
  satirHatalari: string[]
}

export interface TopluBorclandirmaConfirmItem {
  unitId: string
  gelirTanimiId: string
  tutar: number
  aciklama?: string
  borcluUserId?: string
  borcluRol?: UserType
}

export interface TopluBorclandirmaConfirmRequest {
  donem?: string
  sonOdemeTarihi?: string
  items: TopluBorclandirmaConfirmItem[]
}

export interface TopluBorclandirmaSonuc {
  batchId: string
  olusturulanSayisi: number
}

// ── Cari Hesap Picker ────────────────────────────────────────────────────────
export enum CariTuru { Kiraci = 0, EvSahibi = 1, Tedarikci = 2, Personel = 3, Diger = 4 }
export const CariTuruLabel: Record<CariTuru, string> = {
  [CariTuru.Kiraci]: 'Kiracı',
  [CariTuru.EvSahibi]: 'Ev Sahibi',
  [CariTuru.Tedarikci]: 'Tedarikçi',
  [CariTuru.Personel]: 'Personel',
  [CariTuru.Diger]: 'Diğer',
}
export interface CariHesapPicker {
  id: string
  hesapKodu: string
  hesapAdi: string
  cariTuru?: CariTuru
}

// ── Ödeme Makbuzu ────────────────────────────────────────────────────────────
export interface OdemeMakbuzu {
  id: string
  evrakNo: string
  islemTarihi: string
  tarih: string
  cariHesapId: string
  cariHesapAdi?: string
  kasaBankaId: string
  kasaBankaAdi?: string
  giderTanimiId: string
  giderTanimiAdi?: string
  tutar: number
  aciklama?: string
  dagitimYapilacak: boolean
  createdAt: string
}
export interface CreateOdemeMakbuzuDto {
  tarih: string
  cariHesapId: string
  kasaBankaId: string
  giderTanimiId: string
  tutar: number
  aciklama?: string
  dagitimYapilacak: boolean
}
export type UpdateOdemeMakbuzuDto = CreateOdemeMakbuzuDto

// ── Gelir Tahsilat Makbuzu (Ödeme Makbuzu'nun ikizi — bağımsız gelir kaydı) ──
export interface GelirTahsilatMakbuzu {
  id: string
  evrakNo: string
  islemTarihi: string
  tarih: string
  cariHesapId: string
  cariHesapAdi?: string
  kasaBankaId: string
  kasaBankaAdi?: string
  gelirTanimiId: string
  gelirTanimiAdi?: string
  tutar: number
  aciklama?: string
  dagitimYapilacak: boolean
  createdAt: string
}
export interface CreateGelirTahsilatMakbuzuDto {
  tarih: string
  cariHesapId: string
  kasaBankaId: string
  gelirTanimiId: string
  tutar: number
  aciklama?: string
  dagitimYapilacak: boolean
}
export type UpdateGelirTahsilatMakbuzuDto = CreateGelirTahsilatMakbuzuDto

// ── Devir Bakiye ─────────────────────────────────────────────────────────────
export interface DevirBakiye {
  id: string
  evrakNo: string
  tarih: string
  unitId?: string
  unitDoorNumber?: string
  borcluUserId?: string
  borcluAdSoyad?: string
  borcluRol?: UserType
  tutar: number
  aciklama?: string
  createdAt: string
}
export interface CreateDevirBakiyeDto {
  tarih: string
  unitId?: string
  borcluUserId?: string
  borcluRol?: UserType
  tutar: number
  aciklama?: string
}
export type UpdateDevirBakiyeDto = CreateDevirBakiyeDto

// ── İade Makbuzu ─────────────────────────────────────────────────────────────
export interface IadeMakbuzu {
  id: string
  evrakNo: string
  tarih: string
  borcluUserId?: string
  borcluAdSoyad?: string
  borcluRol?: UserType
  kasaBankaId?: string
  kasaBankaAdi?: string
  tutar: number
  aciklama?: string
  createdAt: string
}
export interface CreateIadeMakbuzuDto {
  tarih: string
  borcluUserId?: string
  borcluRol?: UserType
  kasaBankaId?: string
  tutar: number
  aciklama?: string
}
export type UpdateIadeMakbuzuDto = CreateIadeMakbuzuDto

// ── Kişilere Göre Finansal Durum ─────────────────────────────────────────────
export interface KisiFinansalDurumSatiri {
  personUserId: string
  adSoyad: string
  borcTutari: number
  gecikme: number
  iadeEdilen: number
  odenen: number
  borc: number
  alacak: number
  bakiye: number
  ba: string // "B" | "A"
}

export interface KisiOzet {
  personUserId: string
  adSoyad: string
  email?: string
  telefon?: string
}

export interface FinansalHareket {
  id: string
  kaynak: string // "Borc" | "Tahsilat" | "Devir"
  evrakTarihi: string
  sonOdemeTarihi?: string
  aciklama?: string
  borc: number
  tazminat: number
  alacak: number
  yurudakiBakiye: number
}

export interface GelirGrubuFinansal {
  gelirGrubuId?: string
  grupAdi: string
  borc: number
  tazminat: number
  alacak: number
  bakiye: number
  hareketler: FinansalHareket[]
}

export interface DaireFinansal {
  unitId?: string
  doorNumber: string
  borc: number
  tazminat: number
  alacak: number
  bakiye: number
  kategoriler: GelirGrubuFinansal[]
}

export interface KisiFinansalDetay {
  kisi: KisiOzet
  toplamBorc: number
  toplamAlacak: number
  toplamBakiye: number
  daireler: DaireFinansal[]
}

// ── Banka Hareketi Excel İçe Aktarma ─────────────────────────────────────────
export interface BankaHareketiImportRow {
  rowIndex: number
  tarih?: string
  aciklama?: string
  referansNo?: string
  tutar?: number
  isValid: boolean
  hatalar: string[]
}
export interface BankaHareketiImportPreview {
  toplamSatir: number
  gecerliSatir: number
  satirlar: BankaHareketiImportRow[]
}
export interface BankaHareketiImportConfirmItem {
  tarih: string
  aciklama: string
  referansNo?: string
  tutar: number
}
export interface BankaHareketiImportConfirmRequest {
  items: BankaHareketiImportConfirmItem[]
}
export interface BankaHareketiImportSonuc {
  olusturulanSayisi: number
}
