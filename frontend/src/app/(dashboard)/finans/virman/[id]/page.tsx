'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft, Plus, Trash2, AlertTriangle } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { hesaplarArasiVirmanApi } from '@/lib/api/finans'
import { gelirTanimlariApi } from '@/lib/api/tanimlar'
import { unitsApi } from '@/lib/api/units'
import { VirmanHesapPicker } from '@/components/finans/VirmanHesapPicker'
import { VirmanHesapTuru, VirmanHesapTuruLabel } from '@/types/finans'
import type { SaveVirmanDto } from '@/types/finans'
import type { GelirTanimi } from '@/types/tanimlar'
import type { UnitSummary } from '@/types/unit'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const cellInput = 'w-full border rounded px-2 py-1 text-xs bg-background focus:outline-none focus:ring-1 focus:ring-ring'

interface Satir {
  key: string
  id?: string
  hesapTuru: VirmanHesapTuru
  hesapId: string
  hesapAdi: string
  unitId: string
  gelirTanimiId: string
  aciklama: string
  borc: string
  alacak: string
}

let keySeq = 0
const yeniSatir = (aciklama = ''): Satir => ({
  key: `s${++keySeq}`, hesapTuru: VirmanHesapTuru.Kisi, hesapId: '', hesapAdi: '', unitId: '', gelirTanimiId: '', aciklama, borc: '', alacak: '',
})
const num = (s: string) => parseFloat(s.replace(',', '.')) || 0

