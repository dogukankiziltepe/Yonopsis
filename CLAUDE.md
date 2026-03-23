# Yonopsis — CLAUDE.md

## Genel Kurallar

- Her özellik, değişiklik veya düzeltme için yeni branch açılır. Doğrudan `main` üzerinde çalışılmaz.
- **PR'ı asla merge etme.** `git merge` ve `gh pr merge` kullanılmaz. Merge işlemi kullanıcı tarafından yapılır.
- Her görev tamamlandıktan sonra `backendclaude.md` güncellenmelidir.
- Her görev tamamlandıktan sonra ilgili Notion maddesinin Status'ü "Frontend In Progress" olarak güncellenir.

```bash
git checkout -b feature/<özellik-adı>
git checkout -b fix/<hata-adı>
git checkout -b refactor/<konu>
```

---

## Proje Yapısı

```
/Yonopsis
├── frontend/          # Next.js 16 uygulaması
└── backendclaude.md   # Backend mimari dokümantasyonu (backend kodu ayrı repoda)
```

Backend: .NET 8 Modular Monolith — `http://localhost:5241` / `https://localhost:7032`
Frontend: Next.js 16 — `http://localhost:3000`

---

## Frontend

### Stack
- **Next.js 16.1.7** App Router, **React 19**, **TypeScript 5**
- **Tailwind CSS v4**, **shadcn/ui** (Radix UI tabanlı)
- **Zustand v5** — auth state
- **Axios** — 2 ayrı instance (loginApi, siteApi)
- **react-hook-form** + **Zod** — form validasyonu
- **jwt-decode** — token parse
- Path alias: `@/*` → `src/*`

### Geliştirme
```bash
cd frontend
npm install
npm run dev    # http://localhost:3000
npm run build
npm start
```

### Klasör Yapısı
```
frontend/src/
├── app/
│   ├── (auth)/          # login, site-selection, change-password
│   ├── (dashboard)/     # / , /buildings, /units  → Management kullanıcıları
│   ├── (admin)/         # /admin/sites/**           → SuperAdmin
│   ├── (owner)/         # /owner                    → Owner kullanıcıları
│   └── (tenant)/        # /tenant                   → Renter kullanıcıları
├── components/
│   ├── layout/          # Sidebar, AdminSidebar, OwnerSidebar, TenantSidebar
│   └── ui/              # shadcn/ui bileşenleri
├── lib/
│   ├── api/             # client.ts, auth.ts, sites.ts, buildings.ts, units.ts
│   └── store/           # auth.store.ts (Zustand)
├── types/               # api.ts, auth.ts, site.ts, building.ts, unit.ts
└── proxy.ts             # Next.js 16 middleware (proxy() export)
```

### İki Axios Instance

