'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, ChevronLeft, ChevronRight, Receipt, ExternalLink } from 'lucide-react'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { tahsilatMakbuzlariApi } from '@/lib/api/finans'
import type { TahsilatMakbuzu } from '@/types/finans'

const PAGE_SIZE = 20

export default function PageComponent() {
  const [items, setItems] = useState<TahsilatMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(async (pg: number, q: string) => {
    setLoading(true)
    const r = await tahsilatMakbuzlariApi.getAll(pg, PAGE_SIZE, q || undefined)
    if (r.success && r.data) {
      setItems(r.data.items)
      setTotal(r.data.totalCount)
    }
    setLoading(false)
  }, [])

  useEffect(() => { load(page, search) }, [page, search, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
    setPage(1)
  }

  const totalPages = Math.ceil(total / PAGE_SIZE)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Tahsilat Makbuzları</h1>
        <div className="flex items-center gap-2">
          <span className="text-sm text-muted-foreground">{total} kayıt</span>
          <Button size="sm" variant="outline" asChild>
            <Link href="/finans/tahsilat-makbuzu">
              <ExternalLink className="h-4 w-4 mr-1" />Tam Ekran
            </Link>
          </Button>
        </div>
      </div>

      <form onSubmit={handleSearch} className="flex gap-2 mb-4">
        <Input
          placeholder="Evrak no veya kişi ara..."
          value={inputVal}
          onChange={e => setInputVal(e.target.value)}
          className="max-w-sm"
        />
        <Button type="submit" variant="outline" size="sm"><Search className="h-4 w-4" /></Button>
        {search && (
          <Button type="button" variant="ghost" size="sm" onClick={() => { setSearch(''); setInputVal(''); setPage(1) }}>
            Temizle
          </Button>
        )}
      </form>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Evrak No</th>
              <th className="text-left px-3 py-2 font-medium">Tarih</th>
              <th className="text-left px-3 py-2 font-medium">Kişi / Daire</th>
              <th className="text-right px-3 py-2 font-medium">Tutar</th>
              <th className="text-left px-3 py-2 font-medium">Açıklama</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={5} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={5} className="text-center py-16 text-muted-foreground">
                  <Receipt className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Tahsilat makbuzu bulunamadı</p>
                </td>
              </tr>
            ) : items.map(item => (
              <tr key={item.id} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2 font-mono text-xs">{item.evrakNo}</td>
                <td className="px-3 py-2 text-muted-foreground">{new Date(item.islemTarihi).toLocaleDateString('tr-TR')}</td>
                <td className="px-3 py-2">{item.borcluAdi ?? item.kasaBankaAdi ?? '—'}</td>
                <td className="px-3 py-2 text-right font-medium text-green-600">
                  {item.odemeTutari.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} ₺
                </td>
                <td className="px-3 py-2 text-muted-foreground text-xs truncate max-w-xs">{item.aciklama ?? '—'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-3">
          <span className="text-sm text-muted-foreground">Sayfa {page} / {totalPages}</span>
          <div className="flex gap-1">
            <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
