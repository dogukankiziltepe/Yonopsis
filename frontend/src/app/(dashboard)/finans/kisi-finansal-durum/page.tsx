'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, X, ChevronDown, Mail, Phone, User } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { kisilerFinansalDurumApi } from '@/lib/api/finans'
import type { KisiFinansalDurumSatiri, KisiFinansalDetay, DaireFinansal, GelirGrubuFinansal } from '@/types/finans'
import { showApiError } from '@/lib/toast'

const PAGE_SIZE = 20
const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

const kaynakStil: Record<string, string> = {
  Borc: 'bg-green-500',
  Tahsilat: 'bg-green-300',
  Devir: 'bg-red-500',
}

function DaireAccordion({ daire }: { daire: DaireFinansal }) {
  const [open, setOpen] = useState(false)
  return (
    <div className="border rounded-md overflow-hidden">
      <button onClick={() => setOpen(v => !v)} className="w-full flex items-center justify-between px-3 py-2 bg-muted/40 hover:bg-muted/60 text-sm">
        <span className="flex items-center gap-2 font-medium">
          <ChevronDown className={`h-3.5 w-3.5 transition-transform ${open ? 'rotate-180' : ''}`} />
          {daire.doorNumber}
        </span>
        <span className="flex gap-4 text-xs text-muted-foreground">
          <span>Borç: {fmt(daire.borc)}</span>
          <span>Tazminat: {fmt(daire.tazminat)}</span>
          <span>Alacak: {fmt(daire.alacak)}</span>
          <span className="font-medium text-foreground">Bakiye: {fmt(daire.bakiye)}</span>
        </span>
      </button>
      {open && (
        <div className="p-2 space-y-2">
          {daire.kategoriler.map((kategori, i) => (
            <KategoriAccordion key={i} kategori={kategori} />
          ))}
        </div>
      )}
    </div>
  )
}

