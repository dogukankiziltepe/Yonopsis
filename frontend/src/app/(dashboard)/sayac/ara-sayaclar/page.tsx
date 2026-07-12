'use client'

import { useEffect, useState, useCallback } from 'react'
import { ChevronLeft, ChevronRight, Gauge, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { daireSayaclarApi, anaSayaclarApi } from '@/lib/api/sayac'
import type { DaireSayac, AnaSayac } from '@/types/sayac'
import { SayacTipi, SayacTipiLabel } from '@/types/sayac'

const PAGE_SIZE = 50

export default function PageComponent() {
  const [items, setItems] = useState<DaireSayac[]>([])
  const [anaSayaclar, setAnaSayaclar] = useState<AnaSayac[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [tipFilter, setTipFilter] = useState<SayacTipi | ''>('')
  const [anaSayacFilter, setAnaSayacFilter] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    anaSayaclarApi.getAllList().then(r => {
      if (r.success && r.data) setAnaSayaclar(r.data)
    })
  }, [])

  const load = useCallback(async (pg: number, q: string, anaSayacId: string, tip: SayacTipi | '') => {
    setLoading(true)
    const r = await daireSayaclarApi.getAll(pg, PAGE_SIZE, q || undefined, anaSayacId || undefined, tip || undefined)
    if (r.success && r.data) {
      setItems(r.data.items)
      setTotal(r.data.totalCount)
    }
    setLoading(false)
  }, [])

  useEffect(() => {
    load(page, search, anaSayacFilter, tipFilter)
  }, [page, search, anaSayacFilter, tipFilter, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
    setPage(1)
  }

  const totalPages = Math.ceil(total / PAGE_SIZE)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Ara Sayaçlar</h1>
        <span className="text-sm text-muted-foreground">{total} kayıt</span>
      </div>

      <div className="flex flex-wrap gap-2 mb-4">
        <form onSubmit={handleSearch} className="flex gap-2">
          <Input
            placeholder="Seri no veya daire ara..."
            value={inputVal}
            onChange={e => setInputVal(e.target.value)}
            className="w-48"
          />
          <Button type="submit" variant="outline" size="sm"><Search className="h-4 w-4" /></Button>
        </form>
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={anaSayacFilter}
          onChange={e => { setAnaSayacFilter(e.target.value); setPage(1) }}
        >
          <option value="">Tüm Ana Sayaçlar</option>
          {anaSayaclar.map(a => (
            <option key={a.id} value={a.id}>{a.ad} ({SayacTipiLabel[a.tip]})</option>
          ))}
        </select>
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={tipFilter}
          onChange={e => { setTipFilter(e.target.value as SayacTipi | ''); setPage(1) }}
        >
          <option value="">Tüm Tipler</option>
          {Object.values(SayacTipi).filter(v => typeof v === 'number').map(t => (
            <option key={t} value={t}>{SayacTipiLabel[t as SayacTipi]}</option>
          ))}
        </select>
      </div>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Daire</th>
              <th className="text-left px-3 py-2 font-medium">Ana Sayaç</th>
              <th className="text-left px-3 py-2 font-medium">Tip</th>
              <th className="text-left px-3 py-2 font-medium">Seri No</th>
              <th className="text-left px-3 py-2 font-medium">Marka</th>
              <th className="text-left px-3 py-2 font-medium">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={6} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={6} className="text-center py-16 text-muted-foreground">
                  <Gauge className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Ara sayaç bulunamadı</p>
                  <p className="text-xs mt-1">Sayaçlar &quot;Daire Sayaçları&quot; sayfasından eklenebilir</p>
                </td>
              </tr>
            ) : items.map(item => (
              <tr key={item.id} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2 font-medium">{item.unitDoorNumber ?? '—'}</td>
                <td className="px-3 py-2 text-muted-foreground">{item.anaSayacAdi ?? '—'}</td>
                <td className="px-3 py-2">
                  <Badge variant="outline">{SayacTipiLabel[item.tip]}</Badge>
                </td>
                <td className="px-3 py-2 text-muted-foreground font-mono text-xs">{item.seriNo ?? '—'}</td>
                <td className="px-3 py-2 text-muted-foreground">{item.marka ?? '—'}</td>
                <td className="px-3 py-2">
                  <Badge variant={item.isActive ? 'default' : 'secondary'}>
                    {item.isActive ? 'Aktif' : 'Pasif'}
                  </Badge>
                </td>
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
