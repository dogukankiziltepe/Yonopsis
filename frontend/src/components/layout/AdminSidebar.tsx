'use client'

import Link from 'next/link'
import { usePathname, useRouter } from 'next/navigation'
import { Building2, LogOut, Package, X } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { useAuthStore } from '@/lib/store/auth.store'
import { authApi } from '@/lib/api/auth'
import { Button } from '@/components/ui/button'

interface AdminSidebarProps {
  mobileOpen?: boolean
  onMobileClose?: () => void
}

const navItems = [
  { href: '/admin/sites', label: 'Siteler', icon: Building2 },
  { href: '/admin/plans', label: 'Planlar', icon: Package },
]

export function AdminSidebar({ mobileOpen = false, onMobileClose }: AdminSidebarProps) {
  const pathname = usePathname()
  const router = useRouter()
  const { user, clearTokens } = useAuthStore()

  const handleLogout = async () => {
    try { await authApi.logout() } catch {}
    clearTokens()
    router.push('/login')
  }

  return (
    <>
      {mobileOpen && (
        <div className="fixed inset-0 bg-black/50 z-40 md:hidden" onClick={onMobileClose} />
      )}
    <aside className={cn(
      "flex h-screen w-64 flex-col border-r bg-sidebar",
      "fixed md:static inset-y-0 left-0 z-50 transition-transform duration-200",
      mobileOpen ? "translate-x-0" : "-translate-x-full md:translate-x-0"
    )}>
      <div className="flex h-14 items-center justify-between border-b px-4">
        <span className="font-semibold text-sidebar-foreground">Sistem Yönetimi</span>
        <button className="md:hidden p-1 rounded text-sidebar-foreground/60 hover:text-sidebar-foreground" onClick={onMobileClose}>
          <X className="h-4 w-4" />
        </button>
      </div>

      <nav className="flex-1 overflow-y-auto px-2 py-3 space-y-0.5">
        {navItems.map((item) => {
          const active = pathname.startsWith(item.href)
          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                active
                  ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                  : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
              )}
            >
              <item.icon className="h-4 w-4 shrink-0" />
              {item.label}
            </Link>
          )
        })}
      </nav>

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
    </>
  )
}
