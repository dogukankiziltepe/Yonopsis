'use client'

import { useEffect, useState } from 'react'
import { Plus, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { personsApi } from '@/lib/api/persons'
import { showSuccess, showError } from '@/lib/toast'
import { PersonGeneralInfoDto, PersonPhoneInputDto } from '@/types/personDetail'

interface Props {
  userSiteId: string
  data: PersonGeneralInfoDto
  onSaved: () => void
}

export function GeneralInfoTab({ userSiteId, data, onSaved }: Props) {
  const [taxOffice, setTaxOffice] = useState(data.taxOffice ?? '')
  const [secondaryEmail, setSecondaryEmail] = useState(data.secondaryEmail ?? '')
  const [address, setAddress] = useState(data.address ?? '')
  const [phones, setPhones] = useState<PersonPhoneInputDto[]>(
    data.phones.map((p) => ({ phoneNumber: p.phoneNumber, label: p.label }))
  )
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setTaxOffice(data.taxOffice ?? '')
    setSecondaryEmail(data.secondaryEmail ?? '')
    setAddress(data.address ?? '')
    setPhones(data.phones.map((p) => ({ phoneNumber: p.phoneNumber, label: p.label })))
  }, [data])

  const addPhone = () => setPhones([...phones, { phoneNumber: '', label: '' }])
  const removePhone = (index: number) => setPhones(phones.filter((_, i) => i !== index))
  const updatePhone = (index: number, field: 'phoneNumber' | 'label', value: string) => {
    setPhones(phones.map((p, i) => (i === index ? { ...p, [field]: value } : p)))
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      await personsApi.updateGeneralInfo(userSiteId, {
        taxOffice: taxOffice.trim() || undefined,
        secondaryEmail: secondaryEmail.trim() || undefined,
        address: address.trim() || undefined,
        phones: phones.filter((p) => p.phoneNumber.trim()).map((p) => ({
          phoneNumber: p.phoneNumber.trim(),
          label: p.label?.trim() || undefined,
        })),
      })
      showSuccess('Genel bilgiler kaydedildi.')
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
          <Label className="text-xs font-medium">Adı Soyadı / Unvan</Label>
          <Input value={`${data.firstName} ${data.lastName}`} disabled />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">TC Kimlik No / Vergi No</Label>
          <Input value={data.nationalId ?? ''} disabled />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Vergi Dairesi</Label>
          <Input value={taxOffice} onChange={(e) => setTaxOffice(e.target.value)} placeholder="Vergi dairesi" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">E-Posta</Label>
          <Input value={data.email} disabled />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">İkincil E-Posta</Label>
          <Input value={secondaryEmail} onChange={(e) => setSecondaryEmail(e.target.value)} placeholder="ikincil@mail.com" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Aktif</Label>
          <div>
            <Badge variant={data.isActive ? 'default' : 'secondary'}>{data.isActive ? 'Aktif' : 'Pasif'}</Badge>
          </div>
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Adres</Label>
        <Input value={address} onChange={(e) => setAddress(e.target.value)} placeholder="Adres" />
      </div>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <Label className="text-xs font-medium">Telefon</Label>
          <Button type="button" variant="outline" size="sm" onClick={addPhone}>
            <Plus className="h-3.5 w-3.5 mr-1" />
            Telefon Ekle
          </Button>
        </div>
        {phones.length === 0 ? (
          <p className="text-xs text-muted-foreground">Kayıtlı telefon numarası yok.</p>
        ) : (
          <div className="space-y-2">
            {phones.map((phone, index) => (
              <div key={index} className="flex gap-2 items-center">
                <Input
                  value={phone.phoneNumber}
                  onChange={(e) => updatePhone(index, 'phoneNumber', e.target.value)}
                  placeholder="0532 000 00 00"
                  className="flex-1"
                />
                <Input
                  value={phone.label ?? ''}
                  onChange={(e) => updatePhone(index, 'label', e.target.value)}
                  placeholder="Etiket (Cep, İş vb.)"
                  className="w-40"
                />
                <Button type="button" variant="ghost" size="icon" onClick={() => removePhone(index)}>
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </div>
            ))}
          </div>
        )}
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
