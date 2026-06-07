export interface AnnouncementSummaryDto {
  id: string
  siteId: string
  title: string
  isPinned: boolean
  publishDate?: string
  expiryDate?: string
  createdAt: string
}

export interface AnnouncementDetailDto extends AnnouncementSummaryDto {
  createdByUserId: string
  content: string
  updatedAt?: string
}

export interface CreateAnnouncementDto {
  title: string
  content: string
  isPinned: boolean
  publishDate?: string
  expiryDate?: string
}

export interface UpdateAnnouncementDto {
  title: string
  content: string
  isPinned: boolean
  publishDate?: string
  expiryDate?: string
}
