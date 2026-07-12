'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Bell, AlertCircle, Search, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { mobilBildirimSablonlariApi } from '@/lib/api/iletisimYonetim'
import type { MobilBildirimSablonu, CreateMobilBildirimSablonuDto, UpdateMobilBildirimSablonuDto } from '@/types/iletisimYonetim'
import { showSuccess, showApiError } from '@/lib/toast'

export default function MobilBildirimSablonlariPage() {
  const [items, setItems] = useState<MobilBildirimSablonu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<MobilBildirimSablonu | null>(null)

  const [formAd, setFormAd] = useState('')
  const [formBaslik, setFormBaslik] = useState('')
  const [formIcerik, setFormIcerik] = useState('')
  const [formKategori, setFormKategori] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const res = await mobilBildirimSablonlariApi.getAll(page, pageSize, search || undefined)
      setItems(res.data.items); setTotal(res.data.totalCount)
    } catch { setError('Veriler yüklenirken bir hata oluştu.') }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])

  const handleSearch = () => { setSearch(searchInput); setPage(1) }

  const openCreate = () => {
    setFormAd(''); setFormBaslik(''); setFormIcerik(''); setFormKategori(''); setFormIsActive(true)
    setFormErrors({}); setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: MobilBildirimSablonu) => {
    setFormAd(item.ad); setFormBaslik(item.baslik); setFormIcerik(item.icerik); setFormKategori(item.kategori ?? ''); setFormIsActive(item.isActive)
    setFormErrors({}); setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const validate = () => {
    const errors: Record<string, string> = {}
    if (!formAd.trim()) errors.ad = 'Ad zorunludur.'
    if (!formBaslik.trim()) errors.baslik = 'Başlık zorunludur.'
    if (!formIcerik.trim()) errors.icerik = 'İçerik zorunludur.'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) return
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateMobilBildirimSablonuDto = { ad: formAd.trim(), baslik: formBaslik.trim(), icerik: formIcerik.trim(), kategori: formKategori.trim() || undefined }
        await mobilBildirimSablonlariApi.create(dto)
        showSuccess('Mobil bildirim şablonu oluşturuldu.')
      } else if (selected) {
        const dto: UpdateMobilBildirimSablonuDto = { ad: formAd.trim(), baslik: formBaslik.trim(), icerik: formIcerik.trim(), kategori: formKategori.trim() || undefined, isActive: formIsActive }
        await mobilBildirimSablonlariApi.update(selected.id, dto)
        showSuccess('Mobil bildirim şablonu güncellendi.')
      }
      closePanel(); load()
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (deleteConfirm !== id) { setDeleteConfirm(id); return }
    try {
      await mobilBildirimSablonlariApi.delete(id)
      showSuccess('Mobil bildirim şablonu silindi.')
      if (selected?.id === id) closePanel()
      load()
    } catch { showApiError() }
    finally { setDeleteConfirm(null) }
  }

  const totalPages = Math.ceil(total / pageSize)

  return (
    <div className="flex h-full">
      <div className={`flex-1 min-w-0 transition-all duration-300 ${panelOpen ? 'pr-[460px]' : ''}`}>
        <div className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold tracking-tight">Mobil Bildirim Şablonları</h1>
              <p className="text-sm text-muted-foreground mt-1">Push bildirimlerinde kullanılacak şablonları yönetin.</p>
            </div>
            <Button onClick={openCreate} size="sm" className="gap-2"><Plus className="h-4 w-4" />Yeni Şablon</Button>
          </div>

          <div className="flex gap-2 mb-4">
            <Input placeholder="Şablon adı ara..." value={searchInput} onChange={e => setSearchInput(e.target.value)} onKeyDown={e => e.key === 'Enter' && handleSearch()} className="max-w-xs" />
            <Button variant="outline" size="sm" onClick={handleSearch}><Search className="h-4 w-4" /></Button>
          </div>

          {error && <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

          {loading ? (
            <div className="space-y-2">{Array.from({ length: 5 }).map((_, i) => <div key={i} className="h-14 rounded-lg bg-muted animate-pulse" />)}</div>
          ) : items.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-20 text-center">
              <Bell className="h-12 w-12 text-muted-foreground mb-4" />
              <p className="text-muted-foreground">Henüz mobil bildirim şablonu eklenmemiş.</p>
              <Button variant="outline" size="sm" className="mt-4 gap-2" onClick={openCreate}><Plus className="h-4 w-4" />İlk şablonu ekle</Button>
            </div>
          ) : (
            <div className="rounded-lg border overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Ad</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden md:table-cell">Başlık</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden lg:table-cell">İçerik</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Durum</th>
                    <th className="px-4 py-3 text-right font-medium text-muted-foreground w-24">İşlem</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {items.map(item => (
                    <tr key={item.id} className={`hover:bg-muted/30 cursor-pointer transition-colors ${selected?.id === item.id ? 'bg-muted/50' : ''}`} onClick={() => openEdit(item)}>
                      <td className="px-4 py-3 font-medium">{item.ad}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.baslik}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell truncate max-w-[200px]">{item.icerik}</td>
                      <td className="px-4 py-3 text-center">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' : 'bg-muted text-muted-foreground'}`}>
                          {item.isActive ? 'Aktif' : 'Pasif'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                        <div className="flex items-center justify-end gap-1">
                          <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => openEdit(item)}><Pencil className="h-3.5 w-3.5" /></Button>
                          <Button variant="ghost" size="icon" className={`h-7 w-7 ${deleteConfirm === item.id ? 'text-destructive bg-destructive/10' : ''}`} onClick={() => handleDelete(item.id)}><Trash2 className="h-3.5 w-3.5" /></Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {totalPages > 1 && (
            <div className="flex items-center justify-between mt-4 text-sm text-muted-foreground">
              <span>{total} kayıt</span>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="icon" className="h-7 w-7" disabled={page === 1} onClick={() => setPage(p => p - 1)}><ChevronLeft className="h-4 w-4" /></Button>
                <span>{page} / {totalPages}</span>
                <Button variant="outline" size="icon" className="h-7 w-7" disabled={page === totalPages} onClick={() => setPage(p => p + 1)}><ChevronRight className="h-4 w-4" /></Button>
              </div>
            </div>
          )}

          {deleteConfirm && (
            <div className="mt-3 p-3 rounded-lg bg-destructive/10 text-destructive text-sm flex items-center justify-between">
              <span>Silmek istediğinizden emin misiniz? Tekrar tıklayın.</span>
              <Button variant="ghost" size="sm" onClick={() => setDeleteConfirm(null)}>İptal</Button>
            </div>
          )}
        </div>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-6 py-4 border-b shrink-0">
            <h2 className="font-semibold text-base">{panelMode === 'create' ? 'Yeni Mobil Bildirim Şablonu' : 'Şablonu Düzenle'}</h2>
            <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
          </div>
          <div className="flex-1 overflow-y-auto px-6 py-5 space-y-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Ad <span className="text-destructive">*</span></Label>
              <Input value={formAd} onChange={e => setFormAd(e.target.value)} placeholder="Şablon adı" maxLength={200} className={formErrors.ad ? 'border-destructive' : ''} />
              {formErrors.ad && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.ad}</p>}
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Bildirim Başlığı <span className="text-destructive">*</span></Label>
              <Input value={formBaslik} onChange={e => setFormBaslik(e.target.value)} placeholder="Bildirimin başlığı" maxLength={200} className={formErrors.baslik ? 'border-destructive' : ''} />
              {formErrors.baslik && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.baslik}</p>}
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">İçerik <span className="text-destructive">*</span></Label>
              <textarea
                value={formIcerik}
                onChange={e => setFormIcerik(e.target.value)}
                placeholder="Bildirim içeriği (max 500 karakter)"
                rows={4}
                maxLength={500}
                className={`w-full rounded-md border bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring resize-y ${formErrors.icerik ? 'border-destructive' : 'border-input'}`}
              />
              <p className="mt-1 text-xs text-muted-foreground text-right">{formIcerik.length}/500</p>
              {formErrors.icerik && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.icerik}</p>}
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Kategori</Label>
              <Input value={formKategori} onChange={e => setFormKategori(e.target.value)} placeholder="örn. Aidat, Güvenlik" maxLength={100} />
            </div>
            {panelMode === 'edit' && (
              <div className="flex items-center gap-3 pt-1">
                <button type="button" onClick={() => setFormIsActive(!formIsActive)} className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${formIsActive ? 'bg-primary' : 'bg-muted-foreground/30'}`}>
                  <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${formIsActive ? 'translate-x-4' : 'translate-x-0'}`} />
                </button>
                <Label className="text-sm cursor-pointer" onClick={() => setFormIsActive(!formIsActive)}>{formIsActive ? 'Aktif' : 'Pasif'}</Label>
              </div>
            )}
          </div>
          <div className="px-6 py-4 border-t shrink-0 flex gap-3">
            <Button onClick={handleSave} disabled={saving} className="flex-1">{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
            {panelMode === 'edit' && selected && (
              <Button variant="destructive" size="icon" onClick={() => handleDelete(selected.id)} disabled={saving}><Trash2 className="h-4 w-4" /></Button>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
