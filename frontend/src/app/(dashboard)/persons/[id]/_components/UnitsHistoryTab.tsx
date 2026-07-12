'use client'

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { PersonUnitHistoryDto } from '@/types/personDetail'
import { UserTypeLabel } from '@/types/person'

interface Props {
  units: PersonUnitHistoryDto[]
}

const formatDate = (value?: string) => (value ? new Date(value).toLocaleDateString('tr-TR') : '-')

export function UnitsHistoryTab({ units }: Props) {
  if (units.length === 0) {
    return <p className="text-sm text-muted-foreground py-8 text-center">Bu kişiye bağlı daire kaydı yok.</p>
  }

  return (
    <div className="border rounded-lg overflow-hidden overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Daire</TableHead>
            <TableHead>Kat Maliki / Kiracı</TableHead>
            <TableHead>Giriş Tarihi</TableHead>
            <TableHead>Çıkış Tarihi</TableHead>
            <TableHead>İlgili Kişi</TableHead>
            <TableHead>Açıklama</TableHead>
            <TableHead>Banka Ödeme Kodu</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {units.map((u) => (
            <TableRow key={u.id}>
              <TableCell className="font-medium">
                {u.buildingName ? `${u.buildingName} - ` : ''}{u.doorNumber}
              </TableCell>
              <TableCell>{UserTypeLabel[u.role]}</TableCell>
              <TableCell>{formatDate(u.entryDate)}</TableCell>
              <TableCell>{u.exitDate ? formatDate(u.exitDate) : '-'}</TableCell>
              <TableCell>{u.contactPerson ?? '-'}</TableCell>
              <TableCell>{u.notes ?? '-'}</TableCell>
              <TableCell>{u.bankPaymentCode ?? '-'}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
