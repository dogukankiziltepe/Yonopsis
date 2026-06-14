'use client'

import { useCallback, useEffect, useState } from 'react'
import { Save } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { showSuccess, showError } from '@/lib/toast'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { HesapListItem, MuhasebeParametre } from '@/types/muhasebe'

function HesapSelect({ value, onChange, hesaplar }: { value: string; onChange: (v: string) => void; hesaplar: HesapListItem[] }) {
  return (
    <select value={value} onChange={(e) => onChange(e.target.value)} className="h-9 w-full rounded-md border bg-background px-2 text-sm">
      <option value="">(Seçilmedi)</option>
      {hesaplar.map((h) => <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>)}
    </select>
  )
}

export default function ParametrelerPage() {
  const [p, setP] = useState<MuhasebeParametre | null>(null)
  const [hesaplar, setHesaplar] = useState<HesapListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    Promise.all([muhasebeApi.getParametre(), muhasebeApi.getHesaplar({ fisKesilebilir: true })])
      .then(([pr, hr]) => { setP(pr.data); setHesaplar(hr.data ?? []) })
      .catch(() => showError('Parametreler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const set = <K extends keyof MuhasebeParametre>(k: K, v: MuhasebeParametre[K]) =>
    setP((prev) => (prev ? { ...prev, [k]: v } : prev))

  const save = async () => {
    if (!p) return
    setSaving(true)
    try {
      const { id: _id, ...rest } = p
      await muhasebeApi.updateParametre(rest)
      showSuccess('Parametreler kaydedildi.')
    } catch {} finally { setSaving(false) }
  }

  if (loading || !p) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  return (
    <div className="flex flex-col h-full max-w-3xl">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Muhasebe Parametreleri</h1>
        <Button size="sm" onClick={save} disabled={saving}>
          <Save className="h-4 w-4 mr-1" /> {saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>

      <div className="space-y-6">
        <section className="space-y-3">
          <h2 className="text-sm font-semibold text-muted-foreground">Varsayılan Hesaplar</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            <div className="space-y-1">
              <Label className="text-xs">Kasa Hesabı</Label>
              <HesapSelect value={p.varsayilanKasaHesapId ?? ''} onChange={(v) => set('varsayilanKasaHesapId', v || null)} hesaplar={hesaplar} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Banka Hesabı</Label>
              <HesapSelect value={p.varsayilanBankaHesapId ?? ''} onChange={(v) => set('varsayilanBankaHesapId', v || null)} hesaplar={hesaplar} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Aidat Gelir Hesabı</Label>
              <HesapSelect value={p.aidatGelirHesapId ?? ''} onChange={(v) => set('aidatGelirHesapId', v || null)} hesaplar={hesaplar} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Gecikme Faizi Hesabı</Label>
              <HesapSelect value={p.gecikmeFaiziHesapId ?? ''} onChange={(v) => set('gecikmeFaiziHesapId', v || null)} hesaplar={hesaplar} />
            </div>
          </div>
        </section>

        <section className="space-y-3">
          <h2 className="text-sm font-semibold text-muted-foreground">Ana Hesap Kodları & Şablonlar</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div className="space-y-1">
              <Label className="text-xs">Alıcılar Ana Hesap</Label>
              <Input value={p.alicilarAnaHesapKodu} onChange={(e) => set('alicilarAnaHesapKodu', e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Satıcılar Ana Hesap</Label>
              <Input value={p.saticilarAnaHesapKodu} onChange={(e) => set('saticilarAnaHesapKodu', e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Gider Ana Hesap</Label>
              <Input value={p.giderAnaHesapKodu} onChange={(e) => set('giderAnaHesapKodu', e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Cari Kod Şablonu</Label>
              <Input value={p.cariKodSablonu} onChange={(e) => set('cariKodSablonu', e.target.value)} className="font-mono" />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Fiş No Şablonu</Label>
              <Input value={p.fisNoSablonu} onChange={(e) => set('fisNoSablonu', e.target.value)} className="font-mono" />
            </div>
          </div>
        </section>

        <section className="space-y-3">
          <h2 className="text-sm font-semibold text-muted-foreground">Genel</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div className="space-y-1">
              <Label className="text-xs">Para Birimi</Label>
              <Input value={p.paraBirimi} onChange={(e) => set('paraBirimi', e.target.value)} />
            </div>
            <div className="space-y-1">
              <Label className="text-xs">KDV Oranı (%)</Label>
              <Input
                type="number" step="0.01"
                value={p.kdvOrani ?? ''}
                onChange={(e) => set('kdvOrani', e.target.value === '' ? null : Number(e.target.value))}
              />
            </div>
          </div>
          <label className="flex items-center gap-2 cursor-pointer">
            <input type="checkbox" checked={p.otomatikTahsilFisi} onChange={(e) => set('otomatikTahsilFisi', e.target.checked)} className="rounded" />
            <span className="text-sm">Ödeme alındığında otomatik <b>Tahsil</b> fişi oluştur</span>
          </label>
          <label className="flex items-center gap-2 cursor-pointer">
            <input type="checkbox" checked={p.otomatikTediyeFisi} onChange={(e) => set('otomatikTediyeFisi', e.target.checked)} className="rounded" />
            <span className="text-sm">Gider kaydında otomatik <b>Tediye</b> fişi oluştur</span>
          </label>
        </section>
      </div>
    </div>
  )
}
