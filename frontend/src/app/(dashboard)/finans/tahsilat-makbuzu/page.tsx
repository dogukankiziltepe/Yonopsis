'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Receipt, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { tahsilatMakbuzlariApi, borcMakbuzlariApi } from '@/lib/api/finans'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import { personsApi } from '@/lib/api/persons'
import type { TahsilatMakbuzu, CreateTahsilatMakbuzuDto, UpdateTahsilatMakbuzuDto, BorcMakbuzu } from '@/types/finans'
import type { KasaBanka } from '@/types/tanimlar'
import type { PersonDto } from '@/types/person'
import { OdemeTipi, OdemeTipiLabel } from '@/types/finans'
import { showSuccess, showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function TahsilatMakbuzuPage() {
  const [items, setItems] = useState<TahsilatMakbuzu[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [kasaBankalar, setKasaBankalar] = useState<KasaBanka[]>([])

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selected, setSelected] = useState<TahsilatMakbuzu | null>(null)

  const [formBorcluUserId, setFormBorcluUserId] = useState('')
  const [formBorcluAdSoyad, setFormBorcluAdSoyad] = useState('')
  const [kisiArama, setKisiArama] = useState('')
  const [kisiSonuclari, setKisiSonuclari] = useState<PersonDto[]>([])

  const [formBorcMakbuzuId, setFormBorcMakbuzuId] = useState('')
  const [selectedBorc, setSelectedBorc] = useState<BorcMakbuzu | null>(null)
  const [borcArama, setBorcArama] = useState('')
  const [borcSonuclari, setBorcSonuclari] = useState<BorcMakbuzu[]>([])

  const [formKasaBankaId, setFormKasaBankaId] = useState('')
  const [formOdemeTutari, setFormOdemeTutari] = useState('')
  const [formOdemeTipi, setFormOdemeTipi] = useState<OdemeTipi>(OdemeTipi.Nakit)
  const [formAciklama, setFormAciklama] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await tahsilatMakbuzlariApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { kasaBankaApi.getAll().then(r => setKasaBankalar(r.data.filter(k => k.isActive))) }, [])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  useEffect(() => {
    if (!kisiArama.trim()) { setKisiSonuclari([]); return }
    const t = setTimeout(() => {
      personsApi.getAll(1, 8, kisiArama).then(r => setKisiSonuclari(r.data.items ?? [])).catch(() => {})
    }, 300)
    return () => clearTimeout(t)
  }, [kisiArama])

  useEffect(() => {
    if (!borcArama.trim()) { setBorcSonuclari([]); return }
    const t = setTimeout(() => {
      borcMakbuzlariApi.getAll(1, 8, borcArama).then(r => setBorcSonuclari(r.data.items ?? [])).catch(() => {})
    }, 300)
    return () => clearTimeout(t)
  }, [borcArama])

  const resetForm = () => {
    setFormBorcluUserId(''); setFormBorcluAdSoyad(''); setKisiArama(''); setKisiSonuclari([])
    setFormBorcMakbuzuId(''); setSelectedBorc(null); setBorcArama(''); setBorcSonuclari([])
    setFormKasaBankaId(''); setFormOdemeTutari(''); setFormOdemeTipi(OdemeTipi.Nakit); setFormAciklama('')
  }

  const openCreate = () => { resetForm(); setSelected(null); setPanelMode('create'); setPanelOpen(true) }
  const openEdit = (item: TahsilatMakbuzu) => {
    resetForm()
    setFormBorcluUserId(item.borcluUserId ?? ''); setFormBorcluAdSoyad(item.borcluAdSoyad ?? '')
    setFormBorcMakbuzuId(item.borcMakbuzuId ?? '')
    setFormKasaBankaId(item.kasaBankaId ?? ''); setFormOdemeTutari(String(item.odemeTutari)); setFormOdemeTipi(item.odemeTipi); setFormAciklama(item.aciklama ?? '')
    setSelected(item); setPanelMode('edit'); setPanelOpen(true)
  }
  const closePanel = () => { setPanelOpen(false); setSelected(null); setDeleteConfirm(null) }

  const selectKisi = (p: PersonDto) => {
    setFormBorcluUserId(p.userId); setFormBorcluAdSoyad(`${p.firstName} ${p.lastName}`); setKisiArama(''); setKisiSonuclari([])
  }

  const selectBorc = (b: BorcMakbuzu) => {
    setFormBorcMakbuzuId(b.id); setSelectedBorc(b); setBorcArama(''); setBorcSonuclari([])
    if (panelMode === 'create') setFormOdemeTutari(String(b.kalanTutar > 0 ? b.kalanTutar : 0))
    if (b.borcluUserId && b.borcluAdSoyad && !formBorcluUserId) {
      setFormBorcluUserId(b.borcluUserId); setFormBorcluAdSoyad(b.borcluAdSoyad)
    }
  }

  const clearBorc = () => { setFormBorcMakbuzuId(''); setSelectedBorc(null) }

  const handleSave = async () => {
    if (!formOdemeTutari || parseFloat(formOdemeTutari) <= 0) { showApiError('Ödeme tutarı sıfırdan büyük olmalıdır.'); return }
    setSaving(true)
    try {
      const dto: CreateTahsilatMakbuzuDto = {
        borcluUserId: formBorcluUserId || undefined,
        kasaBankaId: formKasaBankaId || undefined,
        borcMakbuzuId: formBorcMakbuzuId || undefined,
        odemeTutari: parseFloat(formOdemeTutari),
        odemeTipi: formOdemeTipi,
        aciklama: formAciklama || undefined,
      }
      if (panelMode === 'create') {
        await tahsilatMakbuzlariApi.create(dto); showSuccess('Tahsilat makbuzu oluşturuldu.')
      } else if (selected) {
        await tahsilatMakbuzlariApi.update(selected.id, dto as UpdateTahsilatMakbuzuDto); showSuccess('Tahsilat makbuzu güncellendi.')
      }
      await load(); closePanel()
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    try { await tahsilatMakbuzlariApi.delete(id); showSuccess('Silindi.'); await load(); setDeleteConfirm(null) }
    catch (e) { showApiError(e) }
  }

  const odemeTipiOptions = Object.entries(OdemeTipiLabel).map(([k, v]) => ({ value: Number(k) as OdemeTipi, label: v }))

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Tahsilat Makbuzu</h1>
        <Button size="sm" onClick={openCreate}><Plus className="h-4 w-4 mr-1" />Yeni Makbuz</Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="Evrak no, isim ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kayıt</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><Receipt className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Tahsilat makbuzu bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Evrak No</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Tarih</th>
                <th className="text-left px-3 py-2 font-medium">Ödeyen</th>
                <th className="text-left px-3 py-2 font-medium hidden lg:table-cell">Hesap</th>
                <th className="text-left px-3 py-2 font-medium hidden md:table-cell">Yöntem</th>
                <th className="text-right px-3 py-2 font-medium">Tutar</th>
                <th className="w-20 px-3 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(item => (
                <tr key={item.id} className="hover:bg-muted/30">
                  <td className="px-3 py-2.5 font-mono text-xs">{item.evrakNo}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell text-xs">{new Date(item.islemTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-3 py-2.5">{item.borcluAdSoyad ?? '—'}</td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden lg:table-cell">{item.kasaBankaAdi ?? '—'}</td>
                  <td className="px-3 py-2.5 hidden md:table-cell"><Badge variant="outline" className="text-xs">{OdemeTipiLabel[item.odemeTipi]}</Badge></td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(item.odemeTutari)}</td>
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
              <h2 className="font-semibold">{panelMode === 'create' ? 'Yeni Tahsilat Makbuzu' : 'Tahsilat Makbuzu Düzenle'}</h2>
              <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              <div className="space-y-1.5">
                <Label>İlişkili Borç Makbuzu</Label>
                {formBorcMakbuzuId ? (
                  <div className="flex items-center justify-between border rounded-md px-3 py-2 text-sm">
                    <div>
                      <div className="font-mono text-xs">{selectedBorc?.evrakNo}</div>
                      {selectedBorc && <div className="text-xs text-muted-foreground">Kalan: {fmt(selectedBorc.kalanTutar)}</div>}
                    </div>
                    <Button variant="ghost" size="sm" className="h-6 px-2 text-xs" onClick={clearBorc}>Kaldır</Button>
                  </div>
                ) : (
                  <div className="relative">
                    <Input value={borcArama} onChange={e => setBorcArama(e.target.value)} placeholder="Evrak no ile ara..." />
                    {borcSonuclari.length > 0 && (
                      <div className="absolute z-10 w-full bg-background border rounded-md mt-1 shadow-lg max-h-48 overflow-y-auto">
                        {borcSonuclari.map(b => (
                          <button key={b.id} type="button" onClick={() => selectBorc(b)} className="w-full text-left px-3 py-2 text-sm hover:bg-muted/50">
                            <div className="font-mono text-xs">{b.evrakNo}</div>
                            <div className="text-xs text-muted-foreground">{b.borcluAdSoyad ?? b.unitDoorNumber ?? '—'} · Kalan: {fmt(b.kalanTutar)}</div>
                          </button>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div className="space-y-1.5">
                <Label>Ödeyen (Borçlu)</Label>
                {formBorcluAdSoyad ? (
                  <div className="flex items-center justify-between border rounded-md px-3 py-2 text-sm">
                    {formBorcluAdSoyad}
                    <Button variant="ghost" size="sm" className="h-6 px-2 text-xs" onClick={() => { setFormBorcluUserId(''); setFormBorcluAdSoyad('') }}>Kaldır</Button>
                  </div>
                ) : (
                  <div className="relative">
                    <Input value={kisiArama} onChange={e => setKisiArama(e.target.value)} placeholder="İsim ile ara..." />
                    {kisiSonuclari.length > 0 && (
                      <div className="absolute z-10 w-full bg-background border rounded-md mt-1 shadow-lg max-h-48 overflow-y-auto">
                        {kisiSonuclari.map(p => (
                          <button key={p.userId} type="button" onClick={() => selectKisi(p)} className="w-full text-left px-3 py-2 text-sm hover:bg-muted/50">
                            {p.firstName} {p.lastName}
                          </button>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="kasaBanka">Kasa / Banka Hesabı</Label>
                <select id="kasaBanka" value={formKasaBankaId} onChange={e => setFormKasaBankaId(e.target.value)} className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring">
                  <option value="">— Seçilmedi —</option>
                  {kasaBankalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
                </select>
              </div>
              <div className="space-y-1.5">
                <Label htmlFor="amount">Tutar (₺) <span className="text-destructive">*</span></Label>
                <Input id="amount" type="number" min={0} step="0.01" value={formOdemeTutari} onChange={e => setFormOdemeTutari(e.target.value)} placeholder="0.00" />
              </div>
              <div className="space-y-1.5">
                <Label>Ödeme Yöntemi</Label>
                <div className="grid grid-cols-2 gap-2">
                  {odemeTipiOptions.map(o => (
                    <label key={o.value} className={`flex items-center gap-2 border rounded-md px-3 py-2 cursor-pointer text-sm ${formOdemeTipi === o.value ? 'border-primary bg-primary/5' : ''}`}>
                      <input type="radio" name="odemeTipi" checked={formOdemeTipi === o.value} onChange={() => setFormOdemeTipi(o.value)} className="h-3.5 w-3.5" />
                      {o.label}
                    </label>
                  ))}
                </div>
              </div>
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
