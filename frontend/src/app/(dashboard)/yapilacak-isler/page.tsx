'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, ClipboardList, AlertCircle, Search, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { yapilacakIslerApi } from '@/lib/api/siteYonetim'
import type { YapilacakIs, CreateYapilacakIsDto, UpdateYapilacakIsDto } from '@/types/siteYonetim'
import { YapilacakIsDurum, YapilacakIsDurumLabel, YapilacakIsOncelik, YapilacakIsOncelikLabel } from '@/types/siteYonetim'
import { showSuccess, showApiError } from '@/lib/toast'

const DURUM_OPTIONS = [YapilacakIsDurum.Beklemede, YapilacakIsDurum.Devam, YapilacakIsDurum.Tamamlandi]
const ONCELIK_OPTIONS = [YapilacakIsOncelik.Dusuk, YapilacakIsOncelik.Normal, YapilacakIsOncelik.Yuksek]

export default function YapilacakIslerPage() {
  const [items, setItems] = useState<YapilacakIs[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [durumFilter, setDurumFilter] = useState<YapilacakIsDurum | ''>('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<YapilacakIs | null>(null)

  const [formBaslik, setFormBaslik] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [formAtananKisi, setFormAtananKisi] = useState('')
  const [formOncelik, setFormOncelik] = useState<YapilacakIsOncelik>(YapilacakIsOncelik.Normal)
  const [formTamamlanmaTarihi, setFormTamamlanmaTarihi] = useState('')
  const [formDurum, setFormDurum] = useState<YapilacakIsDurum>(YapilacakIsDurum.Beklemede)
  const [formIsActive, setFormIsActive] = useState(true)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const res = await yapilacakIslerApi.getAll(page, pageSize, search || undefined, durumFilter !== '' ? durumFilter : undefined)
      setItems(res.data.items); setTotal(res.data.totalCount)
    } catch { setError('Veriler yuklenirken bir hata olustu.') }
    finally { setLoading(false) }
  }, [page, search, durumFilter])

  useEffect(() => { load() }, [load])

  const handleSearch = () => { setSearch(searchInput); setPage(1) }

  const openCreate = () => {
    setFormBaslik(''); setFormAciklama(''); setFormAtananKisi(''); setFormOncelik(YapilacakIsOncelik.Normal);
    setFormTamamlanmaTarihi(''); setFormDurum(YapilacakIsDurum.Beklemede); setFormIsActive(true)
    setFormErrors({}); setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: YapilacakIs) => {
    setFormBaslik(item.baslik); setFormAciklama(item.aciklama ?? ''); setFormAtananKisi(item.atananKisi ?? '');
    setFormOncelik(item.oncelik); setFormTamamlanmaTarihi(item.tamamlanmaTarihi ? item.tamamlanmaTarihi.slice(0, 10) : '');
    setFormDurum(item.durum); setFormIsActive(item.isActive)
    setFormErrors({}); setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const validate = () => {
    const errors: Record<string, string> = {}
    if (!formBaslik.trim()) errors.baslik = 'Baslik zorunludur.'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) return
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateYapilacakIsDto = {
          baslik: formBaslik.trim(), aciklama: formAciklama.trim() || undefined,
          atananKisi: formAtananKisi.trim() || undefined, oncelik: formOncelik,
          tamamlanmaTarihi: formTamamlanmaTarihi || undefined, durum: formDurum,
        }
        await yapilacakIslerApi.create(dto)
        showSuccess('Gorev olusturuldu.')
      } else if (selected) {
        const dto: UpdateYapilacakIsDto = {
          baslik: formBaslik.trim(), aciklama: formAciklama.trim() || undefined,
          atananKisi: formAtananKisi.trim() || undefined, oncelik: formOncelik,
          tamamlanmaTarihi: formTamamlanmaTarihi || undefined, durum: formDurum, isActive: formIsActive,
        }
        await yapilacakIslerApi.update(selected.id, dto)
        showSuccess('Gorev guncellendi.')
      }
      closePanel(); load()
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (deleteConfirm !== id) { setDeleteConfirm(id); return }
    try {
      await yapilacakIslerApi.delete(id)
      showSuccess('Gorev silindi.')
      if (selected?.id === id) closePanel()
      load()
    } catch { showApiError() }
    finally { setDeleteConfirm(null) }
  }

  const totalPages = Math.ceil(total / pageSize)

  const durumColors: Record<YapilacakIsDurum, string> = {
    [YapilacakIsDurum.Beklemede]:  'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400',
    [YapilacakIsDurum.Devam]:      'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400',
    [YapilacakIsDurum.Tamamlandi]: 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400',
  }

  const oncelikColors: Record<YapilacakIsOncelik, string> = {
    [YapilacakIsOncelik.Dusuk]:  'text-muted-foreground',
    [YapilacakIsOncelik.Normal]: 'text-blue-600',
    [YapilacakIsOncelik.Yuksek]: 'text-red-600 font-semibold',
  }

  return (
    <div className="flex h-full">
      <div className={`flex-1 min-w-0 transition-all duration-300 ${panelOpen ? 'pr-[460px]' : ''}`}>
        <div className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold tracking-tight">Yapilacak Isler</h1>
              <p className="text-sm text-muted-foreground mt-1">Gorev ve yapilacaklar listesini yonetin.</p>
            </div>
            <Button onClick={openCreate} size="sm" className="gap-2"><Plus className="h-4 w-4" />Yeni Gorev</Button>
          </div>

          <div className="flex gap-2 mb-4 flex-wrap">
            <Input placeholder="Baslik ara..." value={searchInput} onChange={e => setSearchInput(e.target.value)} onKeyDown={e => e.key === 'Enter' && handleSearch()} className="max-w-xs" />
            <Button variant="outline" size="sm" onClick={handleSearch}><Search className="h-4 w-4" /></Button>
            <select value={durumFilter} onChange={e => { setDurumFilter(e.target.value === '' ? '' : Number(e.target.value) as YapilacakIsDurum); setPage(1) }}
              className="rounded-md border border-input bg-transparent px-3 py-1.5 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
              <option value="">Tum Durumlar</option>
              {DURUM_OPTIONS.map(d => <option key={d} value={d}>{YapilacakIsDurumLabel[d]}</option>)}
            </select>
          </div>

          {error && <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

          {loading ? (
            <div className="space-y-2">{Array.from({ length: 5 }).map((_, i) => <div key={i} className="h-14 rounded-lg bg-muted animate-pulse" />)}</div>
          ) : items.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-20 text-center">
              <ClipboardList className="h-12 w-12 text-muted-foreground mb-4" />
              <p className="text-muted-foreground">Henuz gorev eklenmemis.</p>
              <Button variant="outline" size="sm" className="mt-4 gap-2" onClick={openCreate}><Plus className="h-4 w-4" />Ilk gorevi ekle</Button>
            </div>
          ) : (
            <div className="rounded-lg border overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Baslik</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden md:table-cell">Atanan</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Oncelik</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-28">Durum</th>
                    <th className="px-4 py-3 text-right font-medium text-muted-foreground w-24">Islem</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {items.map(item => (
                    <tr key={item.id} className={`hover:bg-muted/30 cursor-pointer transition-colors ${selected?.id === item.id ? 'bg-muted/50' : ''}`} onClick={() => openEdit(item)}>
                      <td className="px-4 py-3 font-medium">{item.baslik}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">{item.atananKisi || '—'}</td>
                      <td className={`px-4 py-3 text-center text-xs ${oncelikColors[item.oncelik]}`}>{YapilacakIsOncelikLabel[item.oncelik]}</td>
                      <td className="px-4 py-3 text-center">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${durumColors[item.durum]}`}>{YapilacakIsDurumLabel[item.durum]}</span>
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
              <span>{total} kayit</span>
              <div className="flex items-center gap-2">
                <Button variant="outline" size="icon" className="h-7 w-7" disabled={page === 1} onClick={() => setPage(p => p - 1)}><ChevronLeft className="h-4 w-4" /></Button>
                <span>{page} / {totalPages}</span>
                <Button variant="outline" size="icon" className="h-7 w-7" disabled={page === totalPages} onClick={() => setPage(p => p + 1)}><ChevronRight className="h-4 w-4" /></Button>
              </div>
            </div>
          )}

          {deleteConfirm && (
            <div className="mt-3 p-3 rounded-lg bg-destructive/10 text-destructive text-sm flex items-center justify-between">
              <span>Silmek istediginizden emin misiniz? Tekrar tiklayin.</span>
              <Button variant="ghost" size="sm" onClick={() => setDeleteConfirm(null)}>Iptal</Button>
            </div>
          )}
        </div>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-6 py-4 border-b shrink-0">
            <h2 className="font-semibold text-base">{panelMode === 'create' ? 'Yeni Gorev' : 'Gorevi Duzenle'}</h2>
            <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
          </div>
          <div className="flex-1 overflow-y-auto px-6 py-5 space-y-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Baslik <span className="text-destructive">*</span></Label>
              <Input value={formBaslik} onChange={e => setFormBaslik(e.target.value)} placeholder="Gorev basligi" maxLength={300} className={formErrors.baslik ? 'border-destructive' : ''} />
              {formErrors.baslik && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.baslik}</p>}
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Oncelik</Label>
                <select value={formOncelik} onChange={e => setFormOncelik(Number(e.target.value) as YapilacakIsOncelik)}
                  className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
                  {ONCELIK_OPTIONS.map(o => <option key={o} value={o}>{YapilacakIsOncelikLabel[o]}</option>)}
                </select>
              </div>
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Durum</Label>
                <select value={formDurum} onChange={e => setFormDurum(Number(e.target.value) as YapilacakIsDurum)}
                  className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
                  {DURUM_OPTIONS.map(d => <option key={d} value={d}>{YapilacakIsDurumLabel[d]}</option>)}
                </select>
              </div>
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Atanan Kisi</Label>
              <Input value={formAtananKisi} onChange={e => setFormAtananKisi(e.target.value)} placeholder="Gorevli adi" maxLength={200} />
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Tamamlanma Tarihi</Label>
              <Input type="date" value={formTamamlanmaTarihi} onChange={e => setFormTamamlanmaTarihi(e.target.value)} />
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Aciklama</Label>
              <textarea value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Gorev detaylari" rows={4} maxLength={2000}
                className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring resize-y" />
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
