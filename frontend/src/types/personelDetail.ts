import type { Gender, EducationStatus, MaritalStatus } from './personDetail'

export type KanGrubu = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
export const KanGrubuLabel: Record<KanGrubu, string> = {
  0: 'A Rh+', 1: 'A Rh-', 2: 'B Rh+', 3: 'B Rh-',
  4: 'AB Rh+', 5: 'AB Rh-', 6: '0 Rh+', 7: '0 Rh-',
}

export type PersonelIzinTuru = 0 | 1 | 2 | 3 | 4
export const PersonelIzinTuruLabel: Record<PersonelIzinTuru, string> = {
  0: 'Yıllık İzin', 1: 'Mazeret İzni', 2: 'Ücretsiz İzin', 3: 'Rapor', 4: 'Diğer',
}

export interface UpdatePersonelDto {
  personelKodu: string
  name: string
  firma?: string | null
  title: string
  cinsiyet?: Gender | null
  yemekKarti?: string | null
  aciklama?: string | null
  email?: string | null
  kanGrubu?: KanGrubu | null
  ogrenimDurumu?: EducationStatus | null
  okulKurum?: string | null
  adres?: string | null
  startDate?: string | null
  cikisTarihi?: string | null
  kidemTazminatiBaslamaTarihi?: string | null
  isActive: boolean
  muhasebeHesapKoduId?: string | null
  bankaSubesiId?: string | null
  bankaHesapNo?: string | null
  bankaIBAN?: string | null
  yillikIzinHakkiGun?: number | null
}

export interface PersonelCoreDto {
  id: string
  personelKodu: string
  name: string
  firma?: string | null
  title: string
  cinsiyet?: Gender | null
  yemekKarti?: string | null
  aciklama?: string | null
  email?: string | null
  kanGrubu?: KanGrubu | null
  ogrenimDurumu?: EducationStatus | null
  okulKurum?: string | null
  adres?: string | null
  startDate?: string | null
  cikisTarihi?: string | null
  kidemTazminatiBaslamaTarihi?: string | null
  isActive: boolean
  muhasebeHesapKoduId?: string | null
  muhasebeHesapKoduAdi?: string | null
}

export interface PersonelTelefonDto { id: string; phoneNumber: string; label?: string | null }
export interface PersonelAcilDurumKisiDto { id: string; adSoyad: string; yakinlik?: string | null; telefon?: string | null }

export interface PersonelEgitimDto {
  id: string
  egitiminKonusu: string
  egitmen?: string | null
  egitimYeri?: string | null
  baslamaTarihi?: string | null
  bitisTarihi?: string | null
  toplamSaat?: number | null
}

export interface PersonelIzinDto {
  id: string
  baslangicTarihi: string
  bitisTarihi: string
  izinTuru: PersonelIzinTuru
  aciklama?: string | null
  sureGun: number
}

export interface PersonelIzinOzetiDto {
  yillikIzinHakkiGun?: number | null
  kullanilanGun: number
  bakiyeGun?: number | null
}

export interface PersonelKimlikDto {
  tcKimlikNo?: string | null
  seri?: string | null
  sira?: string | null
  babaAdi?: string | null
  anaAdi?: string | null
  oncekiSoyad?: string | null
  dogumYeri?: string | null
  dogumTarihi?: string | null
  medeniHali?: MaritalStatus | null
  il?: string | null
  ilce?: string | null
  mahalleKoy?: string | null
  ciltNo?: string | null
  aileSiraNo?: string | null
  siraNo?: string | null
  verildigiYer?: string | null
  verilisNedeni?: string | null
  kayitNo?: string | null
  verilisTarihi?: string | null
}

export type UpdatePersonelKimlikDto = PersonelKimlikDto

