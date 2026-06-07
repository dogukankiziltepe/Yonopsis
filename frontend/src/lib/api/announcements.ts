import { siteApi } from './client'
import {
  AnnouncementSummaryDto,
  AnnouncementDetailDto,
  CreateAnnouncementDto,
  UpdateAnnouncementDto,
} from '@/types/announcement'
import { PaginatedResult } from '@/types/api'

export const announcementsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<AnnouncementSummaryDto>>('/api/announcements', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<AnnouncementDetailDto>(`/api/announcements/${id}`),

  create: (data: CreateAnnouncementDto) =>
    siteApi.post<{ id: string }>('/api/announcements', data),

  update: (id: string, data: UpdateAnnouncementDto) =>
    siteApi.put(`/api/announcements/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/announcements/${id}`),
}
