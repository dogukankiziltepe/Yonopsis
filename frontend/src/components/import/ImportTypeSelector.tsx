import { Building2, Home, Users } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { ImportType } from '@/types/import'

interface Option {
  type: ImportType
  label: string
  description: string
  icon: React.ElementType
}

const OPTIONS: Option[] = [
  { type: 'buildings', label: 'Binalar', description: 'Toplu bina ekle', icon: Building2 },
  { type: 'units',     label: 'Daireler', description: 'Toplu daire ekle', icon: Home },
  { type: 'users',     label: 'Kullanıcılar', description: 'Toplu kullanıcı ekle', icon: Users },
]

interface Props {
  value: ImportType
  onChange: (type: ImportType) => void
}

export function ImportTypeSelector({ value, onChange }: Props) {
  return (
    <div className="grid grid-cols-3 gap-3">
      {OPTIONS.map((opt) => {
        const Icon = opt.icon
        const active = value === opt.type
        return (
          <button
            key={opt.type}
            onClick={() => onChange(opt.type)}
            className={cn(
              'flex flex-col items-center gap-2 rounded-lg border p-4 text-sm transition-colors',
              active
                ? 'border-primary bg-primary/5 text-primary'
                : 'border-border text-muted-foreground hover:border-primary/50 hover:text-foreground'
            )}
          >
            <Icon className="h-6 w-6" />
            <div>
              <p className="font-medium">{opt.label}</p>
              <p className="text-xs text-muted-foreground">{opt.description}</p>
            </div>
          </button>
        )
      })}
    </div>
  )
}
