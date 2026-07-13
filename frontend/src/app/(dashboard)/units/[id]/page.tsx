'use client'

import { useCallback, useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { unitsApi } from '@/lib/api/units'
import { showError } from '@/lib/toast'
import { UnitStatusLabel } from '@/types/unit'
import { UnitFullDetailDto } from '@/types/unitDetail'
import { DaireTab } from './_components/DaireTab'
import { IliskiliKisilerTab } from './_components/IliskiliKisilerTab'
import { DetayBilgileriTab } from './_components/DetayBilgileriTab'
import { AraclarTab } from './_components/AraclarTab'

export default function UnitDetailPage() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter()
  const [detail, setDetail] = useState<UnitFullDetailDto | null>(null)
  const [loading, setLoading] = useState(true)

  const load = useCallback(() => {
    setLoading(true)
    unitsApi.getFullDetail(id)
      .then((res) => setDetail(res.data))
      .catch(() => showError('Daire detayı yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [id])

  useEffect(() => {
    load()
  }, [load])

  if (loading) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  if (!detail) {
    return <div className="text-center py-12 text-muted-foreground">Daire bulunamadı.</div>
  }

  const { core } = detail

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => router.push('/units')}>
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <div className="flex-1">
          <h1 className="text-lg font-semibold">
            {core.buildingName ? `${core.buildingName} - ` : ''}{core.doorNumber}
          </h1>
          <p className="text-xs text-muted-foreground">{core.unitTypeName ?? ''}</p>
        </div>
        <Badge variant={core.status === 1 ? 'default' : 'secondary'}>{UnitStatusLabel[core.status]}</Badge>
      </div>

      <Tabs defaultValue="daire" className="w-full">
        <TabsList className="flex-wrap h-auto">
          <TabsTrigger value="daire">Daire</TabsTrigger>
          <TabsTrigger value="iliskili-kisiler">Daire İle İlişkili Kişiler</TabsTrigger>
          <TabsTrigger value="detay">Detay Bilgileri</TabsTrigger>
          <TabsTrigger value="araclar">Araçlar</TabsTrigger>
        </TabsList>

        <TabsContent value="daire">
          <DaireTab unitId={id} detail={detail} onSaved={load} />
        </TabsContent>
        <TabsContent value="iliskili-kisiler">
          <IliskiliKisilerTab relatedPersons={detail.relatedPersons} />
        </TabsContent>
        <TabsContent value="detay">
          <DetayBilgileriTab unitId={id} detail={detail} onSaved={load} />
        </TabsContent>
        <TabsContent value="araclar">
          <AraclarTab vehicles={detail.vehicles} />
        </TabsContent>
      </Tabs>
    </div>
  )
}
