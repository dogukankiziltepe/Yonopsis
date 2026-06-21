import { Save } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface Props {
  validCount: number
  loading: boolean
  onConfirm: () => void
}

export function ConfirmImportButton({ validCount, loading, onConfirm }: Props) {
  return (
    <Button
      onClick={onConfirm}
      disabled={validCount === 0 || loading}
      className="gap-2"
    >
      <Save className="h-4 w-4" />
      {loading ? 'Kaydediliyor…' : `${validCount} Geçerli Satırı Kaydet`}
    </Button>
  )
}
