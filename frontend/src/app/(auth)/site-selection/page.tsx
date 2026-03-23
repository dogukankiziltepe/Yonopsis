'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Building2, LogOut, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { authApi } from '@/lib/api/auth'
import { getMyPages } from '@/lib/api/pages'
import { useAuthStore } from '@/lib/store/auth.store'
import { UserSiteListDto } from '@/types/auth'

export default function SiteSelectionPage() {
  const router = useRouter()
  const { setSiteToken, setPages, clearTokens } = useAuthStore()
  const [sites, setSites] = useState<UserSiteListDto[]>([])
  const [loading, setLoading] = useState(true)
  const [selecting, setSelecting] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    authApi.mySites()
      .then((res) => setSites(res.data ?? []))
      .catch(() => setError('Siteler yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [])

  const handleSelect = async (siteId: string, userType: string) => {
    setSelecting(`${siteId}:${userType}`)
    setError(null)
    try {
      const res = await authApi.selectSite({ siteId, userType })
      setSiteToken(res.data.accessToken, { siteId: res.data.siteId, siteName: res.data.siteName, userType: res.data.userType, roleTypeId: res.data.roleTypeId, roleName: res.data.roleName })

      const pagesRes = await getMyPages()
      if (pagesRes.isSuccess && pagesRes.value) {
        setPages(pagesRes.value)
      }

      const redirectMap: Record<string, string> = {
        Management: '/',
        Owner: '/owner',
        Renter: '/tenant',
      }
      router.push(redirectMap[res.data.userType] ?? '/')
    } catch {
      setError('Site seçimi başarısız.')
      setSelecting(null)
    }
  }

  const handleLogout = async () => {
    try { await authApi.logout() } catch {}
    clearTokens()
    router.push('/login')
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Site Seçimi</CardTitle>
              <CardDescription>Yönetmek istediğiniz siteyi seçin</CardDescription>
            </div>
            <Button variant="ghost" size="sm" onClick={handleLogout}>
              <LogOut className="h-4 w-4 mr-1" />
              Çıkış
            </Button>
          </div>
        </CardHeader>
        <CardContent className="space-y-2">
          {loading && (
            <div className="text-center py-8 text-muted-foreground text-sm">Yükleniyor...</div>
          )}

          {!loading && sites.length === 0 && (
            <div className="text-center py-8 text-muted-foreground text-sm">
              Onaylanmış siteniz bulunmamaktadır.
            </div>
          )}

          {sites.map((site) =>
            site.userTypes.map((userType) => {
              const key = `${site.siteId}:${userType}`
              return (
                <button
                  key={key}
                  onClick={() => handleSelect(site.siteId, userType)}
                  disabled={!!selecting}
                  className="w-full flex items-center gap-3 p-4 rounded-lg border hover:bg-muted/50 transition-colors text-left disabled:opacity-50"
                >
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 shrink-0">
                    <Building2 className="h-5 w-5 text-primary" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium truncate">{site.siteName}</p>
                    <p className="text-xs text-muted-foreground">{userType}</p>
                  </div>
                  <div className="flex items-center gap-2 shrink-0">
                    {selecting === key ? (
                      <span className="text-xs text-muted-foreground">Seçiliyor...</span>
                    ) : (
                      <ChevronRight className="h-4 w-4 text-muted-foreground" />
                    )}
                  </div>
                </button>
              )
            })
          )}

          {error && (
            <div className="rounded-md bg-destructive/10 border border-destructive/20 px-3 py-2">
              <p className="text-sm text-destructive">{error}</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
