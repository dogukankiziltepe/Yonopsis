'use client'

import { useEffect, useState, useCallback, useRef } from 'react'
import { Plus, X, CreditCard, ChevronLeft, ChevronRight, Trash2, Download, Upload } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { PaginationControls } from '@/components/ui/pagination-controls'
import { paymentsApi } from '@/lib/api/payments'
import { showSuccess, showError } from '@/lib/toast'
import {
  PaymentSummaryDto,
  PaymentItemDto,
  ImportPreviewRow,
  ImportSkippedRow,
  PaymentStatus,
  PaymentStatusLabel,
  PaymentStatusColor,
  CreatePaymentDto,
  BulkCreatePaymentsDto,
} from '@/types/payment'

const PAGE_SIZE = 20

const SUGGESTED_ITEMS = ['Isınma', 'Sıcak Su', 'Soğuk Su', 'Ortak Alan Giderleri', 'Klima', 'Asansör', 'Bahçe Bakımı']

function ItemsEditor({
  items,
  onChange,
}: {
  items: PaymentItemDto[]
  onChange: (items: PaymentItemDto[]) => void
}) {
  const add = () => onChange([...items, { name: '', amount: 0 }])
  const remove = (i: number) => onChange(items.filter((_, idx) => idx !== i))
  const update = (i: number, field: keyof PaymentItemDto, value: string) => {
    const next = items.map((item, idx) =>
      idx === i
        ? { ...item, [field]: field === 'amount' ? parseFloat(value) || 0 : value }
        : item
    )
    onChange(next)
  }
  const addSuggestion = (name: string) => {
    if (!items.find(i => i.name === name)) onChange([...items, { name, amount: 0 }])
  }

  const total = items.reduce((s, i) => s + (i.amount || 0), 0)

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <Label className="text-xs font-medium">Aidat Kalemleri <span className="text-destructive">*</span></Label>
        <Button type="button" size="sm" variant="outline" className="h-6 text-xs px-2" onClick={add}>
          <Plus className="h-3 w-3 mr-1" /> Kalem Ekle
        </Button>
      </div>

      <div className="flex flex-wrap gap-1">
        {SUGGESTED_ITEMS.map(s => (
          <button
            key={s}
            type="button"
            onClick={() => addSuggestion(s)}
            className="text-xs px-2 py-0.5 rounded-full border border-dashed border-muted-foreground/40 text-muted-foreground hover:border-primary hover:text-primary transition-colors"
          >
            + {s}
          </button>
        ))}
      </div>

      {items.length === 0 && (
        <p className="text-xs text-muted-foreground italic">Yukarıdan hızlı ekle veya "Kalem Ekle" butonunu kullan.</p>
      )}

      <div className="space-y-1.5">
        {items.map((item, i) => (
          <div key={i} className="flex gap-2 items-center">
            <Input
              className="flex-1 h-8 text-sm"
              placeholder="Kalem adı (Isınma, Sıcak Su...)"
              value={item.name}
              onChange={e => update(i, 'name', e.target.value)}
            />
            <Input
              type="number"
              className="w-28 h-8 text-sm"
              placeholder="0.00"
              min="0"
              step="0.01"
              value={item.amount || ''}
              onChange={e => update(i, 'amount', e.target.value)}
            />
            <button
              type="button"
              onClick={() => remove(i)}
              className="text-muted-foreground hover:text-destructive transition-colors shrink-0"
            >
              <Trash2 className="h-4 w-4" />
            </button>
          </div>
        ))}
      </div>

      {items.length > 0 && (
        <div className="flex justify-between items-center border-t pt-2 text-sm">
          <span className="text-muted-foreground">Toplam</span>
          <span className="font-semibold">
            {new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(total)}
          </span>
        </div>
      )}
    </div>
  )
}

