'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { ArrowLeft } from 'lucide-react'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { sitesApi } from '@/lib/api/sites'
import { DbMode } from '@/types/site'

const schema = z.object({
  name: z.string().min(1, 'Site adı zorunludur').max(200),
  dbMode: z.nativeEnum(DbMode),
  adminFirstName: z.string().min(1, 'Ad zorunludur'),
  adminLastName: z.string().min(1, 'Soyad zorunludur'),
  adminEmail: z.string().email('Geçerli e-posta giriniz'),
  adminPhone: z.string().optional(),
  address: z.string().optional(),
  district: z.string().optional(),
  city: z.string().optional(),
  postalCode: z.string().optional(),
  phone: z.string().optional(),
  email: z.string().email().optional().or(z.literal('')),
  taxOffice: z.string().optional(),
  taxNumber: z.string().optional(),
})

type FormData = z.infer<typeof schema>

export default function NewSitePage() {
  const router = useRouter()
  const [error, setError] = useState<string | null>(null)

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: { dbMode: DbMode.Shared },
  })

  const onSubmit = async (data: FormData) => {
    setError(null)
    try {
      await sitesApi.create(data)
      router.push('/admin/sites')
    } catch (err: any) {
      setError(err.response?.data?.error ?? 'Site oluşturulamadı.')
    }
  }

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center gap-3">
        <Link href="/admin/sites">
          <Button variant="ghost" size="icon">
            <ArrowLeft className="h-4 w-4" />
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Yeni Site</h1>
          <p className="text-muted-foreground">Yeni bir site ve yönetici hesabı oluşturun</p>
        </div>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Card>
          <CardHeader><CardTitle className="text-base">Site Bilgileri</CardTitle></CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="col-span-2 space-y-1.5">
                <Label htmlFor="name">Site Adı *</Label>
                <Input id="name" {...register('name')} />
                {errors.name && <p className="text-sm text-destructive">{errors.name.message}</p>}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="dbMode">Veritabanı Modu *</Label>
                <select
                  id="dbMode"
                  {...register('dbMode', { valueAsNumber: true })}
                  className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                >
                  <option value={DbMode.Shared}>Shared (Paylaşımlı)</option>
                  <option value={DbMode.Dedicated}>Dedicated (Ayrı DB)</option>
                </select>
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="city">Şehir</Label>
                <Input id="city" {...register('city')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="district">İlçe</Label>
                <Input id="district" {...register('district')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="address">Adres</Label>
                <Input id="address" {...register('address')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="phone">Telefon</Label>
                <Input id="phone" {...register('phone')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="email">E-posta</Label>
                <Input id="email" type="email" {...register('email')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="taxNumber">Vergi No</Label>
                <Input id="taxNumber" {...register('taxNumber')} />
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="taxOffice">Vergi Dairesi</Label>
                <Input id="taxOffice" {...register('taxOffice')} />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-base">Yönetici Bilgileri</CardTitle></CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-1.5">
                <Label htmlFor="adminFirstName">Ad *</Label>
                <Input id="adminFirstName" {...register('adminFirstName')} />
                {errors.adminFirstName && <p className="text-sm text-destructive">{errors.adminFirstName.message}</p>}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="adminLastName">Soyad *</Label>
                <Input id="adminLastName" {...register('adminLastName')} />
                {errors.adminLastName && <p className="text-sm text-destructive">{errors.adminLastName.message}</p>}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="adminEmail">E-posta *</Label>
                <Input id="adminEmail" type="email" {...register('adminEmail')} />
                {errors.adminEmail && <p className="text-sm text-destructive">{errors.adminEmail.message}</p>}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="adminPhone">Telefon</Label>
                <Input id="adminPhone" {...register('adminPhone')} />
              </div>
            </div>
          </CardContent>
        </Card>

        {error && (
          <div className="rounded-md bg-destructive/10 border border-destructive/20 px-4 py-3">
            <p className="text-sm text-destructive">{error}</p>
          </div>
        )}

        <div className="flex gap-3">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Oluşturuluyor...' : 'Site Oluştur'}
          </Button>
          <Link href="/admin/sites">
            <Button type="button" variant="outline">İptal</Button>
          </Link>
        </div>
      </form>
    </div>
  )
}
