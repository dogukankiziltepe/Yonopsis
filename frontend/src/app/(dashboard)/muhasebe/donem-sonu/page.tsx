'use client'

import { useEffect, useState } from 'react'
import { CheckCircle2, AlertTriangle, Lock } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showSuccess, showError } from '@/lib/toast'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { Donem, KapanisOnizleme } from '@/types/muhasebe'

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

export default function DonemSonuPage() {
  const [donemler, setDonemler] = useState<Donem[]>([])
  const [donemId, setDonemId] = useState('')
  const [onizleme, setOnizleme] = useState<KapanisOnizleme | null>(null)
  const [loading, setLoading] = useState(false)
  const [kapaniyor, setKapaniyor] = useState(false)

  useEffect(() => {
    muhasebeApi.getDonemler()
      .then((r) => {
        const list = r.data ?? []
        setDonemler(list)
        const acik = list.find((d) => d.durum === 0)
        if (acik) setDonemId(acik.id)
      })
      .catch(() => showError('Dönemler yüklenemedi.'))
  }, [])

  const loadOnizleme = async () => {
    if (!donemId) return
    setLoading(true)
    setOnizleme(null)
    try {
      const { data } = await muhasebeApi.kapanisOnizleme(donemId)
      setOnizleme(data)
    } catch { showError('Önizleme alınamadı.') } finally { setLoading(false) }
  }

  const kapat = async () => {
    if (!donemId) return
    if (!confirm('Dönem kapatılacak: kapanış fişi üretilecek, dönem kilitlenecek ve yeni dönem açılış fişi oluşturulacak. Bu işlem geri alınamaz. Devam edilsin mi?')) return
    setKapaniyor(true)
    try {
      await muhasebeApi.donemSonuKapanis(donemId)
      showSuccess('Dönem kapatıldı, yeni dönem açıldı.')
      setOnizleme(null)
      const r = await muhasebeApi.getDonemler()
      setDonemler(r.data ?? [])
      const acik = (r.data ?? []).find((d) => d.durum === 0)
      setDonemId(acik?.id ?? '')
    } catch {} finally { setKapaniyor(false) }
  }

  const secili = donemler.find((d) => d.id === donemId)
  const kapali = secili?.durum === 1

  return (
    <div className="flex flex-col h-full max-w-4xl">
      <h1 className="text-xl font-semibold mb-4">Dönem Sonu İşlemleri</h1>

      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1">
          <Label className="text-xs">Dönem</Label>
          <select value={donemId} onChange={(e) => { setDonemId(e.target.value); setOnizleme(null) }} className="h-9 rounded-md border bg-background px-2 text-sm w-56">
            <option value="">Dönem seç...</option>
            {donemler.map((d) => <option key={d.id} value={d.id}>{d.ad} {d.durum === 1 ? '(Kapalı)' : ''}</option>)}
          </select>
        </div>
        <Button size="sm" variant="outline" onClick={loadOnizleme} disabled={!donemId || loading || kapali}>
          {loading ? 'Hesaplanıyor...' : 'Önizleme'}
        </Button>
        {kapali && (
          <span className="flex items-center gap-1 text-sm text-muted-foreground"><Lock className="h-4 w-4" /> Bu dönem kapalı.</span>
        )}
      </div>

      {onizleme && (
        <div className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div className="border rounded-lg p-3">
              <div className="text-xs text-muted-foreground">Toplam Gelir</div>
              <div className="text-lg font-semibold tabular-nums">{fmt(onizleme.toplamGelir)}</div>
            </div>
            <div className="border rounded-lg p-3">
              <div className="text-xs text-muted-foreground">Toplam Gider</div>
              <div className="text-lg font-semibold tabular-nums">{fmt(onizleme.toplamGider)}</div>
            </div>
            <div className="border rounded-lg p-3">
              <div className="text-xs text-muted-foreground">Net Sonuç</div>
              <div className={`text-lg font-semibold tabular-nums ${onizleme.netSonuc >= 0 ? 'text-emerald-600' : 'text-destructive'}`}>
                {fmt(onizleme.netSonuc)}
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2">
            {onizleme.dengeli ? (
              <Badge variant="default" className="gap-1"><CheckCircle2 className="h-3 w-3" /> Defter dengeli</Badge>
            ) : (
              <Badge variant="destructive" className="gap-1"><AlertTriangle className="h-3 w-3" /> Defter dengesiz</Badge>
            )}
          </div>

          <div className="border rounded-lg overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-muted/50">
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Kodu</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap Adı</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground">Net Bakiye</th>
                </tr>
              </thead>
              <tbody>
                {onizleme.bakiyeler.length === 0 ? (
                  <tr><td colSpan={3} className="text-center py-8 text-muted-foreground">Hareket yok.</td></tr>
                ) : onizleme.bakiyeler.map((b) => (
                  <tr key={b.hesapId} className="border-b last:border-0">
                    <td className="px-3 py-1.5 font-mono text-xs">{b.hesapKodu}</td>
                    <td className="px-3 py-1.5">{b.hesapAdi}</td>
                    <td className="px-3 py-1.5 text-right tabular-nums">{fmt(b.net)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex justify-end">
            <Button onClick={kapat} disabled={kapaniyor || !onizleme.dengeli}>
              <Lock className="h-4 w-4 mr-1" /> {kapaniyor ? 'Kapatılıyor...' : 'Dönemi Kapat'}
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
