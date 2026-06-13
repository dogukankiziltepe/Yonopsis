'use client'

import { useCallback, useEffect, useState } from 'react'
import { Download, BookOpen } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { showError } from '@/lib/toast'
import { exportToCsv } from '@/lib/utils/exportCsv'
import { muhasebeApi } from '@/lib/api/muhasebe'
import {
  Defter,
  HesapListItem,
  YevmiyeDefteri,
  fisTuruLabel,
} from '@/types/muhasebe'

type Tab = 'yevmiye' | 'kebir' | 'muavin'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export default function DefterlerPage() {
  const [tab, setTab] = useState<Tab>('yevmiye')
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [hesapId, setHesapId] = useState('')
  const [hesaplar, setHesaplar] = useState<HesapListItem[]>([])

  const [yevmiye, setYevmiye] = useState<YevmiyeDefteri | null>(null)
  const [defter, setDefter] = useState<Defter | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    muhasebeApi.getHesaplar().then((r) => setHesaplar(r.data ?? [])).catch(() => {})
  }, [])

  const load = useCallback(() => {
    setLoading(true)
    const f = from || undefined
    const t = to || undefined
    if (tab === 'yevmiye') {
      muhasebeApi.getYevmiye(f, t)
        .then((r) => setYevmiye(r.data))
        .catch(() => showError('Yevmiye defteri yüklenemedi.'))
        .finally(() => setLoading(false))
    } else {
      if (!hesapId) { setDefter(null); setLoading(false); return }
      const call = tab === 'kebir' ? muhasebeApi.getKebir : muhasebeApi.getMuavin
      call(hesapId, f, t)
        .then((r) => setDefter(r.data))
        .catch(() => showError('Defter yüklenemedi.'))
        .finally(() => setLoading(false))
    }
  }, [tab, from, to, hesapId])

  useEffect(() => { load() }, [load])

  const exportYevmiye = () => {
    if (!yevmiye) return
    exportToCsv('yevmiye-defteri',
      ['Yev. No', 'Fiş No', 'Tarih', 'Tür', 'Hesap Kodu', 'Hesap Adı', 'Açıklama', 'Borç', 'Alacak'],
      yevmiye.satirlar.map((s) => [
        s.yevmiyeNo ?? '', s.fisNo, s.fisTarihi.slice(0, 10), fisTuruLabel[s.fisTuru],
        s.hesapKodu, s.hesapAdi, s.aciklama ?? '', s.borcTutar, s.alacakTutar,
      ]))
  }

  const exportDefter = () => {
    if (!defter) return
    exportToCsv(`${tab}-${defter.hesapKodu}`,
      ['Tarih', 'Fiş No', 'Yev. No', 'Hesap', 'Açıklama', 'Borç', 'Alacak', 'Bakiye'],
      defter.satirlar.map((s) => [
        s.fisTarihi.slice(0, 10), s.fisNo, s.yevmiyeNo ?? '', `${s.hesapKodu} ${s.hesapAdi}`,
        s.aciklama ?? '', s.borcTutar, s.alacakTutar, s.yuruyenBakiye,
      ]))
  }

  const tabs: { key: Tab; label: string }[] = [
    { key: 'yevmiye', label: 'Yevmiye Defteri' },
    { key: 'kebir', label: 'Defter-i Kebir' },
    { key: 'muavin', label: 'Muavin Defter' },
  ]

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Defterler</h1>
        <Button size="sm" variant="outline" onClick={tab === 'yevmiye' ? exportYevmiye : exportDefter}>
          <Download className="h-4 w-4 mr-1" /> Excel/CSV
        </Button>
      </div>

      <div className="flex gap-1 border-b mb-4">
        {tabs.map((t) => (
          <button
            key={t.key}
            onClick={() => setTab(t.key)}
            className={`px-3 py-2 text-sm font-medium border-b-2 -mb-px ${
              tab === t.key ? 'border-primary text-foreground' : 'border-transparent text-muted-foreground hover:text-foreground'
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      <div className="flex flex-wrap items-end gap-2 mb-3">
        {tab !== 'yevmiye' && (
          <div className="space-y-1">
            <Label className="text-xs">Hesap</Label>
            <select value={hesapId} onChange={(e) => setHesapId(e.target.value)} className="h-8 rounded-md border bg-background px-2 text-sm w-72">
              <option value="">Hesap seç...</option>
              {hesaplar.map((h) => <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>)}
            </select>
          </div>
        )}
        <div className="space-y-1">
          <Label className="text-xs">Başlangıç</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="h-8" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Bitiş</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="h-8" />
        </div>
      </div>

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : tab === 'yevmiye' ? (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Yev.</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Fiş No</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tarih</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Açıklama</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak</th>
              </tr>
            </thead>
            <tbody>
              {!yevmiye || yevmiye.satirlar.length === 0 ? (
                <tr><td colSpan={7} className="text-center py-12 text-muted-foreground">Kayıt yok.</td></tr>
              ) : yevmiye.satirlar.map((s, i) => (
                <tr key={i} className="border-b last:border-0">
                  <td className="px-3 py-1.5">{s.yevmiyeNo ?? '-'}</td>
                  <td className="px-3 py-1.5 font-mono text-xs">{s.fisNo}</td>
                  <td className="px-3 py-1.5">{s.fisTarihi.slice(0, 10)}</td>
                  <td className="px-3 py-1.5"><span className="font-mono text-xs text-muted-foreground">{s.hesapKodu}</span> {s.hesapAdi}</td>
                  <td className="px-3 py-1.5 max-w-[200px] truncate">{s.aciklama}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.borcTutar ? fmt(s.borcTutar) : ''}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.alacakTutar ? fmt(s.alacakTutar) : ''}</td>
                </tr>
              ))}
            </tbody>
            {yevmiye && (
              <tfoot>
                <tr className="border-t bg-muted/30 font-medium">
                  <td colSpan={5} className="px-3 py-2 text-right">Toplam</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(yevmiye.toplamBorc)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(yevmiye.toplamAlacak)}</td>
                </tr>
              </tfoot>
            )}
          </table>
        </div>
      ) : !hesapId ? (
        <div className="flex flex-col items-center py-12">
          <BookOpen className="h-10 w-10 text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">Lütfen bir hesap seçin.</p>
        </div>
      ) : !defter ? (
        <div className="text-center py-12 text-muted-foreground">Kayıt yok.</div>
      ) : (
        <div className="space-y-3">
          <div className="flex flex-wrap gap-4 text-sm">
            <span><span className="text-muted-foreground">Hesap:</span> <b>{defter.hesapKodu}</b> {defter.hesapAdi}</span>
            <span><span className="text-muted-foreground">Açılış:</span> {fmt(defter.acilisBakiye)}</span>
            <span><span className="text-muted-foreground">Kapanış:</span> <b>{fmt(defter.kapanisBakiye)}</b></span>
          </div>
          <div className="border rounded-lg overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-muted/50">
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tarih</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Fiş No</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Açıklama</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground">Bakiye</th>
                </tr>
              </thead>
              <tbody>
                {defter.satirlar.length === 0 ? (
                  <tr><td colSpan={6} className="text-center py-12 text-muted-foreground">Hareket yok.</td></tr>
                ) : defter.satirlar.map((s, i) => (
                  <tr key={i} className="border-b last:border-0">
                    <td className="px-3 py-1.5">{s.fisTarihi.slice(0, 10)}</td>
                    <td className="px-3 py-1.5 font-mono text-xs">{s.fisNo}</td>
                    <td className="px-3 py-1.5 max-w-[260px] truncate">{s.aciklama}</td>
                    <td className="px-3 py-1.5 text-right tabular-nums">{s.borcTutar ? fmt(s.borcTutar) : ''}</td>
                    <td className="px-3 py-1.5 text-right tabular-nums">{s.alacakTutar ? fmt(s.alacakTutar) : ''}</td>
                    <td className="px-3 py-1.5 text-right tabular-nums">{fmt(s.yuruyenBakiye)}</td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr className="border-t bg-muted/30 font-medium">
                  <td colSpan={3} className="px-3 py-2 text-right">Toplam</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.toplamBorc)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.toplamAlacak)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(defter.kapanisBakiye)}</td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      )}
    </div>
  )
}
