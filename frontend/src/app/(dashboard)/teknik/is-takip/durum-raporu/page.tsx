'use client'

import { useEffect, useState } from 'react'
import { Badge } from '@/components/ui/badge'
import { isEmirleriApi } from '@/lib/api/teknik'
import type { IsEmri } from '@/types/teknik'
import { IsEmriDurum, IsEmriDurumLabel, IsEmriOncelik, IsEmriOncelikLabel } from '@/types/teknik'

const statusOrder = [
  IsEmriDurum.YeniTalep,
  IsEmriDurum.Atandi,
  IsEmriDurum.Devam,
  IsEmriDurum.Tamamlandi,
  IsEmriDurum.Iptal,
]

const durumColor: Record<IsEmriDurum, string> = {
  [IsEmriDurum.YeniTalep]: 'bg-slate-100 text-slate-700 border-slate-200',
  [IsEmriDurum.Atandi]: 'bg-blue-50 text-blue-700 border-blue-200',
  [IsEmriDurum.Devam]: 'bg-yellow-50 text-yellow-700 border-yellow-200',
  [IsEmriDurum.Tamamlandi]: 'bg-green-50 text-green-700 border-green-200',
  [IsEmriDurum.Iptal]: 'bg-red-50 text-red-700 border-red-200',
}

const durumVariant: Record<IsEmriDurum, 'default' | 'secondary' | 'outline' | 'destructive'> = {
  [IsEmriDurum.YeniTalep]: 'secondary',
  [IsEmriDurum.Atandi]: 'outline',
  [IsEmriDurum.Devam]: 'default',
  [IsEmriDurum.Tamamlandi]: 'secondary',
  [IsEmriDurum.Iptal]: 'destructive',
}

const oncelikColor: Record<IsEmriOncelik, string> = {
  [IsEmriOncelik.Dusuk]: 'text-slate-500',
  [IsEmriOncelik.Normal]: 'text-blue-500',
  [IsEmriOncelik.Yuksek]: 'text-orange-500',
  [IsEmriOncelik.Kritik]: 'text-red-600 font-bold',
}

export default function PageComponent() {
  const [pano, setPano] = useState<Record<string, IsEmri[]>>({})
  const [loading, setLoading] = useState(true)
  const [selected, setSelected] = useState<IsEmriDurum | null>(null)

  useEffect(() => {
    isEmirleriApi.getPano().then(r => {
      if (r.success && r.data) setPano(r.data)
      setLoading(false)
    })
  }, [])

  const total = Object.values(pano).reduce((s, a) => s + a.length, 0)

  const getItems = (durum: IsEmriDurum): IsEmri[] => {
    const key = String(durum)
    return pano[key] ?? []
  }

  const displayItems = selected !== null ? getItems(selected) : Object.values(pano).flat()

  return (
    <div className="flex flex-col h-full gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">İş Takibi Durum Raporu</h1>
        <span className="text-sm text-muted-foreground">Toplam {total} iş emri</span>
      </div>

      {loading ? (
        <div className="border rounded-lg flex items-center justify-center py-16 text-muted-foreground">Yükleniyor...</div>
      ) : (
        <>
          {/* Summary cards */}
          <div className="grid grid-cols-2 sm:grid-cols-5 gap-2">
            {statusOrder.map(durum => {
              const count = getItems(durum).length
              const isActive = selected === durum
              return (
                <button
                  key={durum}
                  onClick={() => setSelected(isActive ? null : durum)}
                  className={`border rounded-lg p-3 text-left transition-all hover:shadow-sm ${durumColor[durum]} ${isActive ? 'ring-2 ring-offset-1 ring-current' : ''}`}
                >
                  <div className="text-2xl font-bold">{count}</div>
                  <div className="text-xs mt-1 font-medium">{IsEmriDurumLabel[durum]}</div>
                </button>
              )
            })}
          </div>

          {/* Work orders list */}
          <div className="border rounded-lg overflow-auto flex-1">
            <div className="bg-muted/50 px-3 py-2 text-sm font-medium border-b">
              {selected !== null ? IsEmriDurumLabel[selected] : 'Tüm İş Emirleri'} — {displayItems.length} kayıt
            </div>
            <table className="w-full text-sm">
              <thead className="bg-muted/30">
                <tr>
                  <th className="text-left px-3 py-2 font-medium">Başlık</th>
                  <th className="text-left px-3 py-2 font-medium">Departman</th>
                  <th className="text-left px-3 py-2 font-medium">Atanan</th>
                  <th className="text-left px-3 py-2 font-medium">Öncelik</th>
                  <th className="text-left px-3 py-2 font-medium">Durum</th>
                </tr>
              </thead>
              <tbody>
                {displayItems.length === 0 ? (
                  <tr><td colSpan={5} className="text-center py-8 text-muted-foreground">Kayıt yok</td></tr>
                ) : displayItems.map(item => (
                  <tr key={item.id} className="border-t hover:bg-muted/30">
                    <td className="px-3 py-2 max-w-xs truncate" title={item.baslik}>{item.baslik}</td>
                    <td className="px-3 py-2 text-muted-foreground">{item.departmanAdi ?? '—'}</td>
                    <td className="px-3 py-2 text-muted-foreground">{item.atananKisiAdi ?? '—'}</td>
                    <td className={`px-3 py-2 ${oncelikColor[item.oncelik]}`}>{IsEmriOncelikLabel[item.oncelik]}</td>
                    <td className="px-3 py-2">
                      <Badge variant={durumVariant[item.durum]}>{IsEmriDurumLabel[item.durum]}</Badge>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  )
}
