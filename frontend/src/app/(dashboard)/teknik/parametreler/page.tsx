'use client'

import { useEffect, useState } from 'react'
import { Plus, Pencil, Trash2, X, Settings } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { birimFiyatlarApi } from '@/lib/api/sayac'
import type { BirimFiyat, CreateBirimFiyatDto, UpdateBirimFiyatDto } from '@/types/sayac'
import { SayacTipi, SayacTipiLabel } from '@/types/sayac'
import { showSuccess, showApiError } from '@/lib/toast'

const emptyForm = (): CreateBirimFiyatDto => ({
  tip: SayacTipi.Elektrik,
  fiyat: 0,
  birim: 'kWh',
  baslangicTarihi: new Date().toISOString().slice(0, 10),
})

const tipBirim: Record<SayacTipi, string> = {
  [SayacTipi.Elektrik]: 'kWh',
  [SayacTipi.Su]: 'm³',
  [SayacTipi.Dogalgaz]: 'm³',
  [SayacTipi.Diger]: 'adet',
}

export default function PageComponent() {
  const [items, setItems] = useState<BirimFiyat[]>([])
  const [loading, setLoading] = useState(true)
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<BirimFiyat | null>(null)
  const [form, setForm] = useState<CreateBirimFiyatDto>(emptyForm())
  const [saving, setSaving] = useState(false)
  const [deleting, setDeleting] = useState<string | null>(null)

  const load = async () => {
    setLoading(true)
    const r = await birimFiyatlarApi.getAll()
    if (r.success && r.data) setItems(r.data)
    setLoading(false)
  }

  useEffect(() => { load() }, [])

  const openCreate = () => {
    setForm(emptyForm())
    setPanelMode('create')
    setSelected(null)
    setPanelOpen(true)
  }

  const openEdit = (item: BirimFiyat) => {
    setForm({
      tip: item.tip,
      fiyat: item.fiyat,
      birim: item.birim,
      baslangicTarihi: item.baslangicTarihi.slice(0, 10),
      bitisTarihi: item.bitisTarihi?.slice(0, 10),
      aciklama: item.aciklama,
    })
    setSelected(item)
    setPanelMode('edit')
    setPanelOpen(true)
  }

  const handleSave = async () => {
    if (!form.baslangicTarihi || form.fiyat <= 0) return
    setSaving(true)
    const r = panelMode === 'create'
      ? await birimFiyatlarApi.create(form)
      : await birimFiyatlarApi.update(selected!.id, form as UpdateBirimFiyatDto)
    if (r.success) {
      showSuccess(panelMode === 'create' ? 'Birim fiyat eklendi' : 'Güncellendi')
      setPanelOpen(false)
      load()
    } else {
      showApiError(r)
    }
    setSaving(false)
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu birim fiyatı silmek istiyor musunuz?')) return
    setDeleting(id)
    const r = await birimFiyatlarApi.delete(id)
    if (r.success) {
      showSuccess('Silindi')
      load()
    } else {
      showApiError(r)
    }
    setDeleting(null)
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Teknik Parametreler</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Birim Fiyat Ekle</Button>
      </div>

      <p className="text-sm text-muted-foreground mb-4">
        Sayaç tüketim hesaplamalarında kullanılacak birim fiyatları ve geçerlilik tarihlerini yönetin.
      </p>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Sayaç Tipi</th>
              <th className="text-left px-3 py-2 font-medium">Fiyat</th>
              <th className="text-left px-3 py-2 font-medium">Birim</th>
              <th className="text-left px-3 py-2 font-medium">Başlangıç</th>
              <th className="text-left px-3 py-2 font-medium">Bitiş</th>
              <th className="text-left px-3 py-2 font-medium">Açıklama</th>
              <th className="px-3 py-2" />
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={7} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={7} className="text-center py-16 text-muted-foreground">
                  <Settings className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Henüz birim fiyat tanımlanmamış</p>
                </td>
              </tr>
            ) : items.map(item => (
              <tr key={item.id} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2">
                  <Badge variant="outline">{SayacTipiLabel[item.tip]}</Badge>
                </td>
                <td className="px-3 py-2 font-medium">{item.fiyat.toFixed(4)} ₺</td>
                <td className="px-3 py-2 text-muted-foreground">{item.birim ?? '—'}</td>
                <td className="px-3 py-2 text-muted-foreground">{new Date(item.baslangicTarihi).toLocaleDateString('tr-TR')}</td>
                <td className="px-3 py-2 text-muted-foreground">
                  {item.bitisTarihi ? new Date(item.bitisTarihi).toLocaleDateString('tr-TR') : '—'}
                </td>
                <td className="px-3 py-2 text-muted-foreground text-xs truncate max-w-xs">{item.aciklama ?? '—'}</td>
                <td className="px-3 py-2">
                  <div className="flex gap-1 justify-end">
                    <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => openEdit(item)}>
                      <Pencil className="h-3 w-3" />
                    </Button>
                    <Button
                      variant="ghost" size="icon" className="h-7 w-7 text-destructive"
                      disabled={deleting === item.id}
                      onClick={() => handleDelete(item.id)}
                    >
                      <Trash2 className="h-3 w-3" />
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed inset-0 z-50 flex">
          <div className="fixed inset-0 bg-black/40" onClick={() => setPanelOpen(false)} />
          <div className="relative ml-auto w-full max-w-sm bg-background shadow-xl flex flex-col h-full p-6 gap-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">{panelMode === 'create' ? 'Birim Fiyat Ekle' : 'Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={() => setPanelOpen(false)}><X className="h-4 w-4" /></Button>
            </div>

            <div className="space-y-4 flex-1 overflow-auto">
              <div className="space-y-1">
                <Label>Sayaç Tipi</Label>
                <select
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background"
                  value={form.tip}
                  onChange={e => {
                    const t = Number(e.target.value) as SayacTipi
                    setForm(f => ({ ...f, tip: t, birim: tipBirim[t] }))
                  }}
                >
                  {(Object.values(SayacTipi).filter(v => typeof v === 'number') as SayacTipi[]).map(t => (
                    <option key={t} value={t}>{SayacTipiLabel[t]}</option>
                  ))}
                </select>
              </div>
              <div className="space-y-1">
                <Label>Fiyat (₺) <span className="text-red-500">*</span></Label>
                <Input
                  type="number" step="0.0001" min="0"
                  value={form.fiyat}
                  onChange={e => setForm(f => ({ ...f, fiyat: parseFloat(e.target.value) || 0 }))}
                />
              </div>
              <div className="space-y-1">
                <Label>Birim</Label>
                <Input
                  value={form.birim ?? ''}
                  onChange={e => setForm(f => ({ ...f, birim: e.target.value }))}
                  placeholder="kWh, m³..."
                />
              </div>
              <div className="space-y-1">
                <Label>Başlangıç Tarihi <span className="text-red-500">*</span></Label>
                <Input
                  type="date"
                  value={form.baslangicTarihi}
                  onChange={e => setForm(f => ({ ...f, baslangicTarihi: e.target.value }))}
                />
              </div>
              <div className="space-y-1">
                <Label>Bitiş Tarihi</Label>
                <Input
                  type="date"
                  value={form.bitisTarihi ?? ''}
                  onChange={e => setForm(f => ({ ...f, bitisTarihi: e.target.value || undefined }))}
                />
              </div>
              <div className="space-y-1">
                <Label>Açıklama</Label>
                <Input
                  value={form.aciklama ?? ''}
                  onChange={e => setForm(f => ({ ...f, aciklama: e.target.value }))}
                />
              </div>
            </div>

            <div className="flex gap-2 pt-4 border-t">
              <Button className="flex-1" onClick={handleSave} disabled={saving || form.fiyat <= 0}>
                {saving ? 'Kaydediliyor...' : 'Kaydet'}
              </Button>
              <Button variant="outline" onClick={() => setPanelOpen(false)}>İptal</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
