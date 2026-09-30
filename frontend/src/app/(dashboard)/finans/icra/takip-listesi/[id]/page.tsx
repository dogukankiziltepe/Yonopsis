'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import Link from 'next/link'
import { ArrowLeft, Gavel } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { icraTakipleriApi } from '@/lib/api/icra'
import { TakipDurumu, TakipDurumuLabel } from '@/types/icra'
import type { TakipDetay, AvukatSecim } from '@/types/icra'
import { IcraEvrakTablosu } from '@/components/finans/IcraEvrakTablosu'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'
const bugun = () => new Date().toISOString().split('T')[0]

export default function TakipDetayPage() {
  const router = useRouter()
  const { id } = useParams<{ id: string }>()

  const [detay, setDetay] = useState<TakipDetay | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [takipTarihi, setTakipTarihi] = useState('')
  const [durum, setDurum] = useState<TakipDurumu>(TakipDurumu.Takipte)
  const [aciklama, setAciklama] = useState('')

  // İcraya Ver formu
  const [icraFormu, setIcraFormu] = useState(false)
  const [avukatlar, setAvukatlar] = useState<AvukatSecim[]>([])
  const [dosyaNo, setDosyaNo] = useState('')
  const [icraTarihi, setIcraTarihi] = useState(bugun())
  const [avukatId, setAvukatId] = useState('')
  const [icraAciklama, setIcraAciklama] = useState('')
  const [icrayaVeriliyor, setIcrayaVeriliyor] = useState(false)

  const load = () => icraTakipleriApi.getById(id)
    .then(r => {
      const d = r.data
      setDetay(d)
      setTakipTarihi(d.takipTarihi.split('T')[0])
      setDurum(d.durum)
      setAciklama(d.aciklama ?? '')
    })
    .catch(showApiError)
    .finally(() => setLoading(false))

  useEffect(() => { load() }, [id]) // eslint-disable-line react-hooks/exhaustive-deps

  const acIcraFormu = () => {
    setIcraFormu(true)
    if (avukatlar.length === 0) icraTakipleriApi.getAvukatlar().then(r => setAvukatlar(r.data)).catch(showApiError)
  }

  const handleSave = async () => {
    if (!takipTarihi) { showApiError('Takip tarihi zorunludur.'); return }
    setSaving(true)
    try {
      await icraTakipleriApi.update(id, { takipTarihi, durum, aciklama: aciklama || undefined })
      showSuccess('Takip güncellendi.')
      router.push('/finans/icra/takip-listesi')
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleIcrayaVer = async () => {
    if (!dosyaNo.trim()) { showApiError('Dosya numarası zorunludur.'); return }
    if (!icraTarihi) { showApiError('İcra tarihi zorunludur.'); return }
    setIcrayaVeriliyor(true)
    try {
      const r = await icraTakipleriApi.icrayaVer(id, { dosyaNo, icraTarihi, avukatId: avukatId || undefined, aciklama: icraAciklama || undefined })
      showSuccess('İcra dosyası oluşturuldu.')
      router.push(`/finans/icra/icra-listesi/${r.data.id}`)
    } catch (e) { showApiError(e) }
    finally { setIcrayaVeriliyor(false) }
  }

  if (loading) return <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
  if (!detay) return <div className="p-8 text-center text-sm text-muted-foreground">Takip kaydı bulunamadı.</div>

  const icradaMi = detay.durum === TakipDurumu.IcrayaVerildi
  const icrayaVerilebilir = !icradaMi && detay.durum !== TakipDurumu.Odendi && detay.durum !== TakipDurumu.IptalEdildi
  // "İcraya Verildi" elle seçilemez; yalnızca icraya verilmiş kayıtta gösterilir
  const durumSecenekleri = Object.entries(TakipDurumuLabel)
    .filter(([v]) => icradaMi || Number(v) !== TakipDurumu.IcrayaVerildi)

  return (
    <div className="flex flex-col gap-4 max-w-5xl">
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => router.push('/finans/icra/takip-listesi')}><ArrowLeft className="h-4 w-4" /></Button>
        <h1 className="text-xl font-semibold">Takip Detayı</h1>
      </div>

      <section className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="space-y-1.5">
          <Label htmlFor="takipTarihi">Takip Tarihi <span className="text-destructive">*</span></Label>
          <Input id="takipTarihi" type="date" value={takipTarihi} onChange={e => setTakipTarihi(e.target.value)} />
        </div>
        <div className="space-y-1.5">
          <Label>Borçlu</Label>
          <div className="border rounded-md px-3 py-2 text-sm bg-muted/30">
            {detay.borcluAdi} <span className="text-xs text-muted-foreground">· {detay.blokAdi ? `${detay.blokAdi} / ` : ''}{detay.doorNumber}</span>
          </div>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="durum">Durumu</Label>
          <select id="durum" className={selectClass} value={durum} disabled={icradaMi}
            onChange={e => setDurum(Number(e.target.value) as TakipDurumu)}>
            {durumSecenekleri.map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          {icradaMi && detay.icraDosyasiId && (
            <p className="text-xs text-muted-foreground">
              İcra dosyası: <Link className="underline" href={`/finans/icra/icra-listesi/${detay.icraDosyasiId}`}>{detay.icraDosyaNo}</Link>
            </p>
          )}
        </div>
        <div className="space-y-1.5 sm:col-span-3">
          <Label htmlFor="aciklama">Açıklama</Label>
          <textarea id="aciklama" value={aciklama} onChange={e => setAciklama(e.target.value)} maxLength={1000} rows={2}
            className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
        </div>
        <div className="sm:col-span-3 flex gap-6 text-sm">
          <span>Takip Başlangıç Tutarı: <strong>{fmt(detay.baslangicTutari)}</strong></span>
          <span>Mevcut Tutar: <strong>{fmt(detay.mevcutTutar)}</strong></span>
        </div>
      </section>

      <div className="flex items-center justify-between">
        <div>
          {icrayaVerilebilir && !icraFormu && (
            <Button variant="outline" onClick={acIcraFormu}><Gavel className="h-4 w-4 mr-1" />İcraya Ver</Button>
          )}
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => router.push('/finans/icra/takip-listesi')}>İptal</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
        </div>
      </div>

      {icraFormu && (
        <section className="border border-amber-300 rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4 bg-amber-50/40 dark:bg-amber-950/10">
          <h2 className="sm:col-span-3 font-medium">İcraya Ver</h2>
          <div className="space-y-1.5">
            <Label htmlFor="dosyaNo">Dosya No <span className="text-destructive">*</span></Label>
            <Input id="dosyaNo" value={dosyaNo} onChange={e => setDosyaNo(e.target.value)} maxLength={100} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="icraTarihi">İcra Tarihi <span className="text-destructive">*</span></Label>
            <Input id="icraTarihi" type="date" value={icraTarihi} onChange={e => setIcraTarihi(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="avukat">Avukat</Label>
            <select id="avukat" className={selectClass} value={avukatId} onChange={e => setAvukatId(e.target.value)}>
              <option value="">— Seçilmedi —</option>
              {avukatlar.map(a => <option key={a.id} value={a.id}>{a.adSoyad}</option>)}
            </select>
          </div>
          <div className="space-y-1.5 sm:col-span-3">
            <Label htmlFor="icraAciklama">Açıklama</Label>
            <Input id="icraAciklama" value={icraAciklama} onChange={e => setIcraAciklama(e.target.value)} maxLength={1000} />
          </div>
          <p className="sm:col-span-3 text-xs text-muted-foreground">Takipteki, kalanı olan borç evrakları icra dosyasına bağlanır ve takip durumu &quot;İcraya Verildi&quot; olur.</p>
          <div className="sm:col-span-3 flex justify-end gap-2">
            <Button variant="ghost" onClick={() => setIcraFormu(false)}>Vazgeç</Button>
            <Button onClick={handleIcrayaVer} disabled={icrayaVeriliyor}>{icrayaVeriliyor ? 'Oluşturuluyor...' : 'İcra Dosyası Oluştur'}</Button>
          </div>
        </section>
      )}

      <IcraEvrakTablosu baslik="Takipteki Evraklar" evraklar={detay.evraklar} dosyaAdi={`takip-evraklari-${detay.doorNumber}.xlsx`} />
    </div>
  )
}
