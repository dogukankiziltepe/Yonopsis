'use client'

import { useEffect, useState } from 'react'
import { BarChart3, Printer, TrendingUp, TrendingDown, Scale } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { gelirFaturalariApi, giderFaturalariApi } from '@/lib/api/finans'
import type { Fatura } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const fmtShort = (n: number) => new Intl.NumberFormat('tr-TR', { notation: 'compact', maximumFractionDigits: 1 }).format(n)

const MONTHS = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara']

interface MonthData {
  key: string   // YYYY-MM
  label: string
  gelir: number
  gider: number
  net: number
}

export default function GelirGiderRaporuPage() {
  const [gelirler, setGelirler] = useState<Fatura[]>([])
  const [giderler, setGiderler] = useState<Fatura[]>([])
  const [loading, setLoading] = useState(true)
  const [filterYil, setFilterYil] = useState(String(new Date().getFullYear()))
  const [viewMode, setViewMode] = useState<'chart' | 'table'>('chart')

  useEffect(() => {
    const fetchAll = async () => {
      setLoading(true)
      try {
        const [gRes, gdRes] = await Promise.all([
          gelirFaturalariApi.getAll(1, 1000),
          giderFaturalariApi.getAll(1, 1000),
        ])
        setGelirler(gRes.data.items ?? [])
        setGiderler(gdRes.data.items ?? [])
      } catch (e) { showApiError(e) }
      finally { setLoading(false) }
    }
    fetchAll()
  }, [])

  const years = Array.from(new Set([
    ...gelirler.map(f => f.faturaTarihi.slice(0, 4)),
    ...giderler.map(f => f.faturaTarihi.slice(0, 4)),
  ])).sort().reverse()

  // Build monthly data for selected year
  const monthData: MonthData[] = Array.from({ length: 12 }, (_, i) => {
    const month = String(i + 1).padStart(2, '0')
    const key = `${filterYil}-${month}`
    const gelir = gelirler
      .filter(f => f.faturaTarihi.startsWith(key))
      .reduce((s, f) => s + f.toplamTutar, 0)
    const gider = giderler
      .filter(f => f.faturaTarihi.startsWith(key))
      .reduce((s, f) => s + f.toplamTutar, 0)
    return { key, label: MONTHS[i], gelir, gider, net: gelir - gider }
  })

  const totalGelir = monthData.reduce((s, m) => s + m.gelir, 0)
  const totalGider = monthData.reduce((s, m) => s + m.gider, 0)
  const totalNet = totalGelir - totalGider

  const maxVal = Math.max(...monthData.map(m => Math.max(m.gelir, m.gider)), 1)

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Gelir-Gider Raporu</h1>
        <Button size="sm" variant="outline" onClick={() => window.print()}>
          <Printer className="h-4 w-4 mr-1" />Yazdır
        </Button>
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Gelir-Gider Raporu — {filterYil}</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {/* Controls */}
      <div className="flex items-center gap-2 print:hidden">
        <select value={filterYil} onChange={e => setFilterYil(e.target.value)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          {years.length === 0
            ? <option value={filterYil}>{filterYil}</option>
            : years.map(y => <option key={y} value={y}>{y}</option>)}
        </select>
        <div className="flex rounded-md border overflow-hidden">
          <button
            className={`px-3 py-1.5 text-sm transition-colors ${viewMode === 'chart' ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'}`}
            onClick={() => setViewMode('chart')}
          >
            <BarChart3 className="h-4 w-4" />
          </button>
          <button
            className={`px-3 py-1.5 text-sm transition-colors ${viewMode === 'table' ? 'bg-primary text-primary-foreground' : 'hover:bg-muted'}`}
            onClick={() => setViewMode('table')}
          >
            Tablo
          </button>
        </div>
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-3 gap-3">
        <div className="border rounded-lg p-3 flex items-center gap-3">
          <TrendingUp className="h-5 w-5 text-emerald-500 shrink-0" />
          <div>
            <p className="text-xs text-muted-foreground">Toplam Gelir</p>
            <p className="font-semibold text-emerald-600">{loading ? '—' : fmt(totalGelir)}</p>
          </div>
        </div>
        <div className="border rounded-lg p-3 flex items-center gap-3">
          <TrendingDown className="h-5 w-5 text-red-500 shrink-0" />
          <div>
            <p className="text-xs text-muted-foreground">Toplam Gider</p>
            <p className="font-semibold text-red-600">{loading ? '—' : fmt(totalGider)}</p>
          </div>
        </div>
        <div className="border rounded-lg p-3 flex items-center gap-3">
          <Scale className="h-5 w-5 text-primary shrink-0" />
          <div>
            <p className="text-xs text-muted-foreground">Net</p>
            <p className={`font-semibold ${totalNet >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>
              {loading ? '—' : fmt(totalNet)}
            </p>
          </div>
        </div>
      </div>

      {loading ? (
        <div className="border rounded-lg p-8 text-center text-sm text-muted-foreground flex-1">Yükleniyor...</div>
      ) : viewMode === 'chart' ? (
        /* Bar chart */
        <div className="border rounded-lg p-4 flex-1">
          <div className="flex items-end gap-1 h-48 md:h-64">
            {monthData.map(m => {
              const gelirH = maxVal > 0 ? (m.gelir / maxVal) * 100 : 0
              const giderH = maxVal > 0 ? (m.gider / maxVal) * 100 : 0
              return (
                <div key={m.key} className="flex-1 flex flex-col items-center gap-0.5 group">
                  <div className="flex items-end gap-0.5 w-full" style={{ height: '100%' }}>
                    <div className="flex-1 bg-emerald-500/80 rounded-t-sm transition-all hover:bg-emerald-500"
                      style={{ height: `${gelirH}%` }}
                      title={`Gelir: ${fmt(m.gelir)}`}
                    />
                    <div className="flex-1 bg-red-400/80 rounded-t-sm transition-all hover:bg-red-500"
                      style={{ height: `${giderH}%` }}
                      title={`Gider: ${fmt(m.gider)}`}
                    />
                  </div>
                  <span className="text-xs text-muted-foreground">{m.label}</span>
                </div>
              )
            })}
          </div>
          <div className="flex items-center gap-4 mt-3 justify-center">
            <div className="flex items-center gap-1.5">
              <div className="w-3 h-3 rounded bg-emerald-500/80" />
              <span className="text-xs text-muted-foreground">Gelir</span>
            </div>
            <div className="flex items-center gap-1.5">
              <div className="w-3 h-3 rounded bg-red-400/80" />
              <span className="text-xs text-muted-foreground">Gider</span>
            </div>
          </div>
        </div>
      ) : (
        /* Table view */
        <div className="border rounded-lg overflow-hidden flex-1">
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Dönem</th>
                <th className="text-right px-3 py-2 font-medium text-emerald-600">Gelir</th>
                <th className="text-right px-3 py-2 font-medium text-red-600">Gider</th>
                <th className="text-right px-3 py-2 font-medium">Net</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Oran</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {monthData.map(m => (
                <tr key={m.key} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-medium">{m.label} {filterYil}</td>
                  <td className="px-3 py-2.5 text-right text-emerald-600">{m.gelir > 0 ? fmt(m.gelir) : '—'}</td>
                  <td className="px-3 py-2.5 text-right text-red-600">{m.gider > 0 ? fmt(m.gider) : '—'}</td>
                  <td className={`px-3 py-2.5 text-right font-medium ${m.net > 0 ? 'text-emerald-600' : m.net < 0 ? 'text-red-600' : 'text-muted-foreground'}`}>
                    {m.gelir > 0 || m.gider > 0 ? fmt(m.net) : '—'}
                  </td>
                  <td className="px-3 py-2.5 text-right hidden md:table-cell">
                    {m.gelir > 0 && m.gider > 0 ? (
                      <div className="flex items-center gap-2 justify-end">
                        <div className="w-20 h-1.5 rounded-full bg-muted overflow-hidden">
                          <div className="h-full bg-emerald-500 rounded-full"
                            style={{ width: `${Math.min(100, (m.gelir / (m.gelir + m.gider)) * 100)}%` }} />
                        </div>
                        <span className="text-xs text-muted-foreground">
                          {Math.round((m.gelir / (m.gelir + m.gider)) * 100)}%
                        </span>
                      </div>
                    ) : '—'}
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot className="bg-muted/50 border-t font-medium">
              <tr>
                <td className="px-3 py-2.5">Yıl Toplamı</td>
                <td className="px-3 py-2.5 text-right text-emerald-600">{fmt(totalGelir)}</td>
                <td className="px-3 py-2.5 text-right text-red-600">{fmt(totalGider)}</td>
                <td className={`px-3 py-2.5 text-right ${totalNet >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>{fmt(totalNet)}</td>
                <td className="hidden md:table-cell" />
              </tr>
            </tfoot>
          </table>
        </div>
      )}
    </div>
  )
}
