'use client'

import { useEffect, useState, useMemo } from 'react'
import Link from 'next/link'
import { Plus, Search, Building2, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import { Pagination } from '@/components/ui/pagination'
import { sitesApi } from '@/lib/api/sites'
import { SiteSummaryDto, DbMode } from '@/types/site'

type SortKey = 'name' | 'city' | 'blockCount' | 'unitCount' | 'createdAt'
type SortDir = 'asc' | 'desc'

const PAGE_SIZE = 20

const COLUMNS: { key: SortKey; label: string }[] = [
  { key: 'name', label: 'Site Adı' },
  { key: 'city', label: 'Şehir' },
  { key: 'blockCount', label: 'Blok / Daire' },
  { key: 'createdAt', label: 'Oluşturulma' },
]

function SortIndicator({ col, sortKey, sortDir }: { col: SortKey; sortKey: SortKey; sortDir: SortDir }) {
  if (sortKey !== col) return <span className="ml-1 text-muted-foreground/40">⇅</span>
  return <span className="ml-1">{sortDir === 'asc' ? '↑' : '↓'}</span>
}

export default function AdminSitesPage() {
  const [sites, setSites] = useState<SiteSummaryDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState<'all' | 'active' | 'passive'>('all')
  const [dbModeFilter, setDbModeFilter] = useState<'all' | 'shared' | 'dedicated'>('all')
  const [sortKey, setSortKey] = useState<SortKey>('name')
  const [sortDir, setSortDir] = useState<SortDir>('asc')
  const [page, setPage] = useState(1)

  const load = () => {
    setLoading(true)
    sitesApi
      .getAll()
      .then((res) => setSites(res.data ?? []))
      .catch(() => setError('Siteler yüklenemedi.'))
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const filtered = useMemo(() => {
    let result = sites

    if (search.trim()) {
      const q = search.toLowerCase()
      result = result.filter(
        (s) =>
          s.name.toLowerCase().includes(q) ||
          s.city?.toLowerCase().includes(q) ||
          s.district?.toLowerCase().includes(q)
      )
    }

    if (statusFilter === 'active') result = result.filter((s) => s.isActive)
    if (statusFilter === 'passive') result = result.filter((s) => !s.isActive)
    if (dbModeFilter === 'shared') result = result.filter((s) => s.dbMode === DbMode.Shared)
    if (dbModeFilter === 'dedicated') result = result.filter((s) => s.dbMode === DbMode.Dedicated)

    result = [...result].sort((a, b) => {
      const va = String(a[sortKey] ?? '')
      const vb = String(b[sortKey] ?? '')
      return sortDir === 'asc' ? va.localeCompare(vb, 'tr') : vb.localeCompare(va, 'tr')
    })

    return result
  }, [sites, search, statusFilter, dbModeFilter, sortKey, sortDir])

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
    if (!confirm('Bu siteyi silmek istediğinize emin misiniz?')) return
    try {
      await sitesApi.delete(id)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  const filterBtnClass = (active: boolean) =>
    `px-3 py-1 text-xs rounded-md border transition-colors ${
      active
        ? 'bg-primary text-primary-foreground border-primary'
        : 'text-muted-foreground border-border hover:bg-muted'
    }`

  return (
    <div className="flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Siteler</h1>
        <Link href="/admin/sites/new">
          <Button size="sm">
            <Plus className="h-4 w-4 mr-1" />
            Yeni Site
          </Button>
        </Link>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-center gap-2 mb-3">
        {/* Search */}
        <div className="relative flex-1 max-w-sm">
          <Search className="h-4 w-4 absolute left-2.5 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Site adı, şehir, ilçe..."
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

        {/* Status filter */}
        <div className="flex gap-1">
          <button className={filterBtnClass(statusFilter === 'all')} onClick={() => { setStatusFilter('all'); setPage(1) }}>Tümü</button>
          <button className={filterBtnClass(statusFilter === 'active')} onClick={() => { setStatusFilter('active'); setPage(1) }}>Aktif</button>
          <button className={filterBtnClass(statusFilter === 'passive')} onClick={() => { setStatusFilter('passive'); setPage(1) }}>Pasif</button>
        </div>

        {/* DbMode filter */}
        <div className="flex gap-1">
          <button className={filterBtnClass(dbModeFilter === 'all')} onClick={() => { setDbModeFilter('all'); setPage(1) }}>Tüm DB</button>
          <button className={filterBtnClass(dbModeFilter === 'shared')} onClick={() => { setDbModeFilter('shared'); setPage(1) }}>Shared</button>
          <button className={filterBtnClass(dbModeFilter === 'dedicated')} onClick={() => { setDbModeFilter('dedicated'); setPage(1) }}>Dedicated</button>
        </div>

        {filtered.length !== sites.length && (
          <span className="text-xs text-muted-foreground">{filtered.length} sonuç</span>
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
      ) : sites.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12 text-center">
            <Building2 className="h-10 w-10 text-muted-foreground/50 mb-3" />
            <p className="text-muted-foreground">Henüz site eklenmemiş</p>
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
                    <th className="text-left px-4 py-2.5 font-medium text-muted-foreground whitespace-nowrap">DB</th>
                    <th className="text-left px-4 py-2.5 font-medium text-muted-foreground whitespace-nowrap">Durum</th>
                    <th className="px-4 py-2.5" />
                  </tr>
                </thead>
                <tbody>
                  {paginated.length === 0 ? (
                    <tr>
                      <td colSpan={7} className="text-center py-10 text-muted-foreground">
                        Arama kriterine uygun site bulunamadı
                      </td>
                    </tr>
                  ) : (
                    paginated.map((site) => (
                      <tr key={site.id} className="border-b last:border-0 hover:bg-muted/30">
                        <td className="px-4 py-2.5 font-medium">{site.name}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">{site.city ?? '-'}</td>
                        <td className="px-4 py-2.5 text-muted-foreground">
                          {site.blockCount} / {site.unitCount}
                        </td>
                        <td className="px-4 py-2.5 text-muted-foreground text-xs">
                          {new Date(site.createdAt).toLocaleDateString('tr-TR')}
                        </td>
                        <td className="px-4 py-2.5">
                          <Badge variant={site.dbMode === DbMode.Dedicated ? 'default' : 'secondary'}>
                            {site.dbMode === DbMode.Dedicated ? 'Dedicated' : 'Shared'}
                          </Badge>
                        </td>
                        <td className="px-4 py-2.5">
                          <Badge variant={site.isActive ? 'default' : 'secondary'}>
                            {site.isActive ? 'Aktif' : 'Pasif'}
                          </Badge>
                        </td>
                        <td className="px-4 py-2.5">
                          <div className="flex gap-1 justify-end">
                            <Link href={`/admin/sites/${site.id}`}>
                              <Button variant="ghost" size="sm" className="h-7 text-xs">
                                Düzenle
                              </Button>
                            </Link>
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-7 w-7 text-destructive hover:text-destructive"
                              onClick={() => handleDelete(site.id)}
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
