'use client'

import { useEffect, useMemo, useState } from 'react'
import { Eye, FileDown, FileSpreadsheet } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { icraRaporuApi } from '@/lib/api/icra'
import { buildingsApi } from '@/lib/api/buildings'
import { unitsApi } from '@/lib/api/units'
import { IcraDurumuLabel } from '@/types/icra'
import type { AvukatSecim, IcraRaporFiltre, IcraRaporSatiri } from '@/types/icra'
import { VirmanHesapPicker } from '@/components/finans/VirmanHesapPicker'
import { VirmanHesapTuru } from '@/types/finans'
import type { Building } from '@/types/building'
import type { UnitSummary } from '@/types/unit'
import { excelIndir } from '@/components/finans/IcraEvrakTablosu'
import { showApiError } from '@/lib/toast'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n)
const tarih = (s: string) => new Date(s).toLocaleDateString('tr-TR')
const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'

const BASLIKLAR = ['İcra Tarihi', 'Dosya No', 'Borçlu', 'Blok', 'Daire', 'Avukat', 'Durumu', 'Evrak', 'Dosya Tutarı', 'Tahsilat', 'Bakiye']
const satirDegerleri = (r: IcraRaporSatiri) => [
  tarih(r.icraTarihi), r.dosyaNo, r.borcluAdi, r.blokAdi ?? '', r.doorNumber, r.avukatAdi ?? '',
  IcraDurumuLabel[r.durum], String(r.evrakSayisi), fmt(r.dosyaTutari), fmt(r.tahsilat), fmt(r.bakiye),
]

const esc = (s: string) => s.replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!))

/** Raporu yalın bir pencerede açıp yazdırma diyaloğunu tetikler ("PDF olarak kaydet" ile indirilebilir). */
function pdfYazdir(satirlar: IcraRaporSatiri[], filtreOzeti: string, toplam: { dosya: number; tahsilat: number; bakiye: number }) {
  const w = window.open('', '_blank', 'width=1100,height=800')
  if (!w) { showApiError('Açılır pencere engellendi. Tarayıcı ayarlarından izin verin.'); return }
  const sagaHizali = new Set([7, 8, 9, 10])
  const th = BASLIKLAR.map((b, i) => `<th${sagaHizali.has(i) ? ' class="r"' : ''}>${b}</th>`).join('')
  const tr = satirlar.map(r => `<tr>${satirDegerleri(r).map((v, i) => `<td${sagaHizali.has(i) ? ' class="r"' : ''}>${esc(v)}</td>`).join('')}</tr>`).join('')
  w.document.write(`<!doctype html><html lang="tr"><head><meta charset="utf-8"><title>İcra Raporu</title>
<style>
  body{font-family:system-ui,-apple-system,Segoe UI,Roboto,sans-serif;font-size:11px;color:#111;margin:24px}
  h1{font-size:16px;margin:0 0 4px} p{margin:0 0 12px;color:#555}
  table{width:100%;border-collapse:collapse} th,td{border:1px solid #ccc;padding:4px 6px;text-align:left}
  th{background:#f3f3f3} .r{text-align:right} tfoot td{font-weight:600;background:#fafafa}
  @page{size:A4 landscape;margin:12mm}
</style></head><body>
<h1>İcra Raporu</h1><p>${esc(filtreOzeti)} · ${satirlar.length} dosya · ${new Date().toLocaleString('tr-TR')}</p>
<table><thead><tr>${th}</tr></thead><tbody>${tr}</tbody>
<tfoot><tr><td colspan="8" class="r">Toplam</td><td class="r">${fmt(toplam.dosya)}</td><td class="r">${fmt(toplam.tahsilat)}</td><td class="r">${fmt(toplam.bakiye)}</td></tr></tfoot>
</table></body></html>`)
  w.document.close()
  w.focus()
  w.print()
}

