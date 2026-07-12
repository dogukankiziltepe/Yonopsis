'use client'

import { useState } from 'react'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from '@/components/ui/table'
import { personelApi } from '@/lib/api/personel'
import { showSuccess, showError } from '@/lib/toast'
import type { PersonelEgitimDto } from '@/types/personelDetail'

interface Props {
  personelId: string
  egitimler: PersonelEgitimDto[]
  onSaved: () => void
}

interface EgitimForm {
  egitiminKonusu: string
  egitmen: string
  egitimYeri: string
  baslamaTarihi: string
  bitisTarihi: string
  toplamSaat: string
}

const EMPTY_FORM: EgitimForm = { egitiminKonusu: '', egitmen: '', egitimYeri: '', baslamaTarihi: '', bitisTarihi: '', toplamSaat: '' }

export function EgitimlerTab({ personelId, egitimler, onSaved }: Props) {
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<PersonelEgitimDto | null>(null)
  const [form, setForm] = useState<EgitimForm>(EMPTY_FORM)
  const [saving, setSaving] = useState(false)

  const openCreate = () => { setEditing(null); setForm(EMPTY_FORM); setOpen(true) }
  const openEdit = (e: PersonelEgitimDto) => {
    setEditing(e)
    setForm({
      egitiminKonusu: e.egitiminKonusu,
      egitmen: e.egitmen ?? '',
      egitimYeri: e.egitimYeri ?? '',
      baslamaTarihi: e.baslamaTarihi ?? '',
      bitisTarihi: e.bitisTarihi ?? '',
      toplamSaat: e.toplamSaat != null ? String(e.toplamSaat) : '',
    })
    setOpen(true)
  }

  const handleSave = async () => {
    if (!form.egitiminKonusu.trim()) { showError('Eğitimin konusu zorunludur.'); return }
    setSaving(true)
    const payload = {
      egitiminKonusu: form.egitiminKonusu.trim(),
      egitmen: form.egitmen || null,
      egitimYeri: form.egitimYeri || null,
      baslamaTarihi: form.baslamaTarihi || null,
      bitisTarihi: form.bitisTarihi || null,
      toplamSaat: form.toplamSaat ? Number(form.toplamSaat) : null,
    }
    try {
      if (editing) await personelApi.updateEgitim(editing.id, payload)
      else await personelApi.addEgitim(personelId, payload)
      showSuccess('Eğitim kaydı kaydedildi.')
      setOpen(false)
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu eğitim kaydını silmek istiyor musunuz?')) return
    try { await personelApi.deleteEgitim(id); onSaved() } catch { showError('Silinemedi.') }
  }

  return (
    <div className="space-y-3">
      <div className="flex justify-end">
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Eğitim</Button>
      </div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Eğitimin Konusu</TableHead>
            <TableHead>Eğitmen</TableHead>
            <TableHead>Eğitim Yeri</TableHead>
            <TableHead>Başlama Tarihi</TableHead>
            <TableHead>Bitiş Tarihi</TableHead>
            <TableHead>Toplam Saat</TableHead>
            <TableHead />
          </TableRow>
        </TableHeader>
        <TableBody>
          {egitimler.length === 0 ? (
            <TableRow><TableCell colSpan={7} className="text-center text-muted-foreground">Eğitim kaydı yok.</TableCell></TableRow>
          ) : egitimler.map((e) => (
            <TableRow key={e.id}>
              <TableCell>{e.egitiminKonusu}</TableCell>
              <TableCell>{e.egitmen ?? '—'}</TableCell>
              <TableCell>{e.egitimYeri ?? '—'}</TableCell>
              <TableCell>{e.baslamaTarihi ?? '—'}</TableCell>
              <TableCell>{e.bitisTarihi ?? '—'}</TableCell>
              <TableCell>{e.toplamSaat ?? '—'}</TableCell>
              <TableCell>
                <div className="flex gap-1">
                  <Button size="icon" variant="ghost" className="h-7 w-7" onClick={() => openEdit(e)}><Pencil className="h-3.5 w-3.5" /></Button>
                  <Button size="icon" variant="ghost" className="h-7 w-7" onClick={() => handleDelete(e.id)}><Trash2 className="h-3.5 w-3.5 text-destructive" /></Button>
                </div>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader><DialogTitle>{editing ? 'Eğitim Düzenle' : 'Yeni Eğitim'}</DialogTitle></DialogHeader>
          <div className="space-y-3 py-2">
            <div className="space-y-1">
              <Label className="text-xs">Eğitimin Konusu *</Label>
              <Input value={form.egitiminKonusu} onChange={(e) => setForm((p) => ({ ...p, egitiminKonusu: e.target.value }))} />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Eğitmen</Label>
                <Input value={form.egitmen} onChange={(e) => setForm((p) => ({ ...p, egitmen: e.target.value }))} />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Eğitim Yeri</Label>
                <Input value={form.egitimYeri} onChange={(e) => setForm((p) => ({ ...p, egitimYeri: e.target.value }))} />
              </div>
            </div>
            <div className="grid grid-cols-3 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Başlama Tarihi</Label>
                <Input type="date" value={form.baslamaTarihi} onChange={(e) => setForm((p) => ({ ...p, baslamaTarihi: e.target.value }))} />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Bitiş Tarihi</Label>
                <Input type="date" value={form.bitisTarihi} onChange={(e) => setForm((p) => ({ ...p, bitisTarihi: e.target.value }))} />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Toplam Saat</Label>
                <Input type="number" value={form.toplamSaat} onChange={(e) => setForm((p) => ({ ...p, toplamSaat: e.target.value }))} />
              </div>
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
