'use client'

import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts'
import { DagilimDilimi } from '@/types/report'
import { useChartTheme } from '@/lib/chartColors'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY', maximumFractionDigits: 0 })

function CustomTooltip({ active, payload, ink }: {
  active?: boolean; ink: { primary: string; secondary: string; surface: string }
  payload?: Array<{ name: string; value: number; payload: DagilimDilimi; color: string }>
}) {
  if (!active || !payload?.length) return null
  const p = payload[0]
  return (
    <div
      className="rounded-md border px-3 py-2 text-xs shadow-md"
      style={{ background: ink.surface, borderColor: ink.secondary, color: ink.primary }}
    >
      <p className="flex items-center gap-1.5 font-medium">
        <span className="h-2 w-2 rounded-full" style={{ background: p.color }} />
        {p.name}
      </p>
      <p>{currency(p.value)} ({p.payload.yuzde.toFixed(1)}%)</p>
    </div>
  )
}

export function DagilimPieChart({ data, loading, emptyText }: { data: DagilimDilimi[]; loading?: boolean; emptyText: string }) {
  const { categorical, ink } = useChartTheme()

  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">{emptyText}</p>

  return (
    <ResponsiveContainer width="100%" height={280}>
      <PieChart>
        <Pie
          data={data}
          dataKey="tutar"
          nameKey="ad"
          innerRadius={60}
          outerRadius={100}
          paddingAngle={2}
          stroke={ink.surface}
          strokeWidth={2}
          label={(props: { percent?: number }) => `${((props.percent ?? 0) * 100).toFixed(0)}%`}
          labelLine={false}
        >
          {data.map((d, i) => (
            <Cell key={d.ad} fill={categorical[i % categorical.length]} />
          ))}
        </Pie>
        <Tooltip content={<CustomTooltip ink={ink} />} />
        <Legend
          layout="vertical"
          align="right"
          verticalAlign="middle"
          wrapperStyle={{ fontSize: 12, color: ink.secondary }}
        />
      </PieChart>
    </ResponsiveContainer>
  )
}
