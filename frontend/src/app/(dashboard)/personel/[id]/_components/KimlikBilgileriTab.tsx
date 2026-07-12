'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { personelApi } from '@/lib/api/personel'
import { showSuccess, showError } from '@/lib/toast'
import { MaritalStatusLabel } from '@/types/personDetail'
import type { PersonelKimlikDto } from '@/types/personelDetail'

interface Props {
  personelId: string
  data: PersonelKimlikDto
  onSaved: () => void
}

export function KimlikBilgileriTab({ personelId, data, onSaved }: Props) {
  const [form, setForm] = useState<PersonelKimlikDto>(data)
  const [saving, setSaving] = useState(false)

  useEffect(() => setForm(data), [data])

  const set = <K extends keyof PersonelKimlikDto>(key: K, value: PersonelKimlikDto[K]) =>
    setForm((prev) => ({ ...prev, [key]: value }))

  const handleSave = async () => {
    setSaving(true)
    try {
      await personelApi.updateKimlik(personelId, form)
      showSuccess('Kimlik bilgileri kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4 max-w-3xl">
      <div className="grid grid-cols-3 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Tc Kimlik Numarası</Label>
          <Input value={form.tcKimlikNo ?? ''} onChange={(e) => set('tcKimlikNo', e.target.value)} maxLength={11} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Seri</Label>
          <Input value={form.seri ?? ''} onChange={(e) => set('seri', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Sıra</Label>
          <Input value={form.sira ?? ''} onChange={(e) => set('sira', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Baba Adı</Label>
          <Input value={form.babaAdi ?? ''} onChange={(e) => set('babaAdi', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Ana Adı</Label>
          <Input value={form.anaAdi ?? ''} onChange={(e) => set('anaAdi', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Önceki Soyad</Label>
          <Input value={form.oncekiSoyad ?? ''} onChange={(e) => set('oncekiSoyad', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Doğum Yeri</Label>
          <Input value={form.dogumYeri ?? ''} onChange={(e) => set('dogumYeri', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Doğum Tarihi</Label>
          <Input type="date" value={form.dogumTarihi ?? ''} onChange={(e) => set('dogumTarihi', e.target.value || null)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Medeni Hali</Label>
          <Select
            value={form.medeniHali !== undefined && form.medeniHali !== null ? String(form.medeniHali) : undefined}
            onValueChange={(v) => set('medeniHali', Number(v) as PersonelKimlikDto['medeniHali'])}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(MaritalStatusLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="border rounded-lg p-3 space-y-3">
        <p className="text-sm font-semibold">Nüfus Kayıt Bilgileri</p>
        <div className="grid grid-cols-3 gap-3">
          <div className="space-y-1">
            <Label className="text-xs font-medium">İl</Label>
            <Input value={form.il ?? ''} onChange={(e) => set('il', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">İlçe</Label>
            <Input value={form.ilce ?? ''} onChange={(e) => set('ilce', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Mahalle-Köy</Label>
            <Input value={form.mahalleKoy ?? ''} onChange={(e) => set('mahalleKoy', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Cilt No</Label>
            <Input value={form.ciltNo ?? ''} onChange={(e) => set('ciltNo', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Aile Sıra No</Label>
            <Input value={form.aileSiraNo ?? ''} onChange={(e) => set('aileSiraNo', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Sıra No</Label>
            <Input value={form.siraNo ?? ''} onChange={(e) => set('siraNo', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Verildiği Yer</Label>
            <Input value={form.verildigiYer ?? ''} onChange={(e) => set('verildigiYer', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Veriliş Nedeni</Label>
            <Input value={form.verilisNedeni ?? ''} onChange={(e) => set('verilisNedeni', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Kayıt No</Label>
            <Input value={form.kayitNo ?? ''} onChange={(e) => set('kayitNo', e.target.value)} />
          </div>
          <div className="space-y-1">
            <Label className="text-xs font-medium">Veriliş Tarihi</Label>
            <Input type="date" value={form.verilisTarihi ?? ''} onChange={(e) => set('verilisTarihi', e.target.value || null)} />
          </div>
        </div>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
