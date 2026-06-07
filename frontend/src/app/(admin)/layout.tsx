'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Menu } from 'lucide-react'
import { useAuthStore } from '@/lib/store/auth.store'
import { AdminSidebar } from '@/components/layout/AdminSidebar'

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const user = useAuthStore((s) => s.user)
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  useEffect(() => {
    if (user === null) return
    if (!user.isSuperAdmin) {
      router.replace('/site-selection')
    }
  }, [user, router])

  if (user === null) return null
  if (!user.isSuperAdmin) return null

  return (
    <div className="flex h-screen overflow-hidden">
      <AdminSidebar mobileOpen={sidebarOpen} onMobileClose={() => setSidebarOpen(false)} />
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="md:hidden flex items-center h-14 px-4 border-b sticky top-0 bg-background z-10">
          <button
            className="p-1 rounded text-foreground/60 hover:text-foreground"
            onClick={() => setSidebarOpen(true)}
          >
            <Menu className="h-5 w-5" />
          </button>
          <span className="ml-3 font-semibold text-sm">Sistem Yönetimi</span>
        </div>
        <div className="container mx-auto px-4 md:px-6 py-4 md:py-6 max-w-7xl">
          {children}
        </div>
      </main>
    </div>
  )
}
