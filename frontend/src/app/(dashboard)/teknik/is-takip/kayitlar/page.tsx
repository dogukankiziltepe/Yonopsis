'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, ClipboardList, Search, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { isEmirleriApi, departmanlarApi, talepTipleriApi, ortakAlanlarApi } from '@/lib/api/teknik'
import type { IsEmri, CreateIsEmriDto, UpdateIsEmriDto, Departman, TalepTipi, OrtakAlan } from '@/types/teknik'
import { IsEmriDurum, IsEmriDurumLabel, IsEmriOncelik, IsEmriOncelikLabel } from '@/types/teknik'
import { showSuccess, showApiError } from '@/lib/toast'

const DURUM_COLORS: Record<IsEmriDurum, string> = {
  [IsEmriDurum.YeniTalep]: 'bg-blue-100 text-blue-800',
  [IsEmriDurum.Atandi]: 'bg-yellow-100 text-yellow-800',
  [IsEmriDurum.Devam]: 'bg-orange-100 text-orange-800',
  [IsEmriDurum.Tamamlandi]: 'bg-green-100 text-green-800',
  [IsEmriDurum.Iptal]: 'bg-gray-100 text-gray-600',
}

const ONCELIK_COLORS: Record<IsEmriOncelik, string> = {
  [IsEmriOncelik.Dusuk]: 'bg-gray-100 text-gray-600',
  [IsEmriOncelik.Normal]: 'bg-blue-100 text-blue-700',
  [IsEmriOncelik.Yuksek]: 'bg-orange-100 text-orange-700',
  [IsEmriOncelik.Kritik]: 'bg-red-100 text-red-700',
}

