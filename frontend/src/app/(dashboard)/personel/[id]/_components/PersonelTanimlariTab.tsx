'use client'

import { useEffect, useState } from 'react'
import { Plus, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '@/components/ui/table'
import { PickerDialog } from '@/components/picker-dialog'
import { personelApi } from '@/lib/api/personel'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { CariTuru } from '@/types/muhasebe'
import type { HesapListItem } from '@/types/muhasebe'
import { showSuccess, showError } from '@/lib/toast'
import { GenderLabel, EducationStatusLabel } from '@/types/personDetail'
import type { Gender, EducationStatus } from '@/types/personDetail'
import { KanGrubuLabel, toUpdatePersonelDto } from '@/types/personelDetail'
import type {
  KanGrubu, PersonelCoreDto, PersonelBankaBilgisiDto, PersonelIzinOzetiDto,
  PersonelTelefonDto, PersonelAcilDurumKisiDto,
} from '@/types/personelDetail'

interface FormState {
  personelKodu: string
  name: string
  firma: string
  title: string
  cinsiyet?: Gender | null
  yemekKarti: string
  aciklama: string
  email: string
  kanGrubu?: KanGrubu | null
  ogrenimDurumu?: EducationStatus | null
  okulKurum: string
  adres: string
  startDate: string
  cikisTarihi: string
  kidemTazminatiBaslamaTarihi: string
  isActive: boolean
  muhasebeHesapKoduId?: string | null
}

interface Props {
  personelId: string
  core: PersonelCoreDto
  banka: PersonelBankaBilgisiDto
  izinOzeti: PersonelIzinOzetiDto
  telefonlar: PersonelTelefonDto[]
  acilDurumKisileri: PersonelAcilDurumKisiDto[]
  onSaved: () => void
}

const toFormState = (core: PersonelCoreDto): FormState => ({
  personelKodu: core.personelKodu,
  name: core.name,
  firma: core.firma ?? '',
  title: core.title,
  cinsiyet: core.cinsiyet,
  yemekKarti: core.yemekKarti ?? '',
  aciklama: core.aciklama ?? '',
  email: core.email ?? '',
  kanGrubu: core.kanGrubu,
  ogrenimDurumu: core.ogrenimDurumu,
  okulKurum: core.okulKurum ?? '',
  adres: core.adres ?? '',
  startDate: core.startDate ?? '',
  cikisTarihi: core.cikisTarihi ?? '',
  kidemTazminatiBaslamaTarihi: core.kidemTazminatiBaslamaTarihi ?? '',
  isActive: core.isActive,
  muhasebeHesapKoduId: core.muhasebeHesapKoduId,
})

export function PersonelTanimlariTab({ personelId, core, banka, izinOzeti, telefonlar, acilDurumKisileri, onSaved }: Props) {
  const [form, setForm] = useState<FormState>(toFormState(core))
  const [muhasebeHesapKoduAdi, setMuhasebeHesapKoduAdi] = useState(core.muhasebeHesapKoduAdi ?? '')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setForm(toFormState(core))
    setMuhasebeHesapKoduAdi(core.muhasebeHesapKoduAdi ?? '')
  }, [core])

  const set = <K extends keyof FormState>(key: K, value: FormState[K]) =>
    setForm((prev) => ({ ...prev, [key]: value }))

  const handleSave = async () => {
    if (!form.name.trim()) { showError('Ad Soyad zorunludur.'); return }
    if (!form.personelKodu.trim()) { showError('Personel Kodu zorunludur.'); return }
    if (!form.title.trim()) { showError('Görevi zorunludur.'); return }
    setSaving(true)
    try {
      await personelApi.update(personelId, toUpdatePersonelDto(core, banka, izinOzeti, {
        personelKodu: form.personelKodu.trim(),
        name: form.name.trim(),
        firma: form.firma || null,
        title: form.title.trim(),
        cinsiyet: form.cinsiyet,
        yemekKarti: form.yemekKarti || null,
        aciklama: form.aciklama || null,
        email: form.email || null,
        kanGrubu: form.kanGrubu,
        ogrenimDurumu: form.ogrenimDurumu,
        okulKurum: form.okulKurum || null,
        adres: form.adres || null,
        startDate: form.startDate || null,
        cikisTarihi: form.cikisTarihi || null,
        kidemTazminatiBaslamaTarihi: form.kidemTazminatiBaslamaTarihi || null,
        isActive: form.isActive,
        muhasebeHesapKoduId: form.muhasebeHesapKoduId,
      }))
      showSuccess('Personel tanımları kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  // ── Telefon mini-CRUD ────────────────────────────────────────────
  const [newPhone, setNewPhone] = useState('')
  const addPhone = async () => {
    if (!newPhone.trim()) return
    try {
      await personelApi.addTelefon(personelId, { phoneNumber: newPhone.trim() })
      setNewPhone('')
      onSaved()
    } catch { showError('Telefon eklenemedi.') }
  }
  const deletePhone = async (telId: string) => {
    try { await personelApi.deleteTelefon(telId); onSaved() } catch { showError('Telefon silinemedi.') }
  }

  // ── Acil Durum Kişileri mini-CRUD ────────────────────────────────
  const [newKisi, setNewKisi] = useState({ adSoyad: '', yakinlik: '', telefon: '' })
  const addKisi = async () => {
    if (!newKisi.adSoyad.trim()) return
    try {
      await personelApi.addAcilDurumKisi(personelId, newKisi)
      setNewKisi({ adSoyad: '', yakinlik: '', telefon: '' })
      onSaved()
    } catch { showError('Acil durum kişisi eklenemedi.') }
  }
  const deleteKisi = async (kisiId: string) => {
    try { await personelApi.deleteAcilDurumKisi(kisiId); onSaved() } catch { showError('Silinemedi.') }
  }

  return (
    <div className="space-y-4 max-w-3xl">
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kodu *</Label>
          <Input value={form.personelKodu} onChange={(e) => set('personelKodu', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Adı Soyadı *</Label>
          <Input value={form.name} onChange={(e) => set('name', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Firma</Label>
          <Input value={form.firma} onChange={(e) => set('firma', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Görevi *</Label>
          <Input value={form.title} onChange={(e) => set('title', e.target.value)} />
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Cinsiyet</Label>
        <RadioGroup
          value={form.cinsiyet !== undefined && form.cinsiyet !== null ? String(form.cinsiyet) : undefined}
          onValueChange={(v) => set('cinsiyet', Number(v) as Gender)}
          className="flex gap-4"
        >
          {[0, 1].map((v) => (
            <div key={v} className="flex items-center gap-2">
              <RadioGroupItem value={String(v)} id={`cinsiyet-${v}`} />
              <Label htmlFor={`cinsiyet-${v}`} className="text-sm font-normal">{GenderLabel[v as 0 | 1]}</Label>
            </div>
          ))}
        </RadioGroup>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Yemek Kartı</Label>
          <Input value={form.yemekKarti} onChange={(e) => set('yemekKarti', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">E-Posta</Label>
          <Input type="email" value={form.email} onChange={(e) => set('email', e.target.value)} />
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Açıklama</Label>
        <Input value={form.aciklama} onChange={(e) => set('aciklama', e.target.value)} />
      </div>

      {/* Telefon listesi */}
      <div className="border rounded-lg p-3 space-y-2">
        <p className="text-sm font-semibold">Telefon</p>
        <div className="space-y-1">
          {telefonlar.map((t) => (
            <div key={t.id} className="flex items-center justify-between text-sm">
              <span>{t.phoneNumber}{t.label ? ` (${t.label})` : ''}</span>
              <Button size="icon" variant="ghost" className="h-6 w-6" onClick={() => deletePhone(t.id)}>
                <Trash2 className="h-3 w-3 text-destructive" />
              </Button>
            </div>
          ))}
        </div>
        <div className="flex gap-2">
          <Input value={newPhone} onChange={(e) => setNewPhone(e.target.value)} placeholder="+90 5xx xxx xx xx" className="h-8" />
          <Button size="sm" variant="outline" onClick={addPhone}><Plus className="h-3.5 w-3.5" /></Button>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kan Grubu</Label>
          <Select
            value={form.kanGrubu !== undefined && form.kanGrubu !== null ? String(form.kanGrubu) : undefined}
            onValueChange={(v) => set('kanGrubu', Number(v) as KanGrubu)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(KanGrubuLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Öğrenim Durumu</Label>
          <Select
            value={form.ogrenimDurumu !== undefined && form.ogrenimDurumu !== null ? String(form.ogrenimDurumu) : undefined}
            onValueChange={(v) => set('ogrenimDurumu', Number(v) as EducationStatus)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(EducationStatusLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Okul/Kurum</Label>
        <Input value={form.okulKurum} onChange={(e) => set('okulKurum', e.target.value)} />
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Adres</Label>
        <Input value={form.adres} onChange={(e) => set('adres', e.target.value)} />
      </div>

      {/* Acil Durum Kişileri */}
      <div className="border rounded-lg p-3 space-y-2">
        <p className="text-sm font-semibold">Acil Durumda Ulaşılacak Kişiler</p>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Ad Soyad</TableHead>
              <TableHead>Yakınlık</TableHead>
              <TableHead>Telefon</TableHead>
              <TableHead />
            </TableRow>
          </TableHeader>
          <TableBody>
            {acilDurumKisileri.map((k) => (
              <TableRow key={k.id}>
                <TableCell>{k.adSoyad}</TableCell>
                <TableCell>{k.yakinlik ?? '—'}</TableCell>
                <TableCell>{k.telefon ?? '—'}</TableCell>
                <TableCell>
                  <Button size="icon" variant="ghost" className="h-6 w-6" onClick={() => deleteKisi(k.id)}>
                    <Trash2 className="h-3 w-3 text-destructive" />
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <div className="grid grid-cols-4 gap-2">
          <Input className="h-8" placeholder="Ad Soyad" value={newKisi.adSoyad} onChange={(e) => setNewKisi((p) => ({ ...p, adSoyad: e.target.value }))} />
          <Input className="h-8" placeholder="Yakınlık" value={newKisi.yakinlik} onChange={(e) => setNewKisi((p) => ({ ...p, yakinlik: e.target.value }))} />
          <Input className="h-8" placeholder="Telefon" value={newKisi.telefon} onChange={(e) => setNewKisi((p) => ({ ...p, telefon: e.target.value }))} />
          <Button size="sm" variant="outline" onClick={addKisi}><Plus className="h-3.5 w-3.5 mr-1" />Ekle</Button>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Giriş Tarihi</Label>
          <Input type="date" value={form.startDate} onChange={(e) => set('startDate', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Çıkış Tarihi</Label>
          <Input type="date" value={form.cikisTarihi} onChange={(e) => set('cikisTarihi', e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kıdem Tazminat Başlama Tarihi</Label>
          <Input type="date" value={form.kidemTazminatiBaslamaTarihi} onChange={(e) => set('kidemTazminatiBaslamaTarihi', e.target.value)} />
        </div>
      </div>

      <PickerDialog<HesapListItem>
        label="Muhasebe Hesap Kodu"
        displayValue={muhasebeHesapKoduAdi}
        fetchItems={(search) => muhasebeApi.getCariHesaplar({ cariTuru: CariTuru.Personel, search }).then((r) => r.data)}
        columns={[
          { key: 'hesapKodu', label: 'Kod', render: (h) => h.hesapKodu },
          { key: 'hesapAdi', label: 'Ad', render: (h) => h.hesapAdi },
        ]}
        getId={(h) => h.id}
        onSelect={(h) => { set('muhasebeHesapKoduId', h.id); setMuhasebeHesapKoduAdi(`${h.hesapKodu} - ${h.hesapAdi}`) }}
        onClear={() => { set('muhasebeHesapKoduId', null); setMuhasebeHesapKoduAdi('') }}
      />

      <div className="flex items-center gap-2">
        <Checkbox id="aktif" checked={form.isActive} onCheckedChange={(v) => set('isActive', v === true)} />
        <Label htmlFor="aktif" className="text-sm font-normal">Aktif</Label>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
