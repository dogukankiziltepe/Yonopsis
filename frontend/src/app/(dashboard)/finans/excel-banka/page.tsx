'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { Upload, ExternalLink } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { kasaBankaApi } from '@/lib/api/tanimlar'
import type { KasaBanka } from '@/types/tanimlar'
import { BankaExcelImportWizard } from '@/components/finans/BankaExcelImportWizard'

export default function ExcelBankaPage() {
  const [kasalar, setKasalar] = useState<KasaBanka[]>([])
  const [selectedId, setSelectedId] = useState('')
  const [wizardOpen, setWizardOpen] = useState(false)

  useEffect(() => { kasaBankaApi.getAll().then(r => setKasalar(r.data.filter(k => k.isActive))) }, [])

  const selectedKasa = kasalar.find(k => k.id === selectedId) ?? null

  return (
    <div className="flex flex-col h-full gap-4 max-w-xl">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Excel ile Banka Hareketleri Yükleme</h1>
        <Button size="sm" variant="outline" asChild>
          <Link href="/finans/banka-hareketleri"><ExternalLink className="h-4 w-4 mr-1" />Banka Hareketleri</Link>
        </Button>
      </div>

      <div className="border rounded-lg p-4 space-y-4">
        <p className="text-sm text-muted-foreground">
          Bir kasa/banka seçip Excel şablonunu indirin, doldurup tekrar yükleyin.
        </p>
        <div className="space-y-1.5">
          <label className="text-sm font-medium">Kasa / Banka</label>
          <select
            value={selectedId}
            onChange={e => setSelectedId(e.target.value)}
            className="w-full border rounded-md px-3 py-2 text-sm bg-background focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">— Seçilmedi —</option>
            {kasalar.map(k => <option key={k.id} value={k.id}>{k.name}</option>)}
          </select>
        </div>
        <Button disabled={!selectedId} onClick={() => setWizardOpen(true)}>
          <Upload className="h-4 w-4 mr-1" />Excel ile Yükle
        </Button>
      </div>

      {selectedId && (
        <BankaExcelImportWizard
          kasaBankaId={selectedId}
          kasaBankaAdi={selectedKasa?.name}
          open={wizardOpen}
          onClose={() => setWizardOpen(false)}
          onImported={() => setWizardOpen(false)}
        />
      )}
    </div>
  )
}
