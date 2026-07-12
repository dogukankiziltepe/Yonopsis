'use client'

import { useEffect, useState, useCallback, useRef } from 'react'
import { useRouter } from 'next/navigation'
import { Download, Upload, AlertCircle, CreditCard, List } from 'lucide-react'
import * as XLSX from 'xlsx'
import { Button } from '@/components/ui/button'
import { aidatKalemleriApi } from '@/lib/api/aidatKalemleri'
import { unitsApi } from '@/lib/api/units'
import { paymentsApi } from '@/lib/api/payments'
import { AidatKalemi, AidatImportRow } from '@/types/aidatKalemi'
import { PaymentSummaryDto } from '@/types/payment'
import { PaginatedResult } from '@/types/api'
import { showError, showInfo } from '@/lib/toast'
import { Badge } from '@/components/ui/badge'

const PaymentStatusLabel: Record<number, string> = { 0: 'Bekliyor', 1: 'Ödendi', 2: 'Gecikti' }
const PaymentStatusColor: Record<number, string> = {
  0: 'bg-yellow-100 text-yellow-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-red-100 text-red-700',
}

const TRY = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' })

export default function AidatlarPage() {
  const router = useRouter()
  const fileInputRef = useRef<HTMLInputElement>(null)

  const [payments, setPayments] = useState<PaymentSummaryDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(true)
  const [templateLoading, setTemplateLoading] = useState(false)
  const [importLoading, setImportLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const res = await paymentsApi.getAll(1, 50)
      const data = res.data as PaginatedResult<PaymentSummaryDto>
      setPayments(data.items)
      setTotalCount(data.totalCount)
    } catch {
      setError('Veriler yüklenirken bir hata oluştu.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { load() }, [load])

  const handleDownloadTemplate = async () => {
    setTemplateLoading(true)
    try {
      const [kalemleriRes, unitsRes] = await Promise.all([
        aidatKalemleriApi.getAll(),
        unitsApi.getAll(),
      ])

      const kalemleri: AidatKalemi[] = kalemleriRes.data.filter((k: AidatKalemi) => k.isActive)
      const units = unitsRes.data

      if (kalemleri.length === 0) {
        showInfo('Önce en az bir aktif aidat kalemi tanımlayın.')
        return
      }

      if (units.length === 0) {
        showInfo('Henüz daire tanımlanmamış.')
        return
      }

      const headers = ['Blok-Daire', 'UnitId', ...kalemleri.map((k) => k.name)]
      const rows = units.map((unit) => [
        `${unit.buildingName ?? 'Bina'} - ${unit.doorNumber}`,
        unit.id,
        ...kalemleri.map(() => 0),
      ])

      const ws = XLSX.utils.aoa_to_sheet([headers, ...rows])

      // UnitId kolonunu sarı yap ve küçük genişlik ver
      ws['!cols'] = [
        { wch: 25 },   // Blok-Daire
        { wch: 36 },   // UnitId
        ...kalemleri.map(() => ({ wch: 18 })),
      ]

      // UnitId başlık hücresini sarı arka planla işaretle
      const unitIdCell = ws['B1']
      if (unitIdCell) {
        unitIdCell.s = {
          fill: { fgColor: { rgb: 'FFFF99' } },
          font: { color: { rgb: '888888' }, sz: 8 },
        }
      }

      const wb = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(wb, ws, 'Aidatlar')
      XLSX.writeFile(wb, 'aidat-sablonu.xlsx')
    } catch {
      showError('Şablon oluşturulurken bir hata oluştu.')
    } finally {
      setTemplateLoading(false)
    }
  }

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    if (fileInputRef.current) fileInputRef.current.value = ''

    setImportLoading(true)
    try {
      const [kalemleriRes] = await Promise.all([aidatKalemleriApi.getAll()])
      const kalemleri: AidatKalemi[] = kalemleriRes.data.filter((k: AidatKalemi) => k.isActive)

      const buffer = await file.arrayBuffer()
      const wb = XLSX.read(buffer, { type: 'array' })
      const ws = wb.Sheets[wb.SheetNames[0]]
      const rawData = XLSX.utils.sheet_to_json<string[]>(ws, { header: 1 }) as string[][]

      if (!rawData || rawData.length < 2) {
        showError('Excel dosyası geçerli veri içermiyor.')
        return
      }

      const headerRow = rawData[0]
      // Header: [Blok-Daire, UnitId, Kalem1, Kalem2, ...]
      const kalemStartIndex = 2
      const kalemHeaders = headerRow.slice(kalemStartIndex)

      // Kalem isimlerine göre ID eşleştir
      const kalemMap = new Map<string, string>()
      kalemleri.forEach((k) => kalemMap.set(k.name, k.id))

      const importRows: AidatImportRow[] = []
      for (let i = 1; i < rawData.length; i++) {
        const row = rawData[i]
        if (!row || row.length < 2) continue
        const displayName = String(row[0] ?? '')
        const unitId = String(row[1] ?? '').trim()
        if (!unitId || unitId.length !== 36) continue

        const amounts = kalemHeaders.map((header, idx) => {
          const kalemId = kalemMap.get(header)
          if (!kalemId) return null
          const raw = row[kalemStartIndex + idx]
          const amount = parseFloat(String(raw ?? '0')) || 0
          return { kalemId, amount }
        }).filter(Boolean) as { kalemId: string; amount: number }[]

        importRows.push({ unitId, displayName, amounts })
      }

      if (importRows.length === 0) {
        showError('Excel dosyasında geçerli daire verisi bulunamadı.')
        return
      }

      // Aidat kalem bilgilerini de sessionStorage'a ekle
      sessionStorage.setItem('aidatImport', JSON.stringify(importRows))
      sessionStorage.setItem('aidatKalemleri', JSON.stringify(kalemleri))
      router.push('/aidatlar/import')
    } catch {
      showError('Excel dosyası okunurken bir hata oluştu.')
    } finally {
      setImportLoading(false)
    }
  }

  return (
    <div className="p-6">
      {/* Başlık */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Aidatlar</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Dairelere toplu aidat ataması yapın ve mevcut aidatları görüntüleyin.
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="gap-2"
            onClick={handleDownloadTemplate}
            disabled={templateLoading}
          >
            <Download className="h-4 w-4" />
            {templateLoading ? 'Hazırlanıyor...' : 'Şablon İndir'}
          </Button>
          <Button
            size="sm"
            className="gap-2"
            onClick={() => fileInputRef.current?.click()}
            disabled={importLoading}
          >
            <Upload className="h-4 w-4" />
            {importLoading ? 'Okunuyor...' : 'Excel Yükle'}
          </Button>
          <input
            ref={fileInputRef}
            type="file"
            accept=".xlsx,.xls"
            className="hidden"
            onChange={handleFileChange}
          />
        </div>
      </div>

      {/* Bilgi kartı */}
      <div className="mb-6 p-4 rounded-lg border bg-muted/30 text-sm text-muted-foreground flex gap-3">
        <List className="h-5 w-5 shrink-0 mt-0.5" />
        <div className="space-y-1">
          <p className="font-medium text-foreground">Toplu Aidat Oluşturma</p>
          <p>1. <strong>Şablon İndir</strong> ile boş Excel'i indirin (daireler ve kalemler dolu gelir).</p>
          <p>2. Her daire için aidat tutarlarını girin (0 bırakılanlar atlanır).</p>
          <p>3. <strong>Excel Yükle</strong> ile dosyayı seçin, önizleme ekranında onaylayın.</p>
        </div>
      </div>

      {error && (
        <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm">
          <AlertCircle className="h-4 w-4 shrink-0" />
          {error}
        </div>
      )}

      {/* Aidat listesi */}
      <div>
        <div className="flex items-center justify-between mb-3">
          <h2 className="text-sm font-medium text-muted-foreground">
            {loading ? 'Yükleniyor...' : `${totalCount} kayıt`}
          </h2>
        </div>

        {loading ? (
          <div className="space-y-2">
            {Array.from({ length: 8 }).map((_, i) => (
              <div key={i} className="h-12 rounded-lg bg-muted animate-pulse" />
            ))}
          </div>
        ) : payments.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 text-center">
            <CreditCard className="h-12 w-12 text-muted-foreground mb-4" />
            <p className="text-muted-foreground">Henüz aidat kaydı bulunmuyor.</p>
            <p className="text-sm text-muted-foreground mt-1">
              Şablon indirerek toplu aidat oluşturabilirsiniz.
            </p>
          </div>
        ) : (
          <div className="rounded-lg border overflow-hidden">
            <table className="w-full text-sm">
              <thead className="bg-muted/50">
                <tr>
                  <th className="px-4 py-3 text-left font-medium text-muted-foreground">Daire</th>
                  <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden md:table-cell">Vade Tarihi</th>
                  <th className="px-4 py-3 text-right font-medium text-muted-foreground">Tutar</th>
                  <th className="px-4 py-3 text-center font-medium text-muted-foreground">Durum</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {payments.map((p) => (
                  <tr key={p.id} className="hover:bg-muted/30 transition-colors">
                    <td className="px-4 py-3 font-medium">{p.unitDoorNumber ?? p.unitId}</td>
                    <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">
                      {new Date(p.dueDate).toLocaleDateString('tr-TR')}
                    </td>
                    <td className="px-4 py-3 text-right font-mono">{TRY.format(p.amount)}</td>
                    <td className="px-4 py-3 text-center">
                      <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${PaymentStatusColor[p.status]}`}>
                        {PaymentStatusLabel[p.status]}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {totalCount > 50 && (
              <div className="px-4 py-3 border-t text-xs text-muted-foreground text-center">
                İlk 50 kayıt gösteriliyor. Toplam {totalCount} kayıt.
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}
