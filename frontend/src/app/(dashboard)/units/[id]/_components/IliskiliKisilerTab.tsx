'use client'

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { UnitRelatedPersonDto } from '@/types/unitDetail'
import { UserTypeLabel } from '@/types/person'
import { PetTypeLabel } from '@/types/personDetail'

interface Props {
  relatedPersons: UnitRelatedPersonDto[]
}

const formatDate = (value?: string) => (value ? new Date(value).toLocaleDateString('tr-TR') : '-')

const formatCurrency = (value: number) =>
  value.toLocaleString('tr-TR', { style: 'currency', currency: 'TRY' })

export function IliskiliKisilerTab({ relatedPersons }: Props) {
  if (relatedPersons.length === 0) {
    return <p className="text-sm text-muted-foreground py-8 text-center">Bu daireye bağlı kişi kaydı yok.</p>
  }

  return (
    <div className="border rounded-lg overflow-hidden overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Tipi</TableHead>
            <TableHead>Adı Soyadı</TableHead>
            <TableHead>Pay</TableHead>
            <TableHead>Evcil Hayvan</TableHead>
            <TableHead>Giriş Tarihi</TableHead>
            <TableHead>Çıkış Tarihi</TableHead>
            <TableHead>İlgili Kişi</TableHead>
            <TableHead>Açıklama</TableHead>
            <TableHead>Bakiye</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {relatedPersons.map((p) => (
            <TableRow key={p.historyId}>
              <TableCell>{UserTypeLabel[p.role]}</TableCell>
              <TableCell className="font-medium">{p.fullName}</TableCell>
              <TableCell>{p.sharePercentage != null ? `%${p.sharePercentage}` : '-'}</TableCell>
              <TableCell>
                {p.petType != null && p.petType !== 0
                  ? `${PetTypeLabel[p.petType]}${p.petDetail ? ' - ' + p.petDetail : ''}`
                  : 'Yok'}
              </TableCell>
              <TableCell>{formatDate(p.entryDate)}</TableCell>
              <TableCell>{p.exitDate ? formatDate(p.exitDate) : '-'}</TableCell>
              <TableCell>{p.contactPerson ?? '-'}</TableCell>
              <TableCell>{p.notes ?? '-'}</TableCell>
              <TableCell>{formatCurrency(p.balance)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
