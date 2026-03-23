'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft, Building2 } from 'lucide-react'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { sitesApi } from '@/lib/api/sites'
import { SiteDetailDto, DbMode } from '@/types/site'

export default function SiteDetailPage() {
  const { id } = useParams<{ id: string }>()
  const [site, setSite] = useState<SiteDetailDto | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    sitesApi.getById(id)
      .then((res) => setSite(res.data.value))
      .catch(() => setError('Site yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [id])

  if (loading) return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  if (error) return <div className="text-center py-12 text-destructive">{error}</div>
  if (!site) return null

  const fields = [
    { label: 'Adres', value: site.address },
    { label: 'İlçe', value: site.district },
    { label: 'Şehir', value: site.city },
    { label: 'Posta Kodu', value: site.postalCode },
    { label: 'Telefon', value: site.phone },
    { label: 'E-posta', value: site.email },
    { label: 'Vergi Dairesi', value: site.taxOffice },
    { label: 'Vergi No', value: site.taxNumber },
  ]

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center gap-3">
        <Link href="/admin/sites">
          <Button variant="ghost" size="icon">
            <ArrowLeft className="h-4 w-4" />
          </Button>
        </Link>
        <div className="flex-1">
          <div className="flex items-center gap-3">
            <h1 className="text-2xl font-bold tracking-tight">{site.name}</h1>
            <Badge variant={site.isActive ? 'success' : 'secondary'}>
              {site.isActive ? 'Aktif' : 'Pasif'}
            </Badge>
          </div>
          <p className="text-muted-foreground text-sm">
            {site.dbMode === DbMode.Dedicated ? 'Dedicated DB' : 'Shared DB'}
          </p>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4">
        <Card>
          <CardContent className="pt-4">
            <p className="text-2xl font-bold">{site.blockCount}</p>
            <p className="text-sm text-muted-foreground">Blok</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="pt-4">
            <p className="text-2xl font-bold">{site.unitCount}</p>
            <p className="text-sm text-muted-foreground">Daire</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Site Bilgileri</CardTitle></CardHeader>
        <CardContent className="space-y-3">
          {fields.map(({ label, value }) => (
            value ? (
              <div key={label} className="flex justify-between text-sm">
                <span className="text-muted-foreground">{label}</span>
                <span className="font-medium">{value}</span>
              </div>
            ) : null
          ))}
        </CardContent>
      </Card>
    </div>
  )
}
