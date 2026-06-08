'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { profileApi, ProfileDto, UpdateProfileDto } from '@/lib/api/profile'
import { siteApi } from '@/lib/api/client'
import { showSuccess, showError } from '@/lib/toast'

const GENDER_OPTIONS = [
  { value: '', label: 'Belirtilmemiş' },
  { value: '0', label: 'Erkek' },
  { value: '1', label: 'Kadın' },
  { value: '2', label: 'Diğer' },
]

export default function ProfilePage() {
  const [profile, setProfile] = useState<ProfileDto | null>(null)
  const [loading, setLoading] = useState(true)

  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [nationalId, setNationalId] = useState('')
  const [gender, setGender] = useState('')
  const [saving, setSaving] = useState(false)

  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [changingPassword, setChangingPassword] = useState(false)

  useEffect(() => {
    profileApi.get()
      .then((res) => {
        const p = res.data
        setProfile(p)
        setFirstName(p.firstName)
        setLastName(p.lastName)
        setPhoneNumber(p.phoneNumber ?? '')
        setNationalId(p.nationalId ?? '')
        setGender(p.gender != null ? p.gender.toString() : '')
      })
      .catch(() => showError('Profil yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  const handleSave = async () => {
    if (!firstName.trim() || !lastName.trim()) { showError('Ad ve soyad zorunludur.'); return }
    setSaving(true)
    try {
      const dto: UpdateProfileDto = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        phoneNumber: phoneNumber || undefined,
        nationalId: nationalId || undefined,
        gender: gender !== '' ? parseInt(gender) : undefined,
      }
      await profileApi.update(dto)
      showSuccess('Profil güncellendi.')
      setProfile(p => p ? { ...p, firstName, lastName } : p)
    } catch { showError('Güncelleme başarısız.') }
    finally { setSaving(false) }
  }

  const handleChangePassword = async () => {
    if (!currentPassword || !newPassword) { showError('Tüm alanları doldurun.'); return }
    if (newPassword !== confirmPassword) { showError('Yeni şifreler eşleşmiyor.'); return }
    if (newPassword.length < 6) { showError('Yeni şifre en az 6 karakter olmalıdır.'); return }
    setChangingPassword(true)
    try {
      await siteApi.post('/api/auth/change-password', { currentPassword, newPassword })
      showSuccess('Şifre başarıyla değiştirildi.')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch { showError('Şifre değiştirme başarısız.') }
    finally { setChangingPassword(false) }
  }

  if (loading) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  return (
    <div className="max-w-xl space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Profil & Hesap</h1>
        <p className="text-muted-foreground">Kişisel bilgilerinizi ve şifrenizi yönetin.</p>
      </div>

      {/* Profil bilgileri */}
      <div className="border rounded-lg p-5 space-y-4">
        <h2 className="text-sm font-semibold">Kişisel Bilgiler</h2>

        <div className="space-y-1">
          <Label className="text-xs font-medium">E-posta</Label>
          <Input value={profile?.email ?? ''} disabled className="bg-muted/50" />
          <p className="text-xs text-muted-foreground">E-posta adresi değiştirilemez.</p>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1">
            <Label className="text-xs font-medium">Ad <span className="text-destructive">*</span></Label>
            <Input value={firstName} onChange={(e) => setFirstName(e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Soyad <span className="text-destructive">*</span></Label>
            <Input value={lastName} onChange={(e) => setLastName(e.target.value)} />
          </div>
        </div>

        <div className="space-y-1">
          <Label className="text-xs font-medium">Telefon</Label>
          <Input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} placeholder="+90 5xx xxx xx xx" />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1">
            <Label className="text-xs font-medium">TC Kimlik No</Label>
            <Input value={nationalId} onChange={(e) => setNationalId(e.target.value)} placeholder="11 haneli TC kimlik" maxLength={11} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Cinsiyet</Label>
            <select
              value={gender}
              onChange={(e) => setGender(e.target.value)}
              className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
            >
              {GENDER_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>
        </div>

        <Button size="sm" onClick={handleSave} disabled={saving}>
          {saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>

      {/* Şifre değiştirme */}
      <div className="border rounded-lg p-5 space-y-4">
        <h2 className="text-sm font-semibold">Şifre Değiştir</h2>

        <div className="space-y-1">
          <Label className="text-xs font-medium">Mevcut Şifre</Label>
          <Input
            type="password"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            placeholder="••••••••"
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Yeni Şifre</Label>
          <Input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            placeholder="••••••••"
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Yeni Şifre (Tekrar)</Label>
          <Input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            placeholder="••••••••"
          />
        </div>

        <Button size="sm" onClick={handleChangePassword} disabled={changingPassword}>
          {changingPassword ? 'Değiştiriliyor...' : 'Şifreyi Değiştir'}
        </Button>
      </div>

      {/* Hesap bilgileri */}
      {profile && (
        <div className="border rounded-lg p-5 space-y-2">
          <h2 className="text-sm font-semibold">Hesap Bilgileri</h2>
          <div className="text-xs text-muted-foreground space-y-1">
            <p>Hesap ID: <span className="font-mono">{profile.id}</span></p>
            <p>Oluşturulma: {new Date(profile.createdAt).toLocaleDateString('tr-TR')}</p>
            <p>Durum: <span className={profile.isActive ? 'text-green-600' : 'text-red-500'}>
              {profile.isActive ? 'Aktif' : 'Pasif'}
            </span></p>
          </div>
        </div>
      )}
    </div>
  )
}
