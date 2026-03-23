'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/lib/store/auth.store'
import { getMyPages } from '@/lib/api/pages'
import { Sidebar } from '@/components/layout/Sidebar'

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const user = useAuthStore((s) => s.user)
  const pages = useAuthStore((s) => s.pages)
  const setPages = useAuthStore((s) => s.setPages)
  const router = useRouter()

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
      <Sidebar />
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="container mx-auto px-6 py-6 max-w-7xl">
          {children}
        </div>
      </main>
    </div>
  )
}
