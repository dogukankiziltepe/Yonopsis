'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { kasaTransferleriApi } from '@/lib/api/finans'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import type { CreateKasaTransferDto } from '@/types/finans'
import type { KasaBanka } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'

export default function KasaTransferDetayPage() {
  const router = useRouter()
  const params = useParams<{ id: string }>()
  const id = params.id
  const isNew = id === 'yeni'

  const [kasalar, setKasalar] = useState<KasaBanka[]>([])
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState(false)

  const [evrakNo, setEvrakNo] = useState('')
  const [tarih, setTarih] = useState(new Date().toISOString().split('T')[0])
  const [belgeNo, setBelgeNo] = useState('')
  const [cikisId, setCikisId] = useState('')
  const [girisId, setGirisId] = useState('')
  const [tutar, setTutar] = useState('')
  const [aciklama, setAciklama] = useState('')

  useEffect(() => { kasaBankaApi.getAll().then(r => setKasalar(r.data.filter(k => k.isActive))).catch(showApiError) }, [])

  useEffect(() => {
    if (isNew) return
    kasaTransferleriApi.getById(id)
      .then(r => {
        const d = r.data
        setEvrakNo(d.evrakNo); setTarih(d.tarih.split('T')[0]); setBelgeNo(d.belgeNo ?? '')
        setCikisId(d.cikisKasaBankaId); setGirisId(d.girisKasaBankaId)
        setTutar(String(d.tutar)); setAciklama(d.aciklama ?? '')
      })
      .catch(showApiError)
      .finally(() => setLoading(false))
  }, [id, isNew])

  const handleCikisChange = (value: string) => {
    setCikisId(value)
    if (value && value === girisId) setGirisId('')
  }

  const handleSave = async () => {
    if (!tarih) { showApiError('Tarih zorunludur.'); return }
    if (!cikisId) { showApiError('Çıkış kasası seçilmelidir.'); return }
    if (!girisId) { showApiError('Giriş kasası seçilmelidir.'); return }
    if (cikisId === girisId) { showApiError('Çıkış ve giriş kasası aynı olamaz.'); return }
    if (!tutar || parseFloat(tutar) <= 0) { showApiError('Tutar sıfırdan büyük olmalıdır.'); return }
    setSaving(true)
    try {
      const dto: CreateKasaTransferDto = {
        tarih, belgeNo: belgeNo || undefined, cikisKasaBankaId: cikisId, girisKasaBankaId: girisId,
        tutar: parseFloat(tutar), aciklama: aciklama || undefined,
      }
      if (isNew) await kasaTransferleriApi.create(dto)
      else await kasaTransferleriApi.update(id, dto)
      showSuccess(isNew ? 'Kasa transfer fişi oluşturuldu.' : 'Kasa transfer fişi güncellendi.')
      router.push('/finans/kasa-transfer')
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    try {
      await kasaTransferleriApi.delete(id)
      showSuccess('Kasa transfer fişi silindi.')
      router.push('/finans/kasa-transfer')
    } catch (e) { showApiError(e) }
  }

  if (loading) return <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>

  return (
    <div className="flex flex-col gap-4 max-w-xl">
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => router.push('/finans/kasa-transfer')}><ArrowLeft className="h-4 w-4" /></Button>
        <h1 className="text-xl font-semibold">{isNew ? 'Yeni Kasa Transfer Fişi' : 'Kasa Transfer Fişi'}</h1>
        {!isNew && <span className="font-mono text-xs text-muted-foreground ml-2">{evrakNo}</span>}
      </div>

      <div className="border rounded-lg p-4 space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <div className="space-y-1.5">
            <Label htmlFor="tarih">Tarih <span className="text-destructive">*</span></Label>
            <Input id="tarih" type="date" value={tarih} onChange={e => setTarih(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="belgeNo">Belge No</Label>
            <Input id="belgeNo" value={belgeNo} onChange={e => setBelgeNo(e.target.value)} maxLength={50} />
          </div>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="cikis">Çıkış Kasası <span className="text-destructive">*</span></Label>
          <select id="cikis" value={cikisId} onChange={e => handleCikisChange(e.target.value)} className={selectClass}>
            <option value="">— Seçilmedi —</option>
            {kasalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="giris">Giriş Kasası <span className="text-destructive">*</span></Label>
          <select id="giris" value={girisId} onChange={e => setGirisId(e.target.value)} className={selectClass}>
            <option value="">— Seçilmedi —</option>
            {kasalar.filter(k => k.id !== cikisId).map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="tutar">Tutar (₺)</Label>
          <Input id="tutar" type="number" min={0} step="0.01" value={tutar} onChange={e => setTutar(e.target.value)} placeholder="0.00" />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="aciklama">Açıklama</Label>
          <textarea id="aciklama" value={aciklama} onChange={e => setAciklama(e.target.value)} maxLength={500} rows={3}
            className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
        </div>
      </div>

      <div className="flex items-center justify-between">
        <div>
          {!isNew && (deleteConfirm
            ? <div className="flex gap-2">
                <Button variant="destructive" onClick={handleDelete}>Silmeyi Onayla</Button>
                <Button variant="ghost" onClick={() => setDeleteConfirm(false)}>Vazgeç</Button>
              </div>
            : <Button variant="ghost" className="text-destructive" onClick={() => setDeleteConfirm(true)}>Sil</Button>)}
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => router.push('/finans/kasa-transfer')}>İptal</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
        </div>
      </div>
    </div>
  )
}
