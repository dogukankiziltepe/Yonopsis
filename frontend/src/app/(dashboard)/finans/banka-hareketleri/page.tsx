'use client'

import { useEffect, useState, useCallback } from 'react'
import { Landmark, Upload, Clock } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { bankaHareketleriApi } from '@/lib/api/finans'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import { BankaHareketiDurum } from '@/types/finans'
import type { BankaHareketi } from '@/types/finans'
import type { KasaBanka } from '@/types/tanimlar'
import { showApiError } from '@/lib/toast'
import { BankaExcelImportWizard } from '@/components/finans/BankaExcelImportWizard'

const fmt = (n: number) => new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(n)

export default function BankaHareketleriPage() {
  const [kasalar, setKasalar] = useState<KasaBanka[]>([])
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [bekleyenler, setBekleyenler] = useState<BankaHareketi[]>([])
  const [loading, setLoading] = useState(false)
  const [wizardOpen, setWizardOpen] = useState(false)

  useEffect(() => { kasaBankaApi.getAll().then(r => setKasalar(r.data.filter(k => k.isActive))) }, [])

  const loadBekleyenler = useCallback(async (kasaBankaId: string) => {
    setLoading(true)
    try {
      const res = await bankaHareketleriApi.getAll(1, 100, kasaBankaId, BankaHareketiDurum.Bekleyen)
      setBekleyenler(res.data.items ?? [])
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { if (selectedId) loadBekleyenler(selectedId) }, [selectedId, loadBekleyenler])

  const selectedKasa = kasalar.find(k => k.id === selectedId) ?? null

  return (
    <div className="flex flex-col h-full gap-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Banka Hareketleri</h1>
        <Button
          size="sm"
          variant="outline"
          disabled={!selectedId}
          title={!selectedId ? 'Önce bir banka seçin' : undefined}
          onClick={() => setWizardOpen(true)}
        >
          <Upload className="h-4 w-4 mr-1" />Excel ile Yükle
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 flex-1 min-h-0">
        <div className="border rounded-lg overflow-y-auto md:col-span-1">
          {kasalar.length === 0 ? (
            <div className="p-8 text-center text-sm text-muted-foreground">Kasa/Banka tanımı bulunamadı.</div>
          ) : (
            <div className="divide-y">
              {kasalar.map(k => (
                <button
                  key={k.id}
                  onClick={() => setSelectedId(k.id)}
                  className={`w-full text-left px-4 py-3 hover:bg-muted/40 transition-colors ${selectedId === k.id ? 'bg-muted/60' : ''}`}
                >
                  <div className="flex items-center gap-2">
                    <Landmark className="h-4 w-4 text-muted-foreground shrink-0" />
                    <span className="font-medium text-sm">{k.name}</span>
                  </div>
                  {k.bankaAdi && <div className="text-xs text-muted-foreground mt-0.5 ml-6">{k.bankaAdi}{k.subeAdi ? ` — ${k.subeAdi}` : ''}</div>}
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="md:col-span-2 flex flex-col gap-4 min-h-0">
          {!selectedKasa ? (
            <div className="border rounded-lg flex-1 flex items-center justify-center text-sm text-muted-foreground">
              Detayları görmek için soldan bir banka/kasa seçin.
            </div>
          ) : (
            <>
              <div className="border rounded-lg p-4 space-y-1">
                <h2 className="font-semibold">{selectedKasa.name}</h2>
                <div className="text-sm text-muted-foreground grid grid-cols-2 gap-x-4 gap-y-1 mt-2">
                  {selectedKasa.bankaAdi && <div>Banka: {selectedKasa.bankaAdi}</div>}
                  {selectedKasa.subeAdi && <div>Şube: {selectedKasa.subeAdi}</div>}
                  {selectedKasa.hesapNo && <div>Hesap No: {selectedKasa.hesapNo}</div>}
                  {selectedKasa.iban && <div>IBAN: {selectedKasa.iban}</div>}
                </div>
              </div>

              <div className="border rounded-lg flex-1 overflow-hidden flex flex-col min-h-0">
                <div className="px-4 py-2.5 border-b flex items-center gap-2 bg-muted/30">
                  <Clock className="h-4 w-4 text-amber-600" />
                  <span className="font-medium text-sm">Onay Bekleyen Hareketler</span>
                  {bekleyenler.length > 0 && <Badge variant="secondary">{bekleyenler.length}</Badge>}
                </div>
                <div className="overflow-y-auto flex-1">
                  {loading ? (
                    <div className="p-8 text-center text-sm text-muted-foreground">Yükleniyor...</div>
                  ) : bekleyenler.length === 0 ? (
                    <div className="p-8 text-center text-sm text-muted-foreground">Bekleyen hareket yok.</div>
                  ) : (
                    <table className="w-full text-sm">
                      <thead className="bg-muted/30 border-b">
                        <tr>
                          <th className="text-left px-3 py-2 font-medium">Tarih</th>
                          <th className="text-left px-3 py-2 font-medium">Açıklama</th>
                          <th className="text-right px-3 py-2 font-medium">Tutar</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y">
                        {bekleyenler.map(h => (
                          <tr key={h.id}>
                            <td className="px-3 py-2 text-muted-foreground text-xs">{new Date(h.tarih).toLocaleDateString('tr-TR')}</td>
                            <td className="px-3 py-2">{h.aciklama}</td>
                            <td className="px-3 py-2 text-right font-medium">{fmt(h.tutar)}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                </div>
              </div>
            </>
          )}
        </div>
      </div>

      {selectedId && (
        <BankaExcelImportWizard
          kasaBankaId={selectedId}
          kasaBankaAdi={selectedKasa?.name}
          open={wizardOpen}
          onClose={() => setWizardOpen(false)}
          onImported={() => { setWizardOpen(false); loadBekleyenler(selectedId) }}
        />
      )}
    </div>
  )
}
