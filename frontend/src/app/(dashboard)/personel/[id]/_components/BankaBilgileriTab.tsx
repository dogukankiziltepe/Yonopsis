'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { PickerDialog } from '@/components/picker-dialog'
import { personelApi } from '@/lib/api/personel'
import { bankaApi } from '@/lib/api/banka'
import { showSuccess, showError } from '@/lib/toast'
import { toUpdatePersonelDto } from '@/types/personelDetail'
import type { PersonelCoreDto, PersonelBankaBilgisiDto, PersonelIzinOzetiDto } from '@/types/personelDetail'
import type { BankaSubesiPickerItem } from '@/types/banka'

interface Props {
  personelId: string
  core: PersonelCoreDto
  izinOzeti: PersonelIzinOzetiDto
  data: PersonelBankaBilgisiDto
  onSaved: () => void
}

export function BankaBilgileriTab({ personelId, core, izinOzeti, data, onSaved }: Props) {
  const [bankaSubesiId, setBankaSubesiId] = useState(data.bankaSubesiId)
  const [displayValue, setDisplayValue] = useState(
    data.bankaAdi && data.subeAdi ? `${data.bankaAdi} - ${data.subeAdi}` : ''
  )
  const [hesapNo, setHesapNo] = useState(data.bankaHesapNo ?? '')
  const [iban, setIban] = useState(data.bankaIBAN ?? '')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setBankaSubesiId(data.bankaSubesiId)
    setDisplayValue(data.bankaAdi && data.subeAdi ? `${data.bankaAdi} - ${data.subeAdi}` : '')
    setHesapNo(data.bankaHesapNo ?? '')
    setIban(data.bankaIBAN ?? '')
  }, [data])

  const handleSave = async () => {
    setSaving(true)
    try {
      await personelApi.update(personelId, toUpdatePersonelDto(core, data, izinOzeti, {
        bankaSubesiId,
        bankaHesapNo: hesapNo || null,
        bankaIBAN: iban || null,
      }))
      showSuccess('Banka bilgileri kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4 max-w-lg">
      <PickerDialog<BankaSubesiPickerItem>
        label="Banka/Şube"
        displayValue={displayValue}
        fetchItems={(search) => bankaApi.getSubeler({ search }).then((r) => r.data)}
        columns={[
          { key: 'bankaAdi', label: 'Banka', render: (s) => s.bankaAdi },
          { key: 'subeAdi', label: 'Şube', render: (s) => s.subeAdi },
        ]}
        getId={(s) => s.id}
        onSelect={(s) => { setBankaSubesiId(s.id); setDisplayValue(`${s.bankaAdi} - ${s.subeAdi}`) }}
        onClear={() => { setBankaSubesiId(null); setDisplayValue('') }}
      />
      <div className="space-y-1">
        <Label className="text-xs font-medium">Hesap No</Label>
        <Input value={hesapNo} onChange={(e) => setHesapNo(e.target.value)} />
      </div>
      <div className="space-y-1">
        <Label className="text-xs font-medium">Iban</Label>
        <Input value={iban} onChange={(e) => setIban(e.target.value)} placeholder="TR.." />
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
