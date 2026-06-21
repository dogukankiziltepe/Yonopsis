'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, AlertCircle, List, Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { aidatKalemleriApi } from '@/lib/api/aidatKalemleri'
import { AidatKalemi, CreateAidatKalemiDto, UpdateAidatKalemiDto } from '@/types/aidatKalemi'
import { showSuccess, showError } from '@/lib/toast'

export default function AidatKalemleriPage() {
  const [items, setItems] = useState<AidatKalemi[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<AidatKalemi | null>(null)

  const [formName, setFormName] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [formOrder, setFormOrder] = useState('0')
  const [formIsActive, setFormIsActive] = useState(true)
  const [formErrors, setFormErrors] = useState<{ name?: string; order?: string }>({})
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const res = await aidatKalemleriApi.getAll()
      setItems(res.data)
    } catch {
      setError('Veriler yüklenirken bir hata oluştu.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setFormName('')
    setFormDescription('')
    setFormOrder(String(items.length + 1))
    setFormIsActive(true)
    setFormErrors({})
    setSelected(null)
    setPanelMode('create')
    setPanelOpen(true)
  }

  const openEdit = (item: AidatKalemi) => {
    setFormName(item.name)
    setFormDescription(item.description ?? '')
    setFormOrder(String(item.order))
    setFormIsActive(item.isActive)
    setFormErrors({})
    setSelected(item)
    setPanelMode('edit')
    setPanelOpen(true)
  }

  const closePanel = () => {
    setPanelOpen(false)
    setSelected(null)
    setDeleteConfirm(null)
  }

  const validate = () => {
    const errors: { name?: string; order?: string } = {}
    if (!formName.trim()) errors.name = 'Kalem adı zorunludur.'
    if (formName.length > 100) errors.name = 'En fazla 100 karakter.'
    const orderNum = parseInt(formOrder)
    if (isNaN(orderNum) || orderNum < 0) errors.order = 'Geçerli bir sıra numarası girin.'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) return
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateAidatKalemiDto = {
          name: formName.trim(),
          description: formDescription.trim() || undefined,
          order: parseInt(formOrder),
        }
        await aidatKalemleriApi.create(dto)
        showSuccess('Aidat kalemi oluşturuldu.')
      } else if (selected) {
        const dto: UpdateAidatKalemiDto = {
          name: formName.trim(),
          description: formDescription.trim() || undefined,
          isActive: formIsActive,
          order: parseInt(formOrder),
        }
        await aidatKalemleriApi.update(selected.id, dto)
        showSuccess('Aidat kalemi güncellendi.')
      }
      closePanel()
      load()
    } catch {
      showError()
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (deleteConfirm !== id) {
      setDeleteConfirm(id)
      return
    }
    try {
      await aidatKalemleriApi.delete(id)
      showSuccess('Aidat kalemi silindi.')
      if (selected?.id === id) closePanel()
      load()
    } catch {
      showError()
    } finally {
      setDeleteConfirm(null)
    }
  }

  return (
    <div className="flex h-full">
      {/* Ana içerik */}
      <div className={`flex-1 min-w-0 transition-all duration-300 ${panelOpen ? 'pr-[460px]' : ''}`}>
        <div className="p-6">
          {/* Başlık */}
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold tracking-tight">Aidat Kalemleri</h1>
              <p className="text-sm text-muted-foreground mt-1">
                Sitenizde tahsil edilen aidat türlerini tanımlayın.
              </p>
            </div>
            <Button onClick={openCreate} size="sm" className="gap-2">
              <Plus className="h-4 w-4" />
              Yeni Kalem
            </Button>
          </div>

          {error && (
            <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm">
              <AlertCircle className="h-4 w-4 shrink-0" />
              {error}
            </div>
          )}

          {loading ? (
            <div className="space-y-2">
              {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="h-14 rounded-lg bg-muted animate-pulse" />
              ))}
            </div>
          ) : items.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-20 text-center">
              <List className="h-12 w-12 text-muted-foreground mb-4" />
              <p className="text-muted-foreground">Henüz aidat kalemi tanımlanmamış.</p>
              <Button variant="outline" size="sm" className="mt-4 gap-2" onClick={openCreate}>
                <Plus className="h-4 w-4" />
                İlk kalemi ekle
              </Button>
            </div>
          ) : (
            <div className="rounded-lg border overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground w-16">Sıra</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Kalem Adı</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden md:table-cell">Açıklama</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Durum</th>
                    <th className="px-4 py-3 text-right font-medium text-muted-foreground w-24">İşlem</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {items.map((item) => (
                    <tr
                      key={item.id}
                      className={`hover:bg-muted/30 cursor-pointer transition-colors ${selected?.id === item.id ? 'bg-muted/50' : ''}`}
                      onClick={() => openEdit(item)}
                    >
                      <td className="px-4 py-3 text-muted-foreground">{item.order}</td>
                      <td className="px-4 py-3 font-medium">{item.name}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">
                        {item.description || '—'}
                      </td>
                      <td className="px-4 py-3 text-center">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${
                          item.isActive
                            ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
                            : 'bg-muted text-muted-foreground'
                        }`}>
                          {item.isActive ? 'Aktif' : 'Pasif'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right" onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-1">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-7 w-7"
                            onClick={() => openEdit(item)}
                          >
                            <Pencil className="h-3.5 w-3.5" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className={`h-7 w-7 ${deleteConfirm === item.id ? 'text-destructive bg-destructive/10' : ''}`}
                            onClick={() => handleDelete(item.id)}
                          >
                            <Trash2 className="h-3.5 w-3.5" />
                          </Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {deleteConfirm && (
            <div className="mt-3 p-3 rounded-lg bg-destructive/10 text-destructive text-sm flex items-center justify-between">
              <span>Silmek istediğinizden emin misiniz? Tekrar tıklayın.</span>
              <Button variant="ghost" size="sm" onClick={() => setDeleteConfirm(null)}>
                İptal
              </Button>
            </div>
          )}
        </div>
      </div>

      {/* Sağ panel */}
      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          {/* Panel başlığı */}
          <div className="flex items-center justify-between px-6 py-4 border-b shrink-0">
            <h2 className="font-semibold text-base">
              {panelMode === 'create' ? 'Yeni Aidat Kalemi' : 'Kalemi Düzenle'}
            </h2>
            <Button variant="ghost" size="icon" onClick={closePanel}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          {/* Panel gövdesi */}
          <div className="flex-1 overflow-y-auto px-6 py-5 space-y-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">
                Kalem Adı <span className="text-destructive">*</span>
              </Label>
              <Input
                value={formName}
                onChange={(e) => setFormName(e.target.value)}
                placeholder="örn. Bakım Bedeli, Su, Asansör"
                className={formErrors.name ? 'border-destructive' : ''}
                maxLength={100}
              />
              {formErrors.name && (
                <p className="mt-1 text-xs text-destructive flex items-center gap-1">
                  <AlertCircle className="h-3 w-3" />{formErrors.name}
                </p>
              )}
            </div>

            <div>
              <Label className="text-xs font-medium mb-1.5 block">Açıklama</Label>
              <Input
                value={formDescription}
                onChange={(e) => setFormDescription(e.target.value)}
                placeholder="İsteğe bağlı açıklama"
                maxLength={500}
              />
            </div>

            <div>
              <Label className="text-xs font-medium mb-1.5 block">Sıra Numarası</Label>
              <Input
                type="number"
                min={0}
                value={formOrder}
                onChange={(e) => setFormOrder(e.target.value)}
                className={formErrors.order ? 'border-destructive' : ''}
              />
              {formErrors.order && (
                <p className="mt-1 text-xs text-destructive flex items-center gap-1">
                  <AlertCircle className="h-3 w-3" />{formErrors.order}
                </p>
              )}
            </div>

            {panelMode === 'edit' && (
              <div className="flex items-center gap-3 pt-1">
                <button
                  type="button"
                  onClick={() => setFormIsActive(!formIsActive)}
                  className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${
                    formIsActive ? 'bg-primary' : 'bg-muted-foreground/30'
                  }`}
                >
                  <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${
                    formIsActive ? 'translate-x-4' : 'translate-x-0'
                  }`} />
                </button>
                <Label className="text-sm cursor-pointer" onClick={() => setFormIsActive(!formIsActive)}>
                  {formIsActive ? 'Aktif' : 'Pasif'}
                </Label>
              </div>
            )}
          </div>

          {/* Panel alt */}
          <div className="px-6 py-4 border-t shrink-0 flex gap-3">
            <Button onClick={handleSave} disabled={saving} className="flex-1">
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </Button>
            {panelMode === 'edit' && selected && (
              <Button
                variant="destructive"
                size="icon"
                onClick={() => handleDelete(selected.id)}
                disabled={saving}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
