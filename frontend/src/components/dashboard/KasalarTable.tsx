import { KasaBakiye, KasaBankaTipi } from '@/types/report'
import { cn } from '@/lib/utils/cn'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })

export function KasalarTable({ data, loading }: { data: KasaBakiye[]; loading?: boolean }) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Kayıtlı kasa/banka hesabı bulunmuyor.</p>

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/50">
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Kasa/Banka</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Devir</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Giren</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Çıkan</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Kalan</th>
          </tr>
        </thead>
        <tbody>
          {data.map((k) => (
            <tr key={k.kasaBankaId} className="border-b last:border-0 hover:bg-muted/20">
              <td className="px-3 py-2">
                {k.ad}
                <span className="ml-2 text-xs text-muted-foreground">
                  {k.tip === KasaBankaTipi.Kasa ? 'Kasa' : 'Banka'}
                </span>
              </td>
              <td className="px-3 py-2 text-right tabular-nums">{currency(k.devir)}</td>
              <td className="px-3 py-2 text-right tabular-nums text-emerald-600">{currency(k.giren)}</td>
              <td className="px-3 py-2 text-right tabular-nums text-red-600">{currency(k.cikan)}</td>
              <td className={cn('px-3 py-2 text-right font-medium tabular-nums', k.kalan < 0 && 'text-red-600')}>
                {currency(k.kalan)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
