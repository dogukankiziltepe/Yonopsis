'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, CircleDollarSign } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { birimFiyatlarApi } from '@/lib/api/sayac'
import type { BirimFiyat, CreateBirimFiyatDto } from '@/types/sayac'
import { SayacTipi, SayacTipiLabel } from '@/types/sayac'
import { showSuccess, showApiError } from '@/lib/toast'

const BIRIM_DEFAULTS: Record<SayacTipi, string> = {
  [SayacTipi.Elektrik]: 'kWh',
  [SayacTipi.Su]: 'm³',
  [SayacTipi.Dogalgaz]: 'm³',
  [SayacTipi.Diger]: '',
}

export default function BirimFiyatPage() {
  const [items, setItems] = useState<BirimFiyat[]>([])
  const [loading, setLoading] = useState(true)
  const [tipFilter, setTipFilter] = useState<SayacTipi | ''>('')
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<BirimFiyat | null>(null)
  const [form, setForm] = useState<CreateBirimFiyatDto>({
    tip: SayacTipi.Elektrik, fiyat: 0, birim: 'kWh', baslangicTarihi: new Date().toISOString().slice(0, 10),
  })
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await birimFiyatlarApi.getAll(tipFilter !== '' ? tipFilter : undefined)
      setItems(res.data)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [tipFilter])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setForm({ tip: SayacTipi.Elektrik, fiyat: 0, birim: 'kWh', baslangicTarihi: new Date().toISOString().slice(0, 10) })
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: BirimFiyat) => {
    setForm({ tip: item.tip, fiyat: item.fiyat, birim: item.birim ?? '', baslangicTarihi: item.baslangicTarihi.slice(0, 10), bitisTarihi: item.bitisTarihi?.slice(0, 10), aciklama: item.aciklama })
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleTipChange = (tip: SayacTipi) => {
    setForm(f => ({ ...f, tip, birim: BIRIM_DEFAULTS[tip] }))
  }

  const handleSave = async () => {
    if (form.fiyat <= 0) { showApiError('Price must be greater than 0.'); return }
    if (!form.baslangicTarihi) { showApiError('Start date is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        await birimFiyatlarApi.create(form)
        showSuccess('Unit price created.')
      } else if (selected) {
        await birimFiyatlarApi.update(selected.id, form)
        showSuccess('Unit price updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await birimFiyatlarApi.delete(id)
      showSuccess('Unit price deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  const fmt = (n: number) => n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 4 })

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Unit Prices</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Price</Button>
      </div>

      <div className="flex gap-2">
        <select value={tipFilter} onChange={e => setTipFilter(e.target.value as SayacTipi | '')}
          className="h-8 text-sm border rounded-md px-2 bg-background">
          <option value="">All Types</option>
          {Object.values(SayacTipi).filter(v => typeof v === 'number').map(v => (
            <option key={v} value={v}>{SayacTipiLabel[v as SayacTipi]}</option>
          ))}
        </select>
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <CircleDollarSign className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No unit prices defined yet.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Type</th>
                <th className="text-left px-4 py-2 font-medium">Price</th>
                <th className="text-left px-4 py-2 font-medium w-20">Unit</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Valid From</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Valid To</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{SayacTipiLabel[item.tip]}</td>
                  <td className="px-4 py-3">{fmt(item.fiyat)} ₺</td>
                  <td className="px-4 py-3 text-muted-foreground">{item.birim ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{new Date(item.baslangicTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.bitisTarihi ? new Date(item.bitisTarihi).toLocaleDateString('tr-TR') : '—'}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Unit Price' : 'Edit Unit Price'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>Meter Type</Label>
                <select value={form.tip} onChange={e => handleTipChange(Number(e.target.value) as SayacTipi)}
                  className="w-full h-9 text-sm border rounded-md px-2 bg-background">
                  {Object.values(SayacTipi).filter(v => typeof v === 'number').map(v => (
                    <option key={v} value={v}>{SayacTipiLabel[v as SayacTipi]}</option>
                  ))}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <Label htmlFor="fiyat">Price (₺) <span className="text-destructive">*</span></Label>
                  <Input id="fiyat" type="number" step="0.0001" min="0" value={form.fiyat}
                    onChange={e => setForm(f => ({ ...f, fiyat: parseFloat(e.target.value) || 0 }))} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="birim">Unit</Label>
                  <Input id="birim" value={form.birim ?? ''} onChange={e => setForm(f => ({ ...f, birim: e.target.value }))} placeholder="kWh, m³" maxLength={20} />
                </div>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="baslangic">Valid From <span className="text-destructive">*</span></Label>
                <Input id="baslangic" type="date" value={form.baslangicTarihi} onChange={e => setForm(f => ({ ...f, baslangicTarihi: e.target.value }))} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="bitis">Valid To</Label>
                <Input id="bitis" type="date" value={form.bitisTarihi ?? ''} onChange={e => setForm(f => ({ ...f, bitisTarihi: e.target.value || undefined }))} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Description</Label>
                <Input id="aciklama" value={form.aciklama ?? ''} onChange={e => setForm(f => ({ ...f, aciklama: e.target.value }))} maxLength={500} />
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
