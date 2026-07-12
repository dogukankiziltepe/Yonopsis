'use client'

import { Construction } from 'lucide-react'

export default function PageComponent() {
  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-xl font-semibold">Takibe Gönder</h1>
      </div>
      <div className="border rounded-lg flex flex-col items-center justify-center py-16 text-center">
        <Construction className="h-10 w-10 text-muted-foreground/50 mb-3" />
        <p className="text-muted-foreground font-medium">Bu sayfa yapım aşamasındadır</p>
        <p className="text-xs text-muted-foreground mt-1">Yakında burada olacak</p>
      </div>
    </div>
  )
}
