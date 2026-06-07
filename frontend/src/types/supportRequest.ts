export type SupportRequestStatus = 0 | 1 | 2  // Open=0, InProgress=1, Resolved=2

export const SupportRequestStatusLabel: Record<SupportRequestStatus, string> = {
  0: 'Açık',
  1: 'İşlemde',
  2: 'Çözüldü',
}

export const SupportRequestStatusColor: Record<SupportRequestStatus, string> = {
  0: 'bg-yellow-100 text-yellow-800',
  1: 'bg-blue-100 text-blue-800',
  2: 'bg-green-100 text-green-800',
}

export interface SupportRequestSummaryDto {
  id: string
  siteId: string
  userId: string
  unitId: string
  unitDoorNumber?: string
  subject: string
  status: SupportRequestStatus
  createdAt: string
  resolvedAt?: string
}

export interface SupportRequestDetailDto extends SupportRequestSummaryDto {
  description?: string
  updatedAt?: string
}

export interface CreateSupportRequestDto {
  unitId: string
  subject: string
  description?: string
}

export interface UpdateSupportRequestDto {
  subject: string
  description?: string
}

export interface UpdateSupportRequestStatusDto {
  status: SupportRequestStatus
}
