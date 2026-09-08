'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, FileText, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { devirBakiyeleriApi } from '@/lib/api/finans'
import { unitsApi } from '@/lib/api/units'
import type { DevirBakiye, CreateDevirBakiyeDto, UpdateDevirBakiyeDto } from '@/types/finans'
import type { UnitSummary } from '@/types/unit'
import type { UserType } from '@/types/person'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function DevirBakiyePage() {
  const [items, setItems] = useState<DevirBakiye[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [units, setUnits] = useState<UnitSummary[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<DevirBakiye | null>(null)

  const [formTarih, setFormTarih] = useState('')
  const [formUnitId, setFormUnitId] = useState('')
  const [formBorcluUserId, setFormBorcluUserId] = useState('')
  const [formBorcluAdSoyad, setFormBorcluAdSoyad] = useState('')
  const [formBorcluRol, setFormBorcluRol] = useState<UserType | undefined>(undefined)
  const [formTutar, setFormTutar] = useState('')
  const [formAciklama, setFormAciklama] = useState('Devir')
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await devirBakiyeleriApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { unitsApi.getAll().then(r => setUnits(r.data)) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const handleUnitChange = async (unitId: string) => {
    setFormUnitId(unitId)
    setFormBorcluUserId(''); setFormBorcluAdSoyad(''); setFormBorcluRol(undefined)
    if (!unitId) return
    try {
      const res = await unitsApi.getBorcluOnerisi(unitId)
      const d = res.data
      if (d.onerilenPersonId && d.onerilenAdSoyad) {
        setFormBorcluUserId(d.onerilenPersonId); setFormBorcluAdSoyad(d.onerilenAdSoyad); setFormBorcluRol(d.onerilenRol)
      }
    } catch (e) { showApiError(e) }
  }

  const openCreate = () => {
    setFormTarih(''); setFormUnitId(''); setFormBorcluUserId(''); setFormBorcluAdSoyad(''); setFormBorcluRol(undefined)
    setFormTutar(''); setFormAciklama('Devir')
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }
  const openEdit = (item: DevirBakiye) => {
    setFormTarih(item.tarih.split('T')[0]); setFormUnitId(item.unitId ?? '')
    setFormBorcluUserId(item.borcluUserId ?? ''); setFormBorcluAdSoyad(item.borcluAdSoyad ?? ''); setFormBorcluRol(item.borcluRol)
    setFormTutar(String(item.tutar)); setFormAciklama(item.aciklama ?? 'Devir')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formTarih) { showApiError('Tarih zorunludur.'); return }
    if (!formTutar || parseFloat(formTutar) <= 0) { showApiError('Tutar sıfırdan büyük olmalıdır.'); return }
    setSaving(true)
    try {
      const dto: CreateDevirBakiyeDto | UpdateDevirBakiyeDto = {
        tarih: formTarih, unitId: formUnitId || undefined, borcluUserId: formBorcluUserId || undefined,
        borcluRol: formBorcluRol, tutar: parseFloat(formTutar), aciklama: formAciklama || undefined,
      }
      if (panelMode === 'create') {
        await devirBakiyeleriApi.create(dto); showSuccess('Devir bakiye kaydı oluşturuldu.')
      } else if (selected) {
        await devirBakiyeleriApi.update(selected.id, dto); showSuccess('Devir bakiye kaydı güncellendi.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await devirBakiyeleriApi.delete(id); showSuccess('Silindi.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Devir Bakiye Girişi</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Devir Kaydı</Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, borçlu, daire ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><FileText className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Devir bakiye kaydı bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Daire</th>
                <th className="text-left px-3 py-2 font-medium">Borçlu</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Açıklama</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.tarih).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5">{item.unitDoorNumber ?? '—'}</td>
                  <td className="px-3 py-2.5">{item.borcluAdSoyad ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.tutar)}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell text-xs">{item.aciklama ?? '—'}</td>
                  <td className="px-3 py-2.5">
                    <div className="flex gap-1 justify-end">
                      <Button variant="ghost" size="sm" className="h-6 px-2 text-xs" onClick={() => openEdit(item)}>Düzenle</Button>
                      {deleteConfirm === item.id
                        ? <Button variant="destructive" size="sm" className="h-6 px-2 text-xs" onClick={() => handleDelete(item.id)}>Onayla</Button>
                        : <Button variant="ghost" size="sm" className="h-6 px-2 text-xs text-destructive" onClick={() => setDeleteConfirm(item.id)}>Sil</Button>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-end gap-2 text-sm">
          <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>Önceki</Button>
          <span className="text-muted-foreground">{page} / {totalPages}</span>
          <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Sonraki</Button>
        </div>
      )}

      {panelOpen && (
        <div className="fixed inset-0 z-40 flex">
          <div className="flex-1 bg-black/30" onClick={closePanel} />
          <div className="w-full max-w-sm bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">{panelMode === 'create' ? 'Yeni Devir Kaydı' : 'Devir Kaydı Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="tarih">Tarih <span className="text-destructive">*</span></Label>
                <Input id="tarih" type="date" value={formTarih} onChange={e => setFormTarih(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="unit">Daire</Label>
                <select id="unit" value={formUnitId} onChange={e => handleUnitChange(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Daire seçilmedi —</option>
                  {units.map(u => <option key={u.id} value={u.id}>{u.buildingName ? `${u.buildingName} / ` : ''}{u.doorNumber}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label>Borçlu</Label>
                <Input value={formBorcluAdSoyad} readOnly placeholder="Daire seçince otomatik doldurulur" className="bg-muted/30" />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Tutar (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" min={0} step="0.01" value={formTutar} onChange={e => setFormTutar(e.target.value)} placeholder="0.00" />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Açıklama</Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} maxLength={500} />
              </div>
            </div>
            <div className="border-t px-4 py-3 flex gap-2 justify-end">
              <Button variant="outline" onClick={closePanel}>İptal</Button>
              <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