export default function PaymentsPage() {
  const [items, setItems] = useState<PaymentSummaryDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchDebounced, setSearchDebounced] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create' | 'bulk' | 'template' | 'import'>('view')
  const [selected, setSelected] = useState<PaymentSummaryDto | null>(null)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formUnitId, setFormUnitId] = useState('')
  const [formItems, setFormItems] = useState<PaymentItemDto[]>([])
  const [formDueDate, setFormDueDate] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [saving, setSaving] = useState(false)
  const [checkoutLoading, setCheckoutLoading] = useState(false)

  const [tplItemNames, setTplItemNames] = useState<string[]>(['Isınma', 'Sıcak Su', 'Soğuk Su', 'Ortak Alan Giderleri'])
  const [tplDownloading, setTplDownloading] = useState(false)

  const [importFile, setImportFile] = useState<File | null>(null)
  const [importDueDate, setImportDueDate] = useState('')
  const [importDescription, setImportDescription] = useState('')
  const [importPreview, setImportPreview] = useState<{
    toCreate: ImportPreviewRow[]
    skipped: ImportSkippedRow[]
    errors: string[]
  } | null>(null)
  const fileInputRef = useRef<HTMLInputElement>(null)

  const [bulkBuildingId, setBulkBuildingId] = useState('')
  const [bulkItems, setBulkItems] = useState<PaymentItemDto[]>([])
  const [bulkDueDate, setBulkDueDate] = useState('')
  const [bulkDescription, setBulkDescription] = useState('')

  useEffect(() => {
    const t = setTimeout(() => setSearchDebounced(search), 300)
    return () => clearTimeout(t)
  }, [search])

  const load = useCallback(() => {
    setLoading(true)
    paymentsApi.getAll(page, PAGE_SIZE, searchDebounced || undefined)
      .then((res) => {
        const d = res.data
        setItems(d.items ?? [])
        setTotalCount(d.totalCount ?? 0)
        setTotalPages(d.totalPages ?? 0)
      })
      .catch(() => showError('Aidatlar yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, searchDebounced])

  useEffect(() => { load() }, [load])

  const handleSearchChange = (val: string) => { setSearch(val); setPage(1) }

  const openView = (item: PaymentSummaryDto, index: number) => {
    setPanelMode('view')
    setSelected(item)
    setPanelIndex(index)
    setPanelOpen(true)
  }

  const openCreate = () => {
    setSelected(null)
    setFormUnitId('')
    setFormItems([])
    setFormDueDate('')
    setFormDescription('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const openBulk = () => {
    setSelected(null)
    setBulkBuildingId('')
    setBulkItems([])
    setBulkDueDate(new Date().toISOString().split('T')[0])
    setBulkDescription('')
    setPanelMode('bulk')
    setPanelOpen(true)
  }

  const validateItems = (itemList: PaymentItemDto[]) => {
    if (itemList.length === 0) { showError('En az bir aidat kalemi eklemelisiniz.'); return false }
    for (const item of itemList) {
      if (!item.name.trim()) { showError('Tüm kalemlerin adı dolu olmalıdır.'); return false }
      if (!item.amount || item.amount <= 0) { showError('Tüm kalemlerin tutarı sıfırdan büyük olmalıdır.'); return false }
    }
    return true
  }

  const handleBulkCreate = async () => {
    if (!validateItems(bulkItems)) return
    if (!bulkDueDate) { showError('Vade tarihi zorunludur.'); return }
    setSaving(true)
    try {
      const dto: BulkCreatePaymentsDto = {
        buildingId: bulkBuildingId.trim() || undefined,
        items: bulkItems,
        dueDate: new Date(bulkDueDate).toISOString(),
        description: bulkDescription || undefined,
      }
      const res = await paymentsApi.bulkCreate(dto)
      showSuccess(res.data.message)
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleMarkOverdue = async () => {
    try {
      const res = await paymentsApi.markOverdue()
      showSuccess(res.data.message)
      load()
    } catch { showError('Gecikmiş aidatlar güncellenemedi.') }
  }

  const handleCreate = async () => {
    if (!formUnitId.trim()) { showError('Daire ID zorunludur.'); return }
    if (!validateItems(formItems)) return
    if (!formDueDate) { showError('Vade tarihi zorunludur.'); return }
    setSaving(true)
    try {
      const dto: CreatePaymentDto = {
        unitId: formUnitId,
        items: formItems,
        dueDate: new Date(formDueDate).toISOString(),
        description: formDescription || undefined,
      }
      await paymentsApi.create(dto)
      showSuccess('Aidat oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleStatusChange = async (status: PaymentStatus) => {
    if (!selected) return
    try {
      await paymentsApi.updateStatus(selected.id, {
        status,
        paidDate: status === 1 ? new Date().toISOString() : undefined,
      })
      showSuccess('Durum güncellendi.')
      const updated = items.map(i => i.id === selected.id ? {
        ...i,
        status,
        paidDate: status === 1 ? new Date().toISOString() : i.paidDate
      } : i)
      setItems(updated)
      setSelected({ ...selected, status })
    } catch { showError('Durum güncellenemedi.') }
  }

  const handleCheckout = async () => {
    if (!selected) return
    setCheckoutLoading(true)
    try {
      const successUrl = window.location.href
      const cancelUrl = window.location.href
      const res = await paymentsApi.createCheckout(selected.id, successUrl, cancelUrl)
      window.location.href = res.data.checkoutUrl
    } catch { showError('Ödeme oturumu oluşturulamadı.') } finally { setCheckoutLoading(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu ödemeyi silmek istediğinize emin misiniz?')) return
    try {
      await paymentsApi.delete(id)
      showSuccess('Ödeme silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  const openTemplate = () => {
    setTplItemNames(['Isınma', 'Sıcak Su', 'Soğuk Su', 'Ortak Alan Giderleri'])
    setPanelMode('template')
    setPanelOpen(true)
  }

  const openImport = () => {
    setImportFile(null)
    setImportDueDate(new Date().toISOString().split('T')[0])
    setImportDescription('')
    setImportPreview(null)
    setPanelMode('import')
    setPanelOpen(true)
  }

  const handleDownloadTemplate = async () => {
    const names = tplItemNames.filter(n => n.trim())
    if (names.length === 0) { showError('En az bir kalem adı giriniz.'); return }
    setTplDownloading(true)
    try {
      const res = await paymentsApi.downloadTemplate(names)
      const url = URL.createObjectURL(new Blob([res.data as BlobPart]))
      const a = document.createElement('a')
      a.href = url
      a.download = 'aidat_sablonu.xlsx'
      a.click()
      URL.revokeObjectURL(url)
    } catch { showError('Şablon indirilemedi.') } finally { setTplDownloading(false) }
  }

  const handlePreview = async () => {
    if (!importFile) { showError('Lütfen bir dosya seçin.'); return }
    if (!importDueDate) { showError('Vade tarihi zorunludur.'); return }
    setSaving(true)
    setImportPreview(null)
    try {
      const res = await paymentsApi.importExcel(
        importFile,
        new Date(importDueDate).toISOString(),
        importDescription || undefined,
        true,
      )
      const d = res.data
      if (d.errors.length > 0 && d.preview.length === 0 && d.skippedRows.length === 0) {
        showError('Dosyada işlenebilecek satır bulunamadı.')
      }
      setImportPreview({ toCreate: d.preview, skipped: d.skippedRows, errors: d.errors })
    } catch {} finally { setSaving(false) }
  }

  const handleConfirmImport = async () => {
    if (!importFile || !importDueDate) return
    setSaving(true)
    try {
      const res = await paymentsApi.importExcel(
        importFile,
        new Date(importDueDate).toISOString(),
        importDescription || undefined,
        false,
      )
      showSuccess(res.data.message)
      setPanelOpen(false)
      setImportPreview(null)
      load()
    } catch {} finally { setSaving(false) }
  }

  const navigatePanel = (dir: -1 | 1) => {
    const next = panelIndex + dir
    if (next < 0 || next >= items.length) return
    openView(items[next], next)
  }

  const formatCurrency = (amount: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(amount)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Aidatlar</h1>
        <div className="flex items-center gap-2">
          <Button size="sm" variant="outline" onClick={handleMarkOverdue}>
            Gecikmiş İşaretle
          </Button>
          <Button size="sm" variant="outline" onClick={openTemplate}>
            <Download className="h-4 w-4 mr-1" />
            Şablon İndir
          </Button>
          <Button size="sm" variant="outline" onClick={openImport}>
            <Upload className="h-4 w-4 mr-1" />
            İçe Aktar
          </Button>
          <Button size="sm" variant="outline" onClick={openBulk}>
            Toplu Oluştur
          </Button>
          <Button size="sm" onClick={openCreate}>
            <Plus className="h-4 w-4 mr-1" />
            Yeni Aidat
          </Button>
        </div>
      </div>

      <div className="mb-3">
        <PaginationControls
          page={page} pageSize={PAGE_SIZE} totalCount={totalCount} totalPages={totalPages}
          search={search} onPageChange={setPage} onSearchChange={handleSearchChange}
          searchPlaceholder="Daire veya açıklama ara..."
        />
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Daire</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tutar</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Kalemler</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Vade</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={5} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={5}>
                <div className="flex flex-col items-center py-12">
                  <CreditCard className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz aidat kaydı bulunmuyor.</p>
                </div>
              </td></tr>
            ) : (
              items.map((item, i) => (
                <tr
                  key={item.id}
                  onClick={() => openView(item, i)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === item.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium">{item.unitDoorNumber ?? '-'}</td>
                  <td className="px-3 py-2.5 font-semibold">{formatCurrency(item.amount)}</td>
                  <td className="px-3 py-2.5 text-muted-foreground text-xs">
                    {item.items && item.items.length > 0
                      ? item.items.map(k => k.name).join(', ')
                      : '—'}
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground">
                    {new Date(item.dueDate).toLocaleDateString('tr-TR')}
                  </td>
                  <td className="px-3 py-2.5">
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${PaymentStatusColor[item.status]}`}>
                      {PaymentStatusLabel[item.status]}
                    </span>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[520px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <div className="flex items-center gap-2">
              {panelMode === 'view' && items.length > 0 && (
                <>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(-1)} disabled={panelIndex === 0}>
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <span className="text-xs text-muted-foreground">{panelIndex + 1} / {items.length}</span>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(1)} disabled={panelIndex >= items.length - 1}>
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </>
              )}
              <h2 className="text-sm font-semibold ml-1">
                {panelMode === 'create' ? 'Yeni Aidat'
                  : panelMode === 'bulk' ? 'Toplu Aidat Oluştur'
                  : panelMode === 'template' ? 'Excel Şablonu İndir'
                  : panelMode === 'import' ? (importPreview ? 'İçe Aktarma Önizlemesi' : 'Excel\'den İçe Aktar')
                  : `Daire ${selected?.unitDoorNumber ?? ''} — Aidat`}
              </h2>
            </div>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'template' ? (
              <>
                <p className="text-xs text-muted-foreground">
                  Sitenin blok ve daire listesi şablona doldurularak gelir. Tutarları girdikten sonra "İçe Aktar" ile yükleyin.
                </p>
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <Label className="text-xs font-medium">Kalem Başlıkları</Label>
                    <Button type="button" size="sm" variant="outline" className="h-6 text-xs px-2"
                      onClick={() => setTplItemNames([...tplItemNames, ''])}>
                      <Plus className="h-3 w-3 mr-1" /> Ekle
                    </Button>
                  </div>
                  {tplItemNames.map((name, i) => (
                    <div key={i} className="flex gap-2 items-center">
                      <Input
                        className="flex-1 h-8 text-sm"
                        placeholder={`Kalem ${i + 1}`}
                        value={name}
                        onChange={e => {
                          const next = [...tplItemNames]
                          next[i] = e.target.value
                          setTplItemNames(next)
                        }}
                      />
                      <button type="button" onClick={() => setTplItemNames(tplItemNames.filter((_, idx) => idx !== i))}
                        className="text-muted-foreground hover:text-destructive transition-colors shrink-0">
                        <Trash2 className="h-4 w-4" />
                      </button>
                    </div>
                  ))}
                  <div className="flex flex-wrap gap-1 pt-1">
                    {SUGGESTED_ITEMS.filter(s => !tplItemNames.includes(s)).map(s => (
                      <button key={s} type="button"
                        onClick={() => setTplItemNames([...tplItemNames, s])}
                        className="text-xs px-2 py-0.5 rounded-full border border-dashed border-muted-foreground/40 text-muted-foreground hover:border-primary hover:text-primary transition-colors">
                        + {s}
                      </button>
                    ))}
                  </div>
                </div>
                <Button size="sm" className="w-full" onClick={handleDownloadTemplate} disabled={tplDownloading}>
                  <Download className="h-4 w-4 mr-1" />
                  {tplDownloading ? 'Hazırlanıyor...' : 'Excel Şablonunu İndir'}
                </Button>
              </>
            ) : panelMode === 'import' ? (
              !importPreview ? (
                /* ── Adım 1: Dosya seç ── */
                <>
                  <p className="text-xs text-muted-foreground">
                    Doldurulmuş Excel şablonunu seçin ve vade tarihini belirtin. Yüklemeden önce önizleme yapabilirsiniz.
                  </p>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Excel Dosyası <span className="text-destructive">*</span></Label>
                    <div
                      className="border-2 border-dashed rounded-lg p-4 text-center cursor-pointer hover:border-primary/50 transition-colors"
                      onClick={() => fileInputRef.current?.click()}
                    >
                      {importFile ? (
                        <div className="flex items-center justify-center gap-2 text-sm">
                          <span className="font-medium text-primary">{importFile.name}</span>
                          <button type="button" onClick={e => { e.stopPropagation(); setImportFile(null) }}
                            className="text-muted-foreground hover:text-destructive">
                            <X className="h-4 w-4" />
                          </button>
                        </div>
                      ) : (
                        <div className="text-muted-foreground">
                          <Upload className="h-8 w-8 mx-auto mb-1 opacity-40" />
                          <p className="text-xs">Dosya seçmek için tıklayın</p>
                          <p className="text-xs opacity-60">.xlsx</p>
                        </div>
                      )}
                    </div>
                    <input ref={fileInputRef} type="file" accept=".xlsx"
                      className="hidden"
                      onChange={e => { setImportFile(e.target.files?.[0] ?? null); setImportPreview(null) }} />
                  </div>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Vade Tarihi <span className="text-destructive">*</span></Label>
                    <Input type="date" value={importDueDate} onChange={e => setImportDueDate(e.target.value)} />
                  </div>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Açıklama</Label>
                    <Input value={importDescription} onChange={e => setImportDescription(e.target.value)}
                      placeholder="Örn: Temmuz 2026 aidatı" />
                  </div>
                  <Button size="sm" className="w-full" onClick={handlePreview} disabled={saving}>
                    {saving ? 'Analiz ediliyor...' : 'Önizle'}
                  </Button>
                </>
              ) : (
                /* ── Adım 2: Önizleme + onay ── */
                <>
                  <div className="flex items-center gap-2">
                    <button
                      type="button"
                      onClick={() => setImportPreview(null)}
                      className="text-xs text-muted-foreground hover:text-foreground flex items-center gap-1"
                    >
                      <ChevronLeft className="h-3.5 w-3.5" /> Geri dön
                    </button>
                    <span className="text-xs text-muted-foreground">
                      {new Date(importDueDate).toLocaleDateString('tr-TR', { month: 'long', year: 'numeric' })} — {importFile?.name}
                    </span>
                  </div>

                  {/* Özet banner */}
                  <div className="grid grid-cols-3 gap-2 text-center">
                    <div className="rounded-lg bg-green-50 border border-green-200 py-2">
                      <p className="text-lg font-bold text-green-700">{importPreview.toCreate.length}</p>
                      <p className="text-xs text-green-600">Oluşturulacak</p>
                    </div>
                    <div className="rounded-lg bg-yellow-50 border border-yellow-200 py-2">
                      <p className="text-lg font-bold text-yellow-700">{importPreview.skipped.length}</p>
                      <p className="text-xs text-yellow-600">Atlanacak</p>
                    </div>
                    <div className="rounded-lg bg-red-50 border border-red-200 py-2">
                      <p className="text-lg font-bold text-red-700">{importPreview.errors.length}</p>
                      <p className="text-xs text-red-600">Hata</p>
                    </div>
                  </div>

                  {/* Oluşturulacak kayıtlar */}
                  {importPreview.toCreate.length > 0 && (
                    <div className="border rounded-lg overflow-hidden">
                      <div className="bg-green-50 px-3 py-1.5 border-b border-green-200">
                        <p className="text-xs font-medium text-green-700">Oluşturulacak ({importPreview.toCreate.length})</p>
                      </div>
                      <div className="max-h-52 overflow-y-auto divide-y text-sm">
                        {importPreview.toCreate.map((row, i) => (
                          <div key={i} className="px-3 py-2">
                            <div className="flex items-center justify-between mb-1">
                              <span className="font-medium">{row.buildingName} — {row.doorNumber}</span>
                              <span className="font-semibold text-green-700">{formatCurrency(row.total)}</span>
                            </div>
                            <div className="flex flex-wrap gap-x-3 gap-y-0.5">
                              {row.items.map((item, j) => (
                                <span key={j} className="text-xs text-muted-foreground">
                                  {item.name}: {formatCurrency(item.amount)}
                                </span>
                              ))}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}

                  {/* Atlanacak kayıtlar */}
                  {importPreview.skipped.length > 0 && (
                    <div className="border rounded-lg overflow-hidden">
                      <div className="bg-yellow-50 px-3 py-1.5 border-b border-yellow-200">
                        <p className="text-xs font-medium text-yellow-700">Atlanacak ({importPreview.skipped.length})</p>
                      </div>
                      <div className="max-h-32 overflow-y-auto divide-y text-xs">
                        {importPreview.skipped.map((row, i) => (
                          <div key={i} className="px-3 py-1.5 flex justify-between gap-2">
                            <span className="font-medium">{row.buildingName} — {row.doorNumber}</span>
                            <span className="text-muted-foreground text-right shrink-0">{row.reason}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}

                  {/* Hatalar */}
                  {importPreview.errors.length > 0 && (
                    <div className="border border-red-200 rounded-lg overflow-hidden">
                      <div className="bg-red-50 px-3 py-1.5 border-b border-red-200">
                        <p className="text-xs font-medium text-red-700">Hatalar ({importPreview.errors.length})</p>
                      </div>
                      <ul className="max-h-28 overflow-y-auto divide-y text-xs">
                        {importPreview.errors.map((e, i) => (
                          <li key={i} className="px-3 py-1.5 text-red-600">• {e}</li>
                        ))}
                      </ul>
                    </div>
                  )}

                  {importPreview.toCreate.length === 0 ? (
                    <p className="text-sm text-center text-muted-foreground py-2">İçe aktarılacak kayıt yok.</p>
                  ) : (
                    <Button size="sm" className="w-full" onClick={handleConfirmImport} disabled={saving}>
                      <Upload className="h-4 w-4 mr-1" />
                      {saving ? 'Aktarılıyor...' : `Onayla ve ${importPreview.toCreate.length} Kaydı İçe Aktar`}
                    </Button>
                  )}
                </>
              )
            ) : panelMode === 'bulk' ? (
              <>
                <p className="text-xs text-muted-foreground">Sitedeki tüm aktif daireler için aylık aidat kaydı oluşturur. Aynı aya ait mevcut kayıtlar atlanır.</p>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Bina ID <span className="text-xs text-muted-foreground">(boş bırakılırsa tüm binalar)</span></Label>
                  <Input value={bulkBuildingId} onChange={(e) => setBulkBuildingId(e.target.value)} placeholder="Bina UUID (opsiyonel)" />
                </div>
                <ItemsEditor items={bulkItems} onChange={setBulkItems} />
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Vade Tarihi <span className="text-destructive">*</span></Label>
                  <Input type="date" value={bulkDueDate} onChange={(e) => setBulkDueDate(e.target.value)} />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Açıklama</Label>
                  <Input value={bulkDescription} onChange={(e) => setBulkDescription(e.target.value)} placeholder="Örn: Temmuz 2026 aidatı" />
                </div>
                <Button size="sm" className="w-full" onClick={handleBulkCreate} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Toplu Oluştur'}
                </Button>
              </>
            ) : panelMode === 'create' ? (
              <>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Daire ID <span className="text-destructive">*</span></Label>
                  <Input value={formUnitId} onChange={(e) => setFormUnitId(e.target.value)} placeholder="Daire UUID" />
                </div>
                <ItemsEditor items={formItems} onChange={setFormItems} />
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Vade Tarihi <span className="text-destructive">*</span></Label>
                  <Input type="date" value={formDueDate} onChange={(e) => setFormDueDate(e.target.value)} />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Açıklama</Label>
                  <Input value={formDescription} onChange={(e) => setFormDescription(e.target.value)} placeholder="Örn: Ocak 2026 aidatı" />
                </div>
                <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Oluştur'}
                </Button>
              </>
            ) : selected ? (
              <>
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Daire</p>
                    <p className="font-medium">{selected.unitDoorNumber ?? '-'}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Toplam Tutar</p>
                    <p className="font-semibold text-base">{formatCurrency(selected.amount)}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Vade Tarihi</p>
                    <p>{new Date(selected.dueDate).toLocaleDateString('tr-TR')}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Durum</p>
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${PaymentStatusColor[selected.status]}`}>
                      {PaymentStatusLabel[selected.status]}
                    </span>
                  </div>
                  {selected.paidDate && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Ödeme Tarihi</p>
                      <p>{new Date(selected.paidDate).toLocaleDateString('tr-TR')}</p>
                    </div>
                  )}
                  {selected.description && (
                    <div className="col-span-2">
                      <p className="text-xs text-muted-foreground mb-0.5">Açıklama</p>
                      <p>{selected.description}</p>
                    </div>
                  )}
                </div>

                {selected.items && selected.items.length > 0 && (
                  <div className="border rounded-lg overflow-hidden">
                    <div className="bg-muted/40 px-3 py-2">
                      <p className="text-xs font-medium text-muted-foreground">AİDAT KALEMLERİ</p>
                    </div>
                    <div className="divide-y">
                      {selected.items.map((item, i) => (
                        <div key={i} className="flex items-center justify-between px-3 py-2 text-sm">
                          <span>{item.name}</span>
                          <span className="font-medium">{formatCurrency(item.amount)}</span>
                        </div>
                      ))}
                      <div className="flex items-center justify-between px-3 py-2 text-sm bg-muted/20">
                        <span className="font-medium">Toplam</span>
                        <span className="font-semibold">{formatCurrency(selected.amount)}</span>
                      </div>
                    </div>
                  </div>
                )}

                {selected.status !== 1 && (
                  <Button
                    size="sm"
                    className="w-full"
                    onClick={handleCheckout}
                    disabled={checkoutLoading}
                  >
                    <CreditCard className="h-4 w-4 mr-1" />
                    {checkoutLoading ? 'Yönlendiriliyor...' : 'Online Öde'}
                  </Button>
                )}

                <div className="space-y-2">
                  <p className="text-xs font-medium text-muted-foreground">DURUM DEĞİŞTİR</p>
                  <div className="flex gap-2 flex-wrap">
                    {([0, 1, 2] as PaymentStatus[]).map((s) => (
                      <Button
                        key={s}
                        size="sm"
                        variant={selected.status === s ? 'default' : 'outline'}
                        onClick={() => handleStatusChange(s)}
                        className="text-xs"
                      >
                        {PaymentStatusLabel[s]}
                      </Button>
                    ))}
                  </div>
                </div>

                <div className="pt-2 border-t">
                  <Button size="sm" variant="destructive" onClick={() => handleDelete(selected.id)}>
                    Sil
                  </Button>
                </div>
              </>
            ) : null}
          </div>
        </div>
      )}
    </div>
  )
}
