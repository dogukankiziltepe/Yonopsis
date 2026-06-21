import { CheckCircle2, XCircle } from 'lucide-react'
import { cn } from '@/lib/utils/cn'
import { ImportPreviewRow, ImportType } from '@/types/import'

const COLUMNS: Record<ImportType, string[]> = {
  buildings: ['BuildingName', 'TotalFloors', 'Address', 'Description'],
  units:     ['BuildingName', 'FloorNumber', 'UnitNumber', 'UnitType', 'SquareMeters', 'Description'],
  users:     ['FirstName', 'LastName', 'Email', 'Phone', 'BuildingName', 'UnitNumber', 'Role'],
}

const DATA_KEYS: Record<ImportType, string[]> = {
  buildings: ['buildingName', 'totalFloors', 'address', 'description'],
  units:     ['buildingName', 'floorNumber', 'unitNumber', 'unitType', 'squareMeters', 'description'],
  users:     ['firstName', 'lastName', 'email', 'phone', 'buildingName', 'unitNumber', 'role'],
}

interface Props {
  importType: ImportType
  rows: ImportPreviewRow[]
}

export function ImportPreviewTable({ importType, rows }: Props) {
  const columns = COLUMNS[importType]
  const dataKeys = DATA_KEYS[importType]

  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/50">
            <th className="px-3 py-2 text-left font-medium text-muted-foreground w-10">#</th>
            <th className="px-3 py-2 text-left font-medium text-muted-foreground w-10">Durum</th>
            {columns.map((col) => (
              <th key={col} className="px-3 py-2 text-left font-medium text-muted-foreground whitespace-nowrap">
                {col}
              </th>
            ))}
            <th className="px-3 py-2 text-left font-medium text-muted-foreground">Hatalar</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr
              key={row.rowIndex}
              className={cn(
                'border-b last:border-0 transition-colors',
                row.isValid ? 'hover:bg-muted/20' : 'bg-destructive/5 hover:bg-destructive/10'
              )}
            >
              <td className="px-3 py-2 text-muted-foreground">{row.rowIndex}</td>
              <td className="px-3 py-2">
                {row.isValid
                  ? <CheckCircle2 className="h-4 w-4 text-emerald-500" />
                  : <XCircle className="h-4 w-4 text-destructive" />
                }
              </td>
              {dataKeys.map((key) => (
                <td key={key} className="px-3 py-2 whitespace-nowrap">
                  {row.data[key] !== null && row.data[key] !== undefined
                    ? String(row.data[key])
                    : <span className="text-muted-foreground/40">—</span>
                  }
                </td>
              ))}
              <td className="px-3 py-2">
                {row.errors.length > 0 && (
                  <ul className="space-y-0.5">
                    {row.errors.map((e, i) => (
                      <li key={i} className="text-xs text-destructive">{e}</li>
                    ))}
                  </ul>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
