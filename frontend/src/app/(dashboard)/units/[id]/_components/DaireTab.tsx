'use client'

import { useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { unitsApi } from '@/lib/api/units'
import { buildingsApi } from '@/lib/api/buildings'
import { unitTypesApi } from '@/lib/api/unitTypes'
import { showSuccess, showError } from '@/lib/toast'
import { UnitStatus, UnitDirection, UnitDirectionLabel, UpdateUnitDto } from '@/types/unit'
import { UnitFullDetailDto } from '@/types/unitDetail'
import { Building } from '@/types/building'
import { UnitTypeDto } from '@/types/unitType'

const UnitCoreStatusLabel: Record<number, string> = {
  0: 'Boş',
  1: 'Dolu',
}

interface Props {
  unitId: string
  detail: UnitFullDetailDto
  onSaved: () => void
}

export function DaireTab({ unitId, detail, onSaved }: Props) {
  const { core } = detail

  const [code, setCode] = useState(core.code ?? '')
  const [buildingId, setBuildingId] = useState(core.buildingId)
  const [unitTypeId, setUnitTypeId] = useState(core.unitTypeId)
  const [grossArea, setGrossArea] = useState<number | undefined>(core.grossArea)
  const [netArea, setNetArea] = useState<number | undefined>(core.netArea)
  const [landShare, setLandShare] = useState<number | undefined>(core.landShare)
  const [floor, setFloor] = useState(core.floor ?? '')
  const [status, setStatus] = useState<UnitStatus>(core.status)
  const [monthlyFee, setMonthlyFee] = useState<number | undefined>(core.monthlyFee)
  const [parkingCount, setParkingCount] = useState<number | undefined>(core.parkingCount)
  const [direction, setDirection] = useState<UnitDirection | undefined>(core.direction)
  const [internet, setInternet] = useState(core.internet ?? '')
  const [hasDask, setHasDask] = useState(core.hasDask)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    setCode(core.code ?? '')
    setBuildingId(core.buildingId)
    setUnitTypeId(core.unitTypeId)
    setGrossArea(core.grossArea)
    setNetArea(core.netArea)
    setLandShare(core.landShare)
    setFloor(core.floor ?? '')
    setStatus(core.status)
    setMonthlyFee(core.monthlyFee)
    setParkingCount(core.parkingCount)
    setDirection(core.direction)
    setInternet(core.internet ?? '')
    setHasDask(core.hasDask)
  }, [core])

  const [buildings, setBuildings] = useState<Building[]>([])
  const [unitTypes, setUnitTypes] = useState<UnitTypeDto[]>([])

  useEffect(() => {
    buildingsApi.getAll(1, 100).then((res) => setBuildings(res.data.items)).catch(() => {})
    unitTypesApi.getAll().then((res) => setUnitTypes(res.data)).catch(() => {})
  }, [])

  const handleSave = async () => {
    setSaving(true)
    try {
      const dto: UpdateUnitDto = {
        buildingId,
        unitTypeId,
        doorNumber: core.doorNumber,
        code: code.trim() || undefined,
        floor: floor.trim() || undefined,
        grossArea,
        netArea,
        landShare,
        status,
        monthlyFee,
        parkingCount,
        direction,
        internet: internet.trim() || undefined,
        hasDask,
        description: detail.detail.description,
        code2: detail.detail.code2,
        code3: detail.detail.code3,
        deliveryDate: detail.detail.deliveryDate,
      }
      await unitsApi.update(unitId, dto)
      showSuccess('Daire bilgileri kaydedildi.')
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
          <Label className="text-xs font-medium">Kod</Label>
          <Input value={code} onChange={(e) => setCode(e.target.value)} placeholder="Kod" />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Bağlı Olduğu Blok</Label>
          <Select value={buildingId} onValueChange={setBuildingId}>
            <SelectTrigger><SelectValue placeholder="Blok seçin" /></SelectTrigger>
            <SelectContent>
              {buildings.map((b) => (
                <SelectItem key={b.id} value={b.id}>{b.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Tipi</Label>
          <Select value={unitTypeId} onValueChange={setUnitTypeId}>
            <SelectTrigger><SelectValue placeholder="Tip seçin" /></SelectTrigger>
            <SelectContent>
              {unitTypes.map((t) => (
                <SelectItem key={t.id} value={t.id}>{t.name}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Kat</Label>
          <Input value={floor} onChange={(e) => setFloor(e.target.value)} placeholder="örn. 3" />
        </div>
      </div>

      <div className="grid grid-cols-3 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Brüt Alan (m²)</Label>
          <Input
            type="number"
            value={grossArea ?? ''}
            onChange={(e) => setGrossArea(e.target.value ? Number(e.target.value) : undefined)}
            min="0"
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Net Alan (m²)</Label>
          <Input
            type="number"
            value={netArea ?? ''}
            onChange={(e) => setNetArea(e.target.value ? Number(e.target.value) : undefined)}
            min="0"
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Arsa Payı</Label>
          <Input
            type="number"
            value={landShare ?? ''}
            onChange={(e) => setLandShare(e.target.value ? Number(e.target.value) : undefined)}
            min="0"
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Durumu</Label>
          <Select value={String(status)} onValueChange={(v) => setStatus(Number(v) as UnitStatus)}>
            <SelectTrigger><SelectValue /></SelectTrigger>
            <SelectContent>
              {Object.entries(UnitCoreStatusLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Aidat (₺)</Label>
          <Input
            type="number"
            value={monthlyFee ?? ''}
            onChange={(e) => setMonthlyFee(e.target.value ? Number(e.target.value) : undefined)}
            min="0"
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-1">
          <Label className="text-xs font-medium">Park Yeri Sayısı</Label>
          <Input
            type="number"
            value={parkingCount ?? ''}
            onChange={(e) => setParkingCount(e.target.value ? Number(e.target.value) : undefined)}
            min="0"
          />
        </div>
        <div className="space-y-1">
          <Label className="text-xs font-medium">Yönü</Label>
          <Select
            value={direction !== undefined ? String(direction) : undefined}
            onValueChange={(v) => setDirection(Number(v) as UnitDirection)}
          >
            <SelectTrigger><SelectValue placeholder="Seçiniz" /></SelectTrigger>
            <SelectContent>
              {Object.entries(UnitDirectionLabel).map(([value, label]) => (
                <SelectItem key={value} value={value}>{label}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-1">
        <Label className="text-xs font-medium">İnternet</Label>
        <Input value={internet} onChange={(e) => setInternet(e.target.value)} placeholder="İnternet" />
      </div>

      <div className="flex items-center gap-2">
        <Checkbox id="hasDask" checked={hasDask} onCheckedChange={(v) => setHasDask(v === true)} />
        <Label htmlFor="hasDask" className="text-sm font-normal cursor-pointer">Dask</Label>
      </div>

      <Button size="sm" onClick={handleSave} disabled={saving}>
        {saving ? 'Kaydediliyor...' : 'Kaydet'}
      </Button>
    </div>
  )
}
