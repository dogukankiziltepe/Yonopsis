'use client'

import { useCallback, useEffect, useState } from 'react'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showError } from '@/lib/toast'
import { muhasebeApi } from '@/lib/api/muhasebe'
import {
  FisDetaylarItem,
  FisDurumu,
  HesapListItem,
  fisDurumuLabel,
} from '@/types/muhasebe'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export default function FisDetaylariPage() {
  const [items, setItems] = useState<FisDetaylarItem[]>([])
  const [loading, setLoading] = useState(true)
  const [hesaplar, setHesaplar] = useState<HesapListItem[]>([])

  const [hesapId, setHesapId] = useState('')
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [durum, setDurum] = useState<FisDurumu | ''>('')

  const load = useCallback(() => {
    setLoading(true)
    muhasebeApi.getFisDetaylari({
      hesapId: hesapId || undefined,
      from: from || undefined,
      to: to || undefined,
      durum: durum === '' ? undefined : durum,
      pageSize: 100,
    })
      .then((res) => setItems(res.data?.items ?? []))
      .catch(() => showError('Fiş detayları yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [hesapId, from, to, durum])

  useEffect(() => { load() }, [load])
  useEffect(() => {
    muhasebeApi.getHesaplar({ fisKesilebilir: true }).then((res) => setHesaplar(res.data ?? [])).catch(() => {})
  }, [])

  return (
    <div className="flex flex-col h-full">
      <h1 className="text-xl font-semibold mb-4">Fiş Detayları</h1>

      <div className="flex flex-wrap items-end gap-2 mb-3">
        <div className="space-y-1">
          <Label className="text-xs">Hesap</Label>
          <select value={hesapId} onChange={(e) => setHesapId(e.target.value)} className="h-8 rounded-md border bg-background px-2 text-sm w-64">
            <option value="">Tüm hesaplar</option>
            {hesaplar.map((h) => <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>)}
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
        <div className="space-y-1">
          <Label className="text-xs">Durum</Label>
          <select
            value={durum}
            onChange={(e) => setDurum(e.target.value === '' ? '' : (Number(e.target.value) as FisDurumu))}
            className="h-8 rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Tümü</option>
            <option value={FisDurumu.Taslak}>Taslak</option>
            <option value={FisDurumu.Onaylandi}>Entegre</option>
            <option value={FisDurumu.Iptal}>İptal</option>
          </select>
        </div>
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Fiş No</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tarih</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Hesap</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Açıklama</th>
              <th className="text-right px-3 py-2.5 font-medium text-muted-foreground">Borç</th>
              <th className="text-right px-3 py-2.5 font-medium text-muted-foreground">Alacak</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={7} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={7} className="text-center py-12 text-muted-foreground">Kayıt bulunamadı.</td></tr>
            ) : (
              items.map((d) => (
                <tr key={d.detayId} className="border-b last:border-0">
                  <td className="px-3 py-2 font-mono text-xs">{d.fisNo}</td>
                  <td className="px-3 py-2">{d.fisTarihi.slice(0, 10)}</td>
                  <td className="px-3 py-2"><span className="font-mono text-xs text-muted-foreground">{d.hesapKodu}</span> {d.hesapAdi}</td>
                  <td className="px-3 py-2 max-w-[220px] truncate">{d.aciklama}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{d.borcTutar ? fmt(d.borcTutar) : ''}</td>
                  <td className="px-3 py-2 text-right tabular-nums">{d.alacakTutar ? fmt(d.alacakTutar) : ''}</td>
                  <td className="px-3 py-2">
                    <Badge variant={d.durum === FisDurumu.Onaylandi ? 'default' : d.durum === FisDurumu.Iptal ? 'destructive' : 'secondary'}>
                      {fisDurumuLabel[d.durum]}
                    </Badge>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
