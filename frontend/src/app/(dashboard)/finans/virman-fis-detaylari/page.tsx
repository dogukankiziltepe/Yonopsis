'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { Plus, ListTree, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { virmanFisDetaylariApi } from '@/lib/api/finans'
import { VirmanHesapTuruLabel } from '@/types/finans'
import type { VirmanSatirListItem } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const tarih = (s?: string) => (s ? new Date(s).toLocaleDateString('tr-TR') : '—')

export default function VirmanFisDetaylariListPage() {
  const router = useRouter()
  const [items, setItems] = useState<VirmanSatirListItem[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await virmanFisDetaylariApi.getAll(page, PAGE_SIZE, search || undefined)
      setItems(res.data.items ?? [])
      setTotal(res.data.totalCount ?? 0)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const totalPages = Math.ceil(total / PAGE_SIZE)

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Virman Fişi Detayları</h1>
        <Button size="sm" onClick={() => router.push('/finans/virman-fis-detaylari/yeni')}>
          <Plus className="h-4 w-4 mr-1" />Yeni Detay
        </Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, hesap, daire ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><ListTree className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Virman fişi detayı bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium">Tarih</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Belge Tarihi</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Son Ödeme Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Daire</th>
                <th className="text-left px-3 py-2 font-medium">Hesap Tipi</th>
                <th className="text-left px-3 py-2 font-medium">Hesap</th>
                <th className="text-right px-3 py-2 font-medium">Borç</th>
                <th className="text-right px-3 py-2 font-medium">Alacak</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => router.push(`/finans/virman-fis-detaylari/${item.id}`)}>
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-xs">{tarih(item.tarih)}</td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground hidden md:table-cell">{tarih(item.belgeTarihi)}</td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground hidden md:table-cell">{tarih(item.sonOdemeTarihi)}</td>
                  <td className="px-3 py-2.5 text-xs">{item.unitDoorNumber ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs">{VirmanHesapTuruLabel[item.hesapTuru]}</td>
                  <td className="px-3 py-2.5">{item.hesapAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right">{item.borcTutari ? fmt(item.borcTutari) : '—'}</td>
                  <td className="px-3 py-2.5 text-right">{item.alacakTutari ? fmt(item.alacakTutari) : '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-end gap-2 text-sm">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Önceki</Button>
          <span className="text-muted-foreground">{page} / {totalPages}</span>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Sonraki</Button>
        </div>
      )}
    </div>
  )
}
