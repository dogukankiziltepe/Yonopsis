'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Search, Scale } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { avukatlarApi } from '@/lib/api/icra'
import type { Avukat, SaveAvukatDto } from '@/types/icra'
import { showSuccess, showApiError } from '@/lib/toast'

const bos: SaveAvukatDto = { adSoyad: '', buroAdi: '', telefon: '', eposta: '', adres: '', isActive: true }

export default function AvukatlarPage() {
  const [items, setItems] = useState<Avukat[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const [open, setOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [form, setForm] = useState<SaveAvukatDto>(bos)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await avukatlarApi.getAll(search || undefined)
      setItems(res.data)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const ac = (a?: Avukat) => {
    setEditId(a?.id ?? null)
    setForm(a ? { adSoyad: a.adSoyad, buroAdi: a.buroAdi ?? '', telefon: a.telefon ?? '', eposta: a.eposta ?? '', adres: a.adres ?? '', isActive: a.isActive } : bos)
    setDeleteConfirm(false)
    setOpen(true)
  }
  const set = (patch: Partial<SaveAvukatDto>) => setForm(f => ({ ...f, ...patch }))

  const kaydet = async () => {
    if (!form.adSoyad.trim()) { showApiError('Avukat adı zorunludur.'); return }
    setSaving(true)
    try {
      const dto: SaveAvukatDto = {
        adSoyad: form.adSoyad.trim(), isActive: form.isActive,
        buroAdi: form.buroAdi || undefined, telefon: form.telefon || undefined,
        eposta: form.eposta || undefined, adres: form.adres || undefined,
      }
      if (editId) await avukatlarApi.update(editId, dto)
      else await avukatlarApi.create(dto)
      showSuccess(editId ? 'Avukat güncellendi.' : 'Avukat eklendi.')
      setOpen(false)
      load()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const sil = async () => {
    if (!editId) return
    try {
      await avukatlarApi.delete(editId)
      showSuccess('Avukat silindi.')
      setOpen(false)
      load()
    } catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Avukatlar</h1>
        <Button size="sm" onClick={() => ac()}><Plus className="h-4 w-4 mr-1" />Yeni Avukat</Button>
      </div>

      <div className="relative max-w-xs">
        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
        <Input className="pl-8" placeholder="Ad, büro ara..." value={searchInput} onChange={e => setSearchInput(e.target.value)} />
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><Scale className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Avukat kaydı bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Ad Soyad</th>
                <th className="text-left px-3 py-2 font-medium">Büro</th>
                <th className="text-left px-3 py-2 font-medium">Telefon</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">E-Posta</th>
                <th className="text-left px-3 py-2 font-medium">Durum</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(a => (
                <tr key={a.id} className="hover:bg-muted/30 cursor-pointer" onClick={() => ac(a)}>
                  <td className="px-3 py-2.5">{a.adSoyad}</td>
                  <td className="px-3 py-2.5">{a.buroAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs">{a.telefon ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs hidden md:table-cell">{a.eposta ?? '—'}</td>
                  <td className="px-3 py-2.5 text-xs">{a.isActive ? 'Aktif' : <span className="text-muted-foreground">Pasif</span>}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>{editId ? 'Avukat Düzenle' : 'Yeni Avukat'}</DialogTitle>
          </DialogHeader>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div className="space-y-1.5 sm:col-span-2">
              <Label htmlFor="ad">Ad Soyad <span className="text-destructive">*</span></Label>
              <Input id="ad" value={form.adSoyad} onChange={e => set({ adSoyad: e.target.value })} maxLength={200} />
            </div>
            <div className="space-y-1.5 sm:col-span-2">
              <Label htmlFor="buro">Büro Adı</Label>
              <Input id="buro" value={form.buroAdi} onChange={e => set({ buroAdi: e.target.value })} maxLength={200} />
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="tel">Telefon</Label>
              <Input id="tel" value={form.telefon} onChange={e => set({ telefon: e.target.value })} maxLength={30} />
            </div>
            <div className="space-y-1.5">
              <Label htmlFor="eposta">E-Posta</Label>
              <Input id="eposta" type="email" value={form.eposta} onChange={e => set({ eposta: e.target.value })} maxLength={200} />
            </div>
            <div className="space-y-1.5 sm:col-span-2">
              <Label htmlFor="adres">Adres</Label>
              <textarea id="adres" value={form.adres} onChange={e => set({ adres: e.target.value })} maxLength={500} rows={2}
                className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
            </div>
            <label className="flex items-center gap-2 text-sm cursor-pointer w-fit">
              <input type="checkbox" className="h-4 w-4" checked={form.isActive} onChange={e => set({ isActive: e.target.checked })} />
              Aktif
            </label>
          </div>
          <div className="flex items-center justify-between pt-2">
            <div>
              {editId && (deleteConfirm
                ? <div className="flex gap-2">
                    <Button size="sm" variant="destructive" onClick={sil}>Silmeyi Onayla</Button>
                    <Button size="sm" variant="ghost" onClick={() => setDeleteConfirm(false)}>Vazgeç</Button>
                  </div>
                : <Button size="sm" variant="ghost" className="text-destructive" onClick={() => setDeleteConfirm(true)}>Sil</Button>)}
            </div>
            <div className="flex gap-2">
              <Button variant="outline" onClick={() => setOpen(false)}>İptal</Button>
              <Button onClick={kaydet} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  )
}
