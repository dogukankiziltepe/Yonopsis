export interface Personel {
  id: string
  siteId: string
  personelKodu: string
  name: string
  firma?: string | null
  title?: string | null
  tcKimlikNo?: string | null
  phone?: string | null
  email?: string | null
  dogumTarihi?: string | null
  aciklama?: string | null
  startDate?: string | null
  cikisTarihi?: string | null
  isActive: boolean
  createdAt: string
  updatedAt?: string | null
}

export interface CreatePersonelDto {
  personelKodu: string
  name: string
  firma?: string | null
  title: string
  email?: string | null
  startDate?: string | null
}
