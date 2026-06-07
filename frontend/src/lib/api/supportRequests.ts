import { siteApi } from './client'
import {
  SupportRequestSummaryDto,
  SupportRequestDetailDto,
  CreateSupportRequestDto,
  UpdateSupportRequestDto,
  UpdateSupportRequestStatusDto,
} from '@/types/supportRequest'

export const supportRequestsApi = {
  getAll: () =>
    siteApi.get<SupportRequestSummaryDto[]>('/api/support-requests'),

  getById: (id: string) =>
    siteApi.get<SupportRequestDetailDto>(`/api/support-requests/${id}`),

  create: (data: CreateSupportRequestDto) =>
    siteApi.post<{ id: string }>('/api/support-requests', data),

  update: (id: string, data: UpdateSupportRequestDto) =>
    siteApi.put(`/api/support-requests/${id}`, data),

  updateStatus: (id: string, data: UpdateSupportRequestStatusDto) =>
    siteApi.patch(`/api/support-requests/${id}/status`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/support-requests/${id}`),
}
