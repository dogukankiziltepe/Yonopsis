'use client'

import Link from 'next/link'
import { usePathname, useRouter } from 'next/navigation'
import {
  Building2,
  Home,
  LogOut,
  Users,
  LayoutDashboard,
  Settings,
  FileText,
  CreditCard,
  Bell,
  ChevronRight,
  LucideIcon,
} from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { useAuthStore } from '@/lib/store/auth.store'
import { authApi } from '@/lib/api/auth'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { PageDto } from '@/types/page'

const iconMap: Record<string, LucideIcon> = {
  home: Home,
  building: Building2,
  building2: Building2,
  users: Users,
  dashboard: LayoutDashboard,
  settings: Settings,
  file: FileText,
  credit: CreditCard,
  bell: Bell,
}

function PageIcon({ name }: { name?: string }) {
  const Icon = name ? (iconMap[name.toLowerCase()] ?? Home) : Home
  return <Icon className="h-4 w-4 shrink-0" />
}

function NavItem({ page, active }: { page: PageDto; active: boolean }) {
  return (
    <Link
      href={page.route}
      data-permission={page.userPermission}
      className={cn(
        'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
        active
          ? 'bg-sidebar-accent text-sidebar-accent-foreground'
          : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
      )}
    >
      <PageIcon name={page.icon} />
      {page.label}
    </Link>
  )
}

export function Sidebar() {
  const pathname = usePathname()
  const router = useRouter()
  const { user, pages, clearTokens } = useAuthStore()

  const handleLogout = async () => {
    try { await authApi.logout() } catch {}
    clearTokens()
    router.push('/login')
  }

  // Üst seviye (parentId yok) → Order'a göre sırala
  const topLevel = pages
    .filter((p) => !p.parentId)
    .sort((a, b) => a.order - b.order)

  // Alt menüler
  const children = pages.filter((p) => !!p.parentId)

  return (
    <aside className="flex h-screen w-64 flex-col border-r bg-sidebar">
      <div className="flex h-14 items-center border-b px-4">
        <span className="font-semibold text-sidebar-foreground">Site Yönetimi</span>
      </div>

      <nav className="flex-1 overflow-y-auto px-2 py-3 space-y-0.5">
        {pages.length === 0 ? (
          // Skeleton
          <div className="space-y-1 px-1">
            {[1, 2, 3].map((i) => (
              <Skeleton key={i} className="h-9 w-full" />
            ))}
          </div>
        ) : (
          topLevel.map((page) => {
            const active = page.route === '/'
              ? pathname === '/'
              : pathname.startsWith(page.route)

            const pageChildren = children
              .filter((c) => c.parentId === page.name)
              .sort((a, b) => a.order - b.order)

            return (
              <div key={page.name}>
                <NavItem page={page} active={active} />
                {pageChildren.length > 0 && (
                  <div className="ml-4 mt-0.5 space-y-0.5 border-l border-sidebar-accent/30 pl-3">
                    {pageChildren.map((child) => {
                      const childActive = pathname.startsWith(child.route)
                      return <NavItem key={child.name} page={child} active={childActive} />
                    })}
                  </div>
                )}
              </div>
            )
          })
        )}
      </nav>

      {/* User */}
      <div className="border-t p-3">
        <div className="flex items-center gap-3 px-2 py-1.5 mb-1">
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground text-sm font-medium shrink-0">
            {user?.firstName?.[0]?.toUpperCase() ?? '?'}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium truncate text-sidebar-foreground">
              {user?.firstName} {user?.lastName}
            </p>
            <p className="text-xs text-sidebar-foreground/50 truncate">{user?.email}</p>
          </div>
        </div>
        <Button
          variant="ghost"
          size="sm"
          className="w-full justify-start text-sidebar-foreground/70 hover:text-sidebar-foreground"
          onClick={handleLogout}
        >
          <LogOut className="h-4 w-4 mr-2" />
          Çıkış Yap
        </Button>
      </div>
    </aside>
  )
}
