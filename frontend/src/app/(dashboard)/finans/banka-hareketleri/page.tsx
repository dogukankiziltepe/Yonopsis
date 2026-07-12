'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Landmark } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { bankaHareketleriApi } from '@/lib/api/finans'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import type { BankaHareketi, CreateBankaHareketiDto, UpdateBankaHareketiDto } from '@/types/finans'
import { BankaHareketiDurum, BankaHareketiDurumLabel } from '@/types/finans'
import type { KasaBanka } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function BankaHareketleriPage() {
  const [items, setItems] = useState<BankaHareketi[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [filterKasaBankaId, setFilterKasaBankaId] = useState('')
  const [kasaBankalar, setKasaBankalar] = useState<KasaBanka[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<BankaHareketi | null>(null)
  const [formKasaBankaId, setFormKasaBankaId] = useState('')
  const [formTarih, setFormTarih] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [formReferansNo, setFormReferansNo] = useState('')
  const [formTutar, setFormTutar] = useState('')
  const [formDurum, setFormDurum] = useState<BankaHareketiDurum>(BankaHareketiDurum.Bekleyen)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await bankaHareketleriApi.getAll(page, PAGE_SIZE, filterKasaBankaId || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, filterKasaBankaId])

  useEffect(() => { load() }, [load])
  useEffect(() => { kasaBankaApi.getAll().then(r => setKasaBankalar(r.data.filter(k => k.isActive))) }, [])

  const today = () => new Date().toISOString().split('T')[0]
  const resetForm = () => { setFormKasaBankaId(''); setFormTarih(today()); setFormAciklama(''); setFormReferansNo(''); setFormTutar(''); setFormDurum(BankaHareketiDurum.Bekleyen) }
  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: BankaHareketi) => {
    setFormKasaBankaId(item.kasaBankaId)
    setFormTarih(item.tarih.split('T')[0])
    setFormAciklama(item.aciklama)
    setFormReferansNo(item.referansNo ?? '')
    setFormTutar(String(item.tutar))
    setFormDurum(item.durum)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formKasaBankaId) { showApiError('Account is required.'); return }
    if (!formAciklama.trim()) { showApiError('Description is required.'); return }
    if (!formTutar || formTutar === '0') { showApiError('Amount cannot be zero.'); return }
    if (!formTarih) { showApiError('Date is required.'); return }
    setSaving(true)
    try {
      const base: CreateBankaHareketiDto = { kasaBankaId: formKasaBankaId, tarih: formTarih, aciklama: formAciklama, referansNo: formReferansNo || undefined, tutar: parseFloat(formTutar) }
      if (panelMode === 'create') {
        await bankaHareketleriApi.create(base); showSuccess('Transaction recorded.')
      } else if (selected) {
        const dto: UpdateBankaHareketiDto = { ...base, durum: formDurum }
        await bankaHareketleriApi.update(selected.id, dto); showSuccess('Transaction updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await bankaHareketleriApi.delete(id); showSuccess('Deleted.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Bank Transactions</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Transaction</Button>
      </div>
      <div className="flex items-center gap-2">
        <select value={filterKasaBankaId} onChange={e => { setFilterKasaBankaId(e.target.value); setPage(1) }} className="border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring max-w-xs">
          <option value="">All Accounts</option>
          {kasaBankalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
        </select>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} records</span>}
      </div>
      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><Landmark className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">No bank transactions found.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Date</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Account</th>
                <th className="text-left px-3 py-2 font-medium">Description</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Ref No</th>
                <th className="text-right px-3 py-2 font-medium">Amount</th>
                <th className="text-center px-3 py-2 font-medium hidden md:table-cell">Status</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 text-xs text-muted-foreground whitespace-nowrap">{new Date(item.tarih).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 hidden md:table-cell text-sm">{item.kasaBankaAdi}</td>
                  <td className="px-3 py-2.5">{item.aciklama}</td>
                  <td className="px-3 py-2.5 font-mono text-xs text-muted-foreground hidden lg:table-cell">{item.referansNo ?? '—'}</td>
                  <td className={`px-3 py-2.5 text-right font-semibold ${item.tutar >= 0 ? 'text-emerald-600' : 'text-red-600'}`}>{fmt(item.tutar)}</td>
                  <td className="px-3 py-2.5 text-center hidden md:table-cell">
                    <Badge variant={item.durum === BankaHareketiDurum.Eslestis ? 'default' : 'secondary'} className="text-xs">
                      {BankaHareketiDurumLabel[item.durum]}
                    </Badge>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Transaction' : 'Edit Transaction'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="hesap">Account <span className="text-destructive">*</span></Label>
                <select id="hesap" value={formKasaBankaId} onChange={e => setFormKasaBankaId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" disabled={panelMode === 'edit'}>
                  <option value="">— Select account —</option>
                  {kasaBankalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tarih">Date <span className="text-destructive">*</span></Label>
                <Input id="tarih" type="date" value={formTarih} onChange={e => setFormTarih(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Description <span className="text-destructive">*</span></Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Transaction description" maxLength={300} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="ref">Reference No</Label>
                <Input id="ref" value={formReferansNo} onChange={e => setFormReferansNo(e.target.value)} placeholder="Optional reference number" maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Amount (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" step="0.01" value={formTutar} onChange={e => setFormTutar(e.target.value)} placeholder="Positive = incoming, negative = outgoing" />
                <p className="text-xs text-muted-foreground">Use positive values for incoming, negative for outgoing.</p>
              </div>
              {panelMode === 'edit' && (
                <div className="space-y-1.5">
                  <Label>Status</Label>
                  <div className="flex gap-3">
                    {[BankaHareketiDurum.Bekleyen, BankaHareketiDurum.Eslestis].map(d => (
                      <label key={d} className="flex items-center gap-2 cursor-pointer">
                        <input type="radio" name="durum" value={d} checked={formDurum === d} onChange={() => setFormDurum(d)} className="accent-primary" />
                        <span className="text-sm">{BankaHareketiDurumLabel[d]}</span>
                      </label>
                    ))}
                  </div>
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
