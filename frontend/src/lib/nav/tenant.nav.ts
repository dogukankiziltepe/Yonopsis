import {
  LayoutDashboard,
  Megaphone,
  CalendarCheck,
  Wrench,
  Inbox,
  Plus,
  Gavel,
  Bell,
  CircleUser,
} from 'lucide-react'
import type { NavGroup } from '@/components/layout/nav-types'

export const tenantNavGroups: NavGroup[] = [
  {
    label: 'Genel',
    items: [
      {
        title: 'Gösterge Paneli',
        path: '/tenant',
        icon: LayoutDashboard,
        exact: true,
      },
    ],
  },
  {
    label: 'Bilgilerim',
    items: [
      {
        title: 'Duyurular',
        path: '/tenant/announcements',
        icon: Megaphone,
      },
      {
        title: 'Ortak Alan Rezervasyonu',
        path: '/tenant/reservations',
        icon: CalendarCheck,
      },
    ],
  },
  {
    label: 'Talepler',
    items: [
      {
        title: 'Talepler & Arızalar',
        icon: Wrench,
        children: [
          { title: 'Taleplerim', path: '/tenant/requests/my', icon: Inbox },
          { title: 'Yeni Talep', path: '/tenant/requests/new', icon: Plus },
        ],
      },
    ],
  },
  {
    label: 'Toplantılar',
    items: [
      {
        title: 'Alınan Kararlar',
        path: '/tenant/meetings/decisions',
        icon: Gavel,
      },
    ],
  },
  {
    label: 'Ayarlar',
    items: [
      {
        title: 'Bildirim Ayarları',
        path: '/tenant/settings/notifications',
        icon: Bell,
      },
      {
        title: 'Profilim',
        path: '/tenant/profile',
        icon: CircleUser,
      },
    ],
  },
]