function KategoriAccordion({ kategori }: { kategori: GelirGrubuFinansal }) {
  const [open, setOpen] = useState(true)
  return (
    <div className="border rounded-md overflow-hidden ml-2">
      <button onClick={() => setOpen(v => !v)} className="w-full flex items-center justify-between px-3 py-1.5 bg-muted/20 hover:bg-muted/40 text-xs">
        <span className="flex items-center gap-2 font-medium">
          <ChevronDown className={`h-3 w-3 transition-transform ${open ? 'rotate-180' : ''}`} />
          {kategori.grupAdi}
        </span>
        <span className="flex gap-3 text-muted-foreground">
          <span>{fmt(kategori.borc)}</span>
          <span>{fmt(kategori.tazminat)}</span>
          <span>{fmt(kategori.alacak)}</span>
          <span className="font-medium text-foreground">{fmt(kategori.bakiye)}</span>
        </span>
      </button>
      {open && (
        <div className="overflow-x-auto">
          <table className="w-full text-xs">
            <thead className="bg-muted/10 border-t">
              <tr>
                <th className="w-2" />
                <th className="text-left px-2 py-1.5 font-medium">Evrak T.</th>
                <th className="text-left px-2 py-1.5 font-medium">Son Ödeme T.</th>
                <th className="text-left px-2 py-1.5 font-medium">Açıklama</th>
                <th className="text-right px-2 py-1.5 font-medium">Borç</th>
                <th className="text-right px-2 py-1.5 font-medium">Tazminat</th>
                <th className="text-right px-2 py-1.5 font-medium">Alacak</th>
                <th className="text-right px-2 py-1.5 font-medium">Bakiye</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {kategori.hareketler.map(h => (
                <tr key={h.id}>
                  <td className="pl-2"><div className={`w-1 h-4 rounded ${kaynakStil[h.kaynak] ?? 'bg-muted'}`} /></td>
                  <td className="px-2 py-1.5 whitespace-nowrap">{new Date(h.evrakTarihi).toLocaleDateString('tr-TR')}</td>
                  <td className="px-2 py-1.5 whitespace-nowrap">{h.sonOdemeTarihi ? new Date(h.sonOdemeTarihi).toLocaleDateString('tr-TR') : ''}</td>
                  <td className="px-2 py-1.5 max-w-xs truncate" title={h.aciklama ?? ''}>{h.aciklama ?? '—'}</td>
                  <td className="px-2 py-1.5 text-right">{h.borc > 0 ? fmt(h.borc) : ''}</td>
                  <td className="px-2 py-1.5 text-right">{h.tazminat > 0 ? fmt(h.tazminat) : ''}</td>
                  <td className="px-2 py-1.5 text-right">{h.alacak > 0 ? fmt(h.alacak) : ''}</td>
                  <td className="px-2 py-1.5 text-right font-medium">{fmt(h.yurudakiBakiye)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default function KisiFinansalDurumPage() {
  const [items, setItems] = useState<KisiFinansalDurumSatiri[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const [selectedPersonId, setSelectedPersonId] = useState<string | null>(null)
  const [detay, setDetay] = useState<KisiFinansalDetay | null>(null)
  const [detayLoading, setDetayLoading] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await kisilerFinansalDurumApi.getAll(page, PAGE_SIZE, search || undefined)
      const d = res.data
      setItems(d.items ?? [])
      setTotal(d.totalCount ?? 0)
      setTotalPages(d.totalPages ?? Math.ceil((d.totalCount ?? 0) / PAGE_SIZE))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [page, search])

  useEffect(() => { load() }, [load])
  useEffect(() => { const t = setTimeout(() => setSearch(searchInput), 350); return () => clearTimeout(t) }, [searchInput])

  const openDetay = async (personUserId: string) => {
    setSelectedPersonId(personUserId)
    setDetayLoading(true)
    try {
      const res = await kisilerFinansalDurumApi.getDetay(personUserId)
      setDetay(res.data)
    } catch (e) { showApiError(e) }
    finally { setDetayLoading(false) }
  }
  const closeDetay = () => { setSelectedPersonId(null); setDetay(null) }

  return (
    <div className="flex flex-col h-full gap-3">
      <h1 className="text-xl font-semibold">Kişilere Göre Finansal Durum</h1>

      <div className="flex items-center gap-2">
        <div className="relative flex-1 max-w-xs">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input className="pl-8" placeholder="İsim ara..." value={searchInput} onChange={e => { setSearchInput(e.target.value); setPage(1) }} />
        </div>
        {total > 0 && <span className="text-sm text-muted-foreground">{total} kişi</span>}
      </div>

      <div className="border rounded-lg overflow-hidden flex-1 overflow-x-auto">
        {loading ? (
          <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center"><User className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" /><p className="text-muted-foreground text-sm">Kayıt bulunamadı.</p></div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-3 py-2 font-medium">Borçlu Kişi</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Borç Tutarı</th>
                <th className="text-right px-3 py-2 font-medium hidden lg:table-cell">Gecikme</th>
                <th className="text-right px-3 py-2 font-medium hidden lg:table-cell">İade Edilen</th>
                <th className="text-right px-3 py-2 font-medium hidden md:table-cell">Ödenen</th>
                <th className="text-right px-3 py-2 font-medium">Borç</th>
                <th className="text-right px-3 py-2 font-medium">Alacak</th>
                <th className="text-right px-3 py-2 font-medium">Bakiye</th>
                <th className="text-center px-3 py-2 font-medium">B/A</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map(row => (
                <tr key={row.personUserId} className="hover:bg-muted/30 cursor-pointer" onClick={() => openDetay(row.personUserId)}>
                  <td className="px-3 py-2.5 font-medium">{row.adSoyad}</td>
                  <td className="px-3 py-2.5 text-right hidden md:table-cell">{fmt(row.borcTutari)}</td>
                  <td className="px-3 py-2.5 text-right hidden lg:table-cell">{fmt(row.gecikme)}</td>
                  <td className="px-3 py-2.5 text-right hidden lg:table-cell">{fmt(row.iadeEdilen)}</td>
                  <td className="px-3 py-2.5 text-right hidden md:table-cell">{fmt(row.odenen)}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(row.borc)}</td>
                  <td className="px-3 py-2.5 text-right">{fmt(row.alacak)}</td>
                  <td className="px-3 py-2.5 text-right font-medium">{fmt(row.bakiye)}</td>
                  <td className="px-3 py-2.5 text-center">
                    <Badge variant={row.ba === 'B' ? 'default' : 'secondary'}>{row.ba}</Badge>
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

      {selectedPersonId && (
        <div className="fixed inset-0 z-40 flex justify-end">
          <div className="flex-1 bg-black/30" onClick={closeDetay} />
          <div className="w-full max-w-2xl bg-background border-l shadow-xl flex flex-col">
            <div className="flex items-center justify-between px-4 py-3 border-b">
              <h2 className="font-semibold">Finansal Durum Detayı</h2>
              <Button variant="ghost" size="icon" onClick={closeDetay}><X className="h-4 w-4" /></Button>
            </div>

            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              {detayLoading ? (
                <div className="text-center text-sm text-muted-foreground py-8">Yükleniyor...</div>
              ) : detay ? (
                <>
                  <div className="border rounded-lg p-4 flex items-start justify-between gap-4">
                    <div>
                      <div className="font-semibold text-base">{detay.kisi.adSoyad}</div>
                      {detay.kisi.email && <div className="text-sm text-muted-foreground flex items-center gap-1.5 mt-1"><Mail className="h-3.5 w-3.5" />{detay.kisi.email}</div>}
                      {detay.kisi.telefon && <div className="text-sm text-muted-foreground flex items-center gap-1.5 mt-0.5"><Phone className="h-3.5 w-3.5" />{detay.kisi.telefon}</div>}
                    </div>
                    <div className="text-right space-y-1 text-sm shrink-0">
                      <div>Borç <span className="font-semibold ml-1">{fmt(detay.toplamBorc)}</span></div>
                      <div>Fazla Ödeme <span className="font-semibold ml-1">{fmt(detay.toplamAlacak)}</span></div>
                      <div className="text-muted-foreground">Bakiye <span className="font-semibold ml-1 text-foreground">{fmt(detay.toplamBakiye)}</span></div>
                    </div>
                  </div>

                  <div className="border rounded-lg p-4 space-y-2">
                    <h3 className="font-medium text-sm mb-2">Daireler</h3>
                    {detay.daireler.length === 0 ? (
                      <p className="text-sm text-muted-foreground">Kişiye ait daire bulunamadı.</p>
                    ) : (
                      <div className="space-y-2">
                        {detay.daireler.map((d, i) => <DaireAccordion key={i} daire={d} />)}
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <div className="text-center text-sm text-muted-foreground py-8">Bulunamadı.</div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
