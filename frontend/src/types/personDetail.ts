import { UserType } from './person'

// ── Enums ────────────────────────────────────────────────────────────────

export type EducationStatus = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
export const EducationStatusLabel: Record<EducationStatus, string> = {
  0: 'İlkokul',
  1: 'Ortaokul',
  2: 'Lise',
  3: 'Ön Lisans',
  4: 'Lisans',
  5: 'Yüksek Lisans',
  6: 'Doktora',
  7: 'Belirtilmemiş',
}

export type Nationality = 0 | 1
export const NationalityLabel: Record<Nationality, string> = {
  0: 'T.C.',
  1: 'Yabancı Uyruklu',
}

export type MaritalStatus = 0 | 1
export const MaritalStatusLabel: Record<MaritalStatus, string> = {
  0: 'Evli',
  1: 'Bekar',
}

export type PetType = 0 | 1 | 2 | 3 | 4
export const PetTypeLabel: Record<PetType, string> = {
  0: 'Yok',
  1: 'Kedi',
  2: 'Köpek',
  3: 'Kuş',
  4: 'Diğer',
}

export type Gender = 0 | 1 | 2
export const GenderLabel: Record<Gender, string> = {
  0: 'Erkek',
  1: 'Kadın',
  2: 'Belirtilmemiş',
}

// ── Genel Bilgiler ───────────────────────────────────────────────────────

export interface PersonPhoneDto {
  id: string
  phoneNumber: string
  label?: string
}

export interface PersonPhoneInputDto {
  phoneNumber: string
  label?: string
}

export interface PersonGeneralInfoDto {
  userId: string
  userSiteId: string
  firstName: string
  lastName: string
  email: string
  nationalId?: string
  taxOffice?: string
  secondaryEmail?: string
  address?: string
  phones: PersonPhoneDto[]
  isActive: boolean
}

export interface UpdatePersonGeneralInfoDto {
  taxOffice?: string
  secondaryEmail?: string
  address?: string
  phones: PersonPhoneInputDto[]
}

// ── Detay Bilgileri ──────────────────────────────────────────────────────

export interface PersonDetailInfoDto {
  description?: string
  cariHesapId?: string
  cariHesapKodu?: string
  cariHesapAdi?: string
  gender?: Gender
  educationStatus?: EducationStatus
  schoolOrInstitution?: string
  profession?: string
  hasPrivateInsurance: boolean
  isMartyrOrVeteranRelative: boolean
  petType?: PetType
  petDetail?: string
}

export interface UpdatePersonDetailInfoDto {
  description?: string
  gender?: Gender
  educationStatus?: EducationStatus
  schoolOrInstitution?: string
  profession?: string
  hasPrivateInsurance: boolean
  isMartyrOrVeteranRelative: boolean
  petType?: PetType
  petDetail?: string
}

// ── Kimlik Bilgileri ─────────────────────────────────────────────────────

export interface PersonIdentityInfoDto {
  nationality?: Nationality
  identitySeriNo?: string
  identitySiraNo?: string
  passportNo?: string
  fatherName?: string
  motherName?: string
  birthPlace?: string
  birthDate?: string
  maritalStatus?: MaritalStatus
  registeredCity?: string
  registeredDistrict?: string
  registeredNeighborhood?: string
  familySiraNo?: string
  kayitSiraNo?: string
}

export type UpdatePersonIdentityInfoDto = PersonIdentityInfoDto

// ── Daireler ─────────────────────────────────────────────────────────────

export interface PersonUnitHistoryDto {
  id: string
  unitId: string
  doorNumber: string
  buildingName?: string
  role: UserType
  entryDate: string
  exitDate?: string
  contactPerson?: string
  notes?: string
  bankPaymentCode?: string
}

// ── Araçlar ──────────────────────────────────────────────────────────────

export interface PersonVehicleDto {
  id: string
  plate: string
  brand?: string
  model?: string
  color?: string
  hgsNo?: string
  isActive: boolean
}

// ── İletişim Geçmişi ─────────────────────────────────────────────────────

export interface PersonEmailLogDto {
  id: string
  sentAt: string
  deliveredAt?: string
  readAt?: string
  recipientEmail: string
  subject: string
  body?: string
  status: string
}

export interface PersonSmsLogDto {
  id: string
  sentAt: string
  phoneNumber: string
  message: string
  status: string
}

export interface PersonWhatsappLogDto {
  id: string
  sentAt: string
  phoneNumber: string
  message: string
  status: string
}

export interface PersonMobilBildirimLogDto {
  id: string
  sentAt: string
  message: string
  status: string
}

export interface PersonContactHistoryDto {
  emailLogs: PersonEmailLogDto[]
  smsLogs: PersonSmsLogDto[]
  whatsappLogs: PersonWhatsappLogDto[]
  mobilBildirimLogs: PersonMobilBildirimLogDto[]
}

// ── Geçiş Kartları ───────────────────────────────────────────────────────

export interface PersonAccessCardDto {
  id: string
  cardNumber: string
  issueDate: string
  expiryDate?: string
  isActive: boolean
}

// ── Talepler ─────────────────────────────────────────────────────────────

export interface PersonSupportRequestDto {
  id: string
  createdAt: string
  unitId: string
  unitDoorNumber?: string
  subject: string
  status: 0 | 1 | 2
}

// ── Aggregate ────────────────────────────────────────────────────────────

export interface PersonFullDetailDto {
  general: PersonGeneralInfoDto
  detail: PersonDetailInfoDto
  identity: PersonIdentityInfoDto
  units: PersonUnitHistoryDto[]
  vehicles: PersonVehicleDto[]
  contactHistory: PersonContactHistoryDto
  accessCards: PersonAccessCardDto[]
  supportRequests: PersonSupportRequestDto[]
}
