'use client'

import { useEffect, useState } from 'react'
import { X } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { cariHesaplariApi } from '@/lib/api/finans'
import { kasaBankaApi, giderTanimlariApi } from '@/lib/api/tanimlar'
import { personsApi } from '@/lib/api/persons'
import { personelApi } from '@/lib/api/personel'
import { VirmanHesapTuru } from '@/types/finans'

interface Secenek { id: string; ad: string }

interface Props {
  hesapTuru: VirmanHesapTuru
  hesapId: string
  hesapAdi: string
  onChange: (id: string, ad: string) => void
  compact?: boolean
}

// Kişi ve Cari çok sayıda olabildiği için aramalı; Banka/Gider/Personel sınırlı olduğu için tek seferde yüklenen select.
const aramali = (t: VirmanHesapTuru) => t === VirmanHesapTuru.Kisi || t === VirmanHesapTuru.Cari

async function ara(tur: VirmanHesapTuru, q: string): Promise<Secenek[]> {
  if (tur === VirmanHesapTuru.Kisi) {
    const r = await personsApi.getAll(1, 8, q)
    return (r.data.items ?? []).map(p => ({ id: p.userId, ad: `${p.firstName} ${p.lastName}` }))
  }
  const r = await cariHesaplariApi.getAll(q)
  return r.data.map(c => ({ id: c.id, ad: c.hesapAdi }))
}

async function listele(tur: VirmanHesapTuru): Promise<Secenek[]> {
  if (tur === VirmanHesapTuru.Banka) {
    const r = await kasaBankaApi.getAll()
    return r.data.filter(k => k.isActive).map(k => ({ id: k.id, ad: k.name }))
  }
  if (tur === VirmanHesapTuru.Gider) {
    const r = await giderTanimlariApi.getAll()
    return r.data.filter(g => g.isActive).map(g => ({ id: g.id, ad: g.name }))
  }
  const r = await personelApi.getAll({ isActive: true, pageSize: 500 })
  return (r.data.items ?? []).map(p => ({ id: p.id, ad: p.name }))
}

export function VirmanHesapPicker({ hesapTuru, hesapId, hesapAdi, onChange, compact }: Props) {
  const inputClass = compact
    ? 'w-full border rounded px-2 py-1 text-xs bg-background focus:outline-none focus:ring-1 focus:ring-ring'
    : 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'

  const [q, setQ] = useState('')
  const [sonuclar, setSonuclar] = useState<Secenek[]>([])
  const [liste, setListe] = useState<Secenek[]>([])

  useEffect(() => {
    if (aramali(hesapTuru)) return
    listele(hesapTuru).then(setListe).catch(() => setListe([]))
  }, [hesapTuru])

  useEffect(() => {
    if (!aramali(hesapTuru) || !q.trim()) { setSonuclar([]); return }
    const t = setTimeout(() => { ara(hesapTuru, q).then(setSonuclar).catch(() => {}) }, 300)
    return () => clearTimeout(t)
  }, [q, hesapTuru])

  if (!aramali(hesapTuru)) {
    return (
      <select className={inputClass} value={hesapId}
        onChange={e => onChange(e.target.value, liste.find(x => x.id === e.target.value)?.ad ?? '')}>
        <option value="">— Seçilmedi —</option>
        {liste.map(x => <option key={x.id} value={x.id}>{x.ad}</option>)}
      </select>
    )
  }

  if (hesapId) {
    return (
      <div className={cn('flex items-center justify-between gap-1 border bg-muted/30', compact ? 'rounded px-2 py-1 text-xs' : 'rounded-md px-3 py-2 text-sm')}>
        <span className="truncate">{hesapAdi || '—'}</span>
        <button type="button" onClick={() => onChange('', '')} className="text-muted-foreground hover:text-foreground"><X className="h-3 w-3" /></button>
      </div>
    )
  }

  return (
    <div className="relative">
      <input className={inputClass} value={q} onChange={e => setQ(e.target.value)}
        placeholder={hesapTuru === VirmanHesapTuru.Kisi ? 'Kişi ara...' : 'Cari hesap ara...'} />
      {sonuclar.length > 0 && (
        <div className="absolute z-20 w-64 bg-background border rounded-md mt-1 shadow-lg max-h-48 overflow-y-auto">
          {sonuclar.map(s => (
            <button key={s.id} type="button" onClick={() => { onChange(s.id, s.ad); setQ(''); setSonuclar([]) }}
              className="w-full text-left px-3 py-1.5 text-xs hover:bg-muted/50">{s.ad}</button>
          ))}
        </div>
      )}
    </div>
  )
}
