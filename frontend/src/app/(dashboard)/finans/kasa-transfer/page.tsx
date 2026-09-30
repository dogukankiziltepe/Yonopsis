'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { Plus, ArrowLeftRight, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { kasaTransferleriApi } from '@/lib/api/finans'
import type { KasaTransfer } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const tarih = (s: string) => new Date(s).toLocaleDateString('tr-TR')

export default function KasaTransferListPage() {
  const router = useRouter()
  const [items, setItems] = useState<KasaTransfer[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await kasaTransferleriApi.getAll(page, PAGE_SIZE, search || undefined)
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
        <h1 className="text-xl font-semibold">Kasa Transfer Fişi</h1>
        <Button size="sm" onClick={() => router.push('/finans/kasa-transfer/yeni')}>
          <Plus className="h-4 w-4 mr-1" />Yeni Transfer
        </Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, belge no, açıklama ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><ArrowLeftRight className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Kasa transfer fişi bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">İşlem Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Çıkış Kasası</th>
                <th className="text-left px-3 py-2 font-medium">Giriş Kasası</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Açıklama</th>
                <th className="text-right px-3 py-2 font-medium">Transfer Tutarı</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => router.push(`/finans/kasa-transfer/${item.id}`)}>
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{tarih(item.islemTarihi)}</td>
                  <td className="px-3 py-2.5 text-xs">{tarih(item.tarih)}</td>
                  <td className="px-3 py-2.5">{item.cikisKasaBankaAdi ?? '—'}</td>
                  <td className="px-3 py-2.5">{item.girisKasaBankaAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell max-w-xs truncate">{item.aciklama ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.tutar)}</td>
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
