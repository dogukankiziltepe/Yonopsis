'use client'

import { useCallback, useEffect, useState } from 'react'
import { Search, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '@/components/ui/table'

interface PickerDialogColumn<T> {
  key: string
  label: string
  render: (item: T) => React.ReactNode
}

interface PickerDialogProps<T> {
  label: string
  placeholder?: string
  displayValue?: string | null
  fetchItems: (search: string) => Promise<T[]>
  columns: PickerDialogColumn<T>[]
  getId: (item: T) => string
  onSelect: (item: T) => void
  onClear?: () => void
  disabled?: boolean
}

export function PickerDialog<T>({
  label,
  placeholder = 'Seçmek için tıklayın...',
  displayValue,
  fetchItems,
  columns,
  getId,
  onSelect,
  onClear,
  disabled,
}: PickerDialogProps<T>) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')
  const [items, setItems] = useState<T[]>([])
  const [loading, setLoading] = useState(false)

  const load = useCallback((s: string) => {
    setLoading(true)
    fetchItems(s)
      .then(setItems)
      .finally(() => setLoading(false))
  }, [fetchItems])

  useEffect(() => {
    if (open) load(search)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open])

  const handleSelect = (item: T) => {
    onSelect(item)
    setOpen(false)
  }

  return (
    <div className="space-y-1">
      <Label className="text-xs">{label}</Label>
      <div className="flex gap-1">
        <Input
          readOnly
          value={displayValue ?? ''}
          placeholder={placeholder}
          onClick={() => !disabled && setOpen(true)}
          className="cursor-pointer"
          disabled={disabled}
        />
        {displayValue && onClear && (
          <Button type="button" size="icon" variant="ghost" onClick={onClear} disabled={disabled}>
            <X className="h-4 w-4" />
          </Button>
        )}
      </div>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>{label} — Seçim</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="relative">
              <Search className="absolute left-2 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
              <Input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && load(search)}
                placeholder="Ara..."
                className="pl-7 h-8"
                autoFocus
              />
            </div>
            <div className="max-h-96 overflow-auto border rounded-md">
              <Table>
                <TableHeader>
                  <TableRow>
                    {columns.map((c) => (
                      <TableHead key={c.key}>{c.label}</TableHead>
                    ))}
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {loading ? (
                    <TableRow>
                      <TableCell colSpan={columns.length} className="text-center text-muted-foreground">
                        Yükleniyor...
                      </TableCell>
                    </TableRow>
                  ) : items.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={columns.length} className="text-center text-muted-foreground">
                        Kayıt bulunamadı.
                      </TableCell>
                    </TableRow>
                  ) : (
                    items.map((item) => (
                      <TableRow
                        key={getId(item)}
                        className="cursor-pointer"
                        onClick={() => handleSelect(item)}
                      >
                        {columns.map((c) => (
                          <TableCell key={c.key}>{c.render(item)}</TableCell>
                        ))}
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
