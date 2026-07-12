'use client'

import { useEffect, useState, useCallback } from 'react'
import { AlertCircle, Save } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { webSiteAyarlariApi } from '@/lib/api/webSitesi'
import type { UpdateSiteTemasDto } from '@/types/webSitesi'
import { showSuccess, showApiError } from '@/lib/toast'

const FONT_OPTIONS = ['Inter', 'Roboto', 'Open Sans', 'Lato', 'Poppins', 'Nunito', 'Montserrat']

export default function SiteTemasPage() {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  const [formPrimary, setFormPrimary] = useState('')
  const [formSecondary, setFormSecondary] = useState('')
  const [formAccent, setFormAccent] = useState('')
  const [formLogoUrl, setFormLogoUrl] = useState('')
  const [formFaviconUrl, setFormFaviconUrl] = useState('')
  const [formFont, setFormFont] = useState('')

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const res = await webSiteAyarlariApi.getTema()
      const d = res.data
      setFormPrimary(d.primaryColor ?? '')
      setFormSecondary(d.secondaryColor ?? '')
      setFormAccent(d.accentColor ?? '')
      setFormLogoUrl(d.logoUrl ?? '')
      setFormFaviconUrl(d.faviconUrl ?? '')
      setFormFont(d.fontFamily ?? '')
    } catch { setError('Tema ayarlari yuklenirken bir hata olustu.') }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const handleSave = async () => {
    setSaving(true)
    try {
      const dto: UpdateSiteTemasDto = {
        primaryColor: formPrimary.trim() || undefined,
        secondaryColor: formSecondary.trim() || undefined,
        accentColor: formAccent.trim() || undefined,
        logoUrl: formLogoUrl.trim() || undefined,
        faviconUrl: formFaviconUrl.trim() || undefined,
        fontFamily: formFont.trim() || undefined,
      }
      await webSiteAyarlariApi.updateTema(dto)
      showSuccess('Tema ayarlari kaydedildi.')
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  if (loading) {
    return (
      <div className="p-6 space-y-4 max-w-2xl">
        {Array.from({ length: 4 }).map((_, i) => <div key={i} className="h-12 rounded-lg bg-muted animate-pulse" />)}
      </div>
    )
  }

  return (
    <div className="p-6 max-w-2xl">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Site Temasi</h1>
          <p className="text-sm text-muted-foreground mt-1">Web sitesinin gorsel temasini yapilandirin.</p>
        </div>
        <Button onClick={handleSave} disabled={saving} className="gap-2">
          <Save className="h-4 w-4" />{saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>

      {error && <div className="flex items-center gap-2 p-3 mb-6 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

      <div className="space-y-6">
        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Renkler</h2>
          <div className="grid grid-cols-3 gap-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Ana Renk</Label>
              <div className="flex gap-2">
                <input type="color" value={formPrimary || '#000000'} onChange={e => setFormPrimary(e.target.value)} className="h-9 w-12 rounded cursor-pointer border border-input" />
                <Input value={formPrimary} onChange={e => setFormPrimary(e.target.value)} placeholder="#3b82f6" maxLength={20} className="flex-1" />
              </div>
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Ikincil Renk</Label>
              <div className="flex gap-2">
                <input type="color" value={formSecondary || '#000000'} onChange={e => setFormSecondary(e.target.value)} className="h-9 w-12 rounded cursor-pointer border border-input" />
                <Input value={formSecondary} onChange={e => setFormSecondary(e.target.value)} placeholder="#6b7280" maxLength={20} className="flex-1" />
              </div>
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Vurgu Rengi</Label>
              <div className="flex gap-2">
                <input type="color" value={formAccent || '#000000'} onChange={e => setFormAccent(e.target.value)} className="h-9 w-12 rounded cursor-pointer border border-input" />
                <Input value={formAccent} onChange={e => setFormAccent(e.target.value)} placeholder="#f59e0b" maxLength={20} className="flex-1" />
              </div>
            </div>
          </div>
        </div>

        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Yazi Tipi</h2>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Font Ailesi</Label>
            <select value={formFont} onChange={e => setFormFont(e.target.value)}
              className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring">
              <option value="">Varsayilan</option>
              {FONT_OPTIONS.map(f => <option key={f} value={f}>{f}</option>)}
            </select>
          </div>
        </div>

        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Logo &amp; Favicon</h2>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Logo URL</Label>
            <Input value={formLogoUrl} onChange={e => setFormLogoUrl(e.target.value)} placeholder="https://..." maxLength={500} />
          </div>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Favicon URL</Label>
            <Input value={formFaviconUrl} onChange={e => setFormFaviconUrl(e.target.value)} placeholder="https://..." maxLength={500} />
          </div>
        </div>
      </div>

      <div className="mt-6 flex justify-end">
        <Button onClick={handleSave} disabled={saving} className="gap-2">
          <Save className="h-4 w-4" />{saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>
    </div>
  )
}
