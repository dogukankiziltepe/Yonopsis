'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/lib/store/auth.store'
import { OwnerSidebar } from '@/components/layout/OwnerSidebar'

export default function OwnerLayout({ children }: { children: React.ReactNode }) {
  const user = useAuthStore((s) => s.user)
  const router = useRouter()

  useEffect(() => {
    if (user !== null && user.userType !== 'Owner') {
      router.replace('/login')
    }
  }, [user, router])

  if (user?.userType !== 'Owner') return null

  return (
    <div className="flex h-screen overflow-hidden">
      <OwnerSidebar />
      <main className="flex-1 overflow-y-auto bg-background">
        <div className="container mx-auto px-6 py-6 max-w-7xl">
          {children}
        </div>
      </main>
    </div>
  )
}
