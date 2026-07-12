'use client'

import { useEffect, useState, useCallback } from 'react'
import { Search, ChevronLeft, ChevronRight, Users } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { personsApi } from '@/lib/api/persons'
import type { PersonDto } from '@/types/person'
import { UserTypeLabel, UserSiteStatusLabel } from '@/types/person'

const PAGE_SIZE = 30

export default function PageComponent() {
  const [items, setItems] = useState<PersonDto[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [inputVal, setInputVal] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(async (pg: number, q: string) => {
    setLoading(true)
    const r = await personsApi.getAll(pg, PAGE_SIZE, q || undefined)
    if (r.success && r.data) {
      setItems(r.data.items)
      setTotal(r.data.totalCount)
    }
    setLoading(false)
  }, [])

  useEffect(() => {
    load(page, search)
  }, [page, search, load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setSearch(inputVal)
    setPage(1)
  }

  const totalPages = Math.ceil(total / PAGE_SIZE)

  const statusVariant = (status: PersonDto['status']): 'default' | 'secondary' | 'destructive' => {
    if (status === 1) return 'default'
    if (status === 2) return 'destructive'
    return 'secondary'
  }

  const typeVariant = (type?: PersonDto['userType']): 'default' | 'secondary' | 'outline' => {
    if (type === 2) return 'default'  // Mal Sahibi
    if (type === 3) return 'secondary' // Kiracı
    return 'outline'
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Site Sakinleri Listesi</h1>
        <span className="text-sm text-muted-foreground">{total} kişi</span>
      </div>

      <form onSubmit={handleSearch} className="flex gap-2 mb-4">
        <Input
          placeholder="Ad, soyad veya e-posta ara..."
          value={inputVal}
          onChange={e => setInputVal(e.target.value)}
          className="max-w-sm"
        />
        <Button type="submit" variant="outline" size="sm"><Search className="h-4 w-4" /></Button>
        {search && (
          <Button type="button" variant="ghost" size="sm" onClick={() => { setSearch(''); setInputVal(''); setPage(1) }}>
            Temizle
          </Button>
        )}
      </form>

      <div className="border rounded-lg overflow-auto flex-1">
        <table className="w-full text-sm">
          <thead className="bg-muted/50">
            <tr>
              <th className="text-left px-3 py-2 font-medium">Ad Soyad</th>
              <th className="text-left px-3 py-2 font-medium">E-posta</th>
              <th className="text-left px-3 py-2 font-medium">Telefon</th>
              <th className="text-left px-3 py-2 font-medium">Tür</th>
              <th className="text-left px-3 py-2 font-medium">Rol</th>
              <th className="text-left px-3 py-2 font-medium">Durum</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan={6} className="text-center py-10 text-muted-foreground">Yükleniyor...</td></tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={6} className="text-center py-16 text-muted-foreground">
                  <Users className="h-10 w-10 mx-auto mb-2 opacity-30" />
                  <p>Sakin bulunamadı</p>
                </td>
              </tr>
            ) : items.map(item => (
              <tr key={item.userSiteId} className="border-t hover:bg-muted/30">
                <td className="px-3 py-2 font-medium">{item.firstName} {item.lastName}</td>
                <td className="px-3 py-2 text-muted-foreground text-xs">{item.email}</td>
                <td className="px-3 py-2 text-muted-foreground">{item.phoneNumber ?? '—'}</td>
                <td className="px-3 py-2">
                  {item.userType != null && (
                    <Badge variant={typeVariant(item.userType)}>{UserTypeLabel[item.userType]}</Badge>
                  )}
                </td>
                <td className="px-3 py-2 text-muted-foreground">{item.roleName ?? '—'}</td>
                <td className="px-3 py-2">
                  <Badge variant={statusVariant(item.status)}>{UserSiteStatusLabel[item.status]}</Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="flex items-center justify-between mt-3">
          <span className="text-sm text-muted-foreground">Sayfa {page} / {totalPages}</span>
          <div className="flex gap-1">
            <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <Button variant="outline" size="sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
