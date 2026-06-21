import type { LucideIcon } from 'lucide-react'

export interface NavChild {
  title: string
  path: string
  icon: LucideIcon
}

export interface NavItem {
  title: string
  path?: string
  icon: LucideIcon
  exact?: boolean
  children?: NavChild[]
}

export interface NavGroup {
  label: string
  items: NavItem[]
}
