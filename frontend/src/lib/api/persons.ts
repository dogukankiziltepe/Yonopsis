import { siteApi } from './client'
import { PersonDto, PersonDetailDto, InvitePersonDto, UpdatePersonDto } from '@/types/person'
import { PaginatedResult } from '@/types/api'

export const personsApi = {
  getAll: (page = 1, pageSize = 20, search?: string) =>
    siteApi.get<PaginatedResult<PersonDto>>('/api/persons', { params: { page, pageSize, search } }),

  getById: (id: string) =>
    siteApi.get<PersonDetailDto>(`/api/persons/${id}`),

  invite: (data: InvitePersonDto) =>
    siteApi.post<{ id: string }>('/api/persons', data),

  update: (id: string, data: UpdatePersonDto) =>
    siteApi.put(`/api/persons/${id}`, data),

  remove: (id: string) =>
    siteApi.delete(`/api/persons/${id}`),
}
