'use client'

import { useEffect, useState } from 'react'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '@/components/ui/table'
import { personelApi } from '@/lib/api/personel'
import { showSuccess, showError } from '@/lib/toast'
import { toUpdatePersonelDto, PersonelIzinTuruLabel } from '@/types/personelDetail'
import type {
  PersonelCoreDto, PersonelBankaBilgisiDto, PersonelIzinOzetiDto, PersonelIzinDto, PersonelIzinTuru,
} from '@/types/personelDetail'

interface Props {
  personelId: string
  core: PersonelCoreDto
  banka: PersonelBankaBilgisiDto
  izinOzeti: PersonelIzinOzetiDto
  izinler: PersonelIzinDto[]
  onSaved: () => void
}

interface IzinForm {
  baslangicTarihi: string
  bitisTarihi: string
  izinTuru: PersonelIzinTuru
  aciklama: string
}

const EMPTY_FORM: IzinForm = { baslangicTarihi: '', bitisTarihi: '', izinTuru: 0, aciklama: '' }

export function IzinYonetimiTab({ personelId, core, banka, izinOzeti, izinler, onSaved }: Props) {
  const [hakEdis, setHakEdis] = useState(izinOzeti.yillikIzinHakkiGun != null ? String(izinOzeti.yillikIzinHakkiGun) : '')
  const [savingHakEdis, setSavingHakEdis] = useState(false)

  useEffect(() => {
    setHakEdis(izinOzeti.yillikIzinHakkiGun != null ? String(izinOzeti.yillikIzinHakkiGun) : '')
  }, [izinOzeti.yillikIzinHakkiGun])

  const saveHakEdis = async () => {
    setSavingHakEdis(true)
    try {
      await personelApi.update(personelId, toUpdatePersonelDto(core, banka, izinOzeti, {
        yillikIzinHakkiGun: hakEdis ? Number(hakEdis) : null,
      }))
      showSuccess('İzin hak edişi kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSavingHakEdis(false)
    }
  }

  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<PersonelIzinDto | null>(null)
  const [form, setForm] = useState<IzinForm>(EMPTY_FORM)
  const [saving, setSaving] = useState(false)

  const openCreate = () => { setEditing(null); setForm(EMPTY_FORM); setOpen(true) }
  const openEdit = (i: PersonelIzinDto) => {
    setEditing(i)
    setForm({ baslangicTarihi: i.baslangicTarihi, bitisTarihi: i.bitisTarihi, izinTuru: i.izinTuru, aciklama: i.aciklama ?? '' })
    setOpen(true)
  }

  const handleSave = async () => {
    if (!form.baslangicTarihi || !form.bitisTarihi) { showError('Başlangıç ve bitiş tarihi zorunludur.'); return }
    setSaving(true)
    const payload = {
      baslangicTarihi: form.baslangicTarihi,
      bitisTarihi: form.bitisTarihi,
      izinTuru: form.izinTuru,
      aciklama: form.aciklama || null,
    }
    try {
      if (editing) await personelApi.updateIzin(editing.id, payload)
      else await personelApi.addIzin(personelId, payload)
      showSuccess('İzin kaydı kaydedildi.')
      setOpen(false)
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu izin kaydını silmek istiyor musunuz?')) return
    try { await personelApi.deleteIzin(id); onSaved() } catch { showError('Silinemedi.') }
  }

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-3 gap-3 max-w-xl">
        <div className="space-y-1">
          <Label className="text-xs font-medium">İzin Hak Edişi (Yıllık, gün)</Label>
          <div className="flex gap-1">
            <Input type="number" value={hakEdis} onChange={(e) => setHakEdis(e.target.value)} />
            <Button size="sm" variant="outline" onClick={saveHakEdis} disabled={savingHakEdis}>Kaydet</Button>
          </div>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kullanılan İzin (gün)</Label>
          <Input value={izinOzeti.kullanilanGun} readOnly disabled />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kullanılabilir İzin Bakiyesi</Label>
          <Input value={izinOzeti.bakiyeGun ?? '—'} readOnly disabled />
        </div>
      </div>

      <div className="flex justify-end">
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni İzin</Button>
      </div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Başlangıç Tarihi</TableHead>
            <TableHead>Bitiş Tarihi</TableHead>
            <TableHead>İzin Türü</TableHead>
            <TableHead>Açıklama</TableHead>
            <TableHead>Süre (gün)</TableHead>
            <TableHead />
          </TableRow>
        </TableHeader>
        <TableBody>
          {izinler.length === 0 ? (
            <TableRow><TableCell colSpan={6} className="text-center text-muted-foreground">İzin kaydı yok.</TableCell></TableRow>
          ) : izinler.map((i) => (
            <TableRow key={i.id}>
              <TableCell>{i.baslangicTarihi}</TableCell>
              <TableCell>{i.bitisTarihi}</TableCell>
              <TableCell>{PersonelIzinTuruLabel[i.izinTuru]}</TableCell>
              <TableCell>{i.aciklama ?? '—'}</TableCell>
              <TableCell>{i.sureGun}</TableCell>
              <TableCell>
                <div className="flex gap-1">
                  <Button size="icon" variant="ghost" className="h-7 w-7" onClick={() => openEdit(i)}><Pencil className="h-3.5 w-3.5" /></Button>
                  <Button size="icon" variant="ghost" className="h-7 w-7" onClick={() => handleDelete(i.id)}><Trash2 className="h-3.5 w-3.5 text-destructive" /></Button>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader><DialogTitle>{editing ? 'İzin Düzenle' : 'Yeni İzin'}</DialogTitle></DialogHeader>
          <div className="space-y-3 py-2">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Başlangıç Tarihi *</Label>
                <Input type="date" value={form.baslangicTarihi} onChange={(e) => setForm((p) => ({ ...p, baslangicTarihi: e.target.value }))} />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Bitiş Tarihi *</Label>
                <Input type="date" value={form.bitisTarihi} onChange={(e) => setForm((p) => ({ ...p, bitisTarihi: e.target.value }))} />
              </div>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">İzin Türü</Label>
              <Select value={String(form.izinTuru)} onValueChange={(v) => setForm((p) => ({ ...p, izinTuru: Number(v) as PersonelIzinTuru }))}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                  {Object.entries(PersonelIzinTuruLabel).map(([value, label]) => (
                    <SelectItem key={value} value={value}>{label}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Açıklama</Label>
              <Input value={form.aciklama} onChange={(e) => setForm((p) => ({ ...p, aciklama: e.target.value }))} />
            </div>
          </div>
          <div className="flex justify-end gap-2 pt-2">
            <Button variant="outline" onClick={() => setOpen(false)}>İptal</Button>
            <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
