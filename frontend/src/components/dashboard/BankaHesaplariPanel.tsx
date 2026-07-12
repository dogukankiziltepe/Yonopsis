import { Landmark } from 'lucide-react'
import { BankaHesabi } from '@/types/report'

export function BankaHesaplariPanel({ data, loading }: { data: BankaHesabi[]; loading?: boolean }) {
  if (loading) return <p className="text-sm text-muted-foreground">Yükleniyor...</p>
  if (!data.length) return <p className="text-sm text-muted-foreground">Kayıtlı banka hesabı bulunmuyor.</p>

  return (
    <div className="grid gap-3 sm:grid-cols-2">
      {data.map((b) => (
        <div key={b.id} className="flex items-start gap-2 rounded-lg border p-3 text-sm">
          <Landmark className="mt-0.5 h-4 w-4 shrink-0 text-muted-foreground" />
          <div>
            <p className="font-medium">{b.ad}</p>
            {b.bankaAdi && <p className="text-xs text-muted-foreground">{b.bankaAdi}{b.subeAdi ? ` — ${b.subeAdi}` : ''}</p>}
            {b.hesapNo && <p className="text-xs text-muted-foreground">Hesap No: {b.hesapNo}</p>}
            {b.iban && <p className="text-xs text-muted-foreground">IBAN: {b.iban}</p>}
          </div>
        </div>
      ))}
    </div>
  )
}
