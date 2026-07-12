'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, TrendingDown, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { giderFaturalariApi } from '@/lib/api/finans'
import { giderTanimlariApi } from '@/lib/api/tanimlar'
import type { Fatura, CreateFaturaDto, UpdateFaturaDto } from '@/types/finans'
import { FaturaTipi } from '@/types/finans'
import type { GiderTanimi } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function GiderFaturalariPage() {
  const [items, setItems] = useState<Fatura[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [tanimlar, setTanimlar] = useState<GiderTanimi[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<Fatura | null>(null)
  const [formCariAdi, setFormCariAdi] = useState('')
  const [formFaturaTarihi, setFormFaturaTarihi] = useState('')
  const [formGiderTanimiId, setFormGiderTanimiId] = useState('')
  const [formToplamTutar, setFormToplamTutar] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await giderFaturalariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { giderTanimlariApi.getAll().then(r => setTanimlar(r.data.filter(t => t.isActive))) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const resetForm = () => { setFormCariAdi(''); setFormFaturaTarihi(''); setFormGiderTanimiId(''); setFormToplamTutar(''); setFormAciklama('') }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: Fatura) => {
    setFormCariAdi(item.cariAdi); setFormFaturaTarihi(item.faturaTarihi.split('T')[0]); setFormGiderTanimiId(item.giderTanimiId ?? ''); setFormToplamTutar(String(item.toplamTutar)); setFormAciklama(item.aciklama ?? '')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formCariAdi.trim()) { showApiError('Counterparty name is required.'); return }
    if (!formToplamTutar || parseFloat(formToplamTutar) <= 0) { showApiError('Amount must be greater than zero.'); return }
    if (!formFaturaTarihi) { showApiError('Invoice date is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateFaturaDto = { tip: FaturaTipi.Gider, faturaTarihi: formFaturaTarihi, cariAdi: formCariAdi, giderTanimiId: formGiderTanimiId || undefined, toplamTutar: parseFloat(formToplamTutar), aciklama: formAciklama || undefined }
        await giderFaturalariApi.create(dto); showSuccess('Expense invoice created.')
      } else if (selected) {
        const dto: UpdateFaturaDto = { faturaTarihi: formFaturaTarihi, cariAdi: formCariAdi, giderTanimiId: formGiderTanimiId || undefined, toplamTutar: parseFloat(formToplamTutar), aciklama: formAciklama || undefined }
        await giderFaturalariApi.update(selected.id, dto); showSuccess('Expense invoice updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await giderFaturalariApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Expense Invoices</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Invoice</Button>
      </div>
      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Search by ref. no, counterparty..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><TrendingDown className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">No expense invoices found.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Ref No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Invoice Date</th>
                <th className="text-left px-3 py-2 font-medium">Counterparty</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Category</th>
                <th className="text-right px-3 py-2 font-medium">Total</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.faturaTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5">{item.cariAdi}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.giderTanimiAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.toplamTutar)}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Expense Invoice' : 'Edit Expense Invoice'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="cari">Counterparty (Supplier) <span className="text-destructive">*</span></Label>
                <Input id="cari" value={formCariAdi} onChange={e => setFormCariAdi(e.target.value)} placeholder="Name of supplier" maxLength={200} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tarih">Invoice Date <span className="text-destructive">*</span></Label>
                <Input id="tarih" type="date" value={formFaturaTarihi} onChange={e => setFormFaturaTarihi(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tanim">Expense Category</Label>
                <select id="tanim" value={formGiderTanimiId} onChange={e => setFormGiderTanimiId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— None —</option>
                  {tanimlar.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Total Amount (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" min={0} step="0.01" value={formToplamTutar} onChange={e => setFormToplamTutar(e.target.value)} placeholder="0.00" />
              </div>
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
