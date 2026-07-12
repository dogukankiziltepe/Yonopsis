'use client'

import { useEffect, useState } from 'react'
import { Award, Printer, Search, TrendingDown } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi } from '@/lib/api/finans'
import type { BorcMakbuzu } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

interface SkorEntry {
  ad: string
  toplamBorc: number
  toplamOdenen: number
  toplamKalan: number
  borcSayisi: number
  odenmisCount: number
  skor: number        // 0-100, higher = better payer
}

function skorLabel(s: number): { label: string; variant: 'default' | 'secondary' | 'destructive' | 'outline' } {
  if (s >= 90) return { label: 'Mükemmel', variant: 'secondary' }
  if (s >= 70) return { label: 'İyi', variant: 'outline' }
  if (s >= 40) return { label: 'Orta', variant: 'outline' }
  return { label: 'Riskli', variant: 'destructive' }
}

function SkorBar({ skor }: { skor: number }) {
  const color = skor >= 90 ? 'bg-emerald-500' : skor >= 70 ? 'bg-blue-500' : skor >= 40 ? 'bg-amber-500' : 'bg-red-500'
  return (
    <div className="flex items-center gap-2">
      <div className="w-24 h-2 rounded-full bg-muted overflow-hidden">
        <div className={`h-full rounded-full ${color}`} style={{ width: `${skor}%` }} />
      </div>
      <span className="text-xs font-medium w-7">{Math.round(skor)}</span>
    </div>
  )
}

