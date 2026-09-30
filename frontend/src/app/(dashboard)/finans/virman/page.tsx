'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { Plus, ArrowLeftRight, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { hesaplarArasiVirmanApi } from '@/lib/api/finans'
import type { VirmanListItem } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const tarih = (s: string) => new Date(s).toLocaleDateString('tr-TR')

export default function VirmanListPage() {
  const router = useRouter()
  const [items, setItems] = useState<VirmanListItem[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await hesaplarArasiVirmanApi.getAll(page, PAGE_SIZE, search || undefined)
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
        <h1 className="text-xl font-semibold">Hesaplar Arası Virman</h1>
        <Button size="sm" onClick={() => router.push('/finans/virman/yeni')}>
          <Plus className="h-4 w-4 mr-1" />Yeni Virman
        </Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, belge no, kişi ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><ArrowLeftRight className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Virman fişi bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">İşlem Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Tarih</th>
                <th className="text-right px-3 py-2 font-medium">Satır Sayısı</th>
                <th className="text-right px-3 py-2 font-medium">Borç</th>
                <th className="text-right px-3 py-2 font-medium">Alacak</th>
                <th className="text-right px-3 py-2 font-medium">Bakiye</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => router.push(`/finans/virman/${item.id}`)}>
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{tarih(item.islemTarihi)}</td>
                  <td className="px-3 py-2.5 text-xs">{tarih(item.tarih)}</td>
                  <td className="px-3 py-2.5 text-right">{item.satirSayisi}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(item.toplamBorc)}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(item.toplamAlacak)}</td>
                  <td className={`px-3 py-2.5 text-right font-medium ${item.bakiye !== 0 ? 'text-amber-600' : ''}`}>{fmt(item.bakiye)}</td>
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
