using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;

namespace SiteYonetimi.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(MasterDbContext db)
    {
        await SeedSuperAdminAsync(db);
        await SeedSubscriptionPlansAsync(db);
        await db.SaveChangesAsync();

        await SeedModulesAndPagesAsync(db);
        await SeedMuhasebeModuleAsync(db);
    }

    private static async Task SeedMuhasebeModuleAsync(MasterDbContext db)
    {
        // Muhasebe modülü
        var modul = await db.Modules.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Name == "Muhasebe");
        if (modul == null)
        {
            modul = new Module
            {
                Name = "Muhasebe",
                DisplayName = "Muhasebe",
                Description = "Çift taraflı muhasebe: hesap planı, fişler, defterler, raporlar",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Modules.Add(modul);
            await db.SaveChangesAsync();
        }

        // Sayfalar (Name = controller [RequirePage] anahtarı ile birebir)
        var pages = new[]
        {
            new { Name = "MuhasebeHesapPlani", Label = "Hesap Planı",     Route = "/muhasebe/hesap-plani",   Icon = (string?)"file",     Order = 20 },
            new { Name = "MuhasebeFis",        Label = "Muhasebe Fişleri", Route = "/muhasebe/fisler",        Icon = (string?)"file",     Order = 21 },
            new { Name = "MuhasebeFisDetay",   Label = "Fiş Detayları",    Route = "/muhasebe/fis-detaylari", Icon = (string?)"file",     Order = 22 },
            new { Name = "MuhasebeDefter",     Label = "Defterler",        Route = "/muhasebe/defterler",     Icon = (string?)"file",     Order = 23 },
            new { Name = "MuhasebeRapor",      Label = "Raporlar",         Route = "/muhasebe/raporlar",      Icon = (string?)"file",     Order = 24 },
            new { Name = "MuhasebeParametre",  Label = "Parametreler",     Route = "/muhasebe/parametreler",  Icon = (string?)"settings", Order = 25 },
            new { Name = "MuhasebeDonemSonu",  Label = "Dönem Sonu",       Route = "/muhasebe/donem-sonu",    Icon = (string?)"settings", Order = 26 },
        };

        foreach (var pd in pages)
        {
            var exists = await db.Pages.IgnoreQueryFilters().AnyAsync(p => p.Name == pd.Name);
            if (exists) continue;

            db.Pages.Add(new Page
            {
                Name = pd.Name,
                DisplayName = pd.Label,
                Label = pd.Label,
                Icon = pd.Icon,
                Route = pd.Route,
                ModuleId = modul.Id,
                OrderIndex = pd.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        // Muhasebe modülünü tüm planlara bağla (özel rollerin yetkilendirilebilmesi için)
        var planIds = await db.SubscriptionPlans.Select(p => p.Id).ToListAsync();
        foreach (var planId in planIds)
        {
            var linked = await db.SubscriptionPlanModules
                .AnyAsync(spm => spm.SubscriptionPlanId == planId && spm.ModuleId == modul.Id);
            if (linked) continue;

            db.SubscriptionPlanModules.Add(new SubscriptionPlanModule
            {
                SubscriptionPlanId = planId,
                ModuleId = modul.Id
            });
        }
        await db.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(MasterDbContext db)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == "gktg@mail.com");
        if (exists) return;

        db.Users.Add(new User
        {
            FirstName = "Super",
            LastName = "Admin",
            Email = "gktg@mail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Sifre1234"),
            IsActive = true,
            IsSuperAdmin = true,
            CreatedAt = DateTime.UtcNow,
            MustChangePassword = false
        });
    }

    private static async Task SeedSubscriptionPlansAsync(MasterDbContext db)
    {
        var plans = new[]
        {
            new { Name = "Temel",    Description = "Temel özellikler ile site yönetimi",              Price = 499.00m  },
            new { Name = "Standart", Description = "Gelişmiş özellikler ile site yönetimi",           Price = 999.00m  },
            new { Name = "Premium",  Description = "Tüm özellikler ile profesyonel site yönetimi",    Price = 1999.00m },
        };

        foreach (var plan in plans)
        {
            var exists = await db.SubscriptionPlans.AnyAsync(p => p.Name == plan.Name);
            if (exists) continue;

            db.SubscriptionPlans.Add(new SubscriptionPlan
            {
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task SeedModulesAndPagesAsync(MasterDbContext db)
    {
        // Seed Temel module
        var temelModule = await db.Modules.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Name == "Temel");
        if (temelModule == null)
        {
            temelModule = new Module
            {
                Name = "Temel",
                DisplayName = "Temel Modül",
                Description = "Temel site yönetimi özellikleri",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Modules.Add(temelModule);
            await db.SaveChangesAsync();
        }

        // Seed pages
        var pagesToSeed = new[]
        {
            new { Name = "Binalar",     Label = "Binalar",      Route = "/buildings",  Icon = (string?)"building", Order = 1 },
            new { Name = "Daireler",    Label = "Daireler",     Route = "/units",       Icon = (string?)"home",     Order = 2 },
            new { Name = "Kisiler",     Label = "Kişiler",      Route = "/persons",     Icon = (string?)"users",    Order = 3 },
            new { Name = "DaireTipleri", Label = "Daire Tipleri", Route = "/unit-types", Icon = (string?)"layers", Order = 4 },
            new { Name = "RoleTipleri",     Label = "Rol Tipleri",      Route = "/role-types",       Icon = (string?)"shield",      Order = 5  },
            new { Name = "DestekTalepleri", Label = "Destek Talepleri", Route = "/support-requests", Icon = (string?)"help-circle", Order = 6  },
            new { Name = "Aidatlar",        Label = "Aidatlar",         Route = "/payments",         Icon = (string?)"credit-card", Order = 7  },
            new { Name = "Araclar",         Label = "Araçlar",          Route = "/vehicles",         Icon = (string?)"car",         Order = 8  },
            new { Name = "GirisKartlari",   Label = "Giriş Kartları",   Route = "/access-cards",     Icon = (string?)"key",         Order = 9  },
            new { Name = "Duyurular",       Label = "Duyurular",        Route = "/announcements",    Icon = (string?)"bell",        Order = 10 },
            new { Name = "Import",          Label = "Toplu Veri Girişi", Route = "/import",          Icon = (string?)"download",    Order = 11 },
        };

        foreach (var pageData in pagesToSeed)
        {
            var exists = await db.Pages.IgnoreQueryFilters().AnyAsync(p => p.Name == pageData.Name);
            if (exists) continue;

            db.Pages.Add(new Page
            {
                Name = pageData.Name,
                DisplayName = pageData.Label,
                Label = pageData.Label,
                Icon = pageData.Icon,
                Route = pageData.Route,
                ModuleId = temelModule.Id,
                OrderIndex = pageData.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }
}
