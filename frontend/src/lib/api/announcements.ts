import { siteApi } from './client'
import {
  AnnouncementSummaryDto,
  AnnouncementDetailDto,
  CreateAnnouncementDto,
  UpdateAnnouncementDto,
} from '@/types/announcement'

export const announcementsApi = {
  getAll: () =>
    siteApi.get<AnnouncementSummaryDto[]>('/api/announcements'),

  getById: (id: string) =>
    siteApi.get<AnnouncementDetailDto>(`/api/announcements/${id}`),

  create: (data: CreateAnnouncementDto) =>
    siteApi.post<{ id: string }>('/api/announcements', data),

  update: (id: string, data: UpdateAnnouncementDto) =>
    siteApi.put(`/api/announcements/${id}`, data),

  delete: (id: string) =>
    siteApi.delete(`/api/announcements/${id}`),
}
