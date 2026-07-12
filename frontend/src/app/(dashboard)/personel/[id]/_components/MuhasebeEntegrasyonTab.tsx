'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { PickerDialog } from '@/components/picker-dialog'
import { personelApi } from '@/lib/api/personel'
import { giderTanimlariApi } from '@/lib/api/tanimlar'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { CariTuru } from '@/types/muhasebe'
import type { HesapListItem } from '@/types/muhasebe'
import type { GiderTanimi } from '@/types/tanimlar'
import { showSuccess, showError } from '@/lib/toast'
import type { PersonelMuhasebeEntegrasyonDto, UpdatePersonelMuhasebeEntegrasyonDto } from '@/types/personelDetail'

interface Props {
  personelId: string
  data: PersonelMuhasebeEntegrasyonDto
  onSaved: () => void
}

type GiderField = keyof {
  [K in keyof UpdatePersonelMuhasebeEntegrasyonDto as K extends `${string}GiderTanimiId` ? K : never]: unknown
}
type HesapField = keyof {
  [K in keyof UpdatePersonelMuhasebeEntegrasyonDto as K extends `${string}HesapId` ? K : never]: unknown
}

const GIDER_FIELDS: { field: GiderField; adiField: keyof PersonelMuhasebeEntegrasyonDto; label: string }[] = [
  { field: 'brutUcretlerGiderTanimiId', adiField: 'brutUcretlerGiderTanimiAdi', label: 'Brüt Ücretler' },
  { field: 'huzurHakkiBrutUcretlerGiderTanimiId', adiField: 'huzurHakkiBrutUcretlerGiderTanimiAdi', label: 'Huzur Hakkı Brüt Ücretler' },
  { field: 'sgkIsverenPayiGiderTanimiId', adiField: 'sgkIsverenPayiGiderTanimiAdi', label: 'Sgk İşveren Payı' },
  { field: 'issizlikSigortasiIsverenPayiGiderTanimiId', adiField: 'issizlikSigortasiIsverenPayiGiderTanimiAdi', label: 'İşsizlik Sigortası İşveren Payı' },
  { field: 'primVeIkramiyelerGiderTanimiId', adiField: 'primVeIkramiyelerGiderTanimiAdi', label: 'Prim Ve İkramiyeler' },
  { field: 'fazlaMesaiGiderTanimiId', adiField: 'fazlaMesaiGiderTanimiAdi', label: 'Fazla Mesai' },
  { field: 'kidemTazminatlariGiderTanimiId', adiField: 'kidemTazminatlariGiderTanimiAdi', label: 'Kıdem Tazminatları' },
  { field: 'ihbarTazminatlariGiderTanimiId', adiField: 'ihbarTazminatlariGiderTanimiAdi', label: 'İhbar Tazminatları' },
  { field: 'yolYardimiGiderTanimiId', adiField: 'yolYardimiGiderTanimiAdi', label: 'Yol Yardımı' },
  { field: 'yemekYardimiGiderTanimiId', adiField: 'yemekYardimiGiderTanimiAdi', label: 'Yemek Yardımı' },
]

const HESAP_FIELDS: { field: HesapField; adiField: keyof PersonelMuhasebeEntegrasyonDto; label: string }[] = [
  { field: 'personelGelirVergisiHesapId', adiField: 'personelGelirVergisiHesapAdi', label: 'Personel Gelir Vergisi' },
  { field: 'personelDamgaVergisiHesapId', adiField: 'personelDamgaVergisiHesapAdi', label: 'Personel Damga Vergisi' },
  { field: 'odenecekSgkHesapId', adiField: 'odenecekSgkHesapAdi', label: 'Ödenecek Sgk' },
  { field: 'asgariGecimIndirimiHesapId', adiField: 'asgariGecimIndirimiHesapAdi', label: 'Asgari Geçim İndirimi (Agi)' },
  { field: 'icraKesintisiHesapId', adiField: 'icraKesintisiHesapAdi', label: 'İcra Kesintisi' },
  { field: 'digerKesintilerHesapId', adiField: 'digerKesintilerHesapAdi', label: 'Diğer Kesintiler' },
  { field: 'besHesapId', adiField: 'besHesapAdi', label: 'Bes' },
]

export function MuhasebeEntegrasyonTab({ personelId, data, onSaved }: Props) {
  const [form, setForm] = useState<UpdatePersonelMuhasebeEntegrasyonDto>(data)
  const [labels, setLabels] = useState<Record<string, string | null | undefined>>(data as unknown as Record<string, string | null | undefined>)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setForm(data)
    setLabels(data as unknown as Record<string, string | null | undefined>)
  }, [data])

  const handleSave = async () => {
    setSaving(true)
    try {
      await personelApi.updateMuhasebeEntegrasyon(personelId, form)
      showSuccess('Muhasebe entegrasyon hesap kodları kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-6 max-w-3xl">
      <div className="space-y-3">
        <p className="text-sm font-semibold">Gider Tanımı ile İlişkilendirilecek Kalemler</p>
        <div className="grid grid-cols-2 gap-3">
          {GIDER_FIELDS.map(({ field, adiField, label }) => (
            <PickerDialog<GiderTanimi>
              key={field}
              label={label}
              displayValue={labels[adiField] ?? ''}
              fetchItems={() => giderTanimlariApi.getAll().then((r) => r.data)}
              columns={[
                { key: 'giderKodu', label: 'Kod', render: (g) => g.giderKodu },
                { key: 'name', label: 'Ad', render: (g) => g.name },
              ]}
              getId={(g) => g.id}
              onSelect={(g) => {
                setForm((p) => ({ ...p, [field]: g.id }))
                setLabels((p) => ({ ...p, [adiField]: `${g.giderKodu} - ${g.name}` }))
              }}
              onClear={() => {
                setForm((p) => ({ ...p, [field]: null }))
                setLabels((p) => ({ ...p, [adiField]: '' }))
              }}
            />
          ))}
        </div>
      </div>

      <div className="space-y-3">
        <p className="text-sm font-semibold">Cari Hesap ile İlişkilendirilecek Kalemler</p>
        <div className="grid grid-cols-2 gap-3">
          {HESAP_FIELDS.map(({ field, adiField, label }) => (
            <PickerDialog<HesapListItem>
              key={field}
              label={label}
              displayValue={labels[adiField] ?? ''}
              fetchItems={(search) => muhasebeApi.getCariHesaplar({ cariTuru: CariTuru.Personel, search }).then((r) => r.data)}
              columns={[
                { key: 'hesapKodu', label: 'Kod', render: (h) => h.hesapKodu },
                { key: 'hesapAdi', label: 'Ad', render: (h) => h.hesapAdi },
              ]}
              getId={(h) => h.id}
              onSelect={(h) => {
                setForm((p) => ({ ...p, [field]: h.id }))
                setLabels((p) => ({ ...p, [adiField]: `${h.hesapKodu} - ${h.hesapAdi}` }))
              }}
              onClear={() => {
                setForm((p) => ({ ...p, [field]: null }))
                setLabels((p) => ({ ...p, [adiField]: '' }))
              }}
            />
          ))}
        </div>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
