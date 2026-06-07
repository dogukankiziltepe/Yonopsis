import { siteApi } from './client'
import {
  SupportRequestSummaryDto,
  SupportRequestDetailDto,
  CreateSupportRequestDto,
  UpdateSupportRequestDto,
  UpdateSupportRequestStatusDto,
} from '@/types/supportRequest'
import { PaginatedResult } from '@/types/api'

export const supportRequestsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<SupportRequestSummaryDto>>('/api/support-requests', { params: { page, pageSize, search } }),

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
