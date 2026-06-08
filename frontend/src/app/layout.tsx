import type { Metadata } from 'next'
import { Geist } from 'next/font/google'
import { Toaster } from 'sonner'
import './globals.css'

const geist = Geist({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Site Yönetimi',
  description: 'Apartman ve site yönetim sistemi',
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="tr">
      <body className={`${geist.className} antialiased`}>
        {children}
        <Toaster richColors position="top-right" duration={4000} />
      </body>
    </html>
  )
}
