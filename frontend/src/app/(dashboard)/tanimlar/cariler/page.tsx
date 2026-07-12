'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Search, Users } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { muhasebeApi } from '@/lib/api/muhasebe'
import type { HesapListItem, CreateCariHesapDto } from '@/types/muhasebe'
import { CariTuru, cariTuruLabel } from '@/types/muhasebe'
import { showSuccess, showApiError } from '@/lib/toast'

export default function PageComponent() {
  const [items, setItems] = useState<HesapListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [tipFilter, setTipFilter] = useState<CariTuru | ''>('')
  const [panelOpen, setPanelOpen] = useState(false)
  const [form, setForm] = useState<CreateCariHesapDto>({ cariTuru: CariTuru.Diger, hesapAdi: '' })
  const [saving, setSaving] = useState(false)

  const load = useCallback(async (q: string, tip: CariTuru | '') => {
    setLoading(true)
    const r = await muhasebeApi.getCariHesaplar({ search: q || undefined, cariTuru: tip !== '' ? tip : undefined })
    if (r.success && r.data) setItems(r.data)
    setLoading(false)
  }, [])

  useEffect(() => {
    load(search, tipFilter)
  }, [search, tipFilter, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
  }

  const openCreate = () => {
    setForm({ cariTuru: CariTuru.Diger, hesapAdi: '' })
    setPanelOpen(true)
  }

  const handleSave = async () => {
    if (!form.hesapAdi.trim()) return
    setSaving(true)
    const r = await muhasebeApi.createCariHesap(form)
    if (r.success) {
      showSuccess('Cari hesap oluşturuldu')
      setPanelOpen(false)
      load(search, tipFilter)
    } else {
      showApiError(r)
    }
    setSaving(false)
  }

  const cariTuruVariant: Record<CariTuru, 'default' | 'secondary' | 'outline'> = {
    [CariTuru.Kiraci]: 'default',
    [CariTuru.EvSahibi]: 'secondary',
    [CariTuru.Tedarikci]: 'outline',
    [CariTuru.Personel]: 'outline',
    [CariTuru.Diger]: 'secondary',
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Cariler</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Cari</Button>
      </div>

      <div className="flex gap-2 mb-4">
        <form onSubmit={handleSearch} className="flex gap-2">
          <Input
            placeholder="Hesap adı veya kodu ara..."
            value={inputVal}
            onChange={e => setInputVal(e.target.value)}
            className="w-56"
          />
          <Button type="submit" variant="outline" size="sm"><Search className="h-4 w-4" /></Button>
        </form>
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={tipFilter}
          onChange={e => setTipFilter(e.target.value === '' ? '' : Number(e.target.value) as CariTuru)}
        >
          <option value="">Tüm Tipler</option>
          {(Object.values(CariTuru).filter(v => typeof v === 'number') as CariTuru[]).map(t => (
            <option key={t} value={t}>{cariTuruLabel[t]}</option>
          ))}
        </select>
      </div>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Hesap Kodu</th>
              <th className="text-left px-3 py-2 font-medium">Hesap Adı</th>
              <th className="text-left px-3 py-2 font-medium">Cari Türü</th>
              <th className="text-left px-3 py-2 font-medium">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={4} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={4} className="text-center py-16 text-muted-foreground">
                  <Users className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Cari hesap bulunamadı</p>
                </td>
              </tr>
            ) : items.map(item => (
              <tr key={item.id} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2 font-mono text-xs">{item.hesapKodu}</td>
                <td className="px-3 py-2 font-medium">{item.hesapAdi}</td>
                <td className="px-3 py-2">
                  {item.cariTuru != null && (
                    <Badge variant={cariTuruVariant[item.cariTuru]}>{cariTuruLabel[item.cariTuru]}</Badge>
                  )}
                </td>
                <td className="px-3 py-2">
                  <Badge variant={item.aktifMi ? 'default' : 'secondary'}>
                    {item.aktifMi ? 'Aktif' : 'Pasif'}
                  </Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Create panel */}
      {panelOpen && (
        <div className="fixed inset-0 z-50 flex">
          <div className="fixed inset-0 bg-black/40" onClick={() => setPanelOpen(false)} />
          <div className="relative ml-auto w-full max-w-sm bg-background shadow-xl flex flex-col h-full p-6 gap-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">Yeni Cari Hesap</h2>
              <Button variant="ghost" size="icon" onClick={() => setPanelOpen(false)}><X className="h-4 w-4" /></Button>
            </div>

            <div className="space-y-4 flex-1">
              <div className="space-y-1">
                <Label>Hesap Adı <span className="text-red-500">*</span></Label>
                <Input
                  value={form.hesapAdi}
                  onChange={e => setForm(f => ({ ...f, hesapAdi: e.target.value }))}
                  placeholder="Hesap adı"
                />
              </div>
              <div className="space-y-1">
                <Label>Cari Türü</Label>
                <select
                  className="w-full border rounded-md px-3 py-2 text-sm bg-background"
                  value={form.cariTuru}
                  onChange={e => setForm(f => ({ ...f, cariTuru: Number(e.target.value) as CariTuru }))}
                >
                  {(Object.values(CariTuru).filter(v => typeof v === 'number') as CariTuru[]).map(t => (
                    <option key={t} value={t}>{cariTuruLabel[t]}</option>
                  ))}
                </select>
              </div>
              <div className="space-y-1">
                <Label>Açıklama</Label>
                <Input
                  value={form.aciklama ?? ''}
                  onChange={e => setForm(f => ({ ...f, aciklama: e.target.value }))}
                  placeholder="İsteğe bağlı"
                />
              </div>
            </div>

            <div className="flex gap-2 pt-4 border-t">
              <Button className="flex-1" onClick={handleSave} disabled={saving || !form.hesapAdi.trim()}>
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
