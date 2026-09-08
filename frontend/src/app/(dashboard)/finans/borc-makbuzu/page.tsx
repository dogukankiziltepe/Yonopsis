'use client'

import { useEffect, useState, useCallback } from 'react'
import Link from 'next/link'
import { Plus, X, FileText, Search, Upload } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { borcMakbuzlariApi } from '@/lib/api/finans'
import { gelirTanimlariApi } from '@/lib/api/tanimlar'
import { unitsApi } from '@/lib/api/units'
import type { BorcMakbuzu, CreateBorcMakbuzuDto, UpdateBorcMakbuzuDto } from '@/types/finans'
import type { GelirTanimi } from '@/types/tanimlar'
import type { UnitSummary } from '@/types/unit'
import type { UserType } from '@/types/person'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function BorcMakbuzuPage() {
  const [items, setItems] = useState<BorcMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const [gelirTanimlari, setGelirTanimlari] = useState<GelirTanimi[]>([])
  const [units, setUnits] = useState<UnitSummary[]>([])
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<BorcMakbuzu | null>(null)

  const [formDonem, setFormDonem] = useState('')
  const [formSonOdemeTarihi, setFormSonOdemeTarihi] = useState('')
  const [formUnitId, setFormUnitId] = useState('')
  const [formGelirTanimiId, setFormGelirTanimiId] = useState('')
  const [formTutar, setFormTutar] = useState('')
  const [formGecikmeTutari, setFormGecikmeTutari] = useState('0')
  const [formOdenenTutar, setFormOdenenTutar] = useState('0')
  const [formAciklama, setFormAciklama] = useState('')

  const [formBorcluUserId, setFormBorcluUserId] = useState('')
  const [formBorcluAdSoyad, setFormBorcluAdSoyad] = useState('')
  const [formBorcluRol, setFormBorcluRol] = useState<UserType | undefined>(undefined)
  const [borcluSecenekleri, setBorcluSecenekleri] = useState<{ userId: string; adSoyad: string; rol: UserType }[]>([])
  const [borcluLoading, setBorcluLoading] = useState(false)

  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await borcMakbuzlariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { gelirTanimlariApi.getAll().then(r => setGelirTanimlari(r.data.filter(g => g.isActive))) }, [])
  useEffect(() => { unitsApi.getAll().then(r => setUnits(r.data)) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const resetBorclu = () => {
    setFormBorcluUserId(''); setFormBorcluAdSoyad(''); setFormBorcluRol(undefined); setBorcluSecenekleri([])
  }

  const handleUnitChange = async (unitId: string) => {
    setFormUnitId(unitId)
    resetBorclu()
    if (!unitId) return
    setBorcluLoading(true)
    try {
      const res = await unitsApi.getBorcluOnerisi(unitId)
      const d = res.data
      const secenekler: { userId: string; adSoyad: string; rol: UserType }[] = []
      if (d.ownerUserId && d.ownerAdSoyad) secenekler.push({ userId: d.ownerUserId, adSoyad: d.ownerAdSoyad, rol: 2 })
      if (d.tenantUserId && d.tenantAdSoyad) secenekler.push({ userId: d.tenantUserId, adSoyad: d.tenantAdSoyad, rol: 3 })
      setBorcluSecenekleri(secenekler)
      if (d.onerilenPersonId && d.onerilenAdSoyad) {
        setFormBorcluUserId(d.onerilenPersonId)
        setFormBorcluAdSoyad(d.onerilenAdSoyad)
        setFormBorcluRol(d.onerilenRol)
      }
    } catch (e) { showApiError(e) }
    finally { setBorcluLoading(false) }
  }

  const selectBorclu = (userId: string, adSoyad: string, rol: UserType) => {
    setFormBorcluUserId(userId); setFormBorcluAdSoyad(adSoyad); setFormBorcluRol(rol)
  }

  const openCreate = () => {
    setFormDonem(''); setFormSonOdemeTarihi(''); setFormUnitId(''); setFormGelirTanimiId(''); setFormTutar(''); setFormGecikmeTutari('0'); setFormOdenenTutar('0'); setFormAciklama('')
    resetBorclu()
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }
  const openEdit = (item: BorcMakbuzu) => {
    setFormDonem(item.donem ?? ''); setFormSonOdemeTarihi(item.sonOdemeTarihi ? item.sonOdemeTarihi.split('T')[0] : ''); setFormUnitId(item.unitId ?? '')
    setFormGelirTanimiId(''); setFormTutar(String(item.tutar)); setFormGecikmeTutari(String(item.gecikmeTutari)); setFormOdenenTutar(String(item.odenenTutar)); setFormAciklama(item.aciklama ?? '')
    setFormBorcluUserId(item.borcluUserId ?? ''); setFormBorcluAdSoyad(item.borcluAdSoyad ?? ''); setFormBorcluRol(item.borcluRol)
    setBorcluSecenekleri([])
    if (item.unitId) {
      unitsApi.getBorcluOnerisi(item.unitId).then(res => {
        const d = res.data
        const secenekler: { userId: string; adSoyad: string; rol: UserType }[] = []
        if (d.ownerUserId && d.ownerAdSoyad) secenekler.push({ userId: d.ownerUserId, adSoyad: d.ownerAdSoyad, rol: 2 })
        if (d.tenantUserId && d.tenantAdSoyad) secenekler.push({ userId: d.tenantUserId, adSoyad: d.tenantAdSoyad, rol: 3 })
        setBorcluSecenekleri(secenekler)
      }).catch(() => {})
    }
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formTutar || parseFloat(formTutar) <= 0) { showApiError('Tutar sıfırdan büyük olmalıdır.'); return }
    setSaving(true)
    try {
      if (panelMode === 'create') {
        const dto: CreateBorcMakbuzuDto = {
          donem: formDonem || undefined, sonOdemeTarihi: formSonOdemeTarihi || undefined,
          unitId: formUnitId || undefined, borcluUserId: formBorcluUserId || undefined, borcluRol: formBorcluRol,
          gelirTanimiId: formGelirTanimiId || undefined, tutar: parseFloat(formTutar), aciklama: formAciklama || undefined,
        }
        await borcMakbuzlariApi.create(dto)
        showSuccess('Borç makbuzu oluşturuldu.')
      } else if (selected) {
        const dto: UpdateBorcMakbuzuDto = {
          donem: formDonem || undefined, sonOdemeTarihi: formSonOdemeTarihi || undefined,
          unitId: formUnitId || undefined, borcluUserId: formBorcluUserId || undefined, borcluRol: formBorcluRol,
          gelirTanimiId: formGelirTanimiId || undefined, tutar: parseFloat(formTutar),
          gecikmeTutari: parseFloat(formGecikmeTutari) || 0, odenenTutar: parseFloat(formOdenenTutar) || 0,
          aciklama: formAciklama || undefined,
        }
        await borcMakbuzlariApi.update(selected.id, dto)
        showSuccess('Borç makbuzu güncellendi.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await borcMakbuzlariApi.delete(id); showSuccess('Silindi.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Borç Makbuzu</h1>
        <div className="flex gap-2">
          <Link href="/finans/toplu-borclandirma">
            <Button size="sm" variant="outline"><Upload className="h-4 w-4 mr-1" />Toplu Borçlandırma</Button>
          </Link>
          <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Makbuz</Button>
        </div>
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
          <div className="p-12 text-center"><FileText className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Borç makbuzu bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">İşlem Tarihi</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Dönem</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Son Ödeme Tarihi</th>
                <th className="text-left px-3 py-2 font-medium">Daire</th>
                <th className="text-left px-3 py-2 font-medium">Borçlu</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Gecikme</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Ödenen</th>
                <th className="text-right px-3 py-2 font-medium">Kalan</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.islemTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.donem ?? '—'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell text-xs">{item.sonOdemeTarihi ? new Date(item.sonOdemeTarihi).toLocaleDateString('tr-TR') : '—'}</td>
                  <td className="px-3 py-2.5">{item.unitDoorNumber ?? '—'}</td>
                  <td className="px-3 py-2.5">{item.borcluAdSoyad ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.tutar)}</td>
                  <td className="px-3 py-2.5 text-right text-muted-foreground hidden md:table-cell">{fmt(item.gecikmeTutari)}</td>
                  <td className="px-3 py-2.5 text-right text-muted-foreground hidden md:table-cell">{fmt(item.odenenTutar)}</td>
                  <td className="px-3 py-2.5 text-right">
                    <Badge variant={item.kalanTutar <= 0 ? 'secondary' : 'default'} className="text-xs">{fmt(item.kalanTutar)}</Badge>
                  </td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'Yeni Borç Makbuzu' : 'Borç Makbuzu Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="unit">Daire</Label>
                <select id="unit" value={formUnitId} onChange={e => handleUnitChange(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Daire seçilmedi —</option>
                  {units.map(u => <option key={u.id} value={u.id}>{u.buildingName ? `${u.buildingName} / ` : ''}{u.doorNumber}</option>)}
                </select>
              </div>

              <div className="space-y-1.5">
                <Label>Borçlu</Label>
                {borcluLoading ? (
                  <div className="text-xs text-muted-foreground">Öneri yükleniyor...</div>
                ) : borcluSecenekleri.length > 1 ? (
                  <div className="flex flex-col gap-1.5">
                    {borcluSecenekleri.map(s => (
                      <button
                        key={s.userId}
                        type="button"
                        onClick={() => selectBorclu(s.userId, s.adSoyad, s.rol)}
                        className={`text-left border rounded-md px-3 py-2 text-sm ${formBorcluUserId === s.userId ? 'border-primary bg-primary/5' : 'hover:bg-muted/50'}`}
                      >
                        {s.rol === 2 ? 'Ev Sahibi' : 'Kiracı'}: {s.adSoyad}
                      </button>
                    ))}
                  </div>
                ) : (
                  <Input value={formBorcluAdSoyad} readOnly placeholder="Daire seçince otomatik doldurulur" className="bg-muted/30" />
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="donem">Dönem</Label>
                <Input id="donem" value={formDonem} onChange={e => setFormDonem(e.target.value)} placeholder="örn. 2026-06" maxLength={10} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="sonOdeme">Son Ödeme Tarihi</Label>
                <Input id="sonOdeme" type="date" value={formSonOdemeTarihi} onChange={e => setFormSonOdemeTarihi(e.target.value)} />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="gelirTanimi">Kalem</Label>
                <select id="gelirTanimi" value={formGelirTanimiId} onChange={e => setFormGelirTanimiId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Seçilmedi —</option>
                  {gelirTanimlari.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Tutar (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" min={0} step="0.01" value={formTutar} onChange={e => setFormTutar(e.target.value)} placeholder="0.00" />
              </div>
              {panelMode === 'edit' && <>
                <div className="space-y-1.5">
                  <Label htmlFor="gecikme">Gecikme Tutarı (₺)</Label>
                  <Input id="gecikme" type="number" min={0} step="0.01" value={formGecikmeTutari} onChange={e => setFormGecikmeTutari(e.target.value)} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="odenen">Ödenen Tutar (₺)</Label>
                  <Input id="odenen" type="number" min={0} step="0.01" value={formOdenenTutar} onChange={e => setFormOdenenTutar(e.target.value)} />
                </div>
              </>}
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Açıklama</Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Opsiyonel not" maxLength={500} />
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
