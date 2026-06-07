'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Menu } from 'lucide-react'
import { useAuthStore } from '@/lib/store/auth.store'
import { getMyPages } from '@/lib/api/pages'
import { Sidebar } from '@/components/layout/Sidebar'

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const user = useAuthStore((s) => s.user)
  const pages = useAuthStore((s) => s.pages)
  const setPages = useAuthStore((s) => s.setPages)
  const router = useRouter()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  useEffect(() => {
    if (user !== null && user.userType !== 'Management') {
      router.replace('/login')
    }
  }, [user, router])

  useEffect(() => {
    if (user?.userType === 'Management' && pages.length === 0) {
      getMyPages().then((res) => {
        if (res.isSuccess && res.value) {
          setPages(res.value)
        }
      })
    }
  }, [user, pages.length, setPages])

  if (user?.userType !== 'Management') return null

  return (
    <div className="flex h-screen overflow-hidden">
      <Sidebar mobileOpen={sidebarOpen} onMobileClose={() => setSidebarOpen(false)} />
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="md:hidden flex items-center h-14 px-4 border-b sticky top-0 bg-background z-10">
          <button
            className="p-1 rounded text-foreground/60 hover:text-foreground"
            onClick={() => setSidebarOpen(true)}
          >
            <Menu className="h-5 w-5" />
          </button>
          <span className="ml-3 font-semibold text-sm">Site Yönetimi</span>
        </div>
        <div className="container mx-auto px-4 md:px-6 py-4 md:py-6 max-w-7xl">
          {children}
        </div>
      </main>
    </div>
  )
}
