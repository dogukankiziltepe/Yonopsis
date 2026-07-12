'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Package, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { kayipEsyaApi } from '@/lib/api/guvenlik'
import type { KayipEsya, CreateKayipEsyaDto, UpdateKayipEsyaDto } from '@/types/guvenlik'
import { KayipEsyaDurum, KayipEsyaDurumLabel } from '@/types/guvenlik'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const DURUMLAR = [KayipEsyaDurum.Beklemede, KayipEsyaDurum.Bulundu, KayipEsyaDurum.TeslimEdildi]
const durumVariant = (d: KayipEsyaDurum) => d === KayipEsyaDurum.TeslimEdildi ? 'secondary' : d === KayipEsyaDurum.Bulundu ? 'default' : 'outline'

export default function KayipEsyaPage() {
  const [items, setItems] = useState<KayipEsya[]>([])
  const [total, setTotal] = useState(0); const [page, setPage] = useState(1); const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState(''); const [search, setSearch] = useState('')
  const [filterDurum, setFilterDurum] = useState<KayipEsyaDurum | ''>('')

  const [panelOpen, setPanelOpen] = useState(false); const [panelMode, setPanelMode] = useState<'create'|'edit'>('create')
  const [selected, setSelected] = useState<KayipEsya | null>(null)
  const [fEsyaAdi, setFEsyaAdi] = useState(''); const [fAciklama, setFAciklama] = useState('')
  const [fBulunanYer, setFBulunanYer] = useState(''); const [fBulunanTarih, setFBulunanTarih] = useState('')
  const [fSahipAdi, setFSahipAdi] = useState(''); const [fSahipIletisim, setFSahipIletisim] = useState('')
  const [fDurum, setFDurum] = useState<KayipEsyaDurum>(KayipEsyaDurum.Beklemede)
  const [saving, setSaving] = useState(false); const [deleteConfirm, setDeleteConfirm] = useState<string|null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await kayipEsyaApi.getAll(page, PAGE_SIZE, search || undefined, filterDurum !== '' ? filterDurum : undefined)
      const d = res.data; setItems(d.items ?? []); setTotal(d.totalCount ?? 0); setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0)/PAGE_SIZE))
    } catch(e) { showApiError(e) } finally { setLoading(false) }
  }, [page, search, filterDurum])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const today = () => new Date().toISOString().split('T')[0]
  const resetForm = () => { setFEsyaAdi(''); setFAciklama(''); setFBulunanYer(''); setFBulunanTarih(today()); setFSahipAdi(''); setFSahipIletisim(''); setFDurum(KayipEsyaDurum.Beklemede) }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: KayipEsya) => {
    setFEsyaAdi(item.esyaAdi); setFAciklama(item.aciklama ?? ''); setFBulunanYer(item.bulunanYer ?? '')
    setFBulunanTarih(item.bulunanTarih.split('T')[0]); setFSahipAdi(item.sahipAdi ?? ''); setFSahipIletisim(item.sahipIletisim ?? '')
    setFDurum(item.durum); setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!fEsyaAdi.trim()) { showApiError('Item name is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateKayipEsyaDto = { esyaAdi: fEsyaAdi, aciklama: fAciklama||undefined, bulunanYer: fBulunanYer||undefined, bulunanTarih: fBulunanTarih, sahipAdi: fSahipAdi||undefined, sahipIletisim: fSahipIletisim||undefined }
        await kayipEsyaApi.create(dto); showSuccess('Lost item recorded.')
      } else if (selected) {
        const dto: UpdateKayipEsyaDto = { esyaAdi: fEsyaAdi, aciklama: fAciklama||undefined, bulunanYer: fBulunanYer||undefined, bulunanTarih: fBulunanTarih, sahipAdi: fSahipAdi||undefined, sahipIletisim: fSahipIletisim||undefined, durum: fDurum }
        await kayipEsyaApi.update(selected.id, dto); showSuccess('Updated.')
      }
      await load(); closePanel()
    } catch(e) { showApiError(e) } finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await kayipEsyaApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) } catch(e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Lost & Found</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Log Item</Button>
      </div>
      <div className="flex items-center gap-2 flex-wrap">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search item, owner..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        <select value={filterDurum} onChange={e=>{ setFilterDurum(e.target.value === '' ? '' : Number(e.target.value) as KayipEsyaDurum); setPage(1) }} className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
          <option value="">All Statuses</option>
          {DURUMLAR.map(d=><option key={d} value={d}>{KayipEsyaDurumLabel[d]}</option>)}
        </select>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        : items.length === 0 ? <div className="p-12 text-center"><Package className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3"/><p className="text-muted-foreground text-sm">No lost & found items.</p></div>
        : <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Item</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Found Location</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Date Found</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Owner</th>
                <th className="text-center px-3 py-2 font-medium">Status</th>
                <th className="w-20 px-3 py-2"/>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-medium">{item.esyaAdi}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell">{item.bulunanYer ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs text-muted-foreground hidden lg:table-cell">{new Date(item.bulunanTarih).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 hidden md:table-cell">{item.sahipAdi ?? <span className="text-muted-foreground">Unknown</span>}</td>
                  <td className="px-3 py-2.5 text-center"><Badge variant={durumVariant(item.durum)} className="text-xs">{KayipEsyaDurumLabel[item.durum]}</Badge></td>
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
              <h2 className="font-semibold">{panelMode==='create' ? 'Log Found Item' : 'Edit Item'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4"/></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Item Name <span className="text-destructive">*</span></Label>
                <Input value={fEsyaAdi} onChange={e=>setFEsyaAdi(e.target.value)} placeholder="e.g. Black wallet, Keys" maxLength={200}/>
              </div>
              <div className="space-y-1.5">
                <Label>Date Found <span className="text-destructive">*</span></Label>
                <Input type="date" value={fBulunanTarih} onChange={e=>setFBulunanTarih(e.target.value)}/>
              </div>
              <div className="space-y-1.5">
                <Label>Found Location</Label>
                <Input value={fBulunanYer} onChange={e=>setFBulunanYer(e.target.value)} placeholder="e.g. Lobby, Elevator" maxLength={300}/>
              </div>
              <div className="space-y-1.5">
                <Label>Description</Label>
                <Input value={fAciklama} onChange={e=>setFAciklama(e.target.value)} placeholder="Additional details" maxLength={1000}/>
              </div>
              <div className="border-t pt-4 space-y-4">
                <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide">Owner Info (optional)</p>
                <div className="space-y-1.5">
                  <Label>Owner Name</Label>
                  <Input value={fSahipAdi} onChange={e=>setFSahipAdi(e.target.value)} placeholder="Name" maxLength={200}/>
                </div>
                <div className="space-y-1.5">
                  <Label>Contact</Label>
                  <Input value={fSahipIletisim} onChange={e=>setFSahipIletisim(e.target.value)} placeholder="Phone or email" maxLength={200}/>
                </div>
              </div>
              {panelMode==='edit' && (
                <div className="space-y-1.5">
                  <Label>Status</Label>
                  <select value={fDurum} onChange={e=>setFDurum(Number(e.target.value) as KayipEsyaDurum)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                    {DURUMLAR.map(d=><option key={d} value={d}>{KayipEsyaDurumLabel[d]}</option>)}
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