export default function IsKayitlarPage() {
  const [items, setItems] = useState<IsEmri[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [durumFilter, setDurumFilter] = useState<IsEmriDurum | ''>('')

  const [departmanlar, setDepartmanlar] = useState<Departman[]>([])
  const [talepTipleri, setTalepTipleri] = useState<TalepTipi[]>([])
  const [ortakAlanlar, setOrtakAlanlar] = useState<OrtakAlan[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<IsEmri | null>(null)
  const [form, setForm] = useState<CreateIsEmriDto>({
    baslik: '', oncelik: IsEmriOncelik.Normal,
  })
  const [formDurum, setFormDurum] = useState<IsEmriDurum>(IsEmriDurum.YeniTalep)
  const [formIslemBitis, setFormIslemBitis] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const loadMeta = useCallback(async () => {
    try {
      const [d, t, o] = await Promise.all([departmanlarApi.getAll(), talepTipleriApi.getAll(), ortakAlanlarApi.getAll()])
      setDepartmanlar(d.data.filter(x => x.isActive))
      setTalepTipleri(t.data.filter(x => x.isActive))
      setOrtakAlanlar(o.data.filter(x => x.isActive))
    } catch { /* silent */ }
  }, [])

  const load = useCallback(async (p = page) => {
    setLoading(true)
    try {
      const res = await isEmirleriApi.getAll(p, pageSize, search || undefined, durumFilter !== '' ? durumFilter : undefined)
      setItems(res.data.items)
      setTotal(res.data.totalCount)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search, durumFilter])

  useEffect(() => { loadMeta() }, [loadMeta])
  useEffect(() => { load() }, [load])

  const handleSearch = () => { setSearch(searchInput); setPage(1) }

  const openCreate = () => {
    setForm({ baslik: '', oncelik: IsEmriOncelik.Normal })
    setFormDurum(IsEmriDurum.YeniTalep); setFormIslemBitis('')
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: IsEmri) => {
    setForm({
      baslik: item.baslik, aciklama: item.aciklama,
      talepTipiId: item.talepTipiId, departmanId: item.departmanId,
      ortakAlanId: item.ortakAlanId, unitId: item.unitId,
      oncelik: item.oncelik, atananKisiAdi: item.atananKisiAdi,
      islemBaslangic: item.islemBaslangic?.slice(0, 16), notlar: item.notlar,
    })
    setFormDurum(item.durum)
    setFormIslemBitis(item.islemBitis?.slice(0, 16) ?? '')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!form.baslik.trim()) { showApiError('Title is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        await isEmirleriApi.create({ ...form, baslik: form.baslik.trim() })
        showSuccess('Work order created.')
      } else if (selected) {
        const dto: UpdateIsEmriDto = { ...form, baslik: form.baslik.trim(), durum: formDurum, islemBitis: formIslemBitis || undefined }
        await isEmirleriApi.update(selected.id, dto)
        showSuccess('Work order updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await isEmirleriApi.delete(id)
      showSuccess('Work order deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  const totalPages = Math.ceil(total / pageSize)

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Work Orders</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Order</Button>
      </div>

      <div className="flex gap-2 flex-wrap">
        <div className="flex gap-1.5 flex-1 min-w-48">
          <Input placeholder="Search title or assignee..." value={searchInput} onChange={e => setSearchInput(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && handleSearch()} className="h-8 text-sm" />
          <Button size="sm" variant="outline" onClick={handleSearch} className="h-8 px-2"><Search className="h-3.5 w-3.5" /></Button>
        </div>
        <select value={durumFilter} onChange={e => { setDurumFilter(e.target.value as IsEmriDurum | ''); setPage(1) }}
          className="h-8 text-sm border rounded-md px-2 bg-background">
          <option value="">All Statuses</option>
          {Object.values(IsEmriDurum).filter(v => typeof v === 'number').map(v => (
            <option key={v} value={v}>{IsEmriDurumLabel[v as IsEmriDurum]}</option>
          ))}
        </select>
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <ClipboardList className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No work orders found.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Title</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Department</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">Assignee</th>
                <th className="text-left px-4 py-2 font-medium w-28">Priority</th>
                <th className="text-left px-4 py-2 font-medium w-32">Status</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{item.baslik}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.departmanAdi ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.atananKisiAdi ?? '—'}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-block px-2 py-0.5 rounded-full text-xs font-medium ${ONCELIK_COLORS[item.oncelik]}`}>
                      {IsEmriOncelikLabel[item.oncelik]}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <span className={`inline-block px-2 py-0.5 rounded-full text-xs font-medium ${DURUM_COLORS[item.durum]}`}>
                      {IsEmriDurumLabel[item.durum]}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 justify-end">
                      <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => openEdit(item)}><Pencil className="h-3.5 w-3.5" /></Button>
                      {deleteConfirm === item.id ? (
                        <Button variant="destructive" size="sm" className="h-7 text-xs" onClick={() => handleDelete(item.id)}>Confirm</Button>
                      ) : (
                        <Button variant="ghost" size="icon" className="h-7 w-7 text-destructive hover:text-destructive" onClick={() => setDeleteConfirm(item.id)}><Trash2 className="h-3.5 w-3.5" /></Button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between text-sm text-muted-foreground">
          <span>{total} records</span>
          <div className="flex items-center gap-1">
            <Button variant="ghost" size="icon" className="h-7 w-7" disabled={page <= 1} onClick={() => setPage(p => p - 1)}><ChevronLeft className="h-4 w-4" /></Button>
            <span className="px-2">{page} / {totalPages}</span>
            <Button variant="ghost" size="icon" className="h-7 w-7" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}><ChevronRight className="h-4 w-4" /></Button>
          </div>
        </div>
      )}

      {panelOpen && (
        <div className="fixed inset-0 z-40 flex">
          <div className="flex-1 bg-black/30" onClick={closePanel} />
          <div className="w-full max-w-md bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Work Order' : 'Edit Work Order'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="baslik">Title <span className="text-destructive">*</span></Label>
                <Input id="baslik" value={form.baslik} onChange={e => setForm(f => ({ ...f, baslik: e.target.value }))} maxLength={300} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Description</Label>
                <textarea id="aciklama" value={form.aciklama ?? ''} onChange={e => setForm(f => ({ ...f, aciklama: e.target.value }))}
                  className="w-full border rounded-md px-3 py-2 text-sm min-h-[80px] bg-background resize-none" maxLength={2000} />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <Label>Priority</Label>
                  <select value={form.oncelik} onChange={e => setForm(f => ({ ...f, oncelik: Number(e.target.value) as IsEmriOncelik }))}
                    className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                    {Object.values(IsEmriOncelik).filter(v => typeof v === 'number').map(v => (
                      <option key={v} value={v}>{IsEmriOncelikLabel[v as IsEmriOncelik]}</option>
                    ))}
                  </select>
                </div>
                {panelMode === 'edit' && (
                  <div className="space-y-1.5">
                    <Label>Status</Label>
                    <select value={formDurum} onChange={e => setFormDurum(Number(e.target.value) as IsEmriDurum)}
                      className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                      {Object.values(IsEmriDurum).filter(v => typeof v === 'number').map(v => (
                        <option key={v} value={v}>{IsEmriDurumLabel[v as IsEmriDurum]}</option>
                      ))}
                    </select>
                  </div>
                )}
              </div>
              <div className="space-y-1.5">
                <Label>Department</Label>
                <select value={form.departmanId ?? ''} onChange={e => setForm(f => ({ ...f, departmanId: e.target.value || undefined }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  <option value="">— None —</option>
                  {departmanlar.map(d => <option key={d.id} value={d.id}>{d.ad}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Request Type</Label>
                <select value={form.talepTipiId ?? ''} onChange={e => setForm(f => ({ ...f, talepTipiId: e.target.value || undefined }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  <option value="">— None —</option>
                  {talepTipleri.map(t => <option key={t.id} value={t.id}>{t.ad}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Common Area</Label>
                <select value={form.ortakAlanId ?? ''} onChange={e => setForm(f => ({ ...f, ortakAlanId: e.target.value || undefined }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  <option value="">— None —</option>
                  {ortakAlanlar.map(o => <option key={o.id} value={o.id}>{o.ad}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="atanan">Assigned To</Label>
                <Input id="atanan" value={form.atananKisiAdi ?? ''} onChange={e => setForm(f => ({ ...f, atananKisiAdi: e.target.value }))} placeholder="Name" maxLength={200} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="baslangic">Start Date</Label>
                <Input id="baslangic" type="datetime-local" value={form.islemBaslangic ?? ''} onChange={e => setForm(f => ({ ...f, islemBaslangic: e.target.value }))} />
              </div>
              {panelMode === 'edit' && (
                <div className="space-y-1.5">
                  <Label htmlFor="bitis">End Date</Label>
                  <Input id="bitis" type="datetime-local" value={formIslemBitis} onChange={e => setFormIslemBitis(e.target.value)} />
                </div>
              )}
              <div className="space-y-1.5">
                <Label htmlFor="notlar">Notes</Label>
                <textarea id="notlar" value={form.notlar ?? ''} onChange={e => setForm(f => ({ ...f, notlar: e.target.value }))}
                  className="w-full border rounded-md px-3 py-2 text-sm min-h-[60px] bg-background resize-none" maxLength={2000} />
              </div>
            </div>
            <div className="border-t px-4 py-3 flex gap-2 justify-end">
              <Button variant="outline" onClick={closePanel}>Cancel</Button>
              <Button onClick={handleSave} disabled={saving}>{saving ? 'Saving...' : 'Save'}</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
