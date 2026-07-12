'use client'

import { useCallback, useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { personsApi } from '@/lib/api/persons'
import { showError } from '@/lib/toast'
import { PersonFullDetailDto } from '@/types/personDetail'
import { GeneralInfoTab } from './_components/GeneralInfoTab'
import { DetailInfoTab } from './_components/DetailInfoTab'
import { IdentityInfoTab } from './_components/IdentityInfoTab'
import { UnitsHistoryTab } from './_components/UnitsHistoryTab'
import { VehiclesTab } from './_components/VehiclesTab'
import { ContactHistoryTab } from './_components/ContactHistoryTab'
import { AccessCardsTab } from './_components/AccessCardsTab'
import { SupportRequestsTab } from './_components/SupportRequestsTab'

export default function PersonDetailPage() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter()
  const [detail, setDetail] = useState<PersonFullDetailDto | null>(null)
  const [loading, setLoading] = useState(true)

  const load = useCallback(() => {
    setLoading(true)
    personsApi.getFullDetail(id)
      .then((res) => setDetail(res.data))
      .catch(() => showError('Kişi detayı yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [id])

  useEffect(() => {
    load()
  }, [load])

  if (loading) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  if (!detail) {
    return <div className="text-center py-12 text-muted-foreground">Kişi bulunamadı.</div>
  }

  const { general } = detail

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => router.push('/persons')}>
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary text-primary-foreground font-medium">
          {general.firstName[0]?.toUpperCase()}
        </div>
        <div className="flex-1">
          <h1 className="text-lg font-semibold">{general.firstName} {general.lastName}</h1>
          <p className="text-xs text-muted-foreground">{general.email}</p>
        </div>
        <Badge variant={general.isActive ? 'default' : 'secondary'}>{general.isActive ? 'Aktif' : 'Pasif'}</Badge>
      </div>

      <Tabs defaultValue="general" className="w-full">
        <TabsList className="flex-wrap h-auto">
          <TabsTrigger value="general">Genel Bilgiler</TabsTrigger>
          <TabsTrigger value="detail">Detay Bilgileri</TabsTrigger>
          <TabsTrigger value="identity">Kimlik Bilgileri</TabsTrigger>
          <TabsTrigger value="units">Daireler</TabsTrigger>
          <TabsTrigger value="vehicles">Araçlar</TabsTrigger>
          <TabsTrigger value="contact">İletişim Geçmişi</TabsTrigger>
          <TabsTrigger value="cards">Geçiş Kartları</TabsTrigger>
          <TabsTrigger value="requests">Talepler</TabsTrigger>
        </TabsList>

        <TabsContent value="general">
          <GeneralInfoTab userSiteId={id} data={detail.general} onSaved={load} />
        </TabsContent>
        <TabsContent value="detail">
          <DetailInfoTab userSiteId={id} data={detail.detail} onSaved={load} />
        </TabsContent>
        <TabsContent value="identity">
          <IdentityInfoTab userSiteId={id} data={detail.identity} onSaved={load} />
        </TabsContent>
        <TabsContent value="units">
          <UnitsHistoryTab units={detail.units} />
        </TabsContent>
        <TabsContent value="vehicles">
          <VehiclesTab vehicles={detail.vehicles} />
        </TabsContent>
        <TabsContent value="contact">
          <ContactHistoryTab contactHistory={detail.contactHistory} />
        </TabsContent>
        <TabsContent value="cards">
          <AccessCardsTab accessCards={detail.accessCards} />
        </TabsContent>
        <TabsContent value="requests">
          <SupportRequestsTab supportRequests={detail.supportRequests} />
        </TabsContent>
      </Tabs>
    </div>
  )
}
