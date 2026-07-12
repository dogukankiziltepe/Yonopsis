'use client'

import { useEffect, useState, useCallback } from 'react'
import { FileText, Printer, Search, X, ChevronDown, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi } from '@/lib/api/finans'
import type { BorcMakbuzu } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 500
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const fmtDate = (s: string) => new Date(s).toLocaleDateString('tr-TR')

interface PersonGroup {
  ad: string
  borclar: BorcMakbuzu[]
  toplamBorc: number
  toplamOdenen: number
  toplamKalan: number
}

export default function DetayliBorcListesiPage() {
  const [allItems, setAllItems] = useState<BorcMakbuzu[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [filterDurum, setFilterDurum] = useState<'all' | 'bekleyen' | 'odenmis'>('all')
  const [filterDonem, setFilterDonem] = useState('')
  const [expanded, setExpanded] = useState<Set<string>>(new Set())

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await borcMakbuzlariApi.getAll(1, PAGE_SIZE)
      setAllItems(res.data.items ?? [])
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const filtered = allItems.filter(item => {
    if (search) {
      const q = search.toLowerCase()
      const ad = (item.borcluAdi ?? item.unitDoorNumber ?? '').toLowerCase()
      if (!ad.includes(q) && !item.evrakNo.toLowerCase().includes(q)) return false
    }
    if (filterDurum === 'bekleyen' && item.kalanTutar <= 0) return false
    if (filterDurum === 'odenmis' && item.kalanTutar > 0) return false
    if (filterDonem && item.donem !== filterDonem) return false
    return true
  })

  const groups = new Map<string, PersonGroup>()
  for (const b of filtered) {
    const key = b.borcluAdi ?? b.unitDoorNumber ?? '(Bilinmiyor)'
    const g = groups.get(key) ?? { ad: key, borclar: [], toplamBorc: 0, toplamOdenen: 0, toplamKalan: 0 }
    g.borclar.push(b)
    g.toplamBorc += b.tutar
    g.toplamOdenen += b.odenenTutar
    g.toplamKalan += b.kalanTutar
    groups.set(key, g)
  }
  const groupList = Array.from(groups.values()).sort((a, b) => b.toplamKalan - a.toplamKalan)
  const donemler = Array.from(new Set(allItems.map(i => i.donem).filter(Boolean))).sort().reverse() as string[]

  const toggleExpand = (ad: string) => {
    setExpanded(prev => {
      const next = new Set(prev)
      next.has(ad) ? next.delete(ad) : next.add(ad)
      return next
    })
  }

  const totalBorc = groupList.reduce((s, g) => s + g.toplamBorc, 0)
  const totalKalan = groupList.reduce((s, g) => s + g.toplamKalan, 0)

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Detaylı Borç Listesi</h1>
        <div className="flex gap-2">
          <Button size="sm" variant="ghost" onClick={() => setExpanded(new Set(groupList.map(g => g.ad)))}>Tümünü Aç</Button>
          <Button size="sm" variant="ghost" onClick={() => setExpanded(new Set())}>Kapat</Button>
          <Button size="sm" variant="outline" onClick={() => window.print()}>
            <Printer className="h-4 w-4 mr-1" />Yazdır
          </Button>
        </div>
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Detaylı Borç Listesi</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      <div className="flex flex-wrap items-center gap-2 print:hidden">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Borçlu adı, evrak no..." value={search}
            onChange={e => setSearch(e.target.value)} />
        </div>
        <select value={filterDonem} onChange={e => setFilterDonem(e.target.value)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="">Tüm Dönemler</option>
          {donemler.map(d => <option key={d} value={d}>{d}</option>)}
        </select>
        <select value={filterDurum} onChange={e => setFilterDurum(e.target.value as typeof filterDurum)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="all">Tümü</option>
          <option value="bekleyen">Bekleyen</option>
          <option value="odenmis">Ödenmiş</option>
        </select>
        {(search || filterDurum !== 'all' || filterDonem) && (
          <Button variant="ghost" size="sm" onClick={() => { setSearch(''); setFilterDurum('all'); setFilterDonem('') }}>
            <X className="h-4 w-4 mr-1" />Temizle
          </Button>
        )}
        <span className="text-sm text-muted-foreground ml-auto">{groupList.length} kişi · {filtered.length} kayıt</span>
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-y-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : groupList.length === 0 ? (
          <div className="p-12 text-center">
            <FileText className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">Borç kaydı bulunamadı.</p>
          </div>
        ) : (
          <div>
            {groupList.map(group => (
              <div key={group.ad} className="border-b last:border-b-0">
                <button
                  className="w-full flex items-center gap-3 px-3 py-2.5 hover:bg-muted/30 transition-colors text-left"
                  onClick={() => toggleExpand(group.ad)}
                >
                  {expanded.has(group.ad)
                    ? <ChevronDown className="h-4 w-4 text-muted-foreground shrink-0" />
                    : <ChevronRight className="h-4 w-4 text-muted-foreground shrink-0" />}
                  <span className="font-medium flex-1">{group.ad}</span>
                  <span className="text-xs text-muted-foreground">{group.borclar.length} borç</span>
                  <span className="text-sm hidden md:inline text-muted-foreground ml-4">{fmt(group.toplamBorc)}</span>
                  <span className={`text-sm font-semibold ml-4 ${group.toplamKalan > 0 ? 'text-red-600' : 'text-emerald-600'}`}>
                    {fmt(group.toplamKalan)}
                  </span>
                  <Badge variant={group.toplamKalan > 0 ? 'default' : 'secondary'} className="text-xs ml-2">
                    {group.toplamKalan > 0 ? 'Bekliyor' : 'Kapalı'}
                  </Badge>
                </button>
                {expanded.has(group.ad) && (
                  <div className="ml-7 border-t bg-muted/20">
                    <table className="w-full text-xs">
                      <thead className="bg-muted/50">
                        <tr>
                          <th className="text-left px-3 py-1.5 font-medium">Evrak No</th>
                          <th className="text-left px-3 py-1.5 font-medium hidden sm:table-cell">Tarih</th>
                          <th className="text-left px-3 py-1.5 font-medium hidden md:table-cell">Dönem</th>
                          <th className="text-left px-3 py-1.5 font-medium hidden md:table-cell">Gelir Türü</th>
                          <th className="text-right px-3 py-1.5 font-medium">Tutar</th>
                          <th className="text-right px-3 py-1.5 font-medium hidden sm:table-cell">Ödenen</th>
                          <th className="text-right px-3 py-1.5 font-medium">Kalan</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-muted">
                        {group.borclar.map(b => (
                          <tr key={b.id} className="hover:bg-muted/30">
                            <td className="px-3 py-2 font-mono">{b.evrakNo}</td>
                            <td className="px-3 py-2 text-muted-foreground hidden sm:table-cell">{fmtDate(b.islemTarihi)}</td>
                            <td className="px-3 py-2 text-muted-foreground hidden md:table-cell">{b.donem ?? '—'}</td>
                            <td className="px-3 py-2 text-muted-foreground hidden md:table-cell">{b.gelirTanimiAdi ?? '—'}</td>
                            <td className="px-3 py-2 text-right">{fmt(b.tutar)}</td>
                            <td className="px-3 py-2 text-right text-emerald-600 hidden sm:table-cell">{fmt(b.odenenTutar)}</td>
                            <td className={`px-3 py-2 text-right font-medium ${b.kalanTutar > 0 ? 'text-red-600' : 'text-muted-foreground'}`}>
                              {fmt(b.kalanTutar)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                      <tfoot className="bg-muted/40 font-medium">
                        <tr>
                          <td colSpan={4} className="px-3 py-1.5">Alt Toplam</td>
                          <td className="px-3 py-1.5 text-right">{fmt(group.toplamBorc)}</td>
                          <td className="px-3 py-1.5 text-right text-emerald-600 hidden sm:table-cell">{fmt(group.toplamOdenen)}</td>
                          <td className={`px-3 py-1.5 text-right ${group.toplamKalan > 0 ? 'text-red-600' : 'text-muted-foreground'}`}>
                            {fmt(group.toplamKalan)}
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                )}
              </div>
            ))}
            <div className="flex items-center justify-between px-3 py-2.5 bg-muted/50 border-t font-medium text-sm">
              <span>Genel Toplam ({groupList.length} kişi · {filtered.length} borç)</span>
              <div className="flex gap-6">
                <span className="text-muted-foreground hidden md:inline">{fmt(totalBorc)}</span>
                <span className={totalKalan > 0 ? 'text-red-600' : 'text-emerald-600'}>{fmt(totalKalan)}</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
