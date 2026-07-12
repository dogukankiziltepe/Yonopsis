'use client'

import { ClipboardList } from 'lucide-react'

export default function OnayBekleyenGuncellemelerPage() {
  return (
    <div className="flex flex-col h-full">
      <div className="mb-4">
        <h1 className="text-xl font-semibold">Pending Profile Updates</h1>
        <p className="text-sm text-muted-foreground mt-0.5">Profile update requests from residents awaiting your review.</p>
      </div>

      <div className="border rounded-lg overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b">
            <tr>
              <th className="text-left px-4 py-2 font-medium">Name</th>
              <th className="text-left px-4 py-2 font-medium hidden md:table-cell">National ID</th>
              <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Email</th>
              <th className="w-32 px-4 py-2" />
            </tr>
          </thead>
          <tbody>
            <tr>
              <td colSpan={4}>
                <div className="p-12 text-center">
                  <ClipboardList className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
                  <p className="text-muted-foreground text-sm font-medium">No pending updates</p>
                  <p className="text-xs text-muted-foreground mt-1">
                    When residents submit profile update requests via the mobile app or resident portal,
                    they will appear here for review.
                  </p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  )
}
