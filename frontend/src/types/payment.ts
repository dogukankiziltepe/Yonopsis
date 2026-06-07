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

export interface PaymentSummaryDto {
  id: string
  siteId: string
  unitId: string
  unitDoorNumber?: string
  amount: number
  dueDate: string
  paidDate?: string
  status: PaymentStatus
  description?: string
  createdAt: string
}

export interface BulkCreatePaymentsDto {
  buildingId?: string
  amount: number
  dueDate: string
  description?: string
}

export interface CreatePaymentDto {
  unitId: string
  amount: number
  dueDate: string
  description?: string
}

export interface UpdatePaymentDto {
  amount: number
  dueDate: string
  description?: string
}

export interface UpdatePaymentStatusDto {
  status: PaymentStatus
  paidDate?: string
}
