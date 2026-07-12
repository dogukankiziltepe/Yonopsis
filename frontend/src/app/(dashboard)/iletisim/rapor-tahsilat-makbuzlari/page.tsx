'use client'

import { useEffect, useState, useCallback } from 'react'
import { Receipt, Printer, Search, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { tahsilatMakbuzlariApi } from '@/lib/api/finans'
import type { TahsilatMakbuzu } from '@/types/finans'
import { OdemeTipi, OdemeTipiLabel } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 50
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const fmtDate = (s: string) => new Date(s).toLocaleDateString('tr-TR')

export default function RaporTahsilatMakbuzlariPage() {
  const [items, setItems] = useState<TahsilatMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [filterBaslangic, setFilterBaslangic] = useState('')
  const [filterBitis, setFilterBitis] = useState('')
  const [filterOdemeTipi, setFilterOdemeTipi] = useState('')

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await tahsilatMakbuzlariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => {
    const t = setTimeout(() => { setSearch(searchInput); setPage(1) }, 350)
    return () => clearTimeout(t)
  }, [searchInput])

  const filtered = items.filter(item => {
    if (filterBaslangic && item.islemTarihi < filterBaslangic) return false
    if (filterBitis && item.islemTarihi > filterBitis + 'T23:59:59') return false
    if (filterOdemeTipi && String(item.odemeTipi) !== filterOdemeTipi) return false
    return true
  })

  const toplam = filtered.reduce((s, i) => s + i.odemeTutari, 0)

  const odemeTipleri = (Object.values(OdemeTipi).filter(v => typeof v === 'number') as OdemeTipi[])
  const byMethod = odemeTipleri.map(v => ({
    tipi: v,
    label: OdemeTipiLabel[v],
    toplam: filtered.filter(i => i.odemeTipi === v).reduce((s, i) => s + i.odemeTutari, 0),
    adet: filtered.filter(i => i.odemeTipi === v).length,
  })).filter(x => x.adet > 0)

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Tahsilat Makbuzlari Raporu</h1>
        <Button size="sm" variant="outline" onClick={() => window.print()}>
          <Printer className="h-4 w-4 mr-1" />Yazdir
        </Button>
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Tahsilat Makbuzlari Raporu</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {byMethod.length > 0 && (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          {byMethod.map(m => (
            <div key={m.tipi} className="border rounded-lg p-3">
              <p className="text-xs text-muted-foreground">{m.label}</p>
              <p className="text-base font-semibold mt-0.5">{fmt(m.toplam)}</p>
              <p className="text-xs text-muted-foreground">{m.adet} makbuz</p>
            </div>
          ))}
        </div>
      )}

      <div className="flex flex-wrap items-center gap-2 print:hidden">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Borclu adi, evrak no..." value={searchInput}
            onChange={e => setSearchInput(e.target.value)} />
        </div>
        <Input type="date" className="w-36" value={filterBaslangic}
          onChange={e => { setFilterBaslangic(e.target.value); setPage(1) }} />
        <span className="text-muted-foreground text-sm">-</span>
        <Input type="date" className="w-36" value={filterBitis}
          onChange={e => { setFilterBitis(e.target.value); setPage(1) }} />
        <select value={filterOdemeTipi} onChange={e => { setFilterOdemeTipi(e.target.value); setPage(1) }}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="">Tum Odeme Tipleri</option>
          {odemeTipleri.map(v => (
            <option key={v} value={String(v)}>{OdemeTipiLabel[v]}</option>
          ))}
        </select>
        {(filterBaslangic || filterBitis || filterOdemeTipi || searchInput) && (
          <Button variant="ghost" size="sm" onClick={() => {
            setFilterBaslangic(''); setFilterBitis(''); setFilterOdemeTipi('')
            setSearchInput(''); setSearch('')
          }}>
            <X className="h-4 w-4 mr-1" />Temizle
          </Button>
        )}
        {total > 0 && <span className="text-sm text-muted-foreground ml-auto">{filtered.length} kayit</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yukleniyor...</div>
        ) : filtered.length === 0 ? (
          <div className="p-12 text-center">
            <Receipt className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">Tahsilat makbuzu bulunamadi.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Borclu</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Hesap</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Odeme Tipi</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filtered.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground whitespace-nowrap">{fmtDate(item.islemTarihi)}</td>
                  <td className="px-3 py-2.5">{item.borcluAdi ?? '-'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell">{item.kasaBankaAdi ?? '-'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{OdemeTipiLabel[item.odemeTipi]}</td>
                  <td className="px-3 py-2.5 text-right font-medium text-emerald-600">{fmt(item.odemeTutari)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot className="bg-muted/50 border-t font-medium">
              <tr>
                <td colSpan={5} className="px-3 py-2.5 text-sm">Toplam ({filtered.length} kayit)</td>
                <td className="px-3 py-2.5 text-right text-emerald-600">{fmt(toplam)}</td>
              </tr>
            </tfoot>
          </table>
        )}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-end gap-2 text-sm print:hidden">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Onceki</Button>
          <span className="text-muted-foreground">{page} / {totalPages}</span>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Sonraki</Button>
        </div>
      )}
    </div>
  )
}
