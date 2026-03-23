import type { Metadata } from 'next'
import { Geist } from 'next/font/google'
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
      </body>
    </html>
  )
}
