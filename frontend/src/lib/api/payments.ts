import { siteApi } from './client'
import {
  PaymentSummaryDto,
  CreatePaymentDto,
  BulkCreatePaymentsDto,
  UpdatePaymentDto,
  UpdatePaymentStatusDto,
} from '@/types/payment'
import { PaginatedResult } from '@/types/api'

export const paymentsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<PaymentSummaryDto>>('/api/payments', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<PaymentSummaryDto>(`/api/payments/${id}`),

  getByUnit: (unitId: string) =>
    siteApi.get<PaymentSummaryDto[]>(`/api/payments/by-unit/${unitId}`),

  create: (data: CreatePaymentDto) =>
    siteApi.post<{ id: string }>('/api/payments', data),

  bulkCreate: (data: BulkCreatePaymentsDto) =>
    siteApi.post<{ count: number; message: string }>('/api/payments/bulk', data),

  markOverdue: () =>
    siteApi.post<{ count: number; message: string }>('/api/payments/mark-overdue'),

  update: (id: string, data: UpdatePaymentDto) =>
    siteApi.put(`/api/payments/${id}`, data),

  updateStatus: (id: string, data: UpdatePaymentStatusDto) =>
    siteApi.patch(`/api/payments/${id}/status`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/payments/${id}`),
}
