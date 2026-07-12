import { siteApi } from './client'
import { PersonDto, PersonDetailDto, InvitePersonDto, UpdatePersonDto } from '@/types/person'
import {
  PersonFullDetailDto,
  UpdatePersonGeneralInfoDto,
  UpdatePersonDetailInfoDto,
  UpdatePersonIdentityInfoDto,
} from '@/types/personDetail'
import { PaginatedResult } from '@/types/api'

export const personsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<PersonDto>>('/api/persons', { params: { page, pageSize, search } }),

  getPending: () =>
    siteApi.get<PersonDto[]>('/api/persons/pending'),

  getById: (id: string) =>
    siteApi.get<PersonDetailDto>(`/api/persons/${id}`),

  getFullDetail: (id: string) =>
    siteApi.get<PersonFullDetailDto>(`/api/persons/${id}/detail`),

  invite: (data: InvitePersonDto) =>
    siteApi.post<{ id: string }>('/api/persons', data),

  update: (id: string, data: UpdatePersonDto) =>
    siteApi.put(`/api/persons/${id}`, data),

  updateGeneralInfo: (id: string, data: UpdatePersonGeneralInfoDto) =>
    siteApi.put(`/api/persons/${id}/general`, data),

  updateDetailInfo: (id: string, data: UpdatePersonDetailInfoDto) =>
    siteApi.put(`/api/persons/${id}/detail-info`, data),

  updateIdentityInfo: (id: string, data: UpdatePersonIdentityInfoDto) =>
    siteApi.put(`/api/persons/${id}/identity`, data),

  approve: (id: string) =>
    siteApi.put(`/api/persons/${id}/approve`),

  reject: (id: string) =>
    siteApi.put(`/api/persons/${id}/reject`),

  remove: (id: string) =>
    siteApi.delete(`/api/persons/${id}`),
}
