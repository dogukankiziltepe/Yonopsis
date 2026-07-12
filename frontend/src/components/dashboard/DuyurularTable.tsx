import { Pin } from 'lucide-react'
import { DuyuruOzet } from '@/types/report'

export function DuyurularTable({ data, loading }: { data: DuyuruOzet[]; loading?: boolean }) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Duyuru bulunmuyor.</p>

  return (
    <div className="max-h-80 space-y-2 overflow-y-auto">
      {data.map((d) => (
        <div key={d.id} className="flex items-start gap-2 border-b pb-2 text-sm last:border-0">
          {d.isPinned && <Pin className="mt-0.5 h-3.5 w-3.5 shrink-0 text-muted-foreground" />}
          <div>
            <p className="font-medium leading-tight">{d.title}</p>
            <p className="text-xs text-muted-foreground">
              {new Date(d.publishDate ?? d.createdAt).toLocaleDateString('tr-TR')}
            </p>
          </div>
        </div>
      ))}
    </div>
  )
}
