'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { Plus, Eye, Trash2, Building2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import { sitesApi } from '@/lib/api/sites'
import { SiteSummaryDto, DbMode } from '@/types/site'

export default function AdminSitesPage() {
  const [sites, setSites] = useState<SiteSummaryDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = () => {
    setLoading(true)
    sitesApi.getAll()
      .then((res) => setSites(res.data ?? []))
      .catch(() => setError('Siteler yüklenemedi.'))
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const handleDelete = async (id: string) => {
    if (!confirm('Bu siteyi silmek istediğinize emin misiniz?')) return
    try {
      await sitesApi.delete(id)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Siteler</h1>
          <p className="text-muted-foreground">Tüm siteleri yönetin</p>
        </div>
        <Link href="/admin/sites/new">
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Yeni Site
          </Button>
        </Link>
      </div>

      {error && (
        <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : sites.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12 text-center">
            <Building2 className="h-10 w-10 text-muted-foreground/50 mb-3" />
            <p className="text-muted-foreground">Henüz site eklenmemiş</p>
          </CardContent>
        </Card>
      ) : (
        <div className="border rounded-lg overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Site Adı</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Şehir</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">DB Modu</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Blok / Daire</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Durum</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {sites.map((site) => (
                <tr key={site.id} className="border-b last:border-0 hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{site.name}</td>
                  <td className="px-4 py-3 text-muted-foreground">{site.city ?? '-'}</td>
                  <td className="px-4 py-3">
                    <Badge variant={site.dbMode === DbMode.Dedicated ? 'default' : 'secondary'}>
                      {site.dbMode === DbMode.Dedicated ? 'Dedicated' : 'Shared'}
                    </Badge>
                  </td>
                  <td className="px-4 py-3 text-muted-foreground">
                    {site.blockCount} / {site.unitCount}
                  </td>
                  <td className="px-4 py-3">
                    <Badge variant={site.isActive ? 'success' : 'secondary'}>
                      {site.isActive ? 'Aktif' : 'Pasif'}
                    </Badge>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 justify-end">
                      <Link href={`/admin/sites/${site.id}`}>
                        <Button variant="ghost" size="icon" className="h-7 w-7">
                          <Eye className="h-3.5 w-3.5" />
                        </Button>
                      </Link>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-destructive hover:text-destructive"
                        onClick={() => handleDelete(site.id)}
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
