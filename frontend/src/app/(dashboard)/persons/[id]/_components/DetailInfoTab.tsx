'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
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
  PersonDetailInfoDto,
  Gender,
  GenderLabel,
  EducationStatus,
  EducationStatusLabel,
  PetType,
  PetTypeLabel,
} from '@/types/personDetail'

interface Props {
  userSiteId: string
  data: PersonDetailInfoDto
  onSaved: () => void
}

export function DetailInfoTab({ userSiteId, data, onSaved }: Props) {
  const [description, setDescription] = useState(data.description ?? '')
  const [gender, setGender] = useState<Gender | undefined>(data.gender)
  const [educationStatus, setEducationStatus] = useState<EducationStatus | undefined>(data.educationStatus)
  const [schoolOrInstitution, setSchoolOrInstitution] = useState(data.schoolOrInstitution ?? '')
  const [profession, setProfession] = useState(data.profession ?? '')
  const [hasPrivateInsurance, setHasPrivateInsurance] = useState(data.hasPrivateInsurance)
  const [isMartyrOrVeteranRelative, setIsMartyrOrVeteranRelative] = useState(data.isMartyrOrVeteranRelative)
  const [petType, setPetType] = useState<PetType | undefined>(data.petType)
  const [petDetail, setPetDetail] = useState(data.petDetail ?? '')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setDescription(data.description ?? '')
    setGender(data.gender)
    setEducationStatus(data.educationStatus)
    setSchoolOrInstitution(data.schoolOrInstitution ?? '')
    setProfession(data.profession ?? '')
    setHasPrivateInsurance(data.hasPrivateInsurance)
    setIsMartyrOrVeteranRelative(data.isMartyrOrVeteranRelative)
    setPetType(data.petType)
    setPetDetail(data.petDetail ?? '')
  }, [data])

  const handleSave = async () => {
    setSaving(true)
    try {
      await personsApi.updateDetailInfo(userSiteId, {
        description: description.trim() || undefined,
        gender,
        educationStatus,
        schoolOrInstitution: schoolOrInstitution.trim() || undefined,
        profession: profession.trim() || undefined,
        hasPrivateInsurance,
        isMartyrOrVeteranRelative,
        petType,
        petDetail: petDetail.trim() || undefined,
      })
      showSuccess('Detay bilgileri kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4 max-w-2xl">
      <div className="space-y-1">
        <Label className="text-xs font-medium">Açıklama</Label>
        <Input value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Açıklama" />
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Muhasebe Hesap Kodu</Label>
        <Input value={data.cariHesapKodu ? `${data.cariHesapKodu} — ${data.cariHesapAdi}` : 'Kayıtlı cari hesap yok'} disabled />
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Cinsiyet</Label>
        <RadioGroup
          className="flex gap-4"
          value={gender !== undefined ? String(gender) : undefined}
          onValueChange={(v) => setGender(Number(v) as Gender)}
        >
          {([0, 1, 2] as Gender[]).map((g) => (
            <div key={g} className="flex items-center gap-2">
              <RadioGroupItem value={String(g)} id={`gender-${g}`} />
              <Label htmlFor={`gender-${g}`} className="text-sm font-normal cursor-pointer">
                {g === 0 ? 'E' : g === 1 ? 'K' : GenderLabel[g]}
              </Label>
            </div>
          ))}
        </RadioGroup>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Öğrenim Durumu</Label>
          <Select
            value={educationStatus !== undefined ? String(educationStatus) : undefined}
            onValueChange={(v) => setEducationStatus(Number(v) as EducationStatus)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(EducationStatusLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Okul / Kurum</Label>
          <Input value={schoolOrInstitution} onChange={(e) => setSchoolOrInstitution(e.target.value)} placeholder="Okul / Kurum" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Mesleği</Label>
          <Input value={profession} onChange={(e) => setProfession(e.target.value)} placeholder="Mesleği" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Evcil Hayvan</Label>
          <Select
            value={petType !== undefined ? String(petType) : undefined}
            onValueChange={(v) => setPetType(Number(v) as PetType)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(PetTypeLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {petType != null && petType !== 0 && (
        <div className="space-y-1">
          <Label className="text-xs font-medium">Evcil Hayvan Detayı</Label>
          <Input value={petDetail} onChange={(e) => setPetDetail(e.target.value)} placeholder="Cins, yaş vb." />
        </div>
      )}

      <div className="flex gap-6">
        <div className="flex items-center gap-2">
          <Checkbox
            id="hasPrivateInsurance"
            checked={hasPrivateInsurance}
            onCheckedChange={(v) => setHasPrivateInsurance(v === true)}
          />
          <Label htmlFor="hasPrivateInsurance" className="text-sm font-normal cursor-pointer">Özel Sigorta</Label>
        </div>
        <div className="flex items-center gap-2">
          <Checkbox
            id="isMartyrOrVeteranRelative"
            checked={isMartyrOrVeteranRelative}
            onCheckedChange={(v) => setIsMartyrOrVeteranRelative(v === true)}
          />
          <Label htmlFor="isMartyrOrVeteranRelative" className="text-sm font-normal cursor-pointer">Şehit / Gazi Yakını</Label>
        </div>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
