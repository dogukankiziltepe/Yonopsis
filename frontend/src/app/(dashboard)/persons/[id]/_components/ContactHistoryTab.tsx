'use client'

import { useState } from 'react'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog'
import { PersonContactHistoryDto, PersonEmailLogDto } from '@/types/personDetail'

interface Props {
  contactHistory: PersonContactHistoryDto
}

const formatDateTime = (value?: string) => (value ? new Date(value).toLocaleString('tr-TR') : '-')

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="space-y-2">
      <h3 className="text-sm font-semibold">{title}</h3>
      {children}
    </div>
  )
}

function EmptyRow({ colSpan, text }: { colSpan: number; text: string }) {
  return (
    <TableRow>
      <TableCell colSpan={colSpan} className="text-center text-muted-foreground py-6">
        {text}
      </TableCell>
    </TableRow>
  )
}

export function ContactHistoryTab({ contactHistory }: Props) {
  const [selectedEmail, setSelectedEmail] = useState<PersonEmailLogDto | null>(null)

  return (
    <div className="space-y-6">
      <Section title="E-Posta">
        <div className="border rounded-lg overflow-hidden overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Gönderim</TableHead>
                <TableHead>İletim</TableHead>
                <TableHead>Okunma</TableHead>
                <TableHead>Alıcı</TableHead>
                <TableHead>Konu</TableHead>
                <TableHead>Durum</TableHead>
                <TableHead>Detay</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {contactHistory.emailLogs.length === 0 ? (
                <EmptyRow colSpan={7} text="Kayıt yok." />
              ) : (
                contactHistory.emailLogs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell>{formatDateTime(log.sentAt)}</TableCell>
                    <TableCell>{formatDateTime(log.deliveredAt)}</TableCell>
                    <TableCell>{formatDateTime(log.readAt)}</TableCell>
                    <TableCell>{log.recipientEmail}</TableCell>
                    <TableCell>{log.subject}</TableCell>
                    <TableCell>{log.status}</TableCell>
                    <TableCell>
                      <Button variant="outline" size="sm" onClick={() => setSelectedEmail(log)}>Detay</Button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </Section>

      <Section title="SMS">
        <div className="border rounded-lg overflow-hidden overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Gönderim Tarihi</TableHead>
                <TableHead>Numara</TableHead>
                <TableHead>Mesaj</TableHead>
                <TableHead>Durum</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {contactHistory.smsLogs.length === 0 ? (
                <EmptyRow colSpan={4} text="Kayıt yok." />
              ) : (
                contactHistory.smsLogs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell>{formatDateTime(log.sentAt)}</TableCell>
                    <TableCell>{log.phoneNumber}</TableCell>
                    <TableCell>{log.message}</TableCell>
                    <TableCell>{log.status}</TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </Section>

      <Section title="WhatsApp">
        <div className="border rounded-lg overflow-hidden overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Gönderim Tarihi</TableHead>
                <TableHead>Numara</TableHead>
                <TableHead>Mesaj</TableHead>
                <TableHead>Durum</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {contactHistory.whatsappLogs.length === 0 ? (
                <EmptyRow colSpan={4} text="Kayıt yok." />
              ) : (
                contactHistory.whatsappLogs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell>{formatDateTime(log.sentAt)}</TableCell>
                    <TableCell>{log.phoneNumber}</TableCell>
                    <TableCell>{log.message}</TableCell>
                    <TableCell>{log.status}</TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </Section>

      <Section title="Mobil Bildirim">
        <div className="border rounded-lg overflow-hidden overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Gönderim Tarihi</TableHead>
                <TableHead>Mesaj</TableHead>
                <TableHead>Durum</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {contactHistory.mobilBildirimLogs.length === 0 ? (
                <EmptyRow colSpan={3} text="Kayıt yok." />
              ) : (
                contactHistory.mobilBildirimLogs.map((log) => (
                  <TableRow key={log.id}>
                    <TableCell>{formatDateTime(log.sentAt)}</TableCell>
                    <TableCell>{log.message}</TableCell>
                    <TableCell>{log.status}</TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </Section>

      <Dialog open={!!selectedEmail} onOpenChange={(open) => !open && setSelectedEmail(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{selectedEmail?.subject}</DialogTitle>
            <DialogDescription>{selectedEmail?.recipientEmail}</DialogDescription>
          </DialogHeader>
          <p className="text-sm whitespace-pre-wrap">{selectedEmail?.body ?? 'İçerik bulunamadı.'}</p>
        </DialogContent>
      </Dialog>
    </div>
  )
}
