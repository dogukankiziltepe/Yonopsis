'use client'

import { useMemo, useState } from 'react'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from 'recharts'
import { AidatTahsilatAy } from '@/types/report'
import { useChartTheme } from '@/lib/chartColors'
import { cn } from '@/lib/utils/cn'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY', maximumFractionDigits: 0 })

type SeriesFilter = 'all' | 'tahsilEdilen' | 'tahsilEdilemeyen'

function CustomTooltip({ active, payload, label, ink }: {
  active?: boolean; label?: string; ink: { primary: string; secondary: string; surface: string }
  payload?: Array<{ name: string; value: number; color: string }>
}) {
  if (!active || !payload?.length) return null
  return (
    <div
      className="rounded-md border px-3 py-2 text-xs shadow-md"
      style={{ background: ink.surface, borderColor: ink.secondary, color: ink.primary }}
    >
      <p className="mb-1 font-medium">{label}</p>
      {payload.map((p) => (
        <p key={p.name} className="flex items-center gap-1.5">
          <span className="h-2 w-2 rounded-full" style={{ background: p.color }} />
          {p.name}: {currency(p.value)}
        </p>
      ))}
    </div>
  )
}

export function AidatTahsilatChart({ data, loading }: { data: AidatTahsilatAy[]; loading?: boolean }) {
  const { tahsilEdilen, tahsilEdilemeyen, ink } = useChartTheme()
  const [filter, setFilter] = useState<SeriesFilter>('all')

  const totals = useMemo(
    () => ({
      edilen: data.reduce((s, d) => s + d.tahsilEdilen, 0),
      edilemeyen: data.reduce((s, d) => s + d.tahsilEdilemeyen, 0),
    }),
    [data]
  )

  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Aidat verisi bulunmuyor.</p>

  return (
    <div>
      <ResponsiveContainer width="100%" height={280}>
        <BarChart data={data} barGap={2} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
          <CartesianGrid vertical={false} stroke={ink.grid} />
          <XAxis dataKey="donem" tick={{ fontSize: 11, fill: ink.muted }} axisLine={{ stroke: ink.axis }} tickLine={false} />
          <YAxis
            tick={{ fontSize: 11, fill: ink.muted }} axisLine={false} tickLine={false}
            tickFormatter={(v: number) => currency(v)}
            width={70}
          />
          <Tooltip content={<CustomTooltip ink={ink} />} cursor={{ fill: ink.grid, opacity: 0.4 }} />
          {filter !== 'tahsilEdilemeyen' && (
            <Bar dataKey="tahsilEdilen" name="Tahsil edilen" stackId="a" fill={tahsilEdilen} radius={[0, 0, 0, 0]} />
          )}
          {filter !== 'tahsilEdilen' && (
            <Bar dataKey="tahsilEdilemeyen" name="Tahsil edilemeyen" stackId="a" fill={tahsilEdilemeyen} radius={[3, 3, 0, 0]} />
          )}
        </BarChart>
      </ResponsiveContainer>
      <div className="mt-2 flex justify-center gap-3">
        <button
          type="button"
          onClick={() => setFilter((f) => (f === 'tahsilEdilemeyen' ? 'all' : 'tahsilEdilemeyen'))}
          className={cn(
            'flex items-center gap-1.5 rounded-full border px-3 py-1 text-xs transition-colors',
            filter === 'tahsilEdilemeyen' ? 'border-current' : 'opacity-70 hover:opacity-100'
          )}
          style={{ color: tahsilEdilemeyen, borderColor: filter === 'tahsilEdilemeyen' ? tahsilEdilemeyen : undefined }}
        >
          <span className="h-2 w-2 rounded-full" style={{ background: tahsilEdilemeyen }} />
          Tahsil edilemeyen ({currency(totals.edilemeyen)})
        </button>
        <button
          type="button"
          onClick={() => setFilter((f) => (f === 'tahsilEdilen' ? 'all' : 'tahsilEdilen'))}
          className={cn(
            'flex items-center gap-1.5 rounded-full border px-3 py-1 text-xs transition-colors',
            filter === 'tahsilEdilen' ? 'border-current' : 'opacity-70 hover:opacity-100'
          )}
          style={{ color: tahsilEdilen, borderColor: filter === 'tahsilEdilen' ? tahsilEdilen : undefined }}
        >
          <span className="h-2 w-2 rounded-full" style={{ background: tahsilEdilen }} />
          Tahsil edilen ({currency(totals.edilen)})
        </button>
      </div>
    </div>
  )
}
