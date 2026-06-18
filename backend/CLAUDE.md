# SiteYönetimi — Geliştirici Rehberi

## Genel Kurallar

- Her geliştirme tamamlandıktan sonra CLAUDE.md güncellenmelidir.
- Her yeni özellik, değişiklik veya düzeltme için mutlaka yeni bir branch açılmalıdır. Doğrudan `main` üzerinde çalışılmaz.
- **PR'ı asla merge etme — `git merge` ve `gh pr merge` kullanılmaz. Merge işlemi kullanıcı tarafından yapılır.**
- **Her görev tamamlandıktan sonra ilgili ClickUp görevinin Status'ü "backend progress" olarak güncellenmelidir.**

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

İki tür endpoint içerir: SuperAdmin'e özel site yönetimi ve tenant kullanıcılara açık operasyonel yönetim.

### Sites Endpoints (SuperAdmin)

SuperAdmin'e özel. `[RequirePage]` kullanılmaz; her action başında `if (!IsSuperAdmin) return Forbid();` kontrolü yapılır.

| Endpoint | Method | Açıklama |
|---|---|---|
| `/api/sites` | GET | Tüm siteleri listele (SiteSummaryDto) |
| `/api/sites/{id:guid}` | GET | Site detayı (SiteDetailDto) |
| `/api/sites` | POST | Site oluştur → `{ id: Guid }` döner |
| `/api/sites/{id:guid}` | PUT | Site güncelle |
| `/api/sites/{id:guid}` | DELETE | Soft delete |

### Buildings Endpoints (Tenant)

`[RequirePage("Binalar")]` ile korunur. Abonelik ve yetki kontrolü pipeline üzerinden otomatik yapılır. `siteId` JWT token claim'inden alınır — istemci göndermez.

| Endpoint | Method | Açıklama |
|---|---|---|
| `/api/buildings` | GET | Sitenin binalarını listele |
| `/api/buildings/{id:guid}` | GET | Bina detayı |
| `/api/buildings` | POST | Bina oluştur → `{ id: Guid }` döner |
| `/api/buildings/{id:guid}` | PUT | Bina güncelle |
| `/api/buildings/{id:guid}` | DELETE | Soft delete |

`CreateBuildingDto` alanları: `Name` (required, max 100). `SiteId` body'de gönderilmez.

### CQRS Yapısı

```
Modules/SiteYonetimi.SiteManagement/
├── Sites/
│   ├── Commands/
│   │   ├── CreateSiteCommand.cs   → Result<Guid>  (MasterDbContext + dedicated DB provisioning)
│   │   ├── UpdateSiteCommand.cs   → Result        (MasterDbContext)
│   │   └── DeleteSiteCommand.cs   → Result        (MasterDbContext, soft delete)
│   ├── Queries/
│   │   ├── GetAllSitesQuery.cs    → Result<List<SiteSummaryDto>>  (MasterDb + SharedTenantDb)
│   │   └── GetSiteByIdQuery.cs    → Result<SiteDetailDto>         (MasterDb + SharedTenantDb)
│   └── DTOs/
│       └── SiteDtos.cs
└── Buildings/
    ├── Commands/
    │   ├── CreateBuildingCommand.cs  → Result<Guid>  (SiteId ayrı parametre, SharedTenantDbContext)
    │   ├── UpdateBuildingCommand.cs  → Result
    │   └── DeleteBuildingCommand.cs  → Result (soft delete)
    ├── Queries/
    │   ├── GetBuildingsBySiteQuery.cs → Result<List<BuildingSummaryDto>>
    │   └── GetBuildingByIdQuery.cs    → Result<BuildingDetailDto>
    └── DTOs/
        └── BuildingDtos.cs
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

## Dinamik Sidebar Navigasyonu

### Page Entity (MasterDB)

`SiteYonetimi.Infrastructure.Entities.Page` — `BaseEntity`'den türer

| Alan | Tip | Açıklama |
|---|---|---|
| Name | string (required, max 100) | PermissionFilter ile eşleşen tekil anahtar |
| DisplayName | string (required, max 200) | Eski görünen başlık alanı (geriye dönük uyumluluk) |
| Label | string (required, max 100) | Frontend'de gösterilecek başlık |
| Icon | string? (max 50) | Opsiyonel ikon adı (örn. "building", "home") |
| Route | string (required, max 200) | Frontend route yolu (örn. "/buildings") |
| ModuleId | Guid (FK → Modules.Id) | Hangi modüle ait olduğu |
| ParentPageId | Guid? (FK → Pages.Id, self-ref) | Alt menü desteği |
| OrderIndex | int | Sidebar sıralaması |
| IsActive | bool (default: true) | Aktif/pasif |

`HasQueryFilter(!IsDeleted && IsActive)` uygulanır.

### GET /api/pages/my-pages

Auth: Site Token (`tokenType = "site"`) — `[Authorize]` yeterli, `[RequirePage]` eklenmez.

**Davranış:**
1. SuperAdmin → tüm aktif sayfaları `FullAccess` ile döner
2. Normal kullanıcı → aktif abonelikten erişilebilir ModuleId listesi çıkarılır
3. Bu modüllere ait sayfalar getirilir
4. RolePermissions tablosundan kullanıcının izin seviyesi belirlenir
5. `PermissionLevel < ReadOnly (1)` olan sayfalar listelenmez
6. Cache: `pages_{userId}_{siteId}`, TTL 2 dakika

**Response DTO:** `PageDto` (`Name`, `Label`, `Icon`, `Route`, `Order`, `ParentId`, `UserPermission`)

### İlgili Dosyalar

| Dosya | Açıklama |
|---|---|
| `SiteYonetimi.Tenancy/Pages/DTOs/PageDto.cs` | Response DTO |
| `SiteYonetimi.Tenancy/Pages/Queries/GetUserPagesQuery.cs` | CQRS query handler |
| `SiteYonetimi.API/Controllers/PagesController.cs` | GET /api/pages/my-pages |

### Seed Data

`DataSeeder.SeedAsync()` içinde `SeedModulesAndPagesAsync` çalışır:
- **Temel modülü** (`Name="Temel"`) yoksa oluşturulur
- **Binalar** (`Name="Binalar"`, Route="/buildings") ve **Daireler** (`Name="Daireler"`, Route="/units") seed edilir
- Her seed `Name` alanıyla idempotent kontrol yapar

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
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000", "http://localhost:5173" ]
  }
}
```

