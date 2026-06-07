'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, HelpCircle, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { PaginationControls } from '@/components/ui/pagination-controls'
import { supportRequestsApi } from '@/lib/api/supportRequests'
import { showSuccess, showError } from '@/lib/toast'
import {
  SupportRequestSummaryDto,
  SupportRequestDetailDto,
  SupportRequestStatus,
  SupportRequestStatusLabel,
  SupportRequestStatusColor,
} from '@/types/supportRequest'

const PAGE_SIZE = 20

export default function SupportRequestsPage() {
  const [items, setItems] = useState<SupportRequestSummaryDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [searchDebounced, setSearchDebounced] = useState('')
  const [loading, setLoading] = useState(true)

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create'>('view')
  const [selected, setSelected] = useState<SupportRequestSummaryDto | null>(null)
  const [detail, setDetail] = useState<SupportRequestDetailDto | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formUnitId, setFormUnitId] = useState('')
  const [formSubject, setFormSubject] = useState('')
  const [formDescription, setFormDescription] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    const t = setTimeout(() => setSearchDebounced(search), 300)
    return () => clearTimeout(t)
  }, [search])

  const load = useCallback((p = page) => {
    setLoading(true)
    supportRequestsApi.getAll(p, PAGE_SIZE, searchDebounced || undefined)
      .then((res) => {
        const d = res.data
        setItems(d.items ?? [])
        setTotalCount(d.totalCount ?? 0)
        setTotalPages(d.totalPages ?? 0)
      })
      .catch(() => showError('Destek talepleri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, searchDebounced])

  useEffect(() => { load() }, [load])

  const handleSearchChange = (val: string) => {
    setSearch(val)
    setPage(1)
  }

  const openView = (item: SupportRequestSummaryDto, index: number) => {
    setPanelMode('view')
    setSelected(item)
    setPanelIndex(index)
    setDetail(null)
    setPanelOpen(true)
    setDetailLoading(true)
    supportRequestsApi.getById(item.id)
      .then((res) => setDetail(res.data))
      .catch(() => {})
      .finally(() => setDetailLoading(false))
  }

  const openCreate = () => {
    setSelected(null)
    setFormUnitId('')
    setFormSubject('')
    setFormDescription('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const handleCreate = async () => {
    if (!formSubject.trim()) { showError('Konu zorunludur.'); return }
    if (!formUnitId.trim()) { showError('Daire ID zorunludur.'); return }
    setSaving(true)
    try {
      await supportRequestsApi.create({ unitId: formUnitId, subject: formSubject, description: formDescription || undefined })
      showSuccess('Destek talebi oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleStatusChange = async (status: SupportRequestStatus) => {
    if (!selected) return
    try {
      await supportRequestsApi.updateStatus(selected.id, { status })
      showSuccess('Durum güncellendi.')
      setItems(prev => prev.map(i => i.id === selected.id ? { ...i, status } : i))
      if (detail) setDetail({ ...detail, status })
      setSelected({ ...selected, status })
    } catch { showError('Durum güncellenemedi.') }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu talebi silmek istediğinize emin misiniz?')) return
    try {
      await supportRequestsApi.delete(id)
      showSuccess('Talep silindi.')
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
        <h1 className="text-xl font-semibold">Destek Talepleri</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Talep
        </Button>
      </div>

      <div className="mb-3">
        <PaginationControls
          page={page} pageSize={PAGE_SIZE} totalCount={totalCount} totalPages={totalPages}
          search={search} onPageChange={setPage} onSearchChange={handleSearchChange}
          searchPlaceholder="Konu ara..."
        />
      </div>

      <div className="border rounded-lg overflow-hidden">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Konu</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Daire</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tarih</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={4}>
                <div className="flex flex-col items-center py-12">
                  <HelpCircle className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz destek talebi bulunmuyor.</p>
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
                  <td className="px-3 py-2.5 font-medium max-w-[240px] truncate">{item.subject}</td>
                  <td className="px-3 py-2.5 text-muted-foreground">{item.unitDoorNumber ?? '-'}</td>
                  <td className="px-3 py-2.5">
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${SupportRequestStatusColor[item.status]}`}>
                      {SupportRequestStatusLabel[item.status]}
                    </span>
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground">
                    {new Date(item.createdAt).toLocaleDateString('tr-TR')}
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
                {panelMode === 'create' ? 'Yeni Destek Talebi' : (selected?.subject ?? 'Detay')}
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
                  <Label className="text-xs font-medium">Daire ID <span className="text-destructive">*</span></Label>
                  <Input value={formUnitId} onChange={(e) => setFormUnitId(e.target.value)} placeholder="Daire UUID" />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Konu <span className="text-destructive">*</span></Label>
                  <Input value={formSubject} onChange={(e) => setFormSubject(e.target.value)} placeholder="Talep konusu" />
                </div>
                <div className="space-y-1">
                  <Label className="text-xs font-medium">Açıklama</Label>
                  <textarea
                    value={formDescription}
                    onChange={(e) => setFormDescription(e.target.value)}
                    placeholder="Detaylı açıklama..."
                    rows={4}
                    className="w-full border rounded-md px-3 py-2 text-sm bg-background resize-none focus:outline-none focus:ring-2 focus:ring-ring"
                  />
                </div>
                <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Oluştur'}
                </Button>
              </>
            ) : detailLoading ? (
              <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
            ) : detail ? (
              <>
                <div className="grid grid-cols-2 gap-3 text-sm">
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Daire</p>
                    <p className="font-medium">{detail.unitDoorNumber ?? '-'}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Durum</p>
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${SupportRequestStatusColor[detail.status]}`}>
                      {SupportRequestStatusLabel[detail.status]}
                    </span>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground mb-0.5">Oluşturulma</p>
                    <p>{new Date(detail.createdAt).toLocaleDateString('tr-TR')}</p>
                  </div>
                  {detail.resolvedAt && (
                    <div>
                      <p className="text-xs text-muted-foreground mb-0.5">Çözüldü</p>
                      <p>{new Date(detail.resolvedAt).toLocaleDateString('tr-TR')}</p>
                    </div>
                  )}
                </div>

                {detail.description && (
                  <div className="border rounded-md p-3 bg-muted/30 text-sm">
                    <p className="text-xs text-muted-foreground mb-1">Açıklama</p>
                    <p>{detail.description}</p>
                  </div>
                )}

                <div className="space-y-2">
                  <p className="text-xs font-medium text-muted-foreground">DURUM DEĞİŞTİR</p>
                  <div className="flex gap-2 flex-wrap">
                    {([0, 1, 2] as SupportRequestStatus[]).map((s) => (
                      <Button
                        key={s}
                        size="sm"
                        variant={detail.status === s ? 'default' : 'outline'}
                        onClick={() => handleStatusChange(s)}
                        className="text-xs"
                      >
                        {SupportRequestStatusLabel[s]}
                      </Button>
                    ))}
                  </div>
                </div>

                <div className="pt-2 border-t">
                  <Button size="sm" variant="destructive" onClick={() => handleDelete(detail.id)}>
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
