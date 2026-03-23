import { loginApi } from './client'
import { SiteSummaryDto, SiteDetailDto, CreateSiteDto, UpdateSiteDto } from '@/types/site'
import { ApiResult } from '@/types/api'

export const sitesApi = {
  getAll: () =>
    loginApi.get<SiteSummaryDto[]>('/api/sites'),

  getById: (id: string) =>
    loginApi.get<ApiResult<SiteDetailDto>>(`/api/sites/${id}`),

  create: (data: CreateSiteDto) =>
    loginApi.post<ApiResult<{ id: string }>>('/api/sites', data),

  update: (id: string, data: UpdateSiteDto) =>
    loginApi.put<ApiResult<null>>(`/api/sites/${id}`, data),

  delete: (id: string) =>
    loginApi.delete<ApiResult<null>>(`/api/sites/${id}`),
}
