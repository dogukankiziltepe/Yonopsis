export type PaymentStatus = 0 | 1 | 2  // Pending=0, Paid=1, Overdue=2

export const PaymentStatusLabel: Record<PaymentStatus, string> = {
  0: 'Beklemede',
  1: 'Ödendi',
  2: 'Gecikmiş',
}

export const PaymentStatusColor: Record<PaymentStatus, string> = {
  0: 'bg-yellow-100 text-yellow-800',
  1: 'bg-green-100 text-green-800',
  2: 'bg-red-100 text-red-800',
}

export interface PaymentItemDto {
  name: string
  amount: number
}

export interface PaymentSummaryDto {
  id: string
  siteId: string
  unitId: string
  unitDoorNumber?: string
  amount: number
  items: PaymentItemDto[]
  dueDate: string
  paidDate?: string
  status: PaymentStatus
  description?: string
  createdAt: string
}

export interface BulkCreatePaymentsDto {
  buildingId?: string
  items: PaymentItemDto[]
  dueDate: string
  description?: string
}

export interface CreatePaymentDto {
  unitId: string
  items: PaymentItemDto[]
  dueDate: string
  description?: string
}

export interface UpdatePaymentDto {
  items: PaymentItemDto[]
  dueDate: string
  description?: string
}

export interface UpdatePaymentStatusDto {
  status: PaymentStatus
  paidDate?: string
}

export interface ImportPreviewRow {
  buildingName: string
  doorNumber: string
  items: PaymentItemDto[]
  total: number
}

export interface ImportSkippedRow {
  buildingName: string
  doorNumber: string
  reason: string
}
