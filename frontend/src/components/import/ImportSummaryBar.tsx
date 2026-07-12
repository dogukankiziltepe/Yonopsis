import { CheckCircle2, XCircle, FileText } from 'lucide-react'

interface Props {
  totalRows: number
  validRows: number
  invalidRows: number
}

export function ImportSummaryBar({ totalRows, validRows, invalidRows }: Props) {
  return (
    <div className="flex items-center gap-4 rounded-lg border bg-muted/30 px-4 py-3 text-sm">
      <div className="flex items-center gap-1.5 text-muted-foreground">
        <FileText className="h-4 w-4" />
        <span>{totalRows} rows</span>
      </div>
      <div className="flex items-center gap-1.5 text-emerald-600">
        <CheckCircle2 className="h-4 w-4" />
        <span>{validRows} valid</span>
      </div>
      {invalidRows > 0 && (
        <div className="flex items-center gap-1.5 text-destructive">
          <XCircle className="h-4 w-4" />
          <span>{invalidRows} invalid</span>
        </div>
      )}
    </div>
  )
}
