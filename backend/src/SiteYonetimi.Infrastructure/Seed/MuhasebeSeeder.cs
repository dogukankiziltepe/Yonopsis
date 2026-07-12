using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Seed;

/// <summary>
/// Per-tenant (site) seed skeleton for the accounting module.
/// When a new site is created, the default (site-management-specific,
/// simplified uniform chart of accounts) and the open fiscal period are seeded.
///
/// NOTE (Phase 1): This seeder is a skeleton. Wiring it to the site-creation
/// flow (calling it from CreateSiteCommand) will be done in later phases.
/// The call is idempotent: skipped if a chart of accounts already exists for the site.
/// </summary>
public static class MuhasebeSeeder
{
    /// <summary>
    /// Seeds the default chart of accounts and the open fiscal period for a site.
    /// Calls SaveChanges.
    /// </summary>
    public static async Task SeedForSiteAsync(SharedTenantDbContext db, Guid siteId, int? yil = null)
    {
        await SeedHesapPlaniAsync(db, siteId);
        await SeedAcikDonemAsync(db, siteId, yil ?? DateTime.UtcNow.Year);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds the default chart of accounts. The hierarchy (ParentId),
    /// level, and "voucher-eligible" (leaf account) flag are computed automatically.
    /// </summary>
    public static async Task SeedHesapPlaniAsync(SharedTenantDbContext db, Guid siteId)
    {
        var alreadyExists = await db.HesapPlani.IgnoreQueryFilters()
            .AnyAsync(h => h.SiteId == siteId);
        if (alreadyExists) return;

        // Code, Name, Category, Normal balance direction — hierarchy derived from code.
        var definitions = VarsayilanPlan();

        // Code -> definition dictionary (for resolving parents)
        var codeSet = definitions.Select(t => t.Kod).ToHashSet();

        // Find the longest (closest) parent prefix code for a given code (e.g. "770.01" -> "770", "100" -> "10").
        string? FindParentCode(string code)
        {
            string? best = null;
            foreach (var candidate in codeSet)
            {
                if (candidate == code) continue;
                if (code.StartsWith(candidate) && (best is null || candidate.Length > best.Length))
                    best = candidate;
            }
            return best;
        }

        // Leaf accounts (no children) are voucher-eligible.
        bool IsLeaf(string code) => !codeSet.Any(a => a != code && a.StartsWith(code));

        // Code -> created entity (for ParentId assignment)
        var created = new Dictionary<string, HesapPlani>();

        // Level = ancestor chain length + 1; sort by code so parents are processed first.
        foreach (var t in definitions.OrderBy(t => t.Kod, StringComparer.Ordinal))
        {
            var parentCode = FindParentCode(t.Kod);
            var level = 1;
            var p = parentCode;
            while (p is not null)
            {
                level++;
                p = FindParentCode(p);
            }

            var account = new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = t.Kod,
                HesapAdi = t.Ad,
                HesapTipi = HesapTipi.Tekduzen,
                HesapKategorisi = t.Kategori,
                NormalBakiye = t.Bakiye,
                Seviye = level,
                ParentId = parentCode is not null && created.TryGetValue(parentCode, out var pe) ? pe.Id : null,
                FisKesilebilirMi = IsLeaf(t.Kod),
                AktifMi = true
            };

            created[t.Kod] = account;
            db.HesapPlani.Add(account);
        }
    }

    /// <summary>
    /// Creates an open fiscal period for the given year if one does not already exist.
    /// </summary>
    public static async Task SeedAcikDonemAsync(SharedTenantDbContext db, Guid siteId, int yil)
    {
        var alreadyExists = await db.MuhasebeDonemler.IgnoreQueryFilters()
            .AnyAsync(d => d.SiteId == siteId && d.Yil == yil);
        if (alreadyExists) return;

        db.MuhasebeDonemler.Add(new MuhasebeDonem
        {
            SiteId = siteId,
            Yil = yil,
            Ad = $"{yil} Fiscal Year",
            BaslangicTarihi = new DateOnly(yil, 1, 1),
            BitisTarihi = new DateOnly(yil, 12, 31),
            Durum = DonemDurumu.Acik,
            SonYevmiyeNo = 0
        });
    }

    private record HesapTanim(string Kod, string Ad, HesapKategorisi Kategori, NormalBakiye Bakiye);

    /// <summary>
    /// Site-management-specific simplified default chart of accounts (see task §11).
    /// Can be reviewed with a financial advisor and customized per tenant.
    /// </summary>
    private static List<HesapTanim> VarsayilanPlan() => new()
    {
        // 1 — CURRENT ASSETS
        new("1",   "CURRENT ASSETS",                          HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("10",  "LIQUID ASSETS",                           HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("100", "CASH",                                    HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("102", "BANKS",                                   HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("12",  "TRADE RECEIVABLES",                       HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("120", "BUYERS (DUES RECEIVABLES)",               HesapKategorisi.Aktif,  NormalBakiye.Borc),
        new("126", "DEPOSITS AND GUARANTEES GIVEN",           HesapKategorisi.Aktif,  NormalBakiye.Borc),

        // 3 — SHORT-TERM LIABILITIES
        new("3",   "SHORT-TERM LIABILITIES",                  HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("32",  "TRADE PAYABLES",                          HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("320", "SUPPLIERS (VENDORS)",                     HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("326", "DEPOSITS AND GUARANTEES RECEIVED",        HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("335", "PAYABLES TO STAFF",                       HesapKategorisi.Pasif,  NormalBakiye.Alacak),
        new("360", "TAXES AND FUNDS PAYABLE",                 HesapKategorisi.Pasif,  NormalBakiye.Alacak),

        // 6 — INCOME STATEMENT ACCOUNTS
        new("6",   "INCOME STATEMENT ACCOUNTS",               HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("60",  "GROSS SALES / REVENUES",                  HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("600", "DUES AND COMMON EXPENSE REVENUES",        HesapKategorisi.Gelir,  NormalBakiye.Alacak),
        new("642", "LATE PAYMENT INTEREST REVENUES",          HesapKategorisi.Gelir,  NormalBakiye.Alacak),

        // 7 — COST / EXPENSE ACCOUNTS
        new("7",      "COST ACCOUNTS",                        HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("77",     "GENERAL ADMINISTRATIVE EXPENSES",      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770",    "GENERAL ADMINISTRATIVE EXPENSES",      HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.01", "Electricity",                          HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.02", "Water",                                HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.03", "Natural Gas",                          HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.04", "Elevator Maintenance",                 HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.05", "Cleaning",                             HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.06", "Security",                             HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.07", "Landscaping/Garden",                   HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.08", "Insurance",                            HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.09", "Staff Salaries",                       HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.10", "Common Area Maintenance/Repair",       HesapKategorisi.Gider,  NormalBakiye.Borc),
        new("770.99", "Other Expenses",                       HesapKategorisi.Gider,  NormalBakiye.Borc),
    };
}
