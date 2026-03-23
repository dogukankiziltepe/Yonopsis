# SiteYönetimi — Geliştirici Rehberi

## Genel Kurallar

- Her geliştirme tamamlandıktan sonra CLAUDE.md güncellenmelidir.
- Her yeni özellik, değişiklik veya düzeltme için mutlaka yeni bir branch açılmalıdır. Doğrudan `main` üzerinde çalışılmaz.
- **PR'ı asla merge etme — `git merge` ve `gh pr merge` kullanılmaz. Merge işlemi kullanıcı tarafından yapılır.**
- **Her görev tamamlandıktan sonra ilgili Notion maddesinin Status'ü "Frontend In Progress" olarak güncellenmelidir.** Bunun için `notion-search` ile maddeyi bul, ardından `notion-update-page` ile Status property'sini güncelle.

```bash
git checkout -b feature/<özellik-adı>
git checkout -b fix/<hata-adı>
git checkout -b refactor/<konu>
```

---

## Proje Mimarisi

Modular Monolith. Projeler:

```
src/
├── SiteYonetimi.API           # Controllers, Filters, Middleware, Program.cs
├── SiteYonetimi.Shared        # BaseEntity, Result<T>, Enums
├── SiteYonetimi.Infrastructure # Entities, DbContexts, Services (PermissionService, EmailService)
└── Modules/
    ├── SiteYonetimi.Auth          # Login, Register, Token, ChangePassword işlemleri
    ├── SiteYonetimi.Tenancy       # Site, UserSite, RoleType yönetimi
    └── SiteYonetimi.SiteManagement # Site CRUD (SuperAdmin)
```

**Patterns:** CQRS (MediatR), FluentValidation, soft delete (global query filter), Result\<T\>

---

## Multi-Tenant Yapı

| DB | Amaç |
|---|---|
| MasterDB | Users, Sites, UserSites, RoleTypes, Pages, RolePermissions, RefreshTokens, Modules, SubscriptionPlans, SubscriptionPlanModules, SiteSubscriptions |
| SharedDB | DbMode=Shared sitelerin operasyonel verisi (SiteId kolonu ile ayrım): Buildings, Units |
| DedicatedDB | DbMode=Dedicated siteler için ayrı fiziksel DB — `SiteYonetimi_{siteId:N}` adıyla oluşturulur, connection string `Sites.ConnectionString`'de saklanır |

> **Not:** SharedDB ve DedicatedDB'nin şeması aynıdır — ikisi de `SharedTenantDbContext` ve `InitialSharedTenantDb` migration'ı kullanır. Tüm tablolarda `SiteId (Guid)` kolonu taşır.

### Site Entity (MasterDB)

`SiteYonetimi.Infrastructure.Entities.Site` — `BaseEntity`'den türer (Guid Id, CreatedAt, UpdatedAt, IsDeleted)

| Alan | Tip | Açıklama |
|---|---|---|
| Name | string (required, max 200) | Site adı |
| Address | string? (max 500) | Adres |
| District | string? (max 100) | İlçe |
| City | string? (max 100) | Şehir |
| PostalCode | string? (max 20) | Posta kodu |
| Phone | string? (max 50) | Telefon |
| Email | string? (max 256) | E-posta |
| TaxOffice | string? (max 200) | Vergi dairesi |
| TaxNumber | string? (max 50) | Vergi numarası |
| DbMode | DbMode enum | Shared=1, Dedicated=2 |
| ConnectionString | string? (max 1000) | Dedicated ise dolu, Shared ise null |
| IsActive | bool | Aktif/pasif |

### SharedDB Entities

`SiteYonetimi.Infrastructure.Entities.Shared` namespace'i altında:

- **Building** — `Guid Id`, `Guid SiteId`, `bool IsDeleted`
- **Unit** — `Guid Id`, `Guid SiteId`, `Guid BuildingId`, `bool IsDeleted`

Her ikisinde de `HasQueryFilter(!IsDeleted)` tanımlıdır.

Tüm entity ID'leri `Guid`'dir. `BaseEntity`: `Id (Guid)`, `CreatedAt`, `UpdatedAt`, `IsDeleted`.

---

## İki Aşamalı Token Sistemi

### Login Token (`tokenType: "login"`, 5 dakika)
- `/api/auth/login` endpoint'inden döner
- **Claims:** `NameIdentifier (Guid)`, `Email`, `GivenName`, `Surname`, `isSuperAdmin`, `tokenType="login"`, `mustChangePassword="True"` (yalnızca `MustChangePassword=true` ise eklenir)
- Sadece şu endpoint'lerde kullanılır:
  - `GET /api/site-selection/my-sites`
  - `POST /api/site-selection/select`
  - `POST /api/auth/change-password`
