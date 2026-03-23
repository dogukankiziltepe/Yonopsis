import { PaginatedResult } from './api'

export interface Block {
  id: string
  siteId: string
  code: string
  name: string
  ada?: string
  aidat: number
  description?: string
  unitCount: number
  createdAt: string
  updatedAt: string
}

export interface BlockUnit {
  id: string
  number: string
  isOccupied: boolean
  landShare?: string
  ownerName?: string
  renterName?: string
}

export interface CreateBlockDto {
  code: string
  name: string
  ada?: string
  aidat?: number
  description?: string
}

export interface UpdateBlockDto {
  code: string
  name: string
  ada?: string
  aidat?: number
  description?: string
}

export type BlocksPage = PaginatedResult<Block>
