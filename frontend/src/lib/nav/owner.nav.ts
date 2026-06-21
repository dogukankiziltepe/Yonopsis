import {
  LayoutDashboard,
  Megaphone,
  Receipt,
  CalendarCheck,
  Wrench,
  Inbox,
  Plus,
  CalendarDays,
  Calendar,
  Gavel,
  Bell,
  CircleUser,
} from 'lucide-react'
import type { NavGroup } from '@/components/layout/nav-types'

export const ownerNavGroups: NavGroup[] = [
  {
    label: 'Genel',
    items: [
      {
        title: 'Gösterge Paneli',
        path: '/owner',
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
        path: '/owner/announcements',
        icon: Megaphone,
      },
      {
        title: 'Aidatlar & Borçlar',
        path: '/owner/dues',
        icon: Receipt,
      },
      {
        title: 'Ortak Alan Rezervasyonu',
        path: '/owner/reservations',
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
          { title: 'Taleplerim', path: '/owner/requests/my', icon: Inbox },
          { title: 'Yeni Talep', path: '/owner/requests/new', icon: Plus },
        ],
      },
    ],
  },
  {
    label: 'Toplantılar',
    items: [
      {
        title: 'Toplantı & Kararlar',
        icon: CalendarDays,
        children: [
          { title: 'Toplantılar', path: '/owner/meetings', icon: Calendar },
          { title: 'Alınan Kararlar', path: '/owner/meetings/decisions', icon: Gavel },
        ],
      },
    ],
  },
  {
    label: 'Ayarlar',
    items: [
      {
        title: 'Bildirim Ayarları',
        path: '/owner/settings/notifications',
        icon: Bell,
      },
      {
        title: 'Profilim',
        path: '/owner/profile',
        icon: CircleUser,
      },
    ],
  },
]
