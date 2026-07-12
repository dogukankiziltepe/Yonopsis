'use client'

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { PersonAccessCardDto } from '@/types/personDetail'

interface Props {
  accessCards: PersonAccessCardDto[]
}

const formatDate = (value?: string) => (value ? new Date(value).toLocaleDateString('tr-TR') : '-')

export function AccessCardsTab({ accessCards }: Props) {
  if (accessCards.length === 0) {
    return <p className="text-sm text-muted-foreground py-8 text-center">Bu kişiye bağlı geçiş kartı yok.</p>
  }

  return (
    <div className="border rounded-lg overflow-hidden overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Kart No</TableHead>
            <TableHead>Veriliş Tarihi</TableHead>
            <TableHead>İptal Tarihi</TableHead>
            <TableHead>Aktif</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {accessCards.map((c) => (
            <TableRow key={c.id}>
              <TableCell className="font-medium">{c.cardNumber}</TableCell>
              <TableCell>{formatDate(c.issueDate)}</TableCell>
              <TableCell>{formatDate(c.expiryDate)}</TableCell>
              <TableCell>
                <Badge variant={c.isActive ? 'default' : 'secondary'}>{c.isActive ? 'Aktif' : 'Pasif'}</Badge>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
