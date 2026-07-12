'use client'

import { useEffect, useState, useCallback } from 'react'
import { Check, X, UserCheck, Mail, Phone } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { personsApi } from '@/lib/api/persons'
import { UserTypeLabel } from '@/types/person'
import type { PersonDto } from '@/types/person'
import { showSuccess, showApiError } from '@/lib/toast'

export default function OnayBekleyenKisilerPage() {
  const [items, setItems] = useState<PersonDto[]>([])
  const [loading, setLoading] = useState(true)
  const [acting, setActing] = useState<string | null>(null)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await personsApi.getPending()
      setItems(res.data)
    } catch (e) { showApiError(e) }
    finally { setLoading(false) }
  }, [])

  useEffect(() => { load() }, [load])

  const handleApprove = async (id: string) => {
    setActing(id)
    try {
      await personsApi.approve(id)
      showSuccess('Person approved.')
      await load()
    } catch (e) { showApiError(e) }
    finally { setActing(null) }
  }

  const handleReject = async (id: string) => {
    setActing(id)
    try {
      await personsApi.reject(id)
      showSuccess('Person rejected.')
      await load()
    } catch (e) { showApiError(e) }
    finally { setActing(null) }
  }

  return (
    <div className="flex flex-col h-full">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h1 className="text-xl font-semibold">Pending Approval</h1>
          <p className="text-sm text-muted-foreground mt-0.5">People who have registered and are waiting for your approval.</p>
        </div>
        {!loading && items.length > 0 && (
          <Badge variant="secondary">{items.length} pending</Badge>
        )}
      </div>

      <div className="border rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-muted-foreground text-sm">Loading...</div>
        ) : items.length === 0 ? (
          <div className="p-12 text-center">
            <UserCheck className="h-8 w-8 mx-auto text-muted-foreground/40 mb-3" />
            <p className="text-muted-foreground text-sm font-medium">No pending approvals</p>
            <p className="text-xs text-muted-foreground mt-1">New registrations will appear here for review.</p>
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-muted/50 border-b">
              <tr>
                <th className="text-left px-4 py-2 font-medium">Name</th>
                <th className="text-left px-4 py-2 font-medium hidden md:table-cell">Contact</th>
                <th className="text-left px-4 py-2 font-medium w-32 hidden lg:table-cell">Role</th>
                <th className="w-36 px-4 py-2" />
              </tr>
            </thead>
            <tbody className="divide-y">
              {items.map((person) => (
                <tr key={person.userSiteId} className="hover:bg-muted/30">
                  <td className="px-4 py-3">
                    <div className="font-medium">{person.firstName} {person.lastName}</div>
                    <div className="text-xs text-muted-foreground md:hidden">{person.email}</div>
                  </td>
                  <td className="px-4 py-3 hidden md:table-cell">
                    <div className="flex items-center gap-1 text-muted-foreground text-xs">
                      <Mail className="h-3 w-3 shrink-0" />
                      <span>{person.email}</span>
                    </div>
                    {person.phoneNumber && (
                      <div className="flex items-center gap-1 text-muted-foreground text-xs mt-0.5">
                        <Phone className="h-3 w-3 shrink-0" />
                        <span>{person.phoneNumber}</span>
                      </div>
                    )}
                  </td>
                  <td className="px-4 py-3 hidden lg:table-cell">
                    {person.userType ? (
                      <Badge variant="outline" className="text-xs">{UserTypeLabel[person.userType]}</Badge>
                    ) : '—'}
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2 justify-end">
                      <Button
                        size="sm"
                        variant="outline"
                        className="h-7 text-xs text-destructive border-destructive/30 hover:bg-destructive/10"
                        disabled={acting === person.userSiteId}
                        onClick={() => handleReject(person.userSiteId)}
                      >
                        <X className="h-3 w-3 mr-1" />Reject
                      </Button>
                      <Button
                        size="sm"
                        className="h-7 text-xs"
                        disabled={acting === person.userSiteId}
                        onClick={() => handleApprove(person.userSiteId)}
                      >
                        <Check className="h-3 w-3 mr-1" />Approve
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
