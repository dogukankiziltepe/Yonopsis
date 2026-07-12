'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { personsApi } from '@/lib/api/persons'
import { showSuccess, showError } from '@/lib/toast'
import {
  PersonIdentityInfoDto,
  Nationality,
  NationalityLabel,
  MaritalStatus,
  MaritalStatusLabel,
} from '@/types/personDetail'

interface Props {
  userSiteId: string
  data: PersonIdentityInfoDto
  onSaved: () => void
}

export function IdentityInfoTab({ userSiteId, data, onSaved }: Props) {
  const [form, setForm] = useState<PersonIdentityInfoDto>(data)
  const [saving, setSaving] = useState(false)

  useEffect(() => setForm(data), [data])

  const set = <K extends keyof PersonIdentityInfoDto>(key: K, value: PersonIdentityInfoDto[K]) =>
    setForm((prev) => ({ ...prev, [key]: value }))

  const handleSave = async () => {
    setSaving(true)
    try {
      await personsApi.updateIdentityInfo(userSiteId, {
        ...form,
        identitySeriNo: form.identitySeriNo?.trim() || undefined,
        identitySiraNo: form.identitySiraNo?.trim() || undefined,
        passportNo: form.passportNo?.trim() || undefined,
        fatherName: form.fatherName?.trim() || undefined,
        motherName: form.motherName?.trim() || undefined,
        birthPlace: form.birthPlace?.trim() || undefined,
        registeredCity: form.registeredCity?.trim() || undefined,
        registeredDistrict: form.registeredDistrict?.trim() || undefined,
        registeredNeighborhood: form.registeredNeighborhood?.trim() || undefined,
        familySiraNo: form.familySiraNo?.trim() || undefined,
        kayitSiraNo: form.kayitSiraNo?.trim() || undefined,
      })
      showSuccess('Kimlik bilgileri kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4 max-w-2xl">
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Uyruk</Label>
          <Select
            value={form.nationality !== undefined ? String(form.nationality) : undefined}
            onValueChange={(v) => set('nationality', Number(v) as Nationality)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(NationalityLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Medeni Hali</Label>
          <Select
            value={form.maritalStatus !== undefined ? String(form.maritalStatus) : undefined}
            onValueChange={(v) => set('maritalStatus', Number(v) as MaritalStatus)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(MaritalStatusLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Seri</Label>
          <Input value={form.identitySeriNo ?? ''} onChange={(e) => set('identitySeriNo', e.target.value)} placeholder="Seri" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Sıra</Label>
          <Input value={form.identitySiraNo ?? ''} onChange={(e) => set('identitySiraNo', e.target.value)} placeholder="Sıra" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Pasaport No</Label>
          <Input value={form.passportNo ?? ''} onChange={(e) => set('passportNo', e.target.value)} placeholder="Pasaport No" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Doğum Tarihi</Label>
          <Input
            type="date"
            value={form.birthDate ? form.birthDate.slice(0, 10) : ''}
            onChange={(e) => set('birthDate', e.target.value || undefined)}
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Baba Adı</Label>
          <Input value={form.fatherName ?? ''} onChange={(e) => set('fatherName', e.target.value)} placeholder="Baba adı" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Ana Adı</Label>
          <Input value={form.motherName ?? ''} onChange={(e) => set('motherName', e.target.value)} placeholder="Ana adı" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Doğum Yeri</Label>
          <Input value={form.birthPlace ?? ''} onChange={(e) => set('birthPlace', e.target.value)} placeholder="Doğum yeri" />
        </div>
      </div>

      <div className="border rounded-lg p-3 space-y-3">
        <p className="text-sm font-semibold">Kayıtlı Olduğu Yer</p>
        <div className="grid grid-cols-2 gap-3">
          <div className="space-y-1">
            <Label className="text-xs font-medium">İl</Label>
            <Input value={form.registeredCity ?? ''} onChange={(e) => set('registeredCity', e.target.value)} placeholder="İl" />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">İlçe</Label>
            <Input value={form.registeredDistrict ?? ''} onChange={(e) => set('registeredDistrict', e.target.value)} placeholder="İlçe" />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Mahalle-Köy</Label>
            <Input value={form.registeredNeighborhood ?? ''} onChange={(e) => set('registeredNeighborhood', e.target.value)} placeholder="Mahalle-Köy" />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Aile Sıra No</Label>
            <Input value={form.familySiraNo ?? ''} onChange={(e) => set('familySiraNo', e.target.value)} placeholder="Aile sıra no" />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Sıra No</Label>
            <Input value={form.kayitSiraNo ?? ''} onChange={(e) => set('kayitSiraNo', e.target.value)} placeholder="Sıra no" />
          </div>
        </div>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
