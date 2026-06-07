'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, ChevronLeft, ChevronRight, X, Users, UserCheck } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { personsApi } from '@/lib/api/persons'
import { showSuccess, showError } from '@/lib/toast'
import {
  PersonDto,
  PersonDetailDto,
  InvitePersonDto,
  UpdatePersonDto,
  UserType,
  UserTypeLabel,
  UserSiteStatus,
  UserSiteStatusLabel,
} from '@/types/person'
import { UnitStatusLabel } from '@/types/unit'

const USER_TYPES: { value: UserType; label: string }[] = [
  { value: 2, label: 'Mal Sahibi' },
  { value: 3, label: 'Kiracı' },
  { value: 4, label: 'Yönetim' },
]

export default function PersonsPage() {
  const [persons, setPersons] = useState<PersonDto[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [selected, setSelected] = useState<PersonDto | null>(null)
  const [detail, setDetail] = useState<PersonDetailDto | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formEmail, setFormEmail] = useState('')
  const [formFirstName, setFormFirstName] = useState('')
  const [formLastName, setFormLastName] = useState('')
  const [formPhone, setFormPhone] = useState('')
  const [formUserType, setFormUserType] = useState<UserType>(2)
  const [saving, setSaving] = useState(false)

  const filtered = persons.filter((p) => {
    const q = search.toLowerCase()
    return (
      p.firstName.toLowerCase().includes(q) ||
      p.lastName.toLowerCase().includes(q) ||
      p.email.toLowerCase().includes(q) ||
      (p.phoneNumber ?? '').toLowerCase().includes(q)
    )
  })

  const load = useCallback(() => {
    setLoading(true)
    personsApi.getAll()
      .then((res) => setPersons(res.data ?? []))
      .catch(() => showError('Kişiler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    load()
  }, [load])

  const openView = (person: PersonDto, index: number) => {
    setPanelMode('view')
    setSelected(person)
    setPanelIndex(index)
    setDetail(null)
    setPanelOpen(true)
    setDetailLoading(true)
    personsApi.getById(person.userSiteId)
      .then((res) => setDetail(res.data))
      .catch(() => {})
      .finally(() => setDetailLoading(false))
  }

  const openCreate = () => {
    setPanelMode('create')
    setSelected(null)
    setDetail(null)
    setFormEmail('')
    setFormFirstName('')
    setFormLastName('')
    setFormPhone('')
    setFormUserType(2)
    setPanelOpen(true)
  }

  const navigatePanel = (dir: 'prev' | 'next') => {
    const newIndex = dir === 'prev' ? panelIndex - 1 : panelIndex + 1
    if (newIndex >= 0 && newIndex < filtered.length) {
      openView(filtered[newIndex], newIndex)
    }
  }

  const handleInvite = async () => {
    if (!formEmail.trim() || !formFirstName.trim() || !formLastName.trim()) {
      showError('Ad, soyad ve e-posta zorunludur.')
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
      await personsApi.invite(dto)
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
      setPanelOpen(false)
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
        <div className="flex items-center gap-2">
          <Input
            placeholder="Ara..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="h-8 w-48"
          />
          <Button size="sm" onClick={openCreate}>
            <Plus className="h-4 w-4 mr-1" />
            Kişi Ekle
          </Button>
        </div>
      </div>

      <div className="border rounded-lg overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Ad Soyad</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">E-posta</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Telefon</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Kullanıcı Tipi</th>
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
              ) : filtered.length === 0 ? (
                <tr>
                  <td colSpan={5}>
                    <div className="flex flex-col items-center justify-center py-12">
                      <Users className="h-10 w-10 text-muted-foreground/50 mb-3" />
                      <p className="text-muted-foreground">
                        {search ? 'Arama sonucu bulunamadı.' : 'Henüz kişi eklenmemiş.'}
                      </p>
                    </div>
                  </td>
                </tr>
              ) : (
                filtered.map((person, index) => (
                  <tr
                    key={person.userSiteId}
                    onClick={() => openView(person, index)}
                    className={`border-b last:border-0 cursor-pointer transition-colors ${
                      selected?.userSiteId === person.userSiteId && panelOpen
                        ? 'bg-accent'
                        : 'hover:bg-muted/30'
                    }`}
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
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Side Panel */}
      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'create' ? 'Kişi Ekle' : 'Kişi Detayı'}
            </h2>
            <div className="flex items-center gap-0.5">
              {panelMode === 'view' && (
                <>
                  <Button
                    variant="ghost" size="icon" className="h-7 w-7"
                    disabled={panelIndex <= 0}
                    onClick={() => navigatePanel('prev')}
                    title="Önceki"
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <Button
                    variant="ghost" size="icon" className="h-7 w-7"
                    disabled={panelIndex >= filtered.length - 1}
                    onClick={() => navigatePanel('next')}
                    title="Sonraki"
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </>
              )}
              <Button
                variant="ghost" size="icon" className="h-7 w-7"
                onClick={() => setPanelOpen(false)}
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'create' ? (
              <>
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
                    onChange={(e) => setFormUserType(Number(e.target.value) as UserType)}
                    className="w-full border rounded-md px-3 py-2 text-sm bg-background"
                  >
                    {USER_TYPES.map((t) => (
                      <option key={t.value} value={t.value}>{t.label}</option>
                    ))}
                  </select>
                </div>
                <Button size="sm" className="w-full" onClick={handleInvite} disabled={saving}>
                  {saving ? 'Ekleniyor...' : 'Ekle'}
                </Button>
              </>
            ) : detailLoading ? (
              <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
            ) : detail ? (
              <>
                <div className="flex items-center gap-3 pb-3 border-b">
                  <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary text-primary-foreground font-medium">
                    {detail.firstName[0]?.toUpperCase()}
                  </div>
                  <div>
                    <p className="font-medium">{detail.firstName} {detail.lastName}</p>
                    <p className="text-xs text-muted-foreground">{detail.email}</p>
                  </div>
                  <div className="ml-auto flex gap-1.5">
                    {detail.userType != null && (
                      <Badge variant="outline">{UserTypeLabel[detail.userType]}</Badge>
                    )}
                    <Badge variant={statusBadgeVariant(detail.status)}>
                      {UserSiteStatusLabel[detail.status]}
                    </Badge>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3 text-sm">
                  {detail.phoneNumber && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Telefon</p>
                      <p>{detail.phoneNumber}</p>
                    </div>
                  )}
                  {detail.roleName && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Rol</p>
                      <p>{detail.roleName}</p>
                    </div>
                  )}
                  {detail.nationalId && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">TC Kimlik</p>
                      <p>{detail.nationalId}</p>
                    </div>
                  )}
                </div>

                {detail.units.length > 0 && (
                  <div>
                    <div className="flex items-center gap-1.5 mb-2">
                      <UserCheck className="h-4 w-4 text-muted-foreground" />
                      <h3 className="text-sm font-semibold">Bağlı Daireler</h3>
                    </div>
                    <div className="border rounded-lg overflow-hidden">
                      <table className="w-full text-xs">
                        <thead>
                          <tr className="border-b bg-muted/50">
                            <th className="text-left px-2 py-2 font-medium text-muted-foreground">Daire</th>
                            <th className="text-left px-2 py-2 font-medium text-muted-foreground">Blok</th>
                            <th className="text-left px-2 py-2 font-medium text-muted-foreground">Durum</th>
                          </tr>
                        </thead>
                        <tbody>
                          {detail.units.map((u) => (
                            <tr key={u.id} className="border-b last:border-0">
                              <td className="px-2 py-2 font-medium">{u.doorNumber}</td>
                              <td className="px-2 py-2 text-muted-foreground">{u.buildingName ?? '-'}</td>
                              <td className="px-2 py-2">
                                <Badge variant="outline" className="text-xs">
                                  {UnitStatusLabel[u.status]}
                                </Badge>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}

                <div className="pt-2 border-t">
                  <Button
                    size="sm"
                    variant="destructive"
                    onClick={() => handleRemove(detail.userSiteId)}
                  >
                    Siteden Kaldır
                  </Button>
                </div>
              </>
            ) : null}
          </div>
        </div>
      )}
    </div>
  )
}
