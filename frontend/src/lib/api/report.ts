import { siteApi } from './client'
import type {
  ReportSummary, ReportDateFilter,
  KasaBakiye, IsTakibiOgesi, AidatTahsilatAy, FinansalDurumNoktasi,
  Evrak, OdenecekFatura, DagilimDilimi, DuyuruOzet, BankaHesabi,
} from '@/types/report'

export const reportApi = {
  getSummary: () =>
    siteApi.get<ReportSummary>('/api/report/summary'),
  getKasalar: (filter?: ReportDateFilter) =>
    siteApi.get<KasaBakiye[]>('/api/report/kasalar', { params: filter }),
  getIsTakibi: (filter?: ReportDateFilter) =>
    siteApi.get<IsTakibiOgesi[]>('/api/report/is-takibi', { params: filter }),
  getAidatTahsilat: (filter?: ReportDateFilter) =>
    siteApi.get<AidatTahsilatAy[]>('/api/report/aidat-tahsilat', { params: filter }),
  getFinansalDurum: (filter?: ReportDateFilter) =>
    siteApi.get<FinansalDurumNoktasi[]>('/api/report/finansal-durum', { params: filter }),
  getGiderEvraklari: () =>
    siteApi.get<Evrak[]>('/api/report/gider-evraklari'),
  getGelirEvraklari: () =>
    siteApi.get<Evrak[]>('/api/report/gelir-evraklari'),
  getOdenecekFaturalar: () =>
    siteApi.get<OdenecekFatura[]>('/api/report/odenecek-faturalar'),
  getGiderDagilimi: (filter?: ReportDateFilter) =>
    siteApi.get<DagilimDilimi[]>('/api/report/gider-dagilimi', { params: filter }),
  getGelirDagilimi: (filter?: ReportDateFilter) =>
    siteApi.get<DagilimDilimi[]>('/api/report/gelir-dagilimi', { params: filter }),
  getDuyurular: () =>
    siteApi.get<DuyuruOzet[]>('/api/report/duyurular'),
  getBankaHesaplari: () =>
    siteApi.get<BankaHesabi[]>('/api/report/banka-hesaplari'),
}
