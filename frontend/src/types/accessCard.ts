export interface AccessCardSummaryDto {
  id: string
  siteId: string
  userId: string
  cardNumber: string
  isActive: boolean
  issueDate: string
  expiryDate?: string
  notes?: string
  createdAt: string
}

export interface CreateAccessCardDto {
  userId: string
  cardNumber: string
  issueDate: string
  expiryDate?: string
  notes?: string
}

export interface UpdateAccessCardDto {
  cardNumber: string
  isActive: boolean
  issueDate: string
  expiryDate?: string
  notes?: string
}
