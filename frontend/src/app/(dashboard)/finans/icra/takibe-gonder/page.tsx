'use client'

import { useEffect, useMemo, useState } from 'react'
import { useRouter } from 'next/navigation'
import { Search, Send } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { takibeGonderApi } from '@/lib/api/icra'
import { buildingsApi } from '@/lib/api/buildings'
import { unitsApi } from '@/lib/api/units'
import { gelirTanimlariApi } from '@/lib/api/tanimlar'
import type { TakipAdayFiltre, TakipAdayi } from '@/types/icra'
import type { Building } from '@/types/building'
import type { UnitSummary } from '@/types/unit'
import type { GelirTanimi } from '@/types/tanimlar'
import { showSuccess, showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)
const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'
const anahtar = (a: TakipAdayi) => `${a.borcluUserId}|${a.unitId}`

export default function TakibeGonderPage() {
  const router = useRouter()
  const [bloklar, setBloklar] = useState<Building[]>([])
  const [daireler, setDaireler] = useState<UnitSummary[]>([])
  const [kategoriler, setKategoriler] = useState<GelirTanimi[]>([])

  const [buildingId, setBuildingId] = useState('')
  const [unitId, setUnitId] = useState('')
  const [gelirTanimiId, setGelirTanimiId] = useState('')
  const [gunSayisi, setGunSayisi] = useState('30')
  const [enAzBakiye, setEnAzBakiye] = useState('0')

  // Takip Başlat, listelemede kullanılan filtreyle yapılır (form sonradan değişse bile)
  const [listelenenFiltre, setListelenenFiltre] = useState<TakipAdayFiltre | null>(null)
  const [adaylar, setAdaylar] = useState<TakipAdayi[]>([])
  const [secili, setSecili] = useState<Set<string>>(new Set())
  const [loading, setLoading] = useState(false)
  const [baslatiliyor, setBaslatiliyor] = useState(false)

  useEffect(() => {
    buildingsApi.getAll(1, 500).then(r => setBloklar(r.data.items ?? [])).catch(showApiError)
    unitsApi.getAll().then(r => setDaireler(r.data)).catch(showApiError)
    gelirTanimlariApi.getAll().then(r => setKategoriler(r.data.filter(g => g.isActive))).catch(showApiError)
  }, [])

  const blokDaireleri = useMemo(
    () => (buildingId ? daireler.filter(d => d.buildingId === buildingId) : daireler),
    [daireler, buildingId])

  const listele = async () => {
    const filtre: TakipAdayFiltre = {
      buildingId: buildingId || undefined,
      unitId: unitId || undefined,
      gelirTanimiId: gelirTanimiId || undefined,
      gunSayisi: parseInt(gunSayisi) || 0,
      enAzBakiye: parseFloat(enAzBakiye.replace(',', '.')) || 0,
    }
    setLoading(true)
    try {
      const r = await takibeGonderApi.getAdaylar(filtre)
      setAdaylar(r.data)
      setListelenenFiltre(filtre)
      setSecili(new Set())
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }

  const toggle = (k: string) => setSecili(prev => {
    const s = new Set(prev)
    if (s.has(k)) s.delete(k); else s.add(k)
    return s
  })
  const hepsiSecili = adaylar.length > 0 && secili.size === adaylar.length
  const toggleHepsi = () => setSecili(hepsiSecili ? new Set() : new Set(adaylar.map(anahtar)))

  const seciliToplam = adaylar.filter(a => secili.has(anahtar(a))).reduce((t, a) => t + a.kalan, 0)

  const takipBaslat = async () => {
    if (!listelenenFiltre || secili.size === 0) return
    setBaslatiliyor(true)
    try {
      const secimler = adaylar.filter(a => secili.has(anahtar(a))).map(a => ({ borcluUserId: a.borcluUserId, unitId: a.unitId }))
      const r = await takibeGonderApi.baslat({ filtre: listelenenFiltre, secimler })
      showSuccess(`${r.data} kayıt için takip başlatıldı.`)
      router.push('/finans/icra/takip-listesi')
    } catch (e) { showApiError(e) }
    finally { setBaslatiliyor(false) }
  }

  return (
    <div className="flex flex-col gap-4">
      <h1 className="text-xl font-semibold">Takibe Gönder</h1>

      <div className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 lg:grid-cols-5 gap-4 items-end">
        <div className="space-y-1.5">
          <Label htmlFor="blok">Blok</Label>
          <select id="blok" className={selectClass} value={buildingId} onChange={e => { setBuildingId(e.target.value); setUnitId('') }}>
            <option value="">Tümü</option>
            {bloklar.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="daire">Daire</Label>
          <select id="daire" className={selectClass} value={unitId} onChange={e => setUnitId(e.target.value)}>
            <option value="">Tümü</option>
            {blokDaireleri.map(u => <option key={u.id} value={u.id}>{!buildingId && u.buildingName ? `${u.buildingName} / ` : ''}{u.doorNumber}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="kategori">Kategori</Label>
          <select id="kategori" className={selectClass} value={gelirTanimiId} onChange={e => setGelirTanimiId(e.target.value)}>
            <option value="">Tümü</option>
            {kategoriler.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="gun">Gün Sayısı</Label>
          <Input id="gun" type="number" min={0} value={gunSayisi} onChange={e => setGunSayisi(e.target.value)} title="Son ödeme tarihinden bu yana en az geçen gün" />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="bakiye">En Az Bakiye</Label>
          <Input id="bakiye" type="number" min={0} step="0.01" value={enAzBakiye} onChange={e => setEnAzBakiye(e.target.value)} />
        </div>
        <div className="sm:col-span-3 lg:col-span-5 flex justify-end">
          <Button onClick={listele} disabled={loading}><Search className="h-4 w-4 mr-1" />{loading ? 'Listeleniyor...' : 'Listele'}</Button>
        </div>
      </div>

      {listelenenFiltre && (
        <>
          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">
              {adaylar.length} kayıt{secili.size > 0 && ` · ${secili.size} seçili · ${fmt(seciliToplam)}`}
            </span>
            <Button onClick={takipBaslat} disabled={secili.size === 0 || baslatiliyor}>
              <Send className="h-4 w-4 mr-1" />{baslatiliyor ? 'Başlatılıyor...' : 'Takip Başlat'}
            </Button>
          </div>

          <div className="border rounded-lg overflow-x-auto">
            {adaylar.length === 0 ? (
              <div className="p-10 text-center text-sm text-muted-foreground">Kriterlere uyan borçlu bulunamadı.</div>
            ) : (
              <table className="w-full text-sm">
                <thead className="bg-muted/50 border-b">
                  <tr>
                    <th className="w-10 px-3 py-2"><input type="checkbox" className="h-4 w-4" checked={hepsiSecili} onChange={toggleHepsi} /></th>
                    <th className="text-left px-3 py-2 font-medium">Kişi</th>
                    <th className="text-left px-3 py-2 font-medium">Daire</th>
                    <th className="text-right px-3 py-2 font-medium">Borç</th>
                    <th className="text-right px-3 py-2 font-medium">Tazminat</th>
                    <th className="text-right px-3 py-2 font-medium">Ödenen</th>
                    <th className="text-right px-3 py-2 font-medium">Kalan</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {adaylar.map(a => {
                    const k = anahtar(a)
                    return (
                      <tr key={k} className="hover:bg-muted/30 cursor-pointer" onClick={() => toggle(k)}>
                        <td className="px-3 py-2.5" onClick={e => e.stopPropagation()}>
                          <input type="checkbox" className="h-4 w-4" checked={secili.has(k)} onChange={() => toggle(k)} />
                        </td>
                        <td className="px-3 py-2.5">{a.borcluAdi}<span className="text-xs text-muted-foreground ml-2">{a.evrakSayisi} evrak</span></td>
                        <td className="px-3 py-2.5 text-xs">{a.blokAdi ? `${a.blokAdi} / ` : ''}{a.doorNumber}</td>
                        <td className="px-3 py-2.5 text-right">{fmt(a.borc)}</td>
                        <td className="px-3 py-2.5 text-right">{fmt(a.tazminat)}</td>
                        <td className="px-3 py-2.5 text-right">{fmt(a.odenen)}</td>
                        <td className="px-3 py-2.5 text-right font-medium">{fmt(a.kalan)}</td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            )}
          </div>
        </>
      )}
    </div>
  )
}
