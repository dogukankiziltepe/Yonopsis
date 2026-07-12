'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { isEmirleriApi, departmanlarApi } from '@/lib/api/teknik'
import type { IsEmri, Departman } from '@/types/teknik'
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
  const [departmanlar, setDepartmanlar] = useState<Departman[]>([])
  const [selectedDeptId, setSelectedDeptId] = useState<string>('')
  const [items, setItems] = useState<IsEmri[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    departmanlarApi.getAll().then(r => {
      if (r.success && r.data) setDepartmanlar(r.data)
    })
  }, [])

  const load = useCallback(async (pg: number, deptId: string, q: string) => {
    setLoading(true)
    const r = await isEmirleriApi.getAll(pg, PAGE_SIZE, q || undefined, undefined, deptId || undefined)
    if (r.success && r.data) {
      setItems(r.data.items)
      setTotal(r.data.totalCount)
    }
    setLoading(false)
  }, [])

  useEffect(() => {
    load(page, selectedDeptId, search)
  }, [page, selectedDeptId, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setPage(1)
    load(1, selectedDeptId, search)
  }

  const totalPages = Math.ceil(total / PAGE_SIZE)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Departmana Göre Görevler</h1>
        <span className="text-sm text-muted-foreground">{total} kayıt</span>
      </div>

      <div className="flex gap-2 mb-4">
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={selectedDeptId}
          onChange={e => { setSelectedDeptId(e.target.value); setPage(1) }}
        >
          <option value="">Tüm Departmanlar</option>
          {departmanlar.map(d => (
            <option key={d.id} value={d.id}>{d.ad}</option>
          ))}
        </select>

        <form onSubmit={handleSearch} className="flex gap-2 flex-1">
          <Input
            placeholder="Başlık ara..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            className="max-w-xs"
          />
          <Button type="submit" variant="outline" size="sm">
            <Search className="h-4 w-4" />
          </Button>
        </form>
      </div>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Başlık</th>
              <th className="text-left px-3 py-2 font-medium">Departman</th>
              <th className="text-left px-3 py-2 font-medium">Atanan</th>
              <th className="text-left px-3 py-2 font-medium">Öncelik</th>
              <th className="text-left px-3 py-2 font-medium">Durum</th>
              <th className="text-left px-3 py-2 font-medium">Tarih</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={6} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={6} className="text-center py-10 text-muted-foreground">Kayıt bulunamadı</td></tr>
            ) : items.map(item => (
              <tr key={item.id} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2 max-w-xs truncate" title={item.baslik}>{item.baslik}</td>
                <td className="px-3 py-2 text-muted-foreground">{item.departmanAdi ?? '—'}</td>
                <td className="px-3 py-2 text-muted-foreground">{item.atananKisiAdi ?? '—'}</td>
                <td className={`px-3 py-2 ${oncelikColor[item.oncelik]}`}>{IsEmriOncelikLabel[item.oncelik]}</td>
                <td className="px-3 py-2">
                  <Badge variant={durumVariant[item.durum]}>{IsEmriDurumLabel[item.durum]}</Badge>
                </td>
                <td className="px-3 py-2 text-muted-foreground">{new Date(item.createdAt).toLocaleDateString('tr-TR')}</td>
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
