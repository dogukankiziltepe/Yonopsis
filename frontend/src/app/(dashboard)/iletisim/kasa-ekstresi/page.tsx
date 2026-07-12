'use client'

import { useEffect, useState } from 'react'
import { Landmark, Printer } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { bankaHareketleriApi } from '@/lib/api/finans'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import type { BankaHareketi } from '@/types/finans'
import type { KasaBanka } from '@/types/tanimlar'
import { BankaHareketiDurum, BankaHareketiDurumLabel } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const fmtDate = (s: string) => new Date(s).toLocaleDateString('tr-TR')

interface ExtreRow extends BankaHareketi {
  bakiye: number
}

export default function KasaEkstresiPage() {
  const [kasaBankalar, setKasaBankalar] = useState<KasaBanka[]>([])
  const [selectedKasa, setSelectedKasa] = useState('')
  const [items, setItems] = useState<BankaHareketi[]>([])
  const [loading, setLoading] = useState(false)
  const [filterBaslangic, setFilterBaslangic] = useState('')
  const [filterBitis, setFilterBitis] = useState('')

  useEffect(() => {
    kasaBankaApi.getAll().then(r => {
      const aktif = r.data.filter(k => k.isActive)
      setKasaBankalar(aktif)
      if (aktif.length > 0) setSelectedKasa(aktif[0].id)
    }).catch(showApiError)
  }, [])

  useEffect(() => {
    if (!selectedKasa) return
    setLoading(true)
    bankaHareketleriApi.getAll(1, 1000, selectedKasa)
      .then(r => setItems(r.data.items ?? []))
      .catch(showApiError)
      .finally(() => setLoading(false))
  }, [selectedKasa])

  const kasaAdi = kasaBankalar.find(k => k.id === selectedKasa)?.name ?? ''

  const filtered = items
    .filter(item => {
      if (filterBaslangic && item.tarih < filterBaslangic) return false
      if (filterBitis && item.tarih > filterBitis + 'T23:59:59') return false
      return true
    })
    .sort((a, b) => a.tarih.localeCompare(b.tarih))

  // Running balance
  const rows: ExtreRow[] = []
  let bal = 0
  for (const item of filtered) {
    bal += item.tutar
    rows.push({ ...item, bakiye: bal })
  }

  const toplamGiris = filtered.filter(i => i.tutar > 0).reduce((s, i) => s + i.tutar, 0)
  const toplamCikis = filtered.filter(i => i.tutar < 0).reduce((s, i) => s + Math.abs(i.tutar), 0)
  const sonBakiye = rows.length > 0 ? rows[rows.length - 1].bakiye : 0

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Kasa Ekstresi</h1>
        {rows.length > 0 && (
          <Button size="sm" variant="outline" onClick={() => window.print()}>
            <Printer className="h-4 w-4 mr-1" />Yazdır
          </Button>
        )}
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Kasa Ekstresi — {kasaAdi}</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-center gap-2 print:hidden">
        <select value={selectedKasa} onChange={e => setSelectedKasa(e.target.value)}
          className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring min-w-[180px]">
          {kasaBankalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
        </select>
        <Input type="date" className="w-36" value={filterBaslangic}
          onChange={e => setFilterBaslangic(e.target.value)} />
        <span className="text-muted-foreground text-sm">—</span>
        <Input type="date" className="w-36" value={filterBitis}
          onChange={e => setFilterBitis(e.target.value)} />
        {rows.length > 0 && <span className="text-sm text-muted-foreground ml-auto">{rows.length} hareket</span>}
      </div>

      {/* Summary */}
      {rows.length > 0 && (
        <div className="grid grid-cols-3 gap-3">
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Toplam Giriş</p>
            <p className="font-semibold text-emerald-600">{fmt(toplamGiris)}</p>
          </div>
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Toplam Çıkış</p>
            <p className="font-semibold text-red-600">{fmt(toplamCikis)}</p>
          </div>
          <div className="border rounded-lg p-3">
            <p className="text-xs text-muted-foreground">Son Bakiye</p>
            <p className={`font-semibold ${sonBakiye >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>{fmt(sonBakiye)}</p>
          </div>
        </div>
      )}

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : rows.length === 0 ? (
          <div className="p-12 text-center">
            <Landmark className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">
              {selectedKasa ? 'Bu hesap için hareket bulunamadı.' : 'Hesap seçin.'}
            </p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Açıklama</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Ref No</th>
                <th className="text-center px-3 py-2 font-medium hidden lg:table-cell">Durum</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
                <th className="text-right px-3 py-2 font-medium">Bakiye</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {rows.map(row => (
                <tr key={row.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 text-xs text-muted-foreground whitespace-nowrap">{fmtDate(row.tarih)}</td>
                  <td className="px-3 py-2.5">{row.aciklama}</td>
                  <td className="px-3 py-2.5 font-mono text-xs text-muted-foreground hidden md:table-cell">{row.referansNo ?? '—'}</td>
                  <td className="px-3 py-2.5 text-center hidden lg:table-cell">
                    <Badge variant={row.durum === BankaHareketiDurum.Eslestis ? 'secondary' : 'outline'} className="text-xs">
                      {BankaHareketiDurumLabel[row.durum]}
                    </Badge>
                  </td>
                  <td className={`px-3 py-2.5 text-right font-medium ${row.tutar >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>
                    {fmt(row.tutar)}
                  </td>
                  <td className={`px-3 py-2.5 text-right font-semibold ${row.bakiye >= 0 ? '' : 'text-red-600'}`}>
                    {fmt(row.bakiye)}
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot className="bg-muted/50 border-t font-medium">
              <tr>
                <td colSpan={4} className="px-3 py-2.5 text-sm">Son Bakiye</td>
                <td className="px-3 py-2.5 text-right hidden" />
                <td className={`px-3 py-2.5 text-right ${sonBakiye >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>
                  {fmt(sonBakiye)}
                </td>
              </tr>
            </tfoot>
          </table>
        )}
      </div>
    </div>
  )
}
