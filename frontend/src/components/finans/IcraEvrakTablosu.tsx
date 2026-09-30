'use client'

import { FileSpreadsheet } from 'lucide-react'
import type { IcraEvrak } from '@/types/icra'

const sayi = (n: number) => new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n)
const tarih = (s?: string) => (s ? new Date(s).toLocaleDateString('tr-TR') : '—')

/** Satırları .xlsx olarak indirir (xlsx paketi yalnızca tıklanınca yüklenir). */
export async function excelIndir(rows: Record<string, string | number>[], sheet: string, dosyaAdi: string) {
  const XLSX = await import('xlsx')
  const ws = XLSX.utils.json_to_sheet(rows)
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, sheet)
  XLSX.writeFile(wb, dosyaAdi)
}

export function IcraEvrakTablosu({ baslik, evraklar, dosyaAdi }: { baslik: string; evraklar: IcraEvrak[]; dosyaAdi: string }) {
  const toplam = evraklar.reduce(
    (a, e) => ({ tutar: a.tutar + e.evrakTutari, tazminat: a.tazminat + e.tazminat, kalan: a.kalan + e.kalan }),
    { tutar: 0, tazminat: 0, kalan: 0 })

  const indir = () => excelIndir(
    evraklar.map(e => ({
      'Son Ödeme Tarihi': tarih(e.sonOdemeTarihi), 'Evrak No': e.evrakNo, 'Daire': e.doorNumber, 'Kategori': e.kategori ?? '',
      'Evrak Tutarı': e.evrakTutari, 'Tazminat': e.tazminat, 'Ödenen': e.odenen, 'Kalan': e.kalan,
    })),
    'Evraklar', dosyaAdi)

  return (
    <section className="flex flex-col gap-2">
      <h2 className="text-lg font-medium">{baslik}</h2>
      <div className="border rounded-lg overflow-x-auto">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b">
            <tr className="text-xs italic text-muted-foreground">
              <th className="text-left px-3 py-2 font-semibold">Son Ödeme Tarihi</th>
              <th className="text-left px-3 py-2 font-semibold">Evrak No</th>
              <th className="text-left px-3 py-2 font-semibold">Daire</th>
              <th className="text-left px-3 py-2 font-semibold">Kategori</th>
              <th className="text-right px-3 py-2 font-semibold">Evrak Tutarı</th>
              <th className="text-right px-3 py-2 font-semibold">Tazminat</th>
              <th className="text-right px-3 py-2 font-semibold">Kalan</th>
              <th className="w-10 px-2 py-1 text-center">
                <button type="button" onClick={indir} disabled={evraklar.length === 0} title="Excel olarak indir"
                  className="text-muted-foreground hover:text-foreground disabled:opacity-30">
                  <FileSpreadsheet className="h-5 w-5" />
                </button>
              </th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {evraklar.length === 0 ? (
              <tr><td colSpan={8} className="px-3 py-6 text-center text-muted-foreground text-xs">Evrak bulunmuyor.</td></tr>
            ) : evraklar.map(e => (
              <tr key={e.borcMakbuzuId} className="[&>td]:border-r last:[&>td]:border-r-0">
                <td className="px-3 py-2.5">{tarih(e.sonOdemeTarihi)}</td>
                <td className="px-3 py-2.5 font-mono text-xs">{e.evrakNo}</td>
                <td className="px-3 py-2.5">{e.doorNumber}</td>
                <td className="px-3 py-2.5">{e.kategori ?? ''}</td>
                <td className="px-3 py-2.5 text-right">{sayi(e.evrakTutari)}</td>
                <td className="px-3 py-2.5 text-right">{sayi(e.tazminat)}</td>
                <td className="px-3 py-2.5 text-right">{sayi(e.kalan)}</td>
                <td />
              </tr>
            ))}
          </tbody>
          {evraklar.length > 0 && (
            <tfoot className="border-t bg-muted/30 font-semibold">
              <tr>
                <td colSpan={4} className="px-3 py-2 text-right">Toplam</td>
                <td className="px-3 py-2 text-right">{sayi(toplam.tutar)}</td>
                <td className="px-3 py-2 text-right">{sayi(toplam.tazminat)}</td>
                <td className="px-3 py-2 text-right">{sayi(toplam.kalan)}</td>
                <td />
              </tr>
            </tfoot>
          )}
        </table>
      </div>
    </section>
  )
}
