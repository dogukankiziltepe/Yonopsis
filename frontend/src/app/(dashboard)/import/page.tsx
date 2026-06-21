'use client'

import { useRef, useState } from 'react'
import { Download, Upload, CheckCircle2, AlertTriangle, Building2, Home, Users } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { showSuccess, showError } from '@/lib/toast'
import { importApi } from '@/lib/api/import'
import { ImportPreview, ImportType } from '@/types/import'

const TYPES: { key: ImportType; label: string; icon: typeof Building2 }[] = [
  { key: 'buildings', label: 'Binalar', icon: Building2 },
  { key: 'units', label: 'Daireler', icon: Home },
  { key: 'users', label: 'Kullanıcılar', icon: Users },
]

export default function ImportPage() {
  const [type, setType] = useState<ImportType>('buildings')
  const [preview, setPreview] = useState<ImportPreview | null>(null)
  const [fileName, setFileName] = useState('')
  const [busy, setBusy] = useState(false)
  const fileRef = useRef<HTMLInputElement>(null)

  const reset = () => { setPreview(null); setFileName('') }

  const changeType = (t: ImportType) => { setType(t); reset() }

  const downloadTemplate = async () => {
    try {
      const res = await importApi.downloadTemplate(type)
      const url = URL.createObjectURL(res.data as Blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `${type}_template.xlsx`
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
      URL.revokeObjectURL(url)
    } catch { showError('Şablon indirilemedi.') }
  }

  const onFile = async (file?: File) => {
    if (!file) return
    if (file.size > 5 * 1024 * 1024) { showError('Dosya 5MB sınırını aşıyor.'); return }
    setFileName(file.name)
    setBusy(true)
    setPreview(null)
    try {
      const { data } = await importApi.preview(type, file)
      setPreview(data)
    } catch { showError('Dosya işlenemedi.') } finally { setBusy(false) }
  }

  const confirm = async () => {
    if (!preview) return
    setBusy(true)
    try {
      const rows = preview.rows.map((r) => r.data)
      const { data } = await importApi.confirm(type, rows)
      showSuccess(`${data.savedRows} kayıt eklendi${data.skippedRows ? `, ${data.skippedRows} satır atlandı` : ''}.`)
      reset()
      if (fileRef.current) fileRef.current.value = ''
    } catch { showError('Kayıt sırasında hata oluştu.') } finally { setBusy(false) }
  }

  const columns = preview && preview.rows.length > 0 ? Object.keys(preview.rows[0].data) : []

  return (
    <div className="flex flex-col h-full">
      <h1 className="text-xl font-semibold mb-4">Toplu Veri Girişi (Excel)</h1>

      {/* Tip seçimi */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4">
        {TYPES.map((t) => {
          const Icon = t.icon
          const active = type === t.key
          return (
            <button
              key={t.key}
              onClick={() => changeType(t.key)}
              className={`flex items-center gap-3 rounded-lg border p-3 text-left transition-colors ${
                active ? 'border-primary bg-accent' : 'hover:bg-muted/40'
              }`}
            >
              <Icon className={`h-5 w-5 ${active ? 'text-primary' : 'text-muted-foreground'}`} />
              <span className="font-medium">{t.label}</span>
            </button>
          )
        })}
      </div>

      {/* Aksiyonlar */}
      <div className="flex flex-wrap items-center gap-2 mb-4">
        <Button size="sm" variant="outline" onClick={downloadTemplate}>
          <Download className="h-4 w-4 mr-1" /> Şablon İndir
        </Button>
        <Button size="sm" onClick={() => fileRef.current?.click()} disabled={busy}>
          <Upload className="h-4 w-4 mr-1" /> Dosya Yükle (.xlsx)
        </Button>
        <input
          ref={fileRef}
          type="file"
          accept=".xlsx,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          className="hidden"
          onChange={(e) => onFile(e.target.files?.[0])}
        />
        {fileName && <span className="text-sm text-muted-foreground">{fileName}</span>}
      </div>

      {busy && <div className="text-center py-8 text-muted-foreground">İşleniyor...</div>}

      {preview && (
        <>
          <div className="flex flex-wrap items-center gap-3 mb-3">
            <Badge variant="default" className="gap-1">
              <CheckCircle2 className="h-3 w-3" /> {preview.validRows} geçerli
            </Badge>
            <Badge variant={preview.invalidRows > 0 ? 'destructive' : 'secondary'} className="gap-1">
              <AlertTriangle className="h-3 w-3" /> {preview.invalidRows} hatalı
            </Badge>
            <span className="text-sm text-muted-foreground">Toplam {preview.totalRows} satır</span>
            <div className="ml-auto">
              <Button size="sm" onClick={confirm} disabled={busy || preview.validRows === 0}>
                Geçerli {preview.validRows} satırı kaydet
              </Button>
            </div>
          </div>

          <div className="border rounded-lg overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-muted/50">
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">#</th>
                  {columns.map((c) => (
                    <th key={c} className="text-left px-3 py-2 font-medium text-muted-foreground">{c}</th>
                  ))}
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Durum</th>
                </tr>
              </thead>
              <tbody>
                {preview.rows.map((r) => (
                  <tr key={r.rowIndex} className={`border-b last:border-0 ${r.isValid ? '' : 'bg-destructive/5'}`}>
                    <td className="px-3 py-1.5 text-muted-foreground">{r.rowIndex}</td>
                    {columns.map((c) => (
                      <td key={c} className="px-3 py-1.5">{r.data[c]}</td>
                    ))}
                    <td className="px-3 py-1.5">
                      {r.isValid ? (
                        <span className="inline-flex items-center gap-1 text-emerald-600"><CheckCircle2 className="h-4 w-4" /> Geçerli</span>
                      ) : (
                        <span className="inline-flex items-center gap-1 text-destructive" title={r.errors.join('\n')}>
                          <AlertTriangle className="h-4 w-4" /> {r.errors.join(', ')}
                        </span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}
