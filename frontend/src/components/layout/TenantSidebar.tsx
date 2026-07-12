'use client'

import { usePathname, useRouter } from 'next/navigation'
import { LogOut, X } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { useAuthStore } from '@/lib/store/auth.store'
import { authApi } from '@/lib/api/auth'
import { Button } from '@/components/ui/button'
import { ThemeToggle } from '@/components/theme-toggle'
import { SidebarNavGroups } from './SidebarNavGroups'
import { tenantNavGroups } from '@/lib/nav/tenant.nav'

interface TenantSidebarProps {
  mobileOpen?: boolean
  onMobileClose?: () => void
}

export function TenantSidebar({ mobileOpen = false, onMobileClose }: TenantSidebarProps) {
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
        'flex h-screen w-64 flex-col border-r bg-sidebar',
        'fixed md:static inset-y-0 left-0 z-50 transition-transform duration-200',
        mobileOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'
      )}>
        <div className="flex h-14 items-center justify-between border-b px-4">
          <span className="font-semibold text-sidebar-foreground">Tenant Panel</span>
          <button
            className="md:hidden p-1 rounded text-sidebar-foreground/60 hover:text-sidebar-foreground"
            onClick={onMobileClose}
          >
            <X className="h-4 w-4" />
          </button>
        </div>

        <nav className="flex-1 overflow-y-auto px-2 py-3">
          <SidebarNavGroups groups={tenantNavGroups} pathname={pathname} />
        </nav>

        <div className="border-t p-3">
          <div className="flex items-center gap-3 px-2 py-1.5 mb-1 rounded-md">
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
          <div className="flex items-center gap-1">
            <Button
              variant="ghost"
              size="sm"
              className="flex-1 justify-start text-sidebar-foreground/70 hover:text-sidebar-foreground"
              onClick={handleLogout}
            >
              <LogOut className="h-4 w-4 mr-2" />
              Log Out
            </Button>
            <ThemeToggle className="text-sidebar-foreground/70 hover:text-sidebar-foreground" />
          </div>
        </div>
      </aside>
    </>
  )
}
