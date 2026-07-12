'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Gauge, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { daireSayaclarApi, anaSayaclarApi } from '@/lib/api/sayac'
import { unitsApi } from '@/lib/api/units'
import type { DaireSayac, CreateDaireSayacDto, UpdateDaireSayacDto, AnaSayac } from '@/types/sayac'
import { SayacTipi, SayacTipiLabel } from '@/types/sayac'
import { showSuccess, showApiError } from '@/lib/toast'

export default function DaireSayaclariPage() {
  const [items, setItems] = useState<DaireSayac[]>([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [anaSayacFilter, setAnaSayacFilter] = useState('')
  const [anaSayaclar, setAnaSayaclar] = useState<AnaSayac[]>([])
  const [units, setUnits] = useState<{ id: string; doorNumber: string }[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<DaireSayac | null>(null)
  const [form, setForm] = useState<CreateDaireSayacDto>({ unitId: '', anaSayacId: '', tip: SayacTipi.Elektrik })
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const loadMeta = useCallback(async () => {
    try {
      const [as, us] = await Promise.all([anaSayaclarApi.getAllList(), unitsApi.getAll()])
      setAnaSayaclar(as.data)
      setUnits(us.data.map((u: { id: string; doorNumber: string }) => ({ id: u.id, doorNumber: u.doorNumber })))
    } catch { /* silent */ }
  }, [])

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await daireSayaclarApi.getAll(1, 50, search || undefined, anaSayacFilter || undefined)
      setItems(res.data.items)
      setTotal(res.data.totalCount)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [search, anaSayacFilter])

  useEffect(() => { loadMeta() }, [loadMeta])
  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setForm({ unitId: '', anaSayacId: anaSayaclar[0]?.id ?? '', tip: SayacTipi.Elektrik }); setFormIsActive(true)
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: DaireSayac) => {
    setForm({ unitId: item.unitId, anaSayacId: item.anaSayacId, tip: item.tip, seriNo: item.seriNo, marka: item.marka, takimTarihi: item.takimTarihi?.slice(0, 10), aciklama: item.aciklama })
    setFormIsActive(item.isActive)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!form.unitId) { showApiError('Unit is required.'); return }
    if (!form.anaSayacId) { showApiError('Main meter is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        await daireSayaclarApi.create(form)
        showSuccess('Unit meter created.')
      } else if (selected) {
        const dto: UpdateDaireSayacDto = { ...form, isActive: formIsActive }
        await daireSayaclarApi.update(selected.id, dto)
        showSuccess('Unit meter updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await daireSayaclarApi.delete(id)
      showSuccess('Unit meter deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Unit Meters</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Meter</Button>
      </div>

      <div className="flex gap-2 flex-wrap">
        <div className="flex gap-1.5 flex-1 min-w-40">
          <Input placeholder="Search serial / unit..." value={searchInput} onChange={e => setSearchInput(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && setSearch(searchInput)} className="h-8 text-sm" />
          <Button size="sm" variant="outline" onClick={() => setSearch(searchInput)} className="h-8 px-2"><Search className="h-3.5 w-3.5" /></Button>
        </div>
        <select value={anaSayacFilter} onChange={e => setAnaSayacFilter(e.target.value)}
          className="h-8 text-sm border rounded-md px-2 bg-background">
          <option value="">All Main Meters</option>
          {anaSayaclar.map(a => <option key={a.id} value={a.id}>{a.ad}</option>)}
        </select>
        {total > 0 && <span className="text-sm text-muted-foreground self-center">{total} meters</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Gauge className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No unit meters yet.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Unit</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Main Meter</th>
                <th className="text-left px-4 py-2 font-medium w-28">Type</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">Serial No</th>
                <th className="text-left px-4 py-2 font-medium w-24">Status</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{item.unitDoorNumber ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.anaSayacAdi ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground">{SayacTipiLabel[item.tip]}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.seriNo ?? '—'}</td>
                  <td className="px-4 py-3">
                    <Badge variant={item.isActive ? 'default' : 'secondary'}>{item.isActive ? 'Active' : 'Inactive'}</Badge>
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

      {panelOpen && (
        <div className="fixed inset-0 z-40 flex">
          <div className="flex-1 bg-black/30" onClick={closePanel} />
          <div className="w-full max-w-sm bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Unit Meter' : 'Edit Unit Meter'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Unit <span className="text-destructive">*</span></Label>
                <select value={form.unitId} onChange={e => setForm(f => ({ ...f, unitId: e.target.value }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  <option value="">— Select Unit —</option>
                  {units.map(u => <option key={u.id} value={u.id}>{u.doorNumber}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Main Meter <span className="text-destructive">*</span></Label>
                <select value={form.anaSayacId} onChange={e => setForm(f => ({ ...f, anaSayacId: e.target.value }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  <option value="">— Select Main Meter —</option>
                  {anaSayaclar.map(a => <option key={a.id} value={a.id}>{a.ad}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Type</Label>
                <select value={form.tip} onChange={e => setForm(f => ({ ...f, tip: Number(e.target.value) as SayacTipi }))}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  {Object.values(SayacTipi).filter(v => typeof v === 'number').map(v => (
                    <option key={v} value={v}>{SayacTipiLabel[v as SayacTipi]}</option>
                  ))}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="seriNo">Serial No</Label>
                <Input id="seriNo" value={form.seriNo ?? ''} onChange={e => setForm(f => ({ ...f, seriNo: e.target.value }))} maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="marka">Brand</Label>
                <Input id="marka" value={form.marka ?? ''} onChange={e => setForm(f => ({ ...f, marka: e.target.value }))} maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="takim">Installation Date</Label>
                <Input id="takim" type="date" value={form.takimTarihi ?? ''} onChange={e => setForm(f => ({ ...f, takimTarihi: e.target.value }))} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Description</Label>
                <Input id="aciklama" value={form.aciklama ?? ''} onChange={e => setForm(f => ({ ...f, aciklama: e.target.value }))} maxLength={500} />
              </div>
              {panelMode === 'edit' && (
                <div className="flex items-center gap-2">
                  <input type="checkbox" id="isActive" checked={formIsActive} onChange={e => setFormIsActive(e.target.checked)} className="h-4 w-4" />
                  <Label htmlFor="isActive">Active</Label>
                </div>
              )}
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
