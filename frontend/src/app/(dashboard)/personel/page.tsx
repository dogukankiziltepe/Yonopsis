'use client'

import { useCallback, useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Plus, Trash2, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { showError, showSuccess } from '@/lib/toast'
import { personelApi } from '@/lib/api/personel'
import type { Personel, CreatePersonelDto } from '@/types/personel'
import type { PaginatedResult } from '@/types/api'

const EMPTY_FORM: CreatePersonelDto = {
  personelKodu: '',
  name: '',
  firma: '',
  title: '',
  email: '',
  startDate: null,
}

export default function PersonelPage() {
  const router = useRouter()
  const [data, setData] = useState<PaginatedResult<Personel> | null>(null)
  const [loading, setLoading] = useState(false)
  const [search, setSearch] = useState('')
  const [filterActive, setFilterActive] = useState<'' | 'true' | 'false'>('')
  const [page, setPage] = useState(1)

  const [open, setOpen] = useState(false)
  const [form, setForm] = useState<CreatePersonelDto>(EMPTY_FORM)
  const [saving, setSaving] = useState(false)

  const load = useCallback(() => {
    setLoading(true)
    personelApi.getAll({
      search: search || undefined,
      isActive: filterActive === '' ? undefined : filterActive === 'true',
      page,
      pageSize: 50,
    })
      .then((r) => setData(r.data))
      .catch(() => showError('Personel listesi yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [search, filterActive, page])

  useEffect(() => { load() }, [load])

  const openCreate = () => {
    setForm(EMPTY_FORM)
    setOpen(true)
  }

  const f = (key: keyof CreatePersonelDto, val: unknown) =>
    setForm((prev) => ({ ...prev, [key]: val }))

  const handleSave = async () => {
    if (!form.name.trim()) { showError('Ad Soyad alanı zorunludur.'); return }
    if (!form.personelKodu.trim()) { showError('Personel Kodu alanı zorunludur.'); return }
    if (!form.title.trim()) { showError('Görevi alanı zorunludur.'); return }
    setSaving(true)
    try {
      const res = await personelApi.create(form)
      showSuccess('Personel eklendi.')
      setOpen(false)
      router.push(`/personel/${res.data.id}`)
    } catch {
      showError('İşlem başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu personeli silmek istiyor musunuz?')) return
    try {
      await personelApi.delete(id)
      showSuccess('Personel silindi.')
      load()
    } catch {
      showError('Silme işlemi başarısız.')
    }
  }

  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / 50)) : 1

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Personel</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" /> Yeni Personel
        </Button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1 flex-1 min-w-[180px]">
          <Label className="text-xs">Ara</Label>
          <div className="relative">
            <Search className="absolute left-2 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && load()}
              placeholder="Ad, kod, görev, e-posta..."
              className="h-8 pl-7"
            />
          </div>
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Durum</Label>
          <select
            value={filterActive}
            onChange={(e) => setFilterActive(e.target.value as '' | 'true' | 'false')}
            className="h-8 rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Tümü</option>
            <option value="true">Aktif</option>
            <option value="false">Pasif</option>
          </select>
        </div>
        <Button size="sm" variant="outline" onClick={() => { setPage(1); load() }}>Ara</Button>
      </div>

      {/* Table */}
      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : (
        <div className="border rounded-lg overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Personel Kodu</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Görevi</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Adı Soyadı</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Firma</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Tc Kimlik No</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Telefon</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">E-Posta</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Doğum Tarihi</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Personel Açıklama</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Giriş Tarihi</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Çıkış Tarihi</th>
                <th className="text-left px-3 py-2 font-medium text-muted-foreground">Aktif</th>
                <th className="text-right px-3 py-2 font-medium text-muted-foreground">İşlem</th>
              </tr>
            </thead>
            <tbody>
              {!data || data.items.length === 0 ? (
                <tr><td colSpan={13} className="text-center py-12 text-muted-foreground">Personel kaydı yok.</td></tr>
              ) : data.items.map((p) => (
                <tr
                  key={p.id}
                  className="border-b last:border-0 hover:bg-muted/20 cursor-pointer"
                  onClick={() => router.push(`/personel/${p.id}`)}
                >
                  <td className="px-3 py-2 font-mono text-xs">{p.personelKodu}</td>
                  <td className="px-3 py-2">{p.title ?? '—'}</td>
                  <td className="px-3 py-2 font-medium">{p.name}</td>
                  <td className="px-3 py-2 text-muted-foreground">{p.firma ?? '—'}</td>
                  <td className="px-3 py-2 font-mono text-xs">{p.tcKimlikNo ?? '—'}</td>
                  <td className="px-3 py-2 font-mono text-xs">{p.phone ?? '—'}</td>
                  <td className="px-3 py-2 text-xs">{p.email ?? '—'}</td>
                  <td className="px-3 py-2">{p.dogumTarihi ?? '—'}</td>
                  <td className="px-3 py-2 text-muted-foreground max-w-[200px] truncate">{p.aciklama ?? '—'}</td>
                  <td className="px-3 py-2">{p.startDate ?? '—'}</td>
                  <td className="px-3 py-2">{p.cikisTarihi ?? '—'}</td>
                  <td className="px-3 py-2">
                    <Badge variant={p.isActive ? 'default' : 'secondary'}>
                      {p.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </td>
                  <td className="px-3 py-2 text-right" onClick={(e) => e.stopPropagation()}>
                    <Button size="sm" variant="ghost" className="text-destructive" onClick={() => handleDelete(p.id)}>
                      <Trash2 className="h-3.5 w-3.5" />
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-3 text-sm">
          <span className="text-muted-foreground">Sayfa {page} / {totalPages} &nbsp;·&nbsp; {data?.totalCount ?? 0} kayıt</span>
          <div className="flex gap-1">
            <Button size="sm" variant="outline" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>Önceki</Button>
            <Button size="sm" variant="outline" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>Sonraki</Button>
          </div>
        </div>
      )}

      {/* Create Dialog */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Yeni Personel</DialogTitle>
          </DialogHeader>
          <div className="space-y-3 py-2">
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Personel Kodu *</Label>
                <Input value={form.personelKodu} onChange={(e) => f('personelKodu', e.target.value)} placeholder="335.001 / P0002" />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">Görevi *</Label>
                <Input value={form.title} onChange={(e) => f('title', e.target.value)} placeholder="Güvenlik, Temizlik..." />
              </div>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">Ad Soyad *</Label>
              <Input value={form.name} onChange={(e) => f('name', e.target.value)} placeholder="Ad Soyad" />
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="space-y-1">
                <Label className="text-xs">Firma</Label>
                <Input value={form.firma ?? ''} onChange={(e) => f('firma', e.target.value)} placeholder="Yönetim firması" />
              </div>
              <div className="space-y-1">
                <Label className="text-xs">E-posta</Label>
                <Input type="email" value={form.email ?? ''} onChange={(e) => f('email', e.target.value)} placeholder="ornek@email.com" />
              </div>
            </div>
            <div className="space-y-1">
              <Label className="text-xs">İşe Giriş Tarihi</Label>
              <Input type="date" value={form.startDate ?? ''} onChange={(e) => f('startDate', e.target.value || null)} />
            </div>
            <p className="text-xs text-muted-foreground">Diğer detaylar (kimlik, banka, muhasebe entegrasyonu vb.) kayıttan sonra detay sayfasından girilebilir.</p>
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
