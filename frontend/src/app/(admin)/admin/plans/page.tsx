'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Package } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { adminPlansApi, PlanDto, ModuleDto } from '@/lib/api/adminPlans'
import { showSuccess, showError } from '@/lib/toast'

export default function AdminPlansPage() {
  const [plans, setPlans] = useState<PlanDto[]>([])
  const [modules, setModules] = useState<ModuleDto[]>([])
  const [loading, setLoading] = useState(true)

  const [selected, setSelected] = useState<PlanDto | null>(null)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [panelOpen, setPanelOpen] = useState(false)

  const [formName, setFormName] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [formPrice, setFormPrice] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    Promise.all([adminPlansApi.getAll(), adminPlansApi.getModules()])
      .then(([plansRes, modulesRes]) => {
        setPlans(plansRes.data ?? [])
        setModules(modulesRes.data ?? [])
      })
      .catch(() => showError('Veriler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const openView = (plan: PlanDto) => {
    setSelected(plan)
    setFormName(plan.name)
    setFormDescription(plan.description ?? '')
    setFormPrice(plan.price.toString())
    setFormIsActive(plan.isActive)
    setPanelMode('view')
    setPanelOpen(true)
  }

  const openCreate = () => {
    setSelected(null)
    setFormName('')
    setFormDescription('')
    setFormPrice('')
    setFormIsActive(true)
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formName.trim()) { showError('Plan adı zorunludur.'); return }
    const price = parseFloat(formPrice)
    if (isNaN(price) || price < 0) { showError('Geçerli bir fiyat giriniz.'); return }
    setSaving(true)
    try {
      await adminPlansApi.create({
        name: formName.trim(),
        description: formDescription || undefined,
        price,
        isActive: formIsActive,
      })
      showSuccess('Plan oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleUpdate = async () => {
    if (!selected || !formName.trim()) return
    const price = parseFloat(formPrice)
    if (isNaN(price) || price < 0) return
    setSaving(true)
    try {
      await adminPlansApi.update(selected.id, {
        name: formName.trim(),
        description: formDescription || undefined,
        price,
        isActive: formIsActive,
      })
      showSuccess('Plan güncellendi.')
      load()
      const updated = { ...selected, name: formName, description: formDescription, price, isActive: formIsActive }
      setSelected(updated)
    } catch {} finally { setSaving(false) }
  }

  const handleModuleToggle = async (moduleId: string, isIncluded: boolean) => {
    if (!selected) return
    try {
      await adminPlansApi.setModule(selected.id, { moduleId, isIncluded })
      const updatedModules = isIncluded
        ? [...selected.moduleNames, modules.find(m => m.id === moduleId)?.name ?? '']
        : selected.moduleNames.filter(n => n !== modules.find(m => m.id === moduleId)?.name)
      const updated = { ...selected, moduleNames: updatedModules.filter(Boolean) }
      setSelected(updated)
      setPlans(plans.map(p => p.id === selected.id ? updated : p))
    } catch { showError('Modül güncellenemedi.') }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Planlar</h1>
          <p className="text-muted-foreground">Abonelik planlarını yönetin</p>
        </div>
        <Button onClick={openCreate}>
          <Plus className="h-4 w-4 mr-2" />
          Yeni Plan
        </Button>
      </div>

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : (
        <div className="border rounded-lg overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Plan Adı</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Fiyat</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Modüller</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Durum</th>
              </tr>
            </thead>
            <tbody>
              {plans.length === 0 ? (
                <tr><td colSpan={4}>
                  <div className="flex flex-col items-center py-12">
                    <Package className="h-10 w-10 text-muted-foreground/50 mb-3" />
                    <p className="text-muted-foreground">Henüz plan eklenmemiş.</p>
                  </div>
                </td></tr>
              ) : plans.map((plan) => (
                <tr
                  key={plan.id}
                  onClick={() => openView(plan)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === plan.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-4 py-3 font-medium">{plan.name}</td>
                  <td className="px-4 py-3">
                    {new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(plan.price)}
                  </td>
                  <td className="px-4 py-3 text-muted-foreground">
                    {plan.moduleNames.length ? plan.moduleNames.join(', ') : '-'}
                  </td>
                  <td className="px-4 py-3">
                    <Badge variant={plan.isActive ? 'default' : 'secondary'}>
                      {plan.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[520px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'create' ? 'Yeni Plan' : selected?.name}
            </h2>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            <div className="space-y-1">
              <Label className="text-xs font-medium">Plan Adı <span className="text-destructive">*</span></Label>
              <Input value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="Örn: Standart" />
            </div>
            <div className="space-y-1">
              <Label className="text-xs font-medium">Açıklama</Label>
              <Input value={formDescription} onChange={(e) => setFormDescription(e.target.value)} placeholder="Kısa açıklama" />
            </div>
            <div className="space-y-1">
              <Label className="text-xs font-medium">Aylık Fiyat (₺) <span className="text-destructive">*</span></Label>
              <Input type="number" value={formPrice} onChange={(e) => setFormPrice(e.target.value)} placeholder="0" min="0" />
            </div>
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="planActive"
                checked={formIsActive}
                onChange={(e) => setFormIsActive(e.target.checked)}
                className="rounded"
              />
              <Label htmlFor="planActive" className="text-xs font-medium cursor-pointer">Aktif</Label>
            </div>

            {panelMode === 'create' ? (
              <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                {saving ? 'Oluşturuluyor...' : 'Oluştur'}
              </Button>
            ) : (
              <>
                <Button size="sm" className="w-full" onClick={handleUpdate} disabled={saving}>
                  {saving ? 'Kaydediliyor...' : 'Kaydet'}
                </Button>

                {modules.length > 0 && (
                  <div className="space-y-2 pt-2 border-t">
                    <p className="text-xs font-medium text-muted-foreground">MODÜLLER</p>
                    <div className="border rounded-lg overflow-hidden">
                      <table className="w-full text-sm">
                        <tbody>
                          {modules.map((mod) => {
                            const included = selected?.moduleNames.includes(mod.name) ?? false
                            return (
                              <tr key={mod.id} className="border-b last:border-0">
                                <td className="px-3 py-2">
                                  <p className="font-medium">{mod.displayName}</p>
                                  {mod.description && <p className="text-xs text-muted-foreground">{mod.description}</p>}
                                </td>
                                <td className="px-3 py-2 text-right">
                                  <input
                                    type="checkbox"
                                    checked={included}
                                    onChange={(e) => handleModuleToggle(mod.id, e.target.checked)}
                                    className="rounded"
                                  />
                                </td>
                              </tr>
                            )
                          })}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