## CORS

- `appsettings.json` içindeki `Cors:AllowedOrigins` dizisinden okunur
- `AllowAnyHeader`, `AllowAnyMethod`, `AllowCredentials` ile yapılandırılır
- `UseCors()` middleware `UseRateLimiter()`'dan önce eklenir
- Yeni bir frontend origin eklemek için `AllowedOrigins` dizisine eklenir

---

## Muhasebe Modülü (geliştirme aşamasında)

Çift taraflı (double-entry) muhasebe modülü fazlı olarak geliştirilmektedir.
Tüm muhasebe verisi tenant (site) bazında izole edilir ve `SharedTenantDbContext`
içinde tutulur (Buildings/Units ile aynı tenant DB şeması).

### Faz 1 — Domain & Persistence (tamamlandı)

**Enum'lar** (`SiteYonetimi.Shared/Enums/MuhasebeEnums.cs`):
`HesapTipi`, `HesapKategorisi`, `NormalBakiye`, `CariTuru`, `FisTuru`,
`FisDurumu`, `DonemDurumu`.

**Entity'ler** (`SiteYonetimi.Infrastructure/Entities/Shared/Muhasebe/`):

| Entity | Açıklama |
|---|---|
| `HesapPlani` | Hiyerarşik hesap planı (self-ref `ParentId`). Cari hesaplar da bu tablodadır (`CariTuru` dolu). `(SiteId, HesapKodu)` unique. Sadece `FisKesilebilirMi` yaprak hesaplara fiş kesilir. |
| `MuhasebeDonem` | Yıl bazlı mali dönem. `(SiteId, Yil)` unique. `SonYevmiyeNo` yevmiye sayacı. |

- EF konfigürasyonu `SharedTenantDbContext.OnModelCreating` içinde; `HasQueryFilter(!IsDeleted)` + unique index'ler.
- Tenant izolasyonu mevcut desene uyar: sorgu/komutlarda `SiteId` filtresi + dedicated DB için per-request connection resolution.

**Seed iskeleti** (`SiteYonetimi.Infrastructure/Seed/MuhasebeSeeder.cs`):
`MuhasebeSeeder.SeedForSiteAsync(db, siteId)` — varsayılan (sadeleştirilmiş TDHP)
hesap planı + açık mali dönem seed eder (idempotent). Hiyerarşi (ParentId/Seviye)
ve "fiş kesilebilir" yaprak tespiti hesap kodundan otomatik türetilir.
> Site oluşturma akışına bağlanması (CreateSiteCommand) ileriki fazda yapılacak.

