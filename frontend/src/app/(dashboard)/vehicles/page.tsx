'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Car, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { PaginationControls } from '@/components/ui/pagination-controls'
import { vehiclesApi } from '@/lib/api/vehicles'
import { showSuccess, showError } from '@/lib/toast'
import { VehicleSummaryDto, CreateVehicleDto } from '@/types/vehicle'

const PAGE_SIZE = 20

export default function VehiclesPage() {
  const [items, setItems] = useState<VehicleSummaryDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchDebounced, setSearchDebounced] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [selected, setSelected] = useState<VehicleSummaryDto | null>(null)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formUserId, setFormUserId] = useState('')
  const [formPlate, setFormPlate] = useState('')
  const [formBrand, setFormBrand] = useState('')
  const [formModel, setFormModel] = useState('')
  const [formColor, setFormColor] = useState('')
  const [formYear, setFormYear] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    const t = setTimeout(() => setSearchDebounced(search), 300)
    return () => clearTimeout(t)
  }, [search])

  const load = useCallback(() => {
    setLoading(true)
    vehiclesApi.getAll(page, PAGE_SIZE, searchDebounced || undefined)
      .then((res) => {
        const d = res.data
        setItems(d.items ?? [])
        setTotalCount(d.totalCount ?? 0)
        setTotalPages(d.totalPages ?? 0)
      })
      .catch(() => showError('Araçlar yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, searchDebounced])

  useEffect(() => { load() }, [load])

  const handleSearchChange = (val: string) => { setSearch(val); setPage(1) }

  const openView = (item: VehicleSummaryDto, index: number) => {
    setPanelMode('view')
    setSelected(item)
    setPanelIndex(index)
    setPanelOpen(true)
  }

  const openCreate = () => {
    setSelected(null)
    setFormUserId('')
    setFormPlate('')
    setFormBrand('')
    setFormModel('')
    setFormColor('')
    setFormYear('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formUserId.trim()) { showError('Kullanıcı ID zorunludur.'); return }
    if (!formPlate.trim()) { showError('Plaka zorunludur.'); return }
    setSaving(true)
    try {
      const dto: CreateVehicleDto = {
        userId: formUserId,
        plate: formPlate,
        brand: formBrand || undefined,
        model: formModel || undefined,
        color: formColor || undefined,
        year: formYear ? parseInt(formYear) : undefined,
      }
      await vehiclesApi.create(dto)
      showSuccess('Araç eklendi.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu aracı silmek istediğinize emin misiniz?')) return
    try {
      await vehiclesApi.delete(id)
      showSuccess('Araç silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  const navigatePanel = (dir: -1 | 1) => {
    const next = panelIndex + dir
    if (next < 0 || next >= items.length) return
    openView(items[next], next)
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Araçlar</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Araç Ekle
        </Button>
      </div>

      <div className="mb-3">
        <PaginationControls
          page={page} pageSize={PAGE_SIZE} totalCount={totalCount} totalPages={totalPages}
          search={search} onPageChange={setPage} onSearchChange={handleSearchChange}
          searchPlaceholder="Plaka veya marka ara..."
        />
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Plaka</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Marka / Model</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Renk</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={4}>
                <div className="flex flex-col items-center py-12">
                  <Car className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz araç kaydı bulunmuyor.</p>
                </div>
              </td></tr>
            ) : (
              items.map((item, i) => (
                <tr
                  key={item.id}
                  onClick={() => openView(item, i)}
                  className={`border-b last:border-0 cursor-pointer transition-colors ${
                    selected?.id === item.id && panelOpen ? 'bg-accent' : 'hover:bg-muted/30'
                  }`}
                >
                  <td className="px-3 py-2.5 font-medium font-mono">{item.plate}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">
                    {[item.brand, item.model].filter(Boolean).join(' ') || '-'}
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground">{item.color ?? '-'}</td>
                  <td className="px-3 py-2.5">
                    <Badge variant={item.isActive ? 'default' : 'secondary'}>
                      {item.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[480px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <div className="flex items-center gap-2">
              {panelMode === 'view' && (
                <>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(-1)} disabled={panelIndex === 0}>
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <span className="text-xs text-muted-foreground">{panelIndex + 1} / {items.length}</span>
                  <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => navigatePanel(1)} disabled={panelIndex >= items.length - 1}>
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </>
              )}
              <h2 className="text-sm font-semibold ml-1">
                {panelMode === 'create' ? 'Araç Ekle' : selected?.plate}
              </h2>
            </div>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'create' ? (
              <>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Kullanıcı ID <span className="text-destructive">*</span></Label>
                  <Input value={formUserId} onChange={(e) => setFormUserId(e.target.value)} placeholder="Kullanıcı UUID" />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Plaka <span className="text-destructive">*</span></Label>
                  <Input value={formPlate} onChange={(e) => setFormPlate(e.target.value.toUpperCase())} placeholder="34 ABC 123" />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Marka</Label>
                    <Input value={formBrand} onChange={(e) => setFormBrand(e.target.value)} placeholder="Toyota" />
                  </div>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Model</Label>
                    <Input value={formModel} onChange={(e) => setFormModel(e.target.value)} placeholder="Corolla" />
                  </div>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Renk</Label>
                    <Input value={formColor} onChange={(e) => setFormColor(e.target.value)} placeholder="Beyaz" />
                  </div>
                  <div className="space-y-1">
                    <Label className="text-xs font-medium">Yıl</Label>
                    <Input type="number" value={formYear} onChange={(e) => setFormYear(e.target.value)} placeholder="2020" min="1900" max="2100" />
                  </div>
                </div>
                <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                  {saving ? 'Ekleniyor...' : 'Ekle'}
                </Button>
              </>
            ) : selected ? (
              <>
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Plaka</p>
                    <p className="font-medium font-mono text-base">{selected.plate}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Durum</p>
                    <Badge variant={selected.isActive ? 'default' : 'secondary'}>
                      {selected.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </div>
                  {selected.brand && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Marka</p>
                      <p>{selected.brand}</p>
                    </div>
                  )}
                  {selected.model && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Model</p>
                      <p>{selected.model}</p>
                    </div>
                  )}
                  {selected.color && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Renk</p>
                      <p>{selected.color}</p>
                    </div>
                  )}
                  {selected.year && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Yıl</p>
                      <p>{selected.year}</p>
                    </div>
                  )}
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Eklenme</p>
                    <p>{new Date(selected.createdAt).toLocaleDateString('tr-TR')}</p>
                  </div>
                </div>

                <div className="pt-2 border-t">
                  <Button size="sm" variant="destructive" onClick={() => handleDelete(selected.id)}>
                    Sil
                  </Button>
                </div>
              </>
            ) : null}
          </div>
        </div>
      )}
    </div>
  )
}
