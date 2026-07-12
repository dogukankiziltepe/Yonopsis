'use client'

import { useCallback, useEffect, useState } from 'react'
import { Download, ChevronLeft, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { showError } from '@/lib/toast'
import { exportToCsv } from '@/lib/utils/exportCsv'
import { muhasebeApi } from '@/lib/api/muhasebe'
import { YevmiyeDefteri, YevmiyeSatir, fisTuruLabel } from '@/types/muhasebe'

const PAGE_SIZE = 100

function fmt(n: number) {
  return n.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function groupByDate(satirlar: YevmiyeSatir[]) {
  const groups: Record<string, YevmiyeSatir[]> = {}
  for (const s of satirlar) {
    const key = s.fisTarihi.slice(0, 10)
    groups[key] = groups[key] ? [...groups[key], s] : [s]
  }
  return groups
}

export default function YevmiyeDefteriPage() {
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [data, setData] = useState<YevmiyeDefteri | null>(null)
  const [loading, setLoading] = useState(false)
  const [page, setPage] = useState(1)

  const load = useCallback(() => {
    setLoading(true)
    muhasebeApi.getYevmiye(from || undefined, to || undefined)
      .then((r) => { setData(r.data); setPage(1) })
      .catch(() => showError('Yevmiye defteri yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [from, to])

  useEffect(() => { load() }, [load])

  const allRows = data?.satirlar ?? []
  const totalPages = Math.max(1, Math.ceil(allRows.length / PAGE_SIZE))
  const pageRows = allRows.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)
  const grouped = groupByDate(pageRows)
  const dateKeys = Object.keys(grouped).sort()

  const doExport = () => {
    if (!data) return
    exportToCsv('yevmiye-defteri',
      ['Yev. No', 'Fiş No', 'Tarih', 'Tür', 'Hesap Kodu', 'Hesap Adı', 'Açıklama', 'Borç', 'Alacak'],
      data.satirlar.map((s) => [
        s.yevmiyeNo ?? '', s.fisNo, s.fisTarihi.slice(0, 10), fisTuruLabel[s.fisTuru],
        s.hesapKodu, s.hesapAdi, s.aciklama ?? '', s.borcTutar, s.alacakTutar,
      ]))
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h1 className="text-xl font-semibold">Yevmiye Defteri</h1>
          {data && (
            <p className="text-xs text-muted-foreground mt-0.5">
              {data.satirlar.length} satır &nbsp;·&nbsp;
              Toplam Borç: <b>{fmt(data.toplamBorc)}</b> &nbsp;·&nbsp;
              Toplam Alacak: <b>{fmt(data.toplamAlacak)}</b>
            </p>
          )}
        </div>
        <Button size="sm" variant="outline" onClick={doExport}>
          <Download className="h-4 w-4 mr-1" /> CSV
        </Button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-end gap-2 mb-4">
        <div className="space-y-1">
          <Label className="text-xs">Başlangıç</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="h-8" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs">Bitiş</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="h-8" />
        </div>
        <Button size="sm" onClick={load} disabled={loading}>Yenile</Button>
      </div>

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : allRows.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">Kayıt yok.</div>
      ) : (
        <>
          <div className="border rounded-lg overflow-x-auto flex-1">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b bg-muted/50 sticky top-0">
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground w-14">Yev.</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground w-28">Fiş No</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground w-20">Tür</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Hesap</th>
                  <th className="text-left px-3 py-2 font-medium text-muted-foreground">Açıklama</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground w-28">Borç</th>
                  <th className="text-right px-3 py-2 font-medium text-muted-foreground w-28">Alacak</th>
                </tr>
              </thead>
              <tbody>
                {dateKeys.map((date) => {
                  const rows = grouped[date]
                  const dayBorc = rows.reduce((a, r) => a + r.borcTutar, 0)
                  const dayAlacak = rows.reduce((a, r) => a + r.alacakTutar, 0)
                  return (
                    <>
                      {/* Date group header */}
                      <tr key={`hdr-${date}`} className="bg-muted/30 border-b">
                        <td colSpan={5} className="px-3 py-1.5 font-semibold text-xs text-muted-foreground">
                          {new Date(date).toLocaleDateString('tr-TR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                        </td>
                        <td className="px-3 py-1.5 text-right tabular-nums text-xs font-medium">{fmt(dayBorc)}</td>
                        <td className="px-3 py-1.5 text-right tabular-nums text-xs font-medium">{fmt(dayAlacak)}</td>
                      </tr>
                      {rows.map((s, i) => (
                        <tr key={`${date}-${i}`} className="border-b last:border-0 hover:bg-muted/20">
                          <td className="px-3 py-1.5 text-muted-foreground">{s.yevmiyeNo ?? '-'}</td>
                          <td className="px-3 py-1.5 font-mono text-xs">{s.fisNo}</td>
                          <td className="px-3 py-1.5">
                            <Badge variant="outline" className="text-xs">{fisTuruLabel[s.fisTuru]}</Badge>
                          </td>
                          <td className="px-3 py-1.5">
                            <span className="font-mono text-xs text-muted-foreground">{s.hesapKodu}</span>{' '}
                            {s.hesapAdi}
                          </td>
                          <td className="px-3 py-1.5 max-w-[220px] truncate text-muted-foreground">{s.aciklama}</td>
                          <td className="px-3 py-1.5 text-right tabular-nums">{s.borcTutar ? fmt(s.borcTutar) : ''}</td>
                          <td className="px-3 py-1.5 text-right tabular-nums">{s.alacakTutar ? fmt(s.alacakTutar) : ''}</td>
                        </tr>
                      ))}
                    </>
                  )
                })}
              </tbody>
              {data && (
                <tfoot>
                  <tr className="border-t bg-muted/30 font-semibold">
                    <td colSpan={5} className="px-3 py-2 text-right">Genel Toplam</td>
                    <td className="px-3 py-2 text-right tabular-nums">{fmt(data.toplamBorc)}</td>
                    <td className="px-3 py-2 text-right tabular-nums">{fmt(data.toplamAlacak)}</td>
                  </tr>
                </tfoot>
              )}
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between mt-3 text-sm">
              <span className="text-muted-foreground">
                Sayfa {page} / {totalPages} &nbsp;·&nbsp; {allRows.length} satır
              </span>
              <div className="flex gap-1">
                <Button size="sm" variant="outline" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
                  <ChevronLeft className="h-4 w-4" />
                </Button>
                <Button size="sm" variant="outline" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
                  <ChevronRight className="h-4 w-4" />
                </Button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  )
}
