'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, BookOpen } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { muhasebeApi } from '@/lib/api/muhasebe'
import type { HesapListItem } from '@/types/muhasebe'
import { HesapKategorisi, hesapKategorisiLabel } from '@/types/muhasebe'

const kategoriBadge: Record<HesapKategorisi, 'default' | 'secondary' | 'outline' | 'destructive'> = {
  [HesapKategorisi.Aktif]: 'default',
  [HesapKategorisi.Pasif]: 'secondary',
  [HesapKategorisi.Gelir]: 'default',
  [HesapKategorisi.Gider]: 'destructive',
  [HesapKategorisi.Maliyet]: 'secondary',
  [HesapKategorisi.Nazim]: 'outline',
}

export default function PageComponent() {
  const [items, setItems] = useState<HesapListItem[]>([])
  const [filtered, setFiltered] = useState<HesapListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [kategoriFilt, setKategoriFilt] = useState<HesapKategorisi | ''>('')
  const [aktifFilt, setAktifFilt] = useState<'' | 'true' | 'false'>('')

  const load = useCallback(async (q: string) => {
    setLoading(true)
    const r = await muhasebeApi.getHesaplar({ search: q || undefined })
    if (r.success && r.data) {
      // Exclude cari accounts (they have cariTuru set)
      setItems(r.data.filter(h => h.cariTuru == null))
    }
    setLoading(false)
  }, [])

  useEffect(() => { load(search) }, [search, load])

  useEffect(() => {
    let result = items
    if (kategoriFilt !== '') result = result.filter(h => h.hesapKategorisi === kategoriFilt)
    if (aktifFilt === 'true') result = result.filter(h => h.aktifMi)
    if (aktifFilt === 'false') result = result.filter(h => !h.aktifMi)
    setFiltered(result)
  }, [items, kategoriFilt, aktifFilt])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Genel Hesaplar</h1>
        <span className="text-sm text-muted-foreground">{filtered.length} hesap</span>
      </div>

      <div className="flex flex-wrap gap-2 mb-4">
        <form onSubmit={handleSearch} className="flex gap-2">
          <Input
            placeholder="Hesap kodu veya adı..."
            value={inputVal}
            onChange={e => setInputVal(e.target.value)}
            className="w-52"
          />
          <Button type="submit" variant="outline" size="sm"><Search className="h-4 w-4" /></Button>
        </form>
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={kategoriFilt}
          onChange={e => setKategoriFilt(e.target.value === '' ? '' : Number(e.target.value) as HesapKategorisi)}
        >
          <option value="">Tüm Kategoriler</option>
          {(Object.values(HesapKategorisi).filter(v => typeof v === 'number') as HesapKategorisi[]).map(k => (
            <option key={k} value={k}>{hesapKategorisiLabel[k]}</option>
          ))}
        </select>
        <select
          className="border rounded-md px-3 py-2 text-sm bg-background"
          value={aktifFilt}
          onChange={e => setAktifFilt(e.target.value as '' | 'true' | 'false')}
        >
          <option value="">Tümü</option>
          <option value="true">Aktif</option>
          <option value="false">Pasif</option>
        </select>
      </div>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Hesap Kodu</th>
              <th className="text-left px-3 py-2 font-medium">Hesap Adı</th>
              <th className="text-left px-3 py-2 font-medium">Kategori</th>
              <th className="text-left px-3 py-2 font-medium">Fatura Kesilebilir</th>
              <th className="text-left px-3 py-2 font-medium">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={5} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : filtered.length === 0 ? (
              <tr>
                <td colSpan={5} className="text-center py-16 text-muted-foreground">
                  <BookOpen className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Hesap bulunamadı</p>
                </td>
              </tr>
            ) : filtered.map(item => (
              <tr key={item.id} className={`border-t hover:bg-muted/30 ${item.seviye > 1 ? 'text-muted-foreground' : ''}`}>
                <td className="px-3 py-2 font-mono text-xs" style={{ paddingLeft: `${(item.seviye - 1) * 16 + 12}px` }}>
                  {item.hesapKodu}
                </td>
                <td className="px-3 py-2">{item.hesapAdi}</td>
                <td className="px-3 py-2">
                  <Badge variant={kategoriBadge[item.hesapKategorisi]}>{hesapKategorisiLabel[item.hesapKategorisi]}</Badge>
                </td>
                <td className="px-3 py-2 text-center">{item.fisKesilebilirMi ? '✓' : '—'}</td>
                <td className="px-3 py-2">
                  <Badge variant={item.aktifMi ? 'default' : 'secondary'}>
                    {item.aktifMi ? 'Aktif' : 'Pasif'}
                  </Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
