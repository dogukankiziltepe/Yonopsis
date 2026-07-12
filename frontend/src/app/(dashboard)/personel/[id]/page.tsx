'use client'

import { useCallback, useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { personelApi } from '@/lib/api/personel'
import { showError } from '@/lib/toast'
import type { PersonelFullDetailDto } from '@/types/personelDetail'
import { PersonelTanimlariTab } from './_components/PersonelTanimlariTab'
import { MuhasebeEntegrasyonTab } from './_components/MuhasebeEntegrasyonTab'
import { BankaBilgileriTab } from './_components/BankaBilgileriTab'
import { KimlikBilgileriTab } from './_components/KimlikBilgileriTab'
import { EgitimlerTab } from './_components/EgitimlerTab'
import { IzinYonetimiTab } from './_components/IzinYonetimiTab'

export default function PersonelDetailPage() {
  const { id } = useParams<{ id: string }>()
  const router = useRouter()
  const [detail, setDetail] = useState<PersonelFullDetailDto | null>(null)
  const [loading, setLoading] = useState(true)

  const load = useCallback(() => {
    setLoading(true)
    personelApi.getFullDetail(id)
      .then((res) => setDetail(res.data))
      .catch(() => showError('Personel detayı yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [id])

  useEffect(() => { load() }, [load])

  if (loading) {
    return <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
  }

  if (!detail) {
    return <div className="text-center py-12 text-muted-foreground">Personel bulunamadı.</div>
  }

  const { core } = detail

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => router.push('/personel')}>
          <ArrowLeft className="h-4 w-4" />
        </Button>
        <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary text-primary-foreground font-medium">
          {core.name[0]?.toUpperCase()}
        </div>
        <div className="flex-1">
          <h1 className="text-lg font-semibold">{core.name}</h1>
          <p className="text-xs text-muted-foreground">{core.personelKodu} · {core.title}</p>
        </div>
        <Badge variant={core.isActive ? 'default' : 'secondary'}>{core.isActive ? 'Aktif' : 'Pasif'}</Badge>
      </div>

      <Tabs defaultValue="tanimlar" className="w-full">
        <TabsList className="flex-wrap h-auto">
          <TabsTrigger value="tanimlar">Personel Tanımları</TabsTrigger>
          <TabsTrigger value="muhasebe">Muhasebe Entegrasyon Hesap Kodları</TabsTrigger>
          <TabsTrigger value="banka">Banka Bilgileri</TabsTrigger>
          <TabsTrigger value="kimlik">Kimlik Bilgileri</TabsTrigger>
          <TabsTrigger value="egitimler">Eğitimler</TabsTrigger>
          <TabsTrigger value="izin">Personel İzin Yönetimi</TabsTrigger>
        </TabsList>

        <TabsContent value="tanimlar">
          <PersonelTanimlariTab
            personelId={id}
            core={core}
            banka={detail.banka}
            izinOzeti={detail.izinOzeti}
            telefonlar={detail.telefonlar}
            acilDurumKisileri={detail.acilDurumKisileri}
            onSaved={load}
          />
        </TabsContent>
        <TabsContent value="muhasebe">
          <MuhasebeEntegrasyonTab personelId={id} data={detail.muhasebeEntegrasyon} onSaved={load} />
        </TabsContent>
        <TabsContent value="banka">
          <BankaBilgileriTab personelId={id} core={core} izinOzeti={detail.izinOzeti} data={detail.banka} onSaved={load} />
        </TabsContent>
        <TabsContent value="kimlik">
          <KimlikBilgileriTab personelId={id} data={detail.kimlik} onSaved={load} />
        </TabsContent>
        <TabsContent value="egitimler">
          <EgitimlerTab personelId={id} egitimler={detail.egitimler} onSaved={load} />
        </TabsContent>
        <TabsContent value="izin">
          <IzinYonetimiTab personelId={id} core={core} banka={detail.banka} izinOzeti={detail.izinOzeti} izinler={detail.izinler} onSaved={load} />
        </TabsContent>
      </Tabs>
    </div>
  )
}
