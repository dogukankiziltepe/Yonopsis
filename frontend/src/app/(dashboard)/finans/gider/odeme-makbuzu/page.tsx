'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, FileText, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import { odemeMakbuzlariApi, cariHesaplariApi } from '@/lib/api/finans'
import { giderTanimlariApi, kasaBankaApi } from '@/lib/api/tanimlar'
import type { OdemeMakbuzu, CreateOdemeMakbuzuDto, UpdateOdemeMakbuzuDto, CariHesapPicker } from '@/types/finans'
import type { GiderTanimi, KasaBanka } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function OdemeMakbuzuPage() {
  const [items, setItems] = useState<OdemeMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const [giderTanimlari, setGiderTanimlari] = useState<GiderTanimi[]>([])
  const [kasaBankalar, setKasaBankalar] = useState<KasaBanka[]>([])
  const [cariArama, setCariArama] = useState('')
  const [cariSonuclari, setCariSonuclari] = useState<CariHesapPicker[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<OdemeMakbuzu | null>(null)

  const [formTarih, setFormTarih] = useState('')
  const [formCariHesapId, setFormCariHesapId] = useState('')
  const [formCariHesapAdi, setFormCariHesapAdi] = useState('')
  const [formKasaBankaId, setFormKasaBankaId] = useState('')
  const [formGiderTanimiId, setFormGiderTanimiId] = useState('')
  const [formTutar, setFormTutar] = useState('')
  const [formAciklama, setFormAciklama] = useState('')
  const [formDagitim, setFormDagitim] = useState(false)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await odemeMakbuzlariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { giderTanimlariApi.getAll().then(r => setGiderTanimlari(r.data.filter(g => g.isActive))) }, [])
  useEffect(() => { kasaBankaApi.getAll().then(r => setKasaBankalar(r.data.filter(k => k.isActive))) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  useEffect(() => {
    if (!cariArama.trim()) { setCariSonuclari([]); return }
    const t = setTimeout(() => {
      cariHesaplariApi.getAll(cariArama).then(r => setCariSonuclari(r.data)).catch(() => {})
    }, 300)
    return () => clearTimeout(t)
  }, [cariArama])

  const selectCari = (c: CariHesapPicker) => {
    setFormCariHesapId(c.id); setFormCariHesapAdi(c.hesapAdi); setCariArama(''); setCariSonuclari([])
  }

  const openCreate = () => {
    setFormTarih(''); setFormCariHesapId(''); setFormCariHesapAdi(''); setCariArama(''); setCariSonuclari([])
    setFormKasaBankaId(''); setFormGiderTanimiId(''); setFormTutar(''); setFormAciklama(''); setFormDagitim(false)
    setSelected(null); setPanelMode('create'); setPanelOpen(true)
  }
  const openEdit = (item: OdemeMakbuzu) => {
    setFormTarih(item.tarih.split('T')[0]); setFormCariHesapId(item.cariHesapId); setFormCariHesapAdi(item.cariHesapAdi ?? '')
    setCariArama(''); setCariSonuclari([])
    setFormKasaBankaId(item.kasaBankaId); setFormGiderTanimiId(item.giderTanimiId); setFormTutar(String(item.tutar))
    setFormAciklama(item.aciklama ?? ''); setFormDagitim(item.dagitimYapilacak)
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const handleSave = async () => {
    if (!formTarih) { showApiError('Tarih zorunludur.'); return }
    if (!formCariHesapId) { showApiError('Cari hesap seçilmelidir.'); return }
    if (!formKasaBankaId) { showApiError('Kasa/Banka seçilmelidir.'); return }
    if (!formGiderTanimiId) { showApiError('Gider hesabı seçilmelidir.'); return }
    if (!formTutar || parseFloat(formTutar) <= 0) { showApiError('Tutar sıfırdan büyük olmalıdır.'); return }
    setSaving(true)
    try {
      const dto: CreateOdemeMakbuzuDto | UpdateOdemeMakbuzuDto = {
        tarih: formTarih, cariHesapId: formCariHesapId, kasaBankaId: formKasaBankaId,
        giderTanimiId: formGiderTanimiId, tutar: parseFloat(formTutar), aciklama: formAciklama || undefined,
        dagitimYapilacak: formDagitim,
      }
      if (panelMode === 'create') {
        await odemeMakbuzlariApi.create(dto); showSuccess('Ödeme makbuzu oluşturuldu.')
      } else if (selected) {
        await odemeMakbuzlariApi.update(selected.id, dto); showSuccess('Ödeme makbuzu güncellendi.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await odemeMakbuzlariApi.delete(id); showSuccess('Silindi.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Ödeme Makbuzu</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Makbuz</Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, cari ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><FileText className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Ödeme makbuzu bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">İşlem Tarihi</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Gider</th>
                <th className="text-left px-3 py-2 font-medium">Cari</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Kasa</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.islemTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell text-xs">{new Date(item.tarih).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5">{item.giderTanimiAdi ?? '—'}</td>
                  <td className="px-3 py-2.5">{item.cariHesapAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.kasaBankaAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.tutar)}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'Yeni Ödeme Makbuzu' : 'Ödeme Makbuzu Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="tarih">Tarih <span className="text-destructive">*</span></Label>
                <Input id="tarih" type="date" value={formTarih} onChange={e => setFormTarih(e.target.value)} />
              </div>

              <div className="space-y-1.5">
                <Label>Cari Hesap <span className="text-destructive">*</span></Label>
                {formCariHesapAdi ? (
                  <div className="flex items-center justify-between border rounded-md px-3 py-2 text-sm">
                    {formCariHesapAdi}
                    <Button variant="ghost" size="sm" className="h-6 px-2 text-xs" onClick={() => { setFormCariHesapId(''); setFormCariHesapAdi('') }}>Kaldır</Button>
                  </div>
                ) : (
                  <div className="relative">
                    <Input value={cariArama} onChange={e => setCariArama(e.target.value)} placeholder="Cari hesap ara..." />
                    {cariSonuclari.length > 0 && (
                      <div className="absolute z-10 w-full bg-background border rounded-md mt-1 shadow-lg max-h-48 overflow-y-auto">
                        {cariSonuclari.map(c => (
                          <button key={c.id} type="button" onClick={() => selectCari(c)} className="w-full text-left px-3 py-2 text-sm hover:bg-muted/50">
                            {c.hesapAdi}
                          </button>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="kasaBanka">Kasa / Banka <span className="text-destructive">*</span></Label>
                <select id="kasaBanka" value={formKasaBankaId} onChange={e => setFormKasaBankaId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Seçilmedi —</option>
                  {kasaBankalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="giderTanimi">Gider Hesabı <span className="text-destructive">*</span></Label>
                <select id="giderTanimi" value={formGiderTanimiId} onChange={e => setFormGiderTanimiId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Seçilmedi —</option>
                  {giderTanimlari.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="tutar">Tutar (₺) <span className="text-destructive">*</span></Label>
                <Input id="tutar" type="number" min={0} step="0.01" value={formTutar} onChange={e => setFormTutar(e.target.value)} placeholder="0.00" />
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="aciklama">Açıklama</Label>
                <Input id="aciklama" value={formAciklama} onChange={e => setFormAciklama(e.target.value)} placeholder="Opsiyonel not" maxLength={500} />
              </div>
              <label className="flex items-center gap-2 text-sm cursor-not-allowed opacity-60" title="Bu özellik henüz aktif değil">
                <Checkbox checked={formDagitim} onCheckedChange={v => setFormDagitim(!!v)} disabled />
                Dağıtım Yapılacak (yakında)
              </label>
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
