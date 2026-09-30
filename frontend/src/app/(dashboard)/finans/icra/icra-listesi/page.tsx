'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { Gavel, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { icraDosyalariApi } from '@/lib/api/icra'
import { IcraDurumuLabel } from '@/types/icra'
import type { IcraDosyasiListItem } from '@/types/icra'
import { excelIndir } from '@/components/finans/IcraEvrakTablosu'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const tarih = (s: string) => new Date(s).toLocaleDateString('tr-TR')

export default function IcraListesiPage() {
  const router = useRouter()
  const [items, setItems] = useState<IcraDosyasiListItem[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await icraDosyalariApi.getAll(page, PAGE_SIZE, search || undefined)
      setItems(res.data.items ?? [])
      setTotal(res.data.totalCount ?? 0)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const totalPages = Math.ceil(total / PAGE_SIZE)

  const indir = () => excelIndir(items.map(i => ({
    'İcra Tarihi': tarih(i.icraTarihi), 'Dosya Numarası': i.dosyaNo, 'Borçlu': i.borcluAdi,
    'Daire': `${i.blokAdi ? `${i.blokAdi} / ` : ''}${i.doorNumber}`, 'Avukat': i.avukatAdi ?? '',
    'Durumu': IcraDurumuLabel[i.durum], 'Dosya Tutarı': i.dosyaTutari, 'Bakiye': i.bakiye,
  })), 'İcra Listesi', 'icra-listesi.xlsx')

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">İcra Listesi</h1>
        <Button size="sm" variant="outline" onClick={indir} disabled={items.length === 0}>Excel</Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Dosya no, borçlu, daire, avukat ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Gavel className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">İcra dosyası bulunamadı.</p>
            <p className="text-xs text-muted-foreground mt-1">İcra dosyaları Takip Listesi&apos;ndeki kayıtlardan &quot;İcraya Ver&quot; ile açılır.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">İcra Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Dosya Numarası</th>
                <th className="text-left px-3 py-2 font-medium">Borçlu</th>
                <th className="text-left px-3 py-2 font-medium">Daire</th>
                <th className="text-left px-3 py-2 font-medium">Avukat</th>
                <th className="text-left px-3 py-2 font-medium">Durumu</th>
                <th className="text-right px-3 py-2 font-medium">Dosya Tutarı</th>
                <th className="text-right px-3 py-2 font-medium">Bakiye</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(i => (
                <tr key={i.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => router.push(`/finans/icra/icra-listesi/${i.id}`)}>
                  <td className="px-3 py-2.5 text-xs">{tarih(i.icraTarihi)}</td>
                  <td className="px-3 py-2.5 font-mono text-xs">{i.dosyaNo}</td>
                  <td className="px-3 py-2.5">{i.borcluAdi}</td>
                  <td className="px-3 py-2.5 text-xs">{i.blokAdi ? `${i.blokAdi} / ` : ''}{i.doorNumber}</td>
                  <td className="px-3 py-2.5">{i.avukatAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs">{IcraDurumuLabel[i.durum]}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(i.dosyaTutari)}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(i.bakiye)}</td>
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
