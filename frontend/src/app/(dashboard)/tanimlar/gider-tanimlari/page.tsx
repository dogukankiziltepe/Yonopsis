'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Receipt } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { giderTanimlariApi, giderGruplariApi } from '@/lib/api/tanimlar'
import type { GiderTanimi, GiderGrubu, CreateGiderTanimiDto, UpdateGiderTanimiDto } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

export default function GiderTanimlariPage() {
  const [items, setItems] = useState<GiderTanimi[]>([])
  const [groups, setGroups] = useState<GiderGrubu[]>([])
  const [loading, setLoading] = useState(true)
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<GiderTanimi | null>(null)
  const [formName, setFormName] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [formGiderGrubuId, setFormGiderGrubuId] = useState('')
  const [formOrder, setFormOrder] = useState('0')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const [itemsRes, groupsRes] = await Promise.all([giderTanimlariApi.getAll(), giderGruplariApi.getAll()])
      setItems(itemsRes.data)
      setGroups(groupsRes.data.filter(g => g.isActive))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setFormName(''); setFormDescription(''); setFormGiderGrubuId(''); setFormOrder(String(items.length + 1)); setFormIsActive(true)
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: GiderTanimi) => {
    setFormName(item.name); setFormDescription(item.description ?? ''); setFormGiderGrubuId(item.giderGrubuId ?? ''); setFormOrder(String(item.order)); setFormIsActive(item.isActive)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formName.trim()) { showApiError('Name is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateGiderTanimiDto = { name: formName.trim(), description: formDescription.trim() || undefined, giderGrubuId: formGiderGrubuId || undefined, order: parseInt(formOrder) || 0 }
        await giderTanimlariApi.create(dto)
        showSuccess('Expense definition created.')
      } else if (selected) {
        const dto: UpdateGiderTanimiDto = { name: formName.trim(), description: formDescription.trim() || undefined, giderGrubuId: formGiderGrubuId || undefined, isActive: formIsActive, order: parseInt(formOrder) || 0 }
        await giderTanimlariApi.update(selected.id, dto)
        showSuccess('Expense definition updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await giderTanimlariApi.delete(id)
      showSuccess('Expense definition deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Expense Definitions</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Definition</Button>
      </div>

      <div className="border rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Receipt className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No expense definitions yet.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Name</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Group</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">Description</th>
                <th className="text-left px-4 py-2 font-medium w-20">Order</th>
                <th className="text-left px-4 py-2 font-medium w-24">Status</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{item.name}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.giderGrubuName ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.description ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground">{item.order}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Expense Definition' : 'Edit Expense Definition'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="name">Name <span className="text-destructive">*</span></Label>
                <Input id="name" value={formName} onChange={e => setFormName(e.target.value)} placeholder="e.g. Electricity" maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="group">Expense Group</Label>
                <select
                  id="group"
                  value={formGiderGrubuId}
                  onChange={e => setFormGiderGrubuId(e.target.value)}
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
                >
                  <option value="">— None —</option>
                  {groups.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="desc">Description</Label>
                <Input id="desc" value={formDescription} onChange={e => setFormDescription(e.target.value)} placeholder="Optional" maxLength={500} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="order">Order</Label>
                <Input id="order" type="number" min={0} value={formOrder} onChange={e => setFormOrder(e.target.value)} />
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
