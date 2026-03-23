import { siteApi } from '@/lib/api/client'
import { ApiResult } from '@/types/api'
import { PageDto } from '@/types/page'

export async function getMyPages(): Promise<ApiResult<PageDto[]>> {
  try {
    const { data } = await siteApi.get<ApiResult<PageDto[]>>('/api/pages/my-pages')
    return data
  } catch {
    return { isSuccess: false, value: null, error: 'Sayfalar yüklenemedi.' }
  }
}
