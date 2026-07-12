import { IsTakibiOgesi } from '@/types/report'

export function IsTakibiTable({ data, loading }: { data: IsTakibiOgesi[]; loading?: boolean }) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Tamamlanmamış iş bulunmuyor.</p>

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/50">
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Başlık</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Atanan</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Öncelik</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Durum</th>
          </tr>
        </thead>
        <tbody>
          {data.map((it) => (
            <tr key={`${it.kaynak}-${it.id}`} className="border-b last:border-0 hover:bg-muted/20">
              <td className="px-3 py-2">{it.baslik}</td>
              <td className="px-3 py-2 text-muted-foreground">{it.atananKisi ?? '—'}</td>
              <td className="px-3 py-2">{it.oncelik}</td>
              <td className="px-3 py-2">{it.durum}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