**`loginApi`** — Auth & SuperAdmin işlemleri
- Token: `loginToken` (Zustand store'dan)
- Kullanım: `/api/auth/*`, `/api/site-selection/*`, `/api/sites/*`

**`siteApi`** — Business endpoint'leri
- Token: `siteToken` (Zustand store'dan)
- 401'de otomatik token refresh (refresh queue pattern)
- Kullanım: `/api/buildings/*`, `/api/units/*`, vb.

### Zustand Auth Store (`lib/store/auth.store.ts`)

| Alan | Tip | Açıklama |
|---|---|---|
| `loginToken` | `string \| null` | 5 dk ömürlü login token |
| `siteToken` | `string \| null` | 60 dk ömürlü site token |
| `user` | `UserClaims \| null` | Decode edilmiş token claim'leri |
| `setLoginToken(token, user)` | fn | session-stage=login cookie set eder |
| `setSiteToken(token, data)` | fn | session-stage=site cookie set eder |
| `clearTokens()` | fn | Tüm token ve cookie'leri temizler |

### Route Guard Mantığı

`proxy.ts` `session-stage` cookie'sine göre yönlendirme yapar:

| Route | Gereken Cookie |
|---|---|
| `/login` | — (var ise yönlendir) |
| `/site-selection`, `/change-password` | `session-stage=login` |
| `/`, `/buildings`, `/units` | `session-stage=site` |
| `/admin/*` | `session-stage=login` (SuperAdmin özel case) |

Layout'larda ek guard:
- `(admin)` → `user.isSuperAdmin === 'True'`
- `(dashboard)` → `user.userType === 'Management'`
- `(owner)` → `user.userType === 'Owner'`
- `(tenant)` → `user.userType === 'Renter'`

---

## İki Aşamalı Auth Akışı

```
Login → loginToken (5 dk)
  ├─ mustChangePassword=true → /change-password
  ├─ isSuperAdmin=true       → /admin/sites  (site seçimi atlanır)
  └─ normal kullanıcı        → /site-selection
                                    ↓
                              siteToken (60 dk)
                                    ↓
                    userType'a göre yönlendirme:
                      Management → /
                      Owner      → /owner
                      Renter     → /tenant
```

---

## Tip Tanımları

### API Sarmalayıcı (`types/api.ts`)
```typescript
interface ApiResult<T> {
  isSuccess: boolean
  value: T | null
  error: string | null
}

interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

enum PermissionLevel {
  Unauthorized = 0,
  ReadOnly = 1,
  ReadAndCreate = 2,
  FullAccess = 3
}
```

### UserClaims (`types/auth.ts`)
```typescript
interface UserClaims {
  userId: string
  email: string
  firstName: string
  lastName: string
  isSuperAdmin: string   // "True" | "False"
  siteId?: string
  siteName?: string
  userType?: string      // "Management" | "Owner" | "Renter"
  roleTypeId?: string
  roleName?: string
  tokenType: string      // "login" | "site"
}
```

---

## Backend Özeti (Detay: backendclaude.md)

### Mimari
- **Modular Monolith**, CQRS (MediatR), FluentValidation, Result\<T\>, soft delete

### Veritabanları
| DB | İçerik |
|---|---|
| MasterDB | Users, Sites, UserSites, RoleTypes, RefreshTokens, Subscription |
| SharedDB | DbMode=Shared sitelerin Buildings & Units (SiteId ile ayrım) |
| DedicatedDB | DbMode=Dedicated siteler için ayrı fiziksel DB |

### Modüller
```
src/Modules/
├── SiteYonetimi.Auth           # Login, Token, Refresh, ChangePassword
├── SiteYonetimi.Tenancy        # Site, UserSite, RoleType yönetimi
└── SiteYonetimi.SiteManagement # Site CRUD (SuperAdmin)
```

### Request Pipeline
```
ValidationExceptionMiddleware → RateLimiter → Authentication → Authorization
→ MustChangePasswordMiddleware → SubscriptionMiddleware → PermissionFilter → Action
```

### Yetki Sistemi
- `[RequirePage("PageName")]` attribute ile controller/action'a bağlanır
- SuperAdmin tüm kontrolleri atlar
- Permission DB'den sorgulanır, 1 dk cache'lenir
- HTTP metodu → gereken seviye: GET=ReadOnly, POST=ReadAndCreate, PUT/DELETE/PATCH=FullAccess

### Seed Data
- SuperAdmin: `gktg@mail.com` / `Sifre1234`
- Planlar: Temel (499₺), Standart (999₺), Premium (1999₺)

### Backend Çalıştırma
```bash
dotnet restore && dotnet build
dotnet run --project ./src/SiteYonetimi.API
# Swagger: http://localhost:5241/swagger
```

### Migration
```bash
# MasterDb
dotnet ef migrations add <Ad> --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context MasterDbContext

# SharedTenantDb
dotnet ef migrations add <Ad> --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext --output-dir Migrations/SharedTenantDb
```

> Mevcut migration dosyalarına asla dokunma. Sadece yeni migration ekle.

---

## Ortam Değişkenleri

`frontend/.env.local`:
```
NEXT_PUBLIC_API_URL=http://localhost:5241
```

`appsettings.json` (backend):
```json
{
  "ConnectionStrings": { "MasterDb": "...", "SharedTenantDb": "..." },
  "Jwt": { "SecretKey": "...", "Issuer": "SiteYonetimi", "Audience": "SiteYonetimi", "AccessTokenExpiryMinutes": "60" },
  "SendGrid": { "ApiKey": "..." }
}
```
