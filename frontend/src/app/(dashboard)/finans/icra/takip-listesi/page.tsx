'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { ClipboardList, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { icraTakipleriApi } from '@/lib/api/icra'
import { TakipDurumu, TakipDurumuLabel } from '@/types/icra'
import type { TakipListItem } from '@/types/icra'
import { excelIndir } from '@/components/finans/IcraEvrakTablosu'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const tarih = (s: string) => new Date(s).toLocaleDateString('tr-TR')

const takipDurumStil: Record<TakipDurumu, string> = {
  [TakipDurumu.Takipte]: 'bg-blue-100 text-blue-700',
  [TakipDurumu.IcrayaVerilecek]: 'bg-amber-100 text-amber-700',
  [TakipDurumu.IcrayaVerildi]: 'bg-red-100 text-red-700',
  [TakipDurumu.Odendi]: 'bg-green-100 text-green-700',
  [TakipDurumu.IptalEdildi]: 'bg-muted text-muted-foreground',
}

export default function TakipListesiPage() {
  const router = useRouter()
  const [items, setItems] = useState<TakipListItem[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [durum, setDurum] = useState<string>('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await icraTakipleriApi.getAll(page, PAGE_SIZE, search || undefined, durum === '' ? undefined : Number(durum) as TakipDurumu)
      setItems(res.data.items ?? [])
      setTotal(res.data.totalCount ?? 0)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search, durum])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const totalPages = Math.ceil(total / PAGE_SIZE)

  const indir = () => excelIndir(items.map(i => ({
    'Takip Tarihi': tarih(i.takipTarihi), 'Borçlu': i.borcluAdi, 'Daire': `${i.blokAdi ? `${i.blokAdi} / ` : ''}${i.doorNumber}`,
    'Takip Başlangıç Tutarı': i.baslangicTutari, 'Mevcut Tutar': i.mevcutTutar, 'Durumu': TakipDurumuLabel[i.durum],
    'Telefon': i.telefon ?? '', 'E-Posta': i.eposta ?? '', 'Adres': i.adres ?? '',
  })), 'Takip Listesi', 'takip-listesi.xlsx')

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Takip Listesi</h1>
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={indir} disabled={items.length === 0}>Excel</Button>
          <Button size="sm" onClick={() => router.push('/finans/icra/takibe-gonder')}>Takibe Gönder</Button>
        </div>
      </div>

      <div className="flex items-center gap-2 flex-wrap">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Borçlu, daire ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        <select className="border rounded-md px-3 py-2 text-sm bg-background" value={durum} onChange={e => { setDurum(e.target.value); setPage(1) }}>
          <option value="">Tüm durumlar</option>
          {Object.entries(TakipDurumuLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
        </select>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><ClipboardList className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Takip kaydı bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Takip Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Borçlu</th>
                <th className="text-left px-3 py-2 font-medium">Daire</th>
                <th className="text-right px-3 py-2 font-medium">Takip Başlangıç Tutarı</th>
                <th className="text-right px-3 py-2 font-medium">Mevcut Tutar</th>
                <th className="text-left px-3 py-2 font-medium">Durumu</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Telefon</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">E-Posta</th>
                <th className="text-left px-3 py-2 font-medium hidden xl:table-cell">Adres</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(i => (
                <tr key={i.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => router.push(`/finans/icra/takip-listesi/${i.id}`)}>
                  <td className="px-3 py-2.5 text-xs">{tarih(i.takipTarihi)}</td>
                  <td className="px-3 py-2.5">{i.borcluAdi}</td>
                  <td className="px-3 py-2.5 text-xs">{i.blokAdi ? `${i.blokAdi} / ` : ''}{i.doorNumber}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(i.baslangicTutari)}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(i.mevcutTutar)}</td>
                  <td className="px-3 py-2.5"><span className={`px-2 py-0.5 rounded text-xs ${takipDurumStil[i.durum]}`}>{TakipDurumuLabel[i.durum]}</span></td>
                  <td className="px-3 py-2.5 text-xs hidden lg:table-cell">{i.telefon ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs hidden lg:table-cell">{i.eposta ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs hidden xl:table-cell max-w-[240px] truncate" title={i.adres}>{i.adres ?? '—'}</td>
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
