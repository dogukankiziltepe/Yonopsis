'use client'

import { useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { BlueprintBg } from '@/components/blueprint-bg'
import { authApi } from '@/lib/api/auth'
import { useAuthStore } from '@/lib/store/auth.store'

const schema = z.object({
  email: z.string().email('Geçerli bir e-posta giriniz'),
  password: z.string().min(1, 'Şifre zorunludur'),
})

type FormData = z.infer<typeof schema>

export default function LoginPage() {
  const router = useRouter()
  const setLoginToken = useAuthStore((s) => s.setLoginToken)
  const [error, setError] = useState<string | null>(null)

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const onSubmit = async (data: FormData) => {
    setError(null)
    try {
      const res = await authApi.login(data)
      const token = res.data.accessToken
      const user = res.data;
      console.log(user,"user")
      setLoginToken(token, user)

      if (user.mustChangePassword) {
        router.push('/change-password')
      } else if (user.isSuperAdmin) {
        router.push('/admin/sites')
      } else {
        router.push('/site-selection')
      }
    } catch (err: any) {
      setError(err.response?.data?.error ?? 'Giriş başarısız. Bilgilerinizi kontrol ediniz.')
    }
  }

  return (
    <div className="fixed inset-0 grid lg:grid-cols-[1.05fr_0.95fr] overflow-auto bg-background">
      {/* Sol: gradient marka paneli */}
      <div
        className="relative hidden lg:flex flex-col justify-between p-12 text-white overflow-hidden"
        style={{ background: 'linear-gradient(135deg, var(--primary-grad-1), var(--primary-grad-2))' }}
      >
        <BlueprintBg className="absolute inset-0 text-white pointer-events-none" opacity={0.18} />
        <div className="relative flex items-center gap-2">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-[var(--accent-action)] font-extrabold">Y</div>
          <span className="text-lg font-extrabold tracking-tight">Yonopsis</span>
        </div>
        <div className="relative space-y-6">
          <h2 className="text-3xl font-extrabold leading-tight tracking-tight">
            Sitenizi tek panelden<br />profesyonelce yönetin.
          </h2>
          <div className="flex gap-8">
            <div>
              <div className="font-mono text-2xl font-bold tabular-nums">96</div>
              <div className="text-sm text-white/70">Daire</div>
            </div>
            <div>
              <div className="font-mono text-2xl font-bold tabular-nums">%87,4</div>
              <div className="text-sm text-white/70">Tahsilat</div>
            </div>
            <div>
              <div className="font-mono text-2xl font-bold tabular-nums">4</div>
              <div className="text-sm text-white/70">Blok</div>
            </div>
          </div>
        </div>
        <div className="relative text-sm text-white/60">© {new Date().getFullYear()} Yonopsis</div>
      </div>

      {/* Sağ: form */}
      <div className="flex items-center justify-center p-6">
        <Card className="w-full max-w-[400px] border-0 shadow-none lg:border lg:shadow-[var(--shadow-card)]">
          <CardHeader>
            <CardTitle className="text-2xl">Tekrar hoş geldiniz</CardTitle>
            <CardDescription>Devam etmek için hesabınıza giriş yapın.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-1.5">
                <Label htmlFor="email">E-posta</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="ornek@mail.com"
                  autoComplete="email"
                  {...register('email')}
                />
                {errors.email && <p className="text-sm text-destructive">{errors.email.message}</p>}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="password">Şifre</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  autoComplete="current-password"
                  {...register('password')}
                />
                {errors.password && <p className="text-sm text-destructive">{errors.password.message}</p>}
              </div>

              {error && (
                <div className="rounded-md bg-destructive/10 border border-destructive/20 px-3 py-2">
                  <p className="text-sm text-destructive">{error}</p>
                </div>
              )}

              <Button type="submit" variant="accent" className="w-full h-11" disabled={isSubmitting}>
                {isSubmitting ? 'Giriş yapılıyor...' : 'Giriş Yap'}
              </Button>

              <div className="text-center">
                <Link href="/forgot-password" className="text-sm text-muted-foreground hover:underline">
                  Şifremi unuttum
                </Link>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
