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
  HelpCircle,
  Car,
  Key,
  Shield,
  Layers,
  UserCircle,
  X,
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
  'credit-card': CreditCard,
  credit: CreditCard,
  bell: Bell,
  'help-circle': HelpCircle,
  car: Car,
  key: Key,
  shield: Shield,
  layers: Layers,
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

interface SidebarProps {
  mobileOpen?: boolean
  onMobileClose?: () => void
}

export function Sidebar({ mobileOpen = false, onMobileClose }: SidebarProps) {
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
    <>
      {mobileOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 md:hidden"
          onClick={onMobileClose}
        />
      )}
    <aside className={cn(
      "flex h-screen w-64 flex-col border-r bg-sidebar",
      "fixed md:static inset-y-0 left-0 z-50 transition-transform duration-200",
      mobileOpen ? "translate-x-0" : "-translate-x-full md:translate-x-0"
    )}>
      <div className="flex h-14 items-center justify-between border-b px-4">
        <span className="font-semibold text-sidebar-foreground">Site Yönetimi</span>
        <button
          className="md:hidden p-1 rounded text-sidebar-foreground/60 hover:text-sidebar-foreground"
          onClick={onMobileClose}
        >
          <X className="h-4 w-4" />
        </button>
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
              .filter((c) => c.parentId === page.id)
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
        <Link
          href="/profile"
          className="flex items-center gap-3 px-2 py-1.5 mb-1 rounded-md hover:bg-sidebar-accent/50 transition-colors"
        >
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-primary-foreground text-sm font-medium shrink-0">
            {user?.firstName?.[0]?.toUpperCase() ?? '?'}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium truncate text-sidebar-foreground">
              {user?.firstName} {user?.lastName}
            </p>
            <p className="text-xs text-sidebar-foreground/50 truncate">{user?.email}</p>
          </div>
          <UserCircle className="h-4 w-4 text-sidebar-foreground/40 shrink-0" />
        </Link>
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
    </>
  )
}
