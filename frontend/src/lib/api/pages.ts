import { siteApi } from '@/lib/api/client'
import { ApiResult } from '@/types/api'
import { PageDto } from '@/types/page'

export async function getMyPages(): Promise<PageDto[]> {
  try {
    const { data } = await siteApi.get<PageDto[]>('/api/pages/my-pages')
    return data
  } catch {
    return [];
  }
}
