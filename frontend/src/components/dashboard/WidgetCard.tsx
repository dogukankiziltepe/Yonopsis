'use client'

import { useEffect, useRef, useState } from 'react'
import { RefreshCw, Printer, Settings } from 'lucide-react'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { cn } from '@/lib/utils/cn'

export interface WidgetFilter {
  all: boolean
  from?: string
  to?: string
}

interface WidgetCardProps {
  title: string
  onRefresh?: () => void
  loading?: boolean
  filter?: WidgetFilter
  onFilterChange?: (filter: WidgetFilter) => void
  className?: string
  contentClassName?: string
  children: React.ReactNode
}

export function WidgetCard({
  title, onRefresh, loading, filter, onFilterChange, className, contentClassName, children,
}: WidgetCardProps) {
  const [settingsOpen, setSettingsOpen] = useState(false)
  const containerRef = useRef<HTMLDivElement>(null)
  const printRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setSettingsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClick)
    return () => document.removeEventListener('mousedown', handleClick)
  }, [])

  const handlePrint = () => {
    const content = printRef.current
    if (!content) return
    const win = window.open('', '_blank', 'width=900,height=700')
    if (!win) return
    win.document.write(
      `<html><head><title>${title}</title><style>
        body { font-family: system-ui, -apple-system, "Segoe UI", sans-serif; padding: 24px; color: #0b0b0b; }
        table { width: 100%; border-collapse: collapse; }
        th, td { border: 1px solid #e1e0d9; padding: 6px 10px; text-align: left; font-size: 13px; }
        th { background: #f3f4f6; }
        h1 { font-size: 18px; margin-bottom: 16px; }
      </style></head><body><h1>${title}</h1>${content.innerHTML}</body></html>`
    )
    win.document.close()
    win.focus()
    win.print()
    win.close()
  }

  return (
    <Card className={className}>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        <div className="flex items-center gap-1" ref={containerRef}>
          {onRefresh && (
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={onRefresh} disabled={loading} title="Yenile">
              <RefreshCw className={cn('h-3.5 w-3.5', loading && 'animate-spin')} />
            </Button>
          )}
          <Button variant="ghost" size="icon" className="h-7 w-7" onClick={handlePrint} title="Yazdır">
            <Printer className="h-3.5 w-3.5" />
          </Button>
          {filter && onFilterChange && (
            <div className="relative">
              <Button
                variant="ghost" size="icon" className="h-7 w-7"
                onClick={() => setSettingsOpen((v) => !v)} title="Ayarlar"
              >
                <Settings className="h-3.5 w-3.5" />
              </Button>
              {settingsOpen && (
                <div className="absolute right-0 top-8 z-20 w-64 rounded-md border bg-popover p-3 text-sm shadow-md">
                  <label className="mb-3 flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={filter.all}
                      onChange={(e) => onFilterChange({ ...filter, all: e.target.checked })}
                    />
                    Tümünü göster
                  </label>
                  {!filter.all && (
                    <div className="space-y-2">
                      <div>
                        <label className="mb-1 block text-xs text-muted-foreground">Başlangıç</label>
                        <Input
                          type="date" value={filter.from ?? ''}
                          onChange={(e) => onFilterChange({ ...filter, from: e.target.value })}
                        />
                      </div>
                      <div>
                        <label className="mb-1 block text-xs text-muted-foreground">Bitiş</label>
                        <Input
                          type="date" value={filter.to ?? ''}
                          onChange={(e) => onFilterChange({ ...filter, to: e.target.value })}
                        />
                      </div>
                    </div>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      </CardHeader>
      <CardContent className={contentClassName}>
        <div ref={printRef}>{children}</div>
      </CardContent>
    </Card>
  )
}
