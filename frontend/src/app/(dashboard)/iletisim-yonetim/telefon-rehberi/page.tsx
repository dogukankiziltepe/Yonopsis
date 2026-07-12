'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, Pencil, Trash2, X, Phone, AlertCircle, Search, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { telefonRehberiApi } from '@/lib/api/iletisimYonetim'
import type { TelefonRehberi, CreateTelefonRehberiDto, UpdateTelefonRehberiDto } from '@/types/iletisimYonetim'
import { showSuccess, showApiError } from '@/lib/toast'

export default function TelefonRehberiPage() {
  const [items, setItems] = useState<TelefonRehberi[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<TelefonRehberi | null>(null)

  const [formAd, setFormAd] = useState('')
  const [formUnvan, setFormUnvan] = useState('')
  const [formTelefon, setFormTelefon] = useState('')
  const [formDahili, setFormDahili] = useState('')
  const [formEmail, setFormEmail] = useState('')
  const [formDepartman, setFormDepartman] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [formErrors, setFormErrors] = useState<Record<string, string>>({})
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const res = await telefonRehberiApi.getAll(page, pageSize, search || undefined)
      setItems(res.data.items); setTotal(res.data.totalCount)
    } catch { setError('Veriler yüklenirken bir hata oluştu.') }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])

  const handleSearch = () => { setSearch(searchInput); setPage(1) }

  const openCreate = () => {
    setFormAd(''); setFormUnvan(''); setFormTelefon(''); setFormDahili(''); setFormEmail(''); setFormDepartman(''); setFormAciklama(''); setFormIsActive(true)
    setFormErrors({}); setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }

  const openEdit = (item: TelefonRehberi) => {
    setFormAd(item.ad); setFormUnvan(item.unvan ?? ''); setFormTelefon(item.telefon); setFormDahili(item.dahili ?? '');
    setFormEmail(item.email ?? ''); setFormDepartman(item.departman ?? ''); setFormAciklama(item.aciklama ?? ''); setFormIsActive(item.isActive)
    setFormErrors({}); setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const validate = () => {
    const errors: Record<string, string> = {}
    if (!formAd.trim()) errors.ad = 'Ad zorunludur.'
    if (!formTelefon.trim()) errors.telefon = 'Telefon zorunludur.'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) return
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateTelefonRehberiDto = {
          ad: formAd.trim(), unvan: formUnvan.trim() || undefined, telefon: formTelefon.trim(),
          dahili: formDahili.trim() || undefined, email: formEmail.trim() || undefined,
          departman: formDepartman.trim() || undefined, aciklama: formAciklama.trim() || undefined,
        }
        await telefonRehberiApi.create(dto)
        showSuccess('Rehber kaydı oluşturuldu.')
      } else if (selected) {
        const dto: UpdateTelefonRehberiDto = {
          ad: formAd.trim(), unvan: formUnvan.trim() || undefined, telefon: formTelefon.trim(),
          dahili: formDahili.trim() || undefined, email: formEmail.trim() || undefined,
          departman: formDepartman.trim() || undefined, aciklama: formAciklama.trim() || undefined,
          isActive: formIsActive,
        }
        await telefonRehberiApi.update(selected.id, dto)
        showSuccess('Rehber kaydı güncellendi.')
      }
      closePanel(); load()
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (deleteConfirm !== id) { setDeleteConfirm(id); return }
    try {
      await telefonRehberiApi.delete(id)
      showSuccess('Rehber kaydı silindi.')
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
              <h1 className="text-2xl font-bold tracking-tight">Telefon Rehberi</h1>
              <p className="text-sm text-muted-foreground mt-1">Site personeli ve önemli kişilerin iletişim bilgilerini yönetin.</p>
            </div>
            <Button onClick={openCreate} size="sm" className="gap-2"><Plus className="h-4 w-4" />Yeni Kayıt</Button>
          </div>

          <div className="flex gap-2 mb-4">
            <Input placeholder="Ad veya departman ara..." value={searchInput} onChange={e => setSearchInput(e.target.value)} onKeyDown={e => e.key === 'Enter' && handleSearch()} className="max-w-xs" />
            <Button variant="outline" size="sm" onClick={handleSearch}><Search className="h-4 w-4" /></Button>
          </div>

          {error && <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

          {loading ? (
            <div className="space-y-2">{Array.from({ length: 5 }).map((_, i) => <div key={i} className="h-14 rounded-lg bg-muted animate-pulse" />)}</div>
          ) : items.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-20 text-center">
              <Phone className="h-12 w-12 text-muted-foreground mb-4" />
              <p className="text-muted-foreground">Henüz telefon rehberi kaydı eklenmemiş.</p>
              <Button variant="outline" size="sm" className="mt-4 gap-2" onClick={openCreate}><Plus className="h-4 w-4" />İlk kaydı ekle</Button>
            </div>
          ) : (
            <div className="rounded-lg border overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Ad</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden md:table-cell">Unvan / Departman</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Telefon</th>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground hidden lg:table-cell">E-Posta</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Durum</th>
                    <th className="px-4 py-3 text-right font-medium text-muted-foreground w-24">İşlem</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {items.map(item => (
                    <tr key={item.id} className={`hover:bg-muted/30 cursor-pointer transition-colors ${selected?.id === item.id ? 'bg-muted/50' : ''}`} onClick={() => openEdit(item)}>
                      <td className="px-4 py-3 font-medium">{item.ad}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden md:table-cell">
                        {[item.unvan, item.departman].filter(Boolean).join(' / ') || '—'}
                      </td>
                      <td className="px-4 py-3">{item.telefon}{item.dahili && <span className="text-muted-foreground"> ({item.dahili})</span>}</td>
                      <td className="px-4 py-3 text-muted-foreground hidden lg:table-cell">{item.email || '—'}</td>
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
            <h2 className="font-semibold text-base">{panelMode === 'create' ? 'Yeni Rehber Kaydı' : 'Kaydı Düzenle'}</h2>
            <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
          </div>
          <div className="flex-1 overflow-y-auto px-6 py-5 space-y-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Ad <span className="text-destructive">*</span></Label>
              <Input value={formAd} onChange={e => setFormAd(e.target.value)} placeholder="Ad Soyad" maxLength={200} className={formErrors.ad ? 'border-destructive' : ''} />
              {formErrors.ad && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.ad}</p>}
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Unvan</Label>
                <Input value={formUnvan} onChange={e => setFormUnvan(e.target.value)} placeholder="Müdür, Güvenlik..." maxLength={100} />
              </div>
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Departman</Label>
                <Input value={formDepartman} onChange={e => setFormDepartman(e.target.value)} placeholder="Teknik, İdari..." maxLength={100} />
              </div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Telefon <span className="text-destructive">*</span></Label>
                <Input value={formTelefon} onChange={e => setFormTelefon(e.target.value)} placeholder="+90 xxx xxx xx xx" maxLength={50} className={formErrors.telefon ? 'border-destructive' : ''} />
                {formErrors.telefon && <p className="mt-1 text-xs text-destructive flex items-center gap-1"><AlertCircle className="h-3 w-3" />{formErrors.telefon}</p>}
              </div>
              <div>
                <Label className="text-xs font-medium mb-1.5 block">Dahili</Label>
                <Input value={formDahili} onChange={e => setFormDahili(e.target.value)} placeholder="101" maxLength={20} />
              </div>
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">E-Posta</Label>
              <Input type="email" value={formEmail} onChange={e => setFormEmail(e.target.value)} placeholder="ornek@site.com" maxLength={200} />
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Açıklama</Label>
              <textarea
                value={formAciklama}
                onChange={e => setFormAciklama(e.target.value)}
                placeholder="İsteğe bağlı not"
                rows={3}
                maxLength={500}
                className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring resize-y"
              />
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
