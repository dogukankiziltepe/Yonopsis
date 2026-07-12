export enum RezervasyonDurum {
  Beklemede  = 0,
  Onaylandi  = 1,
  Reddedildi = 2,
  Iptal      = 3,
}

export const rezervasyonDurumLabel: Record<RezervasyonDurum, string> = {
  [RezervasyonDurum.Beklemede]:  'Beklemede',
  [RezervasyonDurum.Onaylandi]:  'Onaylandı',
  [RezervasyonDurum.Reddedildi]: 'Reddedildi',
  [RezervasyonDurum.Iptal]:      'İptal',
}

export const rezervasyonDurumVariant: Record<RezervasyonDurum, 'default' | 'secondary' | 'destructive' | 'outline'> = {
  [RezervasyonDurum.Beklemede]:  'secondary',
  [RezervasyonDurum.Onaylandi]:  'default',
  [RezervasyonDurum.Reddedildi]: 'destructive',
  [RezervasyonDurum.Iptal]:      'outline',
}

export interface Rezervasyon {
  id: string
  siteId: string
  tesisId?: string | null
  tesisAdi?: string | null
  personId?: string | null
  startDate: string
  endDate: string
  durum: RezervasyonDurum
  notes?: string | null
  createdAt: string
  updatedAt?: string | null
}

export interface CreateRezervasyonDto {
  tesisId?: string | null
  personId?: string | null
  startDate: string
  endDate: string
  durum: RezervasyonDurum
  notes?: string | null
}

export type UpdateRezervasyonDto = CreateRezervasyonDto
