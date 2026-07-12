'use client'

import { useCallback, useEffect, useState } from 'react'
import { Download, BookOpen } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { showError } from '@/lib/toast'
import { exportToCsv } from '@/lib/utils/exportCsv'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { Defter, HesapListItem } from '@/types/muhasebe'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export default function BuyukDefterPage() {
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [hesapId, setHesapId] = useState('')
  const [hesaplar, setHesaplar] = useState<HesapListItem[]>([])
  const [defter, setDefter] = useState<Defter | null>(null)
  const [loading, setLoading] = useState(false)
  const [search, setSearch] = useState('')

  useEffect(() => {
    muhasebeApi.getHesaplar({ fisKesilebilir: undefined })
      .then((r) => setHesaplar(r.data ?? []))
      .catch(() => {})
  }, [])

  const load = useCallback(() => {
    if (!hesapId) return
    setLoading(true)
    muhasebeApi.getKebir(hesapId, from || undefined, to || undefined)
      .then((r) => setDefter(r.data))
      .catch(() => showError('Büyük defter yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [hesapId, from, to])

  useEffect(() => {
    if (hesapId) load()
    else setDefter(null)
  }, [hesapId, load])

  const doExport = () => {
    if (!defter) return
    exportToCsv(`buyuk-defter-${defter.hesapKodu}`,
      ['Tarih', 'Fiş No', 'Yev. No', 'Hesap Kodu', 'Hesap Adı', 'Açıklama', 'Borç', 'Alacak', 'Bakiye'],
      defter.satirlar.map((s) => [
        s.fisTarihi.slice(0, 10), s.fisNo, s.yevmiyeNo ?? '',
        s.hesapKodu, s.hesapAdi, s.aciklama ?? '',
        s.borcTutar, s.alacakTutar, s.yuruyenBakiye,
      ]))
  }

  const filtered = hesaplar.filter((h) =>
    !search || h.hesapKodu.includes(search) || h.hesapAdi.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h1 className="text-xl font-semibold">Büyük Defter</h1>
          {defter && (
            <p className="text-xs text-muted-foreground mt-0.5">
              <span className="font-mono">{defter.hesapKodu}</span> — {defter.hesapAdi} &nbsp;·&nbsp;
              Açılış: <b>{fmt(defter.acilisBakiye)}</b> &nbsp;·&nbsp;
              Kapanış: <b>{fmt(defter.kapanisBakiye)}</b>
            </p>
          )}
        </div>
        <Button size="sm" variant="outline" onClick={doExport} disabled={!defter}>
          <Download className="h-4 w-4 mr-1" /> CSV
        </Button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1 flex-1 min-w-[200px]">
          <Label className="text-xs">Hesap Ara</Label>
          <Input
            placeholder="Kod veya isim..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="h-8"
          />
        </div>
        <div className="space-y-1 min-w-[280px]">
          <Label className="text-xs">Hesap</Label>
          <select
            value={hesapId}
            onChange={(e) => setHesapId(e.target.value)}
            className="h-8 w-full rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Hesap seçin...</option>
            {filtered.map((h) => (
              <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Başlangıç</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="h-8" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Bitiş</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="h-8" />
        </div>
        <Button size="sm" onClick={load} disabled={!hesapId || loading}>Yenile</Button>
      </div>

      {!hesapId ? (
        <div className="flex flex-col items-center py-16">
          <BookOpen className="h-10 w-10 text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">Bir hesap seçin.</p>
          <p className="text-xs text-muted-foreground mt-1">Seçilen hesabın tüm alt hesapları dahil edilir.</p>
        </div>
      ) : loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : !defter || defter.satirlar.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">Bu hesap için hareket yok.</div>
      ) : (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tarih</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Fiş No</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground w-10">Yev.</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Açıklama</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Bakiye</th>
              </tr>
            </thead>
            <tbody>
              {/* Opening */}
              <tr className="border-b bg-muted/20">
                <td colSpan={7} className="px-3 py-1.5 text-xs text-muted-foreground italic">Dönem başı bakiyesi</td>
                <td className="px-3 py-1.5 text-right tabular-nums text-xs font-medium">{fmt(defter.acilisBakiye)}</td>
              </tr>
              {defter.satirlar.map((s, i) => (
                <tr key={i} className="border-b last:border-0 hover:bg-muted/20">
                  <td className="px-3 py-1.5">{s.fisTarihi.slice(0, 10)}</td>
                  <td className="px-3 py-1.5 font-mono text-xs">{s.fisNo}</td>
                  <td className="px-3 py-1.5 text-muted-foreground text-xs">{s.yevmiyeNo ?? '-'}</td>
                  <td className="px-3 py-1.5 font-mono text-xs text-muted-foreground">{s.hesapKodu}</td>
                  <td className="px-3 py-1.5 max-w-[200px] truncate">{s.aciklama}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.borcTutar ? fmt(s.borcTutar) : ''}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.alacakTutar ? fmt(s.alacakTutar) : ''}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums font-medium">{fmt(s.yuruyenBakiye)}</td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr className="border-t bg-muted/30 font-semibold">
                <td colSpan={5} className="px-3 py-2 text-right">Toplam / Kapanış</td>
                <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.toplamBorc)}</td>
                <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.toplamAlacak)}</td>
                <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.kapanisBakiye)}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      )}
    </div>
  )
}
