'use client'

import { useEffect, useState, useCallback } from 'react'
import {
  Plus,
  ChevronLeft,
  ChevronRight,
  X,
  Home,
  AlertCircle,
} from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { unitsApi } from '@/lib/api/units'
import { buildingsApi } from '@/lib/api/buildings'
import {
  UnitSummary,
  UnitStatus,
  UnitStatusLabel,
  CreateUnitDto,
  UpdateUnitDto,
} from '@/types/unit'
import { Building } from '@/types/building'

type PanelMode = 'create' | 'edit'

const emptyForm = (): CreateUnitDto => ({
  buildingId: '',
  doorNumber: '',
  status: 0,
  hasDask: false,
})

export default function UnitsPage() {
  const [units, setUnits] = useState<UnitSummary[]>([])
  const [buildings, setBuildings] = useState<Building[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Panel state
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<PanelMode>('create')
  const [selectedUnit, setSelectedUnit] = useState<UnitSummary | null>(null)
  const [panelIndex, setPanelIndex] = useState(0)

  // Form state
  const [form, setForm] = useState<CreateUnitDto>(emptyForm())
  const [formErrors, setFormErrors] = useState<{ buildingId?: string; doorNumber?: string }>({})
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    unitsApi
      .getAll()
      .then((res) => {
        const data = res.data
        setUnits(Array.isArray(data) ? data : (data as any)?.value ?? [])
      })
      .catch(() => setError('Daireler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    load()
    buildingsApi.getAll(1, 100).then((res) => {
      const data = res.data
      const items = (data as any)?.items ?? (data as any)?.value?.items ?? []
      setBuildings(items)
    }).catch(() => {})
  }, [load])

  const resetForm = () => {
    setForm(emptyForm())
    setFormErrors({})
  }

  const openCreate = () => {
    setPanelMode('create')
    setSelectedUnit(null)
    resetForm()
    setPanelOpen(true)
  }

  const openEdit = (unit: UnitSummary, index: number) => {
    setPanelMode('edit')
    setSelectedUnit(unit)
    setPanelIndex(index)
    setForm({
      buildingId: unit.buildingId,
      doorNumber: unit.doorNumber,
      code: unit.code,
      floor: unit.floor,
      status: unit.status,
      monthlyFee: unit.monthlyFee,
      hasDask: unit.hasDask,
    })
    setFormErrors({})
    setPanelOpen(true)
  }

  const navigatePanel = (dir: 'prev' | 'next') => {
    const newIndex = dir === 'prev' ? panelIndex - 1 : panelIndex + 1
    if (newIndex >= 0 && newIndex < units.length) {
      openEdit(units[newIndex], newIndex)
    }
  }

  const validateForm = () => {
    const errors: { buildingId?: string; doorNumber?: string } = {}
    if (!form.buildingId) errors.buildingId = 'Blok seçiniz'
    if (!form.doorNumber.trim()) errors.doorNumber = 'Kapı No zorunludur'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validateForm()) return
    setSaving(true)
    try {
      if (panelMode === 'create') {
        await unitsApi.create(form)
      } else if (selectedUnit) {
        await unitsApi.update(selectedUnit.id, form as UpdateUnitDto)
      }
      setPanelOpen(false)
      load()
    } catch {
      setError('Kaydetme işlemi başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu daireyi silmek istediğinize emin misiniz?')) return
    try {
      await unitsApi.delete(id)
      if (selectedUnit?.id === id) setPanelOpen(false)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  const setField = <K extends keyof CreateUnitDto>(key: K, value: CreateUnitDto[K]) => {
    setForm((f) => ({ ...f, [key]: value }))
    if (key in formErrors) {
      setFormErrors((prev) => ({ ...prev, [key]: undefined }))
    }
  }

  return (
    <div className="flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Daireler</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Ekle
        </Button>
      </div>

      {error && (
        <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3 mb-3">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      {/* Table */}
      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Kapı No</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Blok</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Kat</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tip</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={5} className="text-center py-12 text-muted-foreground">
                  Yükleniyor...
                </td>
              </tr>
            ) : units.length === 0 ? (
              <tr>
                <td colSpan={5}>
                  <div className="flex flex-col items-center justify-center py-12 text-center">
                    <Home className="h-10 w-10 text-muted-foreground/50 mb-3" />
                    <p className="text-muted-foreground">Henüz daire eklenmemiş</p>
                  </div>
                </td>
              </tr>
            ) : (
              units.map((u, index) => (
                <tr
                  key={u.id}
                  onClick={() => openEdit(u, index)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selectedUnit?.id === u.id && panelOpen
                      ? 'bg-accent'
                      : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium">{u.doorNumber}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">{u.buildingName ?? '-'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">{u.floor ?? '-'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">{u.unitTypeName ?? '-'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">{UnitStatusLabel[u.status]}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Side Panel */}
      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[420px] bg-background border-l shadow-2xl flex flex-col z-50">
          {/* Panel header */}
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'create' ? 'Yeni Daire' : 'Daire Düzenle'}
            </h2>
            <div className="flex items-center gap-0.5">
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                disabled={panelMode === 'create' || panelIndex <= 0}
                onClick={() => navigatePanel('prev')}
                title="Önceki kayıt"
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                disabled={panelMode === 'create' || panelIndex >= units.length - 1}
                onClick={() => navigatePanel('next')}
                title="Sonraki kayıt"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                onClick={() => setPanelOpen(false)}
                title="Kapat"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </div>

          {/* Form */}
          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {/* Blok */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">
                Blok <span className="text-destructive">*</span>
              </Label>
              <div className="relative">
                <select
                  value={form.buildingId}
                  onChange={(e) => setField('buildingId', e.target.value)}
                  className={`flex h-9 w-full rounded-md border bg-transparent px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-1 focus:ring-ring ${
                    formErrors.buildingId ? 'border-destructive pr-8' : 'border-input'
                  }`}
                >
                  <option value="">Blok seçin</option>
                  {buildings.map((b) => (
                    <option key={b.id} value={b.id}>{b.name}</option>
                  ))}
                </select>
                {formErrors.buildingId && (
                  <AlertCircle className="h-4 w-4 text-destructive absolute right-2 top-1/2 -translate-y-1/2" />
                )}
              </div>
              {formErrors.buildingId && (
                <p className="text-xs text-destructive">{formErrors.buildingId}</p>
              )}
            </div>

            {/* Kapı No */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">
                Kapı No <span className="text-destructive">*</span>
              </Label>
              <div className="relative">
                <Input
                  value={form.doorNumber}
                  onChange={(e) => setField('doorNumber', e.target.value)}
                  className={formErrors.doorNumber ? 'border-destructive pr-8' : ''}
                  placeholder="örn. 1, 2A"
                />
                {formErrors.doorNumber && (
                  <AlertCircle className="h-4 w-4 text-destructive absolute right-2 top-1/2 -translate-y-1/2" />
                )}
              </div>
              {formErrors.doorNumber && (
                <p className="text-xs text-destructive">{formErrors.doorNumber}</p>
              )}
            </div>

            {/* Kat + Durum */}
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs font-medium">Kat</Label>
                <Input
                  value={form.floor ?? ''}
                  onChange={(e) => setField('floor', e.target.value || undefined)}
                  placeholder="örn. 3"
                />
              </div>
              <div className="space-y-1">
                <Label className="text-xs font-medium">Durum</Label>
                <select
                  value={String(form.status)}
                  onChange={(e) => setField('status', Number(e.target.value) as UnitStatus)}
                  className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-1 focus:ring-ring"
                >
                  {(Object.entries(UnitStatusLabel) as [string, string][]).map(([k, label]) => (
                    <option key={k} value={k}>{label}</option>
                  ))}
                </select>
              </div>
            </div>

            {/* Aylık Aidat + Kod */}
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs font-medium">Aylık Aidat (₺)</Label>
                <Input
                  type="number"
                  value={form.monthlyFee ?? ''}
                  onChange={(e) =>
                    setField('monthlyFee', e.target.value ? Number(e.target.value) : undefined)
                  }
                  placeholder="0"
                  min="0"
                />
              </div>
              <div className="space-y-1">
                <Label className="text-xs font-medium">Kod</Label>
                <Input
                  value={form.code ?? ''}
                  onChange={(e) => setField('code', e.target.value || undefined)}
                  placeholder="opsiyonel"
                />
              </div>
            </div>

            {/* Actions */}
            <div className="flex gap-2 pt-1">
              <Button size="sm" onClick={handleSave} disabled={saving} className="flex-1">
                {saving ? 'Kaydediliyor...' : 'Kaydet'}
              </Button>
              {panelMode === 'edit' && selectedUnit && (
                <Button
                  size="sm"
                  variant="destructive"
                  onClick={() => handleDelete(selectedUnit.id)}
                >
                  Sil
                </Button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