**Migration (bu ortamda .NET SDK kısıtı nedeniyle üretilemedi — lokalde çalıştırın):**
```bash
dotnet ef migrations add Muhasebe_Faz1 --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext --output-dir Migrations/SharedTenantDb
dotnet ef database update --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext
```

### Faz 2 — Hesap Planı CRUD + Cari Hesap (tamamlandı)

Yeni modül: **`SiteYonetimi.Muhasebe`** (`src/Modules/SiteYonetimi.Muhasebe`), `AddMuhasebeModule()` ile Program.cs'e kayıtlı. Mevcut CQRS/Result/SharedTenantDbContext desenine uyar.

**CQRS** (`Hesaplar/` altında):
- Commands: `CreateHesapCommand`, `UpdateHesapCommand`, `ToggleHesapAktifCommand`, `CreateCariHesapCommand`
- Queries: `GetHesapPlaniTreeQuery`, `GetHesapListQuery`, `GetHesapByIdQuery`, `GetCariHesaplarQuery`
- Servis: `ICariHesapService` / `CariHesapService` — cari hesap kodu üretimi tek noktada (Faz 5'te PersonCreated handler da kullanacak). PersonId ile idempotent. Ana hesap eşlemesi: Kiracı/EvSahibi→120, Tedarikçi→320, Personel→335 (Faz 6'da parametreyle override edilecek).

**Controller:** `MuhasebeHesaplarController` — `[RequirePage("MuhasebeHesapPlani")]`, route `api/muhasebe/...`:
`GET hesap-plani/tree`, `GET/POST hesaplar`, `GET/PUT hesaplar/{id}`, `PATCH hesaplar/{id}/aktif`, `GET/POST cari-hesaplar`.

> ⚠️ Yetki: `MuhasebeHesapPlani` page'i ve rol-permission kaydı **Faz 7'de** seed edilecek. O zamana kadar endpoint'ler permission'a takılır (SuperAdmin hariç). Bu, doküman faz planına uygundur.

