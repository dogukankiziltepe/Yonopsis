import type { UserType } from './person'

export interface UnitBorcluOnerisi {
  ownerUserId?: string
  ownerAdSoyad?: string
  tenantUserId?: string
  tenantAdSoyad?: string
  onerilenPersonId?: string
  onerilenAdSoyad?: string
  onerilenRol?: UserType
}
