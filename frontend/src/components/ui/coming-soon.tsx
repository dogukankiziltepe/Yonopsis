import { Construction } from 'lucide-react'

interface ComingSoonProps {
  title: string
}

export function ComingSoon({ title }: ComingSoonProps) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{title}</h1>
      </div>
      <div className="flex flex-col items-center justify-center h-64 gap-3 text-muted-foreground border rounded-lg">
        <Construction className="h-8 w-8" />
        <p className="text-sm">This page is coming soon.</p>
      </div>
    </div>
  )
}