export default function VirmanDetayPage() {
  const router = useRouter()
  const { id } = useParams<{ id: string }>()
  const isNew = id === 'yeni'

  const [units, setUnits] = useState<UnitSummary[]>([])
  const [kategoriler, setKategoriler] = useState<GelirTanimi[]>([])
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState(false)

  const [evrakNo, setEvrakNo] = useState('')
  const [tarih, setTarih] = useState(new Date().toISOString().split('T')[0])
  const [belgeTarihi, setBelgeTarihi] = useState('')
  const [belgeNo, setBelgeNo] = useState('')
  const [aciklama, setAciklama] = useState('')
  const [satirlar, setSatirlar] = useState<Satir[]>(() => [yeniSatir()])

  useEffect(() => {
    unitsApi.getAll().then(r => setUnits(r.data)).catch(showApiError)
    gelirTanimlariApi.getAll().then(r => setKategoriler(r.data.filter(g => g.isActive))).catch(showApiError)
  }, [])

  useEffect(() => {
    if (isNew) return
    hesaplarArasiVirmanApi.getById(id)
      .then(r => {
        const d = r.data
        setEvrakNo(d.evrakNo); setTarih(d.tarih.split('T')[0])
        setBelgeTarihi(d.belgeTarihi ? d.belgeTarihi.split('T')[0] : ''); setBelgeNo(d.belgeNo ?? ''); setAciklama(d.aciklama ?? '')
        setSatirlar(d.satirlar.map(s => ({
          key: `s${++keySeq}`, id: s.id, hesapTuru: s.hesapTuru, hesapId: s.hesapId, hesapAdi: s.hesapAdi ?? '',
          unitId: s.unitId ?? '', gelirTanimiId: s.gelirTanimiId ?? '', aciklama: s.aciklama ?? '',
          borc: s.borcTutari ? String(s.borcTutari) : '', alacak: s.alacakTutari ? String(s.alacakTutari) : '',
        })))
      })
      .catch(showApiError)
      .finally(() => setLoading(false))
  }, [id, isNew])

  const updateSatir = (key: string, patch: Partial<Satir>) =>
    setSatirlar(prev => prev.map(s => s.key === key ? { ...s, ...patch } : s))
  const removeSatir = (key: string) => setSatirlar(prev => prev.filter(s => s.key !== key))
  const addSatir = () => setSatirlar(prev => [...prev, yeniSatir(prev[prev.length - 1]?.aciklama ?? '')])

  const toplamBorc = satirlar.reduce((a, s) => a + num(s.borc), 0)
  const toplamAlacak = satirlar.reduce((a, s) => a + num(s.alacak), 0)
  const fark = Math.round((toplamBorc - toplamAlacak) * 100) / 100

  const handleSave = async () => {
    if (!tarih) { showApiError('Tarih zorunludur.'); return }
    if (satirlar.length === 0) { showApiError('En az bir satır girilmelidir.'); return }
    for (let i = 0; i < satirlar.length; i++) {
      const s = satirlar[i]
      if (!s.hesapId) { showApiError(`${i + 1}. satırda hesap seçilmelidir.`); return }
      const b = num(s.borc), a = num(s.alacak)
      if (b === 0 && a === 0) { showApiError(`${i + 1}. satırda borç veya alacak tutarı girilmelidir.`); return }
      if (b > 0 && a > 0) { showApiError(`${i + 1}. satırda borç ve alacak aynı anda girilemez.`); return }
    }
    setSaving(true)
    try {
      const dto: SaveVirmanDto = {
        tarih, belgeTarihi: belgeTarihi || undefined, belgeNo: belgeNo || undefined, aciklama: aciklama || undefined,
        satirlar: satirlar.map(s => ({
          id: s.id, hesapTuru: s.hesapTuru, hesapId: s.hesapId,
          unitId: s.unitId || undefined, gelirTanimiId: s.gelirTanimiId || undefined,
          aciklama: s.aciklama || undefined, borcTutari: num(s.borc), alacakTutari: num(s.alacak),
        })),
      }
      if (isNew) await hesaplarArasiVirmanApi.create(dto)
      else await hesaplarArasiVirmanApi.update(id, dto)
      showSuccess(isNew ? 'Virman fişi oluşturuldu.' : 'Virman fişi güncellendi.')
      router.push('/finans/virman')
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    try {
      await hesaplarArasiVirmanApi.delete(id)
      showSuccess('Virman fişi silindi.')
      router.push('/finans/virman')
    } catch (e) { showApiError(e) }
  }

  if (loading) return <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => router.push('/finans/virman')}><ArrowLeft className="h-4 w-4" /></Button>
        <h1 className="text-xl font-semibold">{isNew ? 'Yeni Virman Fişi' : 'Virman Fişi'}</h1>
        {!isNew && <span className="font-mono text-xs text-muted-foreground ml-2">{evrakNo}</span>}
      </div>

      <div className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4 max-w-3xl">
        <div className="space-y-1.5">
          <Label htmlFor="tarih">Tarih <span className="text-destructive">*</span></Label>
          <Input id="tarih" type="date" value={tarih} onChange={e => setTarih(e.target.value)} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="belgeTarihi">Belge Tarihi</Label>
          <Input id="belgeTarihi" type="date" value={belgeTarihi} onChange={e => setBelgeTarihi(e.target.value)} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="belgeNo">Belge No</Label>
          <Input id="belgeNo" value={belgeNo} onChange={e => setBelgeNo(e.target.value)} maxLength={50} />
        </div>
      </div>

      <div className="border rounded-lg overflow-x-auto">
        <table className="w-full text-sm min-w-[1000px]">
          <thead className="bg-muted/50 border-b">
            <tr>
              <th className="text-left px-2 py-2 font-medium w-28">Hesap Türü</th>
              <th className="text-left px-2 py-2 font-medium w-52">Hesap</th>
              <th className="text-left px-2 py-2 font-medium w-32">İlgili Daire</th>
              <th className="text-left px-2 py-2 font-medium w-48">Kategori</th>
              <th className="text-left px-2 py-2 font-medium">Açıklama</th>
              <th className="text-right px-2 py-2 font-medium w-28">Borç Tutarı</th>
              <th className="text-right px-2 py-2 font-medium w-28">Alacak Tutarı</th>
              <th className="w-8" />
            </tr>
          </thead>
          <tbody className="divide-y">
            {satirlar.map(s => (
              <tr key={s.key} className="align-top">
                <td className="px-2 py-1.5">
                  <select className={cellInput} value={s.hesapTuru}
                    onChange={e => updateSatir(s.key, { hesapTuru: Number(e.target.value) as VirmanHesapTuru, hesapId: '', hesapAdi: '' })}>
                    {Object.entries(VirmanHesapTuruLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
                  </select>
                </td>
                <td className="px-2 py-1.5">
                  <VirmanHesapPicker compact hesapTuru={s.hesapTuru} hesapId={s.hesapId} hesapAdi={s.hesapAdi}
                    onChange={(hesapId, hesapAdi) => updateSatir(s.key, { hesapId, hesapAdi })} />
                </td>
                <td className="px-2 py-1.5">
                  <select className={cellInput} value={s.unitId} onChange={e => updateSatir(s.key, { unitId: e.target.value })}>
                    <option value="">—</option>
                    {units.map(u => <option key={u.id} value={u.id}>{u.buildingName ? `${u.buildingName} / ` : ''}{u.doorNumber}</option>)}
                  </select>
                </td>
                <td className="px-2 py-1.5">
                  <select className={cellInput} value={s.gelirTanimiId} onChange={e => updateSatir(s.key, { gelirTanimiId: e.target.value })}>
                    <option value="">—</option>
                    {kategoriler.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                  </select>
                </td>
                <td className="px-2 py-1.5">
                  <textarea className={cellInput} rows={2} value={s.aciklama} maxLength={1000} onChange={e => updateSatir(s.key, { aciklama: e.target.value })} />
                </td>
                <td className="px-2 py-1.5">
                  <input className={`${cellInput} text-right`} type="number" min={0} step="0.01" value={s.borc}
                    onChange={e => updateSatir(s.key, { borc: e.target.value, alacak: e.target.value ? '' : s.alacak })} placeholder="0,00" />
                </td>
                <td className="px-2 py-1.5">
                  <input className={`${cellInput} text-right`} type="number" min={0} step="0.01" value={s.alacak}
                    onChange={e => updateSatir(s.key, { alacak: e.target.value, borc: e.target.value ? '' : s.borc })} placeholder="0,00" />
                </td>
                <td className="px-1 py-1.5 pt-2">
                  <button type="button" onClick={() => removeSatir(s.key)} disabled={satirlar.length === 1}
                    className="text-muted-foreground hover:text-destructive disabled:opacity-30"><Trash2 className="h-4 w-4" /></button>
                </td>
              </tr>
            ))}
          </tbody>
          <tfoot className="border-t bg-muted/30">
            <tr>
              <td colSpan={5} className="px-2 py-2">
                <Button type="button" size="sm" variant="outline" onClick={addSatir}><Plus className="h-4 w-4 mr-1" />Satır Ekle</Button>
              </td>
              <td className="px-2 py-2 text-right font-semibold">{fmt(toplamBorc)}</td>
              <td className="px-2 py-2 text-right font-semibold">{fmt(toplamAlacak)}</td>
              <td />
            </tr>
            <tr>
              <td colSpan={5} className="px-2 py-2 text-right font-medium">Bakiye Farkı</td>
              <td colSpan={2} className={`px-2 py-2 text-right font-semibold ${fark !== 0 ? 'text-amber-600' : ''}`}>
                <span className="inline-flex items-center gap-1">
                  {fark !== 0 && <AlertTriangle className="h-4 w-4" />}{fmt(fark)}
                </span>
              </td>
              <td />
            </tr>
          </tfoot>
        </table>
      </div>
      {fark !== 0 && (
        <p className="text-xs text-amber-600">Borç ve alacak toplamları eşit değil. Kayıt yapılabilir ancak virmanın dengeli olması önerilir.</p>
      )}

      <div className="space-y-1.5 max-w-3xl">
        <Label htmlFor="aciklama">Açıklama</Label>
        <textarea id="aciklama" value={aciklama} onChange={e => setAciklama(e.target.value)} maxLength={1000} rows={3}
          className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
      </div>

      <div className="flex items-center justify-between max-w-3xl">
        <div>
          {!isNew && (deleteConfirm
            ? <div className="flex gap-2">
                <Button variant="destructive" onClick={handleDelete}>Silmeyi Onayla</Button>
                <Button variant="ghost" onClick={() => setDeleteConfirm(false)}>Vazgeç</Button>
              </div>
            : <Button variant="ghost" className="text-destructive" onClick={() => setDeleteConfirm(true)}>Sil</Button>)}
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => router.push('/finans/virman')}>İptal</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
        </div>
      </div>
    </div>
  )
}
