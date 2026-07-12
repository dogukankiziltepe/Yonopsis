'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Tag } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { talepTipleriApi } from '@/lib/api/teknik'
import type { TalepTipi, CreateTalepTipiDto, UpdateTalepTipiDto } from '@/types/teknik'
import { showSuccess, showApiError } from '@/lib/toast'

export default function TalepTipleriPage() {
  const [items, setItems] = useState<TalepTipi[]>([])
  const [loading, setLoading] = useState(true)
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<TalepTipi | null>(null)
  const [formAd, setFormAd] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await talepTipleriApi.getAll()
      setItems(res.data)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setFormAd(''); setFormAciklama(''); setFormIsActive(true)
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: TalepTipi) => {
    setFormAd(item.ad); setFormAciklama(item.aciklama ?? ''); setFormIsActive(item.isActive)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formAd.trim()) { showApiError('Request type name is required.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateTalepTipiDto = { ad: formAd.trim(), aciklama: formAciklama.trim() || undefined }
        await talepTipleriApi.create(dto)
        showSuccess('Request type created.')
      } else if (selected) {
        const dto: UpdateTalepTipiDto = { ad: formAd.trim(), aciklama: formAciklama.trim() || undefined, isActive: formIsActive }
        await talepTipleriApi.update(selected.id, dto)
        showSuccess('Request type updated.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await talepTipleriApi.delete(id)
      showSuccess('Request type deleted.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Request Types</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />New Type</Button>
      </div>

      <div className="border rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Tag className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">No request types yet.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Name</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Description</th>
                <th className="text-left px-4 py-2 font-medium w-24">Status</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{item.ad}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.aciklama ?? '—'}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'New Request Type' : 'Edit Request Type'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="ad">Name <span className="text-destructive">*</span></Label>
                <Input id="ad" value={formAd} onChange={e => setFormAd(e.target.value)} placeholder="e.g. Electrical Issue" maxLength={200} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Description</Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Optional" maxLength={500} />
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
