'use client'

import { useEffect, useState } from 'react'
import { useAuthStore } from '@/lib/store/auth.store'
import { unitsApi } from '@/lib/api/units'
import { UnitDetail, UnitStatusLabel, UnitDirectionLabel } from '@/types/unit'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Building2, Home, Layers, Compass, User, Wifi } from 'lucide-react'

export default function TenantPage() {
  const user = useAuthStore((s) => s.user)
  const [unit, setUnit] = useState<UnitDetail | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    unitsApi.getMyUnitAsTenant()
      .then((res) => setUnit(res.data))
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
        <h2 className="text-lg font-semibold mb-3">Dairem</h2>
        {loading ? (
          <Card className="animate-pulse">
            <CardContent className="h-40" />
          </Card>
        ) : unit === null ? (
          <Card>
            <CardContent className="py-8 text-center text-muted-foreground">
              Henüz atanmış bir daireniz bulunmamaktadır.
            </CardContent>
          </Card>
        ) : (
          <Card>
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
            <CardContent>
              <div className="grid gap-2 sm:grid-cols-2">
                {unit.buildingName && (
                  <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Building2 className="h-4 w-4" />
                    <span>{unit.buildingName}</span>
                  </div>
                )}
                {unit.floor && (
                  <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Layers className="h-4 w-4" />
                    <span>{unit.floor}. Kat</span>
                  </div>
                )}
                {unit.direction != null && (
                  <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Compass className="h-4 w-4" />
                    <span>Cephe: {UnitDirectionLabel[unit.direction]}</span>
                  </div>
                )}
                {unit.internet && (
                  <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Wifi className="h-4 w-4" />
                    <span>İnternet: {unit.internet}</span>
                  </div>
                )}
                {unit.ownerFullName && (
                  <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <User className="h-4 w-4" />
                    <span>Ev Sahibi: {unit.ownerFullName}</span>
                  </div>
                )}
                {unit.monthlyFee != null && (
                  <div className="text-sm">
                    <span className="text-muted-foreground">Aylık Aidat: </span>
                    <span className="font-medium">{unit.monthlyFee.toLocaleString('tr-TR')} ₺</span>
                  </div>
                )}
                {unit.grossArea != null && (
                  <div className="text-sm">
                    <span className="text-muted-foreground">Brüt Alan: </span>
                    <span className="font-medium">{unit.grossArea} m²</span>
                  </div>
                )}
                {unit.netArea != null && (
                  <div className="text-sm">
                    <span className="text-muted-foreground">Net Alan: </span>
                    <span className="font-medium">{unit.netArea} m²</span>
                  </div>
                )}
              </div>
              {unit.description && (
                <p className="mt-3 text-sm text-muted-foreground border-t pt-3">{unit.description}</p>
              )}
            </CardContent>
          </Card>
        )}
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium">Açık Talepler</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-muted-foreground">Yakında kullanılabilir olacak.</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium">Son Duyurular</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-xs text-muted-foreground">Yakında kullanılabilir olacak.</p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
