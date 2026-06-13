'use client'

import { useCallback, useEffect, useMemo, useState } from 'react'
import { Plus, X, Trash2, ArrowLeft, CheckCircle2, Ban } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showSuccess, showError } from '@/lib/toast'
import { muhasebeApi } from '@/lib/api/muhasebe'
import {
  FisDurumu,
  FisListItem,
  FisTuru,
  HesapListItem,
  fisDurumuLabel,
  fisTuruLabel,
} from '@/types/muhasebe'

interface LineState {
  hesapId: string
  borc: string
  alacak: string
  aciklama: string
  belgeNo: string
}

const fisTuruOptions = Object.values(FisTuru).filter((v) => typeof v === 'number') as FisTuru[]

const emptyLine = (): LineState => ({ hesapId: '', borc: '', alacak: '', aciklama: '', belgeNo: '' })

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function today() {
  return new Date().toISOString().slice(0, 10)
}

export default function FislerPage() {
  const [mode, setMode] = useState<'list' | 'form'>('list')

  // Liste
  const [items, setItems] = useState<FisListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [durumFilter, setDurumFilter] = useState<FisDurumu | ''>('')
  const [search, setSearch] = useState('')

  // Form
  const [editId, setEditId] = useState<string | null>(null)
  const [readOnly, setReadOnly] = useState(false)
  const [durum, setDurum] = useState<FisDurumu>(FisDurumu.Taslak)
  const [fisNo, setFisNo] = useState('')
  const [fisTuru, setFisTuru] = useState<FisTuru>(FisTuru.Mahsup)
  const [fisTarihi, setFisTarihi] = useState(today())
  const [aciklama, setAciklama] = useState('')
  const [lines, setLines] = useState<LineState[]>([emptyLine(), emptyLine()])
  const [saving, setSaving] = useState(false)

  const [hesaplar, setHesaplar] = useState<HesapListItem[]>([])

  const loadList = useCallback(() => {
    setLoading(true)
    muhasebeApi.getFisler({
      durum: durumFilter === '' ? undefined : durumFilter,
      q: search.trim() || undefined,
      pageSize: 50,
    })
      .then((res) => setItems(res.data?.items ?? []))
      .catch(() => showError('Fişler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [durumFilter, search])

  const loadHesaplar = useCallback(() => {
    muhasebeApi.getHesaplar({ fisKesilebilir: true, aktif: true })
      .then((res) => setHesaplar(res.data ?? []))
      .catch(() => {})
  }, [])

  useEffect(() => { if (mode === 'list') loadList() }, [mode, loadList])
  useEffect(() => { loadHesaplar() }, [loadHesaplar])

  const totals = useMemo(() => {
    let borc = 0, alacak = 0
    for (const l of lines) {
      borc += parseFloat(l.borc) || 0
      alacak += parseFloat(l.alacak) || 0
    }
    return { borc, alacak, fark: borc - alacak }
  }, [lines])

  const dengeli = Math.abs(totals.fark) < 0.005 && totals.borc > 0

  const openNew = () => {
    setEditId(null)
    setReadOnly(false)
    setDurum(FisDurumu.Taslak)
    setFisNo('')
    setFisTuru(FisTuru.Mahsup)
    setFisTarihi(today())
    setAciklama('')
    setLines([emptyLine(), emptyLine()])
    setMode('form')
  }

  const openFis = async (id: string) => {
    try {
      const { data } = await muhasebeApi.getFis(id)
      setEditId(data.id)
      setDurum(data.durum)
      setReadOnly(data.durum !== FisDurumu.Taslak)
      setFisNo(data.fisNo)
      setFisTuru(data.fisTuru)
      setFisTarihi(data.fisTarihi.slice(0, 10))
      setAciklama(data.aciklama)
      setLines(
        data.detaylar.length > 0
          ? data.detaylar.map((d) => ({
              hesapId: d.hesapId,
              borc: d.borcTutar ? String(d.borcTutar) : '',
              alacak: d.alacakTutar ? String(d.alacakTutar) : '',
              aciklama: d.aciklama ?? '',
              belgeNo: d.belgeNo ?? '',
            }))
          : [emptyLine()]
      )
      setMode('form')
    } catch {
      showError('Fiş yüklenemedi.')
    }
  }

  const updateLine = (i: number, patch: Partial<LineState>) => {
    setLines((prev) => prev.map((l, idx) => (idx === i ? { ...l, ...patch } : l)))
  }
  const addLine = () => setLines((prev) => [...prev, emptyLine()])
  const removeLine = (i: number) => setLines((prev) => prev.filter((_, idx) => idx !== i))

  const buildPayload = () => ({
    fisTuru,
    fisTarihi,
    aciklama: aciklama.trim(),
    detaylar: lines
      .filter((l) => l.hesapId && ((parseFloat(l.borc) || 0) > 0 || (parseFloat(l.alacak) || 0) > 0))
      .map((l) => ({
        hesapId: l.hesapId,
        borcTutar: parseFloat(l.borc) || 0,
        alacakTutar: parseFloat(l.alacak) || 0,
        aciklama: l.aciklama.trim() || undefined,
        belgeNo: l.belgeNo.trim() || undefined,
      })),
  })

  // Kaydeder ve fiş id'sini döner (yoksa null).
  const saveDraft = async (): Promise<string | null> => {
    const payload = buildPayload()
    if (payload.detaylar.length === 0) {
      showError('En az bir geçerli satır girilmeli.')
      return null
    }
    setSaving(true)
    try {
      if (editId) {
        await muhasebeApi.updateFis(editId, payload)
        return editId
      }
      const { data } = await muhasebeApi.createFis(payload)
      setEditId(data.id)
      return data.id
    } catch {
      return null
    } finally {
      setSaving(false)
    }
  }

  const handleSave = async () => {
    const id = await saveDraft()
    if (id) {
      showSuccess('Fiş kaydedildi (taslak).')
      setMode('list')
    }
  }

  const handleOnayla = async () => {
    const id = await saveDraft()
    if (!id) return
    setSaving(true)
    try {
      await muhasebeApi.onaylaFis(id)
      showSuccess('Fiş onaylandı (entegre).')
      setMode('list')
    } catch {} finally { setSaving(false) }
  }

  const handleIptal = async () => {
    if (!editId) return
    if (!confirm('Onaylı fiş için ters kayıt (storno) üretilecek. Devam edilsin mi?')) return
    try {
      await muhasebeApi.iptalFis(editId)
      showSuccess('Fiş iptal edildi.')
      setMode('list')
    } catch {}
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Taslak fiş silinsin mi?')) return
    try {
      await muhasebeApi.deleteFis(id)
      showSuccess('Fiş silindi.')
      loadList()
    } catch {}
  }

  // ---- Liste görünümü ----
  if (mode === 'list') {
    return (
      <div className="flex flex-col h-full">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-xl font-semibold">Muhasebe Fişleri</h1>
          <Button size="sm" onClick={openNew}>
            <Plus className="h-4 w-4 mr-1" /> Yeni Fiş
          </Button>
        </div>

        <div className="flex flex-wrap items-center gap-2 mb-3">
          <select
            value={durumFilter}
            onChange={(e) => setDurumFilter(e.target.value === '' ? '' : (Number(e.target.value) as FisDurumu))}
            className="h-8 rounded-md border bg-background px-2 text-sm"
          >
            <option value="">Tüm Durumlar</option>
            <option value={FisDurumu.Taslak}>Taslak</option>
            <option value={FisDurumu.Onaylandi}>Entegre</option>
            <option value={FisDurumu.Iptal}>İptal</option>
          </select>
          <Input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Fiş no / açıklama ara"
            className="h-8 w-56"
          />
        </div>

        <div className="border rounded-lg overflow-hidden overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Fiş No</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Yev. No</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tarih</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Tür</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Açıklama</th>
                <th className="text-right px-3 py-2.5 font-medium text-muted-foreground">Borç</th>
                <th className="text-right px-3 py-2.5 font-medium text-muted-foreground">Alacak</th>
                <th className="text-left px-3 py-2.5 font-medium text-muted-foreground">Durum</th>
                <th className="px-3 py-2.5"></th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={9} className="text-center py-12 text-muted-foreground">Yükleniyor...</td></tr>
              ) : items.length === 0 ? (
                <tr><td colSpan={9} className="text-center py-12 text-muted-foreground">Fiş bulunamadı.</td></tr>
              ) : (
                items.map((f) => (
                  <tr key={f.id} onClick={() => openFis(f.id)} className="border-b last:border-0 cursor-pointer hover:bg-muted/30">
                    <td className="px-3 py-2.5 font-mono text-xs">{f.fisNo}</td>
                    <td className="px-3 py-2.5">{f.yevmiyeNo ?? '-'}</td>
                    <td className="px-3 py-2.5">{f.fisTarihi.slice(0, 10)}</td>
                    <td className="px-3 py-2.5">{fisTuruLabel[f.fisTuru]}</td>
                    <td className="px-3 py-2.5 max-w-[220px] truncate">{f.aciklama}</td>
                    <td className="px-3 py-2.5 text-right tabular-nums">{fmt(f.toplamBorc)}</td>
                    <td className="px-3 py-2.5 text-right tabular-nums">{fmt(f.toplamAlacak)}</td>
                    <td className="px-3 py-2.5">
                      <Badge variant={f.durum === FisDurumu.Onaylandi ? 'default' : f.durum === FisDurumu.Iptal ? 'destructive' : 'secondary'}>
                        {fisDurumuLabel[f.durum]}
                      </Badge>
                    </td>
                    <td className="px-3 py-2.5 text-right">
                      {f.durum === FisDurumu.Taslak && (
                        <button
                          onClick={(e) => { e.stopPropagation(); handleDelete(f.id) }}
                          className="text-muted-foreground hover:text-destructive"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    )
  }

  // ---- Form görünümü ----
  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Button variant="ghost" size="icon" className="h-8 w-8" onClick={() => setMode('list')}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <h1 className="text-xl font-semibold">
            {editId ? `Fiş ${fisNo}` : 'Yeni Fiş'}
          </h1>
          {durum !== FisDurumu.Taslak && (
            <Badge variant={durum === FisDurumu.Iptal ? 'destructive' : 'default'}>{fisDurumuLabel[durum]}</Badge>
          )}
        </div>
        {durum === FisDurumu.Onaylandi && (
          <Button size="sm" variant="destructive" onClick={handleIptal}>
            <Ban className="h-4 w-4 mr-1" /> İptal (Storno)
          </Button>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-3 mb-4">
        <div className="space-y-1">
          <Label className="text-xs">Fiş Türü</Label>
          <select
            value={fisTuru}
            disabled={readOnly}
            onChange={(e) => setFisTuru(Number(e.target.value) as FisTuru)}
            className="h-9 w-full rounded-md border bg-background px-2 text-sm disabled:opacity-60"
          >
            {fisTuruOptions.map((t) => <option key={t} value={t}>{fisTuruLabel[t]}</option>)}
          </select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Fiş Tarihi</Label>
          <Input type="date" value={fisTarihi} disabled={readOnly} onChange={(e) => setFisTarihi(e.target.value)} />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Açıklama</Label>
          <Input value={aciklama} disabled={readOnly} onChange={(e) => setAciklama(e.target.value)} />
        </div>
      </div>

      <div className="border rounded-lg overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b bg-muted/50">
              <th className="text-left px-2 py-2 font-medium text-muted-foreground w-[34%]">Hesap</th>
              <th className="text-right px-2 py-2 font-medium text-muted-foreground">Borç</th>
              <th className="text-right px-2 py-2 font-medium text-muted-foreground">Alacak</th>
              <th className="text-left px-2 py-2 font-medium text-muted-foreground">Açıklama</th>
              <th className="text-left px-2 py-2 font-medium text-muted-foreground">Belge No</th>
              {!readOnly && <th className="px-2 py-2 w-8"></th>}
            </tr>
          </thead>
          <tbody>
            {lines.map((l, i) => (
              <tr key={i} className="border-b last:border-0">
                <td className="px-2 py-1.5">
                  <select
                    value={l.hesapId}
                    disabled={readOnly}
                    onChange={(e) => updateLine(i, { hesapId: e.target.value })}
                    className="h-8 w-full rounded-md border bg-background px-1.5 text-xs disabled:opacity-60"
                  >
                    <option value="">Hesap seç...</option>
                    {hesaplar.map((h) => (
                      <option key={h.id} value={h.id}>{h.hesapKodu} — {h.hesapAdi}</option>
                    ))}
                  </select>
                </td>
                <td className="px-2 py-1.5">
                  <Input
                    type="number" step="0.01" min="0" inputMode="decimal"
                    value={l.borc} disabled={readOnly}
                    onChange={(e) => updateLine(i, { borc: e.target.value, alacak: '' })}
                    className="h-8 text-right tabular-nums"
                  />
                </td>
                <td className="px-2 py-1.5">
                  <Input
                    type="number" step="0.01" min="0" inputMode="decimal"
                    value={l.alacak} disabled={readOnly}
                    onChange={(e) => updateLine(i, { alacak: e.target.value, borc: '' })}
                    className="h-8 text-right tabular-nums"
                  />
                </td>
                <td className="px-2 py-1.5">
                  <Input value={l.aciklama} disabled={readOnly} onChange={(e) => updateLine(i, { aciklama: e.target.value })} className="h-8" />
                </td>
                <td className="px-2 py-1.5">
                  <Input value={l.belgeNo} disabled={readOnly} onChange={(e) => updateLine(i, { belgeNo: e.target.value })} className="h-8 w-28" />
                </td>
                {!readOnly && (
                  <td className="px-2 py-1.5 text-center">
                    <button onClick={() => removeLine(i)} className="text-muted-foreground hover:text-destructive">
                      <X className="h-4 w-4" />
                    </button>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
          <tfoot>
            <tr className="border-t bg-muted/30 font-medium">
              <td className="px-2 py-2 text-right">Toplam</td>
              <td className="px-2 py-2 text-right tabular-nums">{fmt(totals.borc)}</td>
              <td className="px-2 py-2 text-right tabular-nums">{fmt(totals.alacak)}</td>
              <td className="px-2 py-2" colSpan={readOnly ? 1 : 3}>
                <span className={Math.abs(totals.fark) < 0.005 ? 'text-emerald-600' : 'text-destructive'}>
                  Fark: {fmt(totals.fark)}
                </span>
              </td>
            </tr>
          </tfoot>
        </table>
      </div>

      {!readOnly && (
        <div className="flex items-center justify-between mt-3">
          <Button size="sm" variant="outline" onClick={addLine}>
            <Plus className="h-4 w-4 mr-1" /> Satır Ekle
          </Button>
          <div className="flex items-center gap-2">
            <Button size="sm" variant="secondary" onClick={handleSave} disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Taslak Kaydet'}
            </Button>
            <Button size="sm" onClick={handleOnayla} disabled={saving || !dengeli} title={!dengeli ? 'Fiş dengeli değil' : ''}>
              <CheckCircle2 className="h-4 w-4 mr-1" /> Onayla
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
