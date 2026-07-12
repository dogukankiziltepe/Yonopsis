'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Users, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { ziyaretciGirisCikisApi } from '@/lib/api/guvenlik'
import type { ZiyaretciGirisCikis, CreateZiyaretciGirisCikisDto, UpdateZiyaretciGirisCikisDto } from '@/types/guvenlik'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmtDt = (s: string) => new Date(s).toLocaleString('tr-TR', { dateStyle: 'short', timeStyle: 'short' })
const toLocalDateTimeInput = (iso: string) => {
  const d = new Date(iso); const pad = (n: number) => String(n).padStart(2,'0')
  return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}
const nowInput = () => toLocalDateTimeInput(new Date().toISOString())

export default function ZiyaretciGirisCikisPage() {
  const [items, setItems] = useState<ZiyaretciGirisCikis[]>([])
  const [total, setTotal] = useState(0); const [page, setPage] = useState(1); const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState(''); const [search, setSearch] = useState('')

  const [panelOpen, setPanelOpen] = useState(false); const [panelMode, setPanelMode] = useState<'create'|'edit'>('create')
  const [selected, setSelected] = useState<ZiyaretciGirisCikis | null>(null)
  const [fGelensAdi, setFGelensAdi] = useState(''); const [fGeldigiKisi, setFGeldigiKisi] = useState('')
  const [fZiyaretAmaci, setFZiyaretAmaci] = useState(''); const [fGirisSaati, setFGirisSaati] = useState(nowInput())
  const [fCikisSaati, setFCikisSaati] = useState(''); const [fPlaka, setFPlaka] = useState(''); const [fAciklama, setFAciklama] = useState('')
  const [saving, setSaving] = useState(false); const [deleteConfirm, setDeleteConfirm] = useState<string|null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await ziyaretciGirisCikisApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data; setItems(d.items ?? []); setTotal(d.totalCount ?? 0); setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0)/PAGE_SIZE))
    } catch(e) { showApiError(e) } finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const resetForm = () => { setFGelensAdi(''); setFGeldigiKisi(''); setFZiyaretAmaci(''); setFGirisSaati(nowInput()); setFCikisSaati(''); setFPlaka(''); setFAciklama('') }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: ZiyaretciGirisCikis) => {
    setFGelensAdi(item.gelensAdi); setFGeldigiKisi(item.geldigiKisi ?? ''); setFZiyaretAmaci(item.ziyaretAmaci ?? '')
    setFGirisSaati(toLocalDateTimeInput(item.girisSaati)); setFCikisSaati(item.cikisSaati ? toLocalDateTimeInput(item.cikisSaati) : '')
    setFPlaka(item.plaka ?? ''); setFAciklama(item.aciklama ?? '')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!fGelensAdi.trim()) { showApiError('Visitor name is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateZiyaretciGirisCikisDto = { gelensAdi: fGelensAdi, geldigiKisi: fGeldigiKisi||undefined, ziyaretAmaci: fZiyaretAmaci||undefined, girisSaati: new Date(fGirisSaati).toISOString(), plaka: fPlaka||undefined, aciklama: fAciklama||undefined }
        await ziyaretciGirisCikisApi.create(dto); showSuccess('Visitor entry recorded.')
      } else if (selected) {
        const dto: UpdateZiyaretciGirisCikisDto = { gelensAdi: fGelensAdi, geldigiKisi: fGeldigiKisi||undefined, ziyaretAmaci: fZiyaretAmaci||undefined, girisSaati: new Date(fGirisSaati).toISOString(), cikisSaati: fCikisSaati ? new Date(fCikisSaati).toISOString() : undefined, plaka: fPlaka||undefined, aciklama: fAciklama||undefined }
        await ziyaretciGirisCikisApi.update(selected.id, dto); showSuccess('Updated.')
      }
      await load(); closePanel()
    } catch(e) { showApiError(e) } finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await ziyaretciGirisCikisApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) } catch(e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Visitor Entry / Exit</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Log Entry</Button>
      </div>
      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search visitor, plate..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        : items.length === 0 ? <div className="p-12 text-center"><Users className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3"/><p className="text-muted-foreground text-sm">No visitor entries found.</p></div>
        : <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Visitor</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Visiting</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Unit</th>
                <th className="text-left px-3 py-2 font-medium">Entry</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Exit</th>
                <th className="w-20 px-3 py-2"/>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-medium">{item.gelensAdi}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell">{item.geldigiKisi ?? '—'}</td>
                  <td className="px-3 py-2.5 hidden lg:table-cell">{item.unitDoorNumber ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs">{fmtDt(item.girisSaati)}</td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground hidden md:table-cell">{item.cikisSaati ? fmtDt(item.cikisSaati) : <span className="text-amber-600 font-medium">Inside</span>}</td>
                  <td className="px-3 py-2.5">
                    <div className="flex gap-1 justify-end">
                      <Button variant="ghost" size="sm" className="h-6 px-2 text-xs" onClick={() => openEdit(item)}>Edit</Button>
                      {deleteConfirm === item.id
                        ? <Button variant="destructive" size="sm" className="h-6 px-2 text-xs" onClick={() => handleDelete(item.id)}>Confirm</Button>
                        : <Button variant="ghost" size="sm" className="h-6 px-2 text-xs text-destructive" onClick={() => setDeleteConfirm(item.id)}>Delete</Button>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>}
      </div>
      {totalPages > 1 && (
        <div className="flex items-center justify-end gap-2 text-sm">
          <Button variant="outline" size="sm" disabled={page<=1} onClick={() => setPage(p=>p-1)}>Previous</Button>
          <span className="text-muted-foreground">{page} / {totalPages}</span>
          <Button variant="outline" size="sm" disabled={page>=totalPages} onClick={() => setPage(p=>p+1)}>Next</Button>
        </div>
      )}
      {panelOpen && (
        <div className="fixed inset-0 z-40 flex">
          <div className="flex-1 bg-black/30" onClick={closePanel}/>
          <div className="w-full max-w-sm bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">{panelMode==='create' ? 'Log Visitor Entry' : 'Edit Visitor Entry'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4"/></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Visitor Name <span className="text-destructive">*</span></Label>
                <Input value={fGelensAdi} onChange={e=>setFGelensAdi(e.target.value)} placeholder="Full name" maxLength={200}/>
              </div>
              <div className="space-y-1.5">
                <Label>Visiting Person / Unit</Label>
                <Input value={fGeldigiKisi} onChange={e=>setFGeldigiKisi(e.target.value)} placeholder="Who are they visiting?" maxLength={200}/>
              </div>
              <div className="space-y-1.5">
                <Label>Purpose of Visit</Label>
                <Input value={fZiyaretAmaci} onChange={e=>setFZiyaretAmaci(e.target.value)} placeholder="e.g. Delivery, Guest" maxLength={500}/>
              </div>
              <div className="space-y-1.5">
                <Label>Vehicle Plate</Label>
                <Input value={fPlaka} onChange={e=>setFPlaka(e.target.value.toUpperCase())} placeholder="Optional" maxLength={20}/>
              </div>
              <div className="space-y-1.5">
                <Label>Entry Time <span className="text-destructive">*</span></Label>
                <Input type="datetime-local" value={fGirisSaati} onChange={e=>setFGirisSaati(e.target.value)}/>
              </div>
              {panelMode==='edit' && (
                <div className="space-y-1.5">
                  <Label>Exit Time</Label>
                  <Input type="datetime-local" value={fCikisSaati} onChange={e=>setFCikisSaati(e.target.value)}/>
                </div>
              )}
              <div className="space-y-1.5">
                <Label>Notes</Label>
                <Input value={fAciklama} onChange={e=>setFAciklama(e.target.value)} placeholder="Optional" maxLength={500}/>
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
