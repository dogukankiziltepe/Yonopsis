'use client'

import { useRef, useState } from 'react'
import { Download, X, CheckCircle2, AlertTriangle } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { bankaHareketleriApi } from '@/lib/api/finans'
import type { BankaHareketiImportPreview, BankaHareketiImportConfirmItem } from '@/types/finans'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

interface Props {
  kasaBankaId: string
  kasaBankaAdi?: string
  open: boolean
  onClose: () => void
  onImported?: () => void
}

export function BankaExcelImportWizard({ kasaBankaId, kasaBankaAdi, open, onClose, onImported }: Props) {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [templateDownloading, setTemplateDownloading] = useState(false)
  const [previewLoading, setPreviewLoading] = useState(false)
  const [preview, setPreview] = useState<BankaHareketiImportPreview | null>(null)
  const [confirming, setConfirming] = useState(false)
  const [sonuc, setSonuc] = useState<{ olusturulanSayisi: number } | null>(null)

  if (!open) return null

  const reset = () => { setPreview(null); setSonuc(null) }
  const handleClose = () => { reset(); onClose() }

  const handleDownloadTemplate = async () => {
    setTemplateDownloading(true)
    try {
      const res = await bankaHareketleriApi.excelImport.downloadTemplate()
      const url = window.URL.createObjectURL(new Blob([res.data as BlobPart]))
      const a = document.createElement('a')
      a.href = url
      a.download = 'banka_hareketleri_sablonu.xlsx'
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
      const res = await bankaHareketleriApi.excelImport.preview(file)
      setPreview(res.data)
    } catch (err) { showApiError(err) }
    finally { setPreviewLoading(false); if (fileInputRef.current) fileInputRef.current.value = '' }
  }

  const handleConfirm = async () => {
    if (!preview) return
    const validItems = preview.satirlar.filter(s => s.isValid)
    if (validItems.length === 0) return
    setConfirming(true)
    try {
      const items: BankaHareketiImportConfirmItem[] = validItems.map(s => ({
        tarih: s.tarih!, aciklama: s.aciklama!, referansNo: s.referansNo, tutar: s.tutar!,
      }))
      const res = await bankaHareketleriApi.excelImport.confirm(kasaBankaId, { items })
      setSonuc({ olusturulanSayisi: res.data.olusturulanSayisi })
      showSuccess(`${res.data.olusturulanSayisi} banka hareketi eklendi.`)
      onImported?.()
    } catch (e) { showApiError(e) }
    finally { setConfirming(false) }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/30" onClick={handleClose} />
      <div className="relative w-full max-w-2xl bg-background border rounded-lg shadow-xl max-h-[85vh] flex flex-col">
        <div className="flex items-center justify-between px-4 py-3 border-b">
          <h2 className="font-semibold">Excel ile Banka Hareketleri Yükleme{kasaBankaAdi ? ` — ${kasaBankaAdi}` : ''}</h2>
          <Button variant="ghost" size="icon" onClick={handleClose}><X className="h-4 w-4" /></Button>
        </div>

        <div className="flex-1 overflow-y-auto p-4 space-y-4">
          {!preview && !sonuc && (
            <>
              <p className="text-sm text-muted-foreground">
                Önce şablonu indirin, Excel&apos;de Tarih/Açıklama/Referans No/Tutar bilgilerini girin, ardından dosyayı yükleyin.
              </p>
              <Button variant="outline" onClick={handleDownloadTemplate} disabled={templateDownloading}>
                <Download className="h-4 w-4 mr-1" />{templateDownloading ? 'İndiriliyor...' : 'Excel Şablonu İndir'}
              </Button>
              <div className="space-y-1.5">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".xlsx"
                  onChange={handleFileSelected}
                  disabled={previewLoading}
                  className="block w-full text-sm border rounded-md file:mr-3 file:py-2 file:px-3 file:border-0 file:bg-muted file:text-sm"
                />
                {previewLoading && <p className="text-xs text-muted-foreground">Dosya işleniyor...</p>}
              </div>
            </>
          )}

          {preview && !sonuc && (
            <>
              <div className="flex gap-4 text-sm">
                <span>Toplam satır: <strong>{preview.toplamSatir}</strong></span>
                <span>Geçerli: <strong>{preview.gecerliSatir}</strong></span>
              </div>
              <div className="border rounded-md overflow-auto max-h-80">
                <table className="w-full text-sm">
                  <thead className="bg-muted/50 border-b sticky top-0">
                    <tr>
                      <th className="text-left px-3 py-2 font-medium">Tarih</th>
                      <th className="text-left px-3 py-2 font-medium">Açıklama</th>
                      <th className="text-right px-3 py-2 font-medium">Tutar</th>
                      <th className="text-left px-3 py-2 font-medium">Hata</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y">
                    {preview.satirlar.map((s, idx) => (
                      <tr key={idx} className={!s.isValid ? 'bg-destructive/5' : ''}>
                        <td className="px-3 py-2">{s.tarih ? new Date(s.tarih).toLocaleDateString('tr-TR') : '—'}</td>
                        <td className="px-3 py-2">{s.aciklama ?? '—'}</td>
                        <td className="px-3 py-2 text-right">{s.tutar != null ? fmt(s.tutar) : '—'}</td>
                        <td className="px-3 py-2 text-xs text-destructive">
                          {s.hatalar.length > 0 && <span className="flex items-center gap-1"><AlertTriangle className="h-3.5 w-3.5" />{s.hatalar.join(' ')}</span>}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}

          {sonuc && (
            <div className="text-center py-8 space-y-3">
              <CheckCircle2 className="h-10 w-10 mx-auto text-green-600" />
              <p className="font-medium">{sonuc.olusturulanSayisi} banka hareketi eklendi.</p>
            </div>
          )}
        </div>

        <div className="border-t px-4 py-3 flex gap-2 justify-end">
          {preview && !sonuc && (
            <>
              <Button variant="outline" onClick={reset}>Geri</Button>
              <Button onClick={handleConfirm} disabled={confirming || preview.gecerliSatir === 0}>
                {confirming ? 'Kaydediliyor...' : `Onayla (${preview.gecerliSatir})`}
              </Button>
            </>
          )}
          {sonuc && <Button onClick={handleClose}>Kapat</Button>}
          {!preview && !sonuc && <Button variant="outline" onClick={handleClose}>İptal</Button>}
        </div>
      </div>
    </div>
  )
}
