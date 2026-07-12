'use client'

import { useEffect, useState, useCallback } from 'react'
import { Pencil, X, Zap, AlertCircle } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { otomatikBildirimlerApi, epostaSablonlariApi, smsSablonlariApi, mobilBildirimSablonlariApi } from '@/lib/api/iletisimYonetim'
import type { OtomatikBildirim, UpsertOtomatikBildirimDto, EpostaSablonu, SmsSablonu, MobilBildirimSablonu } from '@/types/iletisimYonetim'
import { OtomatikBildirimOlay, OtomatikBildirimOlayLabel } from '@/types/iletisimYonetim'
import { showSuccess, showApiError } from '@/lib/toast'

const ALL_OLAYLAR = Object.values(OtomatikBildirimOlay).filter(v => typeof v === 'number') as OtomatikBildirimOlay[]

export default function OtomatikBildirimlerPage() {
  const [items, setItems] = useState<OtomatikBildirim[]>([])
  const [epostaSablonlar, setEpostaSablonlar] = useState<EpostaSablonu[]>([])
  const [smsSablonlar, setSmsSablonlar] = useState<SmsSablonu[]>([])
  const [mobilSablonlar, setMobilSablonlar] = useState<MobilBildirimSablonu[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [panelOpen, setPanelOpen] = useState(false)
  const [selected, setSelected] = useState<OtomatikBildirim | null>(null)

  const [formEpostaAktif, setFormEpostaAktif] = useState(false)
  const [formSmsAktif, setFormSmsAktif] = useState(false)
  const [formMobilAktif, setFormMobilAktif] = useState(false)
  const [formEpostaSablonuId, setFormEpostaSablonuId] = useState('')
  const [formSmsSablonuId, setFormSmsSablonuId] = useState('')
  const [formMobilSablonuId, setFormMobilSablonuId] = useState('')
  const [formIsActive, setFormIsActive] = useState(true)
  const [saving, setSaving] = useState(false)

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const [bildirimRes, epostaRes, smsRes, mobilRes] = await Promise.all([
        otomatikBildirimlerApi.getAll(),
        epostaSablonlariApi.getAll(1, 200),
        smsSablonlariApi.getAll(1, 200),
        mobilBildirimSablonlariApi.getAll(1, 200),
      ])
      setItems(bildirimRes.data)
      setEpostaSablonlar(epostaRes.data.items)
      setSmsSablonlar(smsRes.data.items)
      setMobilSablonlar(mobilRes.data.items)
    } catch { setError('Veriler yüklenirken bir hata oluştu.') }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const getForOlay = (olay: OtomatikBildirimOlay) => items.find(i => i.olayTipi === olay)

  const openEdit = (olay: OtomatikBildirimOlay) => {
    const item = getForOlay(olay)
    setFormEpostaAktif(item?.epostaAktif ?? false)
    setFormSmsAktif(item?.smsAktif ?? false)
    setFormMobilAktif(item?.mobilAktif ?? false)
    setFormEpostaSablonuId(item?.epostaSablonuId ?? '')
    setFormSmsSablonuId(item?.smsSablonuId ?? '')
    setFormMobilSablonuId(item?.mobilSablonuId ?? '')
    setFormIsActive(item?.isActive ?? true)
    setSelected(item ?? { id: '', siteId: '', olayTipi: olay, epostaAktif: false, smsAktif: false, mobilAktif: false, isActive: true })
    setPanelOpen(true)
  }

  const closePanel = () => { setPanelOpen(false); setSelected(null) }

  const handleSave = async () => {
    if (!selected) return
    setSaving(true)
    try {
      const dto: UpsertOtomatikBildirimDto = {
        olayTipi: selected.olayTipi,
        epostaAktif: formEpostaAktif,
        smsAktif: formSmsAktif,
        mobilAktif: formMobilAktif,
        epostaSablonuId: formEpostaSablonuId || undefined,
        smsSablonuId: formSmsSablonuId || undefined,
        mobilSablonuId: formMobilSablonuId || undefined,
        isActive: formIsActive,
      }
      await otomatikBildirimlerApi.upsert(dto)
      showSuccess('Otomatik bildirim ayarları kaydedildi.')
      closePanel(); load()
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  return (
    <div className="flex h-full">
      <div className={`flex-1 min-w-0 transition-all duration-300 ${panelOpen ? 'pr-[460px]' : ''}`}>
        <div className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold tracking-tight">Otomatik Bildirimler</h1>
              <p className="text-sm text-muted-foreground mt-1">Sistem olaylarına bağlı otomatik bildirim kanallarını yapılandırın.</p>
            </div>
          </div>

          {error && <div className="flex items-center gap-2 p-3 mb-4 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

          {loading ? (
            <div className="space-y-2">{Array.from({ length: 8 }).map((_, i) => <div key={i} className="h-16 rounded-lg bg-muted animate-pulse" />)}</div>
          ) : (
            <div className="rounded-lg border overflow-hidden">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Olay</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">E-Posta</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">SMS</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Mobil</th>
                    <th className="px-4 py-3 text-center font-medium text-muted-foreground w-24">Durum</th>
                    <th className="px-4 py-3 text-right font-medium text-muted-foreground w-24">İşlem</th>
                  </tr>
                </thead>
                <tbody className="divide-y">
                  {ALL_OLAYLAR.map(olay => {
                    const item = getForOlay(olay)
                    const isSelected = selected?.olayTipi === olay && panelOpen
                    return (
                      <tr key={olay} className={`hover:bg-muted/30 cursor-pointer transition-colors ${isSelected ? 'bg-muted/50' : ''}`} onClick={() => openEdit(olay)}>
                        <td className="px-4 py-3 font-medium">{OtomatikBildirimOlayLabel[olay]}</td>
                        <td className="px-4 py-3 text-center">
                          {item?.epostaAktif ? <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">Açık</span>
                            : <span className="text-muted-foreground text-xs">—</span>}
                        </td>
                        <td className="px-4 py-3 text-center">
                          {item?.smsAktif ? <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">Açık</span>
                            : <span className="text-muted-foreground text-xs">—</span>}
                        </td>
                        <td className="px-4 py-3 text-center">
                          {item?.mobilAktif ? <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400">Açık</span>
                            : <span className="text-muted-foreground text-xs">—</span>}
                        </td>
                        <td className="px-4 py-3 text-center">
                          {item ? (
                            <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' : 'bg-muted text-muted-foreground'}`}>
                              {item.isActive ? 'Aktif' : 'Pasif'}
                            </span>
                          ) : <span className="text-muted-foreground text-xs">Yapılandırılmadı</span>}
                        </td>
                        <td className="px-4 py-3 text-right" onClick={e => e.stopPropagation()}>
                          <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => openEdit(olay)}><Pencil className="h-3.5 w-3.5" /></Button>
                        </td>
                      </tr>
                    )
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {panelOpen && selected && (
        <div className="fixed right-0 top-0 h-screen w-[460px] bg-background border-l shadow-2xl flex flex-col z-50">
          <div className="flex items-center justify-between px-6 py-4 border-b shrink-0">
            <div>
              <h2 className="font-semibold text-base">Bildirim Ayarı</h2>
              <p className="text-xs text-muted-foreground">{OtomatikBildirimOlayLabel[selected.olayTipi]}</p>
            </div>
            <Button variant="ghost" size="icon" onClick={closePanel}><X className="h-4 w-4" /></Button>
          </div>
          <div className="flex-1 overflow-y-auto px-6 py-5 space-y-5">
            {/* E-posta */}
            <div className="rounded-lg border p-4 space-y-3">
              <div className="flex items-center justify-between">
                <Label className="text-sm font-medium">E-Posta Bildirimi</Label>
                <button type="button" onClick={() => setFormEpostaAktif(!formEpostaAktif)} className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${formEpostaAktif ? 'bg-primary' : 'bg-muted-foreground/30'}`}>
                  <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${formEpostaAktif ? 'translate-x-4' : 'translate-x-0'}`} />
                </button>
              </div>
              {formEpostaAktif && (
                <div>
                  <Label className="text-xs font-medium mb-1.5 block">E-Posta Şablonu</Label>
                  <select value={formEpostaSablonuId} onChange={e => setFormEpostaSablonuId(e.target.value)} className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
                    <option value="">Şablon seçin</option>
                    {epostaSablonlar.map(s => <option key={s.id} value={s.id}>{s.ad}</option>)}
                  </select>
                </div>
              )}
            </div>

            {/* SMS */}
            <div className="rounded-lg border p-4 space-y-3">
              <div className="flex items-center justify-between">
                <Label className="text-sm font-medium">SMS Bildirimi</Label>
                <button type="button" onClick={() => setFormSmsAktif(!formSmsAktif)} className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${formSmsAktif ? 'bg-primary' : 'bg-muted-foreground/30'}`}>
                  <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${formSmsAktif ? 'translate-x-4' : 'translate-x-0'}`} />
                </button>
              </div>
              {formSmsAktif && (
                <div>
                  <Label className="text-xs font-medium mb-1.5 block">SMS Şablonu</Label>
                  <select value={formSmsSablonuId} onChange={e => setFormSmsSablonuId(e.target.value)} className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
                    <option value="">Şablon seçin</option>
                    {smsSablonlar.map(s => <option key={s.id} value={s.id}>{s.ad}</option>)}
                  </select>
                </div>
              )}
            </div>

            {/* Mobil */}
            <div className="rounded-lg border p-4 space-y-3">
              <div className="flex items-center justify-between">
                <Label className="text-sm font-medium">Mobil Bildirimi</Label>
                <button type="button" onClick={() => setFormMobilAktif(!formMobilAktif)} className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${formMobilAktif ? 'bg-primary' : 'bg-muted-foreground/30'}`}>
                  <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${formMobilAktif ? 'translate-x-4' : 'translate-x-0'}`} />
                </button>
              </div>
              {formMobilAktif && (
                <div>
                  <Label className="text-xs font-medium mb-1.5 block">Mobil Bildirim Şablonu</Label>
                  <select value={formMobilSablonuId} onChange={e => setFormMobilSablonuId(e.target.value)} className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
                    <option value="">Şablon seçin</option>
                    {mobilSablonlar.map(s => <option key={s.id} value={s.id}>{s.ad}</option>)}
                  </select>
                </div>
              )}
            </div>

            <div className="flex items-center gap-3 pt-1">
              <button type="button" onClick={() => setFormIsActive(!formIsActive)} className={`relative inline-flex h-5 w-9 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors ${formIsActive ? 'bg-primary' : 'bg-muted-foreground/30'}`}>
                <span className={`pointer-events-none inline-block h-4 w-4 rounded-full bg-white shadow transform transition-transform ${formIsActive ? 'translate-x-4' : 'translate-x-0'}`} />
              </button>
              <Label className="text-sm cursor-pointer" onClick={() => setFormIsActive(!formIsActive)}>{formIsActive ? 'Aktif' : 'Pasif'}</Label>
            </div>
          </div>
          <div className="px-6 py-4 border-t shrink-0">
            <Button onClick={handleSave} disabled={saving} className="w-full">{saving ? 'Kaydediliyor...' : 'Kaydet'}</Button>
          </div>
        </div>
      )}
    </div>
  )
}
