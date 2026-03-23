'use client'

import { useEffect, useState, useCallback } from 'react'
import {
  Plus,
  SlidersHorizontal,
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  X,
  Settings,
  Building2,
  AlertCircle,
  FileSpreadsheet,
} from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { blocksApi } from '@/lib/api/blocks'
import { Block, BlockUnit, CreateBlockDto, UpdateBlockDto } from '@/types/block'

const ALPHA_LETTERS = [
  'Tümü', '#',
  'A', 'B', 'C', 'Ç', 'D', 'E', 'F', 'G', 'Ğ', 'H',
  'I', 'İ', 'J', 'K', 'L', 'M', 'N', 'O', 'Ö', 'P',
  'R', 'S', 'Ş', 'T', 'U', 'Ü', 'V', 'Y', 'Z',
]

type SortKey = 'code' | 'name' | 'ada' | 'unitCount' | 'description'
type SortDir = 'asc' | 'desc'

const PAGE_SIZE = 20

const COLUMNS: { key: SortKey; label: string }[] = [
  { key: 'code', label: 'Kodu' },
  { key: 'name', label: 'Adı' },
  { key: 'ada', label: 'Ada' },
  { key: 'unitCount', label: 'Daire Sayısı' },
  { key: 'description', label: 'Açıklama' },
]

