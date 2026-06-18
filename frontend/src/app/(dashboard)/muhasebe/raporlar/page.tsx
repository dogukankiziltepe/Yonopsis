'use client'

import { useCallback, useEffect, useState } from 'react'
import { Download } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showError } from '@/lib/toast'
import { exportToCsv } from '@/lib/utils/exportCsv'
import { muhasebeApi } from '@/lib/api/muhasebe'
import {
  CariBakiye,
  CariTuru,
  Defter,
  HesapListItem,
  Mizan,
  cariTuruLabel,
} from '@/types/muhasebe'

type Tab = 'mizan' | 'cari-ekstre' | 'borc-alacak'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const cariTuruOptions = Object.values(CariTuru).filter((v) => typeof v === 'number') as CariTuru[]

export default function RaporlarPage() {
  const [tab, setTab] = useState<Tab>('mizan')
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [loading, setLoading] = useState(false)

  const [mizan, setMizan] = useState<Mizan | null>(null)

  const [cariList, setCariList] = useState<HesapListItem[]>([])
  const [cariHesapId, setCariHesapId] = useState('')
  const [ekstre, setEkstre] = useState<Defter | null>(null)

  const [cariTuru, setCariTuru] = useState<CariTuru | ''>('')
  const [bakiyeler, setBakiyeler] = useState<CariBakiye[]>([])

  useEffect(() => {
    muhasebeApi.getCariHesaplar().then((r) => setCariList(r.data ?? [])).catch(() => {})
  }, [])

  const load = useCallback(() => {
    setLoading(true)
    const f = from || undefined
    const t = to || undefined
    if (tab === 'mizan') {
      muhasebeApi.getMizan(f, t)
        .then((r) => setMizan(r.data))
        .catch(() => showError('Mizan yüklenemedi.'))
        .finally(() => setLoading(false))
    } else if (tab === 'cari-ekstre') {
      if (!cariHesapId) { setEkstre(null); setLoading(false); return }
      muhasebeApi.getCariEkstre(cariHesapId, f, t)
        .then((r) => setEkstre(r.data))
        .catch(() => showError('Cari ekstre yüklenemedi.'))
        .finally(() => setLoading(false))
    } else {
      muhasebeApi.getBorcAlacak(cariTuru === '' ? undefined : cariTuru)
        .then((r) => setBakiyeler(r.data ?? []))
        .catch(() => showError('Borç/Alacak durumu yüklenemedi.'))
        .finally(() => setLoading(false))
    }
  }, [tab, from, to, cariHesapId, cariTuru])

  useEffect(() => { load() }, [load])

  const exportCurrent = () => {
    if (tab === 'mizan' && mizan) {
      exportToCsv('mizan',
        ['Hesap Kodu', 'Hesap Adı', 'Borç', 'Alacak', 'Borç Bakiye', 'Alacak Bakiye'],
        mizan.satirlar.map((s) => [s.hesapKodu, s.hesapAdi, s.borcToplam, s.alacakToplam, s.borcBakiye, s.alacakBakiye]))
    } else if (tab === 'cari-ekstre' && ekstre) {
      exportToCsv(`cari-ekstre-${ekstre.hesapKodu}`,
        ['Tarih', 'Fiş No', 'Açıklama', 'Borç', 'Alacak', 'Bakiye'],
        ekstre.satirlar.map((s) => [s.fisTarihi.slice(0, 10), s.fisNo, s.aciklama ?? '', s.borcTutar, s.alacakTutar, s.yuruyenBakiye]))
    } else if (tab === 'borc-alacak') {
      exportToCsv('borc-alacak',
        ['Hesap Kodu', 'Hesap Adı', 'Tür', 'Borç', 'Alacak', 'Bakiye'],
        bakiyeler.map((b) => [b.hesapKodu, b.hesapAdi, b.cariTuru != null ? cariTuruLabel[b.cariTuru] : '', b.borcToplam, b.alacakToplam, b.bakiye]))
    }
  }

  const tabs: { key: Tab; label: string }[] = [
    { key: 'mizan', label: 'Mizan' },
    { key: 'cari-ekstre', label: 'Cari Ekstre' },
    { key: 'borc-alacak', label: 'Borç / Alacak Durumu' },
  ]

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Raporlar</h1>
        <Button size="sm" variant="outline" onClick={exportCurrent}>
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
        {tab === 'cari-ekstre' && (
          <div className="space-y-1">
            <Label className="text-xs">Cari Hesap</Label>
            <select value={cariHesapId} onChange={(e) => setCariHesapId(e.target.value)} className="h-8 rounded-md border bg-background px-2 text-sm w-72">
              <option value="">Cari seç...</option>
              {cariList.map((h) => <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>)}
            </select>
          </div>
        )}
        {tab === 'borc-alacak' && (
          <div className="space-y-1">
            <Label className="text-xs">Cari Türü</Label>
            <select
              value={cariTuru}
              onChange={(e) => setCariTuru(e.target.value === '' ? '' : (Number(e.target.value) as CariTuru))}
              className="h-8 rounded-md border bg-background px-2 text-sm"
            >
              <option value="">Tümü</option>
              {cariTuruOptions.map((c) => <option key={c} value={c}>{cariTuruLabel[c]}</option>)}
            </select>
          </div>
        )}
        {tab !== 'borc-alacak' && (
          <>
            <div className="space-y-1">
              <Label className="text-xs">Başlangıç</Label>
              <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="h-8" />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Bitiş</Label>
              <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="h-8" />
            </div>
          </>
        )}
      </div>

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : tab === 'mizan' ? (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Kodu</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Adı</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç Bakiye</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak Bakiye</th>
              </tr>
            </thead>
            <tbody>
              {!mizan || mizan.satirlar.length === 0 ? (
                <tr><td colSpan={6} className="text-center py-12 text-muted-foreground">Kayıt yok.</td></tr>
              ) : mizan.satirlar.map((s) => (
                <tr key={s.hesapId} className="border-b last:border-0">
                  <td className="px-3 py-1.5 font-mono text-xs">{s.hesapKodu}</td>
                  <td className="px-3 py-1.5">{s.hesapAdi}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{fmt(s.borcToplam)}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{fmt(s.alacakToplam)}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.borcBakiye ? fmt(s.borcBakiye) : ''}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{s.alacakBakiye ? fmt(s.alacakBakiye) : ''}</td>
                </tr>
              ))}
            </tbody>
            {mizan && (
              <tfoot>
                <tr className="border-t bg-muted/30 font-medium">
                  <td colSpan={2} className="px-3 py-2 text-right">Toplam</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(mizan.toplamBorc)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(mizan.toplamAlacak)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(mizan.toplamBorcBakiye)}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{fmt(mizan.toplamAlacakBakiye)}</td>
                </tr>
              </tfoot>
            )}
          </table>
        </div>
      ) : tab === 'cari-ekstre' ? (
        !cariHesapId ? (
          <div className="text-center py-12 text-muted-foreground">Lütfen bir cari hesap seçin.</div>
        ) : !ekstre ? (
          <div className="text-center py-12 text-muted-foreground">Kayıt yok.</div>
        ) : (
          <div className="space-y-3">
            <div className="flex flex-wrap gap-4 text-sm">
              <span><span className="text-muted-foreground">Cari:</span> <b>{ekstre.hesapKodu}</b> {ekstre.hesapAdi}</span>
              <span><span className="text-muted-foreground">Açılış:</span> {fmt(ekstre.acilisBakiye)}</span>
              <span><span className="text-muted-foreground">Bakiye:</span> <b>{fmt(ekstre.kapanisBakiye)}</b></span>
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
                  {ekstre.satirlar.length === 0 ? (
                    <tr><td colSpan={6} className="text-center py-12 text-muted-foreground">Hareket yok.</td></tr>
                  ) : ekstre.satirlar.map((s, i) => (
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
              </table>
            </div>
          </div>
        )
      ) : (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Kodu</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Adı</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tür</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Borç</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Alacak</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">Bakiye</th>
              </tr>
            </thead>
            <tbody>
              {bakiyeler.length === 0 ? (
                <tr><td colSpan={6} className="text-center py-12 text-muted-foreground">Cari hesap yok.</td></tr>
              ) : bakiyeler.map((b) => (
                <tr key={b.hesapId} className="border-b last:border-0">
                  <td className="px-3 py-1.5 font-mono text-xs">{b.hesapKodu}</td>
                  <td className="px-3 py-1.5">{b.hesapAdi}</td>
                  <td className="px-3 py-1.5">{b.cariTuru != null ? <Badge variant="outline">{cariTuruLabel[b.cariTuru]}</Badge> : '-'}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{fmt(b.borcToplam)}</td>
                  <td className="px-3 py-1.5 text-right tabular-nums">{fmt(b.alacakToplam)}</td>
                  <td className={`px-3 py-1.5 text-right tabular-nums font-medium ${b.bakiye > 0 ? 'text-destructive' : b.bakiye < 0 ? 'text-emerald-600' : ''}`}>{fmt(b.bakiye)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
