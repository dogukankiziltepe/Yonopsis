'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, CreditCard, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { paymentsApi } from '@/lib/api/payments'
import { showSuccess, showError } from '@/lib/toast'
import {
  PaymentSummaryDto,
  PaymentStatus,
  PaymentStatusLabel,
  PaymentStatusColor,
  CreatePaymentDto,
} from '@/types/payment'

export default function PaymentsPage() {
  const [items, setItems] = useState<PaymentSummaryDto[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [selected, setSelected] = useState<PaymentSummaryDto | null>(null)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formUnitId, setFormUnitId] = useState('')
  const [formAmount, setFormAmount] = useState('')
  const [formDueDate, setFormDueDate] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [saving, setSaving] = useState(false)

  const filtered = items.filter((r) => {
    const q = search.toLowerCase()
    return (
      (r.unitDoorNumber ?? '').toLowerCase().includes(q) ||
      (r.description ?? '').toLowerCase().includes(q)
    )
  })

  const load = useCallback(() => {
    setLoading(true)
    paymentsApi.getAll()
      .then((res) => setItems(res.data ?? []))
      .catch(() => showError('Aidatlar yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const openView = (item: PaymentSummaryDto, index: number) => {
    setPanelMode('view')
    setSelected(item)
    setPanelIndex(index)
    setPanelOpen(true)
  }

  const openCreate = () => {
    setSelected(null)
    setFormUnitId('')
    setFormAmount('')
    setFormDueDate('')
    setFormDescription('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formUnitId.trim()) { showError('Daire ID zorunludur.'); return }
    const amount = parseFloat(formAmount)
    if (!formAmount || isNaN(amount) || amount <= 0) { showError('Geçerli bir tutar giriniz.'); return }
    if (!formDueDate) { showError('Vade tarihi zorunludur.'); return }
    setSaving(true)
    try {
      const dto: CreatePaymentDto = {
        unitId: formUnitId,
        amount,
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

  const handleDelete = async (id: string) => {
    if (!confirm('Bu ödemeyi silmek istediğinize emin misiniz?')) return
    try {
      await paymentsApi.delete(id)
      showSuccess('Ödeme silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  const navigatePanel = (dir: -1 | 1) => {
    const next = panelIndex + dir
    if (next < 0 || next >= filtered.length) return
    openView(filtered[next], next)
  }

  const formatCurrency = (amount: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(amount)

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Aidatlar</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Aidat
        </Button>
      </div>

      <div className="mb-3">
        <Input
          placeholder="Daire veya açıklama ara..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="max-w-xs"
        />
      </div>

      <div className="border rounded-lg overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Daire</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tutar</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Vade</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : filtered.length === 0 ? (
              <tr><td colSpan={4}>
                <div className="flex flex-col items-center py-12">
                  <CreditCard className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz aidat kaydı bulunmuyor.</p>
                </div>
              </td></tr>
            ) : (
              filtered.map((item, i) => (
                <tr
                  key={item.id}
                  onClick={() => openView(item, i)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === item.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium">{item.unitDoorNumber ?? '-'}</td>
                  <td className="px-3 py-2.5">{formatCurrency(item.amount)}</td>
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
        <div className="fixed right-0 top-0 h-screen w-[480px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <div className="flex items-center gap-2">
              {panelMode === 'view' && (
                <>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(-1)} disabled={panelIndex === 0}>
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <span className="text-xs text-muted-foreground">{panelIndex + 1} / {filtered.length}</span>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(1)} disabled={panelIndex >= filtered.length - 1}>
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </>
              )}
              <h2 className="text-sm font-semibold ml-1">
                {panelMode === 'create' ? 'Yeni Aidat' : `Daire ${selected?.unitDoorNumber ?? ''} — Aidat`}
              </h2>
            </div>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'create' ? (
              <>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Daire ID <span className="text-destructive">*</span></Label>
                  <Input value={formUnitId} onChange={(e) => setFormUnitId(e.target.value)} placeholder="Daire UUID" />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Tutar (₺) <span className="text-destructive">*</span></Label>
                  <Input type="number" value={formAmount} onChange={(e) => setFormAmount(e.target.value)} placeholder="0.00" min="0" />
                </div>
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
                    <p className="text-xs text-muted-foreground mb-0.5">Tutar</p>
                    <p className="font-semibold">{formatCurrency(selected.amount)}</p>
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
