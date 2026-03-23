import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react'

interface PaginationProps {
  page: number
  totalPages: number
  totalCount: number
  onPageChange: (page: number) => void
}

export function Pagination({ page, totalPages, totalCount, onPageChange }: PaginationProps) {
  return (
    <div className="flex items-center justify-end gap-1 mt-3 text-sm text-muted-foreground">
      <button
        onClick={() => onPageChange(1)}
        disabled={page === 1}
        className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
        title="İlk sayfa"
      >
        <ChevronsLeft className="h-4 w-4" />
      </button>
      <button
        onClick={() => onPageChange(Math.max(1, page - 1))}
        disabled={page === 1}
        className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
        title="Önceki sayfa"
      >
        <ChevronLeft className="h-4 w-4" />
      </button>
      <span className="px-2 tabular-nums">
        {page} / {totalPages} ({totalCount} Kayıt)
      </span>
      <button
        onClick={() => onPageChange(Math.min(totalPages, page + 1))}
        disabled={page === totalPages}
        className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
        title="Sonraki sayfa"
      >
        <ChevronRight className="h-4 w-4" />
      </button>
      <button
        onClick={() => onPageChange(totalPages)}
        disabled={page === totalPages}
        className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
        title="Son sayfa"
      >
        <ChevronsRight className="h-4 w-4" />
      </button>
    </div>
  )
}
