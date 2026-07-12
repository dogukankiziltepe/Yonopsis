import { Evrak } from '@/types/report'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })

interface Props {
  data: Evrak[]
  loading?: boolean
  cariLabel: string
  emptyText: string
}

export function EvrakTable({ data, loading, cariLabel, emptyText }: Props) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">{emptyText}</p>

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/50">
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Tarih</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Evrak No</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">{cariLabel}</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Tutar</th>
          </tr>
        </thead>
        <tbody>
          {data.map((e) => (
            <tr key={e.id} className="border-b last:border-0 hover:bg-muted/20">
              <td className="px-3 py-2">{new Date(e.tarih).toLocaleDateString('tr-TR')}</td>
              <td className="px-3 py-2">{e.evrakNo}</td>
              <td className="px-3 py-2">{e.cariAdi}</td>
              <td className="px-3 py-2 text-right tabular-nums">{currency(e.tutar)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
