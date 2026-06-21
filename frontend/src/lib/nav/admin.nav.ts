import {
  Building2,
  CreditCard,
  Package,
  ShieldCheck,
  List,
  Plus,
} from 'lucide-react'
import type { NavGroup } from '@/components/layout/nav-types'

export const adminNavGroups: NavGroup[] = [
  {
    label: 'Platform',
    items: [
      {
        title: 'Siteler',
        icon: Building2,
        children: [
          { title: 'Site Listesi', path: '/admin/sites', icon: List },
          { title: 'Site Ekle', path: '/admin/sites/new', icon: Plus },
        ],
      },
      {
        title: 'Abonelik Planları',
        path: '/admin/plans',
        icon: CreditCard,
      },
      {
        title: 'Modüller',
        path: '/admin/modules',
        icon: Package,
      },
      {
        title: 'Platform Kullanıcıları',
        path: '/admin/users',
        icon: ShieldCheck,
      },
    ],
  },
]
