'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { unitsApi } from '@/lib/api/units'
import { showSuccess, showError } from '@/lib/toast'
import { UpdateUnitDto } from '@/types/unit'
import { UnitFullDetailDto } from '@/types/unitDetail'

interface Props {
  unitId: string
  detail: UnitFullDetailDto
  onSaved: () => void
}

export function DetayBilgileriTab({ unitId, detail, onSaved }: Props) {
  const { core, detail: detailInfo } = detail

  const [code2, setCode2] = useState(detailInfo.code2 ?? '')
  const [code3, setCode3] = useState(detailInfo.code3 ?? '')
  const [deliveryDate, setDeliveryDate] = useState(detailInfo.deliveryDate ? detailInfo.deliveryDate.slice(0, 10) : '')
  const [description, setDescription] = useState(detailInfo.description ?? '')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setCode2(detailInfo.code2 ?? '')
    setCode3(detailInfo.code3 ?? '')
    setDeliveryDate(detailInfo.deliveryDate ? detailInfo.deliveryDate.slice(0, 10) : '')
    setDescription(detailInfo.description ?? '')
  }, [detailInfo])

  const handleSave = async () => {
    setSaving(true)
    try {
      const dto: UpdateUnitDto = {
        buildingId: core.buildingId,
        unitTypeId: core.unitTypeId,
        doorNumber: core.doorNumber,
        code: core.code,
        floor: core.floor,
        grossArea: core.grossArea,
        netArea: core.netArea,
        landShare: core.landShare,
        status: core.status,
        monthlyFee: core.monthlyFee,
        parkingCount: core.parkingCount,
        direction: core.direction,
        internet: core.internet,
        hasDask: core.hasDask,
        description: description.trim() || undefined,
        code2: code2.trim() || undefined,
        code3: code3.trim() || undefined,
        deliveryDate: deliveryDate || undefined,
      }
      await unitsApi.update(unitId, dto)
      showSuccess('Detay bilgileri kaydedildi.')
      onSaved()
    } catch {
      showError('Kaydedilirken bir hata oluştu.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4 max-w-2xl">
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kod 2</Label>
          <Input value={code2} onChange={(e) => setCode2(e.target.value)} placeholder="Kod 2" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kod 3</Label>
          <Input value={code3} onChange={(e) => setCode3(e.target.value)} placeholder="Kod 3" />
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Teslim Tarihi</Label>
        <Input type="date" value={deliveryDate} onChange={(e) => setDeliveryDate(e.target.value)} />
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">Açıklama</Label>
        <Input value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Açıklama" />
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