**Frontend:** `/muhasebe/hesap-plani` — hiyerarşik ağaç + Cari Hesaplar sekmesi, oluştur/düzenle yan paneli (`frontend/src/app/(dashboard)/muhasebe/hesap-plani/page.tsx`, `lib/api/muhasebe.ts`, `types/muhasebe.ts`). Sidebar linki dinamik (page seed'i Faz 7).

> Migration gerekmez (Faz 2 yeni tablo eklemez). Faz 1 migration'ı yeterli.

### Faz 3 — Muhasebe Fişi (tamamlandı)

**Yeni entity'ler** (`Entities/Shared/Muhasebe/`): `MuhasebeFisi`, `MuhasebeFisiDetay`. EF config `SharedTenantDbContext`'te (decimal(18,2), cascade detay, `(SiteId,FisNo)` unique, query filter). **Yeni migration gerekir** (`Muhasebe_Faz3`).

**Dönem** (`Donemler/`): `CreateDonemCommand`, `GetDonemlerQuery`, `GetAktifDonemQuery` + `MuhasebeDonemlerController` (`api/muhasebe/donemler`).

**Fiş** (`Fisler/`):
- Commands: `CreateFis` (taslak), `UpdateFis` (yalnız taslak), `OnaylaFis`, `IptalFis`, `DeleteFis`
- Queries: `GetFisList` (sayfalı+filtre), `GetFisDetay`, `GetFisDetaylar` (düz satır ekranı)
- `IFisService`/`FisService`: dönem çözümleme (yoksa açık dönem üretir), satır doğrulama (borç XOR alacak, fiş kesilebilir/aktif hesap), fiş no üretimi (`{yil}-{sıra:0000000}`).
- `MuhasebeFislerController` (`api/muhasebe/fisler`, `fis-detaylari`), `[RequirePage("MuhasebeFis")]`.

**İş kuralları:** Dengesiz/boş fiş onaylanamaz. Onayda dönem `SonYevmiyeNo` **Serializable transaction** içinde atomik artırılıp sıralı yevmiye no atanır. Kapalı döneme fiş girilemez. Onaylı fiş düzenlenemez; iptalde **ters kayıt (storno)** fişi üretilir. Taslak fiş soft-delete ile silinir.

**Frontend:** `/muhasebe/fisler` (liste + dengeli giriş formu: Toplam Borç/Alacak/Fark, fark≠0 ise Onayla disabled, onaylıda salt-okunur + storno) ve `/muhasebe/fis-detaylari` (filtreli düz satır listesi).

**Migration komutu (lokalde):**
```bash
dotnet ef migrations add Muhasebe_Faz3 --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext --output-dir Migrations/SharedTenantDb
```

### Faz 4 — Defterler & Raporlar (tamamlandı)

Salt-okunur sorgular; yalnızca **onaylı (entegre)** fişler dikkate alınır. Migration gerekmez.

**Defterler** (`Raporlar/Queries`):
- `GetYevmiyeDefteriQuery` (tarih aralığı, sıralı tüm hareketler + toplam)
- `GetDefteriKebirQuery` (tek hesap, alt hesaplar dahil, yürüyen bakiye)
- `GetMuavinDefterQuery` (tek hesap, sadece kendisi)

**Raporlar:**
- `GetMizanQuery` (hesap bazlı borç/alacak toplam + bakiye, ΣBorç=ΣAlacak)
- `GetCariEkstreQuery` (tek cari hesap dökümü + yürüyen bakiye)
- `GetBorcAlacakDurumuQuery` (tüm carilerin özet bakiyeleri, CariTuru filtresi)

`IRaporService.HesapDefteriAsync` ortak hesap-defteri çıkarımı (açılış bakiyesi = aralık öncesi net, yürüyen bakiye). Controller: `MuhasebeRaporlarController` (`api/muhasebe/defterler/*`, `api/muhasebe/raporlar/*`), `[RequirePage("MuhasebeRapor")]`.

**Frontend:** `/muhasebe/defterler` (Yevmiye/Kebir/Muavin sekmeleri) ve `/muhasebe/raporlar` (Mizan/Cari Ekstre/Borç-Alacak). Excel/CSV export client-side (`lib/utils/exportCsv.ts`, UTF-8 BOM + ; ayraç).

### Faz 5 — Entegrasyon (Person → otomatik cari) (tamamlandı)

**Domain event altyapısı** (en az invaziv, MediatR `INotification`):
- `SiteYonetimi.Shared/Events/PersonEvents.cs`: `PersonCreatedDomainEvent`, `PersonRemovedFromSiteDomainEvent`. (Shared'a `MediatR.Contracts` paketi eklendi.)
- Yayınlama: `InvitePersonCommand` (Owner/Renter eklenince) ve `RemovePersonFromSiteCommand` `IPublisher.Publish` ile event atar.
- Dinleyiciler (Muhasebe `Integration/`): `PersonCreatedAccountingHandler` → `ICariHesapService.EnsureCariHesapAsync` (Owner→EvSahibi, Renter→Kiracı; idempotent, hata davet akışını bozmaz). `PersonRemovedAccountingHandler` → cari hesabı pasife alır.

**Site oluşturma → hesap planı seed:** `CreateSiteCommand` commit sonrası `MuhasebeSeeder.SeedForSiteAsync` çağırır (Shared veya Dedicated DB'ye göre connection). Böylece yeni sitede otomatik cari için gerekli ana hesaplar (120 vb.) hazır olur. (Faz 1'de ertelenen bağlama tamamlandı.)

> Migration gerekmez. **Opsiyonel** Payment→Tahsil ve Gider→Tediye otomatik fişleri parametre bağımlı olduğundan **Faz 6'ya** (MuhasebeParametre ile birlikte) bırakıldı.

### Faz 6 — Parametreler & Dönem Sonu (tamamlandı)

**Yeni entity:** `MuhasebeParametre` (tenant başına tek kayıt, `(SiteId)` unique). **Yeni migration gerekir** (`Muhasebe_Faz6`).

**Parametreler** (`Parametreler/`): `IParametreService.GetOrCreateAsync`, `GetMuhasebeParametreQuery`, `UpdateMuhasebeParametreCommand` + `MuhasebeParametreController` (`api/muhasebe/parametreler`, `[RequirePage("MuhasebeParametre")]`). Varsayılan hesaplar, ana hesap kodları, kod/fiş-no şablonları, para birimi, KDV, otomatik tahsil/tediye bayrakları.

**Dönem Sonu** (`Donemler/`): `IDonemSonuService.HesapBakiyeleriAsync`, `GetKapanisOnizlemeQuery` (gelir/gider/net + bakiyeler), `DonemSonuKapanisCommand` + `MuhasebeDonemSonuController` (`api/muhasebe/donem-sonu/onizleme|kapanis`, `[RequirePage("MuhasebeDonemSonu")]`).
- Kapanış: tüm bakiyeleri sıfırlayan **kapanış fişi** (onaylı) → dönem `Kapali` → bir sonraki dönem açılır → **bilanço** hesaplarını taşıyan **açılış fişi**; net sonuç `570` sonuç hesabına yazılır (yoksa oluşturulur). Serializable transaction.

**Opsiyonel Payment → Tahsil fişi:** `PaymentPaidDomainEvent` (`UpdatePaymentStatusCommand` "ödendi"ye ilk geçişte yayınlar) → `PaymentTahsilFisiHandler` parametre `OtomatikTahsilFisi` açıksa Borç Kasa / Alacak cari Tahsil fişi üretip onaylar (mevcut `CreateFis`/`OnaylaFis` komutlarını kullanır). Gider→Tediye: sistemde gider entity'si bulunmadığından kapsam dışı (parametre bayrağı ileride için hazır).

**Frontend:** `/muhasebe/parametreler` (form) ve `/muhasebe/donem-sonu` (önizleme → dengeli ise kapat wizard).

**Migration komutu (lokalde):**
```bash
dotnet ef migrations add Muhasebe_Faz6 --project ./src/SiteYonetimi.Infrastructure --startup-project ./src/SiteYonetimi.API --context SharedTenantDbContext --output-dir Migrations/SharedTenantDb
```

### Faz 7 — Yetkilendirme & Cila (tamamlandı)

Şema değişikliği yok — yalnızca **seed verisi** (Modules/Pages/SubscriptionPlanModule). Migration gerekmez.

`DataSeeder.SeedMuhasebeModuleAsync` (startup'ta idempotent):
- **"Muhasebe" modülü** oluşturulur.
- 7 sayfa seed edilir; `Page.Name` controller `[RequirePage]` anahtarlarıyla **birebir**: `MuhasebeHesapPlani`, `MuhasebeFis`, `MuhasebeFisDetay`, `MuhasebeDefter`, `MuhasebeRapor`, `MuhasebeParametre`, `MuhasebeDonemSonu`.
- Muhasebe modülü **tüm planlara** (`SubscriptionPlanModule`) bağlanır.

Controller hizalaması: `fis-detaylari` → `MuhasebeFisDetay`, defter aksiyonları → `MuhasebeDefter` (action-level `[RequirePage]` override). Böylece her route ↔ sayfa ↔ izin **1:1**.

**Yetki davranışı:** Varsayılan `SiteAdmin` rolü (`IsDefault`) hem sidebar (my-pages) hem `PermissionFilter` tarafında **FullAccess** alır → muhasebe sayfaları otomatik görünür ve tüm uçlar çalışır. Özel roller için yöneticiler Rol Tipleri ekranından sayfa bazında izin verir (mevcut akış).

> Muhasebe modülü 7 faz ile **tamamlandı**. Tüm fazların lokal `dotnet build` + 3 migration (`Muhasebe_Faz1/3/6`) ile doğrulanması gerekir (bu ortamda .NET SDK ağ politikasıyla engelliydi).

---

## Toplu Veri Girişi — Excel Import (SiteManagement)

Excel tabanlı toplu import: **Binalar / Daireler / Kullanıcılar**. `ClosedXML` ile şablon üretimi + parse.

- Servis: `IImportService` (`Modules/SiteYonetimi.SiteManagement/Import/Services/ImportService.cs`) — `GenerateTemplate`, `PreviewAsync` (parse + validation), `ConfirmAsync` (yalnız geçerli satırları transaction içinde kaydeder).
- Controller: `ImportController` (`api/import`, `[RequirePage("Import")]`):
  - `GET template/{type}` → .xlsx şablon
  - `POST preview/{type}` (multipart, max 5MB) → satır bazlı validation önizlemesi
  - `POST confirm/{type}` → geçerli satırları kaydet
- Validation: Buildings (ad zorunlu+unique, TotalFloors pozitif), Units (BuildingName mevcut, UnitNumber bina içinde unique), Users (email unique+format, TR telefon, Role∈{Resident,Owner,Manager}, Resident/Owner için daire eşleşmesi). UnitType yoksa otomatik oluşturulur; Owner/Resident dairenin Owner/TenantUserId'sine işlenir.
- Seed: `Import` sayfası (Temel modülü, route `/import`). Frontend: `/import` (tip seçimi, şablon indir, yükle, önizleme highlight, onay).

> Not: `Building` entity'sinde TotalFloors/Address alanları yok; şablonda alınır ancak yalnızca `Name` kalıcıdır (FloorNumber<=TotalFloors kuralı bu nedenle uygulanmaz). Users iki DB'ye yazdığından (MasterDb + SharedTenantDb) kayıt context bazında transaction'lıdır.
