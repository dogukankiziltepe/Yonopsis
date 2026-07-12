'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Receipt } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { giderTanimlariApi, giderGruplariApi } from '@/lib/api/tanimlar'
import type { GiderTanimi, GiderGrubu, CreateGiderTanimiDto, UpdateGiderTanimiDto, DagitimSekli, BorclandirilacakKisiTuru } from '@/types/tanimlar'
import { DagitimSekliLabel } from '@/types/tanimlar'
import { cariTuruLabel } from '@/types/muhasebe'
import { showSuccess, showApiError } from '@/lib/toast'

export default function GiderTanimlariPage() {
  const [items, setItems] = useState<GiderTanimi[]>([])
  const [groups, setGroups] = useState<GiderGrubu[]>([])
  const [loading, setLoading] = useState(true)
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<GiderTanimi | null>(null)
  const [formKodu, setFormKodu] = useState('')
  const [formName, setFormName] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [formGiderGrubuId, setFormGiderGrubuId] = useState('')
  const [formDagitimSekli, setFormDagitimSekli] = useState('')
  const [formBosDairelereDagit, setFormBosDairelereDagit] = useState(false)
  const [formKdv, setFormKdv] = useState('')
  const [formBorclandirilacakKisi, setFormBorclandirilacakKisi] = useState('')
  const [formMuhasebeKodu, setFormMuhasebeKodu] = useState('')
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

  const resetForm = () => {
    setFormKodu(''); setFormName(''); setFormDescription(''); setFormGiderGrubuId('')
    setFormDagitimSekli(''); setFormBosDairelereDagit(false); setFormKdv('')
    setFormBorclandirilacakKisi(''); setFormMuhasebeKodu('')
  }

  const openCreate = () => {
    resetForm()
    setFormOrder(String(items.length + 1)); setFormIsActive(true)
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: GiderTanimi) => {
    setFormKodu(item.giderKodu); setFormName(item.name); setFormDescription(item.description ?? '')
    setFormGiderGrubuId(item.giderGrubuId ?? '')
    setFormDagitimSekli(item.dagitimSekli !== undefined ? String(item.dagitimSekli) : '')
    setFormBosDairelereDagit(item.bosDairelereDagit)
    setFormKdv(item.kdv !== undefined ? String(item.kdv) : '')
    setFormBorclandirilacakKisi(item.borclandirilacakKisi !== undefined ? String(item.borclandirilacakKisi) : '')
    setFormMuhasebeKodu(item.muhasebeKodu ?? '')
    setFormOrder(String(item.order)); setFormIsActive(item.isActive)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formKodu.trim()) { showApiError('Gider kodu zorunludur.'); return }
    if (!formName.trim()) { showApiError('Ad zorunludur.'); return }
    setSaving(true)
    const shared = {
      giderKodu: formKodu.trim(),
      name: formName.trim(),
      description: formDescription.trim() || undefined,
      giderGrubuId: formGiderGrubuId || undefined,
      dagitimSekli: formDagitimSekli ? (Number(formDagitimSekli) as DagitimSekli) : undefined,
      bosDairelereDagit: formBosDairelereDagit,
      kdv: formKdv ? Number(formKdv) : undefined,
      borclandirilacakKisi: formBorclandirilacakKisi ? (Number(formBorclandirilacakKisi) as BorclandirilacakKisiTuru) : undefined,
      muhasebeKodu: formMuhasebeKodu.trim() || undefined,
      order: parseInt(formOrder) || 0,
    }
    try {
      if (panelMode === 'create') {
        const dto: CreateGiderTanimiDto = shared
        await giderTanimlariApi.create(dto)
        showSuccess('Gider tanımı oluşturuldu.')
      } else if (selected) {
        const dto: UpdateGiderTanimiDto = { ...shared, isActive: formIsActive }
        await giderTanimlariApi.update(selected.id, dto)
        showSuccess('Gider tanımı güncellendi.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try {
      await giderTanimlariApi.delete(id)
      showSuccess('Gider tanımı silindi.')
      await load(); setDeleteConfirm(null)
    } catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Gider Tanımları</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Tanım</Button>
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <Receipt className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm">Henüz gider tanımı yok.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Gider Kodu</th>
                <th className="text-left px-4 py-2 font-medium">Gider Adı</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Grubu</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">Dağıtım Şekli</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">KDV</th>
                <th className="text-left px-4 py-2 font-medium hidden lg:table-cell">Muhasebe Kodu</th>
                <th className="text-left px-4 py-2 font-medium w-24">Durum</th>
                <th className="w-24 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((item) => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-4 py-3 font-mono text-xs">{item.giderKodu}</td>
                  <td className="px-4 py-3 font-medium">{item.name}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.giderGrubuName ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.dagitimSekli !== undefined ? DagitimSekliLabel[item.dagitimSekli] : '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.kdv ?? '—'}</td>
                  <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.muhasebeKodu ?? '—'}</td>
                  <td className="px-4 py-3">
                    <Badge variant={item.isActive ? 'default' : 'secondary'}>{item.isActive ? 'Aktif' : 'Pasif'}</Badge>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 justify-end">
                      <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => openEdit(item)}><Pencil className="h-3.5 w-3.5" /></Button>
                      {deleteConfirm === item.id ? (
                        <Button variant="destructive" size="sm" className="h-7 text-xs" onClick={() => handleDelete(item.id)}>Onayla</Button>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'Yeni Gider Tanımı' : 'Gider Tanımını Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="kodu">Gider Kodu <span className="text-destructive">*</span></Label>
                <Input id="kodu" value={formKodu} onChange={e => setFormKodu(e.target.value)} placeholder="örn. G001" maxLength={30} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="name">Gider Adı <span className="text-destructive">*</span></Label>
                <Input id="name" value={formName} onChange={e => setFormName(e.target.value)} placeholder="örn. Elektrik" maxLength={100} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="group">Grubu</Label>
                <select
                  id="group"
                  value={formGiderGrubuId}
                  onChange={e => setFormGiderGrubuId(e.target.value)}
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
                >
                  <option value="">— Seçilmedi —</option>
                  {groups.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="dagitim">Dağıtım Şekli</Label>
                <select
                  id="dagitim"
                  value={formDagitimSekli}
                  onChange={e => setFormDagitimSekli(e.target.value)}
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
                >
                  <option value="">— Seçilmedi —</option>
                  {Object.entries(DagitimSekliLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
                </select>
              </div>
              <div className="flex items-center gap-2">
                <input type="checkbox" id="bosDaire" checked={formBosDairelereDagit} onChange={e => setFormBosDairelereDagit(e.target.checked)} className="h-4 w-4" />
                <Label htmlFor="bosDaire">Boş Dairelere Dağıtım</Label>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="kdv">KDV (%)</Label>
                <Input id="kdv" type="number" min={0} max={100} value={formKdv} onChange={e => setFormKdv(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="borclu">Borçlandırılacak Kişi</Label>
                <select
                  id="borclu"
                  value={formBorclandirilacakKisi}
                  onChange={e => setFormBorclandirilacakKisi(e.target.value)}
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
                >
                  <option value="">— Seçilmedi —</option>
                  {Object.entries(cariTuruLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="muhasebeKodu">Muhasebe Kodu</Label>
                <Input id="muhasebeKodu" value={formMuhasebeKodu} onChange={e => setFormMuhasebeKodu(e.target.value)} placeholder="örn. 770.01" maxLength={50} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="desc">Açıklama</Label>
                <Input id="desc" value={formDescription} onChange={e => setFormDescription(e.target.value)} placeholder="Opsiyonel" maxLength={500} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="order">Sıra</Label>
                <Input id="order" type="number" min={0} value={formOrder} onChange={e => setFormOrder(e.target.value)} />
              </div>
              {panelMode === 'edit' && (
                <div className="flex items-center gap-2">
                  <input type="checkbox" id="isActive" checked={formIsActive} onChange={e => setFormIsActive(e.target.checked)} className="h-4 w-4" />
                  <Label htmlFor="isActive">Aktif</Label>
                </div>
              )}
            </div>
            <div className="border-t px-4 py-3 flex gap-2 justify-end">
              <Button variant="outline" onClick={closePanel}>İptal</Button>
              <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
