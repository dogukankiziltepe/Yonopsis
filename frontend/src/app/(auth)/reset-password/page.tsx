'use client'

import { useState } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { loginApi } from '@/lib/api/client'
import Link from 'next/link'

const schema = z.object({
  newPassword: z.string().min(6, 'Şifre en az 6 karakter olmalıdır'),
  confirmPassword: z.string().min(1, 'Şifre tekrarı zorunludur'),
}).refine(d => d.newPassword === d.confirmPassword, {
  message: 'Şifreler eşleşmiyor',
  path: ['confirmPassword'],
})
type FormData = z.infer<typeof schema>

export default function ResetPasswordPage() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const token = searchParams.get('token') ?? ''
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  if (!token) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Geçersiz Bağlantı</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">Şifre sıfırlama bağlantısı geçersiz veya eksik.</p>
          <Link href="/forgot-password" className="mt-4 block text-sm text-primary hover:underline">
            Yeni sıfırlama bağlantısı iste
          </Link>
        </CardContent>
      </Card>
    )
  }

  const onSubmit = async (data: FormData) => {
    setError(null)
    try {
      await loginApi.post('/api/auth/reset-password', { token, newPassword: data.newPassword })
      setSuccess(true)
      setTimeout(() => router.push('/login'), 3000)
    } catch (err: any) {
      setError(err.response?.data?.message ?? 'Geçersiz veya süresi dolmuş bağlantı.')
    }
  }

  if (success) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Şifre Değiştirildi</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="rounded-md bg-green-50 border border-green-200 px-4 py-3 mb-4">
            <p className="text-sm text-green-800">
              Şifreniz başarıyla değiştirildi. Giriş sayfasına yönlendiriliyorsunuz...
            </p>
          </div>
          <Link href="/login" className="text-sm text-primary hover:underline">
            Giriş sayfasına git
          </Link>
        </CardContent>
      </Card>
    )
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Yeni Şifre Belirle</CardTitle>
        <CardDescription>Hesabınız için yeni bir şifre giriniz.</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-1.5">
            <Label htmlFor="newPassword">Yeni Şifre</Label>
            <Input
              id="newPassword"
              type="password"
              placeholder="••••••••"
              autoComplete="new-password"
              {...register('newPassword')}
            />
            {errors.newPassword && <p className="text-sm text-destructive">{errors.newPassword.message}</p>}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="confirmPassword">Yeni Şifre (Tekrar)</Label>
            <Input
              id="confirmPassword"
              type="password"
              placeholder="••••••••"
              autoComplete="new-password"
              {...register('confirmPassword')}
            />
            {errors.confirmPassword && <p className="text-sm text-destructive">{errors.confirmPassword.message}</p>}
          </div>

          {error && (
            <div className="rounded-md bg-destructive/10 border border-destructive/20 px-3 py-2">
              <p className="text-sm text-destructive">{error}</p>
            </div>
          )}

          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting ? 'Kaydediliyor...' : 'Şifreyi Değiştir'}
          </Button>
        </form>
      </CardContent>
    </Card>
  )
}
