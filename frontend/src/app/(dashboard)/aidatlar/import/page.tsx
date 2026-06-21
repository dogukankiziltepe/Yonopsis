'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { ArrowLeft, Save, AlertCircle, CheckCircle2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { paymentsApi } from '@/lib/api/payments'
import { AidatKalemi, AidatImportRow, BulkCreateAidatPaymentsDto } from '@/types/aidatKalemi'
import { showSuccess, showError } from '@/lib/toast'

export default function AidatImportPage() {
  const router = useRouter()

  const [rows, setRows] = useState<AidatImportRow[]>([])
  const [kalemleri, setKalemleri] = useState<AidatKalemi[]>([])
  const [dueDate, setDueDate] = useState<string>(() => {
    const now = new Date()
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-01`
  })
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [loaded, setLoaded] = useState(false)

  useEffect(() => {
    try {
      const importData = sessionStorage.getItem('aidatImport')
      const kalemlerData = sessionStorage.getItem('aidatKalemleri')
      if (!importData || !kalemlerData) {
        router.replace('/aidatlar')
        return
      }
      const parsedRows: AidatImportRow[] = JSON.parse(importData)
      const parsedKalemleri: AidatKalemi[] = JSON.parse(kalemlerData)
      setRows(parsedRows)
      setKalemleri(parsedKalemleri)
      setLoaded(true)
    } catch {
      router.replace('/aidatlar')
    }
  }, [router])

  const updateAmount = (rowIndex: number, kalemId: string, value: string) => {
    const amount = parseFloat(value) || 0
    setRows((prev) =>
      prev.map((row, i) =>
        i === rowIndex
          ? {
              ...row,
              amounts: row.amounts.map((a) =>
                a.kalemId === kalemId ? { ...a, amount } : a
              ),
            }
          : row
      )
    )
  }

  const activeCount = rows.reduce(
    (acc, row) => acc + row.amounts.filter((a) => a.amount > 0).length,
    0
  )

  const handleSave = async () => {
    if (activeCount === 0) {
      setError('Kaydetmek için en az bir tutarın sıfırdan büyük olması gerekir.')
      return
    }

    setSaving(true)
    setError(null)
    try {
      const items = rows.flatMap((row) =>
        row.amounts
          .filter((a) => a.amount > 0)
          .map((a) => ({
            unitId: row.unitId,
            aidatKalemId: a.kalemId,
            amount: a.amount,
          }))
      )

      const dto: BulkCreateAidatPaymentsDto = {
        dueDate: new Date(dueDate).toISOString(),
        items,
      }

      const res = await paymentsApi.bulkAidat(dto)
      showSuccess(res.data.message || `${res.data.count} aidat kaydı oluşturuldu.`)
      sessionStorage.removeItem('aidatImport')
      sessionStorage.removeItem('aidatKalemleri')
      router.push('/aidatlar')
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
      setError(msg || 'Kayıt sırasında bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  if (!loaded) {
    return (
      <div className="p-6 flex items-center justify-center h-64">
        <div className="text-muted-foreground text-sm">Yükleniyor...</div>
      </div>
    )
  }

  return (
    <div className="p-6 max-w-full">
      {/* Başlık */}
      <div className="flex items-center gap-4 mb-6">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => router.push('/aidatlar')}
          className="shrink-0"
        >
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <div className="flex-1 min-w-0">
          <h1 className="text-2xl font-bold tracking-tight">Aidat Önizleme</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Tutarları kontrol edin, vade tarihini seçin ve kaydedin.
          </p>
        </div>
        <Button onClick={handleSave} disabled={saving || activeCount === 0} className="gap-2 shrink-0">
          <Save className="h-4 w-4" />
          {saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>

      {/* Ayar satırı */}
      <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 mb-6 p-4 rounded-lg border bg-muted/30">
        <div className="flex items-center gap-3">
          <label className="text-sm font-medium text-muted-foreground whitespace-nowrap">Vade Tarihi</label>
          <input
            type="month"
            value={dueDate.slice(0, 7)}
            onChange={(e) => setDueDate(`${e.target.value}-01`)}
            className="px-3 py-1.5 text-sm rounded-md border bg-background focus:outline-none focus:ring-1 focus:ring-ring"
          />
        </div>
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <CheckCircle2 className="h-4 w-4 text-green-500" />
          <span>
            <strong className="text-foreground">{activeCount}</strong> aidat kaydı oluşturulacak
          </span>
        </div>
      </div>

      {error && (
        <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm">
          <AlertCircle className="h-4 w-4 shrink-0" />
          {error}
        </div>
      )}

      {/* Düzenlenebilir tablo */}
      <div className="rounded-lg border overflow-x-auto">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="px-4 py-3 text-left font-medium text-muted-foreground sticky left-0 bg-muted/50 min-w-[180px]">
                Blok / Daire
              </th>
              {kalemleri.map((k) => (
                <th key={k.id} className="px-3 py-3 text-right font-medium text-muted-foreground min-w-[140px] whitespace-nowrap">
                  {k.name}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y">
            {rows.map((row, rowIndex) => {
              const hasAny = row.amounts.some((a) => a.amount > 0)
              return (
                <tr
                  key={row.unitId}
                  className={`transition-colors ${hasAny ? 'hover:bg-muted/30' : 'hover:bg-muted/20 opacity-60'}`}
                >
                  <td className="px-4 py-2.5 font-medium sticky left-0 bg-background border-r">
                    {row.displayName}
                  </td>
                  {kalemleri.map((k) => {
                    const entry = row.amounts.find((a) => a.kalemId === k.id)
                    return (
                      <td key={k.id} className="px-3 py-2">
                        <input
                          type="number"
                          min={0}
                          step={0.01}
                          value={entry?.amount === 0 ? '' : entry?.amount ?? ''}
                          placeholder="0"
                          onChange={(e) => updateAmount(rowIndex, k.id, e.target.value)}
                          className={`w-full text-right px-2 py-1.5 rounded border text-sm bg-background focus:outline-none focus:ring-1 focus:ring-ring ${
                            (entry?.amount ?? 0) > 0 ? 'border-primary/50 bg-primary/5' : ''
                          }`}
                        />
                      </td>
                    )
                  })}
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>

      {/* Alt buton */}
      <div className="mt-6 flex items-center justify-between">
        <p className="text-xs text-muted-foreground">
          0 veya boş bırakılan satırlar için aidat oluşturulmaz.
        </p>
        <Button onClick={handleSave} disabled={saving || activeCount === 0} className="gap-2">
          <Save className="h-4 w-4" />
          {saving ? 'Kaydediliyor...' : `${activeCount} Kaydı Oluştur`}
        </Button>
      </div>
    </div>
  )
}
