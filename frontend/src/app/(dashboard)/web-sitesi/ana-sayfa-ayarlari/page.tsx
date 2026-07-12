'use client'

import { useEffect, useState, useCallback } from 'react'
import { AlertCircle, Save } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { webSiteAyarlariApi } from '@/lib/api/webSitesi'
import type { UpdateAnaSayfaAyarDto } from '@/types/webSitesi'
import { showSuccess, showApiError } from '@/lib/toast'

export default function AnaSayfaAyarlariPage() {
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  const [formSiteAdi, setFormSiteAdi] = useState('')
  const [formSlogan, setFormSlogan] = useState('')
  const [formKisaAciklama, setFormKisaAciklama] = useState('')
  const [formTelefon, setFormTelefon] = useState('')
  const [formEmail, setFormEmail] = useState('')
  const [formAdres, setFormAdres] = useState('')
  const [formLogoUrl, setFormLogoUrl] = useState('')
  const [formKapakFotoUrl, setFormKapakFotoUrl] = useState('')

  const load = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const res = await webSiteAyarlariApi.getAnaSayfa()
      const d = res.data
      setFormSiteAdi(d.siteAdi ?? '')
      setFormSlogan(d.slogan ?? '')
      setFormKisaAciklama(d.kisaAciklama ?? '')
      setFormTelefon(d.iletisimTelefon ?? '')
      setFormEmail(d.iletisimEmail ?? '')
      setFormAdres(d.adres ?? '')
      setFormLogoUrl(d.logoUrl ?? '')
      setFormKapakFotoUrl(d.kapakFotoUrl ?? '')
    } catch { setError('Ayarlar yuklenirken bir hata olustu.') }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const handleSave = async () => {
    setSaving(true)
    try {
      const dto: UpdateAnaSayfaAyarDto = {
        siteAdi: formSiteAdi.trim() || undefined,
        slogan: formSlogan.trim() || undefined,
        kisaAciklama: formKisaAciklama.trim() || undefined,
        iletisimTelefon: formTelefon.trim() || undefined,
        iletisimEmail: formEmail.trim() || undefined,
        adres: formAdres.trim() || undefined,
        logoUrl: formLogoUrl.trim() || undefined,
        kapakFotoUrl: formKapakFotoUrl.trim() || undefined,
      }
      await webSiteAyarlariApi.updateAnaSayfa(dto)
      showSuccess('Ana sayfa ayarlari kaydedildi.')
    } catch { showApiError() }
    finally { setSaving(false) }
  }

  if (loading) {
    return (
      <div className="p-6 space-y-4 max-w-2xl">
        {Array.from({ length: 6 }).map((_, i) => <div key={i} className="h-12 rounded-lg bg-muted animate-pulse" />)}
      </div>
    )
  }

  return (
    <div className="p-6 max-w-2xl">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Ana Sayfa Ayarlari</h1>
          <p className="text-sm text-muted-foreground mt-1">Web sitesi ana sayfasinda gosterilecek bilgileri yapilandirin.</p>
        </div>
        <Button onClick={handleSave} disabled={saving} className="gap-2">
          <Save className="h-4 w-4" />{saving ? 'Kaydediliyor...' : 'Kaydet'}
        </Button>
      </div>

      {error && <div className="flex items-center gap-2 p-3 mb-6 rounded-lg bg-destructive/10 text-destructive text-sm"><AlertCircle className="h-4 w-4 shrink-0" />{error}</div>}

      <div className="space-y-6">
        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Genel Bilgiler</h2>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Site Adi</Label>
            <Input value={formSiteAdi} onChange={e => setFormSiteAdi(e.target.value)} placeholder="Site adi" maxLength={200} />
          </div>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Slogan</Label>
            <Input value={formSlogan} onChange={e => setFormSlogan(e.target.value)} placeholder="Site slogani" maxLength={300} />
          </div>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Kisa Aciklama</Label>
            <textarea value={formKisaAciklama} onChange={e => setFormKisaAciklama(e.target.value)} placeholder="Site hakkinda kisa aciklama" rows={3} maxLength={500}
              className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring resize-y" />
          </div>
        </div>

        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Iletisim Bilgileri</h2>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label className="text-xs font-medium mb-1.5 block">Telefon</Label>
              <Input value={formTelefon} onChange={e => setFormTelefon(e.target.value)} placeholder="+90 xxx xxx xx xx" maxLength={50} />
            </div>
            <div>
              <Label className="text-xs font-medium mb-1.5 block">E-Posta</Label>
              <Input type="email" value={formEmail} onChange={e => setFormEmail(e.target.value)} placeholder="info@site.com" maxLength={200} />
            </div>
          </div>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Adres</Label>
            <textarea value={formAdres} onChange={e => setFormAdres(e.target.value)} placeholder="Site adresi" rows={2} maxLength={500}
              className="w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring resize-y" />
          </div>
        </div>

        <div className="rounded-lg border p-5 space-y-4">
          <h2 className="font-semibold text-sm">Gorseller</h2>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Logo URL</Label>
            <Input value={formLogoUrl} onChange={e => setFormLogoUrl(e.target.value)} placeholder="https://..." maxLength={500} />
          </div>
          <div>
            <Label className="text-xs font-medium mb-1.5 block">Kapak Fotografi URL</Label>
            <Input value={formKapakFotoUrl} onChange={e => setFormKapakFotoUrl(e.target.value)} placeholder="https://..." maxLength={500} />
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
