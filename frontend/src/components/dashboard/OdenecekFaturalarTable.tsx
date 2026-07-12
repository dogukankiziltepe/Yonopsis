import { OdenecekFatura } from '@/types/report'

const currency = (v: number) =>
  v.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })

export function OdenecekFaturalarTable({ data, loading }: { data: OdenecekFatura[]; loading?: boolean }) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Ödenecek fatura bulunmuyor.</p>

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/50">
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Evrak No</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Firma/Kişi</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Son Ödeme</th>
            <th className="px-3 py-2 text-right font-medium text-muted-foreground">Tutar</th>
          </tr>
        </thead>
        <tbody>
          {data.map((f) => {
            const overdue = f.sonOdemeTarihi && new Date(f.sonOdemeTarihi) < today
            return (
              <tr key={f.id} className="border-b last:border-0 hover:bg-muted/20">
                <td className="px-3 py-2">{f.evrakNo}</td>
                <td className="px-3 py-2">{f.cariAdi}</td>
                <td className={overdue ? 'px-3 py-2 text-red-600' : 'px-3 py-2'}>
                  {f.sonOdemeTarihi ? new Date(f.sonOdemeTarihi).toLocaleDateString('tr-TR') : '—'}
                </td>
                <td className="px-3 py-2 text-right tabular-nums">{currency(f.tutar)}</td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
