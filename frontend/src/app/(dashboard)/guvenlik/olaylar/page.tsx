'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, ShieldAlert, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { olaylarApi } from '@/lib/api/guvenlik'
import type { Olay, CreateOlayDto, UpdateOlayDto } from '@/types/guvenlik'
import { OlayTipi, OlayTipiLabel, OlayDurum, OlayDurumLabel } from '@/types/guvenlik'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const TIPLER = [OlayTipi.Hirsizlik, OlayTipi.Vandalizm, OlayTipi.Kaza, OlayTipi.Kavga, OlayTipi.Yangin, OlayTipi.Diger]
const DURUMLAR = [OlayDurum.Acik, OlayDurum.Inceleniyor, OlayDurum.Kapandi]
const durumVariant = (d: OlayDurum) => d === OlayDurum.Kapandi ? 'secondary' : d === OlayDurum.Inceleniyor ? 'default' : 'destructive'

export default function OlaylarPage() {
  const [items, setItems] = useState<Olay[]>([])
  const [total, setTotal] = useState(0); const [page, setPage] = useState(1); const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState(''); const [search, setSearch] = useState('')
  const [filterDurum, setFilterDurum] = useState<OlayDurum | ''>('')

  const [panelOpen, setPanelOpen] = useState(false); const [panelMode, setPanelMode] = useState<'create'|'edit'>('create')
  const [selected, setSelected] = useState<Olay | null>(null)
  const [fBaslik, setFBaslik] = useState(''); const [fAciklama, setFAciklama] = useState('')
  const [fTip, setFTip] = useState<OlayTipi>(OlayTipi.Diger); const [fKonum, setFKonum] = useState('')
  const [fOlayTarihi, setFOlayTarihi] = useState(''); const [fDurum, setFDurum] = useState<OlayDurum>(OlayDurum.Acik)
  const [saving, setSaving] = useState(false); const [deleteConfirm, setDeleteConfirm] = useState<string|null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await olaylarApi.getAll(page, PAGE_SIZE, search || undefined, filterDurum !== '' ? filterDurum : undefined)
      const d = res.data; setItems(d.items ?? []); setTotal(d.totalCount ?? 0); setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0)/PAGE_SIZE))
    } catch(e) { showApiError(e) } finally { setLoading(false) }
  }, [page, search, filterDurum])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const today = () => new Date().toISOString().split('T')[0]
  const resetForm = () => { setFBaslik(''); setFAciklama(''); setFTip(OlayTipi.Diger); setFKonum(''); setFOlayTarihi(today()); setFDurum(OlayDurum.Acik) }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: Olay) => {
    setFBaslik(item.baslik); setFAciklama(item.aciklama); setFTip(item.tip); setFKonum(item.konum ?? '')
    setFOlayTarihi(item.olayTarihi.split('T')[0]); setFDurum(item.durum)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!fBaslik.trim()) { showApiError('Title is required.'); return }
    if (!fAciklama.trim()) { showApiError('Description is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateOlayDto = { baslik: fBaslik, aciklama: fAciklama, olayTarihi: fOlayTarihi, tip: fTip, konum: fKonum||undefined }
        await olaylarApi.create(dto); showSuccess('Incident recorded.')
      } else if (selected) {
        const dto: UpdateOlayDto = { baslik: fBaslik, aciklama: fAciklama, olayTarihi: fOlayTarihi, tip: fTip, konum: fKonum||undefined, durum: fDurum }
        await olaylarApi.update(selected.id, dto); showSuccess('Updated.')
      }
      await load(); closePanel()
    } catch(e) { showApiError(e) } finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await olaylarApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) } catch(e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Incidents</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Incident</Button>
      </div>
      <div className="flex items-center gap-2 flex-wrap">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search title, location..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        <select value={filterDurum} onChange={e=>{ setFilterDurum(e.target.value === '' ? '' : Number(e.target.value) as OlayDurum); setPage(1) }} className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="">All Statuses</option>
          {DURUMLAR.map(d=><option key={d} value={d}>{OlayDurumLabel[d]}</option>)}
        </select>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        : items.length === 0 ? <div className="p-12 text-center"><ShieldAlert className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3"/><p className="text-muted-foreground text-sm">No incidents found.</p></div>
        : <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Title</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Type</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Date</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Location</th>
                <th className="text-center px-3 py-2 font-medium">Status</th>
                <th className="w-20 px-3 py-2"/>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-medium">{item.baslik}</td>
                  <td className="px-3 py-2.5 hidden md:table-cell"><Badge variant="outline" className="text-xs">{OlayTipiLabel[item.tip]}</Badge></td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground hidden lg:table-cell">{new Date(item.olayTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.konum ?? '—'}</td>
                  <td className="px-3 py-2.5 text-center"><Badge variant={durumVariant(item.durum)} className="text-xs">{OlayDurumLabel[item.durum]}</Badge></td>
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
              <h2 className="font-semibold">{panelMode==='create' ? 'New Incident' : 'Edit Incident'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4"/></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Title <span className="text-destructive">*</span></Label>
                <Input value={fBaslik} onChange={e=>setFBaslik(e.target.value)} placeholder="Brief title" maxLength={300}/>
              </div>
              <div className="space-y-1.5">
                <Label>Incident Date <span className="text-destructive">*</span></Label>
                <Input type="date" value={fOlayTarihi} onChange={e=>setFOlayTarihi(e.target.value)}/>
              </div>
              <div className="space-y-1.5">
                <Label>Type</Label>
                <select value={fTip} onChange={e=>setFTip(Number(e.target.value) as OlayTipi)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  {TIPLER.map(t=><option key={t} value={t}>{OlayTipiLabel[t]}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Location</Label>
                <Input value={fKonum} onChange={e=>setFKonum(e.target.value)} placeholder="e.g. Parking lot, Entrance" maxLength={200}/>
              </div>
              <div className="space-y-1.5">
                <Label>Description <span className="text-destructive">*</span></Label>
                <textarea value={fAciklama} onChange={e=>setFAciklama(e.target.value)} placeholder="Describe what happened..." maxLength={3000} rows={4} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring resize-none"/>
              </div>
              {panelMode==='edit' && (
                <div className="space-y-1.5">
                  <Label>Status</Label>
                  <select value={fDurum} onChange={e=>setFDurum(Number(e.target.value) as OlayDurum)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                    {DURUMLAR.map(d=><option key={d} value={d}>{OlayDurumLabel[d]}</option>)}
                  </select>
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
