'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Car, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { aracGirisCikisApi } from '@/lib/api/guvenlik'
import type { AracGirisCikis, CreateAracGirisCikisDto, UpdateAracGirisCikisDto } from '@/types/guvenlik'
import { AracTipi, AracTipiLabel } from '@/types/guvenlik'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmtDt = (s: string) => new Date(s).toLocaleString('tr-TR', { dateStyle: 'short', timeStyle: 'short' })
const toLocalDateTimeInput = (iso: string) => {
  const d = new Date(iso); const pad = (n: number) => String(n).padStart(2,'0')
  return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}
const nowInput = () => toLocalDateTimeInput(new Date().toISOString())
const ARAC_TIPLERI = [AracTipi.Otomobil, AracTipi.Kamyon, AracTipi.Motosiklet, AracTipi.Minibus, AracTipi.Diger]

export default function AracGirisCikisPage() {
  const [items, setItems] = useState<AracGirisCikis[]>([])
  const [total, setTotal] = useState(0); const [page, setPage] = useState(1); const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState(''); const [search, setSearch] = useState('')

  const [panelOpen, setPanelOpen] = useState(false); const [panelMode, setPanelMode] = useState<'create'|'edit'>('create')
  const [selected, setSelected] = useState<AracGirisCikis | null>(null)
  const [fPlaka, setFPlaka] = useState(''); const [fSuruculAdi, setFSuruculAdi] = useState('')
  const [fAracTipi, setFAracTipi] = useState<AracTipi|''>(''); const [fGirisSaati, setFGirisSaati] = useState(nowInput())
  const [fCikisSaati, setFCikisSaati] = useState(''); const [fAciklama, setFAciklama] = useState('')
  const [saving, setSaving] = useState(false); const [deleteConfirm, setDeleteConfirm] = useState<string|null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await aracGirisCikisApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data; setItems(d.items ?? []); setTotal(d.totalCount ?? 0); setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0)/PAGE_SIZE))
    } catch(e) { showApiError(e) } finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const resetForm = () => { setFPlaka(''); setFSuruculAdi(''); setFAracTipi(''); setFGirisSaati(nowInput()); setFCikisSaati(''); setFAciklama('') }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: AracGirisCikis) => {
    setFPlaka(item.plaka); setFSuruculAdi(item.suruculAdi ?? ''); setFAracTipi(item.aracTipi ?? '')
    setFGirisSaati(toLocalDateTimeInput(item.girisSaati)); setFCikisSaati(item.cikisSaati ? toLocalDateTimeInput(item.cikisSaati) : '')
    setFAciklama(item.aciklama ?? ''); setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!fPlaka.trim()) { showApiError('Plate is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateAracGirisCikisDto = { plaka: fPlaka, suruculAdi: fSuruculAdi||undefined, aracTipi: fAracTipi||undefined, girisSaati: new Date(fGirisSaati).toISOString(), aciklama: fAciklama||undefined }
        await aracGirisCikisApi.create(dto); showSuccess('Vehicle entry recorded.')
      } else if (selected) {
        const dto: UpdateAracGirisCikisDto = { plaka: fPlaka, suruculAdi: fSuruculAdi||undefined, aracTipi: fAracTipi||undefined, girisSaati: new Date(fGirisSaati).toISOString(), cikisSaati: fCikisSaati ? new Date(fCikisSaati).toISOString() : undefined, aciklama: fAciklama||undefined }
        await aracGirisCikisApi.update(selected.id, dto); showSuccess('Updated.')
      }
      await load(); closePanel()
    } catch(e) { showApiError(e) } finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await aracGirisCikisApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) } catch(e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Vehicle Entry / Exit</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Log Vehicle</Button>
      </div>
      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search plate, driver..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        : items.length === 0 ? <div className="p-12 text-center"><Car className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3"/><p className="text-muted-foreground text-sm">No vehicle entries found.</p></div>
        : <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Plate</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Driver</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Type</th>
                <th className="text-left px-3 py-2 font-medium">Entry</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Exit</th>
                <th className="w-20 px-3 py-2"/>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono font-semibold">{item.plaka}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell">{item.suruculAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 hidden lg:table-cell">{item.aracTipi != null ? <Badge variant="secondary" className="text-xs">{AracTipiLabel[item.aracTipi]}</Badge> : '—'}</td>
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
              <h2 className="font-semibold">{panelMode==='create' ? 'Log Vehicle Entry' : 'Edit Vehicle Entry'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4"/></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Plate <span className="text-destructive">*</span></Label>
                <Input value={fPlaka} onChange={e=>setFPlaka(e.target.value.toUpperCase())} placeholder="e.g. 34ABC123" maxLength={20}/>
              </div>
              <div className="space-y-1.5">
                <Label>Driver Name</Label>
                <Input value={fSuruculAdi} onChange={e=>setFSuruculAdi(e.target.value)} placeholder="Optional" maxLength={200}/>
              </div>
              <div className="space-y-1.5">
                <Label>Vehicle Type</Label>
                <select value={fAracTipi} onChange={e=>setFAracTipi(e.target.value === '' ? '' : Number(e.target.value) as AracTipi)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Not specified —</option>
                  {ARAC_TIPLERI.map(t => <option key={t} value={t}>{AracTipiLabel[t]}</option>)}
                </select>
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
