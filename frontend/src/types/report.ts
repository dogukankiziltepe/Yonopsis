// ── Enums ───────────────────────────────────────────────────────────────────
export enum KasaBankaTipi { Kasa = 1, BankHesabi = 2 }

// ── Kasalar ─────────────────────────────────────────────────────────────────
export interface KasaBakiye {
  kasaBankaId: string
  ad: string
  tip: KasaBankaTipi
  devir: number
  giren: number
  cikan: number
  kalan: number
}

// ── İş Takibi ───────────────────────────────────────────────────────────────
export interface IsTakibiOgesi {
  id: string
  kaynak: 'IsEmri' | 'YapilacakIs'
  baslik: string
  atananKisi?: string
  oncelik: string
  durum: string
  tarih?: string
}

// ── Aidat Tahsilat Durumu ─────────────────────────────────────────────────────
export interface AidatTahsilatAy {
  donem: string
  tahsilEdilen: number
  tahsilEdilemeyen: number
}

// ── Finansal Durum ────────────────────────────────────────────────────────────
export interface FinansalDurumNoktasi {
  tarih: string
  kasaBankaId: string
  kasaBankaAdi: string
  bakiye: number
}

// ── Evraklar ────────────────────────────────────────────────────────────────
export interface Evrak {
  id: string
  tarih: string
  evrakNo: string
  cariAdi: string
  tutar: number
}

// ── Ödenecek Faturalar ────────────────────────────────────────────────────────
export interface OdenecekFatura {
  id: string
  evrakNo: string
  cariAdi: string
  tutar: number
  sonOdemeTarihi?: string
}

// ── Gelir/Gider Dağılımı ──────────────────────────────────────────────────────
export interface DagilimDilimi {
  ad: string
  tutar: number
  yuzde: number
}

// ── Duyurular ───────────────────────────────────────────────────────────────
export interface DuyuruOzet {
  id: string
  title: string
  isPinned: boolean
  publishDate?: string
  createdAt: string
}

// ── Banka Hesapları ───────────────────────────────────────────────────────────
export interface BankaHesabi {
  id: string
  ad: string
  bankaAdi?: string
  subeAdi?: string
  hesapNo?: string
  iban?: string
}

// ── Özet ────────────────────────────────────────────────────────────────────
export interface ReportSummary {
  kasalar: KasaBakiye[]
  isTakibi: IsTakibiOgesi[]
  aidatTahsilat: AidatTahsilatAy[]
  finansalDurum: FinansalDurumNoktasi[]
  giderEvraklari: Evrak[]
  gelirEvraklari: Evrak[]
  odenecekFaturalar: OdenecekFatura[]
  giderDagilimi: DagilimDilimi[]
  gelirDagilimi: DagilimDilimi[]
  duyurular: DuyuruOzet[]
  bankaHesaplari: BankaHesabi[]
}

export interface ReportDateFilter {
  all?: boolean
  from?: string
  to?: string
}
