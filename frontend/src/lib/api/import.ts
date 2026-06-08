import { siteApi } from './client'
import {
  ImportPreviewResult,
  ImportConfirmResult,
  BuildingImportRowData,
  UnitImportRowData,
  UserImportRowData,
} from '@/types/import'

export const importApi = {
  downloadTemplate: (type: 'buildings' | 'units' | 'users') =>
    siteApi.get<Blob>(`/api/import/template/${type}`, { responseType: 'blob' }),

  preview: (type: 'buildings' | 'units' | 'users', file: File) => {
    const form = new FormData()
    form.append('file', file)
    return siteApi.post<ImportPreviewResult>(`/api/import/preview/${type}`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  confirmBuildings: (rows: BuildingImportRowData[]) =>
    siteApi.post<ImportConfirmResult>('/api/import/confirm/buildings', { rows }),

  confirmUnits: (rows: UnitImportRowData[]) =>
    siteApi.post<ImportConfirmResult>('/api/import/confirm/units', { rows }),

  confirmUsers: (rows: UserImportRowData[]) =>
    siteApi.post<ImportConfirmResult>('/api/import/confirm/users', { rows }),
}
