'use client'

import { useEffect } from 'react'
import { useAuthStore } from '@/lib/store/auth.store'
import { reportApi } from '@/lib/api/report'
import { useReportSection, useReportRefresh } from '@/hooks/useReportSection'
import { WidgetCard } from '@/components/dashboard/WidgetCard'
import { KasalarTable } from '@/components/dashboard/KasalarTable'
import { IsTakibiTable } from '@/components/dashboard/IsTakibiTable'
import { AidatTahsilatChart } from '@/components/dashboard/AidatTahsilatChart'
import { FinansalDurumChart } from '@/components/dashboard/FinansalDurumChart'
import { EvrakTable } from '@/components/dashboard/EvrakTable'
import { OdenecekFaturalarTable } from '@/components/dashboard/OdenecekFaturalarTable'
import { DagilimPieChart } from '@/components/dashboard/DagilimPieChart'
import { DuyurularTable } from '@/components/dashboard/DuyurularTable'
import { BankaHesaplariPanel } from '@/components/dashboard/BankaHesaplariPanel'

export default function DashboardPage() {
  const user = useAuthStore((s) => s.user)

  const kasalar = useReportSection(reportApi.getKasalar, [])
  const isTakibi = useReportSection(reportApi.getIsTakibi, [])
  const aidatTahsilat = useReportSection(reportApi.getAidatTahsilat, [])
  const finansalDurum = useReportSection(reportApi.getFinansalDurum, [])
  const giderDagilimi = useReportSection(reportApi.getGiderDagilimi, [])
  const gelirDagilimi = useReportSection(reportApi.getGelirDagilimi, [])

  const giderEvraklari = useReportRefresh(reportApi.getGiderEvraklari, [])
  const gelirEvraklari = useReportRefresh(reportApi.getGelirEvraklari, [])
  const odenecekFaturalar = useReportRefresh(reportApi.getOdenecekFaturalar, [])
  const duyurular = useReportRefresh(reportApi.getDuyurular, [])
  const bankaHesaplari = useReportRefresh(reportApi.getBankaHesaplari, [])

  useEffect(() => {
    reportApi.getSummary().then(({ data }) => {
      kasalar.setData(data.kasalar)
      isTakibi.setData(data.isTakibi)
      aidatTahsilat.setData(data.aidatTahsilat)
      finansalDurum.setData(data.finansalDurum)
      giderDagilimi.setData(data.giderDagilimi)
      gelirDagilimi.setData(data.gelirDagilimi)
      giderEvraklari.setData(data.giderEvraklari)
      gelirEvraklari.setData(data.gelirEvraklari)
      odenecekFaturalar.setData(data.odenecekFaturalar)
      duyurular.setData(data.duyurular)
      bankaHesaplari.setData(data.bankaHesaplari)
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Özet</h1>
        <p className="text-muted-foreground">
          Hoş geldiniz, {user?.firstName} {user?.lastName}
        </p>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <WidgetCard
          title="Kasalar" loading={kasalar.loading} onRefresh={() => kasalar.load()}
          filter={kasalar.filter} onFilterChange={kasalar.changeFilter}
        >
          <KasalarTable data={kasalar.data} loading={kasalar.loading} />
        </WidgetCard>

        <WidgetCard
          title="İş Takibi" loading={isTakibi.loading} onRefresh={() => isTakibi.load()}
          filter={isTakibi.filter} onFilterChange={isTakibi.changeFilter}
        >
          <IsTakibiTable data={isTakibi.data} loading={isTakibi.loading} />
        </WidgetCard>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <WidgetCard
          title="Aidat Tahsilat Durumu" loading={aidatTahsilat.loading} onRefresh={() => aidatTahsilat.load()}
          filter={aidatTahsilat.filter} onFilterChange={aidatTahsilat.changeFilter}
        >
          <AidatTahsilatChart data={aidatTahsilat.data} loading={aidatTahsilat.loading} />
        </WidgetCard>

        <WidgetCard
          title="Finansal Durum" loading={finansalDurum.loading} onRefresh={() => finansalDurum.load()}
          filter={finansalDurum.filter} onFilterChange={finansalDurum.changeFilter}
        >
          <FinansalDurumChart data={finansalDurum.data} loading={finansalDurum.loading} />
        </WidgetCard>
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <WidgetCard title="Son Eklenen Gider Evrakları" loading={giderEvraklari.loading} onRefresh={() => giderEvraklari.load()}>
          <EvrakTable data={giderEvraklari.data} loading={giderEvraklari.loading} cariLabel="Firma/Kişi/Gider" emptyText="Gider evrakı bulunmuyor." />
        </WidgetCard>

        <WidgetCard title="Son Eklenen Gelir Evrakları" loading={gelirEvraklari.loading} onRefresh={() => gelirEvraklari.load()}>
          <EvrakTable data={gelirEvraklari.data} loading={gelirEvraklari.loading} cariLabel="Firma/Kişi/Gelir" emptyText="Gelir evrakı bulunmuyor." />
        </WidgetCard>

        <WidgetCard title="Ödenecek Faturalar" loading={odenecekFaturalar.loading} onRefresh={() => odenecekFaturalar.load()}>
          <OdenecekFaturalarTable data={odenecekFaturalar.data} loading={odenecekFaturalar.loading} />
        </WidgetCard>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <WidgetCard
          title="Gider Dağılımı" loading={giderDagilimi.loading} onRefresh={() => giderDagilimi.load()}
          filter={giderDagilimi.filter} onFilterChange={giderDagilimi.changeFilter}
        >
          <DagilimPieChart data={giderDagilimi.data} loading={giderDagilimi.loading} emptyText="Gider verisi bulunmuyor." />
        </WidgetCard>

        <WidgetCard
          title="Gelir Dağılımı" loading={gelirDagilimi.loading} onRefresh={() => gelirDagilimi.load()}
          filter={gelirDagilimi.filter} onFilterChange={gelirDagilimi.changeFilter}
        >
          <DagilimPieChart data={gelirDagilimi.data} loading={gelirDagilimi.loading} emptyText="Gelir verisi bulunmuyor." />
        </WidgetCard>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <WidgetCard title="Duyurular" loading={duyurular.loading} onRefresh={() => duyurular.load()}>
          <DuyurularTable data={duyurular.data} loading={duyurular.loading} />
        </WidgetCard>

        <WidgetCard title="Banka Hesapları" loading={bankaHesaplari.loading} onRefresh={() => bankaHesaplari.load()}>
          <BankaHesaplariPanel data={bankaHesaplari.data} loading={bankaHesaplari.loading} />
        </WidgetCard>
      </div>
    </div>
  )
}