- Business endpoint'lerde kullanılamaz (SubscriptionMiddleware ve PermissionFilter tokenType="site" olmayanlarda devreye girmez)

### Site Token (`tokenType: "site"`, 60 dakika)
- `/api/site-selection/select` endpoint'inden döner
- **Claims:** `NameIdentifier (Guid)`, `Email`, `GivenName`, `Surname`, `isSuperAdmin`, `siteId`, `userType`, `roleTypeId`, `tokenType="site"`
- Tüm iş mantığı endpoint'lerinde kullanılır
- X-Site-Id header gerekmez — siteId claim'den okunur

### Refresh Token (30 gün)
- `/api/auth/refresh` endpoint'i tokenType'a bakarak uygun token üretir
- login token refresh → yeni login token
- site token refresh → yeni site token (siteId/userType claim'den)

### Auth API

| Endpoint | Method | Auth | Açıklama |
|---|---|---|---|
| `/api/auth/login` | POST | Anonymous | Email + password → LoginResponse (Login Token) |
| `/api/auth/register` | POST | Anonymous | Kullanıcı oluştur |
| `/api/auth/refresh` | POST | Anonymous | Refresh token → yeni token (tokenType'a göre) |
| `/api/auth/logout` | POST | Login/Site Token | Refresh token'ı iptal et |
| `/api/auth/change-password` | POST | Login/Site Token | Mevcut şifreyi değiştir, `MustChangePassword` false yapar |
| `/api/auth/me` | GET | Login/Site Token | Mevcut kullanıcı bilgisi |
| `/api/site-selection/my-sites` | GET | Login Token | Kullanıcının onaylı site listesi |
| `/api/site-selection/select` | POST | Login Token | Site seç → SelectSiteResponse (Site Token) |

### UserSite.Status

`UserSiteStatus` enum: `Pending=0`, `Approved=1`, `Rejected=2`

- Yalnızca `Approved` kayıtlar `/my-sites` listesinde görünür
- `Pending` kayıtlar `PendingApplications` listesinde döner
- `SelectSite` sadece `Approved` kayıtlar için çalışır

---

## Request Pipeline Sırası

```
1. ValidationExceptionMiddleware      → FluentValidation hatalarını 400 olarak döner
2. UseRateLimiter                     → Login endpoint'i 1dk/5 istek limiti
3. UseAuthentication                  → JWT doğrulama
4. UseAuthorization                   → .NET auth policy
5. MustChangePasswordMiddleware       → mustChangePassword claim "True" ise sadece /api/auth/change-password'a izin verir, diğerleri 403
6. SubscriptionMiddleware             → Sadece tokenType="site" ise: aktif abonelik kontrolü
7. PermissionFilter (ActionFilter)    → Sadece tokenType="site" ise: [RequirePage] yetki kontrolü
8. Controller Action                  → İş mantığı
```

---

## Abonelik (Subscription) Sistemi

- Her sitenin `SiteSubscription` kaydı olmalı (`StartDate`, `EndDate`, `IsActive`)
- `SubscriptionMiddleware` her request'te `X-Site-Id` header'ını okur
- Aktif subscription yoksa **403** döner
- SuperAdmin subscription kontrolünden muaftır
- `X-Site-Id` yoksa middleware devreye girmez

Modeller:
- `Module` — Sistemdeki özellik modülleri
- `SubscriptionPlan` — Abonelik paketleri (modül listesi + fiyat, `decimal(18,2)`)
- `SubscriptionPlanModule` — Plan ↔ Module many-to-many
- `SiteSubscription` — Sitenin aktif aboneliği

---

## Permission Sistemi

JWT token içine permission **gömülmez**. Her request'te `PermissionFilter` (ActionFilter) üzerinden DB'den sorgulanır.

### Kullanım

```csharp
// Controller seviyesinde
[RequirePage("Units")]
public class UnitsController : BaseController { }

// Action seviyesinde override
[HttpGet("report")]
[RequirePage("Units.Reports")]
public async Task<IActionResult> GetReport() { }

// RequirePage olmayan controller'lar filtreye takılmaz
public class AuthController : BaseController { }
```

### HTTP Metodu → Gereken Minimum PermissionLevel

| Method | Gereken Seviye |
|---|---|
| GET | ReadOnly (1) |
| POST | ReadAndCreate (2) |
| PUT / DELETE / PATCH | FullAccess (3) |

### PermissionLevel Enum

```
Unauthorized  = 0
ReadOnly      = 1
ReadAndCreate = 2
FullAccess    = 3
```

### Yetki Kontrol Akışı

1. `RequirePageAttribute` okunur (Action > Controller önceliği)
2. `RequirePage` yoksa → filter devreye girmez
3. `isSuperAdmin` claim "True" ise → geç
4. `X-Site-Id` header'dan SiteId alınır
5. `IPermissionService.GetUserPermissionAsync(userId, siteId, pageName)` çağrılır
6. `UserSite` → `RoleTypeId` → `RolePermission` → `PermissionLevel` sorgusu yapılır
7. `RoleTypeId` yoksa (Owner/Renter) → `ReadOnly` varsayılır
8. `RolePermission` kaydı yoksa → `Unauthorized (0)` varsayılır
9. `userPermission >= requiredLevel` değilse **403** dönülür

### İlgili Dosyalar

| Dosya | Açıklama |
|---|---|
| `SiteYonetimi.API/Filters/RequirePageAttribute.cs` | Attribute tanımı |
| `SiteYonetimi.API/Filters/PermissionFilter.cs` | IAsyncActionFilter implementasyonu |
| `SiteYonetimi.Infrastructure/Services/PermissionService.cs` | DB sorgulama |

---

## Seed Data

Uygulama her ayağa kalktığında `DataSeeder.SeedAsync()` otomatik çalışır (idempotent — mevcut kayıtlar tekrar eklenmez).

`DataSeeder.cs` konumu: `SiteYonetimi.Infrastructure/Seed/DataSeeder.cs`

| Seed | Kontrol | Değer |
|---|---|---|
| SuperAdmin kullanıcısı | Email = `gktg@mail.com` yoksa ekle | Şifre: `Sifre1234` (bcrypt) |
| Temel plan | Name = "Temel" yoksa ekle | 499₺ |
| Standart plan | Name = "Standart" yoksa ekle | 999₺ |
| Premium plan | Name = "Premium" yoksa ekle | 1999₺ |

`Program.cs` startup sırası:
```csharp
await db.Database.MigrateAsync(); // migration'ları otomatik uygular
await DataSeeder.SeedAsync(db);   // seed data
```

> Not: Yeni site eklendiğinde `CreateSiteCommand` içinde varsayılan `SiteAdmin` RoleType otomatik oluşturulur.

## Rate Limiting

- Sadece login endpoint'ine uygulanır (`[EnableRateLimiting("login")]`)
- Sabit pencere: 1 dakikada maksimum 5 istek
- Aşılırsa **429** döner
- `UseRateLimiter()` middleware `UseAuthentication()`'dan önce eklenir

## Permission Cache

- `IMemoryCache` kullanılır (singleton, DI'da `AddMemoryCache()` ile kayıtlı)
- Cache key: `perm_{userId}_{siteId}_{pageName}`
- TTL: 1 dakika — kullanıcı rolü veya permission'ı değiştiğinde 1 dk içinde otomatik expire olur
- Kritik sistemlerde TTL düşürülebilir

## E-posta Servisi (SendGrid)

- `IEmailService` arayüzü: `Task SendAsync(string to, string subject, string body)`
- `SendGridEmailService` implementasyonu: `SiteYonetimi.Infrastructure/Services/EmailService.cs`
- API key `appsettings.json` içinde `SendGrid:ApiKey` altında saklanır
- DI'da `scoped` olarak kayıtlıdır (`AddScoped<IEmailService, SendGridEmailService>()`)

---

## Audit Log

- Sadece write işlemleri loglanır: `POST`, `PUT`, `DELETE`, `PATCH`
- `GET` istekleri loglanmaz
- `AuditLogFilter` global action filter olarak kayıtlı
- Log formatı: `AUDIT | UserId=... | SiteId=... | Method=... | Path=... | StatusCode=... | Time=...`
- Audit logları ayrı dosyaya yazılır: `logs/audit-{tarih}.log` (nlog.config'de `auditfile` target)

## SiteManagement Modülü

SuperAdmin'e özel site yönetimi. `[RequirePage]` kullanılmaz; her action başında `if (!IsSuperAdmin) return Forbid();` kontrolü yapılır.

### Endpoints

| Endpoint | Method | Açıklama |
|---|---|---|
| `/api/sites` | GET | Tüm siteleri listele (SiteSummaryDto) |
| `/api/sites/{id:guid}` | GET | Site detayı (SiteDetailDto) |
| `/api/sites` | POST | Site oluştur → `{ id: Guid }` döner |
| `/api/sites/{id:guid}` | PUT | Site güncelle |
| `/api/sites/{id:guid}` | DELETE | Soft delete |

### CQRS Yapısı

```
Modules/SiteYonetimi.SiteManagement/Sites/
├── Commands/
│   ├── CreateSiteCommand.cs   → Result<Guid>  (MasterDbContext + dedicated DB provisioning)
│   ├── UpdateSiteCommand.cs   → Result        (MasterDbContext)
│   └── DeleteSiteCommand.cs   → Result        (MasterDbContext, soft delete)
├── Queries/
│   ├── GetAllSitesQuery.cs    → Result<List<SiteSummaryDto>>  (MasterDb + SharedTenantDb)
│   └── GetSiteByIdQuery.cs    → Result<SiteDetailDto>         (MasterDb + SharedTenantDb)
└── DTOs/
    └── SiteDtos.cs            → CreateSiteDto, UpdateSiteDto, SiteDetailDto, SiteSummaryDto
```

### CreateSiteCommand — Tek Transaction İçinde Yapılanlar

`POST /api/sites` aşağıdakileri tek bir DB transaction'ında gerçekleştirir:

1. `Site` kaydı oluşturulur (`DbMode` ve diğer alanlar set edilir)
2. Siteye özel `RoleType` (`Name="SiteAdmin"`, `IsDefault=true`) oluşturulur
3. `User` kaydı oluşturulur (`MustChangePassword=true`, bcrypt hashed geçici şifre)
4. `UserSite` kaydı oluşturulur (`UserType=Management`, `Status=Approved`, `RoleTypeId=SiteAdmin role`)
5. `DbMode == Dedicated` ise:
   - DB adı: `SiteYonetimi_{site.Id:N}` (dash yok)
   - MasterDb connection string'inden `SqlConnectionStringBuilder` ile dedicated connection string türetilir
   - Raw SQL ile DB oluşturulur: `IF NOT EXISTS (...) CREATE DATABASE [...]`
   - `SharedTenantDbContext` o connection string ile instantiate edilip `MigrateAsync` çalıştırılır
   - `site.ConnectionString` kaydedilir
6. `SaveChangesAsync` + `CommitAsync`
7. Geçici şifre `IEmailService` ile `AdminEmail`'e gönderilir

`CreateSiteDto` alanları: `Name`, `DbMode` (required), `AdminFirstName`, `AdminLastName`, `AdminEmail` (required), `AdminPhone` (optional), diğer site alanları (optional).

`BlockCount` ve `UnitCount`, `SharedTenantDbContext` üzerinden hesaplanır:
```csharp
var blockCount = await _sharedDb.Buildings.CountAsync(b => b.SiteId == site.Id);
var unitCount  = await _sharedDb.Units.CountAsync(u => u.SiteId == site.Id);
```

---

## SuperAdmin Controller Pattern

`[RequirePage]` kullanılmayan, SuperAdmin'e özel controller'larda yetki kontrolü şu şekilde yapılır:

```csharp
[Route("api/sites")]
public class SitesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsSuperAdmin) return Forbid();
        // ...
    }
}
```

- `[Authorize]` BaseController'da zaten tanımlı; tekrar eklenmez
- `[RequirePage]` eklenmez; `PermissionFilter` devreye girmez
- `IsSuperAdmin` BaseController'da tanımlıdır; controller'da yeniden tanımlanmaz

---

## Yeni Modül Eklerken

1. `src/Modules/` altına yeni classlib projesi oluştur
2. MediatR + FluentValidation.DependencyInjectionExtensions paketlerini ekle
3. Shared ve Infrastructure'a ProjectReference ekle
4. `XxxModule.cs` ile `AddXxxModule()` extension metodu yaz
5. `Program.cs`'e `builder.Services.AddXxxModule()` ekle
6. API projesine ProjectReference ekle
7. Controller `BaseController`'dan türesin
8. Tenant kullanıcılarına açık endpoint'ler için `[RequirePage]` ekle; `PageName`, `Pages` tablosundaki `Name` alanıyla eşleşmeli
9. SuperAdmin'e özel endpoint'ler için `[RequirePage]` ekleme; `if (!IsSuperAdmin) return Forbid();` kullan

---

## Migration Komutları

> **Mevcut migration dosyalarına asla dokunma. Sadece yeni migration ekle.**

```bash
# MasterDb
dotnet ef migrations add <Ad> --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context MasterDbContext
dotnet ef database update --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context MasterDbContext

# SharedTenantDb (hem Shared hem Dedicated DB şeması için)
dotnet ef migrations add <Ad> --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext --output-dir Migrations/SharedTenantDb
dotnet ef database update --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext
```

Migrations startup'ta `Database.Migrate()` ile otomatik uygulanır.

---

## Build & Run

```bash
dotnet restore
dotnet build
dotnet run --project ./src/SiteYonetimi.API
```

API: http://5241 / https://7032 — Swagger: `/swagger`

---

## Yapılandırma (appsettings.json)

```json
{
  "ConnectionStrings": {
    "MasterDb": "...",
    "SharedTenantDb": "..."
  },
  "Jwt": {
    "SecretKey": "...",
    "Issuer": "SiteYonetimi",
    "Audience": "SiteYonetimi",
    "AccessTokenExpiryMinutes": "60"
  },
  "SendGrid": {
    "ApiKey": "..."
  }
}
```
