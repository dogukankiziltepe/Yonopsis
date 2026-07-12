'use client'

import { useEffect, useState, useCallback } from 'react'
import { useRouter } from 'next/navigation'
import { Plus, X, Users } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { PaginationControls } from '@/components/ui/pagination-controls'
import { personsApi } from '@/lib/api/persons'
import { unitsApi } from '@/lib/api/units'
import { showSuccess, showError } from '@/lib/toast'
import {
  PersonDto,
  InvitePersonDto,
  UserType,
  UserTypeLabel,
  UserSiteStatus,
  UserSiteStatusLabel,
} from '@/types/person'
import { UnitSummary } from '@/types/unit'

const PAGE_SIZE = 20

const USER_TYPES: { value: UserType; label: string }[] = [
  { value: 2, label: 'Mal Sahibi' },
  { value: 3, label: 'Kiracı' },
  { value: 4, label: 'Yönetim' },
]

export default function PersonsPage() {
  const router = useRouter()
  const [persons, setPersons] = useState<PersonDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchDebounced, setSearchDebounced] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)

  const [formEmail, setFormEmail] = useState('')
  const [formFirstName, setFormFirstName] = useState('')
  const [formLastName, setFormLastName] = useState('')
  const [formPhone, setFormPhone] = useState('')
  const [formUserType, setFormUserType] = useState<UserType>(2)
  const [formUnitId, setFormUnitId] = useState('')
  const [units, setUnits] = useState<UnitSummary[]>([])
  const [unitsLoading, setUnitsLoading] = useState(false)
  const [saving, setSaving] = useState(false)

  const needsUnit = (type: UserType) => type === 2 || type === 3

  useEffect(() => {
    const t = setTimeout(() => setSearchDebounced(search), 300)
    return () => clearTimeout(t)
  }, [search])

  const load = useCallback(() => {
    setLoading(true)
    personsApi.getAll(page, PAGE_SIZE, searchDebounced || undefined)
      .then((res) => {
        const d = res.data
        setPersons(d.items ?? [])
        setTotalCount(d.totalCount ?? 0)
        setTotalPages(d.totalPages ?? 0)
      })
      .catch(() => showError('Kişiler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, searchDebounced])

  useEffect(() => {
    load()
  }, [load])

  const handleSearchChange = (val: string) => { setSearch(val); setPage(1) }

  const loadUnits = () => {
    setUnitsLoading(true)
    unitsApi.getAll()
      .then((res) => {
        const data = res.data
        setUnits(Array.isArray(data) ? data : (data as any)?.value ?? [])
      })
      .catch(() => {})
      .finally(() => setUnitsLoading(false))
  }

  const openCreate = () => {
    setFormEmail('')
    setFormFirstName('')
    setFormLastName('')
    setFormPhone('')
    setFormUserType(2)
    setFormUnitId('')
    setPanelOpen(true)
    loadUnits()
  }

  const handleInvite = async () => {
    if (!formEmail.trim() || !formFirstName.trim() || !formLastName.trim()) {
      showError('Ad, soyad ve e-posta zorunludur.')
      return
    }
    if (needsUnit(formUserType) && !formUnitId) {
      showError('Mal sahibi veya kiracı için daire seçimi zorunludur.')
      return
    }
    setSaving(true)
    const dto: InvitePersonDto = {
      email: formEmail.trim(),
      firstName: formFirstName.trim(),
      lastName: formLastName.trim(),
      phoneNumber: formPhone.trim() || undefined,
      userType: formUserType,
    }
    try {
      const res = await personsApi.invite(dto)
      const createdUserId = (res.data as any)?.id ?? (res.data as any)?.value?.id

      if (formUnitId && createdUserId) {
        if (formUserType === 2) {
          await unitsApi.assignOwner(formUnitId, createdUserId)
        } else if (formUserType === 3) {
          await unitsApi.assignTenant(formUnitId, createdUserId)
        }
      }

      showSuccess('Kişi başarıyla eklendi.')
      setPanelOpen(false)
      load()
    } catch {
      // showApiError already called by interceptor
    } finally {
      setSaving(false)
    }
  }

  const handleRemove = async (id: string) => {
    if (!confirm('Bu kişiyi siteden kaldırmak istediğinize emin misiniz?')) return
    try {
      await personsApi.remove(id)
      showSuccess('Kişi siteden kaldırıldı.')
      load()
    } catch {}
  }

  const statusBadgeVariant = (status: UserSiteStatus) => {
    if (status === 1) return 'default' as const
    if (status === 2) return 'destructive' as const
    return 'secondary' as const
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Kişiler</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Kişi Ekle
        </Button>
      </div>

      <div className="mb-3">
        <PaginationControls
          page={page} pageSize={PAGE_SIZE} totalCount={totalCount} totalPages={totalPages}
          search={search} onPageChange={setPage} onSearchChange={handleSearchChange}
          searchPlaceholder="Ad, soyad veya e-posta ara..."
        />
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Ad Soyad</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">E-posta</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Telefon</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Kullanıcı Tipi</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
                <th className="text-right px-3 py-2.5 font-medium text-muted-foreground">İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-12 text-muted-foreground">
                    Yükleniyor...
                  </td>
                </tr>
              ) : persons.length === 0 ? (
                <tr>
                  <td colSpan={6}>
                    <div className="flex flex-col items-center justify-center py-12">
                      <Users className="h-10 w-10 text-muted-foreground/50 mb-3" />
                      <p className="text-muted-foreground">
                        {search ? 'Arama sonucu bulunamadı.' : 'Henüz kişi eklenmemiş.'}
                      </p>
                    </div>
                  </td>
                </tr>
              ) : (
                persons.map((person) => (
                  <tr
                    key={person.userSiteId}
                    onClick={() => router.push(`/persons/${person.userSiteId}`)}
                    className="border-b last:border-0 cursor-pointer transition-colors hover:bg-muted/30"
                  >
                    <td className="px-3 py-2.5 font-medium">
                      {person.firstName} {person.lastName}
                    </td>
                    <td className="px-3 py-2.5 text-muted-foreground">{person.email}</td>
                    <td className="px-3 py-2.5 text-muted-foreground">{person.phoneNumber ?? '-'}</td>
                    <td className="px-3 py-2.5">
                      {person.userType != null ? (
                        <Badge variant="outline">{UserTypeLabel[person.userType]}</Badge>
                      ) : (
                        <span className="text-muted-foreground">-</span>
                      )}
                    </td>
                    <td className="px-3 py-2.5">
                      <Badge variant={statusBadgeVariant(person.status)}>
                        {UserSiteStatusLabel[person.status]}
                      </Badge>
                    </td>
                    <td className="px-3 py-2.5 text-right">
                      <Button
                        size="sm"
                        variant="ghost"
                        className="text-destructive hover:text-destructive"
                        onClick={(e) => {
                          e.stopPropagation()
                          handleRemove(person.userSiteId)
                        }}
                      >
                        Kaldır
                      </Button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Kişi Ekle Panel */}
      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">Kişi Ekle</h2>
            <Button
              variant="ghost" size="icon" className="h-7 w-7"
              onClick={() => setPanelOpen(false)}
            >
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            <div className="space-y-1">
              <Label className="text-xs font-medium">E-posta <span className="text-destructive">*</span></Label>
              <Input value={formEmail} onChange={(e) => setFormEmail(e.target.value)} placeholder="ornek@mail.com" />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs font-medium">Ad <span className="text-destructive">*</span></Label>
                <Input value={formFirstName} onChange={(e) => setFormFirstName(e.target.value)} placeholder="Ad" />
              </div>
              <div className="space-y-1">
                <Label className="text-xs font-medium">Soyad <span className="text-destructive">*</span></Label>
                <Input value={formLastName} onChange={(e) => setFormLastName(e.target.value)} placeholder="Soyad" />
              </div>
            </div>
            <div className="space-y-1">
              <Label className="text-xs font-medium">Telefon</Label>
              <Input value={formPhone} onChange={(e) => setFormPhone(e.target.value)} placeholder="+90 555 000 00 00" />
            </div>
            <div className="space-y-1">
              <Label className="text-xs font-medium">Kullanıcı Tipi <span className="text-destructive">*</span></Label>
              <select
                value={formUserType}
                onChange={(e) => {
                  setFormUserType(Number(e.target.value) as UserType)
                  setFormUnitId('')
                }}
                className="w-full border rounded-md px-3 py-2 text-sm bg-background"
              >
                {USER_TYPES.map((t) => (
                  <option key={t.value} value={t.value}>{t.label}</option>
                ))}
              </select>
            </div>

            {needsUnit(formUserType) && (
              <div className="space-y-1">
                <Label className="text-xs font-medium">
                  Daire <span className="text-destructive">*</span>
                </Label>
                {unitsLoading ? (
                  <p className="text-xs text-muted-foreground py-2">Daireler yükleniyor...</p>
                ) : (
                  <select
                    value={formUnitId}
                    onChange={(e) => setFormUnitId(e.target.value)}
                    className="w-full border rounded-md px-3 py-2 text-sm bg-background"
                  >
                    <option value="">Daire seçin</option>
                    {units.map((u) => (
                      <option key={u.id} value={u.id}>
                        {u.buildingName ? `${u.buildingName} - ` : ''}{u.doorNumber}
                        {u.floor ? ` (Kat ${u.floor})` : ''}
                      </option>
                    ))}
                  </select>
                )}
              </div>
            )}

            <Button size="sm" className="w-full" onClick={handleInvite} disabled={saving}>
              {saving ? 'Ekleniyor...' : 'Ekle'}
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
