'use client'

import { useCallback, useEffect, useState } from 'react'
import { Download, Wallet } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { showError } from '@/lib/toast'
import { exportToCsv } from '@/lib/utils/exportCsv'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { Defter, MuhasebeParametre } from '@/types/muhasebe'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export default function KasaDefteriPage() {
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [parametreler, setParametreler] = useState<MuhasebeParametre | null>(null)
  const [defter, setDefter] = useState<Defter | null>(null)
  const [loading, setLoading] = useState(false)
  const [loadingParams, setLoadingParams] = useState(true)

  useEffect(() => {
    muhasebeApi.getParametre()
      .then((r) => setParametreler(r.data))
      .catch(() => showError('Muhasebe parametreleri yüklenemedi.'))
      .finally(() => setLoadingParams(false))
  }, [])

  const load = useCallback(() => {
    if (!parametreler?.varsayilanKasaHesapId) return
    setLoading(true)
    muhasebeApi.getKebir(parametreler.varsayilanKasaHesapId, from || undefined, to || undefined)
      .then((r) => setDefter(r.data))
      .catch(() => showError('Kasa defteri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [parametreler, from, to])

  useEffect(() => {
    if (parametreler) load()
  }, [parametreler, load])

  const doExport = () => {
    if (!defter) return
    exportToCsv(`kasa-defteri-${defter.hesapKodu}`,
      ['Tarih', 'Fiş No', 'Açıklama', 'Borç (Giriş)', 'Alacak (Çıkış)', 'Bakiye'],
      defter.satirlar.map((s) => [
        s.fisTarihi.slice(0, 10), s.fisNo, s.aciklama ?? '', s.borcTutar, s.alacakTutar, s.yuruyenBakiye,
      ]))
  }

  if (loadingParams) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  if (!parametreler?.varsayilanKasaHesapId) {
    return (
      <div className="flex flex-col h-full">
        <h1 className="text-xl font-semibold mb-4">Kasa Defteri</h1>
        <div className="border rounded-lg flex flex-col items-center justify-center py-16 text-center">
          <Wallet className="h-10 w-10 text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground font-medium">Varsayılan kasa hesabı tanımlanmamış</p>
          <p className="text-xs text-muted-foreground mt-1">
            Muhasebe → Parametreler ekranından varsayılan kasa hesabını seçin.
          </p>
        </div>
      </div>
    )
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h1 className="text-xl font-semibold">Kasa Defteri</h1>
          {defter && (
            <p className="text-xs text-muted-foreground mt-0.5">
              <span className="font-mono">{defter.hesapKodu}</span> — {defter.hesapAdi} &nbsp;·&nbsp;
              Açılış: <b>{fmt(defter.acilisBakiye)}</b> &nbsp;·&nbsp;
              Kapanış: <b className={defter.kapanisBakiye >= 0 ? 'text-emerald-600' : 'text-destructive'}>{fmt(defter.kapanisBakiye)}</b>
            </p>
          )}
        </div>
        <Button size="sm" variant="outline" onClick={doExport} disabled={!defter}>
          <Download className="h-4 w-4 mr-1" /> CSV
        </Button>
      </div>

      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1">
          <Label className="text-xs">Başlangıç</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="h-8" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Bitiş</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="h-8" />
        </div>
        <Button size="sm" onClick={load} disabled={loading}>Yenile</Button>
      </div>

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : !defter || defter.satirlar.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">Bu dönemde kasa hareketi yok.</div>
      ) : (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tarih</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Fiş No</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Açıklama</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Giriş (Borç)</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Çıkış (Alacak)</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Bakiye</th>
              </tr>
            </thead>
            <tbody>
              {/* Opening row */}
              <tr className="border-b bg-muted/20">
                <td colSpan={5} className="px-3 py-1.5 text-xs text-muted-foreground italic">Dönem başı bakiyesi</td>
                <td className="px-3 py-1.5 text-right tabular-nums text-xs font-medium">{fmt(defter.acilisBakiye)}</td>
              </tr>
              {defter.satirlar.map((s, i) => (
                <tr key={i} className="border-b last:border-0 hover:bg-muted/20">
                  <td className="px-3 py-1.5">{s.fisTarihi.slice(0, 10)}</td>
                  <td className="px-3 py-1.5 font-mono text-xs">{s.fisNo}</td>
                  <td className="px-3 py-1.5 max-w-[240px] truncate">{s.aciklama}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums text-emerald-700 dark:text-emerald-400">
                    {s.borcTutar ? fmt(s.borcTutar) : ''}
                  </td>
                  <td className="px-3 py-1.5 text-right tabular-nums text-destructive">
                    {s.alacakTutar ? fmt(s.alacakTutar) : ''}
                  </td>
                  <td className={`px-3 py-1.5 text-right tabular-nums font-medium ${s.yuruyenBakiye < 0 ? 'text-destructive' : ''}`}>
                    {fmt(s.yuruyenBakiye)}
                  </td>
                </tr>
              ))}
            </tbody>
            <tfoot>
              <tr className="border-t bg-muted/30 font-semibold">
                <td colSpan={3} className="px-3 py-2 text-right">Toplam / Kapanış</td>
                <td className="px-3 py-2 text-right tabular-nums text-emerald-700 dark:text-emerald-400">{fmt(defter.toplamBorc)}</td>
                <td className="px-3 py-2 text-right tabular-nums text-destructive">{fmt(defter.toplamAlacak)}</td>
                <td className={`px-3 py-2 text-right tabular-nums ${defter.kapanisBakiye < 0 ? 'text-destructive' : 'text-emerald-600'}`}>
                  {fmt(defter.kapanisBakiye)}
                </td>
              </tr>
            </tfoot>
          </table>
        </div>
      )}
    </div>
  )
}
