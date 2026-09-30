'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import Link from 'next/link'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { icraDosyalariApi } from '@/lib/api/icra'
import { IcraDurumu, IcraDurumuLabel } from '@/types/icra'
import type { IcraDosyasiDetay, AvukatSecim } from '@/types/icra'
import { IcraEvrakTablosu } from '@/components/finans/IcraEvrakTablosu'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'

export default function IcraDosyasiDetayPage() {
  const router = useRouter()
  const { id } = useParams<{ id: string }>()

  const [detay, setDetay] = useState<IcraDosyasiDetay | null>(null)
  const [avukatlar, setAvukatlar] = useState<AvukatSecim[]>([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState(false)

  const [dosyaNo, setDosyaNo] = useState('')
  const [icraTarihi, setIcraTarihi] = useState('')
  const [durum, setDurum] = useState<IcraDurumu>(IcraDurumu.Icrada)
  const [avukatId, setAvukatId] = useState('')
  const [aciklama, setAciklama] = useState('')

  useEffect(() => {
    icraDosyalariApi.getAvukatlar().then(r => setAvukatlar(r.data)).catch(showApiError)
    icraDosyalariApi.getById(id)
      .then(r => {
        const d = r.data
        setDetay(d)
        setDosyaNo(d.dosyaNo); setIcraTarihi(d.icraTarihi.split('T')[0]); setDurum(d.durum)
        setAvukatId(d.avukatId ?? ''); setAciklama(d.aciklama ?? '')
      })
      .catch(showApiError)
      .finally(() => setLoading(false))
  }, [id])

  const handleSave = async () => {
    if (!dosyaNo.trim()) { showApiError('Dosya numarası zorunludur.'); return }
    if (!icraTarihi) { showApiError('İcra tarihi zorunludur.'); return }
    setSaving(true)
    try {
      await icraDosyalariApi.update(id, { dosyaNo, icraTarihi, durum, avukatId: avukatId || undefined, aciklama: aciklama || undefined })
      showSuccess('İcra dosyası güncellendi.')
      router.push('/finans/icra/icra-listesi')
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    try {
      await icraDosyalariApi.delete(id)
      showSuccess('İcra dosyası silindi; takip "İcraya Verilecek" durumuna döndü.')
      router.push('/finans/icra/icra-listesi')
    } catch (e) { showApiError(e) }
  }

  if (loading) return <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
  if (!detay) return <div className="p-8 text-center text-sm text-muted-foreground">İcra dosyası bulunamadı.</div>

  // Pasif yapılmış avukat seçiliyse listede yine görünsün
  const avukatSecenekleri = detay.avukatId && !avukatlar.some(a => a.id === detay.avukatId)
    ? [{ id: detay.avukatId, adSoyad: detay.avukatAdi ?? '—' }, ...avukatlar]
    : avukatlar

  return (
    <div className="flex flex-col gap-4 max-w-5xl">
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => router.push('/finans/icra/icra-listesi')}><ArrowLeft className="h-4 w-4" /></Button>
        <h1 className="text-xl font-semibold">İcra Dosyası</h1>
        <span className="font-mono text-xs text-muted-foreground ml-2">{detay.dosyaNo}</span>
      </div>

      <section className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="space-y-1.5">
          <Label htmlFor="dosyaNo">Dosya No <span className="text-destructive">*</span></Label>
          <Input id="dosyaNo" value={dosyaNo} onChange={e => setDosyaNo(e.target.value)} maxLength={100} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="icraTarihi">İcra Tarihi <span className="text-destructive">*</span></Label>
          <Input id="icraTarihi" type="date" value={icraTarihi} onChange={e => setIcraTarihi(e.target.value)} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="durum">Durumu</Label>
          <select id="durum" className={selectClass} value={durum} onChange={e => setDurum(Number(e.target.value) as IcraDurumu)}>
            {Object.entries(IcraDurumuLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label>Kişi</Label>
          <div className="border rounded-md px-3 py-2 text-sm bg-muted/30">
            {detay.borcluAdi} <span className="text-xs text-muted-foreground">· {detay.blokAdi ? `${detay.blokAdi} / ` : ''}{detay.doorNumber}</span>
          </div>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="avukat">Avukat</Label>
          <select id="avukat" className={selectClass} value={avukatId} onChange={e => setAvukatId(e.target.value)}>
            <option value="">— Seçilmedi —</option>
            {avukatSecenekleri.map(a => <option key={a.id} value={a.id}>{a.adSoyad}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label>Tutarlar</Label>
          <div className="border rounded-md px-3 py-2 text-sm bg-muted/30 flex justify-between gap-2">
            <span>Dosya: <strong>{fmt(detay.dosyaTutari)}</strong></span>
            <span>Bakiye: <strong>{fmt(detay.bakiye)}</strong></span>
          </div>
        </div>
        <div className="space-y-1.5 sm:col-span-3">
          <Label htmlFor="aciklama">Açıklama</Label>
          <textarea id="aciklama" value={aciklama} onChange={e => setAciklama(e.target.value)} maxLength={1000} rows={2}
            className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
        </div>
        {detay.takipId && (
          <p className="sm:col-span-3 text-xs text-muted-foreground">
            Kaynak takip: <Link className="underline" href={`/finans/icra/takip-listesi/${detay.takipId}`}>Takip detayına git</Link>
          </p>
        )}
      </section>

      <div className="flex items-center justify-between">
        <div>
          {deleteConfirm
            ? <div className="flex gap-2">
                <Button variant="destructive" onClick={handleDelete}>Silmeyi Onayla</Button>
                <Button variant="ghost" onClick={() => setDeleteConfirm(false)}>Vazgeç</Button>
              </div>
            : <Button variant="ghost" className="text-destructive" onClick={() => setDeleteConfirm(true)}>Sil</Button>}
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => router.push('/finans/icra/icra-listesi')}>İptal</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
        </div>
      </div>

      <IcraEvrakTablosu baslik="İcraya Verilen Evrak" evraklar={detay.evraklar} dosyaAdi={`icra-${detay.dosyaNo}.xlsx`} />
    </div>
  )
}
