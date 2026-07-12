'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, FileText, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi } from '@/lib/api/finans'
import { gelirTanimlariApi } from '@/lib/api/tanimlar'
import type { BorcMakbuzu, CreateBorcMakbuzuDto, UpdateBorcMakbuzuDto } from '@/types/finans'
import type { GelirTanimi } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function BorcMakbuzuPage() {
  const [items, setItems] = useState<BorcMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const [gelirTanimlari, setGelirTanimlari] = useState<GelirTanimi[]>([])
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<BorcMakbuzu | null>(null)

  const [formDonem, setFormDonem] = useState('')
  const [formSonOdemeTarihi, setFormSonOdemeTarihi] = useState('')
  const [formBorcluAdi, setFormBorcluAdi] = useState('')
  const [formGelirTanimiId, setFormGelirTanimiId] = useState('')
  const [formTutar, setFormTutar] = useState('')
  const [formGecikmeTutari, setFormGecikmeTutari] = useState('0')
  const [formOdenenTutar, setFormOdenenTutar] = useState('0')
  const [formAciklama, setFormAciklama] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await borcMakbuzlariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { gelirTanimlariApi.getAll().then(r => setGelirTanimlari(r.data.filter(g => g.isActive))) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const openCreate = () => {
    setFormDonem(''); setFormSonOdemeTarihi(''); setFormBorcluAdi(''); setFormGelirTanimiId(''); setFormTutar(''); setFormGecikmeTutari('0'); setFormOdenenTutar('0'); setFormAciklama('')
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }
  const openEdit = (item: BorcMakbuzu) => {
    setFormDonem(item.donem ?? ''); setFormSonOdemeTarihi(item.sonOdemeTarihi ? item.sonOdemeTarihi.split('T')[0] : ''); setFormBorcluAdi(item.borcluAdi ?? '')
    setFormGelirTanimiId(''); setFormTutar(String(item.tutar)); setFormGecikmeTutari(String(item.gecikmeTutari)); setFormOdenenTutar(String(item.odenenTutar)); setFormAciklama(item.aciklama ?? '')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formTutar || parseFloat(formTutar) <= 0) { showApiError('Amount must be greater than zero.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateBorcMakbuzuDto = { donem: formDonem || undefined, sonOdemeTarihi: formSonOdemeTarihi || undefined, borcluAdi: formBorcluAdi || undefined, gelirTanimiId: formGelirTanimiId || undefined, tutar: parseFloat(formTutar), aciklama: formAciklama || undefined }
        await borcMakbuzlariApi.create(dto)
        showSuccess('Debt receipt created.')
      } else if (selected) {
        const dto: UpdateBorcMakbuzuDto = { donem: formDonem || undefined, sonOdemeTarihi: formSonOdemeTarihi || undefined, borcluAdi: formBorcluAdi || undefined, gelirTanimiId: formGelirTanimiId || undefined, tutar: parseFloat(formTutar), gecikmeTutari: parseFloat(formGecikmeTutari) || 0, odenenTutar: parseFloat(formOdenenTutar) || 0, aciklama: formAciklama || undefined }
        await borcMakbuzlariApi.update(selected.id, dto)
        showSuccess('Debt receipt updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await borcMakbuzlariApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Debt Receipts</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Receipt</Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search by ref. no, name..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><FileText className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">No debt receipts found.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Ref No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Date</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Period</th>
                <th className="text-left px-3 py-2 font-medium">Debtor</th>
                <th className="text-right px-3 py-2 font-medium">Amount</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Paid</th>
                <th className="text-right px-3 py-2 font-medium">Balance</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.islemTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.donem ?? '—'}</td>
                  <td className="px-3 py-2.5">{item.borcluAdi ?? item.unitDoorNumber ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.tutar)}</td>
                  <td className="px-3 py-2.5 text-right text-muted-foreground hidden md:table-cell">{fmt(item.odenenTutar)}</td>
                  <td className="px-3 py-2.5 text-right">
                    <Badge variant={item.kalanTutar <= 0 ? 'secondary' : 'default'} className="text-xs">{fmt(item.kalanTutar)}</Badge>
                  </td>
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
          </table>
        )}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-end gap-2 text-sm">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Previous</Button>
          <span className="text-muted-foreground">{page} / {totalPages}</span>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Next</Button>
        </div>
      )}

      {panelOpen && (
        <div className="fixed inset-0 z-40 flex">
          <div className="flex-1 bg-black/30" onClick={closePanel} />
          <div className="w-full max-w-sm bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Debt Receipt' : 'Edit Debt Receipt'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="borcluAdi">Debtor Name</Label>
                <Input id="borcluAdi" value={formBorcluAdi} onChange={e => setFormBorcluAdi(e.target.value)} placeholder="Name of debtor" maxLength={200} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="donem">Period</Label>
                <Input id="donem" value={formDonem} onChange={e => setFormDonem(e.target.value)} placeholder="e.g. 2026-06" maxLength={10} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="sonOdeme">Due Date</Label>
                <Input id="sonOdeme" type="date" value={formSonOdemeTarihi} onChange={e => setFormSonOdemeTarihi(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="gelirTanimi">Income Category</Label>
                <select id="gelirTanimi" value={formGelirTanimiId} onChange={e => setFormGelirTanimiId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— None —</option>
                  {gelirTanimlari.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Amount (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" min={0} step="0.01" value={formTutar} onChange={e => setFormTutar(e.target.value)} placeholder="0.00" />
              </div>
              {panelMode === 'edit' && <>
                <div className="space-y-1.5">
                  <Label htmlFor="gecikme">Late Fee (₺)</Label>
                  <Input id="gecikme" type="number" min={0} step="0.01" value={formGecikmeTutari} onChange={e => setFormGecikmeTutari(e.target.value)} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="odenen">Amount Paid (₺)</Label>
                  <Input id="odenen" type="number" min={0} step="0.01" value={formOdenenTutar} onChange={e => setFormOdenenTutar(e.target.value)} />
                </div>
              </>}
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Notes</Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Optional notes" maxLength={500} />
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
