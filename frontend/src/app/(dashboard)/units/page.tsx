'use client'

import { useEffect, useState, useMemo, useCallback } from 'react'
import { Plus, Search, Home, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent } from '@/components/ui/card'
import { Pagination } from '@/components/ui/pagination'
import { unitsApi } from '@/lib/api/units'
import { Unit } from '@/types/unit'

type SortKey = 'number' | 'buildingName' | 'floor' | 'type' | 'ownerName'
type SortDir = 'asc' | 'desc'

const PAGE_SIZE = 20

const COLUMNS: { key: SortKey; label: string }[] = [
  { key: 'number', label: 'Daire No' },
  { key: 'buildingName', label: 'Blok' },
  { key: 'floor', label: 'Kat' },
  { key: 'type', label: 'Tip' },
  { key: 'ownerName', label: 'Sahibi' },
]

function SortIndicator({ col, sortKey, sortDir }: { col: SortKey; sortKey: SortKey; sortDir: SortDir }) {
  if (sortKey !== col) return <span className="ml-1 text-muted-foreground/40">⇅</span>
  return <span className="ml-1">{sortDir === 'asc' ? '↑' : '↓'}</span>
}

export default function UnitsPage() {
  const [units, setUnits] = useState<Unit[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [search, setSearch] = useState('')
  const [sortKey, setSortKey] = useState<SortKey>('number')
  const [sortDir, setSortDir] = useState<SortDir>('asc')
  const [page, setPage] = useState(1)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    unitsApi
      .getAll()
      .then((res) => setUnits(res.data.value ?? []))
      .catch(() => setError('Daireler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const filtered = useMemo(() => {
    let result = units

    if (search.trim()) {
      const q = search.toLowerCase()
      result = result.filter(
        (u) =>
          u.number.toLowerCase().includes(q) ||
          u.buildingName?.toLowerCase().includes(q) ||
          u.ownerName?.toLowerCase().includes(q) ||
          u.renterName?.toLowerCase().includes(q) ||
          u.type?.toLowerCase().includes(q)
      )
    }

    result = [...result].sort((a, b) => {
      const va = String(a[sortKey] ?? '')
      const vb = String(b[sortKey] ?? '')
      return sortDir === 'asc' ? va.localeCompare(vb, 'tr') : vb.localeCompare(va, 'tr')
    })

    return result
  }, [units, search, sortKey, sortDir])

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE))
  const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'))
    } else {
      setSortKey(key)
      setSortDir('asc')
    }
    setPage(1)
  }

  const handleSearch = (value: string) => {
    setSearch(value)
    setPage(1)
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu daireyi silmek istediğinize emin misiniz?')) return
    try {
      await unitsApi.delete(id)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  return (
    <div className="flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Daireler</h1>
        <Button size="sm">
          <Plus className="h-4 w-4 mr-1" />
          Yeni Daire
        </Button>
      </div>

      {/* Filters */}
      <div className="flex items-center gap-2 mb-3">
        <div className="relative flex-1 max-w-sm">
          <Search className="h-4 w-4 absolute left-2.5 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Daire no, blok, sahibi..."
            value={search}
            onChange={(e) => handleSearch(e.target.value)}
            className="pl-8 h-8 text-sm"
          />
          {search && (
            <button
              onClick={() => handleSearch('')}
              className="absolute right-2 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
            >
              <X className="h-3.5 w-3.5" />
            </button>
          )}
        </div>
        {search && (
          <span className="text-xs text-muted-foreground">
            {filtered.length} sonuç
          </span>
        )}
      </div>

      {error && (
        <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3 mb-3">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      {/* Table */}
      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : units.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12 text-center">
            <Home className="h-10 w-10 text-muted-foreground/50 mb-3" />
            <p className="text-muted-foreground">Henüz daire eklenmemiş</p>
          </CardContent>
        </Card>
      ) : (
        <>
          <div className="border rounded-lg overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-muted/50">
                    {COLUMNS.map((col) => (
                      <th
                        key={col.key}
                        onClick={() => handleSort(col.key)}
                        className="text-left px-4 py-2.5 font-medium text-muted-foreground cursor-pointer select-none hover:text-foreground whitespace-nowrap"
                      >
                        {col.label}
                        <SortIndicator col={col.key} sortKey={sortKey} sortDir={sortDir} />
                      </th>
                    ))}
                    <th className="px-4 py-2.5" />
                  </tr>
                </thead>
                <tbody>
                  {paginated.length === 0 ? (
                    <tr>
                      <td colSpan={6} className="text-center py-10 text-muted-foreground">
                        Arama kriterine uygun daire bulunamadı
                      </td>
                    </tr>
                  ) : (
                    paginated.map((u) => (
                      <tr key={u.id} className="border-b last:border-0 hover:bg-muted/30">
                        <td className="px-4 py-2.5 font-medium">{u.number}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">{u.buildingName ?? '-'}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">{u.floor ?? '-'}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">{u.type ?? '-'}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">{u.ownerName ?? '-'}</td>
                        <td className="px-4 py-2.5">
                          <div className="flex gap-1 justify-end">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-7 w-7 text-destructive hover:text-destructive"
                              onClick={() => handleDelete(u.id)}
                            >
                              <X className="h-3.5 w-3.5" />
                            </Button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <Pagination
            page={page}
            totalPages={totalPages}
            totalCount={filtered.length}
            onPageChange={setPage}
          />
        </>
      )}
    </div>
  )
}
