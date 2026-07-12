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
        // Accounting module
        var modul = await db.Modules.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Name == "Muhasebe");
        if (modul == null)
        {
            modul = new Module
            {
                Name = "Muhasebe",
                DisplayName = "Accounting",
                Description = "Double-entry accounting: chart of accounts, vouchers, ledgers, reports",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Modules.Add(modul);
            await db.SaveChangesAsync();
        }

        // Accounting top menu group
        var muhasebeGroup = await db.Pages.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Name == "MuhasebeGroup");
        if (muhasebeGroup == null)
        {
            muhasebeGroup = new Page
            {
                Name = "MuhasebeGroup",
                DisplayName = "Accounting",
                Label = "Accounting",
                Icon = "book-open",
                Route = "#",
                ModuleId = modul.Id,
                OrderIndex = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Pages.Add(muhasebeGroup);
            await db.SaveChangesAsync();
        }

        // Pages (Name matches the controller [RequirePage] key exactly)
        var pages = new[]
        {
            new { Name = "MuhasebeHesapPlani", Label = "Chart of Accounts",   Route = "/muhasebe/hesap-plani",   Icon = (string?)"file",     Order = 1 },
            new { Name = "MuhasebeFis",        Label = "Accounting Vouchers",  Route = "/muhasebe/fisler",        Icon = (string?)"file",     Order = 2 },
            new { Name = "MuhasebeFisDetay",   Label = "Voucher Details",      Route = "/muhasebe/fis-detaylari", Icon = (string?)"file",     Order = 3 },
            new { Name = "MuhasebeDefter",     Label = "Ledgers",              Route = "/muhasebe/defterler",     Icon = (string?)"file",     Order = 4 },
            new { Name = "MuhasebeRapor",      Label = "Reports",              Route = "/muhasebe/raporlar",      Icon = (string?)"file",     Order = 5 },
            new { Name = "MuhasebeParametre",  Label = "Parameters",           Route = "/muhasebe/parametreler",  Icon = (string?)"settings", Order = 6 },
            new { Name = "MuhasebeDonemSonu",  Label = "Period End",           Route = "/muhasebe/donem-sonu",    Icon = (string?)"settings", Order = 7 },
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
                ParentPageId = muhasebeGroup.Id,
                OrderIndex = pd.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        // Update existing accounting pages' ParentPageId (idempotent)
        var muhasebeChildNames = new[] { "MuhasebeHesapPlani", "MuhasebeFis", "MuhasebeFisDetay",
                                         "MuhasebeDefter", "MuhasebeRapor", "MuhasebeParametre", "MuhasebeDonemSonu" };
        var existingChildren = await db.Pages.IgnoreQueryFilters()
            .Where(p => muhasebeChildNames.Contains(p.Name))
            .ToListAsync();

        bool changed = false;
        foreach (var child in existingChildren)
        {
            if (child.ParentPageId == muhasebeGroup.Id) continue;
            child.ParentPageId = muhasebeGroup.Id;
            changed = true;
        }
        if (changed) await db.SaveChangesAsync();

        // Link accounting module to all plans (so custom roles can be authorized)
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
            new { Name = "Temel",    Description = "Site management with basic features",            Price = 499.00m  },
            new { Name = "Standart", Description = "Site management with advanced features",         Price = 999.00m  },
            new { Name = "Premium",  Description = "Professional site management with all features", Price = 1999.00m },
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
        // Seed Core module
        var temelModule = await db.Modules.IgnoreQueryFilters().FirstOrDefaultAsync(m => m.Name == "Temel");
        if (temelModule == null)
        {
            temelModule = new Module
            {
                Name = "Temel",
                DisplayName = "Core Module",
                Description = "Core site management features",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Modules.Add(temelModule);
            await db.SaveChangesAsync();
        }

        // Top menu groups (parent pages – route "#", children are grouped beneath them in the sidebar)
        var menuGroups = new[]
        {
            new { Name = "SiteGroup",              Label = "Site",            Icon = (string?)"building",          Order = 1  },
            new { Name = "KisilerGroup",           Label = "People",          Icon = (string?)"users",             Order = 2  },
            new { Name = "FinansGroup",            Label = "Finance",         Icon = (string?)"credit-card",       Order = 3  },
            new { Name = "GuvenlikGroup",          Label = "Security",        Icon = (string?)"shield",            Order = 5  },
            new { Name = "IletisimGroup",          Label = "Reports",         Icon = (string?)"bell",              Order = 6  },
            new { Name = "IletisimYonetimGroup",   Label = "Communications",  Icon = (string?)"mail",              Order = 7  },
            new { Name = "TanimlarGroup",          Label = "Definitions",     Icon = (string?)"settings",          Order = 8  },
            new { Name = "TeknikGroup",            Label = "Technical",       Icon = (string?)"wrench",            Order = 9  },
            new { Name = "SayacGroup",             Label = "Meters",          Icon = (string?)"calculator",        Order = 10 },
            new { Name = "WebSitesiGroup",         Label = "Website",         Icon = (string?)"layout-dashboard",  Order = 11 },
            new { Name = "AdaGroup",               Label = "ADA",             Icon = (string?)"bar-chart3",        Order = 12 },
        };

        foreach (var g in menuGroups)
        {
            var exists = await db.Pages.IgnoreQueryFilters().AnyAsync(p => p.Name == g.Name);
            if (exists) continue;

            db.Pages.Add(new Page
            {
                Name = g.Name,
                DisplayName = g.Label,
                Label = g.Label,
                Icon = g.Icon,
                Route = "#",
                ModuleId = temelModule.Id,
                OrderIndex = g.Order,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        // Child pages
        var pagesToSeed = new[]
        {
            // --- SiteGroup ---
            new { Name = "Binalar",         Label = "Buildings",     Route = "/buildings",        Icon = (string?)"building",    Order = 1 },
            new { Name = "Daireler",        Label = "Units",         Route = "/units",            Icon = (string?)"home",        Order = 2 },
            new { Name = "DaireTipleri",    Label = "Unit Types",    Route = "/unit-types",       Icon = (string?)"layers",      Order = 3 },
            // --- KisilerGroup ---
            new { Name = "Kisiler",                       Label = "People",                          Route = "/persons",                                      Icon = (string?)"users",            Order = 1 },
            new { Name = "OnayBekleyenKisiler",           Label = "Pending Approval",                Route = "/persons/onay-bekleyen",                        Icon = (string?)"user-check",       Order = 2 },
            new { Name = "OnayBekleyenGuncellemeler",     Label = "Pending Updates",                 Route = "/persons/onay-bekleyen-guncellemeler",          Icon = (string?)"user-check",       Order = 3 },
            new { Name = "Personel",                      Label = "Staff",                           Route = "/personel",                                     Icon = (string?)"circle-user",      Order = 4 },
            // --- FinansGroup ---
            new { Name = "Aidatlar",              Label = "Dues",                       Route = "/aidatlar",                         Icon = (string?)"credit-card",    Order = 1  },
            new { Name = "AidatKalemleri",        Label = "Dues Items",                 Route = "/aidat-kalemleri",                  Icon = (string?)"list",           Order = 2  },
            new { Name = "BankaHareketleri",      Label = "Bank Transactions",          Route = "/finans/banka-hareketleri",         Icon = (string?)"landmark",       Order = 3  },
            new { Name = "BorcMakbuzu",           Label = "Debt Receipt",               Route = "/finans/borc-makbuzu",              Icon = (string?)"file",           Order = 4  },
            new { Name = "TahsilatMakbuzu",       Label = "Collection Receipt",         Route = "/finans/tahsilat-makbuzu",          Icon = (string?)"receipt",        Order = 5  },
            new { Name = "TopluBorclandirma",     Label = "Bulk Charging",              Route = "/finans/toplu-borclandirma",        Icon = (string?)"credit-card",    Order = 6  },
            new { Name = "TopluTahsilat",         Label = "Bulk Collection",            Route = "/finans/toplu-tahsilat",            Icon = (string?)"credit-card",    Order = 7  },
            new { Name = "DetayliTahsilat",       Label = "Detailed Collection Entry",  Route = "/finans/detayli-tahsilat",          Icon = (string?)"receipt",        Order = 8  },
            new { Name = "OtomatikBorclandirma",  Label = "Automatic Charging",         Route = "/finans/otomatik-borclandirma",     Icon = (string?)"calendar-check", Order = 9  },
            new { Name = "GelirFaturalari",       Label = "Income Invoices",            Route = "/finans/gelir/faturalar",           Icon = (string?)"file-text",      Order = 10 },
            new { Name = "GelirFisi",             Label = "Income Voucher",             Route = "/finans/gelir/fis",                 Icon = (string?)"file",           Order = 11 },
            new { Name = "GelirDagitimi",         Label = "Income Distribution",        Route = "/finans/gelir-dagitimi",            Icon = (string?)"trending-up",    Order = 12 },
            new { Name = "TahsilatMakbuzlariGelir", Label = "Collection Receipts",      Route = "/finans/gelir/tahsilat-makbuzlari", Icon = (string?)"receipt",        Order = 13 },
            new { Name = "GiderFaturalari",       Label = "Expense Invoices",           Route = "/finans/gider/faturalar",           Icon = (string?)"file-text",      Order = 14 },
            new { Name = "GiderFisi",             Label = "Expense Voucher",            Route = "/finans/gider/fis",                 Icon = (string?)"file",           Order = 15 },
            new { Name = "GiderDagitimi",         Label = "Expense Distribution",       Route = "/finans/gider-dagitimi",            Icon = (string?)"trending-down",  Order = 16 },
            new { Name = "IadeMakbuzu",           Label = "Refund Receipt",             Route = "/finans/iade-makbuzu",              Icon = (string?)"receipt",        Order = 17 },
            new { Name = "HesaplarArasiVirman",   Label = "Inter-Account Transfer",     Route = "/finans/virman",                    Icon = (string?)"arrow-left-right", Order = 18 },
            new { Name = "VirmanFisDetaylari",    Label = "Transfer Voucher Details",   Route = "/finans/virman-fis-detaylari",      Icon = (string?)"file",           Order = 19 },
            new { Name = "KasaTransfer",          Label = "Cash Transfer Voucher",      Route = "/finans/kasa-transfer",             Icon = (string?)"arrow-left-right", Order = 20 },
            new { Name = "DevirBakiye",           Label = "Opening Balance Entry",      Route = "/finans/devir-bakiye",              Icon = (string?)"arrow-left-right", Order = 21 },
            new { Name = "CariHesapAcilisFisi",   Label = "Receivable Opening Voucher", Route = "/finans/acilis-fisleri/cari-hesap", Icon = (string?)"file",           Order = 22 },
            new { Name = "KasaAcilisFisi",        Label = "Cash Opening Voucher",       Route = "/finans/acilis-fisleri/kasa",       Icon = (string?)"file",           Order = 23 },
            new { Name = "PersonelAcilisFisi",    Label = "Staff Opening Voucher",      Route = "/finans/acilis-fisleri/personel",   Icon = (string?)"file",           Order = 24 },
            new { Name = "IsletmeProjesi",        Label = "Operating Budget",           Route = "/finans/isletme-projesi",           Icon = (string?)"bar-chart3",     Order = 25 },
            new { Name = "TekrarlananEvraklar",   Label = "Recurring Documents",        Route = "/finans/tekrarlanan-evraklar",      Icon = (string?)"calendar",       Order = 26 },
            new { Name = "ExcelBanka",            Label = "Excel Bank Import",          Route = "/finans/excel-banka",               Icon = (string?)"file-spreadsheet", Order = 27 },
            new { Name = "IcraListesi",           Label = "Enforcement List",           Route = "/finans/icra/icra-listesi",         Icon = (string?)"gavel",          Order = 28 },
            new { Name = "TakibeGonder",          Label = "Send to Collection",         Route = "/finans/icra/takibe-gonder",        Icon = (string?)"gavel",          Order = 29 },
            new { Name = "TakipListesi",          Label = "Collection List",            Route = "/finans/icra/takip-listesi",        Icon = (string?)"list",           Order = 30 },
            new { Name = "KisiFinansalDurum",     Label = "Financial Status by Person", Route = "/finans/kisi-finansal-durum",       Icon = (string?)"pie-chart",      Order = 31 },
            // --- GuvenlikGroup ---
            new { Name = "Araclar",              Label = "Vehicles",                Route = "/vehicles",                        Icon = (string?)"car",          Order = 1  },
            new { Name = "GirisKartlari",        Label = "Access Cards",            Route = "/access-cards",                    Icon = (string?)"key",          Order = 2  },
            new { Name = "AracGirisCikis",       Label = "Vehicle Entry/Exit",      Route = "/guvenlik/arac-giris-cikis",       Icon = (string?)"car",          Order = 3  },
            new { Name = "GecisKapilari",        Label = "Access Gates",            Route = "/guvenlik/gecis-kapilari",         Icon = (string?)"door-open",    Order = 4  },
            new { Name = "GecisYetkiGruplari",   Label = "Access Permission Groups",Route = "/guvenlik/gecis-yetki-gruplari",   Icon = (string?)"shield",       Order = 5  },
            new { Name = "ZiyaretciGirisCikis",  Label = "Visitor Entry/Exit",      Route = "/guvenlik/ziyaretci-giris-cikis",  Icon = (string?)"users",        Order = 6  },
            new { Name = "ZiyaretciKartlari",    Label = "Visitor Cards",           Route = "/guvenlik/ziyaretci-kartlari",     Icon = (string?)"key",          Order = 7  },
            new { Name = "ZiyaretciQr",          Label = "Visitor QR Form",         Route = "/guvenlik/ziyaretci-qr",           Icon = (string?)"file",         Order = 8  },
            new { Name = "Otoparklar",           Label = "Parking Lots",            Route = "/guvenlik/otoparklar",             Icon = (string?)"car",          Order = 9  },
            new { Name = "KayipEsya",            Label = "Lost & Found",            Route = "/guvenlik/kayip-esya",             Icon = (string?)"package",      Order = 10 },
            new { Name = "Olaylar",              Label = "Incidents",               Route = "/guvenlik/olaylar",                Icon = (string?)"bell",         Order = 11 },
            new { Name = "ZamanAraligi",         Label = "Time Slots",              Route = "/guvenlik/zaman-araligi",          Icon = (string?)"calendar",     Order = 12 },
            new { Name = "GuvenlikParametreler", Label = "Security Parameters",     Route = "/guvenlik/parametreler",           Icon = (string?)"settings",     Order = 13 },
            // --- IletisimGroup ---
            new { Name = "Duyurular",                  Label = "Announcements",                         Route = "/announcements",                      Icon = (string?)"megaphone",       Order = 1  },
            new { Name = "DestekTalepleri",            Label = "Support Requests",                      Route = "/support-requests",                   Icon = (string?)"help-circle",     Order = 2  },
            new { Name = "RaporMerkezi",               Label = "Report Center",                         Route = "/iletisim/rapor-merkezi",             Icon = (string?)"bar-chart3",      Order = 3  },
            new { Name = "SiteIstatistikleri",         Label = "Site Statistics",                       Route = "/iletisim/site-istatistikleri",       Icon = (string?)"bar-chart3",      Order = 4  },
            new { Name = "FinansalHareketler",         Label = "Financial Movements",                   Route = "/iletisim/finansal-hareketler",       Icon = (string?)"trending-up",     Order = 5  },
            new { Name = "MaliDurum",                  Label = "Financial Position",                    Route = "/iletisim/mali-durum",                Icon = (string?)"pie-chart",       Order = 6  },
            new { Name = "GelirGiderRaporu",           Label = "Income & Expense Report",               Route = "/iletisim/gelir-gider-raporu",        Icon = (string?)"bar-chart3",      Order = 7  },
            new { Name = "OzetGelirGider",             Label = "Summary Income & Expense",              Route = "/iletisim/ozet-gelir-gider",          Icon = (string?)"bar-chart3",      Order = 8  },
            new { Name = "IsletmeProjesiRaporu",       Label = "Operating Budget Report",               Route = "/iletisim/isletme-projesi-raporu",    Icon = (string?)"file-search",     Order = 9  },
            new { Name = "IsletmeProjesiPlanlanan",    Label = "Budget Planned vs Actual",              Route = "/iletisim/isletme-projesi-planlanan", Icon = (string?)"bar-chart3",      Order = 10 },
            new { Name = "IsletmeProjesiDagilim",      Label = "Budget Distribution Report",            Route = "/iletisim/isletme-projesi-dagilim",   Icon = (string?)"pie-chart",       Order = 11 },
            new { Name = "FaturaDagilim",              Label = "Invoice Distribution Report",           Route = "/iletisim/fatura-dagilim",            Icon = (string?)"pie-chart",       Order = 12 },
            new { Name = "DetayliBorcListesi",         Label = "Detailed Debt List",                    Route = "/iletisim/detayli-borc-listesi",      Icon = (string?)"list",            Order = 13 },
            new { Name = "VadesiGecmisBorclar",        Label = "Overdue Debts",                         Route = "/iletisim/vadesi-gecmis-borclar",     Icon = (string?)"list",            Order = 14 },
            new { Name = "TarihAralikliBorc",          Label = "Date-Range Debt List",                  Route = "/iletisim/tarih-aralikli-borc",       Icon = (string?)"calendar",        Order = 15 },
            new { Name = "BorcKapamaListesi",          Label = "Debt Closure List",                     Route = "/iletisim/borc-kapama-listesi",        Icon = (string?)"list",            Order = 16 },
            new { Name = "SakinHesapEkstresi",         Label = "Resident Account Statement",            Route = "/iletisim/sakin-hesap-ekstresi",       Icon = (string?)"file-text",       Order = 17 },
            new { Name = "CariHesapEkstresi",          Label = "Receivable Account Statement",          Route = "/iletisim/cari-hesap-ekstresi",        Icon = (string?)"file-text",       Order = 18 },
            new { Name = "CariHesapListesi",           Label = "Receivable Account List",               Route = "/iletisim/cari-hesap-listesi",         Icon = (string?)"list",            Order = 19 },
            new { Name = "KasaEkstresi",               Label = "Cash Statement",                        Route = "/iletisim/kasa-ekstresi",              Icon = (string?)"file-text",       Order = 20 },
            new { Name = "GenelHesapEkstresi",         Label = "General Account Statement",             Route = "/iletisim/genel-hesap-ekstresi",       Icon = (string?)"file-text",       Order = 21 },
            new { Name = "KategoriBakiye",             Label = "Category Balance List",                 Route = "/iletisim/kategori-bakiye",            Icon = (string?)"list",            Order = 22 },
            new { Name = "DonemsekBakiye",             Label = "Periodic Balance List",                 Route = "/iletisim/donemsek-bakiye",            Icon = (string?)"calendar",        Order = 23 },
            new { Name = "OnlineBanka",                Label = "Online Bank Transactions",              Route = "/iletisim/online-banka",               Icon = (string?)"landmark",        Order = 24 },
            new { Name = "SakinlerListesi",            Label = "Residents List",                        Route = "/iletisim/sakinler-listesi",           Icon = (string?)"list",            Order = 25 },
            new { Name = "DaireIletisimBilgileri",     Label = "Unit Contact Info",                     Route = "/iletisim/daire-iletisim",             Icon = (string?)"file",            Order = 26 },
            new { Name = "DaireAracBilgileri",         Label = "Unit Vehicle Info",                     Route = "/iletisim/daire-arac-bilgileri",       Icon = (string?)"car",             Order = 27 },
            new { Name = "DaireBilgiFormlari",         Label = "Unit Info Forms",                       Route = "/iletisim/daire-bilgi-formlari",       Icon = (string?)"file",            Order = 28 },
            new { Name = "DaireGirisCikisRaporu",      Label = "Unit Entry/Exit Report",                Route = "/iletisim/daire-giris-cikis",          Icon = (string?)"bar-chart3",      Order = 29 },
            new { Name = "AdresEtiket",                Label = "Address Label Print",                   Route = "/iletisim/adres-etiket",               Icon = (string?)"file",            Order = 30 },
            new { Name = "BankaOdemeKodlari",          Label = "Bank Payment Codes",                    Route = "/iletisim/banka-odeme-kodlari",        Icon = (string?)"landmark",        Order = 31 },
            new { Name = "RaporBorcMakbuzlari",        Label = "Debt Receipts Report",                  Route = "/iletisim/rapor-borc-makbuzlari",      Icon = (string?)"file-search",     Order = 32 },
            new { Name = "RaporTahsilatMakbuzlari",    Label = "Collection Receipts Report",            Route = "/iletisim/rapor-tahsilat-makbuzlari",  Icon = (string?)"file-search",     Order = 33 },
            new { Name = "BorcMakbuzuToplu",           Label = "Bulk Debt Receipts",                    Route = "/iletisim/borc-makbuzu-toplu",         Icon = (string?)"file-spreadsheet", Order = 34 },
            new { Name = "TahsilatTopluDokum",         Label = "Bulk Collection Receipts",              Route = "/iletisim/tahsilat-toplu-dokum",       Icon = (string?)"file-spreadsheet", Order = 35 },
            new { Name = "BirlesikBorcMakbuzu",        Label = "Consolidated Debt Receipt",             Route = "/iletisim/birlesik-borc-makbuzu",      Icon = (string?)"file",            Order = 36 },
            new { Name = "BirlesikTahsilatMakbuzu",    Label = "Consolidated Collection Receipt",       Route = "/iletisim/birlesik-tahsilat-makbuzu",  Icon = (string?)"receipt",         Order = 37 },
            new { Name = "BorcsuklukBelgesi",          Label = "Debt-Free Certificate",                 Route = "/iletisim/borcsukluk-belgesi",         Icon = (string?)"file-text",       Order = 38 },
            new { Name = "IhtarYazisi",                Label = "Warning Letter",                        Route = "/iletisim/ihtar-yazisi",               Icon = (string?)"file-text",       Order = 39 },
            new { Name = "AvukatCikti",                Label = "Attorney Output Form",                  Route = "/iletisim/avukat-cikti",               Icon = (string?)"gavel",           Order = 40 },
            new { Name = "CariOdemeTalimati",          Label = "Payment Instruction",                   Route = "/iletisim/cari-odeme-talimati",        Icon = (string?)"file",            Order = 41 },
            new { Name = "PersonelBilgiFormu",         Label = "Staff Info Form",                       Route = "/iletisim/personel-bilgi-formu",       Icon = (string?)"file",            Order = 42 },
            new { Name = "PersonelEkstresi",           Label = "Staff Statement",                       Route = "/iletisim/personel-ekstresi",          Icon = (string?)"file-text",       Order = 43 },
            new { Name = "BordroDokumu",               Label = "Payroll Print",                         Route = "/iletisim/bordro-dokumu",              Icon = (string?)"file",            Order = 44 },
            new { Name = "IsletmeDefteri",             Label = "Operations Ledger",                     Route = "/iletisim/isletme-defteri",            Icon = (string?)"book-open",       Order = 45 },
            new { Name = "DenetimRaporu",              Label = "Audit Report",                          Route = "/iletisim/denetim-raporu",             Icon = (string?)"file-search",     Order = 46 },
            new { Name = "DenetimTablosu",             Label = "Audit Table",                           Route = "/iletisim/denetim-tablosu",            Icon = (string?)"file-search",     Order = 47 },
            new { Name = "HazirunListesi",             Label = "Attendance List",                       Route = "/iletisim/hazirun-listesi",            Icon = (string?)"list",            Order = 48 },
            new { Name = "AnketRaporu",                Label = "Survey Report",                         Route = "/iletisim/anket-raporu",               Icon = (string?)"bar-chart3",      Order = 49 },
            new { Name = "KisiOdemeSkoru",             Label = "Person Payment Score",                  Route = "/iletisim/kisi-odeme-skoru",           Icon = (string?)"bar-chart3",      Order = 50 },
            new { Name = "SiteOdemeSkoru",             Label = "Site Payment Score",                    Route = "/iletisim/site-odeme-skoru",           Icon = (string?)"pie-chart",       Order = 51 },
            new { Name = "PosNakitAkis",               Label = "POS Cash Flow",                         Route = "/iletisim/pos-nakit-akis",             Icon = (string?)"credit-card",     Order = 52 },
            new { Name = "GizlenmisEvraklar",          Label = "Hidden Documents",                      Route = "/iletisim/gizlenmis-evraklar",         Icon = (string?)"file",            Order = 53 },
            // --- TanimlarGroup ---
            new { Name = "RoleTipleri",         Label = "Role Types",               Route = "/role-types",                   Icon = (string?)"shield",         Order = 1  },
            new { Name = "Import",              Label = "Bulk Data Import",          Route = "/import",                       Icon = (string?)"download",       Order = 2  },
            new { Name = "Adalar",              Label = "Blocks",                    Route = "/tanimlar/adalar",              Icon = (string?)"building2",      Order = 3  },
            new { Name = "TanimCariler",        Label = "Accounts",                  Route = "/tanimlar/cariler",             Icon = (string?)"users",          Order = 4  },
            new { Name = "DaireGruplari",       Label = "Unit Groups",               Route = "/tanimlar/daire-gruplari",      Icon = (string?)"layers",         Order = 5  },
            new { Name = "EvrakKategorileri",   Label = "Document Categories",       Route = "/tanimlar/evrak-kategorileri",  Icon = (string?)"file",           Order = 6  },
            new { Name = "GelirGruplari",       Label = "Income Groups",             Route = "/tanimlar/gelir-gruplari",      Icon = (string?)"trending-up",    Order = 7  },
            new { Name = "GelirTanimlari",      Label = "Income Definitions",        Route = "/tanimlar/gelir-tanimlari",     Icon = (string?)"trending-up",    Order = 8  },
            new { Name = "GenelHesaplar",       Label = "General Accounts",          Route = "/tanimlar/genel-hesaplar",      Icon = (string?)"landmark",       Order = 9  },
            new { Name = "GiderGruplari",       Label = "Expense Groups",            Route = "/tanimlar/gider-gruplari",      Icon = (string?)"trending-down",  Order = 10 },
            new { Name = "GiderTanimlari",      Label = "Expense Definitions",       Route = "/tanimlar/gider-tanimlari",     Icon = (string?)"trending-down",  Order = 11 },
            new { Name = "KasaBanka",           Label = "Cash & Bank Definitions",   Route = "/tanimlar/kasa-banka",          Icon = (string?)"landmark",       Order = 12 },
            new { Name = "Tesisler",            Label = "Facilities",                Route = "/tanimlar/tesisler",            Icon = (string?)"building2",      Order = 13 },
            new { Name = "Yetkililer",          Label = "Authorized Persons",        Route = "/tanimlar/yetkililer",          Icon = (string?)"user-cog",       Order = 14 },
            // --- TeknikGroup ---
            new { Name = "Departmanlar",           Label = "Departments",                   Route = "/teknik/departmanlar",                    Icon = (string?)"users",           Order = 1 },
            new { Name = "IsTakipPano",            Label = "Work Tracking Board",           Route = "/teknik/is-takip/pano",                   Icon = (string?)"layout-dashboard", Order = 2 },
            new { Name = "IsTakipKayitlar",        Label = "Work Tracking Records",         Route = "/teknik/is-takip/kayitlar",               Icon = (string?)"list",            Order = 3 },
            new { Name = "IsTakipDurumRaporu",     Label = "Work Tracking Status Report",   Route = "/teknik/is-takip/durum-raporu",           Icon = (string?)"bar-chart3",      Order = 4 },
            new { Name = "DepartmanGorevleri",     Label = "Tasks by Department",           Route = "/teknik/is-takip/departman-gorevleri",    Icon = (string?)"clipboard-list",  Order = 5 },
            new { Name = "PersonelGorevleri",      Label = "Tasks by Staff",                Route = "/teknik/is-takip/personel-gorevleri",     Icon = (string?)"clipboard-list",  Order = 6 },
            new { Name = "OrtakAlanlar",           Label = "Common Areas",                  Route = "/teknik/ortak-alanlar",                   Icon = (string?)"building2",       Order = 7 },
            new { Name = "TalepTipleri",           Label = "Request Types",                 Route = "/teknik/talep-tipleri",                   Icon = (string?)"list",            Order = 8 },
            new { Name = "TeknikParametreler",     Label = "Technical Parameters",          Route = "/teknik/parametreler",                    Icon = (string?)"settings",        Order = 9 },
            // --- SayacGroup ---
            new { Name = "AnaSayac",          Label = "Main Meter",          Route = "/sayac/ana-sayac",        Icon = (string?)"calculator", Order = 1 },
            new { Name = "AraSayaclar",       Label = "Sub-Meters",          Route = "/sayac/ara-sayaclar",     Icon = (string?)"calculator", Order = 2 },
            new { Name = "DaireSayaclari",    Label = "Unit Meters",         Route = "/sayac/daire-sayaclari",  Icon = (string?)"home",       Order = 3 },
            new { Name = "Modemler",          Label = "Modems",              Route = "/sayac/modemler",         Icon = (string?)"settings",   Order = 4 },
            new { Name = "SayacDagitim",      Label = "Meter Distribution",  Route = "/sayac/dagitim",          Icon = (string?)"arrow-left-right", Order = 5 },
            new { Name = "BirimFiyat",        Label = "By Unit Price",       Route = "/sayac/birim-fiyat",      Icon = (string?)"credit-card", Order = 6 },
            new { Name = "FaturaUzerinden",   Label = "By Invoice",          Route = "/sayac/fatura-uzerinden", Icon = (string?)"file-text",  Order = 7 },
            new { Name = "SayacParametreler", Label = "Meter Parameters",    Route = "/sayac/parametreler",     Icon = (string?)"settings2",  Order = 8 },
            // --- IletisimYonetimGroup ---
            new { Name = "EpostaSablonlari",          Label = "Email Templates",                 Route = "/iletisim-yonetim/e-posta-sablonlari",          Icon = (string?)"mail",            Order = 1 },
            new { Name = "SmsSablonlari",             Label = "SMS Templates",                   Route = "/iletisim-yonetim/sms-sablonlari",              Icon = (string?)"message-square",  Order = 2 },
            new { Name = "MobilBildirimSablonlari",   Label = "Mobile Notification Templates",   Route = "/iletisim-yonetim/mobil-bildirim-sablonlari",   Icon = (string?)"bell",            Order = 3 },
            new { Name = "OtomatikBildirimler",       Label = "Automatic Notifications",         Route = "/iletisim-yonetim/otomatik-bildirimler",        Icon = (string?)"zap",             Order = 4 },
            new { Name = "TelefonRehberi",            Label = "Phone Directory",                 Route = "/iletisim-yonetim/telefon-rehberi",             Icon = (string?)"phone",           Order = 5 },
            // --- WebSitesiGroup ---
            new { Name = "AnaSayfaAyarlari",          Label = "Home Page Settings",      Route = "/web-sitesi/ana-sayfa-ayarlari",    Icon = (string?)"settings",        Order = 1 },
            new { Name = "WebAnket",                  Label = "Survey",                  Route = "/web-sitesi/anket",                 Icon = (string?)"file-text",       Order = 2 },
            new { Name = "ErisimHaklari",             Label = "Access Rights",           Route = "/web-sitesi/erisim-haklari",        Icon = (string?)"shield",          Order = 3 },
            new { Name = "FotografGalerisi",          Label = "Photo Gallery",           Route = "/web-sitesi/fotograf-galerisi",     Icon = (string?)"file",            Order = 4 },
            new { Name = "KullaniciIstatistikleri",   Label = "User Statistics",         Route = "/web-sitesi/kullanici-istatistikleri", Icon = (string?)"bar-chart3",   Order = 5 },
            new { Name = "OzelSayfalar",              Label = "Custom Pages",            Route = "/web-sitesi/ozel-sayfalar",         Icon = (string?)"file-text",       Order = 6 },
            new { Name = "SabitSayfalar",             Label = "Static Pages",            Route = "/web-sitesi/sabit-sayfalar",        Icon = (string?)"file-text",       Order = 7 },
            new { Name = "SiteTemasi",                Label = "Site Theme",              Route = "/web-sitesi/tema",                  Icon = (string?)"settings2",       Order = 8 },
            // --- AdaGroup ---
            new { Name = "AdaYonetimPaneli", Label = "ADA Management Panel", Route = "/ada",               Icon = (string?)"layout-dashboard", Order = 1 },
            new { Name = "AdaHakkinda",      Label = "About ADA",            Route = "/ada/hakkinda",      Icon = (string?)"help-circle",      Order = 2 },
            new { Name = "AdaPoster",        Label = "ADA Poster",           Route = "/ada/poster",        Icon = (string?)"file",             Order = 3 },
            new { Name = "VeriStudyosu",     Label = "Data Studio",          Route = "/ada/veri-studyosu", Icon = (string?)"bar-chart3",       Order = 4 },
            // --- SiteGroup additions ---
            new { Name = "Rezervasyonlar",  Label = "Reservations",   Route = "/rezervasyon",     Icon = (string?)"calendar-check", Order = 4 },
            // --- Other standalone pages (root level, no group) ---
            new { Name = "Ajanda",          Label = "Agenda",         Route = "/ajanda",          Icon = (string?)"calendar-days",  Order = 1 },
            new { Name = "Toplantilar",     Label = "Meetings",       Route = "/toplantilar",     Icon = (string?)"calendar-check", Order = 2 },
            new { Name = "Teklifler",       Label = "Proposals",      Route = "/teklifler",       Icon = (string?)"file-text",      Order = 3 },
            new { Name = "YapilacakIsler",  Label = "To-Do List",     Route = "/yapilacak-isler", Icon = (string?)"clipboard-list", Order = 4 },
            new { Name = "Ozet",            Label = "Summary",        Route = "/",                Icon = (string?)"layout-dashboard", Order = 0 },
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

        // "Aidatlar" page route updated from "/payments" to "/aidatlar"
        var aidatlarPage = await db.Pages.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Name == "Aidatlar" && p.Route == "/payments");
        if (aidatlarPage != null)
        {
            aidatlarPage.Route = "/aidatlar";
            await db.SaveChangesAsync();
        }

        // Parent–child relationship assignment
        await AssignPageParentsAsync(db);

        // Link the core module to all plans
        var planIds = await db.SubscriptionPlans.Select(p => p.Id).ToListAsync();
        foreach (var planId in planIds)
        {
            var linked = await db.SubscriptionPlanModules
                .AnyAsync(spm => spm.SubscriptionPlanId == planId && spm.ModuleId == temelModule.Id);
            if (linked) continue;

            db.SubscriptionPlanModules.Add(new SubscriptionPlanModule
            {
                SubscriptionPlanId = planId,
                ModuleId = temelModule.Id
            });
        }
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Assigns each child page's ParentPageId to the corresponding top menu group.
    /// Works safely on existing installations (idempotent).
    /// </summary>
    private static async Task AssignPageParentsAsync(MasterDbContext db)
    {
        // childName → parentGroupName
        var map = new Dictionary<string, string>
        {
            // SiteGroup
            { "Binalar",                      "SiteGroup"       },
            { "Daireler",                     "SiteGroup"       },
            { "DaireTipleri",                 "SiteGroup"       },
            { "Rezervasyonlar",               "SiteGroup"       },
            // KisilerGroup
            { "Kisiler",                      "KisilerGroup"    },
            { "OnayBekleyenKisiler",          "KisilerGroup"    },
            { "OnayBekleyenGuncellemeler",    "KisilerGroup"    },
            { "Personel",                     "KisilerGroup"    },
            // FinansGroup
            { "Aidatlar",                     "FinansGroup"     },
            { "AidatKalemleri",               "FinansGroup"     },
            { "BankaHareketleri",             "FinansGroup"     },
            { "BorcMakbuzu",                  "FinansGroup"     },
            { "TahsilatMakbuzu",              "FinansGroup"     },
            { "TopluBorclandirma",            "FinansGroup"     },
            { "TopluTahsilat",                "FinansGroup"     },
            { "DetayliTahsilat",              "FinansGroup"     },
            { "OtomatikBorclandirma",         "FinansGroup"     },
            { "GelirFaturalari",              "FinansGroup"     },
            { "GelirFisi",                    "FinansGroup"     },
            { "GelirDagitimi",                "FinansGroup"     },
            { "TahsilatMakbuzlariGelir",      "FinansGroup"     },
            { "GiderFaturalari",              "FinansGroup"     },
            { "GiderFisi",                    "FinansGroup"     },
            { "GiderDagitimi",                "FinansGroup"     },
            { "IadeMakbuzu",                  "FinansGroup"     },
            { "HesaplarArasiVirman",          "FinansGroup"     },
            { "VirmanFisDetaylari",           "FinansGroup"     },
            { "KasaTransfer",                 "FinansGroup"     },
            { "DevirBakiye",                  "FinansGroup"     },
            { "CariHesapAcilisFisi",          "FinansGroup"     },
            { "KasaAcilisFisi",               "FinansGroup"     },
            { "PersonelAcilisFisi",           "FinansGroup"     },
            { "IsletmeProjesi",               "FinansGroup"     },
            { "TekrarlananEvraklar",          "FinansGroup"     },
            { "ExcelBanka",                   "FinansGroup"     },
            { "IcraListesi",                  "FinansGroup"     },
            { "TakibeGonder",                 "FinansGroup"     },
            { "TakipListesi",                 "FinansGroup"     },
            { "KisiFinansalDurum",            "FinansGroup"     },
            // GuvenlikGroup
            { "Araclar",                      "GuvenlikGroup"   },
            { "GirisKartlari",                "GuvenlikGroup"   },
            { "AracGirisCikis",               "GuvenlikGroup"   },
            { "GecisKapilari",                "GuvenlikGroup"   },
            { "GecisYetkiGruplari",           "GuvenlikGroup"   },
            { "ZiyaretciGirisCikis",          "GuvenlikGroup"   },
            { "ZiyaretciKartlari",            "GuvenlikGroup"   },
            { "ZiyaretciQr",                  "GuvenlikGroup"   },
            { "Otoparklar",                   "GuvenlikGroup"   },
            { "KayipEsya",                    "GuvenlikGroup"   },
            { "Olaylar",                      "GuvenlikGroup"   },
            { "ZamanAraligi",                 "GuvenlikGroup"   },
            { "GuvenlikParametreler",         "GuvenlikGroup"   },
            // IletisimGroup
            { "Duyurular",                    "IletisimGroup"   },
            { "DestekTalepleri",              "IletisimGroup"   },
            { "RaporMerkezi",                 "IletisimGroup"   },
            { "SiteIstatistikleri",           "IletisimGroup"   },
            { "FinansalHareketler",           "IletisimGroup"   },
            { "MaliDurum",                    "IletisimGroup"   },
            { "GelirGiderRaporu",             "IletisimGroup"   },
            { "OzetGelirGider",               "IletisimGroup"   },
            { "IsletmeProjesiRaporu",         "IletisimGroup"   },
            { "IsletmeProjesiPlanlanan",      "IletisimGroup"   },
            { "IsletmeProjesiDagilim",        "IletisimGroup"   },
            { "FaturaDagilim",                "IletisimGroup"   },
            { "DetayliBorcListesi",           "IletisimGroup"   },
            { "VadesiGecmisBorclar",          "IletisimGroup"   },
            { "TarihAralikliBorc",            "IletisimGroup"   },
            { "BorcKapamaListesi",            "IletisimGroup"   },
            { "SakinHesapEkstresi",           "IletisimGroup"   },
            { "CariHesapEkstresi",            "IletisimGroup"   },
            { "CariHesapListesi",             "IletisimGroup"   },
            { "KasaEkstresi",                 "IletisimGroup"   },
            { "GenelHesapEkstresi",           "IletisimGroup"   },
            { "KategoriBakiye",               "IletisimGroup"   },
            { "DonemsekBakiye",               "IletisimGroup"   },
            { "OnlineBanka",                  "IletisimGroup"   },
            { "SakinlerListesi",              "IletisimGroup"   },
            { "DaireIletisimBilgileri",       "IletisimGroup"   },
            { "DaireAracBilgileri",           "IletisimGroup"   },
            { "DaireBilgiFormlari",           "IletisimGroup"   },
            { "DaireGirisCikisRaporu",        "IletisimGroup"   },
            { "AdresEtiket",                  "IletisimGroup"   },
            { "BankaOdemeKodlari",            "IletisimGroup"   },
            { "RaporBorcMakbuzlari",          "IletisimGroup"   },
            { "RaporTahsilatMakbuzlari",      "IletisimGroup"   },
            { "BorcMakbuzuToplu",             "IletisimGroup"   },
            { "TahsilatTopluDokum",           "IletisimGroup"   },
            { "BirlesikBorcMakbuzu",          "IletisimGroup"   },
            { "BirlesikTahsilatMakbuzu",      "IletisimGroup"   },
            { "BorcsuklukBelgesi",            "IletisimGroup"   },
            { "IhtarYazisi",                  "IletisimGroup"   },
            { "AvukatCikti",                  "IletisimGroup"   },
            { "CariOdemeTalimati",            "IletisimGroup"   },
            { "PersonelBilgiFormu",           "IletisimGroup"   },
            { "PersonelEkstresi",             "IletisimGroup"   },
            { "BordroDokumu",                 "IletisimGroup"   },
            { "IsletmeDefteri",               "IletisimGroup"   },
            { "DenetimRaporu",                "IletisimGroup"   },
            { "DenetimTablosu",               "IletisimGroup"   },
            { "HazirunListesi",               "IletisimGroup"   },
            { "AnketRaporu",                  "IletisimGroup"   },
            { "KisiOdemeSkoru",               "IletisimGroup"   },
            { "SiteOdemeSkoru",               "IletisimGroup"   },
            { "PosNakitAkis",                 "IletisimGroup"   },
            { "GizlenmisEvraklar",            "IletisimGroup"   },
            // TanimlarGroup
            { "RoleTipleri",                  "TanimlarGroup"   },
            { "Import",                       "TanimlarGroup"   },
            { "Adalar",                       "TanimlarGroup"   },
            { "TanimCariler",                 "TanimlarGroup"   },
            { "DaireGruplari",                "TanimlarGroup"   },
            { "EvrakKategorileri",            "TanimlarGroup"   },
            { "GelirGruplari",                "TanimlarGroup"   },
            { "GelirTanimlari",               "TanimlarGroup"   },
            { "GenelHesaplar",                "TanimlarGroup"   },
            { "GiderGruplari",                "TanimlarGroup"   },
            { "GiderTanimlari",               "TanimlarGroup"   },
            { "KasaBanka",                    "TanimlarGroup"   },
            { "Tesisler",                     "TanimlarGroup"   },
            { "Yetkililer",                   "TanimlarGroup"   },
            // TeknikGroup
            { "Departmanlar",                 "TeknikGroup"     },
            { "IsTakipPano",                  "TeknikGroup"     },
            { "IsTakipKayitlar",              "TeknikGroup"     },
            { "IsTakipDurumRaporu",           "TeknikGroup"     },
            { "DepartmanGorevleri",           "TeknikGroup"     },
            { "PersonelGorevleri",            "TeknikGroup"     },
            { "OrtakAlanlar",                 "TeknikGroup"     },
            { "TalepTipleri",                 "TeknikGroup"     },
            { "TeknikParametreler",           "TeknikGroup"     },
            // SayacGroup
            { "AnaSayac",                     "SayacGroup"      },
            { "AraSayaclar",                  "SayacGroup"      },
            { "DaireSayaclari",               "SayacGroup"      },
            { "Modemler",                     "SayacGroup"      },
            { "SayacDagitim",                 "SayacGroup"      },
            { "BirimFiyat",                   "SayacGroup"      },
            { "FaturaUzerinden",              "SayacGroup"      },
            { "SayacParametreler",            "SayacGroup"      },
            // IletisimYonetimGroup
            { "EpostaSablonlari",             "IletisimYonetimGroup" },
            { "SmsSablonlari",                "IletisimYonetimGroup" },
            { "MobilBildirimSablonlari",      "IletisimYonetimGroup" },
            { "OtomatikBildirimler",          "IletisimYonetimGroup" },
            { "TelefonRehberi",               "IletisimYonetimGroup" },
            // WebSitesiGroup
            { "AnaSayfaAyarlari",             "WebSitesiGroup"  },
            { "WebAnket",                     "WebSitesiGroup"  },
            { "ErisimHaklari",                "WebSitesiGroup"  },
            { "FotografGalerisi",             "WebSitesiGroup"  },
            { "KullaniciIstatistikleri",      "WebSitesiGroup"  },
            { "OzelSayfalar",                 "WebSitesiGroup"  },
            { "SabitSayfalar",                "WebSitesiGroup"  },
            { "SiteTemasi",                   "WebSitesiGroup"  },
            // AdaGroup
            { "AdaYonetimPaneli",             "AdaGroup"        },
            { "AdaHakkinda",                  "AdaGroup"        },
            { "AdaPoster",                    "AdaGroup"        },
            { "VeriStudyosu",                 "AdaGroup"        },
        };

        // Fetch parent pages in a single query
        var parentNames = map.Values.Distinct().ToList();
        var parents = await db.Pages.IgnoreQueryFilters()
            .Where(p => parentNames.Contains(p.Name))
            .ToDictionaryAsync(p => p.Name, p => p.Id);

        // Fetch child pages in a single query
        var childNames = map.Keys.ToList();
        var children = await db.Pages.IgnoreQueryFilters()
            .Where(p => childNames.Contains(p.Name))
            .ToListAsync();

        bool changed = false;
        foreach (var child in children)
        {
            if (!map.TryGetValue(child.Name, out var parentName)) continue;
            if (!parents.TryGetValue(parentName, out var parentId)) continue;
            if (child.ParentPageId == parentId) continue;

            child.ParentPageId = parentId;
            changed = true;
        }

        if (changed)
            await db.SaveChangesAsync();
    }
}
