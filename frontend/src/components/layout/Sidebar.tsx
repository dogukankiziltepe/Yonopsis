'use client'

import { useState } from 'react'
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
  ChevronDown,
  LucideIcon,
  HelpCircle,
  Car,
  Key,
  Shield,
  Layers,
  UserCircle,
  X,
  Megaphone,
  Wrench,
  Receipt,
  DoorOpen,
  Upload,
  CircleUser,
  Book,
  BookOpen,
  BarChart3,
  Settings2,
  Calendar,
  CalendarCheck,
  FileSearch,
  Calculator,
  Gavel,
  CalendarDays,
  UserCheck,
  Package,
  Landmark,
  Boxes,
  ClipboardList,
  PieChart,
  TrendingUp,
  TrendingDown,
  ArrowLeftRight,
  FileSpreadsheet,
  UserCog,
  List,
} from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { useAuthStore } from '@/lib/store/auth.store'
import { authApi } from '@/lib/api/auth'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { ThemeToggle } from '@/components/theme-toggle'
import { PageDto } from '@/types/page'

const iconMap: Record<string, LucideIcon> = {
  home: Home,
  building: Building2,
  building2: Building2,
  users: Users,
  dashboard: LayoutDashboard,
  'layout-dashboard': LayoutDashboard,
  settings: Settings,
  settings2: Settings2,
  file: FileText,
  'file-text': FileText,
  'credit-card': CreditCard,
  credit: CreditCard,
  bell: Bell,
  'help-circle': HelpCircle,
  car: Car,
  key: Key,
  shield: Shield,
  layers: Layers,
  megaphone: Megaphone,
  wrench: Wrench,
  receipt: Receipt,
  'door-open': DoorOpen,
  dooropen: DoorOpen,
  upload: Upload,
  'circle-user': CircleUser,
  circleuser: CircleUser,
  book: Book,
  'book-open': BookOpen,
  bookopen: BookOpen,
  'bar-chart': BarChart3,
  'bar-chart3': BarChart3,
  barchart3: BarChart3,
  calendar: Calendar,
  'calendar-check': CalendarCheck,
  calendarcheck: CalendarCheck,
  'calendar-days': CalendarDays,
  calendardays: CalendarDays,
  'file-search': FileSearch,
  filesearch: FileSearch,
  calculator: Calculator,
  gavel: Gavel,
  'user-check': UserCheck,
  usercheck: UserCheck,
  package: Package,
  landmark: Landmark,
  boxes: Boxes,
  'clipboard-list': ClipboardList,
  clipboardlist: ClipboardList,
  'pie-chart': PieChart,
  piechart: PieChart,
  'trending-up': TrendingUp,
  trendingup: TrendingUp,
  'trending-down': TrendingDown,
  trendingdown: TrendingDown,
  'arrow-left-right': ArrowLeftRight,
  arrowleftright: ArrowLeftRight,
  'file-spreadsheet': FileSpreadsheet,
  filespreadsheet: FileSpreadsheet,
  'user-cog': UserCog,
  usercog: UserCog,
  list: List,
}

function PageIcon({ name }: { name?: string }) {
  const Icon = name ? (iconMap[name.toLowerCase()] ?? Home) : Home
  return <Icon className="h-4 w-4 shrink-0" />
}

interface NavItemProps {
  page: PageDto
  pathname: string
  children?: PageDto[]
}

function NavItem({ page, pathname, children = [] }: NavItemProps) {
  const selfActive = page.route === '/'
    ? pathname === '/'
    : pathname.startsWith(page.route)
  const childActive = children.some((c) => pathname.startsWith(c.route))
  const active = selfActive || childActive

  const [manualOpen, setManualOpen] = useState(false)
  const open = childActive || manualOpen

  if (children.length > 0) {
    return (
      <div>
        <button
          onClick={() => { if (!childActive) setManualOpen((v) => !v) }}
          data-permission={page.userPermission}
          className={cn(
            'w-full flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
            active
              ? 'bg-sidebar-accent text-sidebar-accent-foreground'
              : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
          )}
        >
          <PageIcon name={page.icon} />
          <span className="flex-1 text-left">{page.label}</span>
          <ChevronDown
            className={cn(
              'h-3.5 w-3.5 shrink-0 transition-transform duration-200',
              open && 'rotate-180'
            )}
          />
        </button>

        {open && (
          <div className="ml-4 mt-0.5 space-y-0.5 border-l border-sidebar-accent/30 pl-3">
            {children.map((child) => {
              const isActive = pathname.startsWith(child.route)
              return (
                <Link
                  key={child.name}
                  href={child.route}
                  data-permission={child.userPermission}
                  className={cn(
                    'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                      : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
                  )}
                >
                  <PageIcon name={child.icon} />
                  {child.label}
                </Link>
              )
            })}
          </div>
        )}
      </div>
    )
  }

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

  const topLevel = pages
    .filter((p) => !p.parentId)
    .sort((a, b) => a.order - b.order)

  const childrenByParent = pages
    .filter((p) => !!p.parentId)
    .reduce<Record<string, PageDto[]>>((acc, p) => {
      const key = p.parentId!
      acc[key] = acc[key] ? [...acc[key], p] : [p]
      return acc
    }, {})

  return (
    <>
      {mobileOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 md:hidden"
          onClick={onMobileClose}
        />
      )}
      <aside className={cn(
        'flex h-screen w-64 flex-col border-r bg-sidebar',
        'fixed md:static inset-y-0 left-0 z-50 transition-transform duration-200',
        mobileOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'
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
            <div className="space-y-1 px-1">
              {[1, 2, 3].map((i) => (
                <Skeleton key={i} className="h-9 w-full" />
              ))}
            </div>
          ) : (
            topLevel.map((page) => (
              <NavItem
                key={page.name}
                page={page}
                pathname={pathname}
                children={(childrenByParent[page.id] ?? []).sort((a, b) => a.order - b.order)}
              />
            ))
          )}
        </nav>

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
          <div className="flex items-center gap-1">
            <Button
              variant="ghost"
              size="sm"
              className="flex-1 justify-start text-sidebar-foreground/70 hover:text-sidebar-foreground"
              onClick={handleLogout}
            >
              <LogOut className="h-4 w-4 mr-2" />
              Çıkış Yap
            </Button>
            <ThemeToggle className="text-sidebar-foreground/70 hover:text-sidebar-foreground" />
          </div>
        </div>
      </aside>
    </>
  )
}