export default function SiteOdemeSkoruPage() {
  const [entries, setEntries] = useState<SkorEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [filterSkor, setFilterSkor] = useState<'all' | 'mukemmel' | 'iyi' | 'orta' | 'riskli'>('all')
  const [sortBy, setSortBy] = useState<'skor' | 'kalan' | 'ad'>('skor')

  useEffect(() => {
    const fetchAll = async () => {
      setLoading(true)
      try {
        const res = await borcMakbuzlariApi.getAll(1, 1000)
        const borclar: BorcMakbuzu[] = res.data.items ?? []

        const map = new Map<string, SkorEntry>()
        for (const b of borclar) {
          const key = b.borcluAdi ?? b.unitDoorNumber ?? '(Bilinmiyor)'
          const e = map.get(key) ?? {
            ad: key,
            toplamBorc: 0, toplamOdenen: 0, toplamKalan: 0,
            borcSayisi: 0, odenmisCount: 0, skor: 0,
          }
          e.toplamBorc += b.tutar
          e.toplamOdenen += b.odenenTutar
          e.toplamKalan += b.kalanTutar
          e.borcSayisi++
          if (b.kalanTutar <= 0) e.odenmisCount++
          map.set(key, e)
        }

        // Calculate score: weighted blend of payment rate by count and by amount
        for (const e of map.values()) {
          const countScore = e.borcSayisi > 0 ? (e.odenmisCount / e.borcSayisi) * 100 : 0
          const amountScore = e.toplamBorc > 0 ? (e.toplamOdenen / e.toplamBorc) * 100 : 100
          e.skor = (countScore * 0.4 + amountScore * 0.6)
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
      if (filterSkor === 'mukemmel' && e.skor < 90) return false
      if (filterSkor === 'iyi' && (e.skor < 70 || e.skor >= 90)) return false
      if (filterSkor === 'orta' && (e.skor < 40 || e.skor >= 70)) return false
      if (filterSkor === 'riskli' && e.skor >= 40) return false
      return true
    })
    .sort((a, b) => {
      if (sortBy === 'ad') return a.ad.localeCompare(b.ad, 'tr')
      if (sortBy === 'kalan') return b.toplamKalan - a.toplamKalan
      return a.skor - b.skor  // worst payers first
    })

  const avgSkor = entries.length > 0 ? entries.reduce((s, e) => s + e.skor, 0) / entries.length : 0
  const riskliCount = entries.filter(e => e.skor < 40).length
  const mukemmelCount = entries.filter(e => e.skor >= 90).length

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Site Ödeme Skoru</h1>
        <Button size="sm" variant="outline" onClick={() => window.print()}>
          <Printer className="h-4 w-4 mr-1" />Yazdır
        </Button>
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Site Ödeme Skoru</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {/* Summary */}
      {!loading && entries.length > 0 && (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Ortalama Skor</p>
            <p className="font-semibold text-lg">{Math.round(avgSkor)}<span className="text-xs text-muted-foreground font-normal">/100</span></p>
          </div>
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Toplam Kişi</p>
            <p className="font-semibold text-lg">{entries.length}</p>
          </div>
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Mükemmel Ödeyici</p>
            <p className="font-semibold text-lg text-emerald-600">{mukemmelCount}</p>
          </div>
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Riskli</p>
            <p className="font-semibold text-lg text-red-600">{riskliCount}</p>
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="flex flex-wrap items-center gap-2 print:hidden">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Kişi adı..." value={search}
            onChange={e => setSearch(e.target.value)} />
        </div>
        <select value={filterSkor} onChange={e => setFilterSkor(e.target.value as typeof filterSkor)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="all">Tüm Skorlar</option>
          <option value="mukemmel">Mükemmel (90+)</option>
          <option value="iyi">İyi (70-89)</option>
          <option value="orta">Orta (40-69)</option>
          <option value="riskli">Riskli (&lt;40)</option>
        </select>
        <select value={sortBy} onChange={e => setSortBy(e.target.value as typeof sortBy)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="skor">Skora göre (en kötü)</option>
          <option value="kalan">Kalan borça göre</option>
          <option value="ad">Ada göre</option>
        </select>
        {filtered.length > 0 && (
          <span className="text-sm text-muted-foreground ml-auto">{filtered.length} kişi</span>
        )}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : filtered.length === 0 ? (
          <div className="p-12 text-center">
            <Award className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">Borç kaydı bulunamadı.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium w-8">#</th>
                <th className="text-left px-3 py-2 font-medium">Kişi / Daire</th>
                <th className="text-left px-3 py-2 font-medium">Ödeme Skoru</th>
                <th className="text-center px-3 py-2 font-medium">Durum</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Toplam Borç</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Ödenen</th>
                <th className="text-right px-3 py-2 font-medium">Kalan</th>
                <th className="text-right px-3 py-2 font-medium hidden lg:table-cell">Borç / Ödenen</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filtered.map((e, idx) => {
                const { label, variant } = skorLabel(e.skor)
                return (
                  <tr key={e.ad} className="hover:bg-muted/30">
                    <td className="px-3 py-2.5 text-xs text-muted-foreground">{idx + 1}</td>
                    <td className="px-3 py-2.5 font-medium">{e.ad}</td>
                    <td className="px-3 py-2.5"><SkorBar skor={e.skor} /></td>
                    <td className="px-3 py-2.5 text-center">
                      <Badge variant={variant} className="text-xs">{label}</Badge>
                    </td>
                    <td className="px-3 py-2.5 text-right hidden md:table-cell text-muted-foreground">{fmt(e.toplamBorc)}</td>
                    <td className="px-3 py-2.5 text-right hidden md:table-cell text-emerald-600">{fmt(e.toplamOdenen)}</td>
                    <td className={`px-3 py-2.5 text-right font-medium ${e.toplamKalan > 0 ? 'text-red-600' : 'text-muted-foreground'}`}>
                      {fmt(e.toplamKalan)}
                    </td>
                    <td className="px-3 py-2.5 text-right hidden lg:table-cell text-xs text-muted-foreground">
                      {e.odenmisCount}/{e.borcSayisi}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        )}
      </div>

      {/* Score legend */}
      <div className="flex flex-wrap gap-4 text-xs text-muted-foreground print:hidden">
        <div className="flex items-center gap-1.5"><div className="w-3 h-1.5 rounded-full bg-emerald-500" /><span>Mükemmel (90–100)</span></div>
        <div className="flex items-center gap-1.5"><div className="w-3 h-1.5 rounded-full bg-blue-500" /><span>İyi (70–89)</span></div>
        <div className="flex items-center gap-1.5"><div className="w-3 h-1.5 rounded-full bg-amber-500" /><span>Orta (40–69)</span></div>
        <div className="flex items-center gap-1.5"><div className="w-3 h-1.5 rounded-full bg-red-500" /><span>Riskli (0–39)</span></div>
      </div>
    </div>
  )
}
