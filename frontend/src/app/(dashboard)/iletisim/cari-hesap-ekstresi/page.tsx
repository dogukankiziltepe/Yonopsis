'use client'

import { useEffect, useState } from 'react'
import { FileSearch, Printer, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi, tahsilatMakbuzlariApi } from '@/lib/api/finans'
import type { BorcMakbuzu, TahsilatMakbuzu } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const fmtDate = (s: string) => new Date(s).toLocaleDateString('tr-TR')

interface EkstreRow {
  tarih: string
  evrakNo: string
  aciklama: string
  borc: number
  alacak: number
  bakiye: number
  tip: 'borc' | 'tahsilat'
}

export default function CariHesapEkstresiPage() {
  const [allBorclar, setAllBorclar] = useState<BorcMakbuzu[]>([])
  const [allTahsilat, setAllTahsilat] = useState<TahsilatMakbuzu[]>([])
  const [loading, setLoading] = useState(true)
  const [cariAdi, setCariAdi] = useState('')
  const [cariSearch, setCariSearch] = useState('')
  const [filterBaslangic, setFilterBaslangic] = useState('')
  const [filterBitis, setFilterBitis] = useState('')

  useEffect(() => {
    const fetchAll = async () => {
      setLoading(true)
      try {
        const [bRes, tRes] = await Promise.all([
          borcMakbuzlariApi.getAll(1, 1000),
          tahsilatMakbuzlariApi.getAll(1, 1000),
        ])
        setAllBorclar(bRes.data.items ?? [])
        setAllTahsilat(tRes.data.items ?? [])
      } catch (e) { showApiError(e) }
      finally { setLoading(false) }
    }
    fetchAll()
  }, [])

  // Unique persons
  const persons = Array.from(new Set([
    ...allBorclar.map(b => b.borcluAdi ?? b.unitDoorNumber ?? ''),
    ...allTahsilat.map(t => t.borcluAdi ?? ''),
  ].filter(Boolean))).sort((a, b) => a.localeCompare(b, 'tr'))

  const filteredPersons = cariSearch
    ? persons.filter(p => p.toLowerCase().includes(cariSearch.toLowerCase()))
    : persons

  // Build ekste rows for selected person
  const rows: EkstreRow[] = []
  if (cariAdi) {
    for (const b of allBorclar) {
      const ad = b.borcluAdi ?? b.unitDoorNumber ?? ''
      if (ad !== cariAdi) continue
      if (filterBaslangic && b.islemTarihi < filterBaslangic) continue
      if (filterBitis && b.islemTarihi > filterBitis + 'T23:59:59') continue
      rows.push({
        tarih: b.islemTarihi,
        evrakNo: b.evrakNo,
        aciklama: `Borç Makbuzu${b.donem ? ' – ' + b.donem : ''}${b.gelirTanimiAdi ? ' (' + b.gelirTanimiAdi + ')' : ''}`,
        borc: b.tutar,
        alacak: 0,
        bakiye: 0,
        tip: 'borc',
      })
    }
    for (const t of allTahsilat) {
      if ((t.borcluAdi ?? '') !== cariAdi) continue
      if (filterBaslangic && t.islemTarihi < filterBaslangic) continue
      if (filterBitis && t.islemTarihi > filterBitis + 'T23:59:59') continue
      rows.push({
        tarih: t.islemTarihi,
        evrakNo: t.evrakNo,
        aciklama: `Tahsilat Makbuzu${t.kasaBankaAdi ? ' – ' + t.kasaBankaAdi : ''}`,
        borc: 0,
        alacak: t.odemeTutari,
        bakiye: 0,
        tip: 'tahsilat',
      })
    }
    rows.sort((a, b) => a.tarih.localeCompare(b.tarih))
    // Running balance
    let bal = 0
    for (const r of rows) {
      bal += r.borc - r.alacak
      r.bakiye = bal
    }
  }

  const totalBorc = rows.reduce((s, r) => s + r.borc, 0)
  const totalAlacak = rows.reduce((s, r) => s + r.alacak, 0)
  const sonBakiye = rows.length > 0 ? rows[rows.length - 1].bakiye : 0

  return (
    <div className="flex flex-col h-full gap-3 print:gap-2">
      <div className="flex items-center justify-between print:hidden">
        <h1 className="text-xl font-semibold">Cari Hesap Ekstresi</h1>
        {cariAdi && rows.length > 0 && (
          <Button size="sm" variant="outline" onClick={() => window.print()}>
            <Printer className="h-4 w-4 mr-1" />Yazdır
          </Button>
        )}
      </div>
      <div className="print:block hidden mb-2">
        <h1 className="text-lg font-bold">Cari Hesap Ekstresi — {cariAdi}</h1>
        <p className="text-xs text-muted-foreground">{new Date().toLocaleDateString('tr-TR')}</p>
      </div>

      {loading ? (
        <div className="border rounded-lg p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
      ) : (
        <div className="flex gap-3 flex-1 min-h-0">
          {/* Person list */}
          <div className="w-56 border rounded-lg flex flex-col shrink-0 print:hidden">
            <div className="p-2 border-b">
              <div className="relative">
                <Search className="absolute left-2 top-2 h-3.5 w-3.5 text-muted-foreground" />
                <Input className="pl-7 h-7 text-xs" placeholder="Cari ara..." value={cariSearch}
                  onChange={e => setCariSearch(e.target.value)} />
              </div>
            </div>
            <div className="flex-1 overflow-y-auto">
              {filteredPersons.length === 0 ? (
                <p className="text-xs text-muted-foreground p-3">Cari bulunamadı</p>
              ) : filteredPersons.map(p => (
                <button key={p}
                  className={`w-full text-left px-3 py-2 text-sm hover:bg-muted/50 transition-colors border-b border-muted/40 last:border-b-0 ${cariAdi === p ? 'bg-primary/10 text-primary font-medium' : ''}`}
                  onClick={() => setCariAdi(p)}
                >
                  {p}
                </button>
              ))}
            </div>
          </div>

          {/* Statement */}
          <div className="flex-1 flex flex-col gap-3 min-w-0">
            {!cariAdi ? (
              <div className="border rounded-lg flex-1 flex flex-col items-center justify-center text-center p-12">
                <FileSearch className="h-8 w-8 text-muted-foreground/40 mb-3" />
                <p className="text-muted-foreground text-sm">Soldaki listeden bir cari seçin</p>
              </div>
            ) : (
              <>
                {/* Header info */}
                <div className="flex items-center gap-3 print:hidden">
                  <h2 className="font-semibold">{cariAdi}</h2>
                  <div className="flex gap-2 ml-auto">
                    <Input type="date" className="w-36" value={filterBaslangic}
                      onChange={e => setFilterBaslangic(e.target.value)} />
                    <span className="text-muted-foreground text-sm self-center">—</span>
                    <Input type="date" className="w-36" value={filterBitis}
                      onChange={e => setFilterBitis(e.target.value)} />
                  </div>
                </div>

                {/* Summary */}
                {rows.length > 0 && (
                  <div className="grid grid-cols-3 gap-3">
                    <div className="border rounded-lg p-3">
                      <p className="text-xs text-muted-foreground">Toplam Borç</p>
                      <p className="font-semibold text-red-600">{fmt(totalBorc)}</p>
                    </div>
                    <div className="border rounded-lg p-3">
                      <p className="text-xs text-muted-foreground">Toplam Ödeme</p>
                      <p className="font-semibold text-emerald-600">{fmt(totalAlacak)}</p>
                    </div>
                    <div className="border rounded-lg p-3">
                      <p className="text-xs text-muted-foreground">Son Bakiye</p>
                      <p className={`font-semibold ${sonBakiye > 0 ? 'text-red-600' : sonBakiye < 0 ? 'text-emerald-600' : ''}`}>
                        {fmt(sonBakiye)}
                      </p>
                    </div>
                  </div>
                )}

                <div className="border rounded-lg overflow-hidden flex-1">
                  {rows.length === 0 ? (
                    <div className="p-12 text-center">
                      <p className="text-muted-foreground text-sm">Bu cari için hareket bulunamadı.</p>
                    </div>
                  ) : (
                    <table className="w-full text-sm">
                      <thead className="bg-muted/50 border-b">
                        <tr>
                          <th className="text-left px-3 py-2 font-medium">Tarih</th>
                          <th className="text-left px-3 py-2 font-medium hidden sm:table-cell">Evrak No</th>
                          <th className="text-left px-3 py-2 font-medium">Açıklama</th>
                          <th className="text-right px-3 py-2 font-medium">Borç</th>
                          <th className="text-right px-3 py-2 font-medium">Alacak</th>
                          <th className="text-right px-3 py-2 font-medium">Bakiye</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y">
                        {rows.map((row, i) => (
                          <tr key={i} className={`hover:bg-muted/30 ${row.tip === 'tahsilat' ? 'text-emerald-700' : ''}`}>
                            <td className="px-3 py-2.5 text-xs text-muted-foreground whitespace-nowrap">{fmtDate(row.tarih)}</td>
                            <td className="px-3 py-2.5 font-mono text-xs hidden sm:table-cell">{row.evrakNo}</td>
                            <td className="px-3 py-2.5 text-xs">{row.aciklama}</td>
                            <td className="px-3 py-2.5 text-right">{row.borc > 0 ? fmt(row.borc) : '—'}</td>
                            <td className="px-3 py-2.5 text-right">{row.alacak > 0 ? fmt(row.alacak) : '—'}</td>
                            <td className={`px-3 py-2.5 text-right font-medium ${row.bakiye > 0 ? 'text-red-600' : row.bakiye < 0 ? 'text-emerald-600' : 'text-muted-foreground'}`}>
                              {fmt(row.bakiye)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                      <tfoot className="bg-muted/50 border-t font-medium">
                        <tr>
                          <td colSpan={3} className="px-3 py-2.5 text-sm">Toplam</td>
                          <td className="px-3 py-2.5 text-right text-red-600">{fmt(totalBorc)}</td>
                          <td className="px-3 py-2.5 text-right text-emerald-600">{fmt(totalAlacak)}</td>
                          <td className={`px-3 py-2.5 text-right ${sonBakiye > 0 ? 'text-red-600' : sonBakiye < 0 ? 'text-emerald-600' : ''}`}>
                            <Badge variant={sonBakiye > 0 ? 'default' : 'secondary'}>{fmt(sonBakiye)}</Badge>
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  )}
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
