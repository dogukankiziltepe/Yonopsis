'use client'

import { useEffect, useState } from 'react'
import { useAuthStore } from '@/lib/store/auth.store'
import { unitsApi } from '@/lib/api/units'
import { UnitSummary, UnitStatusLabel } from '@/types/unit'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Building2, Home, Layers } from 'lucide-react'

export default function OwnerPage() {
  const user = useAuthStore((s) => s.user)
  const [units, setUnits] = useState<UnitSummary[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    unitsApi.getMyUnitsAsOwner()
      .then((res) => setUnits(res.data ?? []))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Ana Sayfa</h1>
        <p className="text-muted-foreground">
          Hoş geldiniz, {user?.firstName} {user?.lastName}
        </p>
      </div>

      <div>
        <h2 className="text-lg font-semibold mb-3">Dairelerim</h2>
        {loading ? (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {[1, 2].map((i) => (
              <Card key={i} className="animate-pulse">
                <CardContent className="h-32" />
              </Card>
            ))}
          </div>
        ) : units.length === 0 ? (
          <Card>
            <CardContent className="py-8 text-center text-muted-foreground">
              Henüz atanmış bir daireniz bulunmamaktadır.
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {units.map((unit) => (
              <Card key={unit.id}>
                <CardHeader className="flex flex-row items-start justify-between pb-2">
                  <div className="flex items-center gap-2">
                    <Home className="h-4 w-4 text-muted-foreground shrink-0" />
                    <CardTitle className="text-sm font-medium">
                      Daire {unit.doorNumber}
                      {unit.code && <span className="text-muted-foreground ml-1">({unit.code})</span>}
                    </CardTitle>
                  </div>
                  <Badge variant={unit.status === 0 ? 'secondary' : unit.status === 2 ? 'default' : 'outline'}>
                    {UnitStatusLabel[unit.status]}
                  </Badge>
                </CardHeader>
                <CardContent className="space-y-1.5">
                  {unit.buildingName && (
                    <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                      <Building2 className="h-3.5 w-3.5" />
                      {unit.buildingName}
                    </div>
                  )}
                  {unit.floor && (
                    <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                      <Layers className="h-3.5 w-3.5" />
                      {unit.floor}. Kat
                    </div>
                  )}
                  {unit.monthlyFee != null && (
                    <p className="text-xs text-muted-foreground">
                      Aylık: <span className="font-medium text-foreground">{unit.monthlyFee.toLocaleString('tr-TR')} ₺</span>
                    </p>
                  )}
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium">Aktif Talepler</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-muted-foreground">Yakında kullanılabilir olacak.</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium">Son Bildirimler</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-muted-foreground">Yakında kullanılabilir olacak.</p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
