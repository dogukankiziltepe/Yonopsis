'use client'

import { useRef, useState } from 'react'
import { Upload, FileSpreadsheet } from 'lucide-react'
import { cn } from '@/lib/utils/cn'

interface Props {
  onFile: (file: File) => void
  disabled?: boolean
}

export function FileUploadZone({ onFile, disabled }: Props) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [dragging, setDragging] = useState(false)

  const handle = (file: File) => {
    if (!file.name.endsWith('.xlsx')) {
      alert('Yalnızca .xlsx dosyaları kabul edilmektedir.')
      return
    }
    onFile(file)
  }

  return (
    <div
      onClick={() => !disabled && inputRef.current?.click()}
      onDragOver={(e) => { e.preventDefault(); !disabled && setDragging(true) }}
      onDragLeave={() => setDragging(false)}
      onDrop={(e) => {
        e.preventDefault()
        setDragging(false)
        if (disabled) return
        const file = e.dataTransfer.files[0]
        if (file) handle(file)
      }}
      className={cn(
        'flex cursor-pointer flex-col items-center gap-3 rounded-lg border-2 border-dashed p-8 transition-colors',
        dragging ? 'border-primary bg-primary/5' : 'border-border hover:border-primary/50',
        disabled && 'pointer-events-none opacity-50'
      )}
    >
      <div className="flex h-12 w-12 items-center justify-center rounded-full bg-muted">
        <FileSpreadsheet className="h-6 w-6 text-muted-foreground" />
      </div>
      <div className="text-center">
        <p className="text-sm font-medium">Excel dosyasını sürükle veya tıkla</p>
        <p className="text-xs text-muted-foreground mt-1">Yalnızca .xlsx — maks 5 MB</p>
      </div>
      <div className="flex items-center gap-1.5 text-xs text-primary">
        <Upload className="h-3.5 w-3.5" />
        Dosya seç
      </div>
      <input
        ref={inputRef}
        type="file"
        accept=".xlsx"
        className="hidden"
        onChange={(e) => { const f = e.target.files?.[0]; if (f) handle(f); e.target.value = '' }}
      />
    </div>
  )
}
