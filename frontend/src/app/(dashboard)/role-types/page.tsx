'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, ShieldCheck, Lock } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { roleTypesApi } from '@/lib/api/roleTypes'
import { showSuccess, showError } from '@/lib/toast'
import {
  RoleTypeDto,
  RoleTypePermissionsDto,
  PermissionLevelLabel,
} from '@/types/roleType'

const PERMISSION_OPTIONS: Array<{ value: 0 | 1 | 2 | 3; label: string }> = [
  { value: 0, label: 'Yetkisiz' },
  { value: 1, label: 'Sadece Görüntüle' },
  { value: 2, label: 'Görüntüle & Oluştur' },
  { value: 3, label: 'Tam Yetki' },
]

export default function RoleTypesPage() {
  const [roles, setRoles] = useState<RoleTypeDto[]>([])
  const [loading, setLoading] = useState(true)
  const [selected, setSelected] = useState<RoleTypeDto | null>(null)
  const [permissions, setPermissions] = useState<RoleTypePermissionsDto | null>(null)
  const [permLoading, setPermLoading] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [panelOpen, setPanelOpen] = useState(false)

  const [formName, setFormName] = useState('')
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    roleTypesApi.getAll()
      .then((res) => setRoles(res.data ?? []))
      .catch(() => showError('Rol tipleri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const openRole = (role: RoleTypeDto) => {
    setSelected(role)
    setPermissions(null)
    setPanelMode('view')
    setPanelOpen(true)
    setPermLoading(true)
    roleTypesApi.getPermissions(role.id)
      .then((res) => setPermissions(res.data))
      .catch(() => {})
      .finally(() => setPermLoading(false))
  }

  const openCreate = () => {
    setSelected(null)
    setPermissions(null)
    setFormName('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formName.trim()) { showError('Rol adı zorunludur.'); return }
    setSaving(true)
    try {
      await roleTypesApi.create({ name: formName.trim() })
      showSuccess('Rol tipi oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu rol tipini silmek istediğinize emin misiniz?')) return
    try {
      await roleTypesApi.delete(id)
      showSuccess('Rol tipi silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  const handlePermissionChange = async (pageId: string, level: 0 | 1 | 2 | 3) => {
    if (!selected || !permissions) return
    const prev = permissions.pages.map(p =>
      p.pageId === pageId ? { ...p, permissionLevel: level } : p
    )
    setPermissions({ ...permissions, pages: prev })
    try {
      await roleTypesApi.setPermission(selected.id, { pageId, permissionLevel: level })
    } catch {
      // revert
      setPermissions(permissions)
    }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Rol Tipleri</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Rol
        </Button>
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Rol Adı</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tür</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={2} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : roles.length === 0 ? (
              <tr><td colSpan={2}>
                <div className="flex flex-col items-center py-12">
                  <ShieldCheck className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz rol tipi eklenmemiş.</p>
                </div>
              </td></tr>
            ) : (
              roles.map((role) => (
                <tr
                  key={role.id}
                  onClick={() => openRole(role)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === role.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium">{role.name}</td>
                  <td className="px-3 py-2.5">
                    {role.isDefault
                      ? <Badge variant="default">Varsayılan</Badge>
                      : <Badge variant="outline">Özel</Badge>
                    }
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[520px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'create' ? 'Yeni Rol Tipi' : selected?.name}
            </h2>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'create' ? (
              <>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Rol Adı <span className="text-destructive">*</span></Label>
                  <Input value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="Örn: Muhasebe" />
                </div>
                <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Oluştur'}
                </Button>
              </>
            ) : permLoading ? (
              <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
            ) : permissions ? (
              <>
                {permissions.isDefault && (
                  <div className="flex items-center gap-2 text-xs text-muted-foreground bg-muted rounded-md px-3 py-2">
                    <Lock className="h-3.5 w-3.5 shrink-0" />
                    Varsayılan rol tüm sayfalara tam erişime sahiptir ve değiştirilemez.
                  </div>
                )}

                <div className="space-y-1">
                  <h3 className="text-sm font-semibold">Sayfa İzinleri</h3>
                  <div className="border rounded-lg overflow-hidden overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b bg-muted/50">
                          <th className="text-left px-3 py-2 font-medium text-muted-foreground">Sayfa</th>
                          <th className="text-left px-3 py-2 font-medium text-muted-foreground">İzin Seviyesi</th>
                        </tr>
                      </thead>
                      <tbody>
                        {permissions.pages.map((page) => (
                          <tr key={page.pageId} className="border-b last:border-0">
                            <td className="px-3 py-2">
                              <p className="font-medium">{page.pageLabel}</p>
                              <p className="text-xs text-muted-foreground">{page.pageRoute}</p>
                            </td>
                            <td className="px-3 py-2">
                              {permissions.isDefault ? (
                                <Badge variant="default" className="text-xs">
                                  {PermissionLevelLabel[page.permissionLevel]}
                                </Badge>
                              ) : (
                                <select
                                  value={page.permissionLevel}
                                  onChange={(e) => handlePermissionChange(page.pageId, Number(e.target.value) as 0 | 1 | 2 | 3)}
                                  className="border rounded px-2 py-1 text-xs bg-background w-full"
                                >
                                  {PERMISSION_OPTIONS.map((opt) => (
                                    <option key={opt.value} value={opt.value}>{opt.label}</option>
                                  ))}
                                </select>
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>

                {!permissions.isDefault && (
                  <div className="pt-2 border-t">
                    <Button
                      size="sm"
                      variant="destructive"
                      onClick={() => selected && handleDelete(selected.id)}
                    >
                      Rolü Sil
                    </Button>
                  </div>
                )}
              </>
            ) : null}
          </div>
        </div>
      )}
    </div>
  )
}
