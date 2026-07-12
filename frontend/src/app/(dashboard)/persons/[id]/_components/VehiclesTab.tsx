'use client'

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { PersonVehicleDto } from '@/types/personDetail'

interface Props {
  vehicles: PersonVehicleDto[]
}

export function VehiclesTab({ vehicles }: Props) {
  if (vehicles.length === 0) {
    return <p className="text-sm text-muted-foreground py-8 text-center">Bu kişiye bağlı araç kaydı yok.</p>
  }

  return (
    <div className="border rounded-lg overflow-hidden overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Plaka</TableHead>
            <TableHead>Marka</TableHead>
            <TableHead>Model</TableHead>
            <TableHead>Renk</TableHead>
            <TableHead>HGS No</TableHead>
            <TableHead>Araç Durumu</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {vehicles.map((v) => (
            <TableRow key={v.id}>
              <TableCell className="font-medium">{v.plate}</TableCell>
              <TableCell>{v.brand ?? '-'}</TableCell>
              <TableCell>{v.model ?? '-'}</TableCell>
              <TableCell>{v.color ?? '-'}</TableCell>
              <TableCell>{v.hgsNo ?? '-'}</TableCell>
              <TableCell>
                <Badge variant={v.isActive ? 'default' : 'secondary'}>{v.isActive ? 'Aktif' : 'Pasif'}</Badge>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
