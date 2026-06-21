'use client'

import { useState } from 'react'
import Link from 'next/link'
import { ChevronDown } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import type { NavGroup, NavItem } from './nav-types'

interface NavItemRowProps {
  item: NavItem
  pathname: string
}

function NavItemRow({ item, pathname }: NavItemRowProps) {
  const childActive = item.children?.some((c) => pathname.startsWith(c.path)) ?? false
  const [manualOpen, setManualOpen] = useState(false)
  const open = childActive || manualOpen

  const selfActive = item.path
    ? item.exact
      ? pathname === item.path
      : pathname.startsWith(item.path)
    : false
  const active = selfActive || childActive

  if (item.children?.length) {
    return (
      <div>
        <button
          onClick={() => { if (!childActive) setManualOpen((v) => !v) }}
          className={cn(
            'w-full flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
            active
              ? 'bg-sidebar-accent text-sidebar-accent-foreground'
              : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
          )}
        >
          <item.icon className="h-4 w-4 shrink-0" />
          <span className="flex-1 text-left">{item.title}</span>
          <ChevronDown
            className={cn(
              'h-3.5 w-3.5 shrink-0 transition-transform duration-200',
              open && 'rotate-180'
            )}
          />
        </button>

        {open && (
          <div className="ml-4 mt-0.5 space-y-0.5 border-l border-sidebar-accent/30 pl-3">
            {item.children.map((child) => {
              const isActive = pathname.startsWith(child.path)
              return (
                <Link
                  key={child.path}
                  href={child.path}
                  className={cn(
                    'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                      : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
                  )}
                >
                  <child.icon className="h-4 w-4 shrink-0" />
                  {child.title}
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
      href={item.path!}
      className={cn(
        'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
        active
          ? 'bg-sidebar-accent text-sidebar-accent-foreground'
          : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-accent-foreground'
      )}
    >
      <item.icon className="h-4 w-4 shrink-0" />
      {item.title}
    </Link>
  )
}

interface SidebarNavGroupsProps {
  groups: NavGroup[]
  pathname: string
}

export function SidebarNavGroups({ groups, pathname }: SidebarNavGroupsProps) {
  return (
    <div className="space-y-4">
      {groups.map((group) => (
        <div key={group.label}>
          <p className="px-3 mb-1 text-xs font-semibold tracking-wider uppercase text-sidebar-foreground/40">
            {group.label}
          </p>
          <div className="space-y-0.5">
            {group.items.map((item) => (
              <NavItemRow key={item.title} item={item} pathname={pathname} />
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
