'use client'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { ChevronLeft, ChevronRight } from 'lucide-react'

interface PaginationControlsProps {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  search: string
  onPageChange: (page: number) => void
  onSearchChange: (value: string) => void
  searchPlaceholder?: string
}

export function PaginationControls({
  page,
  pageSize,
  totalCount,
  totalPages,
  search,
  onPageChange,
  onSearchChange,
  searchPlaceholder = 'Search...',
}: PaginationControlsProps) {
  const start = totalCount === 0 ? 0 : (page - 1) * pageSize + 1
  const end = Math.min(page * pageSize, totalCount)

  return (
    <div className="flex items-center justify-between gap-4 flex-wrap">
      <Input
        placeholder={searchPlaceholder}
        value={search}
        onChange={(e) => onSearchChange(e.target.value)}
        className="max-w-xs h-8 text-sm"
      />
      <div className="flex items-center gap-3 text-sm text-muted-foreground">
        <span>{totalCount > 0 ? `${start}–${end} / ${totalCount}` : '0 records'}</span>
        <div className="flex items-center gap-1">
          <Button
            variant="outline"
            size="icon"
            className="h-7 w-7"
            disabled={page <= 1}
            onClick={() => onPageChange(page - 1)}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="px-1">{page} / {Math.max(totalPages, 1)}</span>
          <Button
            variant="outline"
            size="icon"
            className="h-7 w-7"
            disabled={page >= totalPages}
            onClick={() => onPageChange(page + 1)}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}
