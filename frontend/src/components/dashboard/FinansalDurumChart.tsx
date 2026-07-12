'use client'

import { useMemo } from 'react'
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from 'recharts'
import { FinansalDurumNoktasi } from '@/types/report'
import { useChartTheme } from '@/lib/chartColors'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY', maximumFractionDigits: 0 })

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
      <p className="mb-1 font-medium">{label ? new Date(label).toLocaleDateString('tr-TR') : ''}</p>
      {payload.map((p) => (
        <p key={p.name} className="flex items-center gap-1.5">
          <span className="h-2 w-2 rounded-full" style={{ background: p.color }} />
          {p.name}: {currency(p.value)}
        </p>
      ))}
    </div>
  )
}

export function FinansalDurumChart({ data, loading }: { data: FinansalDurumNoktasi[]; loading?: boolean }) {
  const { categorical, ink } = useChartTheme()

  const { rows, seriesNames } = useMemo(() => {
    const byDate = new Map<string, Record<string, number | string>>()
    const names = new Set<string>()
    for (const p of data) {
      const key = p.tarih.slice(0, 10)
      names.add(p.kasaBankaAdi)
      const row = byDate.get(key) ?? { tarih: key }
      row[p.kasaBankaAdi] = p.bakiye
      byDate.set(key, row)
    }
    return {
      rows: Array.from(byDate.values()).sort((a, b) => String(a.tarih).localeCompare(String(b.tarih))),
      seriesNames: Array.from(names),
    }
  }, [data])

  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!rows.length) return <p className="text-sm text-muted-foreground">Finansal hareket verisi bulunmuyor.</p>

  return (
    <ResponsiveContainer width="100%" height={300}>
      <AreaChart data={rows} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
        <CartesianGrid vertical={false} stroke={ink.grid} />
        <XAxis
          dataKey="tarih" tick={{ fontSize: 11, fill: ink.muted }} axisLine={{ stroke: ink.axis }} tickLine={false}
          tickFormatter={(v: string) => new Date(v).toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit' })}
          minTickGap={24}
        />
        <YAxis
          tick={{ fontSize: 11, fill: ink.muted }} axisLine={false} tickLine={false}
          tickFormatter={(v: number) => currency(v)}
          width={70}
        />
        <Tooltip content={<CustomTooltip ink={ink} />} />
        <Legend wrapperStyle={{ fontSize: 12, color: ink.secondary }} />
        {seriesNames.map((name, i) => {
          const color = categorical[i % categorical.length]
          return (
            <Area
              key={name}
              type="monotone"
              dataKey={name}
              name={name}
              stroke={color}
              fill={color}
              fillOpacity={0.18}
              strokeWidth={2}
            />
          )
        })}
      </AreaChart>
    </ResponsiveContainer>
  )
}
