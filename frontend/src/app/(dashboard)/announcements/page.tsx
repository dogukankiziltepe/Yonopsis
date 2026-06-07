'use client'

import { useEffect, useState, useCallback } from 'react'
import { Plus, X, Bell, Pin, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { PaginationControls } from '@/components/ui/pagination-controls'
import { announcementsApi } from '@/lib/api/announcements'
import { showSuccess, showError } from '@/lib/toast'
import {
  AnnouncementSummaryDto,
  AnnouncementDetailDto,
  CreateAnnouncementDto,
} from '@/types/announcement'

const PAGE_SIZE = 20

export default function AnnouncementsPage() {
  const [items, setItems] = useState<AnnouncementSummaryDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [totalPages, setTotalPages] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchDebounced, setSearchDebounced] = useState('')

  const [panelOpen, setPanelOpen] = useState(false)
  const [panelMode, setPanelMode] = useState<'view' | 'create' | 'edit'>('view')
  const [selected, setSelected] = useState<AnnouncementSummaryDto | null>(null)
  const [detail, setDetail] = useState<AnnouncementDetailDto | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [panelIndex, setPanelIndex] = useState(0)

  const [formTitle, setFormTitle] = useState('')
  const [formContent, setFormContent] = useState('')
  const [formIsPinned, setFormIsPinned] = useState(false)
  const [formPublishDate, setFormPublishDate] = useState('')
  const [formExpiryDate, setFormExpiryDate] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    const t = setTimeout(() => setSearchDebounced(search), 300)
    return () => clearTimeout(t)
  }, [search])

  const load = useCallback(() => {
    setLoading(true)
    announcementsApi.getAll(page, PAGE_SIZE, searchDebounced || undefined)
      .then((res) => {
        const d = res.data
        setItems(d.items ?? [])
        setTotalCount(d.totalCount ?? 0)
        setTotalPages(d.totalPages ?? 0)
      })
      .catch(() => showError('Duyurular yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [page, searchDebounced])

  useEffect(() => { load() }, [load])

  const handleSearchChange = (val: string) => { setSearch(val); setPage(1) }

  const openView = (item: AnnouncementSummaryDto, index: number) => {
    setPanelMode('view')
    setSelected(item)
    setPanelIndex(index)
    setDetail(null)
    setPanelOpen(true)
    setDetailLoading(true)
    announcementsApi.getById(item.id)
      .then((res) => setDetail(res.data))
      .catch(() => {})
      .finally(() => setDetailLoading(false))
  }

  const openCreate = () => {
    setSelected(null)
    setDetail(null)
    setFormTitle('')
    setFormContent('')
    setFormIsPinned(false)
    setFormPublishDate('')
    setFormExpiryDate('')
    setPanelMode('create')
    setPanelOpen(true)
  }

  const openEdit = (d: AnnouncementDetailDto) => {
    setFormTitle(d.title)
    setFormContent(d.content)
    setFormIsPinned(d.isPinned)
    setFormPublishDate(d.publishDate ? d.publishDate.split('T')[0] : '')
    setFormExpiryDate(d.expiryDate ? d.expiryDate.split('T')[0] : '')
    setPanelMode('edit')
  }

  const handleCreate = async () => {
    if (!formTitle.trim()) { showError('Başlık zorunludur.'); return }
    if (!formContent.trim()) { showError('İçerik zorunludur.'); return }
    setSaving(true)
    try {
      const dto: CreateAnnouncementDto = {
        title: formTitle,
        content: formContent,
        isPinned: formIsPinned,
        publishDate: formPublishDate ? new Date(formPublishDate).toISOString() : undefined,
        expiryDate: formExpiryDate ? new Date(formExpiryDate).toISOString() : undefined,
      }
      await announcementsApi.create(dto)
      showSuccess('Duyuru oluşturuldu.')
      setPanelOpen(false)
      load()
    } catch {} finally { setSaving(false) }
  }

  const handleUpdate = async () => {
    if (!selected || !formTitle.trim() || !formContent.trim()) return
    setSaving(true)
    try {
      await announcementsApi.update(selected.id, {
        title: formTitle,
        content: formContent,
        isPinned: formIsPinned,
        publishDate: formPublishDate ? new Date(formPublishDate).toISOString() : undefined,
        expiryDate: formExpiryDate ? new Date(formExpiryDate).toISOString() : undefined,
      })
      showSuccess('Duyuru güncellendi.')
      load()
      setPanelOpen(false)
    } catch {} finally { setSaving(false) }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Bu duyuruyu silmek istediğinize emin misiniz?')) return
    try {
      await announcementsApi.delete(id)
      showSuccess('Duyuru silindi.')
      setPanelOpen(false)
      load()
    } catch {}
  }

  const navigatePanel = (dir: -1 | 1) => {
    const next = panelIndex + dir
    if (next < 0 || next >= items.length) return
    openView(items[next], next)
  }

  const FormFields = () => (
    <>
      <div className="space-y-1">
        <Label className="text-xs font-medium">Başlık <span className="text-destructive">*</span></Label>
        <Input value={formTitle} onChange={(e) => setFormTitle(e.target.value)} placeholder="Duyuru başlığı" />
      </div>
      <div className="space-y-1">
        <Label className="text-xs font-medium">İçerik <span className="text-destructive">*</span></Label>
        <textarea
          value={formContent}
          onChange={(e) => setFormContent(e.target.value)}
          placeholder="Duyuru içeriği..."
          rows={6}
          className="w-full border rounded-md px-3 py-2 text-sm bg-background resize-none focus:outline-none focus:ring-2 focus:ring-ring"
        />
      </div>
      <div className="flex items-center gap-2">
        <input
          type="checkbox"
          id="isPinned"
          checked={formIsPinned}
          onChange={(e) => setFormIsPinned(e.target.checked)}
          className="rounded"
        />
        <Label htmlFor="isPinned" className="text-xs font-medium cursor-pointer">Sabitle</Label>
      </div>
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Yayın Tarihi</Label>
          <Input type="date" value={formPublishDate} onChange={(e) => setFormPublishDate(e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Bitiş Tarihi</Label>
          <Input type="date" value={formExpiryDate} onChange={(e) => setFormExpiryDate(e.target.value)} />
        </div>
      </div>
    </>
  )

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Duyurular</h1>
        <Button size="sm" onClick={openCreate}>
          <Plus className="h-4 w-4 mr-1" />
          Yeni Duyuru
        </Button>
      </div>

      <div className="mb-3">
        <PaginationControls
          page={page} pageSize={PAGE_SIZE} totalCount={totalCount} totalPages={totalPages}
          search={search} onPageChange={setPage} onSearchChange={handleSearchChange}
          searchPlaceholder="Başlık ara..."
        />
      </div>

      <div className="border rounded-lg overflow-hidden overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Başlık</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Yayın</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Bitiş</th>
              <th className="text-left px-3 py-2.5 font-medium text-muted-foreground"></th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={4} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr><td colSpan={4}>
                <div className="flex flex-col items-center py-12">
                  <Bell className="h-10 w-10 text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground">Henüz duyuru bulunmuyor.</p>
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
                  <td className="px-3 py-2.5">
                    <div className="flex items-center gap-1.5">
                      {item.isPinned && <Pin className="h-3.5 w-3.5 text-muted-foreground shrink-0" />}
                      <span className="font-medium truncate max-w-[200px]">{item.title}</span>
                    </div>
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground">
                    {item.publishDate ? new Date(item.publishDate).toLocaleDateString('tr-TR') : '-'}
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground">
                    {item.expiryDate ? new Date(item.expiryDate).toLocaleDateString('tr-TR') : 'Süresiz'}
                  </td>
                  <td className="px-3 py-2.5">
                    {item.isPinned && <Badge variant="secondary" className="text-xs">Sabit</Badge>}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {panelOpen && (
        <div className="fixed right-0 top-0 h-screen w-[520px] bg-background border-l shadow-2xl flex flex-col z-50">
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
                {panelMode === 'create' ? 'Yeni Duyuru' :
                 panelMode === 'edit' ? 'Duyuru Düzenle' :
                 selected?.title ?? 'Detay'}
              </h2>
            </div>
            <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setPanelOpen(false)}>
              <X className="h-4 w-4" />
            </Button>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {panelMode === 'create' ? (
              <>
                <FormFields />
                <Button size="sm" className="w-full" onClick={handleCreate} disabled={saving}>
                  {saving ? 'Oluşturuluyor...' : 'Oluştur'}
                </Button>
              </>
            ) : panelMode === 'edit' ? (
              <>
                <FormFields />
                <div className="flex gap-2">
                  <Button size="sm" className="flex-1" onClick={handleUpdate} disabled={saving}>
                    {saving ? 'Kaydediliyor...' : 'Kaydet'}
                  </Button>
                  <Button size="sm" variant="outline" onClick={() => detail && openView(selected!, panelIndex)}>
                    İptal
                  </Button>
                </div>
              </>
            ) : detailLoading ? (
              <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
            ) : detail ? (
              <>
                <div className="space-y-1">
                  <div className="flex items-center gap-2 flex-wrap">
                    {detail.isPinned && <Badge variant="secondary"><Pin className="h-3 w-3 mr-1" />Sabit</Badge>}
                    {detail.publishDate && (
                      <span className="text-xs text-muted-foreground">
                        Yayın: {new Date(detail.publishDate).toLocaleDateString('tr-TR')}
                      </span>
                    )}
                    {detail.expiryDate && (
                      <span className="text-xs text-muted-foreground">
                        Bitiş: {new Date(detail.expiryDate).toLocaleDateString('tr-TR')}
                      </span>
                    )}
                  </div>
                </div>

                <div className="border rounded-md p-3 bg-muted/30 text-sm whitespace-pre-wrap leading-relaxed">
                  {detail.content}
                </div>

                <div className="text-xs text-muted-foreground">
                  Oluşturulma: {new Date(detail.createdAt).toLocaleDateString('tr-TR')}
                  {detail.updatedAt && ` · Güncelleme: ${new Date(detail.updatedAt).toLocaleDateString('tr-TR')}`}
                </div>

                <div className="pt-2 border-t flex gap-2">
                  <Button size="sm" variant="outline" onClick={() => openEdit(detail)}>
                    Düzenle
                  </Button>
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
