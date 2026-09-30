'use client'

import { useEffect, useMemo, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { virmanFisDetaylariApi } from '@/lib/api/finans'
import { gelirTanimlariApi } from '@/lib/api/tanimlar'
import { unitsApi } from '@/lib/api/units'
import { VirmanHesapPicker } from '@/components/finans/VirmanHesapPicker'
import { VirmanHesapTuru, VirmanHesapTuruLabel, TazminatUygulamaSekli, TazminatUygulamaSekliLabel } from '@/types/finans'
import type { SaveVirmanSatirDto, VirmanSecim } from '@/types/finans'
import type { GelirTanimi } from '@/types/tanimlar'
import type { UnitSummary } from '@/types/unit'
import { showSuccess, showApiError } from '@/lib/toast'

const selectClass = 'w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring'
const gun = (s?: string) => (s ? s.split('T')[0] : '')
const trTarih = (s?: string) => (s ? new Date(s).toLocaleDateString('tr-TR') : '—')
const num = (s: string) => parseFloat(s.replace(',', '.')) || 0

// Borç Dönemi: 2 yıl geri – 1 yıl ileri, yeniden eskiye YYYY-AA listesi
function donemListesi(): string[] {
  const now = new Date()
  const list: string[] = []
  for (let y = now.getFullYear() + 1; y >= now.getFullYear() - 2; y--)
    for (let m = 12; m >= 1; m--) list.push(`${y}-${String(m).padStart(2, '0')}`)
  return list
}

export default function VirmanFisDetayPage() {
  const router = useRouter()
  const { id } = useParams<{ id: string }>()
  const isNew = id === 'yeni'

  const [fisler, setFisler] = useState<VirmanSecim[]>([])
  const [units, setUnits] = useState<UnitSummary[]>([])
  const [kategoriler, setKategoriler] = useState<GelirTanimi[]>([])
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [deleteConfirm, setDeleteConfirm] = useState(false)

  const [virmanId, setVirmanId] = useState('')
  const [ilgiliFis, setIlgiliFis] = useState<{ evrakNo: string; tarih: string; belgeTarihi?: string; belgeNo?: string; aciklama?: string } | null>(null)
  const [borcDonemi, setBorcDonemi] = useState('')
  const [hesapTuru, setHesapTuru] = useState<VirmanHesapTuru>(VirmanHesapTuru.Kisi)
  const [hesapId, setHesapId] = useState('')
  const [hesapAdi, setHesapAdi] = useState('')
  const [unitId, setUnitId] = useState('')
  const [gelirTanimiId, setGelirTanimiId] = useState('')
  const [tazminatUygula, setTazminatUygula] = useState(false)
  const [tazminatBaslama, setTazminatBaslama] = useState('')
  const [sonOdeme, setSonOdeme] = useState('')
  const [uygulamaSekli, setUygulamaSekli] = useState<TazminatUygulamaSekli>(TazminatUygulamaSekli.Aylik)
  const [yuzde, setYuzde] = useState('')
  const [tazminatHesapTarihi, setTazminatHesapTarihi] = useState('')
  const [aciklama, setAciklama] = useState('')
  const [borc, setBorc] = useState('')
  const [alacak, setAlacak] = useState('')
  const [icraTakibinde, setIcraTakibinde] = useState(false)
  const [icraTarihi, setIcraTarihi] = useState('')
  const [icraDosyaNo, setIcraDosyaNo] = useState('')

  const donemler = useMemo(() => {
    const l = donemListesi()
    return borcDonemi && !l.includes(borcDonemi) ? [borcDonemi, ...l] : l
  }, [borcDonemi])

  useEffect(() => {
    virmanFisDetaylariApi.getFisler().then(r => setFisler(r.data)).catch(showApiError)
    unitsApi.getAll().then(r => setUnits(r.data)).catch(showApiError)
    gelirTanimlariApi.getAll().then(r => setKategoriler(r.data.filter(g => g.isActive))).catch(showApiError)
  }, [])

  useEffect(() => {
    if (isNew) return
    virmanFisDetaylariApi.getById(id)
      .then(r => {
        const d = r.data
        setVirmanId(d.virmanId)
        setIlgiliFis({ evrakNo: d.virmanEvrakNo, tarih: d.virmanTarih, belgeTarihi: d.virmanBelgeTarihi, belgeNo: d.virmanBelgeNo, aciklama: d.virmanAciklama })
        setBorcDonemi(d.borcDonemi ?? '')
        setHesapTuru(d.hesapTuru); setHesapId(d.hesapId); setHesapAdi(d.hesapAdi ?? '')
        setUnitId(d.unitId ?? ''); setGelirTanimiId(d.gelirTanimiId ?? '')
        setTazminatUygula(d.gecikmeTazminatiUygula)
        setTazminatBaslama(gun(d.tazminatBaslamaTarihi)); setSonOdeme(gun(d.sonOdemeTarihi))
        if (d.tazminatUygulamaSekli !== undefined && d.tazminatUygulamaSekli !== null) setUygulamaSekli(d.tazminatUygulamaSekli)
        setYuzde(d.aylikTazminatYuzdesi != null ? String(d.aylikTazminatYuzdesi) : '')
        setTazminatHesapTarihi(gun(d.tazminatHesapTarihi))
        setAciklama(d.aciklama ?? '')
        setBorc(d.borcTutari ? String(d.borcTutari) : ''); setAlacak(d.alacakTutari ? String(d.alacakTutari) : '')
        setIcraTakibinde(d.icraTakibinde); setIcraTarihi(gun(d.icrayaVerilmeTarihi)); setIcraDosyaNo(d.icraDosyaNo ?? '')
      })
      .catch(showApiError)
      .finally(() => setLoading(false))
  }, [id, isNew])

  const onFisChange = (v: string) => {
    setVirmanId(v)
    const f = fisler.find(x => x.id === v)
    setIlgiliFis(f ? { evrakNo: f.evrakNo, tarih: f.tarih, belgeNo: f.belgeNo } : null)
  }

  const handleSave = async () => {
    if (!virmanId) { showApiError('Virman fişi seçilmelidir.'); return }
    if (!hesapId) { showApiError('Hesap seçilmelidir.'); return }
    const b = num(borc), a = num(alacak)
    if (b === 0 && a === 0) { showApiError('Borç veya alacak tutarı girilmelidir.'); return }
    if (b > 0 && a > 0) { showApiError('Borç ve alacak aynı anda girilemez.'); return }
    if (tazminatUygula && yuzde && (num(yuzde) < 0 || num(yuzde) > 100)) { showApiError('Tazminat yüzdesi 0-100 arasında olmalıdır.'); return }

    setSaving(true)
    try {
      const dto: SaveVirmanSatirDto = {
        virmanId,
        borcDonemi: borcDonemi || undefined,
        hesapTuru, hesapId,
        unitId: unitId || undefined,
        gelirTanimiId: gelirTanimiId || undefined,
        gecikmeTazminatiUygula: tazminatUygula,
        tazminatBaslamaTarihi: tazminatBaslama || undefined,
        sonOdemeTarihi: sonOdeme || undefined,
        tazminatUygulamaSekli: tazminatUygula ? uygulamaSekli : undefined,
        aylikTazminatYuzdesi: tazminatUygula && yuzde ? num(yuzde) : undefined,
        tazminatHesapTarihi: tazminatHesapTarihi || undefined,
        aciklama: aciklama || undefined,
        borcTutari: b, alacakTutari: a,
        icraTakibinde,
        icrayaVerilmeTarihi: icraTakibinde ? icraTarihi || undefined : undefined,
        icraDosyaNo: icraTakibinde ? icraDosyaNo || undefined : undefined,
      }
      if (isNew) await virmanFisDetaylariApi.create(dto)
      else await virmanFisDetaylariApi.update(id, dto)
      showSuccess(isNew ? 'Virman fişi detayı oluşturuldu.' : 'Virman fişi detayı güncellendi.')
      router.push('/finans/virman-fis-detaylari')
    } catch (e) { showApiError(e) }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    try {
      await virmanFisDetaylariApi.delete(id)
      showSuccess('Virman fişi detayı silindi.')
      router.push('/finans/virman-fis-detaylari')
    } catch (e) { showApiError(e) }
  }

  if (loading) return <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>

  return (
    <div className="flex flex-col gap-4 max-w-4xl">
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => router.push('/finans/virman-fis-detaylari')}><ArrowLeft className="h-4 w-4" /></Button>
        <h1 className="text-xl font-semibold">{isNew ? 'Yeni Virman Fişi Detayı' : 'Virman Fişi Detayı'}</h1>
      </div>

      {/* Fiş */}
      <section className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="space-y-1.5">
          <Label htmlFor="virman">Virman Fişi <span className="text-destructive">*</span></Label>
          <select id="virman" className={selectClass} value={virmanId} onChange={e => onFisChange(e.target.value)}>
            <option value="">— Seçiniz —</option>
            {fisler.map(f => <option key={f.id} value={f.id}>{f.evrakNo} · {trTarih(f.tarih)}{f.belgeNo ? ` · ${f.belgeNo}` : ''}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label>İlgili Virman Fişi</Label>
          <div className="border rounded-md px-3 py-2 text-sm bg-muted/30 min-h-[38px]">
            {ilgiliFis
              ? <span><span className="font-mono text-xs">{ilgiliFis.evrakNo}</span> · {trTarih(ilgiliFis.tarih)}{ilgiliFis.belgeNo ? ` · Belge ${ilgiliFis.belgeNo}` : ''}{ilgiliFis.aciklama ? ` · ${ilgiliFis.aciklama}` : ''}</span>
              : <span className="text-muted-foreground">—</span>}
          </div>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="donem">Borç Dönemi</Label>
          <select id="donem" className={selectClass} value={borcDonemi} onChange={e => setBorcDonemi(e.target.value)}>
            <option value="">— Seçilmedi —</option>
            {donemler.map(d => <option key={d} value={d}>{d}</option>)}
          </select>
        </div>
      </section>

      {/* Hesap */}
      <section className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div className="space-y-1.5">
          <Label htmlFor="hesapTuru">Hesap Tipi</Label>
          <select id="hesapTuru" className={selectClass} value={hesapTuru}
            onChange={e => { setHesapTuru(Number(e.target.value) as VirmanHesapTuru); setHesapId(''); setHesapAdi('') }}>
            {Object.entries(VirmanHesapTuruLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label>Hesap <span className="text-destructive">*</span></Label>
          <VirmanHesapPicker hesapTuru={hesapTuru} hesapId={hesapId} hesapAdi={hesapAdi}
            onChange={(i, ad) => { setHesapId(i); setHesapAdi(ad) }} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="daire">Daire</Label>
          <select id="daire" className={selectClass} value={unitId} onChange={e => setUnitId(e.target.value)}>
            <option value="">— Seçilmedi —</option>
            {units.map(u => <option key={u.id} value={u.id}>{u.buildingName ? `${u.buildingName} / ` : ''}{u.doorNumber}</option>)}
          </select>
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="kategori">Kategori</Label>
          <select id="kategori" className={selectClass} value={gelirTanimiId} onChange={e => setGelirTanimiId(e.target.value)}>
            <option value="">— Seçilmedi —</option>
            {kategoriler.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
          </select>
        </div>
      </section>

      {/* Gecikme tazminatı */}
      <section className="border rounded-lg p-4 space-y-4">
        <label className="flex items-center gap-2 text-sm font-medium cursor-pointer w-fit">
          <input type="checkbox" className="h-4 w-4" checked={tazminatUygula} onChange={e => setTazminatUygula(e.target.checked)} />
          Gecikme Tazminatı Uygula
        </label>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div className="space-y-1.5">
            <Label htmlFor="tazBas">Tazminat Başlama Tarihi</Label>
            <Input id="tazBas" type="date" value={tazminatBaslama} onChange={e => setTazminatBaslama(e.target.value)} disabled={!tazminatUygula} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="sonOdeme">Son Ödeme Tarihi</Label>
            <Input id="sonOdeme" type="date" value={sonOdeme} onChange={e => setSonOdeme(e.target.value)} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="sekil">Uygulama Şekli</Label>
            <select id="sekil" className={selectClass} value={uygulamaSekli} disabled={!tazminatUygula}
              onChange={e => setUygulamaSekli(Number(e.target.value) as TazminatUygulamaSekli)}>
              {Object.entries(TazminatUygulamaSekliLabel).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
            </select>
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="yuzde">Aylık Gecikme Tazminatı Yüzdesi</Label>
            <Input id="yuzde" type="number" min={0} max={100} step="0.01" value={yuzde} onChange={e => setYuzde(e.target.value)} disabled={!tazminatUygula} placeholder="%" />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="tazHesap">Tazminat Hesap Tarihi</Label>
            <Input id="tazHesap" type="date" value={tazminatHesapTarihi} onChange={e => setTazminatHesapTarihi(e.target.value)} disabled={!tazminatUygula} />
          </div>
        </div>
      </section>

      {/* Açıklama + bakiye */}
      <section className="border rounded-lg p-4 grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="space-y-1.5 sm:col-span-3">
          <Label htmlFor="aciklama">Detay Açıklama</Label>
          <textarea id="aciklama" value={aciklama} onChange={e => setAciklama(e.target.value)} maxLength={1000} rows={2}
            className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring" />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="borc">Bakiye — Borç</Label>
          <Input id="borc" type="number" min={0} step="0.01" className="text-right" value={borc} placeholder="0,00"
            onChange={e => { setBorc(e.target.value); if (e.target.value) setAlacak('') }} />
        </div>
        <div className="space-y-1.5">
          <Label htmlFor="alacak">Bakiye — Alacak</Label>
          <Input id="alacak" type="number" min={0} step="0.01" className="text-right" value={alacak} placeholder="0,00"
            onChange={e => { setAlacak(e.target.value); if (e.target.value) setBorc('') }} />
        </div>
      </section>

      {/* İcra */}
      <section className="border rounded-lg p-4 space-y-4">
        <label className="flex items-center gap-2 text-sm font-medium cursor-pointer w-fit">
          <input type="checkbox" className="h-4 w-4" checked={icraTakibinde} onChange={e => setIcraTakibinde(e.target.checked)} />
          İcra Takibinde
        </label>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div className="space-y-1.5">
            <Label htmlFor="icraTarih">İcraya Verilme Tarihi</Label>
            <Input id="icraTarih" type="date" value={icraTarihi} onChange={e => setIcraTarihi(e.target.value)} disabled={!icraTakibinde} />
          </div>
          <div className="space-y-1.5 sm:col-span-2">
            <Label htmlFor="icraNo">İcra Dosya Numarası</Label>
            <Input id="icraNo" value={icraDosyaNo} onChange={e => setIcraDosyaNo(e.target.value)} maxLength={100} disabled={!icraTakibinde} />
          </div>
        </div>
      </section>

      <div className="flex items-center justify-between">
        <div>
          {!isNew && (deleteConfirm
            ? <div className="flex gap-2">
                <Button variant="destructive" onClick={handleDelete}>Silmeyi Onayla</Button>
                <Button variant="ghost" onClick={() => setDeleteConfirm(false)}>Vazgeç</Button>
              </div>
            : <Button variant="ghost" className="text-destructive" onClick={() => setDeleteConfirm(true)}>Sil</Button>)}
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => router.push('/finans/virman-fis-detaylari')}>İptal</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
        </div>
      </div>
    </div>
  )
}
