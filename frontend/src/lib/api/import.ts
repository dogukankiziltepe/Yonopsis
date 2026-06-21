import { siteApi } from './client'
import { ImportPreview, ImportResult, ImportType } from '@/types/import'

export const importApi = {
  downloadTemplate: (type: ImportType) =>
    siteApi.get(`/api/import/template/${type}`, { responseType: 'blob' }),

  preview: (type: ImportType, file: File) => {
    const fd = new FormData()
    fd.append('file', file)
    return siteApi.post<ImportPreview>(`/api/import/preview/${type}`, fd, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  confirm: (type: ImportType, rows: Record<string, string | null>[]) =>
    siteApi.post<ImportResult>(`/api/import/confirm/${type}`, { rows }),
}
