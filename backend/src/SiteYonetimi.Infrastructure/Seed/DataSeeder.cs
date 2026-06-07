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
            new { Name = "Binalar",  Label = "Binalar",  Route = "/buildings", Icon = (string?)"building", Order = 1 },
            new { Name = "Daireler", Label = "Daireler", Route = "/units",      Icon = (string?)"home",     Order = 2 },
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
