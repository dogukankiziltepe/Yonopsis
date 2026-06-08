'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Layers } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { unitTypesApi } from '@/lib/api/unitTypes'
import { showSuccess, showError } from '@/lib/toast'
import { UnitTypeDto } from '@/types/unitType'

export default function UnitTypesPage() {
  const [items, setItems] = useState<UnitTypeDto[]>([])
  const [loading, setLoading] = useState(true)
  const [selected, setSelected] = useState<UnitTypeDto | null>(null)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [panelOpen, setPanelOpen] = useState(false)

  const [formName, setFormName] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    unitTypesApi.getAll()
      .then((res) => setItems(res.data ?? []))
      .catch(() => showError('Daire tipleri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const openView = (item: UnitTypeDto) => {
    setSelected(item)
    setFormName(item.name)
    setFormIsActive(item.isActive)
    setPanelMode('view')
    setPanelOpen(true)
  }

  const openCreate = () => {
    setSelected(null)
    setFormName('')
    setFormIsActive(true)
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formName.trim()) { showError('Daire tipi adı zorunludur.'); return }
    setSaving(true)
    try {
      await unitTypesApi.create({ name: formName.trim() })
      showSuccess('Daire tipi oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleUpdate = async () => {
    if (!selected || !formName.trim()) return
    setSaving(true)
    try {
      await unitTypesApi.update(selected.id, { name: formName.trim(), isActive: formIsActive })
      showSuccess('Daire tipi güncellendi.')
      load()
      setPanelOpen(false)
    } catch {} finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu daire tipini silmek istediğinize emin misiniz?')) return
    try {
      await unitTypesApi.delete(id)
      showSuccess('Daire tipi silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Daire Tipleri</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Tip
        </Button>
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tip Adı</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={2} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={2}>
                <div className="flex flex-col items-center py-12">
                  <Layers className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz daire tipi eklenmemiş.</p>
                </div>
              </td></tr>
            ) : (
              items.map((item) => (
                <tr
                  key={item.id}
                  onClick={() => openView(item)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === item.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium">{item.name}</td>
                  <td className="px-3 py-2.5">
                    <Badge variant={item.isActive ? 'default' : 'secondary'}>
                      {item.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[400px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'create' ? 'Yeni Daire Tipi' : selected?.name}
            </h2>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            <div className="space-y-1">
              <Label className="text-xs font-medium">Tip Adı <span className="text-destructive">*</span></Label>
              <Input value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="Örn: Daire, Dükkan, Depo" />
            </div>

            {panelMode === 'view' && (
              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={formIsActive}
                  onChange={(e) => setFormIsActive(e.target.checked)}
                  className="rounded"
                />
                <Label htmlFor="isActive" className="text-xs font-medium cursor-pointer">Aktif</Label>
              </div>
            )}

            {panelMode === 'create' ? (
              <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                {saving ? 'Oluşturuluyor...' : 'Oluştur'}
              </Button>
            ) : (
              <>
                <Button size="sm" className="w-full" onClick={handleUpdate} disabled={saving}>
                  {saving ? 'Kaydediliyor...' : 'Kaydet'}
                </Button>
                <div className="pt-2 border-t">
                  <Button size="sm" variant="destructive" onClick={() => selected && handleDelete(selected.id)}>
                    Sil
                  </Button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
