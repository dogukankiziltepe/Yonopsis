import { siteApi } from './client'
import {
  PaymentSummaryDto,
  CreatePaymentDto,
  UpdatePaymentDto,
  UpdatePaymentStatusDto,
} from '@/types/payment'

export const paymentsApi = {
  getAll: () =>
    siteApi.get<PaymentSummaryDto[]>('/api/payments'),

  getById: (id: string) =>
    siteApi.get<PaymentSummaryDto>(`/api/payments/${id}`),

  getByUnit: (unitId: string) =>
    siteApi.get<PaymentSummaryDto[]>(`/api/payments/by-unit/${unitId}`),

  create: (data: CreatePaymentDto) =>
    siteApi.post<{ id: string }>('/api/payments', data),

  update: (id: string, data: UpdatePaymentDto) =>
    siteApi.put(`/api/payments/${id}`, data),

  updateStatus: (id: string, data: UpdatePaymentStatusDto) =>
    siteApi.patch(`/api/payments/${id}/status`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/payments/${id}`),
}
