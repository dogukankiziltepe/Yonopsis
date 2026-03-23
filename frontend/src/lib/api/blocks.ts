import { siteApi } from './client'
import { Block, BlockUnit, CreateBlockDto, UpdateBlockDto, BlocksPage } from '@/types/block'
import { ApiResult } from '@/types/api'

export interface GetBlocksParams {
  page?: number
  pageSize?: number
  startsWith?: string
  sortBy?: string
  sortDesc?: boolean
}

export const blocksApi = {
  getAll: (params?: GetBlocksParams) =>
    siteApi.get<ApiResult<BlocksPage>>('/api/buildings', { params }),

  getById: (id: string) =>
    siteApi.get<ApiResult<Block>>(`/api/buildings/${id}`),

  getUnits: (id: string) =>
    siteApi.get<ApiResult<BlockUnit[]>>(`/api/buildings/${id}/units`),

  create: (data: CreateBlockDto) =>
    siteApi.post<ApiResult<{ id: string }>>('/api/buildings', data),

  update: (id: string, data: UpdateBlockDto) =>
    siteApi.put<ApiResult<null>>(`/api/buildings/${id}`, data),

  delete: (id: string) =>
    siteApi.delete<ApiResult<null>>(`/api/buildings/${id}`),
}
