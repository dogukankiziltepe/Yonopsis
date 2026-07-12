'use client'

import { useEffect, useState, useCallback } from 'react'
import { RefreshCw, ClipboardList } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { isEmirleriApi } from '@/lib/api/teknik'
import type { IsEmri } from '@/types/teknik'
import { IsEmriDurum, IsEmriDurumLabel, IsEmriOncelik, IsEmriOncelikLabel } from '@/types/teknik'
import { showApiError, showSuccess } from '@/lib/toast'

const COLUMNS = [
  { durum: IsEmriDurum.YeniTalep, color: 'border-blue-300', headerBg: 'bg-blue-50', dot: 'bg-blue-400' },
  { durum: IsEmriDurum.Atandi,    color: 'border-yellow-300', headerBg: 'bg-yellow-50', dot: 'bg-yellow-400' },
  { durum: IsEmriDurum.Devam,     color: 'border-orange-300', headerBg: 'bg-orange-50', dot: 'bg-orange-400' },
  { durum: IsEmriDurum.Tamamlandi,color: 'border-green-300', headerBg: 'bg-green-50', dot: 'bg-green-400' },
]

const ONCELIK_COLORS: Record<IsEmriOncelik, string> = {
  [IsEmriOncelik.Dusuk]: 'bg-gray-100 text-gray-600',
  [IsEmriOncelik.Normal]: 'bg-blue-100 text-blue-700',
  [IsEmriOncelik.Yuksek]: 'bg-orange-100 text-orange-700',
  [IsEmriOncelik.Kritik]: 'bg-red-100 text-red-700',
}

const NEXT_DURUM: Partial<Record<IsEmriDurum, IsEmriDurum>> = {
  [IsEmriDurum.YeniTalep]: IsEmriDurum.Atandi,
  [IsEmriDurum.Atandi]: IsEmriDurum.Devam,
  [IsEmriDurum.Devam]: IsEmriDurum.Tamamlandi,
}

export default function PanoPage() {
  const [board, setBoard] = useState<Record<IsEmriDurum, IsEmri[]>>({
    [IsEmriDurum.YeniTalep]: [],
    [IsEmriDurum.Atandi]: [],
    [IsEmriDurum.Devam]: [],
    [IsEmriDurum.Tamamlandi]: [],
    [IsEmriDurum.Iptal]: [],
  })
  const [loading, setLoading] = useState(true)
  const [movingId, setMovingId] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await isEmirleriApi.getPano()
      setBoard(prev => ({ ...prev, ...(res.data as Record<IsEmriDurum, IsEmri[]>) }))
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const handleAdvance = async (item: IsEmri) => {
    const next = NEXT_DURUM[item.durum]
    if (next === undefined) return
    setMovingId(item.id)
    try {
      await isEmirleriApi.updateDurum(item.id, next)
      showSuccess(`Moved to "${IsEmriDurumLabel[next]}"`)
      await load()
    } catch (e) { showApiError(e) }
    finally { setMovingId(null) }
  }

  const total = COLUMNS.reduce((acc, c) => acc + (board[c.durum]?.length ?? 0), 0)

  return (
    <div className="flex flex-col h-full gap-3">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Work Board</h1>
          {!loading && <p className="text-xs text-muted-foreground mt-0.5">{total} active work orders</p>}
        </div>
        <Button size="sm" variant="outline" onClick={load} disabled={loading}>
          <RefreshCw className={`h-3.5 w-3.5 mr-1 ${loading ? 'animate-spin' : ''}`} />Refresh
        </Button>
      </div>

      {loading ? (
        <div className="flex-1 flex items-center justify-center text-muted-foreground text-sm">Loading board...</div>
      ) : (
        <div className="flex gap-3 flex-1 overflow-x-auto pb-2">
          {COLUMNS.map(col => {
            const colItems = board[col.durum] ?? []
            return (
              <div key={col.durum} className={`flex flex-col rounded-lg border-2 ${col.color} min-w-[240px] flex-1 max-w-xs`}>
                <div className={`${col.headerBg} rounded-t-lg px-3 py-2 flex items-center gap-2 border-b ${col.color}`}>
                  <span className={`h-2 w-2 rounded-full ${col.dot}`} />
                  <span className="font-medium text-sm">{IsEmriDurumLabel[col.durum]}</span>
                  <span className="ml-auto text-xs text-muted-foreground font-medium bg-white/70 rounded-full px-1.5 py-0.5">{colItems.length}</span>
                </div>
                <div className="flex-1 overflow-y-auto p-2 space-y-2">
                  {colItems.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-8 text-muted-foreground/50">
                      <ClipboardList className="h-6 w-6 mb-1" />
                      <p className="text-xs">Empty</p>
                    </div>
                  ) : colItems.map(item => (
                    <div key={item.id} className="bg-background rounded-md border p-3 shadow-sm space-y-2">
                      <p className="text-sm font-medium leading-snug">{item.baslik}</p>
                      <div className="flex flex-wrap gap-1">
                        <span className={`text-xs px-1.5 py-0.5 rounded-full font-medium ${ONCELIK_COLORS[item.oncelik]}`}>
                          {IsEmriOncelikLabel[item.oncelik]}
                        </span>
                        {item.departmanAdi && (
                          <span className="text-xs px-1.5 py-0.5 rounded-full bg-muted text-muted-foreground">{item.departmanAdi}</span>
                        )}
                      </div>
                      {item.atananKisiAdi && (
                        <p className="text-xs text-muted-foreground">👤 {item.atananKisiAdi}</p>
                      )}
                      {NEXT_DURUM[item.durum] !== undefined && (
                        <Button size="sm" variant="outline" className="w-full h-6 text-xs mt-1"
                          disabled={movingId === item.id}
                          onClick={() => handleAdvance(item)}>
                          {movingId === item.id ? 'Moving...' : `→ ${IsEmriDurumLabel[NEXT_DURUM[item.durum]!]}`}
                        </Button>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )
          })}
        </div>
      )}
    </div>
  )
}
