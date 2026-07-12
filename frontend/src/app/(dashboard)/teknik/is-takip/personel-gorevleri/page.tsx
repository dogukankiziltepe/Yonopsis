'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, ChevronLeft, ChevronRight, User } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { isEmirleriApi } from '@/lib/api/teknik'
import type { IsEmri } from '@/types/teknik'
import { IsEmriDurum, IsEmriDurumLabel, IsEmriOncelik, IsEmriOncelikLabel } from '@/types/teknik'

const PAGE_SIZE = 20

const durumVariant: Record<IsEmriDurum, 'default' | 'secondary' | 'outline' | 'destructive'> = {
  [IsEmriDurum.YeniTalep]: 'secondary',
  [IsEmriDurum.Atandi]: 'outline',
  [IsEmriDurum.Devam]: 'default',
  [IsEmriDurum.Tamamlandi]: 'secondary',
  [IsEmriDurum.Iptal]: 'destructive',
}

const oncelikColor: Record<IsEmriOncelik, string> = {
  [IsEmriOncelik.Dusuk]: 'text-slate-500',
  [IsEmriOncelik.Normal]: 'text-blue-500',
  [IsEmriOncelik.Yuksek]: 'text-orange-500',
  [IsEmriOncelik.Kritik]: 'text-red-600 font-bold',
}

export default function PageComponent() {
  const [items, setItems] = useState<IsEmri[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(async (pg: number, q: string) => {
    setLoading(true)
    const r = await isEmirleriApi.getAll(pg, PAGE_SIZE, q || undefined)
    if (r.success && r.data) {
      setItems(r.data.items)
      setTotal(r.data.totalCount)
    }
    setLoading(false)
  }, [])

  useEffect(() => {
    load(page, search)
  }, [page, search, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
    setPage(1)
  }

  // Group items by assigned person
  const grouped = items.reduce<Record<string, IsEmri[]>>((acc, item) => {
    const key = item.atananKisiAdi ?? 'Atanmamış'
    if (!acc[key]) acc[key] = []
    acc[key].push(item)
    return acc
  }, {})

  const totalPages = Math.ceil(total / PAGE_SIZE)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Personele Göre Görevler</h1>
        <span className="text-sm text-muted-foreground">{total} kayıt</span>
      </div>

      <form onSubmit={handleSearch} className="flex gap-2 mb-4">
        <Input
          placeholder="Personel adı veya başlık ara..."
          value={inputVal}
          onChange={e => setInputVal(e.target.value)}
          className="max-w-sm"
        />
        <Button type="submit" variant="outline" size="sm">
          <Search className="h-4 w-4" />
        </Button>
        {search && (
          <Button type="button" variant="ghost" size="sm" onClick={() => { setSearch(''); setInputVal(''); setPage(1) }}>
            Temizle
          </Button>
        )}
      </form>

      <div className="flex-1 overflow-auto space-y-4">
        {loading ? (
          <div className="border rounded-lg flex items-center justify-center py-16 text-muted-foreground">Yükleniyor...</div>
        ) : Object.keys(grouped).length === 0 ? (
          <div className="border rounded-lg flex items-center justify-center py-16 text-muted-foreground">Kayıt bulunamadı</div>
        ) : Object.entries(grouped).map(([person, orders]) => (
          <div key={person} className="border rounded-lg overflow-hidden">
            <div className="bg-muted/50 px-3 py-2 flex items-center gap-2">
              <User className="h-4 w-4 text-muted-foreground" />
              <span className="font-medium text-sm">{person}</span>
              <span className="text-xs text-muted-foreground ml-auto">{orders.length} görev</span>
            </div>
            <table className="w-full text-sm">
              <tbody>
                {orders.map(item => (
                  <tr key={item.id} className="border-t hover:bg-muted/30">
                    <td className="px-3 py-2 max-w-xs truncate" title={item.baslik}>{item.baslik}</td>
                    <td className="px-3 py-2 text-muted-foreground">{item.departmanAdi ?? '—'}</td>
                    <td className={`px-3 py-2 ${oncelikColor[item.oncelik]}`}>{IsEmriOncelikLabel[item.oncelik]}</td>
                    <td className="px-3 py-2">
                      <Badge variant={durumVariant[item.durum]}>{IsEmriDurumLabel[item.durum]}</Badge>
                    </td>
                    <td className="px-3 py-2 text-muted-foreground text-xs">{new Date(item.createdAt).toLocaleDateString('tr-TR')}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ))}
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
