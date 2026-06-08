'use client'

import { useState } from 'react'
import { Download, RefreshCw, CheckCircle2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { ImportTypeSelector } from '@/components/import/ImportTypeSelector'
import { FileUploadZone } from '@/components/import/FileUploadZone'
import { ImportPreviewTable } from '@/components/import/ImportPreviewTable'
import { ImportSummaryBar } from '@/components/import/ImportSummaryBar'
import { ConfirmImportButton } from '@/components/import/ConfirmImportButton'
import { importApi } from '@/lib/api/import'
import {
  ImportType,
  ImportPreviewResult,
  ImportConfirmResult,
  BuildingImportRowData,
  UnitImportRowData,
  UserImportRowData,
} from '@/types/import'

type Step = 'idle' | 'uploading' | 'previewing' | 'confirming' | 'success'

export default function ImportPage() {
  const [importType, setImportType] = useState<ImportType>('buildings')
  const [step, setStep] = useState<Step>('idle')
  const [preview, setPreview] = useState<ImportPreviewResult | null>(null)
  const [confirmResult, setConfirmResult] = useState<ImportConfirmResult | null>(null)
  const [error, setError] = useState<string | null>(null)

  const handleTypeChange = (type: ImportType) => {
    setImportType(type)
    setStep('idle')
    setPreview(null)
    setConfirmResult(null)
    setError(null)
  }

  const handleDownloadTemplate = async () => {
    try {
      const res = await importApi.downloadTemplate(importType)
      const url = URL.createObjectURL(res.data)
      const a = document.createElement('a')
      a.href = url
      a.download = `${importType}_template.xlsx`
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      setError('Şablon indirilirken hata oluştu.')
    }
  }

  const handleFile = async (file: File) => {
    setError(null)
    setStep('uploading')
    try {
      const res = await importApi.preview(importType, file)
      setPreview(res.data)
      setStep('previewing')
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg ?? 'Dosya işlenirken hata oluştu.')
      setStep('idle')
    }
  }

  const handleConfirm = async () => {
    if (!preview) return
    setStep('confirming')
    setError(null)

    const validRows = preview.rows.filter((r) => r.isValid)

    try {
      let res
      if (importType === 'buildings') {
        const rows = validRows.map((r) => r.data as unknown as BuildingImportRowData)
        res = await importApi.confirmBuildings(rows)
      } else if (importType === 'units') {
        const rows = validRows.map((r) => r.data as unknown as UnitImportRowData)
        res = await importApi.confirmUnits(rows)
      } else {
        const rows = validRows.map((r) => r.data as unknown as UserImportRowData)
        res = await importApi.confirmUsers(rows)
      }
      setConfirmResult(res.data)
      setStep('success')
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg ?? 'Kayıt sırasında hata oluştu.')
      setStep('previewing')
    }
  }

  const handleReset = () => {
    setStep('idle')
    setPreview(null)
    setConfirmResult(null)
    setError(null)
  }

  return (
    <div className="space-y-6 max-w-5xl">
      <div>
        <h1 className="text-2xl font-semibold">Toplu Veri Girişi</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Excel şablonunu indirin, doldurun ve sisteme yükleyin.
        </p>
      </div>

      {/* Step 1: Type selector */}
      <div className="space-y-3">
        <h2 className="text-sm font-medium text-muted-foreground uppercase tracking-wide">1. Veri Tipi Seçin</h2>
        <ImportTypeSelector value={importType} onChange={handleTypeChange} />
      </div>

      {/* Step 2: Download template */}
      <div className="space-y-3">
        <h2 className="text-sm font-medium text-muted-foreground uppercase tracking-wide">2. Şablonu İndirin</h2>
        <Button variant="outline" onClick={handleDownloadTemplate} className="gap-2">
          <Download className="h-4 w-4" />
          {importType === 'buildings' ? 'Bina' : importType === 'units' ? 'Daire' : 'Kullanıcı'} Şablonunu İndir
        </Button>
      </div>

      {/* Step 3: Upload */}
      {step !== 'success' && (
        <div className="space-y-3">
          <h2 className="text-sm font-medium text-muted-foreground uppercase tracking-wide">3. Doldurulmuş Dosyayı Yükleyin</h2>
          <FileUploadZone onFile={handleFile} disabled={step === 'uploading' || step === 'confirming'} />
          {step === 'uploading' && (
            <p className="text-sm text-muted-foreground flex items-center gap-2">
              <RefreshCw className="h-4 w-4 animate-spin" />
              Dosya işleniyor…
            </p>
          )}
        </div>
      )}

      {/* Error */}
      {error && (
        <div className="rounded-lg border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
          {error}
        </div>
      )}

      {/* Preview */}
      {(step === 'previewing' || step === 'confirming') && preview && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-sm font-medium text-muted-foreground uppercase tracking-wide">4. Önizleme ve Onay</h2>
            <Button variant="ghost" size="sm" onClick={handleReset} className="gap-1.5 text-muted-foreground">
              <RefreshCw className="h-3.5 w-3.5" />
              Yeni Dosya
            </Button>
          </div>

          <ImportSummaryBar
            totalRows={preview.totalRows}
            validRows={preview.validRows}
            invalidRows={preview.invalidRows}
          />

          <ImportPreviewTable importType={importType} rows={preview.rows} />

          <div className="flex items-center gap-3">
            <ConfirmImportButton
              validCount={preview.validRows}
              loading={step === 'confirming'}
              onConfirm={handleConfirm}
            />
            {preview.invalidRows > 0 && (
              <p className="text-xs text-muted-foreground">
                {preview.invalidRows} hatalı satır atlanacak.
              </p>
            )}
          </div>
        </div>
      )}

      {/* Success */}
      {step === 'success' && confirmResult && (
        <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-6 space-y-4">
          <div className="flex items-center gap-2 text-emerald-700">
            <CheckCircle2 className="h-5 w-5" />
            <h2 className="font-semibold">İçe Aktarma Tamamlandı</h2>
          </div>
          <div className="text-sm text-emerald-700 space-y-1">
            <p><strong>{confirmResult.savedCount}</strong> kayıt başarıyla oluşturuldu.</p>
            {confirmResult.skippedCount > 0 && <p>{confirmResult.skippedCount} satır atlandı.</p>}
          </div>
          {confirmResult.errors.length > 0 && (
            <ul className="text-xs text-amber-700 space-y-1 border-t border-emerald-200 pt-3">
              {confirmResult.errors.map((e, i) => <li key={i}>• {e}</li>)}
            </ul>
          )}
          <Button onClick={handleReset} variant="outline" size="sm" className="gap-2">
            <RefreshCw className="h-4 w-4" />
            Yeni İçe Aktarma
          </Button>
        </div>
      )}
    </div>
  )
}