export default function BuildingsPage() {
  const [blocks, setBlocks] = useState<Block[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set())
  const [alphaFilter, setAlphaFilter] = useState('Tümü')
  const [sortKey, setSortKey] = useState<SortKey>('name')
  const [sortDir, setSortDir] = useState<SortDir>('asc')

  // Panel state
  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'create' | 'edit'>('create')
  const [selectedBlock, setSelectedBlock] = useState<Block | null>(null)
  const [panelIndex, setPanelIndex] = useState(0)
  const [blockUnits, setBlockUnits] = useState<BlockUnit[]>([])
  const [unitsLoading, setUnitsLoading] = useState(false)

  // Form state
  const [formCode, setFormCode] = useState('')
  const [formName, setFormName] = useState('')
  const [formAda, setFormAda] = useState('')
  const [formAidat, setFormAidat] = useState('0')
  const [formErrors, setFormErrors] = useState<{ code?: string; name?: string }>({})
  const [saving, setSaving] = useState(false)

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE))

  const load = useCallback(() => {
    setLoading(true)
    setError(null)
    blocksApi
      .getAll({
        page,
        pageSize: PAGE_SIZE,
        startsWith: alphaFilter === 'Tümü' ? undefined : alphaFilter,
        sortBy: sortKey,
        sortDesc: sortDir === 'desc',
      })
      .then((res) => {
        const data = res.data.value
        if (data) {
          setBlocks(data.items)
          setTotalCount(data.totalCount)
        } else {
          setBlocks([])
          setTotalCount(0)
        }
      })
      .catch(() => setError('Bloklar yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, alphaFilter, sortKey, sortDir])

  useEffect(() => {
    load()
  }, [load])

  const handleAlphaFilter = (letter: string) => {
    setAlphaFilter(letter)
    setPage(1)
  }

  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDir((d) => (d === 'asc' ? 'desc' : 'asc'))
    } else {
      setSortKey(key)
      setSortDir('asc')
    }
    setPage(1)
  }

  const toggleSelectAll = () => {
    if (selectedIds.size === blocks.length && blocks.length > 0) {
      setSelectedIds(new Set())
    } else {
      setSelectedIds(new Set(blocks.map((b) => b.id)))
    }
  }

  const toggleSelect = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev)
      if (next.has(id)) next.delete(id)
      else next.add(id)
      return next
    })
  }

  const resetForm = () => {
    setFormCode('')
    setFormName('')
    setFormAda('')
    setFormAidat('0')
    setFormErrors({})
    setBlockUnits([])
  }

  const openCreate = () => {
    setPanelMode('create')
    setSelectedBlock(null)
    resetForm()
    setPanelOpen(true)
  }

  const loadUnits = (blockId: string) => {
    setUnitsLoading(true)
    setBlockUnits([])
    blocksApi
      .getUnits(blockId)
      .then((res) => setBlockUnits(res.data.value ?? []))
      .catch(() => {})
      .finally(() => setUnitsLoading(false))
  }

  const openEdit = (block: Block, index: number) => {
    setPanelMode('edit')
    setSelectedBlock(block)
    setPanelIndex(index)
    setFormCode(block.code ?? '')
    setFormName(block.name ?? '')
    setFormAda(block.ada ?? '')
    setFormAidat(block.aidat?.toString() ?? '0')
    setFormErrors({})
    setPanelOpen(true)
    loadUnits(block.id)
  }

  const navigatePanel = (dir: 'prev' | 'next') => {
    const newIndex = dir === 'prev' ? panelIndex - 1 : panelIndex + 1
    if (newIndex >= 0 && newIndex < blocks.length) {
      openEdit(blocks[newIndex], newIndex)
    }
  }

  const validateForm = () => {
    const errors: { code?: string; name?: string } = {}
    if (!formCode.trim()) errors.code = 'Blok Kodu zorunludur'
    if (!formName.trim()) errors.name = 'Blok Adı zorunludur'
    setFormErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSave = async () => {
    if (!validateForm()) return
    setSaving(true)
    const dto: CreateBlockDto | UpdateBlockDto = {
      code: formCode.trim(),
      name: formName.trim(),
      ada: formAda.trim() || undefined,
      aidat: parseFloat(formAidat) || 0,
    }
    try {
      if (panelMode === 'create') {
        await blocksApi.create(dto as CreateBlockDto)
      } else if (selectedBlock) {
        await blocksApi.update(selectedBlock.id, dto as UpdateBlockDto)
      }
      setPanelOpen(false)
      load()
    } catch {
      setError('Kaydetme işlemi başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu bloğu silmek istediğinize emin misiniz?')) return
    try {
      await blocksApi.delete(id)
      if (selectedBlock?.id === id) setPanelOpen(false)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  const handleExport = async () => {
    const XLSX = await import('xlsx')
    const ws = XLSX.utils.json_to_sheet(
      blocks.map((b) => ({
        Kodu: b.code ?? '',
        Adı: b.name,
        Ada: b.ada ?? '',
        'Daire Sayısı': b.unitCount ?? 0,
        Açıklama: b.description ?? '',
      }))
    )
    const wb = XLSX.utils.book_new()
    XLSX.utils.book_append_sheet(wb, ws, 'Bloklar')
    XLSX.writeFile(wb, 'bloklar.xlsx')
  }

  const handleExportUnits = async () => {
    if (blockUnits.length === 0) return
    const XLSX = await import('xlsx')
    const ws = XLSX.utils.json_to_sheet(
      blockUnits.map((u) => ({
        'Daire No': u.number,
        Kullanımda: u.isOccupied ? 'Evet' : 'Hayır',
        'Arsa Payı': u.landShare ?? '',
        Sahibi: u.ownerName ?? '',
        Kiracı: u.renterName ?? '',
      }))
    )
    const wb = XLSX.utils.book_new()
    XLSX.utils.book_append_sheet(wb, ws, 'Daireler')
    XLSX.writeFile(wb, `${selectedBlock?.name ?? 'blok'}-daireler.xlsx`)
  }

  const SortIndicator = ({ col }: { col: SortKey }) => {
    if (sortKey !== col) return <span className="ml-1 text-muted-foreground/40">⇅</span>
    return <span className="ml-1">{sortDir === 'asc' ? '↑' : '↓'}</span>
  }

  return (
    <div className="flex flex-col h-full">
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Bloklar</h1>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="icon" className="h-8 w-8" onClick={handleExport} title="Excel olarak indir">
            <FileSpreadsheet className="h-4 w-4" />
          </Button>
          <Button variant="outline" size="icon" className="h-8 w-8" title="Filtre ve sıralama">
            <SlidersHorizontal className="h-4 w-4" />
          </Button>
          <Button size="sm" onClick={openCreate}>
            <Plus className="h-4 w-4 mr-1" />
            Yeni Ekle
          </Button>
        </div>
      </div>

      {/* Tab bar */}
      <div className="flex items-center border-b mb-3">
        <button className="px-4 py-2 text-sm font-medium border-b-2 border-primary text-primary -mb-px">
          Hepsi
        </button>
      </div>

      {/* Alphabetic filter */}
      <div className="flex flex-wrap gap-0.5 mb-3">
        {ALPHA_LETTERS.map((letter) => (
          <button
            key={letter}
            onClick={() => handleAlphaFilter(letter)}
            className={`px-2 py-0.5 text-xs rounded transition-colors ${
              alphaFilter === letter
                ? 'bg-primary text-primary-foreground'
                : 'text-muted-foreground hover:bg-muted'
            }`}
          >
            {letter}
          </button>
        ))}
      </div>

      {error && (
        <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3 mb-3">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      {/* Table */}
      <div className="border rounded-lg overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="px-3 py-2.5 w-10">
                  <input
                    type="checkbox"
                    checked={blocks.length > 0 && selectedIds.size === blocks.length}
                    onChange={toggleSelectAll}
                    className="rounded cursor-pointer"
                  />
                </th>
                {COLUMNS.map((col) => (
                  <th
                    key={col.key}
                    onClick={() => handleSort(col.key)}
                    className="text-left px-3 py-2.5 font-medium text-muted-foreground cursor-pointer select-none hover:text-foreground whitespace-nowrap"
                  >
                    {col.label}
                    <SortIndicator col={col.key} />
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-12 text-muted-foreground">
                    Yükleniyor...
                  </td>
                </tr>
              ) : blocks.length === 0 ? (
                <tr>
                  <td colSpan={6}>
                    <div className="flex flex-col items-center justify-center py-12 text-center">
                      <Building2 className="h-10 w-10 text-muted-foreground/50 mb-3" />
                      <p className="text-muted-foreground">Henüz blok eklenmemiş</p>
                    </div>
                  </td>
                </tr>
              ) : (
                blocks.map((block, index) => (
                  <tr
                    key={block.id}
                    onClick={() => openEdit(block, index)}
                    className={`border-b last:border-0 cursor-pointer transition-colors ${
                      selectedBlock?.id === block.id && panelOpen
                        ? 'bg-accent'
                        : 'hover:bg-muted/30'
                    }`}
                  >
                    <td
                      className="px-3 py-2.5"
                      onClick={(e) => e.stopPropagation()}
                    >
                      <input
                        type="checkbox"
                        checked={selectedIds.has(block.id)}
                        onChange={() => toggleSelect(block.id)}
                        className="rounded cursor-pointer"
                      />
                    </td>
                    <td className="px-3 py-2.5 font-medium">{block.code ?? '-'}</td>
                    <td className="px-3 py-2.5">{block.name}</td>
                    <td className="px-3 py-2.5 text-muted-foreground">{block.ada ?? '-'}</td>
                    <td className="px-3 py-2.5 text-muted-foreground">{block.unitCount ?? 0}</td>
                    <td className="px-3 py-2.5 text-muted-foreground">{block.description ?? '-'}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-end gap-1 mt-3 text-sm text-muted-foreground">
        <button
          onClick={() => setPage(1)}
          disabled={page === 1}
          className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
          title="İlk sayfa"
        >
          <ChevronsLeft className="h-4 w-4" />
        </button>
        <button
          onClick={() => setPage((p) => Math.max(1, p - 1))}
          disabled={page === 1}
          className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
          title="Önceki sayfa"
        >
          <ChevronLeft className="h-4 w-4" />
        </button>
        <span className="px-2 tabular-nums">
          {page} / {totalPages} ({totalCount} Kayıt)
        </span>
        <button
          onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
          disabled={page === totalPages}
          className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
          title="Sonraki sayfa"
        >
          <ChevronRight className="h-4 w-4" />
        </button>
        <button
          onClick={() => setPage(totalPages)}
          disabled={page === totalPages}
          className="p-1 rounded hover:bg-muted disabled:opacity-40 disabled:cursor-not-allowed"
          title="Son sayfa"
        >
          <ChevronsRight className="h-4 w-4" />
        </button>
      </div>

      {/* Detail / Edit Panel */}
      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          {/* Panel header */}
          <div className="flex items-center justify-between px-4 py-3 border-b shrink-0">
            <h2 className="text-sm font-semibold">Bloklar</h2>
            <div className="flex items-center gap-0.5">
              <Button variant="ghost" size="icon" className="h-7 w-7" title="Ayarlar">
                <Settings className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                disabled={panelMode === 'create' || panelIndex <= 0}
                onClick={() => navigatePanel('prev')}
                title="Önceki kayıt"
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                disabled={panelMode === 'create' || panelIndex >= blocks.length - 1}
                onClick={() => navigatePanel('next')}
                title="Sonraki kayıt"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                className="h-7 w-7"
                onClick={() => setPanelOpen(false)}
                title="Kapat"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </div>

          {/* Form */}
          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {/* Blok Kodu */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">
                Blok Kodu <span className="text-destructive">*</span>
              </Label>
              <div className="relative">
                <Input
                  value={formCode}
                  onChange={(e) => {
                    setFormCode(e.target.value)
                    if (formErrors.code) setFormErrors((prev) => ({ ...prev, code: undefined }))
                  }}
                  className={formErrors.code ? 'border-destructive pr-8' : ''}
                  placeholder="Blok kodu girin"
                />
                {formErrors.code && (
                  <AlertCircle className="h-4 w-4 text-destructive absolute right-2 top-1/2 -translate-y-1/2" />
                )}
              </div>
              {formErrors.code && <p className="text-xs text-destructive">{formErrors.code}</p>}
            </div>

            {/* Blok Adı */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">
                Blok Adı <span className="text-destructive">*</span>
              </Label>
              <div className="relative">
                <Input
                  value={formName}
                  onChange={(e) => {
                    setFormName(e.target.value)
                    if (formErrors.name) setFormErrors((prev) => ({ ...prev, name: undefined }))
                  }}
                  className={formErrors.name ? 'border-destructive pr-8' : ''}
                  placeholder="Blok adı girin"
                />
                {formErrors.name && (
                  <AlertCircle className="h-4 w-4 text-destructive absolute right-2 top-1/2 -translate-y-1/2" />
                )}
              </div>
              {formErrors.name && <p className="text-xs text-destructive">{formErrors.name}</p>}
            </div>

            {/* Bulunduğu Ada */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">Bulunduğu Ada</Label>
              <Input
                value={formAda}
                onChange={(e) => setFormAda(e.target.value)}
                placeholder="Ada girin"
              />
            </div>

            {/* Aidat */}
            <div className="space-y-1">
              <Label className="text-xs font-medium">Aidat</Label>
              <Input
                type="number"
                value={formAidat}
                onChange={(e) => setFormAidat(e.target.value)}
                step="0.01"
                min="0"
                placeholder="0,00"
              />
            </div>

            {/* Actions */}
            <div className="flex gap-2 pt-1">
              <Button size="sm" onClick={handleSave} disabled={saving} className="flex-1">
                {saving ? 'Kaydediliyor...' : 'Kaydet'}
              </Button>
              {panelMode === 'edit' && selectedBlock && (
                <Button
                  size="sm"
                  variant="destructive"
                  onClick={() => handleDelete(selectedBlock.id)}
                >
                  Sil
                </Button>
              )}
            </div>

            {/* Blokdaki Daireler */}
            {panelMode === 'edit' && (
              <div className="pt-2">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="text-sm font-semibold">Blokdaki Daireler</h3>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-7 w-7"
                    onClick={handleExportUnits}
                    title="XLS olarak indir"
                  >
                    <FileSpreadsheet className="h-4 w-4" />
                  </Button>
                </div>
                <div className="border rounded-lg overflow-hidden">
                  <table className="w-full text-xs">
                    <thead>
                      <tr className="border-b bg-muted/50">
                        <th className="text-left px-2 py-2 font-medium text-muted-foreground">Daire No</th>
                        <th className="text-left px-2 py-2 font-medium text-muted-foreground">Kullanımda</th>
                        <th className="text-left px-2 py-2 font-medium text-muted-foreground">Arsa Payı</th>
                        <th className="text-left px-2 py-2 font-medium text-muted-foreground">Sahibi</th>
                        <th className="text-left px-2 py-2 font-medium text-muted-foreground">Kiracı</th>
                      </tr>
                    </thead>
                    <tbody>
                      {unitsLoading ? (
                        <tr>
                          <td colSpan={5} className="text-center py-4 text-muted-foreground">
                            Yükleniyor...
                          </td>
                        </tr>
                      ) : blockUnits.length === 0 ? (
                        <tr>
                          <td colSpan={5} className="text-center py-4 text-muted-foreground">
                            Daire bulunamadı
                          </td>
                        </tr>
                      ) : (
                        blockUnits.map((unit) => (
                          <tr key={unit.id} className="border-b last:border-0">
                            <td className="px-2 py-2 font-medium">{unit.number}</td>
                            <td className="px-2 py-2">
                              <span
                                className={`inline-block px-1.5 py-0.5 rounded text-xs font-medium ${
                                  unit.isOccupied
                                    ? 'bg-green-100 text-green-700'
                                    : 'bg-muted text-muted-foreground'
                                }`}
                              >
                                {unit.isOccupied ? 'Kullanımda' : 'Boş'}
                              </span>
                            </td>
                            <td className="px-2 py-2 text-muted-foreground">{unit.landShare ?? '-'}</td>
                            <td className="px-2 py-2 text-muted-foreground">{unit.ownerName ?? '-'}</td>
                            <td className="px-2 py-2 text-muted-foreground">{unit.renterName ?? '-'}</td>
                          </tr>
                        ))
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
