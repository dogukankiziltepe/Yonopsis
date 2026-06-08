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

  createCheckout: (id: string, successUrl: string, cancelUrl: string) =>
    siteApi.post<{ checkoutUrl: string }>(`/api/online-payments/${id}/checkout`, { successUrl, cancelUrl }),

  downloadTemplate: (items: string[]) =>
    siteApi.get('/api/payments/template', {
      params: { items },
      paramsSerializer: { indexes: null },
      responseType: 'blob',
    }),

  importExcel: (file: File, dueDate: string, description?: string) => {
    const form = new FormData()
    form.append('file', file)
    form.append('dueDate', dueDate)
    if (description) form.append('description', description)
    return siteApi.post<{ created: number; skipped: number; errors: string[]; message: string }>(
      '/api/payments/import',
      form,
      { headers: { 'Content-Type': 'multipart/form-data' } },
    )
  },
}
