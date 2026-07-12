'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Landmark } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import { KasaBankaTipi } from '@/types/tanimlar'
import type { KasaBanka, CreateKasaBankaDto, UpdateKasaBankaDto } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

export default function KasaBankaPage() {
  const [items, setItems] = useState<KasaBanka[]>([])
  const [loading, setLoading] = useState(true)
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<KasaBanka | null>(null)
  const [formName, setFormName] = useState('')
  const [formTip, setFormTip] = useState<KasaBankaTipi>(KasaBankaTipi.Kasa)
  const [formBankaAdi, setFormBankaAdi] = useState('')
  const [formSubeAdi, setFormSubeAdi] = useState('')
  const [formHesapNo, setFormHesapNo] = useState('')
  const [formIban, setFormIban] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await kasaBankaApi.getAll()
      setItems(res.data)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const resetForm = () => {
    setFormName(''); setFormTip(KasaBankaTipi.Kasa); setFormBankaAdi(''); setFormSubeAdi(''); setFormHesapNo(''); setFormIban(''); setFormIsActive(true)
  }

  const openCreate = () => {
    resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: KasaBanka) => {
    setFormName(item.name); setFormTip(item.tip); setFormBankaAdi(item.bankaAdi ?? ''); setFormSubeAdi(item.subeAdi ?? ''); setFormHesapNo(item.hesapNo ?? ''); setFormIban(item.iban ?? ''); setFormIsActive(item.isActive)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formName.trim()) { showApiError('Name is required.'); return }
    setSaving(true)
    try {
      const bankFields = formTip === KasaBankaTipi.Banka ? {
        bankaAdi: formBankaAdi.trim() || undefined,
        subeAdi: formSubeAdi.trim() || undefined,
        hesapNo: formHesapNo.trim() || undefined,
        iban: formIban.trim() || undefined,
      } : {}
      if (panelMode === 'create') {
        const dto: CreateKasaBankaDto = { name: formName.trim(), tip: formTip, ...bankFields }
        await kasaBankaApi.create(dto)
        showSuccess('Cash/bank account created.')
      } else if (selected) {
        const dto: UpdateKasaBankaDto = { name: formName.trim(), tip: formTip, isActive: formIsActive, ...bankFields }
        await kasaBankaApi.update(selected.id, dto)
        showSuccess('Cash/bank account updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await kasaBankaApi.delete(id)
      showSuccess('Account deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  const tipLabel = (tip: KasaBankaTipi) => tip === KasaBankaTipi.Kasa ? 'Cash' : 'Bank'

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Cash / Bank Accounts</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Account</Button>
      </div>

      <div className="border rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Landmark className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No accounts defined yet.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Name</th>
                <th className="text-left px-4 py-2 font-medium w-24">Type</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Bank</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">IBAN</th>
                <th className="text-left px-4 py-2 font-medium w-24">Status</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{item.name}</td>
                  <td className="px-4 py-3">
                    <Badge variant="outline">{tipLabel(item.tip)}</Badge>
                  </td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">
                    {item.tip === KasaBankaTipi.Banka ? (item.bankaAdi ?? '—') : '—'}
                  </td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell font-mono text-xs">
                    {item.tip === KasaBankaTipi.Banka ? (item.iban ?? '—') : '—'}
                  </td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Account' : 'Edit Account'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="name">Name <span className="text-destructive">*</span></Label>
                <Input id="name" value={formName} onChange={e => setFormName(e.target.value)} placeholder="e.g. Main Cash Register" maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label>Type <span className="text-destructive">*</span></Label>
                <div className="flex gap-3">
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input type="radio" name="tip" checked={formTip === KasaBankaTipi.Kasa} onChange={() => setFormTip(KasaBankaTipi.Kasa)} />
                    <span className="text-sm">Cash</span>
                  </label>
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input type="radio" name="tip" checked={formTip === KasaBankaTipi.Banka} onChange={() => setFormTip(KasaBankaTipi.Banka)} />
                    <span className="text-sm">Bank</span>
                  </label>
                </div>
              </div>

              {formTip === KasaBankaTipi.Banka && (
                <>
                  <div className="space-y-1.5">
                    <Label htmlFor="bankaAdi">Bank Name</Label>
                    <Input id="bankaAdi" value={formBankaAdi} onChange={e => setFormBankaAdi(e.target.value)} placeholder="e.g. Ziraat Bankası" maxLength={100} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="subeAdi">Branch</Label>
                    <Input id="subeAdi" value={formSubeAdi} onChange={e => setFormSubeAdi(e.target.value)} placeholder="e.g. Kadıköy Branch" maxLength={100} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="hesapNo">Account Number</Label>
                    <Input id="hesapNo" value={formHesapNo} onChange={e => setFormHesapNo(e.target.value)} placeholder="Account number" maxLength={50} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="iban">IBAN</Label>
                    <Input id="iban" value={formIban} onChange={e => setFormIban(e.target.value)} placeholder="TR00 0000 0000 0000 0000 0000 00" maxLength={34} className="font-mono" />
                  </div>
                </>
              )}

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
