// ── İcra Takibi ──────────────────────────────────────────────────────────────

export enum TakipDurumu { Takipte = 0, IcrayaVerilecek = 1, IcrayaVerildi = 2, Odendi = 3, IptalEdildi = 4 }
export const TakipDurumuLabel: Record<TakipDurumu, string> = {
  [TakipDurumu.Takipte]: 'Takipte',
  [TakipDurumu.IcrayaVerilecek]: 'İcraya Verilecek',
  [TakipDurumu.IcrayaVerildi]: 'İcraya Verildi',
  [TakipDurumu.Odendi]: 'Ödendi',
  [TakipDurumu.IptalEdildi]: 'İptal Edildi',
}

export enum IcraDurumu { Icrada = 0, HacizAsamasinda = 1, Kapandi = 2, IptalEdildi = 3 }
export const IcraDurumuLabel: Record<IcraDurumu, string> = {
  [IcraDurumu.Icrada]: 'İcrada',
  [IcraDurumu.HacizAsamasinda]: 'Haciz Aşamasında',
  [IcraDurumu.Kapandi]: 'Kapandı',
  [IcraDurumu.IptalEdildi]: 'İptal Edildi',
}

export interface Avukat {
  id: string
  adSoyad: string
  buroAdi?: string
  telefon?: string
  eposta?: string
  adres?: string
  isActive: boolean
}
export type SaveAvukatDto = Omit<Avukat, 'id'>
export interface AvukatSecim { id: string; adSoyad: string }

export interface TakipAdayFiltre {
  buildingId?: string
  unitId?: string
  gelirTanimiId?: string
  gunSayisi: number
  enAzBakiye: number
}
export interface TakipAdayi {
  borcluUserId: string
  borcluAdi: string
  unitId: string
  blokAdi?: string
  doorNumber: string
  evrakSayisi: number
  borc: number
  tazminat: number
  odenen: number
  kalan: number
}
export interface TakipBaslatDto {
  filtre: TakipAdayFiltre
  secimler: { borcluUserId: string; unitId: string }[]
}

export interface IcraEvrak {
  borcMakbuzuId: string
  evrakNo: string
  sonOdemeTarihi?: string
  doorNumber: string
  kategori?: string
  evrakTutari: number
  tazminat: number
  odenen: number
  kalan: number
}

export interface TakipListItem {
  id: string
  takipTarihi: string
  borcluUserId: string
  borcluAdi: string
  unitId: string
  blokAdi?: string
  doorNumber: string
  baslangicTutari: number
  mevcutTutar: number
  durum: TakipDurumu
  telefon?: string
  eposta?: string
  adres?: string
  icraDosyasiId?: string
}
export interface TakipDetay {
  id: string
  takipTarihi: string
  borcluUserId: string
  borcluAdi: string
  unitId: string
  blokAdi?: string
  doorNumber: string
  baslangicTutari: number
  mevcutTutar: number
  durum: TakipDurumu
  aciklama?: string
  icraDosyasiId?: string
  icraDosyaNo?: string
  evraklar: IcraEvrak[]
}
export interface UpdateTakipDto { takipTarihi: string; durum: TakipDurumu; aciklama?: string }
export interface IcrayaVerDto { dosyaNo: string; icraTarihi: string; avukatId?: string; aciklama?: string }

export interface IcraDosyasiListItem {
  id: string
  icraTarihi: string
  dosyaNo: string
  borcluUserId: string
  borcluAdi: string
  blokAdi?: string
  doorNumber: string
  avukatAdi?: string
  durum: IcraDurumu
  dosyaTutari: number
  bakiye: number
}
export interface IcraDosyasiDetay {
  id: string
  dosyaNo: string
  icraTarihi: string
  durum: IcraDurumu
  borcluUserId: string
  borcluAdi: string
  unitId: string
  blokAdi?: string
  doorNumber: string
  avukatId?: string
  avukatAdi?: string
  aciklama?: string
  dosyaTutari: number
  bakiye: number
  takipId?: string
  evraklar: IcraEvrak[]
}
export interface UpdateIcraDosyasiDto { dosyaNo: string; icraTarihi: string; durum: IcraDurumu; avukatId?: string; aciklama?: string }

export interface IcraRaporFiltre {
  ilkTarih?: string
  sonTarih?: string
  buildingId?: string
  unitId?: string
  borcluUserId?: string
  avukatId?: string
  dosyaNo?: string
}
export interface IcraRaporSatiri {
  id: string
  icraTarihi: string
  dosyaNo: string
  borcluAdi: string
  blokAdi?: string
  doorNumber: string
  avukatAdi?: string
  durum: IcraDurumu
  evrakSayisi: number
  dosyaTutari: number
  tahsilat: number
  bakiye: number
}
