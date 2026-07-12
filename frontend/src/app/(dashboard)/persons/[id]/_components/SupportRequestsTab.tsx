'use client'

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { PersonSupportRequestDto } from '@/types/personDetail'
import { SupportRequestStatusLabel } from '@/types/supportRequest'

interface Props {
  supportRequests: PersonSupportRequestDto[]
}

const formatDate = (value?: string) => (value ? new Date(value).toLocaleDateString('tr-TR') : '-')

export function SupportRequestsTab({ supportRequests }: Props) {
  if (supportRequests.length === 0) {
    return <p className="text-sm text-muted-foreground py-8 text-center">Bu kişiye ait talep kaydı yok.</p>
  }

  return (
    <div className="border rounded-lg overflow-hidden overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Tarih</TableHead>
            <TableHead>İlgili Daire</TableHead>
            <TableHead>Konu</TableHead>
            <TableHead>Durum</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {supportRequests.map((r) => (
            <TableRow key={r.id}>
              <TableCell>{formatDate(r.createdAt)}</TableCell>
              <TableCell>{r.unitDoorNumber ?? '-'}</TableCell>
              <TableCell>{r.subject}</TableCell>
              <TableCell>
                <Badge variant="outline">{SupportRequestStatusLabel[r.status]}</Badge>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