export default function IcraRaporuPage() {
  const [bloklar, setBloklar] = useState<Building[]>([])
  const [daireler, setDaireler] = useState<UnitSummary[]>([])
  const [avukatlar, setAvukatlar] = useState<AvukatSecim[]>([])

  const [ilkTarih, setIlkTarih] = useState('')
  const [sonTarih, setSonTarih] = useState('')
  const [buildingId, setBuildingId] = useState('')
  const [unitId, setUnitId] = useState('')
  const [kisiId, setKisiId] = useState('')
  const [kisiAdi, setKisiAdi] = useState('')
  const [avukatId, setAvukatId] = useState('')
  const [dosyaNo, setDosyaNo] = useState('')

  const [satirlar, setSatirlar] = useState<IcraRaporSatiri[] | null>(null)
  const [filtreOzeti, setFiltreOzeti] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    buildingsApi.getAll(1, 500).then(r => setBloklar(r.data.items ?? [])).catch(showApiError)
    unitsApi.getAll().then(r => setDaireler(r.data)).catch(showApiError)
    icraRaporuApi.getAvukatlar().then(r => setAvukatlar(r.data)).catch(showApiError)
  }, [])

  const blokDaireleri = useMemo(
    () => (buildingId ? daireler.filter(d => d.buildingId === buildingId) : daireler),
    [daireler, buildingId])

  const goster = async () => {
    const filtre: IcraRaporFiltre = {
      ilkTarih: ilkTarih || undefined, sonTarih: sonTarih || undefined,
      buildingId: buildingId || undefined, unitId: unitId || undefined,
      borcluUserId: kisiId || undefined, avukatId: avukatId || undefined, dosyaNo: dosyaNo.trim() || undefined,
    }
    setLoading(true)
    try {
      const r = await icraRaporuApi.get(filtre)
      setSatirlar(r.data)
      const ozet = [
        ilkTarih || sonTarih ? `Tarih: ${ilkTarih ? tarih(ilkTarih) : '…'} – ${sonTarih ? tarih(sonTarih) : '…'}` : null,
        buildingId ? `Blok: ${bloklar.find(b => b.id === buildingId)?.name}` : null,
        unitId ? `Daire: ${daireler.find(d => d.id === unitId)?.doorNumber}` : null,
        kisiId ? `Kişi: ${kisiAdi}` : null,
        avukatId ? `Avukat: ${avukatlar.find(a => a.id === avukatId)?.adSoyad}` : null,
        dosyaNo.trim() ? `Dosya No: ${dosyaNo.trim()}` : null,
      ].filter(Boolean).join(' · ')
      setFiltreOzeti(ozet || 'Tüm icra dosyaları')
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }

  const toplam = (satirlar ?? []).reduce(
    (a, r) => ({ dosya: a.dosya + r.dosyaTutari, tahsilat: a.tahsilat + r.tahsilat, bakiye: a.bakiye + r.bakiye }),
    { dosya: 0, tahsilat: 0, bakiye: 0 })

  const excel = () => satirlar && excelIndir(satirlar.map(r => ({
    'İcra Tarihi': tarih(r.icraTarihi), 'Dosya No': r.dosyaNo, 'Borçlu': r.borcluAdi, 'Blok': r.blokAdi ?? '', 'Daire': r.doorNumber,
    'Avukat': r.avukatAdi ?? '', 'Durumu': IcraDurumuLabel[r.durum], 'Evrak Sayısı': r.evrakSayisi,
    'Dosya Tutarı': r.dosyaTutari, 'Tahsilat': r.tahsilat, 'Bakiye': r.bakiye,
  })), 'İcra Raporu', 'icra-raporu.xlsx')

  return (
    <div className="flex flex-col gap-4">
      <h1 className="text-xl font-semibold">İcra Raporu</h1>

      <div className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 items-end">
        <div className="space-y-1.5">
          <Label htmlFor="ilk">İlk Tarih</Label>
          <Input id="ilk" type="date" value={ilkTarih} onChange={e => setIlkTarih(e.target.value)} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="son">Son Tarih</Label>
          <Input id="son" type="date" value={sonTarih} onChange={e => setSonTarih(e.target.value)} />
        </div>
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
          <Label>Kişi</Label>
          <VirmanHesapPicker hesapTuru={VirmanHesapTuru.Kisi} hesapId={kisiId} hesapAdi={kisiAdi} onChange={(i, ad) => { setKisiId(i); setKisiAdi(ad) }} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="avukat">Avukat</Label>
          <select id="avukat" className={selectClass} value={avukatId} onChange={e => setAvukatId(e.target.value)}>
            <option value="">Tümü</option>
            {avukatlar.map(a => <option key={a.id} value={a.id}>{a.adSoyad}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="dosyaNo">Dosya No</Label>
          <Input id="dosyaNo" value={dosyaNo} onChange={e => setDosyaNo(e.target.value)} maxLength={100} />
        </div>
        <div className="flex justify-end">
          <Button onClick={goster} disabled={loading} className="w-full sm:w-auto"><Eye className="h-4 w-4 mr-1" />{loading ? 'Hazırlanıyor...' : 'Göster'}</Button>
        </div>
      </div>

      {satirlar && (
        <>
          <div className="flex items-center justify-between gap-2 flex-wrap">
            <span className="text-sm text-muted-foreground">{filtreOzeti} · {satirlar.length} dosya</span>
            <div className="flex gap-2">
              <Button size="sm" variant="outline" onClick={excel} disabled={satirlar.length === 0}><FileSpreadsheet className="h-4 w-4 mr-1" />Excel</Button>
              <Button size="sm" variant="outline" onClick={() => pdfYazdir(satirlar, filtreOzeti, toplam)} disabled={satirlar.length === 0}><FileDown className="h-4 w-4 mr-1" />PDF</Button>
            </div>
          </div>

          <div className="border rounded-lg overflow-x-auto">
            {satirlar.length === 0 ? (
              <div className="p-10 text-center text-sm text-muted-foreground">Filtrelere uyan icra dosyası bulunamadı.</div>
            ) : (
              <table className="w-full text-sm">
                <thead className="bg-muted/50 border-b">
                  <tr>
                    {BASLIKLAR.map((b, i) => <th key={b} className={`px-3 py-2 font-medium ${i >= 7 ? 'text-right' : 'text-left'}`}>{b}</th>)}
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {satirlar.map(r => (
                    <tr key={r.id}>
                      {satirDegerleri(r).map((v, i) => <td key={i} className={`px-3 py-2 ${i >= 7 ? 'text-right' : ''} ${i === 1 ? 'font-mono text-xs' : ''}`}>{v}</td>)}
                    </tr>
                  ))}
                </tbody>
                <tfoot className="border-t bg-muted/30 font-semibold">
                  <tr>
                    <td colSpan={8} className="px-3 py-2 text-right">Toplam</td>
                    <td className="px-3 py-2 text-right">{fmt(toplam.dosya)}</td>
                    <td className="px-3 py-2 text-right">{fmt(toplam.tahsilat)}</td>
                    <td className="px-3 py-2 text-right">{fmt(toplam.bakiye)}</td>
                  </tr>
                </tfoot>
              </table>
            )}
          </div>
        </>
      )}
    </div>
  )
}
