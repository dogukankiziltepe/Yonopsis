'use client'

import { useEffect, useState } from 'react'
import { Users, Printer, Search, TrendingDown, TrendingUp, Minus } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi, tahsilatMakbuzlariApi } from '@/lib/api/finans'
import type { BorcMakbuzu, TahsilatMakbuzu } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

interface CariEntry {
  ad: string
  toplamBorc: number
  toplamOdenen: number
  toplamKalan: number
  borcSayisi: number
  tahsilatSayisi: number
}

export default function CariHesapListesiPage() {
  const [entries, setEntries] = useState<CariEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [filterDurum, setFilterDurum] = useState<'all' | 'borclu' | 'alacakli' | 'sifir'>('all')
  const [sortBy, setSortBy] = useState<'ad' | 'kalan'>('kalan')

  useEffect(() => {
    const fetchAll = async () => {
      setLoading(true)
      try {
        const [borcRes, tahsilatRes] = await Promise.all([
          borcMakbuzlariApi.getAll(1, 1000),
          tahsilatMakbuzlariApi.getAll(1, 1000),
        ])
        const borclar: BorcMakbuzu[] = borcRes.data.items ?? []
        const tahsilat: TahsilatMakbuzu[] = tahsilatRes.data.items ?? []

        const map = new Map<string, CariEntry>()

        for (const b of borclar) {
          const key = b.borcluAdi ?? b.unitDoorNumber ?? '(Bilinmiyor)'
          const e = map.get(key) ?? { ad: key, toplamBorc: 0, toplamOdenen: 0, toplamKalan: 0, borcSayisi: 0, tahsilatSayisi: 0 }
          e.toplamBorc += b.tutar
          e.toplamOdenen += b.odenenTutar
          e.toplamKalan += b.kalanTutar
          e.borcSayisi++
          map.set(key, e)
        }

        for (const t of tahsilat) {
          const key = t.borcluAdi ?? '(Bilinmiyor)'
          if (!map.has(key)) {
            map.set(key, { ad: key, toplamBorc: 0, toplamOdenen: t.odemeTutari, toplamKalan: -t.odemeTutari, borcSayisi: 0, tahsilatSayisi: 1 })
          } else {
            const e = map.get(key)!
            e.tahsilatSayisi++
            map.set(key, e)
          }
        }

        setEntries(Array.from(map.values()))
      } catch (e) { showApiError(e) }
      finally { setLoading(false) }
    }
    fetchAll()
  }, [])

  const filtered = entries
    .filter(e => {
      if (search && !e.ad.toLowerCase().includes(search.toLowerCase())) return false
      if (filterDurum === 'borclu' && e.toplamKalan <= 0) return false
      if (filterDurum === 'alacakli' && e.toplamKalan >= 0) return false
      if (filterDurum === 'sifir' && e.toplamKalan !== 0) return false
      return true
    })
    .sort((a, b) => sortBy === 'ad' ? a.ad.localeCompare(b.ad, 'tr') : b.toplamKalan - a.toplamKalan)

  const totalBorc = filtered.reduce((s, e) => s + e.toplamBorc, 0)
  const totalKalan = filtered.reduce((s, e) => s + e.toplamKalan, 0)

  const borcluCount = entries.filter(e => e.toplamKalan > 0).length
  const alacakliCount = entries.filter(e => e.toplamKalan < 0).length
  const kapaliCount = entries.filter(e => e.toplamKalan === 0).length

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Cari Hesap Listesi</h1>
        <Button size="sm" variant="outline" onClick={() => window.print()}>
          <Printer className="h-4 w-4 mr-1" />Yazdir
        </Button>
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Cari Hesap Listesi</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {!loading && (
        <div className="grid grid-cols-3 gap-3">
          <div className="border rounded-lg p-3 flex items-center gap-3">
            <TrendingDown className="h-5 w-5 text-red-500 shrink-0" />
            <div>
              <p className="text-xs text-muted-foreground">Borclu</p>
              <p className="font-semibold">{borcluCount} kisi</p>
            </div>
          </div>
          <div className="border rounded-lg p-3 flex items-center gap-3">
            <TrendingUp className="h-5 w-5 text-emerald-500 shrink-0" />
            <div>
              <p className="text-xs text-muted-foreground">Alacakli</p>
              <p className="font-semibold">{alacakliCount} kisi</p>
            </div>
          </div>
          <div className="border rounded-lg p-3 flex items-center gap-3">
            <Minus className="h-5 w-5 text-muted-foreground shrink-0" />
            <div>
              <p className="text-xs text-muted-foreground">Kapali</p>
              <p className="font-semibold">{kapaliCount} kisi</p>
            </div>
          </div>
        </div>
      )}

      <div className="flex flex-wrap items-center gap-2 print:hidden">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Cari adi..." value={search}
            onChange={e => setSearch(e.target.value)} />
        </div>
        <select value={filterDurum} onChange={e => setFilterDurum(e.target.value as typeof filterDurum)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="all">Tumu</option>
          <option value="borclu">Borclu</option>
          <option value="alacakli">Alacakli</option>
          <option value="sifir">Sifir Bakiye</option>
        </select>
        <select value={sortBy} onChange={e => setSortBy(e.target.value as 'ad' | 'kalan')}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="kalan">Bakiyeye gore sirala</option>
          <option value="ad">Ada gore sirala</option>
        </select>
        {filtered.length > 0 && (
          <span className="text-sm text-muted-foreground ml-auto">{filtered.length} cari</span>
        )}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yukleniyor...</div>
        ) : filtered.length === 0 ? (
          <div className="p-12 text-center">
            <Users className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">Cari bulunamadi.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Cari Adi</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Borc</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Odenen</th>
                <th className="text-right px-3 py-2 font-medium">Bakiye</th>
                <th className="text-center px-3 py-2 font-medium">Durum</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filtered.map((e, i) => (
                <tr key={i} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-medium">{e.ad}</td>
                  <td className="px-3 py-2.5 text-right text-muted-foreground hidden md:table-cell">{fmt(e.toplamBorc)}</td>
                  <td className="px-3 py-2.5 text-right text-emerald-600 hidden md:table-cell">{fmt(e.toplamOdenen)}</td>
                  <td className={`px-3 py-2.5 text-right font-semibold ${e.toplamKalan > 0 ? 'text-red-600' : e.toplamKalan < 0 ? 'text-emerald-600' : 'text-muted-foreground'}`}>
                    {fmt(e.toplamKalan)}
                  </td>
                  <td className="px-3 py-2.5 text-center">
                    <Badge variant={e.toplamKalan > 0 ? 'default' : 'secondary'} className="text-xs">
                      {e.toplamKalan > 0 ? 'Borclu' : e.toplamKalan < 0 ? 'Alacakli' : 'Kapali'}
                    </Badge>
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot className="bg-muted/50 border-t font-medium">
              <tr>
                <td className="px-3 py-2.5 text-sm">Toplam ({filtered.length} cari)</td>
                <td className="px-3 py-2.5 text-right hidden md:table-cell">{fmt(totalBorc)}</td>
                <td className="hidden md:table-cell" />
                <td className="px-3 py-2.5 text-right text-primary">{fmt(totalKalan)}</td>
                <td />
              </tr>
            </tfoot>
          </table>
        )}
      </div>
    </div>
  )
}
