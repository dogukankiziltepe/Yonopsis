import { siteApi } from './client'
import { UploadedFileDto } from '@/types/file'

export const filesApi = {
  upload: (file: File) => {
    const form = new FormData()
    form.append('file', file)
    return siteApi.post<UploadedFileDto>('/api/files', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  getUrl: (id: string) => `/api/files/${id}`,

  delete: (id: string) => siteApi.delete(`/api/files/${id}`),
}
