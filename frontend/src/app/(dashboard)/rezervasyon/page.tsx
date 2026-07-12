'use client'

import { useCallback, useEffect, useState } from 'react'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { showError, showSuccess } from '@/lib/toast'
import { rezervasyonApi } from '@/lib/api/rezervasyon'

import { siteApi } from '@/lib/api/client'
import {
  Rezervasyon,
  CreateRezervasyonDto,
  RezervasyonDurum,
  rezervasyonDurumLabel,
  rezervasyonDurumVariant,
} from '@/types/rezervasyon'
import type { PaginatedResult } from '@/types/api'
import type { Tesis } from '@/types/tanimlar'

const DURUM_OPTIONS = Object.values(RezervasyonDurum)
  .filter((v) => typeof v === 'number') as RezervasyonDurum[]

const EMPTY_FORM: CreateRezervasyonDto = {
  tesisId: null,
  personId: null,
  startDate: '',
  endDate: '',
  durum: RezervasyonDurum.Beklemede,
  notes: '',
}

export default function RezervasyonPage() {
  const [data, setData] = useState<PaginatedResult<Rezervasyon> | null>(null)
  const [tesisler, setTesisler] = useState<Tesis[]>([])
  const [loading, setLoading] = useState(false)
  const [page, setPage] = useState(1)
  const [filterTesisId, setFilterTesisId] = useState('')
  const [filterFrom, setFilterFrom] = useState('')
  const [filterTo, setFilterTo] = useState('')
  const [filterDurum, setFilterDurum] = useState<RezervasyonDurum | ''>('')

  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<Rezervasyon | null>(null)
  const [form, setForm] = useState<CreateRezervasyonDto>(EMPTY_FORM)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    siteApi.get<Tesis[]>('/api/tesisler')
      .then((r) => setTesisler(r.data ?? []))
      .catch(() => {})
  }, [])

  const load = useCallback(() => {
    setLoading(true)
    rezervasyonApi.getAll({
      tesisId: filterTesisId || undefined,
      from: filterFrom || undefined,
      to: filterTo || undefined,
      durum: filterDurum === '' ? undefined : filterDurum,
      page,
      pageSize: 50,
    })
      .then((r) => setData(r.data))
      .catch(() => showError('Rezervasyonlar yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, filterTesisId, filterFrom, filterTo, filterDurum])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setEditing(null)
    setForm(EMPTY_FORM)
    setOpen(true)
  }

  const openEdit = (r: Rezervasyon) => {
    setEditing(r)
    setForm({
      tesisId:   r.tesisId ?? null,
      personId:  r.personId ?? null,
      startDate: r.startDate,
      endDate:   r.endDate,
      durum:     r.durum,
      notes:     r.notes ?? '',
    })
    setOpen(true)
  }

  const handleSave = async () => {
    if (!form.startDate || !form.endDate) { showError('Başlangıç ve bitiş tarihi zorunludur.'); return }
    setSaving(true)
    try {
      if (editing) {
        await rezervasyonApi.update(editing.id, form)
        showSuccess('Rezervasyon güncellendi.')
      } else {
        await rezervasyonApi.create(form)
        showSuccess('Rezervasyon oluşturuldu.')
      }
      setOpen(false)
      load()
    } catch {
      showError('İşlem başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu rezervasyonu silmek istiyor musunuz?')) return
    try {
      await rezervasyonApi.delete(id)
      showSuccess('Rezervasyon silindi.')
      load()
    } catch {
      showError('Silme işlemi başarısız.')
    }
  }

  const f = (key: keyof CreateRezervasyonDto, val: unknown) =>
    setForm((prev) => ({ ...prev, [key]: val }))

  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / 50)) : 1

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Rezervasyonlar</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" /> Yeni Rezervasyon
        </Button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1">
          <Label className="text-xs">Tesis</Label>
          <select
            value={filterTesisId}
            onChange={(e) => setFilterTesisId(e.target.value)}
            className="h-8 rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Tümü</option>
            {tesisler.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Durum</Label>
          <select
            value={filterDurum}
            onChange={(e) => setFilterDurum(e.target.value === '' ? '' : Number(e.target.value) as RezervasyonDurum)}
            className="h-8 rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Tümü</option>
            {DURUM_OPTIONS.map((d) => <option key={d} value={d}>{rezervasyonDurumLabel[d]}</option>)}
          </select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Başlangıç</Label>
          <Input type="date" value={filterFrom} onChange={(e) => setFilterFrom(e.target.value)} className="h-8" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Bitiş</Label>
          <Input type="date" value={filterTo} onChange={(e) => setFilterTo(e.target.value)} className="h-8" />
        </div>
        <Button size="sm" variant="outline" onClick={() => { setPage(1); load() }}>Filtrele</Button>
      </div>

      {/* Table */}
      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tesis</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Başlangıç</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Bitiş</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Durum</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Notlar</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">İşlem</th>
              </tr>
            </thead>
            <tbody>
              {!data || data.items.length === 0 ? (
                <tr><td colSpan={6} className="text-center py-12 text-muted-foreground">Rezervasyon yok.</td></tr>
              ) : data.items.map((r) => (
                <tr key={r.id} className="border-b last:border-0 hover:bg-muted/20">
                  <td className="px-3 py-2">{r.tesisAdi ?? <span className="text-muted-foreground">—</span>}</td>
                  <td className="px-3 py-2">{r.startDate}</td>
                  <td className="px-3 py-2">{r.endDate}</td>
                  <td className="px-3 py-2">
                    <Badge variant={rezervasyonDurumVariant[r.durum]}>{rezervasyonDurumLabel[r.durum]}</Badge>
                  </td>
                  <td className="px-3 py-2 max-w-[200px] truncate text-muted-foreground">{r.notes}</td>
                  <td className="px-3 py-2 text-right">
                    <div className="flex justify-end gap-1">
                      <Button size="sm" variant="ghost" onClick={() => openEdit(r)}>
                        <Pencil className="h-3.5 w-3.5" />
                      </Button>
                      <Button size="sm" variant="ghost" className="text-destructive" onClick={() => handleDelete(r.id)}>
                        <Trash2 className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-3 text-sm">
          <span className="text-muted-foreground">Sayfa {page} / {totalPages} &nbsp;·&nbsp; {data?.totalCount ?? 0} kayıt</span>
          <div className="flex gap-1">
            <Button size="sm" variant="outline" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>Önceki</Button>
            <Button size="sm" variant="outline" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>Sonraki</Button>
          </div>
        </div>
      )}

      {/* Dialog */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>{editing ? 'Rezervasyon Düzenle' : 'Yeni Rezervasyon'}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3 py-2">
            <div className="space-y-1">
              <Label className="text-xs">Tesis</Label>
              <select
                value={form.tesisId ?? ''}
                onChange={(e) => f('tesisId', e.target.value || null)}
                className="h-9 w-full rounded-md border bg-background px-3 text-sm"
              >
                <option value="">Seç (opsiyonel)...</option>
                {tesisler.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
              </select>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Başlangıç *</Label>
                <Input type="date" value={form.startDate} onChange={(e) => f('startDate', e.target.value)} />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Bitiş *</Label>
                <Input type="date" value={form.endDate} onChange={(e) => f('endDate', e.target.value)} />
              </div>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Durum</Label>
              <select
                value={form.durum}
                onChange={(e) => f('durum', Number(e.target.value) as RezervasyonDurum)}
                className="h-9 w-full rounded-md border bg-background px-3 text-sm"
              >
                {DURUM_OPTIONS.map((d) => <option key={d} value={d}>{rezervasyonDurumLabel[d]}</option>)}
              </select>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Notlar</Label>
              <Input value={form.notes ?? ''} onChange={(e) => f('notes', e.target.value)} placeholder="Opsiyonel..." />
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="outline" onClick={() => setOpen(false)}>İptal</Button>
            <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
