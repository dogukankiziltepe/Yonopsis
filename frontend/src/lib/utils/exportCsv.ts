// Basit CSV (Excel uyumlu) dışa aktarma. Türkçe Excel ";" ayracı bekler;
// UTF-8 BOM eklenir ki Türkçe karakterler doğru görünsün.

type Cell = string | number | null | undefined

export function exportToCsv(filename: string, headers: string[], rows: Cell[][]) {
  const escape = (c: Cell) => {
    const s = c == null ? '' : String(c)
    if (/[";\n]/.test(s)) return `"${s.replace(/"/g, '""')}"`
    return s
  }

  const lines = [headers.map(escape).join(';')]
  for (const row of rows) lines.push(row.map(escape).join(';'))

  const blob = new Blob(['﻿' + lines.join('\r\n')], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = filename.endsWith('.csv') ? filename : `${filename}.csv`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}
