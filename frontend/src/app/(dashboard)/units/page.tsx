'use client'

import { useEffect, useState } from 'react'
import { Plus, Pencil, Trash2, Home } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { unitsApi } from '@/lib/api/units'
import { Unit } from '@/types/unit'

export default function UnitsPage() {
  const [units, setUnits] = useState<Unit[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = () => {
    setLoading(true)
    unitsApi.getAll()
      .then((res) => setUnits(res.data.value ?? []))
      .catch(() => setError('Daireler yüklenemedi.'))
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const handleDelete = async (id: string) => {
    if (!confirm('Bu daireyi silmek istediğinize emin misiniz?')) return
    try {
      await unitsApi.delete(id)
      load()
    } catch {
      setError('Silme işlemi başarısız.')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Daireler</h1>
          <p className="text-muted-foreground">Sitedeki daireleri yönetin</p>
        </div>
        <Button>
          <Plus className="h-4 w-4 mr-2" />
          Yeni Daire
        </Button>
      </div>

      {error && (
        <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3">
          <p className="text-sm text-destructive">{error}</p>
        </div>
      )}

      {loading ? (
        <div className="text-center py-12 text-muted-foreground">Yükleniyor...</div>
      ) : units.length === 0 ? (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12 text-center">
            <Home className="h-10 w-10 text-muted-foreground/50 mb-3" />
            <p className="text-muted-foreground">Henüz daire eklenmemiş</p>
          </CardContent>
        </Card>
      ) : (
        <div className="border rounded-lg overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b bg-muted/50">
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Daire No</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Blok</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Kat</th>
                <th className="text-left px-4 py-3 font-medium text-muted-foreground">Tip</th>
                <th className="px-4 py-3"></th>
              </tr>
            </thead>
            <tbody>
              {units.map((u) => (
                <tr key={u.id} className="border-b last:border-0 hover:bg-muted/30">
                  <td className="px-4 py-3 font-medium">{u.number}</td>
                  <td className="px-4 py-3 text-muted-foreground">{u.buildingName ?? '-'}</td>
                  <td className="px-4 py-3 text-muted-foreground">{u.floor ?? '-'}</td>
                  <td className="px-4 py-3 text-muted-foreground">{u.type ?? '-'}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-1 justify-end">
                      <Button variant="ghost" size="icon" className="h-7 w-7">
                        <Pencil className="h-3.5 w-3.5" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-destructive hover:text-destructive"
                        onClick={() => handleDelete(u.id)}
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
