'use client'

import { useEffect, useRef, useState } from 'react'
import { Download, Upload, ArrowLeft, ArrowRight, CheckCircle2, AlertTriangle } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi } from '@/lib/api/finans'
import type { GelirTanimiSecim, TopluBorclandirmaPreview, TopluBorclandirmaConfirmItem } from '@/types/finans'
import type { UserType } from '@/types/person'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function TopluBorclandirmaPage() {
  const [step, setStep] = useState<1 | 2 | 3>(1)
  const [gelirTanimlari, setGelirTanimlari] = useState<GelirTanimiSecim[]>([])
  const [selectedKalemIds, setSelectedKalemIds] = useState<string[]>([])
  const [donem, setDonem] = useState('')
  const [sonOdemeTarihi, setSonOdemeTarihi] = useState('')
  const [borcluRolTercihi, setBorcluRolTercihi] = useState<UserType>(3) // varsayılan: Kiracı

  const [templateDownloading, setTemplateDownloading] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [previewLoading, setPreviewLoading] = useState(false)
  const [preview, setPreview] = useState<TopluBorclandirmaPreview | null>(null)
  const [confirming, setConfirming] = useState(false)
  const [sonuc, setSonuc] = useState<{ olusturulanSayisi: number } | null>(null)

  useEffect(() => {
    borcMakbuzlariApi.topluBorclandirma.getAktifGelirTanimlari().then(r => setGelirTanimlari(r.data)).catch(showApiError)
  }, [])

  const toggleKalem = (id: string) => {
    setSelectedKalemIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id])
  }

  const goToStep2 = () => {
    if (selectedKalemIds.length === 0) { showApiError('En az bir kalem seçilmelidir.'); return }
    if (!donem) { showApiError('Dönem seçilmelidir.'); return }
    setStep(2)
  }

  const handleDownloadTemplate = async () => {
    setTemplateDownloading(true)
    try {
      const res = await borcMakbuzlariApi.topluBorclandirma.downloadTemplate({
        gelirTanimiIds: selectedKalemIds,
        borcluRolTercihi,
        donem: donem || undefined,
        sonOdemeTarihi: sonOdemeTarihi || undefined,
      })
      const url = window.URL.createObjectURL(new Blob([res.data as BlobPart]))
      const a = document.createElement('a')
      a.href = url
      a.download = 'toplu_borclandirma_sablonu.xlsx'
      a.click()
      window.URL.revokeObjectURL(url)
    } catch (e) { showApiError(e) }
    finally { setTemplateDownloading(false) }
  }

  const handleFileSelected = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setPreviewLoading(true)
    setPreview(null)
    try {
      const res = await borcMakbuzlariApi.topluBorclandirma.preview(file)
      setPreview(res.data)
      setStep(3)
    } catch (err) { showApiError(err) }
    finally { setPreviewLoading(false); if (fileInputRef.current) fileInputRef.current.value = '' }
  }

  const handleConfirm = async () => {
    if (!preview || preview.items.length === 0) return
    setConfirming(true)
    try {
      const items: TopluBorclandirmaConfirmItem[] = preview.items.map(i => ({
        unitId: i.unitId,
        gelirTanimiId: i.gelirTanimiId,
        tutar: i.tutar,
        aciklama: i.aciklama,
        borcluUserId: i.borcluUserId,
        borcluRol: i.borcluRol,
      }))
      const res = await borcMakbuzlariApi.topluBorclandirma.confirm({
        donem: preview.donem, sonOdemeTarihi: preview.sonOdemeTarihi, items,
      })
      setSonuc({ olusturulanSayisi: res.data.olusturulanSayisi })
      showSuccess(`${res.data.olusturulanSayisi} borç makbuzu oluşturuldu.`)
    } catch (e) { showApiError(e) }
    finally { setConfirming(false) }
  }

  const restart = () => {
    setStep(1); setSelectedKalemIds([]); setDonem(''); setSonOdemeTarihi(''); setPreview(null); setSonuc(null)
  }

  return (
    <div className="flex flex-col h-full gap-4 max-w-3xl">
      <h1 className="text-xl font-semibold">Toplu Borçlandırma</h1>

      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        <span className={step === 1 ? 'font-semibold text-foreground' : ''}>1. Kalem &amp; Parametreler</span>
        <span>→</span>
        <span className={step === 2 ? 'font-semibold text-foreground' : ''}>2. Şablon İndir / Yükle</span>
        <span>→</span>
        <span className={step === 3 ? 'font-semibold text-foreground' : ''}>3. Önizle &amp; Onayla</span>
      </div>

      {step === 1 && (
        <div className="border rounded-lg p-4 space-y-4">
          <div className="space-y-1.5">
            <Label>Kalemler (Gelir Tanımı) <span className="text-destructive">*</span></Label>
            <div className="flex flex-col gap-2 border rounded-md p-3 max-h-56 overflow-y-auto">
              {gelirTanimlari.length === 0 && <p className="text-sm text-muted-foreground">Aktif kalem bulunamadı.</p>}
              {gelirTanimlari.map(g => (
                <label key={g.id} className="flex items-center gap-2 text-sm cursor-pointer">
                  <Checkbox checked={selectedKalemIds.includes(g.id)} onCheckedChange={() => toggleKalem(g.id)} />
                  {g.name}
                </label>
              ))}
            </div>
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="donem">Dönem <span className="text-destructive">*</span></Label>
            <Input id="donem" value={donem} onChange={e => setDonem(e.target.value)} placeholder="örn. 2026-07" maxLength={10} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="sonOdeme">Son Ödeme Tarihi</Label>
            <Input id="sonOdeme" type="date" value={sonOdemeTarihi} onChange={e => setSonOdemeTarihi(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <Label>Borçlu Rolü</Label>
            <RadioGroup value={String(borcluRolTercihi)} onValueChange={v => setBorcluRolTercihi(Number(v) as UserType)} className="flex gap-4">
              <label className="flex items-center gap-2 text-sm cursor-pointer">
                <RadioGroupItem value="3" /> Kiracı (varsa), yoksa ev sahibi
              </label>
              <label className="flex items-center gap-2 text-sm cursor-pointer">
                <RadioGroupItem value="2" /> Ev Sahibi, yoksa kiracı
              </label>
            </RadioGroup>
          </div>
          <div className="flex justify-end">
            <Button onClick={goToStep2}>Devam Et <ArrowRight className="h-4 w-4 ml-1" /></Button>
          </div>
        </div>
      )}

      {step === 2 && (
        <div className="border rounded-lg p-4 space-y-4">
          <p className="text-sm text-muted-foreground">
            Önce şablonu indirin, Excel&apos;de ilgili dairelere tutar girin, ardından doldurulmuş dosyayı yükleyin.
          </p>
          <Button variant="outline" onClick={handleDownloadTemplate} disabled={templateDownloading}>
            <Download className="h-4 w-4 mr-1" />{templateDownloading ? 'İndiriliyor...' : 'Excel Şablonu İndir'}
          </Button>
          <div className="space-y-1.5">
            <Label htmlFor="file">Doldurulmuş Dosyayı Yükle</Label>
            <input
              ref={fileInputRef}
              id="file"
              type="file"
              accept=".xlsx"
              onChange={handleFileSelected}
              disabled={previewLoading}
              className="block w-full text-sm border rounded-md file:mr-3 file:py-2 file:px-3 file:border-0 file:bg-muted file:text-sm"
            />
            {previewLoading && <p className="text-xs text-muted-foreground">Dosya işleniyor...</p>}
          </div>
          <div className="flex justify-between">
            <Button variant="outline" onClick={() => setStep(1)}><ArrowLeft className="h-4 w-4 mr-1" />Geri</Button>
          </div>
        </div>
      )}

      {step === 3 && preview && !sonuc && (
        <div className="border rounded-lg p-4 space-y-4">
          {preview.satirHatalari.length > 0 && (
            <div className="border border-destructive/40 bg-destructive/5 rounded-md p-3 text-sm space-y-1">
              {preview.satirHatalari.map((e, i) => <p key={i} className="flex gap-1.5"><AlertTriangle className="h-4 w-4 shrink-0 text-destructive" />{e}</p>)}
            </div>
          )}
          <div className="flex gap-4 text-sm">
            <span>Toplam kombinasyon: <strong>{preview.toplamKombinasyon}</strong></span>
            <span>Toplam tutar: <strong>{fmt(preview.toplamTutar)}</strong></span>
            {preview.mukerrerSayisi > 0 && <span className="text-amber-600">Mükerrer: <strong>{preview.mukerrerSayisi}</strong></span>}
          </div>
          <div className="border rounded-md overflow-auto max-h-96">
            <table className="w-full text-sm">
              <thead className="bg-muted/50 border-b sticky top-0">
                <tr>
                  <th className="text-left px-3 py-2 font-medium">Daire</th>
                  <th className="text-left px-3 py-2 font-medium">Kalem</th>
                  <th className="text-left px-3 py-2 font-medium">Borçlu</th>
                  <th className="text-right px-3 py-2 font-medium">Tutar</th>
                  <th className="text-left px-3 py-2 font-medium">Uyarı</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {preview.items.map((item, idx) => (
                  <tr key={idx} className={item.uyarilar.length > 0 ? 'bg-amber-50' : ''}>
                    <td className="px-3 py-2">{item.buildingName ? `${item.buildingName} / ` : ''}{item.unitDoorNumber}</td>
                    <td className="px-3 py-2">{item.gelirTanimiAdi}</td>
                    <td className="px-3 py-2">{item.borcluAdSoyad ?? '—'}</td>
                    <td className="px-3 py-2 text-right">{fmt(item.tutar)}</td>
                    <td className="px-3 py-2 text-xs text-amber-700">
                      {item.mukerrer && <Badge variant="secondary" className="mr-1">Mükerrer</Badge>}
                      {item.uyarilar.join(' ')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="flex justify-between">
            <Button variant="outline" onClick={() => setStep(2)}><ArrowLeft className="h-4 w-4 mr-1" />Geri</Button>
            <Button onClick={handleConfirm} disabled={confirming || preview.items.length === 0}>
              {confirming ? 'Kaydediliyor...' : `Onayla (${preview.items.length})`}
            </Button>
          </div>
        </div>
      )}

      {sonuc && (
        <div className="border rounded-lg p-8 text-center space-y-3">
          <CheckCircle2 className="h-10 w-10 mx-auto text-green-600" />
          <p className="font-medium">{sonuc.olusturulanSayisi} borç makbuzu oluşturuldu.</p>
          <div className="flex justify-center gap-2">
            <Button variant="outline" onClick={restart}>Yeni Toplu Borçlandırma</Button>
            <a href="/finans/borc-makbuzu"><Button>Borç Makbuzu Listesine Git</Button></a>
          </div>
        </div>
      )}
    </div>
  )
}
