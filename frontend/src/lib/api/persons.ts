import { siteApi } from './client'
import { PersonDto, PersonDetailDto, InvitePersonDto, UpdatePersonDto } from '@/types/person'
import { PaginatedResult } from '@/types/api'

export const personsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<PersonDto>>('/api/persons', { params: { page, pageSize, search } }),

  getPending: () =>
    siteApi.get<PersonDto[]>('/api/persons/pending'),

  getById: (id: string) =>
    siteApi.get<PersonDetailDto>(`/api/persons/${id}`),

  invite: (data: InvitePersonDto) =>
    siteApi.post<{ id: string }>('/api/persons', data),

  update: (id: string, data: UpdatePersonDto) =>
    siteApi.put(`/api/persons/${id}`, data),

  approve: (id: string) =>
    siteApi.put(`/api/persons/${id}/approve`),

  reject: (id: string) =>
    siteApi.put(`/api/persons/${id}/reject`),

  remove: (id: string) =>
    siteApi.delete(`/api/persons/${id}`),
}
