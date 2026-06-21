'use client'

import { useCallback, useEffect, useState } from 'react'
import { ChevronDown, ChevronRight, Plus, X, FolderTree, Users } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showSuccess, showError } from '@/lib/toast'
import { muhasebeApi } from '@/lib/api/muhasebe'
import {
  CariTuru,
  HesapKategorisi,
  HesapListItem,
  HesapNode,
  HesapTipi,
  NormalBakiye,
  cariTuruLabel,
  hesapKategorisiLabel,
  normalBakiyeLabel,
} from '@/types/muhasebe'

type Tab = 'plan' | 'cari'
type PanelMode = 'createHesap' | 'editHesap' | 'createCari'

const kategoriOptions = Object.values(HesapKategorisi).filter((v) => typeof v === 'number') as HesapKategorisi[]
const bakiyeOptions = Object.values(NormalBakiye).filter((v) => typeof v === 'number') as NormalBakiye[]
const cariTuruOptions = Object.values(CariTuru).filter((v) => typeof v === 'number') as CariTuru[]

export default function HesapPlaniPage() {
  const [tab, setTab] = useState<Tab>('plan')

  // Ağaç
  const [tree, setTree] = useState<HesapNode[]>([])
  const [loading, setLoading] = useState(true)
  const [expanded, setExpanded] = useState<Set<string>>(new Set())

  // Cari liste
  const [cariList, setCariList] = useState<HesapListItem[]>([])
  const [cariLoading, setCariLoading] = useState(false)
  const [cariFilter, setCariFilter] = useState<CariTuru | ''>('')

  // Üst hesap seçimi için düz liste
  const [flatList, setFlatList] = useState<HesapListItem[]>([])

  // Panel
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<PanelMode>('createHesap')
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  // Form alanları
  const [fHesapKodu, setFHesapKodu] = useState('')
  const [fHesapAdi, setFHesapAdi] = useState('')
  const [fKategori, setFKategori] = useState<HesapKategorisi>(HesapKategorisi.Aktif)
  const [fBakiye, setFBakiye] = useState<NormalBakiye>(NormalBakiye.Borc)
  const [fParentId, setFParentId] = useState<string>('')
  const [fFisKesilebilir, setFFisKesilebilir] = useState(false)
  const [fAciklama, setFAciklama] = useState('')
  const [fAktif, setFAktif] = useState(true)
  const [fCariTuru, setFCariTuru] = useState<CariTuru>(CariTuru.Kiraci)

  const loadTree = useCallback(() => {
    setLoading(true)
    muhasebeApi.getTree()
      .then((res) => setTree(res.data ?? []))
      .catch(() => showError('Hesap planı yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  const loadFlat = useCallback(() => {
    muhasebeApi.getHesaplar()
      .then((res) => setFlatList(res.data ?? []))
      .catch(() => {})
  }, [])

  const loadCari = useCallback(() => {
    setCariLoading(true)
    muhasebeApi.getCariHesaplar(cariFilter === '' ? undefined : { cariTuru: cariFilter })
      .then((res) => setCariList(res.data ?? []))
      .catch(() => showError('Cari hesaplar yüklenemedi.'))
      .finally(() => setCariLoading(false))
  }, [cariFilter])

  useEffect(() => { loadTree(); loadFlat() }, [loadTree, loadFlat])
  useEffect(() => { if (tab === 'cari') loadCari() }, [tab, loadCari])

  const toggleExpand = (id: string) => {
    setExpanded((prev) => {
      const next = new Set(prev)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })
  }

  const openCreateHesap = () => {
    setPanelMode('createHesap')
    setSelectedId(null)
    setFHesapKodu('')
    setFHesapAdi('')
    setFKategori(HesapKategorisi.Aktif)
    setFBakiye(NormalBakiye.Borc)
    setFParentId('')
    setFFisKesilebilir(false)
    setFAciklama('')
    setPanelOpen(true)
  }

  const openEditHesap = async (id: string) => {
    setPanelMode('editHesap')
    setSelectedId(id)
    setPanelOpen(true)
    try {
      const { data } = await muhasebeApi.getHesap(id)
      setFHesapKodu(data.hesapKodu)
      setFHesapAdi(data.hesapAdi)
      setFKategori(data.hesapKategorisi)
      setFBakiye(data.normalBakiye)
      setFFisKesilebilir(data.fisKesilebilirMi)
      setFAktif(data.aktifMi)
      setFAciklama(data.aciklama ?? '')
    } catch {
      showError('Hesap detayı yüklenemedi.')
      setPanelOpen(false)
    }
  }

  const openCreateCari = () => {
    setPanelMode('createCari')
    setSelectedId(null)
    setFCariTuru(CariTuru.Kiraci)
    setFHesapAdi('')
    setFAciklama('')
    setPanelOpen(true)
  }

  const handleCreateHesap = async () => {
    if (!fHesapKodu.trim() || !fHesapAdi.trim()) {
      showError('Hesap kodu ve adı zorunludur.')
      return
    }
    setSaving(true)
    try {
      await muhasebeApi.createHesap({
        hesapKodu: fHesapKodu.trim(),
        hesapAdi: fHesapAdi.trim(),
        hesapKategorisi: fKategori,
        normalBakiye: fBakiye,
        parentId: fParentId || null,
        fisKesilebilirMi: fFisKesilebilir,
        aciklama: fAciklama.trim() || undefined,
      })
      showSuccess('Hesap oluşturuldu.')
      setPanelOpen(false)
      loadTree(); loadFlat()
    } catch {} finally { setSaving(false) }
  }

  const handleUpdateHesap = async () => {
    if (!selectedId || !fHesapAdi.trim()) return
    setSaving(true)
    try {
      await muhasebeApi.updateHesap(selectedId, {
        hesapAdi: fHesapAdi.trim(),
        hesapKategorisi: fKategori,
        normalBakiye: fBakiye,
        fisKesilebilirMi: fFisKesilebilir,
        aciklama: fAciklama.trim() || undefined,
      })
      if (selectedId) await muhasebeApi.toggleAktif(selectedId, fAktif)
      showSuccess('Hesap güncellendi.')
      setPanelOpen(false)
      loadTree(); loadFlat()
    } catch {} finally { setSaving(false) }
  }

  const handleCreateCari = async () => {
    if (!fHesapAdi.trim()) {
      showError('Cari hesap adı zorunludur.')
      return
    }
    setSaving(true)
    try {
      await muhasebeApi.createCariHesap({
        cariTuru: fCariTuru,
        hesapAdi: fHesapAdi.trim(),
        aciklama: fAciklama.trim() || undefined,
      })
      showSuccess('Cari hesap oluşturuldu.')
      setPanelOpen(false)
      loadCari(); loadTree(); loadFlat()
    } catch {} finally { setSaving(false) }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Hesap Planı</h1>
        {tab === 'plan' ? (
          <Button size="sm" onClick={openCreateHesap}>
            <Plus className="h-4 w-4 mr-1" /> Yeni Hesap
          </Button>
        ) : (
          <Button size="sm" onClick={openCreateCari}>
            <Plus className="h-4 w-4 mr-1" /> Yeni Cari
          </Button>
        )}
      </div>

      {/* Sekmeler */}
      <div className="flex gap-1 border-b mb-4">
        <button
          onClick={() => setTab('plan')}
          className={`flex items-center gap-1.5 px-3 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
            tab === 'plan' ? 'border-primary text-foreground' : 'border-transparent text-muted-foreground hover:text-foreground'
          }`}
        >
          <FolderTree className="h-4 w-4" /> Hesap Ağacı
        </button>
        <button
          onClick={() => setTab('cari')}
          className={`flex items-center gap-1.5 px-3 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
            tab === 'cari' ? 'border-primary text-foreground' : 'border-transparent text-muted-foreground hover:text-foreground'
          }`}
        >
          <Users className="h-4 w-4" /> Cari Hesaplar
        </button>
      </div>

      {tab === 'plan' ? (
        <div className="border rounded-lg overflow-hidden">
          {loading ? (
            <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
          ) : tree.length === 0 ? (
            <div className="flex flex-col items-center py-12">
              <FolderTree className="h-10 w-10 text-muted-foreground/50 mb-3" />
              <p className="text-muted-foreground">Hesap planı boş. Site için seed çalıştırılmamış olabilir.</p>
            </div>
          ) : (
            <div className="divide-y">
              {tree.map((node) => (
                <TreeRow
                  key={node.id}
                  node={node}
                  depth={0}
                  expanded={expanded}
                  onToggle={toggleExpand}
                  onSelect={openEditHesap}
                />
              ))}
            </div>
          )}
        </div>
      ) : (
        <div className="flex flex-col gap-3">
          <div className="flex items-center gap-2">
            <Label className="text-xs">Cari Türü:</Label>
            <select
              value={cariFilter}
              onChange={(e) => setCariFilter(e.target.value === '' ? '' : (Number(e.target.value) as CariTuru))}
              className="h-8 rounded-md border bg-background px-2 text-sm"
            >
              <option value="">Tümü</option>
              {cariTuruOptions.map((t) => (
                <option key={t} value={t}>{cariTuruLabel[t]}</option>
              ))}
            </select>
          </div>
          <div className="border rounded-lg overflow-hidden overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-muted/50">
                  <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Hesap Kodu</th>
                  <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Hesap Adı</th>
                  <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tür</th>
                  <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
                </tr>
              </thead>
              <tbody>
                {cariLoading ? (
                  <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
                ) : cariList.length === 0 ? (
                  <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Cari hesap bulunamadı.</td></tr>
                ) : (
                  cariList.map((c) => (
                    <tr key={c.id} onClick={() => openEditHesap(c.id)} className="border-b last:border-0 cursor-pointer hover:bg-muted/30">
                      <td className="px-3 py-2.5 font-mono text-xs">{c.hesapKodu}</td>
                      <td className="px-3 py-2.5 font-medium">{c.hesapAdi}</td>
                      <td className="px-3 py-2.5">{c.cariTuru != null ? cariTuruLabel[c.cariTuru] : '-'}</td>
                      <td className="px-3 py-2.5">
                        <Badge variant={c.aktifMi ? 'default' : 'secondary'}>{c.aktifMi ? 'Aktif' : 'Pasif'}</Badge>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[420px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">
              {panelMode === 'createHesap' && 'Yeni Hesap'}
              {panelMode === 'editHesap' && 'Hesap Düzenle'}
              {panelMode === 'createCari' && 'Yeni Cari Hesap'}
            </h2>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'createCari' ? (
              <>
                <Field label="Cari Türü" required>
                  <select
                    value={fCariTuru}
                    onChange={(e) => setFCariTuru(Number(e.target.value) as CariTuru)}
                    className="h-9 w-full rounded-md border bg-background px-2 text-sm"
                  >
                    {cariTuruOptions.map((t) => (
                      <option key={t} value={t}>{cariTuruLabel[t]}</option>
                    ))}
                  </select>
                </Field>
                <Field label="Cari Adı / Unvan" required>
                  <Input value={fHesapAdi} onChange={(e) => setFHesapAdi(e.target.value)} placeholder="Örn: Ahmet Yılmaz" />
                </Field>
                <Field label="Açıklama">
                  <Input value={fAciklama} onChange={(e) => setFAciklama(e.target.value)} />
                </Field>
                <p className="text-xs text-muted-foreground">
                  Cari kod ilgili ana hesap altında otomatik üretilir (örn. 120.01.0001).
                </p>
                <Button size="sm" className="w-full" onClick={handleCreateCari} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Oluştur'}
                </Button>
              </>
            ) : (
              <>
                <Field label="Hesap Kodu" required>
                  <Input
                    value={fHesapKodu}
                    onChange={(e) => setFHesapKodu(e.target.value)}
                    placeholder="Örn: 770.11"
                    disabled={panelMode === 'editHesap'}
                    className="font-mono"
                  />
                </Field>
                <Field label="Hesap Adı" required>
                  <Input value={fHesapAdi} onChange={(e) => setFHesapAdi(e.target.value)} />
                </Field>
                {panelMode === 'createHesap' && (
                  <Field label="Üst Hesap">
                    <select
                      value={fParentId}
                      onChange={(e) => setFParentId(e.target.value)}
                      className="h-9 w-full rounded-md border bg-background px-2 text-sm"
                    >
                      <option value="">(Kök hesap)</option>
                      {flatList.map((h) => (
                        <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>
                      ))}
                    </select>
                  </Field>
                )}
                <Field label="Kategori" required>
                  <select
                    value={fKategori}
                    onChange={(e) => setFKategori(Number(e.target.value) as HesapKategorisi)}
                    className="h-9 w-full rounded-md border bg-background px-2 text-sm"
                  >
                    {kategoriOptions.map((k) => (
                      <option key={k} value={k}>{hesapKategorisiLabel[k]}</option>
                    ))}
                  </select>
                </Field>
                <Field label="Normal Bakiye" required>
                  <select
                    value={fBakiye}
                    onChange={(e) => setFBakiye(Number(e.target.value) as NormalBakiye)}
                    className="h-9 w-full rounded-md border bg-background px-2 text-sm"
                  >
                    {bakiyeOptions.map((b) => (
                      <option key={b} value={b}>{normalBakiyeLabel[b]}</option>
                    ))}
                  </select>
                </Field>
                <label className="flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" checked={fFisKesilebilir} onChange={(e) => setFFisKesilebilir(e.target.checked)} className="rounded" />
                  <span className="text-xs font-medium">Fiş kesilebilir (yaprak hesap)</span>
                </label>
                {panelMode === 'editHesap' && (
                  <label className="flex items-center gap-2 cursor-pointer">
                    <input type="checkbox" checked={fAktif} onChange={(e) => setFAktif(e.target.checked)} className="rounded" />
                    <span className="text-xs font-medium">Aktif</span>
                  </label>
                )}
                <Field label="Açıklama">
                  <Input value={fAciklama} onChange={(e) => setFAciklama(e.target.value)} />
                </Field>
                <Button
                  size="sm"
                  className="w-full"
                  onClick={panelMode === 'createHesap' ? handleCreateHesap : handleUpdateHesap}
                  disabled={saving}
                >
                  {saving ? 'Kaydediliyor...' : panelMode === 'createHesap' ? 'Oluştur' : 'Kaydet'}
                </Button>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  )
}

function Field({ label, required, children }: { label: string; required?: boolean; children: React.ReactNode }) {
  return (
    <div className="space-y-1">
      <Label className="text-xs font-medium">
        {label} {required && <span className="text-destructive">*</span>}
      </Label>
      {children}
    </div>
  )
}

function TreeRow({
  node,
  depth,
  expanded,
  onToggle,
  onSelect,
}: {
  node: HesapNode
  depth: number
  expanded: Set<string>
  onToggle: (id: string) => void
  onSelect: (id: string) => void
}) {
  const hasChildren = node.children.length > 0
  const isOpen = expanded.has(node.id)

  return (
    <>
      <div
        className="flex items-center gap-2 px-3 py-2 hover:bg-muted/30 text-sm"
        style={{ paddingLeft: `${depth * 18 + 12}px` }}
      >
        {hasChildren ? (
          <button onClick={() => onToggle(node.id)} className="shrink-0 text-muted-foreground hover:text-foreground">
            {isOpen ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
          </button>
        ) : (
          <span className="w-4 shrink-0" />
        )}
        <button onClick={() => onSelect(node.id)} className="flex items-center gap-2 flex-1 text-left min-w-0">
          <span className="font-mono text-xs text-muted-foreground shrink-0">{node.hesapKodu}</span>
          <span className={`truncate ${node.aktifMi ? '' : 'line-through text-muted-foreground'}`}>{node.hesapAdi}</span>
          {node.hesapTipi === HesapTipi.Cari && (
            <Badge variant="outline" className="text-[10px]">Cari</Badge>
          )}
          {node.fisKesilebilirMi && (
            <Badge variant="secondary" className="text-[10px]">Fiş</Badge>
          )}
        </button>
      </div>
      {hasChildren && isOpen && node.children.map((child) => (
        <TreeRow key={child.id} node={child} depth={depth + 1} expanded={expanded} onToggle={onToggle} onSelect={onSelect} />
      ))}
    </>
  )
}