export interface PersonelMuhasebeEntegrasyonDto {
  brutUcretlerGiderTanimiId?: string | null
  brutUcretlerGiderTanimiAdi?: string | null
  huzurHakkiBrutUcretlerGiderTanimiId?: string | null
  huzurHakkiBrutUcretlerGiderTanimiAdi?: string | null
  sgkIsverenPayiGiderTanimiId?: string | null
  sgkIsverenPayiGiderTanimiAdi?: string | null
  issizlikSigortasiIsverenPayiGiderTanimiId?: string | null
  issizlikSigortasiIsverenPayiGiderTanimiAdi?: string | null
  primVeIkramiyelerGiderTanimiId?: string | null
  primVeIkramiyelerGiderTanimiAdi?: string | null
  fazlaMesaiGiderTanimiId?: string | null
  fazlaMesaiGiderTanimiAdi?: string | null
  kidemTazminatlariGiderTanimiId?: string | null
  kidemTazminatlariGiderTanimiAdi?: string | null
  ihbarTazminatlariGiderTanimiId?: string | null
  ihbarTazminatlariGiderTanimiAdi?: string | null
  yolYardimiGiderTanimiId?: string | null
  yolYardimiGiderTanimiAdi?: string | null
  yemekYardimiGiderTanimiId?: string | null
  yemekYardimiGiderTanimiAdi?: string | null
  personelGelirVergisiHesapId?: string | null
  personelGelirVergisiHesapAdi?: string | null
  personelDamgaVergisiHesapId?: string | null
  personelDamgaVergisiHesapAdi?: string | null
  odenecekSgkHesapId?: string | null
  odenecekSgkHesapAdi?: string | null
  asgariGecimIndirimiHesapId?: string | null
  asgariGecimIndirimiHesapAdi?: string | null
  icraKesintisiHesapId?: string | null
  icraKesintisiHesapAdi?: string | null
  digerKesintilerHesapId?: string | null
  digerKesintilerHesapAdi?: string | null
  besHesapId?: string | null
  besHesapAdi?: string | null
}

export interface UpdatePersonelMuhasebeEntegrasyonDto {
  brutUcretlerGiderTanimiId?: string | null
  huzurHakkiBrutUcretlerGiderTanimiId?: string | null
  sgkIsverenPayiGiderTanimiId?: string | null
  issizlikSigortasiIsverenPayiGiderTanimiId?: string | null
  primVeIkramiyelerGiderTanimiId?: string | null
  fazlaMesaiGiderTanimiId?: string | null
  kidemTazminatlariGiderTanimiId?: string | null
  ihbarTazminatlariGiderTanimiId?: string | null
  yolYardimiGiderTanimiId?: string | null
  yemekYardimiGiderTanimiId?: string | null
  personelGelirVergisiHesapId?: string | null
  personelDamgaVergisiHesapId?: string | null
  odenecekSgkHesapId?: string | null
  asgariGecimIndirimiHesapId?: string | null
  icraKesintisiHesapId?: string | null
  digerKesintilerHesapId?: string | null
  besHesapId?: string | null
}

export interface PersonelBankaBilgisiDto {
  bankaSubesiId?: string | null
  bankaAdi?: string | null
  subeAdi?: string | null
  bankaHesapNo?: string | null
  bankaIBAN?: string | null
}

export interface PersonelFullDetailDto {
  core: PersonelCoreDto
  banka: PersonelBankaBilgisiDto
  kimlik: PersonelKimlikDto
  muhasebeEntegrasyon: PersonelMuhasebeEntegrasyonDto
  telefonlar: PersonelTelefonDto[]
  acilDurumKisileri: PersonelAcilDurumKisiDto[]
  egitimler: PersonelEgitimDto[]
  izinler: PersonelIzinDto[]
  izinOzeti: PersonelIzinOzetiDto
}

/**
 * PUT /api/personel/{id} tüm alanları birden günceller (kısmi patch değil).
 * Personel Tanımları, Banka Bilgileri ve İzin Yönetimi tabları aynı endpoint'i
 * kullandığından, her biri diğerlerinin alanlarını değiştirmeden geri göndermek
 * için bu birleştiriciyi kullanır — aksi halde bir tab'ı kaydetmek diğerinin
 * verisini sıfırlar.
 */
export function toUpdatePersonelDto(
  core: PersonelCoreDto,
  banka: PersonelBankaBilgisiDto,
  izinOzeti: PersonelIzinOzetiDto,
  overrides: Partial<UpdatePersonelDto> = {}
): UpdatePersonelDto {
  return {
    personelKodu: core.personelKodu,
    name: core.name,
    firma: core.firma,
    title: core.title,
    cinsiyet: core.cinsiyet,
    yemekKarti: core.yemekKarti,
    aciklama: core.aciklama,
    email: core.email,
    kanGrubu: core.kanGrubu,
    ogrenimDurumu: core.ogrenimDurumu,
    okulKurum: core.okulKurum,
    adres: core.adres,
    startDate: core.startDate,
    cikisTarihi: core.cikisTarihi,
    kidemTazminatiBaslamaTarihi: core.kidemTazminatiBaslamaTarihi,
    isActive: core.isActive,
    muhasebeHesapKoduId: core.muhasebeHesapKoduId,
    bankaSubesiId: banka.bankaSubesiId,
    bankaHesapNo: banka.bankaHesapNo,
    bankaIBAN: banka.bankaIBAN,
    yillikIzinHakkiGun: izinOzeti.yillikIzinHakkiGun,
    ...overrides,
  }
}
