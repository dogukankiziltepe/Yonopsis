'use client'

import { useRef, useState } from 'react'
import { Upload, X, FileText, Loader2 } from 'lucide-react'
import { Button } from './button'
import { filesApi } from '@/lib/api/files'
import { showError } from '@/lib/toast'

interface UploadedItem {
  id: string
  fileName: string
  size: number
}

interface FileUploadProps {
  onUploaded?: (file: UploadedItem) => void
  onRemove?: (id: string) => void
  value?: UploadedItem[]
  maxFiles?: number
  accept?: string
  disabled?: boolean
}

const ACCEPTED = '.jpg,.jpeg,.png,.gif,.webp,.pdf,.doc,.docx,.xls,.xlsx,.txt'
const MAX_SIZE = 10 * 1024 * 1024

function formatSize(bytes: number) {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1024 / 1024).toFixed(1)} MB`
}

export function FileUpload({
  onUploaded,
  onRemove,
  value = [],
  maxFiles = 5,
  accept = ACCEPTED,
  disabled = false,
}: FileUploadProps) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [dragging, setDragging] = useState(false)
  const [uploading, setUploading] = useState(false)

  const handleFiles = async (files: FileList | null) => {
    if (!files || files.length === 0) return
    const file = files[0]
    if (file.size > MAX_SIZE) {
      showError('File size cannot exceed 10 MB.')
      return
    }
    setUploading(true)
    try {
      const res = await filesApi.upload(file)
      onUploaded?.(res.data)
    } catch (e: unknown) {
      const err = e as { response?: { data?: { message?: string } } }
      showError(err?.response?.data?.message ?? 'File could not be uploaded.')
    } finally {
      setUploading(false)
      if (inputRef.current) inputRef.current.value = ''
    }
  }

  const canUpload = !disabled && !uploading && value.length < maxFiles

  return (
    <div className="space-y-2">
      <div
        className={`border-2 border-dashed rounded-lg p-4 text-center transition-colors ${
          dragging ? 'border-primary bg-primary/5' : 'border-muted-foreground/30'
        } ${canUpload ? 'cursor-pointer hover:border-primary/60' : 'opacity-50 cursor-not-allowed'}`}
        onClick={() => canUpload && inputRef.current?.click()}
        onDragOver={(e) => { e.preventDefault(); if (canUpload) setDragging(true) }}
        onDragLeave={() => setDragging(false)}
        onDrop={(e) => {
          e.preventDefault()
          setDragging(false)
          if (canUpload) handleFiles(e.dataTransfer.files)
        }}
      >
        {uploading ? (
          <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
            <Loader2 className="h-4 w-4 animate-spin" />
            Uploading...
          </div>
        ) : (
          <div className="flex flex-col items-center gap-1">
            <Upload className="h-6 w-6 text-muted-foreground/60" />
            <p className="text-sm text-muted-foreground">
              Drag a file or <span className="text-primary font-medium">select</span>
            </p>
            <p className="text-xs text-muted-foreground/60">Max. 10 MB</p>
          </div>
        )}
      </div>
      <input
        ref={inputRef}
        type="file"
        accept={accept}
        className="hidden"
        onChange={(e) => handleFiles(e.target.files)}
      />
      {value.length > 0 && (
        <ul className="space-y-1">
          {value.map((f) => (
            <li key={f.id} className="flex items-center gap-2 text-sm border rounded-md px-3 py-1.5">
              <FileText className="h-4 w-4 text-muted-foreground shrink-0" />
              <span className="flex-1 truncate">{f.fileName}</span>
              <span className="text-xs text-muted-foreground shrink-0">{formatSize(f.size)}</span>
              {onRemove && (
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-5 w-5 shrink-0"
                  onClick={(e) => { e.stopPropagation(); onRemove(f.id) }}
                  disabled={disabled}
                >
                  <X className="h-3 w-3" />
                </Button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
