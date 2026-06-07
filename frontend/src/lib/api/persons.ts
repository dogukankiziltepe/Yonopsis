import { siteApi } from './client'
import { PersonDto, PersonDetailDto, InvitePersonDto, UpdatePersonDto } from '@/types/person'

export const personsApi = {
  getAll: () =>
    siteApi.get<PersonDto[]>('/api/persons'),

  getById: (id: string) =>
    siteApi.get<PersonDetailDto>(`/api/persons/${id}`),

  invite: (data: InvitePersonDto) =>
    siteApi.post<{ id: string }>('/api/persons', data),

  update: (id: string, data: UpdatePersonDto) =>
    siteApi.put(`/api/persons/${id}`, data),

  remove: (id: string) =>
    siteApi.delete(`/api/persons/${id}`),
}
