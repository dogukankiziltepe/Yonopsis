'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { useAuthStore } from '@/lib/store/auth.store'
import {
  Building2, Home, Users, HelpCircle, CreditCard, Bell, AlertCircle, CheckCircle2, Clock
} from 'lucide-react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { siteApi } from '@/lib/api/client'
import { BuildingSummary } from '@/types/building'
import { UnitSummary } from '@/types/unit'
import { PersonDto } from '@/types/person'
import { SupportRequestSummaryDto } from '@/types/supportRequest'
import { PaymentSummaryDto } from '@/types/payment'
import { AnnouncementSummaryDto } from '@/types/announcement'

interface Stats {
  buildingCount: number
  unitCount: number
  occupiedCount: number
  personCount: number
  openRequestCount: number
  pendingPaymentCount: number
  overduePaymentCount: number
  pinnedAnnouncements: AnnouncementSummaryDto[]
}

export default function DashboardPage() {
  const user = useAuthStore((s) => s.user)
  const [stats, setStats] = useState<Stats | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchAll = async () => {
      try {
        const [buildings, units, persons, requests, payments, announcements] = await Promise.allSettled([
          siteApi.get<BuildingSummary[]>('/api/buildings'),
          siteApi.get<UnitSummary[]>('/api/units'),
          siteApi.get<PersonDto[]>('/api/persons'),
          siteApi.get<SupportRequestSummaryDto[]>('/api/support-requests'),
          siteApi.get<PaymentSummaryDto[]>('/api/payments'),
          siteApi.get<AnnouncementSummaryDto[]>('/api/announcements'),
        ])

        const b = buildings.status === 'fulfilled' ? (buildings.value.data ?? []) : []
        const u = units.status === 'fulfilled' ? (units.value.data ?? []) : []
        const p = persons.status === 'fulfilled' ? (persons.value.data ?? []) : []
        const r = requests.status === 'fulfilled' ? (requests.value.data ?? []) : []
        const pay = payments.status === 'fulfilled' ? (payments.value.data ?? []) : []
        const ann = announcements.status === 'fulfilled' ? (announcements.value.data ?? []) : []

        setStats({
          buildingCount: b.length,
          unitCount: u.length,
          occupiedCount: u.filter((x) => x.status !== 0).length,
          personCount: p.length,
          openRequestCount: r.filter((x) => x.status === 0).length,
          pendingPaymentCount: pay.filter((x) => x.status === 0).length,
          overduePaymentCount: pay.filter((x) => x.status === 2).length,
          pinnedAnnouncements: ann.filter((x) => x.isPinned).slice(0, 3),
        })
      } catch {
        setStats(null)
      } finally {
        setLoading(false)
      }
    }
    fetchAll()
  }, [])

  const StatCard = ({
    href, icon: Icon, title, value, sub, color,
  }: {
    href: string
    icon: React.ElementType
    title: string
    value: number | string
    sub?: string
    color?: string
  }) => (
    <Link href={href}>
      <Card className="hover:shadow-md transition-shadow cursor-pointer">
        <CardHeader className="flex flex-row items-center justify-between pb-2">
          <CardTitle className="text-sm font-medium text-muted-foreground">{title}</CardTitle>
          <Icon className={`h-4 w-4 ${color ?? 'text-muted-foreground'}`} />
        </CardHeader>
        <CardContent>
          <p className="text-2xl font-bold">{loading ? '—' : value}</p>
          {sub && <p className="text-xs text-muted-foreground mt-0.5">{sub}</p>}
        </CardContent>
      </Card>
    </Link>
  )

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Ana Sayfa</h1>
        <p className="text-muted-foreground">
          Hoş geldiniz, {user?.firstName} {user?.lastName}
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <StatCard
          href="/buildings"
          icon={Building2}
          title="Blok"
          value={stats?.buildingCount ?? 0}
          sub="toplam blok"
        />
        <StatCard
          href="/units"
          icon={Home}
          title="Daire"
          value={stats?.unitCount ?? 0}
          sub={stats ? `${stats.occupiedCount} dolu, ${stats.unitCount - stats.occupiedCount} boş` : ''}
        />
        <StatCard
          href="/persons"
          icon={Users}
          title="Kişi"
          value={stats?.personCount ?? 0}
          sub="kayıtlı kişi"
        />
        <StatCard
          href="/support-requests"
          icon={HelpCircle}
          title="Açık Talep"
          value={stats?.openRequestCount ?? 0}
          sub="bekleyen destek talebi"
          color={stats && stats.openRequestCount > 0 ? 'text-yellow-500' : undefined}
        />
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium">Aidat Durumu</CardTitle>
            <CreditCard className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent className="space-y-2">
            {loading ? (
              <p className="text-muted-foreground text-sm">Yükleniyor...</p>
            ) : (
              <>
                <div className="flex items-center gap-2 text-sm">
                  <Clock className="h-4 w-4 text-yellow-500 shrink-0" />
                  <span className="text-muted-foreground">Bekleyen:</span>
                  <span className="font-semibold">{stats?.pendingPaymentCount ?? 0}</span>
                </div>
                <div className="flex items-center gap-2 text-sm">
                  <AlertCircle className="h-4 w-4 text-red-500 shrink-0" />
                  <span className="text-muted-foreground">Gecikmiş:</span>
                  <span className="font-semibold text-red-600">{stats?.overduePaymentCount ?? 0}</span>
                </div>
                <Link href="/payments" className="text-xs text-primary hover:underline block mt-1">
                  Tümünü gör →
                </Link>
              </>
            )}
          </CardContent>
        </Card>

        <Card className="col-span-1 lg:col-span-2">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium">Sabit Duyurular</CardTitle>
            <Bell className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            {loading ? (
              <p className="text-muted-foreground text-sm">Yükleniyor...</p>
            ) : !stats?.pinnedAnnouncements.length ? (
              <p className="text-muted-foreground text-sm">Sabit duyuru bulunmuyor.</p>
            ) : (
              <div className="space-y-2">
                {stats.pinnedAnnouncements.map((a) => (
                  <div key={a.id} className="flex items-start gap-2 text-sm">
                    <CheckCircle2 className="h-4 w-4 text-muted-foreground mt-0.5 shrink-0" />
                    <div>
                      <p className="font-medium leading-tight">{a.title}</p>
                      <p className="text-xs text-muted-foreground">
                        {new Date(a.createdAt).toLocaleDateString('tr-TR')}
                      </p>
                    </div>
                  </div>
                ))}
                <Link href="/announcements" className="text-xs text-primary hover:underline block mt-1">
                  Tüm duyurular →
                </Link>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
