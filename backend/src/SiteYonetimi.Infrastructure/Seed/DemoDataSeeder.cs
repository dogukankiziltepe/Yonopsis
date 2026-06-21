using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Seed;

/// <summary>
/// Demo/test amaçlı örnek veri seeder'ı.
/// 2 site, 3'er blok, 21'er daire, kiracı/ev sahibi atamaları,
/// araçlar, geçiş kartları ve muhasebe tanımlarını içerir.
/// İdempotent: "Güneş Sitesi" zaten varsa atlanır.
/// </summary>
public static class DemoDataSeeder
{
    private static readonly string DemoPasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234");

    public static async Task SeedAsync(MasterDbContext masterDb, SharedTenantDbContext sharedDb)
    {
        if (await masterDb.Sites.IgnoreQueryFilters().AnyAsync(s => s.Name == "Güneş Sitesi"))
            return;

        var premiumPlan = await masterDb.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == "Premium");

        // ── Siteler ──────────────────────────────────────────────────────────
        var site1 = MakeSite("Güneş Sitesi", "Moda Cad. No:45", "Kadıköy", "İstanbul",
            "34710", "0216 123 45 67", "info@gunessitesi.com", "Kadıköy V.D.", "1234567890");
        var site2 = MakeSite("Mavi Köy Sitesi", "Barbaros Blv. No:120", "Beşiktaş", "İstanbul",
            "34349", "0212 456 78 90", "info@mavikoysitesi.com", "Beşiktaş V.D.", "9876543210");
        masterDb.Sites.AddRange(site1, site2);

        // ── SiteAdmin Rolleri ─────────────────────────────────────────────────
        var role1 = new RoleType { SiteId = site1.Id, Name = "SiteAdmin", IsDefault = true, CreatedAt = DateTime.UtcNow };
        var role2 = new RoleType { SiteId = site2.Id, Name = "SiteAdmin", IsDefault = true, CreatedAt = DateTime.UtcNow };
        masterDb.RoleTypes.AddRange(role1, role2);

        // ── Yöneticiler ───────────────────────────────────────────────────────
        var mgmt1 = MakeUser("Ahmet", "Yıldız", "admin@gunessitesi.com", "0555 111 00 01");
        var mgmt2 = MakeUser("Zeynep", "Çelik", "admin@mavikoysitesi.com", "0555 222 00 01");
        masterDb.Users.AddRange(mgmt1, mgmt2);
        masterDb.UserSites.Add(MakeUserSite(mgmt1.Id, site1.Id, UserType.Management, role1.Id));
        masterDb.UserSites.Add(MakeUserSite(mgmt2.Id, site2.Id, UserType.Management, role2.Id));

        // ── Abonelikler ───────────────────────────────────────────────────────
        if (premiumPlan is not null)
        {
            masterDb.SiteSubscriptions.AddRange(
                MakeSub(site1.Id, premiumPlan.Id),
                MakeSub(site2.Id, premiumPlan.Id));
        }

        // ── Kişiler — Site 1 ──────────────────────────────────────────────────
        var s1Owners = new[]
        {
            MakeUser("Mehmet",  "Kaya",     "mehmet.kaya@mail.com",    "0532 101 0001"),
            MakeUser("Ayşe",    "Demir",    "ayse.demir@mail.com",     "0532 101 0002"),
            MakeUser("Fatma",   "Şahin",    "fatma.sahin@mail.com",    "0532 101 0003"),
            MakeUser("Ali",     "Çelik",    "ali.celik@mail.com",      "0532 101 0004"),
            MakeUser("Zeynep",  "Arslan",   "zeynep.arslan@mail.com",  "0532 101 0005"),
            MakeUser("Hasan",   "Doğan",    "hasan.dogan@mail.com",    "0532 101 0006"),
            MakeUser("Elif",    "Yıldırım", "elif.yildirim@mail.com",  "0532 101 0007"),
            MakeUser("Mustafa", "Kurt",     "mustafa.kurt@mail.com",   "0532 101 0008"),
            MakeUser("Selin",   "Aydın",    "selin.aydin@mail.com",    "0532 101 0009"),
            MakeUser("Kemal",   "Yılmaz",   "kemal.yilmaz@mail.com",   "0532 101 0010"),
            MakeUser("Burak",   "Özdemir",  "burak.ozdemir@mail.com",  "0532 101 0011"),
            MakeUser("Merve",   "Koç",      "merve.koc@mail.com",      "0532 101 0012"),
            MakeUser("Serkan",  "Erdoğan",  "serkan.erdogan@mail.com", "0532 101 0013"),
            MakeUser("Pınar",   "Akdağ",    "pinar.akdag@mail.com",    "0532 101 0014"),
            MakeUser("Volkan",  "Öztürk",   "volkan.ozturk@mail.com",  "0532 101 0015"),
        };
        var s1Tenants = new[]
        {
            MakeUser("Canan",  "Yılmaz",  "canan.yilmaz@mail.com",  "0543 201 0001"),
            MakeUser("Tarık",  "Bozkurt", "tarik.bozkurt@mail.com", "0543 201 0002"),
            MakeUser("Sibel",  "Güneş",   "sibel.gunes@mail.com",   "0543 201 0003"),
            MakeUser("Deniz",  "Kaplan",  "deniz.kaplan@mail.com",  "0543 201 0004"),
            MakeUser("Ece",    "Polat",   "ece.polat@mail.com",     "0543 201 0005"),
            MakeUser("Oğuz",   "Kılıç",   "oguz.kilic@mail.com",    "0543 201 0006"),
            MakeUser("Berna",  "Çoban",   "berna.coban@mail.com",   "0543 201 0007"),
            MakeUser("Emre",   "Arslan",  "emre.arslan@mail.com",   "0543 201 0008"),
        };
        masterDb.Users.AddRange(s1Owners);
        masterDb.Users.AddRange(s1Tenants);
        foreach (var u in s1Owners)
            masterDb.UserSites.Add(MakeUserSite(u.Id, site1.Id, UserType.Owner));
        foreach (var u in s1Tenants)
            masterDb.UserSites.Add(MakeUserSite(u.Id, site1.Id, UserType.Renter));

        // ── Kişiler — Site 2 ──────────────────────────────────────────────────
        var s2Owners = new[]
        {
            MakeUser("Tamer",  "Özkan",   "tamer.ozkan@mail.com",   "0532 202 0001"),
            MakeUser("Nalan",  "Yıldız",  "nalan.yildiz@mail.com",  "0532 202 0002"),
            MakeUser("Rıfat",  "Güler",   "rifat.guler@mail.com",   "0532 202 0003"),
            MakeUser("Dilek",  "Şimşek",  "dilek.simsek@mail.com",  "0532 202 0004"),
            MakeUser("Okan",   "Kaya",    "okan.kaya@mail.com",     "0532 202 0005"),
            MakeUser("Bahar",  "Demir",   "bahar.demir@mail.com",   "0532 202 0006"),
            MakeUser("Gökhan", "Aslan",   "gokhan.aslan@mail.com",  "0532 202 0007"),
            MakeUser("İpek",   "Çetin",   "ipek.cetin@mail.com",    "0532 202 0008"),
            MakeUser("Mert",   "Yücel",   "mert.yucel@mail.com",    "0532 202 0009"),
            MakeUser("Tuğba",  "Öztürk",  "tugba.ozturk@mail.com",  "0532 202 0010"),
            MakeUser("Cenk",   "Aydın",   "cenk.aydin@mail.com",    "0532 202 0011"),
            MakeUser("Aylin",  "Koç",     "aylin.koc@mail.com",     "0532 202 0012"),
            MakeUser("Erdem",  "Öz",      "erdem.oz@mail.com",      "0532 202 0013"),
            MakeUser("Seda",   "Arslan",  "seda.arslan@mail.com",   "0532 202 0014"),
            MakeUser("Ufuk",   "Kılıç",   "ufuk.kilic@mail.com",    "0532 202 0015"),
        };
        var s2Tenants = new[]
        {
            MakeUser("Buse",    "Demirci", "buse.demirci@mail.com",  "0543 302 0001"),
            MakeUser("Tolga",   "Şahin",   "tolga.sahin@mail.com",   "0543 302 0002"),
            MakeUser("Pelin",   "Aksu",    "pelin.aksu@mail.com",    "0543 302 0003"),
            MakeUser("Kadir",   "Doğan",   "kadir.dogan@mail.com",   "0543 302 0004"),
            MakeUser("Gamze",   "Kurt",    "gamze.kurt@mail.com",    "0543 302 0005"),
            MakeUser("Levent",  "Yılmaz",  "levent.yilmaz@mail.com", "0543 302 0006"),
            MakeUser("Suna",    "Erdoğan", "suna.erdogan@mail.com",  "0543 302 0007"),
            MakeUser("Hüseyin", "Polat",   "huseyin.polat@mail.com", "0543 302 0008"),
        };
        masterDb.Users.AddRange(s2Owners);
        masterDb.Users.AddRange(s2Tenants);
        foreach (var u in s2Owners)
            masterDb.UserSites.Add(MakeUserSite(u.Id, site2.Id, UserType.Owner));
        foreach (var u in s2Tenants)
            masterDb.UserSites.Add(MakeUserSite(u.Id, site2.Id, UserType.Renter));

        await masterDb.SaveChangesAsync();

        // ── RolePermissions — SiteAdmin rolleri tüm sayfalara FullAccess ──────
        // GetUserPagesQuery sidebar için RolePermissions'ı doğrudan okur;
        // PermissionService'in IsDefault kısayolu sadece endpoint filtresi içindir.
        await SeedRolePermissionsAsync(masterDb, role1.Id);
        await SeedRolePermissionsAsync(masterDb, role2.Id);

        // ── SharedDB verileri ─────────────────────────────────────────────────
        await SeedSite1SharedAsync(sharedDb, site1.Id, s1Owners, s1Tenants);
        await SeedSite2SharedAsync(sharedDb, site2.Id, s2Owners, s2Tenants);

        // ── Muhasebe ──────────────────────────────────────────────────────────
        await MuhasebeSeeder.SeedForSiteAsync(sharedDb, site1.Id);
        await MuhasebeSeeder.SeedForSiteAsync(sharedDb, site2.Id);

        await SeedMuhasebeExtrasAsync(sharedDb, site1.Id, s1Owners, s1Tenants);
        await SeedMuhasebeExtrasAsync(sharedDb, site2.Id, s2Owners, s2Tenants);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Site 1 — Güneş Sitesi
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedSite1SharedAsync(
        SharedTenantDbContext db, Guid siteId, User[] owners, User[] tenants)
    {
        // Daire tipleri
        var ut1p1 = MakeUnitType(siteId, "1+1");
        var ut2p1 = MakeUnitType(siteId, "2+1");
        var ut3p1 = MakeUnitType(siteId, "3+1");
        db.UnitTypes.AddRange(ut1p1, ut2p1, ut3p1);
        await db.SaveChangesAsync();

        // Bloklar
        var blokA = MakeBuilding(siteId, "A Blok", 7);
        var blokB = MakeBuilding(siteId, "B Blok", 7);
        var blokC = MakeBuilding(siteId, "C Blok", 7);
        db.Buildings.AddRange(blokA, blokB, blokC);
        await db.SaveChangesAsync();

        // Daireler — A Blok
        // owners: 0=Mehmet(Dolu) 1=Ayşe(Kiralik) 2=Fatma(Kiralik) 3=Ali(Dolu) 4=Zeynep(Kiralik)
        // tenants: 0=Canan 1=Tarık 2=Sibel
        var units = new List<Unit>
        {
            MakeUnit(siteId, blokA.Id, ut2p1.Id, "1",  "1", 110m, 65m,  1800m, UnitStatus.Dolu,    owners[0].Id, null),
            MakeUnit(siteId, blokA.Id, ut2p1.Id, "2",  "1", 115m, 68m,  2200m, UnitStatus.Kiralik,  owners[1].Id, tenants[0].Id),
            MakeUnit(siteId, blokA.Id, ut2p1.Id, "3",  "2", 112m, 66m,  2200m, UnitStatus.Kiralik,  owners[2].Id, tenants[1].Id),
            MakeUnit(siteId, blokA.Id, ut1p1.Id, "4",  "2",  75m, 55m,  1500m, UnitStatus.Dolu,    owners[3].Id, null),
            MakeUnit(siteId, blokA.Id, ut3p1.Id, "5",  "3", 145m, 88m,  2800m, UnitStatus.Kiralik,  owners[4].Id, tenants[2].Id),
            MakeUnit(siteId, blokA.Id, ut2p1.Id, "6",  "3", 113m, 67m,  2200m, UnitStatus.Bos,     null,        null),
            MakeUnit(siteId, blokA.Id, ut1p1.Id, "7",  "4",  74m, 54m,  1500m, UnitStatus.Bos,     null,        null),

            // B Blok
            // owners: 5=Hasan(Dolu) 6=Elif(Kiralik) 7=Mustafa(Dolu) 8=Selin(Kiralik) 9=Kemal(Dolu)
            // tenants: 3=Deniz 4=Ece
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "1",  "1", 116m, 69m,  2200m, UnitStatus.Dolu,    owners[5].Id, null),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "2",  "1", 118m, 70m,  2200m, UnitStatus.Kiralik,  owners[6].Id, tenants[3].Id),
            MakeUnit(siteId, blokB.Id, ut3p1.Id, "3",  "2", 148m, 90m,  2900m, UnitStatus.Dolu,    owners[7].Id, null),
            MakeUnit(siteId, blokB.Id, ut1p1.Id, "4",  "2",  76m, 56m,  1500m, UnitStatus.Kiralik,  owners[8].Id, tenants[4].Id),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "5",  "3", 114m, 67m,  2200m, UnitStatus.Dolu,    owners[9].Id, null),
            MakeUnit(siteId, blokB.Id, ut1p1.Id, "6",  "3",  73m, 53m,  1500m, UnitStatus.Bos,     null,        null),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "7",  "4", 112m, 65m,  2200m, UnitStatus.Bos,     null,        null),

            // C Blok
            // owners: 10=Burak(Dolu) 11=Merve(Kiralik) 12=Serkan(Kiralik) 13=Pınar(Kiralik) 14=Volkan(Dolu)
            // tenants: 5=Oğuz 6=Berna 7=Emre
            MakeUnit(siteId, blokC.Id, ut3p1.Id, "1",  "1", 150m, 92m,  2900m, UnitStatus.Dolu,    owners[10].Id, null),
            MakeUnit(siteId, blokC.Id, ut2p1.Id, "2",  "1", 116m, 68m,  2200m, UnitStatus.Kiralik,  owners[11].Id, tenants[5].Id),
            MakeUnit(siteId, blokC.Id, ut1p1.Id, "3",  "2",  76m, 56m,  1600m, UnitStatus.Kiralik,  owners[12].Id, tenants[6].Id),
            MakeUnit(siteId, blokC.Id, ut2p1.Id, "4",  "2", 118m, 70m,  2300m, UnitStatus.Kiralik,  owners[13].Id, tenants[7].Id),
            MakeUnit(siteId, blokC.Id, ut3p1.Id, "5",  "3", 152m, 94m,  3000m, UnitStatus.Dolu,    owners[14].Id, null),
            MakeUnit(siteId, blokC.Id, ut2p1.Id, "6",  "3", 114m, 66m,  2200m, UnitStatus.Bos,     null,        null),
            MakeUnit(siteId, blokC.Id, ut1p1.Id, "7",  "4",  74m, 54m,  1600m, UnitStatus.Bos,     null,        null),
        };
        db.Units.AddRange(units);
        await db.SaveChangesAsync();

        // Araçlar — dolu/kiralık dairelere birer araç
        var occupiedUnits = units.Where(u => u.Status != UnitStatus.Bos).ToList();
        var vehicles = new[]
        {
            MakeVehicle(siteId, occupiedUnits[0].Id,  owners[0].Id,  "34 ABC 123", "Toyota",     "Corolla",   "Beyaz",   2020),
            MakeVehicle(siteId, occupiedUnits[1].Id,  tenants[0].Id, "34 DEF 456", "Honda",      "Civic",     "Gri",     2021),
            MakeVehicle(siteId, occupiedUnits[2].Id,  tenants[1].Id, "34 GHI 789", "Volkswagen", "Polo",      "Siyah",   2019),
            MakeVehicle(siteId, occupiedUnits[3].Id,  owners[3].Id,  "34 JKL 012", "Renault",    "Clio",      "Mavi",    2022),
            MakeVehicle(siteId, occupiedUnits[4].Id,  tenants[2].Id, "34 MNO 345", "Ford",       "Focus",     "Kırmızı", 2018),
            MakeVehicle(siteId, occupiedUnits[5].Id,  owners[5].Id,  "34 PQR 678", "Hyundai",    "i20",       "Beyaz",   2023),
            MakeVehicle(siteId, occupiedUnits[6].Id,  tenants[3].Id, "34 STU 901", "Kia",        "Picanto",   "Gümüş",   2020),
            MakeVehicle(siteId, occupiedUnits[7].Id,  owners[7].Id,  "34 VWX 234", "BMW",        "3 Serisi",  "Siyah",   2021),
            MakeVehicle(siteId, occupiedUnits[8].Id,  tenants[4].Id, "34 YZA 567", "Mercedes",   "C200",      "Beyaz",   2022),
            MakeVehicle(siteId, occupiedUnits[9].Id,  owners[9].Id,  "34 BCD 890", "Fiat",       "Egea",      "Gri",     2019),
            MakeVehicle(siteId, occupiedUnits[10].Id, owners[10].Id, "34 EFG 123", "Audi",       "A3",        "Mavi",    2023),
            MakeVehicle(siteId, occupiedUnits[11].Id, tenants[5].Id, "34 HIJ 456", "Peugeot",    "208",       "Kırmızı", 2020),
            MakeVehicle(siteId, occupiedUnits[12].Id, tenants[6].Id, "34 KLM 789", "Nissan",     "Micra",     "Sarı",    2021),
            MakeVehicle(siteId, occupiedUnits[13].Id, tenants[7].Id, "34 NOP 012", "Skoda",      "Fabia",     "Gümüş",   2022),
            MakeVehicle(siteId, occupiedUnits[14].Id, owners[14].Id, "34 QRS 345", "Volvo",      "XC40",      "Beyaz",   2023),
        };
        db.Vehicles.AddRange(vehicles);

        // Geçiş Kartları
        var allResidents = occupiedUnits.Select((u, i) =>
        {
            var userId = u.TenantUserId ?? u.OwnerUserId!.Value;
            return MakeAccessCard(siteId, userId, u.Id, $"GS-{i + 1:D4}");
        }).ToList();
        // Ek kart — ev sahipleri için (kiracısı olan dairelerde)
        foreach (var u in occupiedUnits.Where(u => u.TenantUserId.HasValue))
            allResidents.Add(MakeAccessCard(siteId, u.OwnerUserId!.Value, u.Id, $"GS-{allResidents.Count + 1:D4}"));

        db.AccessCards.AddRange(allResidents);

        // Ödemeler — son 3 aya ait aidat
        var payments = new List<Payment>();
        var now = DateTime.UtcNow;
        foreach (var u in occupiedUnits)
        {
            for (var m = -2; m <= 0; m++)
            {
                var due = new DateTime(now.Year, now.Month, 1).AddMonths(m);
                var isPaid = m < 0;
                payments.Add(new Payment
                {
                    SiteId = siteId,
                    UnitId = u.Id,
                    Amount = u.MonthlyFee ?? 2000m,
                    DueDate = due,
                    PaidDate = isPaid ? due.AddDays(5) : null,
                    Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Pending,
                    Description = $"{due:MMMM yyyy} Aidatı",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        db.Payments.AddRange(payments);
        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Site 2 — Mavi Köy Sitesi
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedSite2SharedAsync(
        SharedTenantDbContext db, Guid siteId, User[] owners, User[] tenants)
    {
        var ut1p1 = MakeUnitType(siteId, "1+1");
        var ut2p1 = MakeUnitType(siteId, "2+1");
        var ut3p1 = MakeUnitType(siteId, "3+1");
        var studio = MakeUnitType(siteId, "Stüdyo");
        db.UnitTypes.AddRange(ut1p1, ut2p1, ut3p1, studio);
        await db.SaveChangesAsync();

        var bina1 = MakeBuilding(siteId, "Bina 1", 5);
        var bina2 = MakeBuilding(siteId, "Bina 2", 5);
        var bina3 = MakeBuilding(siteId, "Bina 3", 5);
        db.Buildings.AddRange(bina1, bina2, bina3);
        await db.SaveChangesAsync();

        // Daireler
        // owners: 0=Tamer(Dolu) 1=Nalan(Kiralik) 2=Rıfat(Dolu) 3=Dilek(Kiralik) 4=Okan(Kiralik)
        //         5=Bahar(Dolu) 6=Gökhan(Kiralik) 7=İpek(Dolu) 8=Mert(Kiralik) 9=Tuğba(Dolu)
        //         10=Cenk(Dolu) 11=Aylin(Kiralik) 12=Erdem(Kiralik) 13=Seda(Dolu) 14=Ufuk(Kiralik)
        // tenants: 0=Buse 1=Tolga 2=Pelin 3=Kadir 4=Gamze 5=Levent 6=Suna 7=Hüseyin
        var units = new List<Unit>
        {
            // Bina 1
            MakeUnit(siteId, bina1.Id, ut3p1.Id, "1", "1", 140m, 85m,  2700m, UnitStatus.Dolu,    owners[0].Id, null),
            MakeUnit(siteId, bina1.Id, ut2p1.Id, "2", "1", 110m, 65m,  2100m, UnitStatus.Kiralik,  owners[1].Id, tenants[0].Id),
            MakeUnit(siteId, bina1.Id, ut2p1.Id, "3", "2", 112m, 66m,  2100m, UnitStatus.Dolu,    owners[2].Id, null),
            MakeUnit(siteId, bina1.Id, ut1p1.Id, "4", "2",  72m, 52m,  1400m, UnitStatus.Kiralik,  owners[3].Id, tenants[1].Id),
            MakeUnit(siteId, bina1.Id, ut2p1.Id, "5", "3", 115m, 68m,  2100m, UnitStatus.Kiralik,  owners[4].Id, tenants[2].Id),
            MakeUnit(siteId, bina1.Id, studio.Id, "6", "3",  60m, 45m,  1200m, UnitStatus.Bos,    null,        null),
            MakeUnit(siteId, bina1.Id, ut1p1.Id, "7", "4",  73m, 53m,  1400m, UnitStatus.Bos,     null,        null),

            // Bina 2
            MakeUnit(siteId, bina2.Id, ut2p1.Id, "1", "1", 113m, 67m,  2100m, UnitStatus.Dolu,    owners[5].Id, null),
            MakeUnit(siteId, bina2.Id, ut2p1.Id, "2", "1", 116m, 69m,  2100m, UnitStatus.Kiralik,  owners[6].Id, tenants[3].Id),
            MakeUnit(siteId, bina2.Id, ut3p1.Id, "3", "2", 145m, 88m,  2800m, UnitStatus.Dolu,    owners[7].Id, null),
            MakeUnit(siteId, bina2.Id, ut1p1.Id, "4", "2",  74m, 54m,  1400m, UnitStatus.Kiralik,  owners[8].Id, tenants[4].Id),
            MakeUnit(siteId, bina2.Id, ut2p1.Id, "5", "3", 114m, 67m,  2100m, UnitStatus.Dolu,    owners[9].Id, null),
            MakeUnit(siteId, bina2.Id, studio.Id, "6", "3",  62m, 46m,  1200m, UnitStatus.Bos,    null,        null),
            MakeUnit(siteId, bina2.Id, ut1p1.Id, "7", "4",  72m, 52m,  1400m, UnitStatus.Bos,     null,        null),

            // Bina 3
            MakeUnit(siteId, bina3.Id, ut2p1.Id, "1", "1", 115m, 68m,  2100m, UnitStatus.Dolu,    owners[10].Id, null),
            MakeUnit(siteId, bina3.Id, ut2p1.Id, "2", "1", 118m, 70m,  2100m, UnitStatus.Kiralik,  owners[11].Id, tenants[5].Id),
            MakeUnit(siteId, bina3.Id, ut1p1.Id, "3", "2",  75m, 55m,  1400m, UnitStatus.Kiralik,  owners[12].Id, tenants[6].Id),
            MakeUnit(siteId, bina3.Id, ut3p1.Id, "4", "2", 148m, 90m,  2800m, UnitStatus.Dolu,    owners[13].Id, null),
            MakeUnit(siteId, bina3.Id, ut2p1.Id, "5", "3", 116m, 68m,  2100m, UnitStatus.Kiralik,  owners[14].Id, tenants[7].Id),
            MakeUnit(siteId, bina3.Id, studio.Id, "6", "3",  61m, 45m,  1200m, UnitStatus.Bos,    null,        null),
            MakeUnit(siteId, bina3.Id, ut1p1.Id, "7", "4",  73m, 53m,  1400m, UnitStatus.Bos,     null,        null),
        };
        db.Units.AddRange(units);
        await db.SaveChangesAsync();

        var occupiedUnits = units.Where(u => u.Status != UnitStatus.Bos).ToList();

        var vehicles = new[]
        {
            MakeVehicle(siteId, occupiedUnits[0].Id,  owners[0].Id,  "34 MK 001", "Volkswagen", "Passat",   "Gümüş",   2021),
            MakeVehicle(siteId, occupiedUnits[1].Id,  tenants[0].Id, "34 MK 002", "Renault",    "Megane",   "Beyaz",   2020),
            MakeVehicle(siteId, occupiedUnits[2].Id,  owners[2].Id,  "34 MK 003", "Ford",       "Mondeo",   "Gri",     2022),
            MakeVehicle(siteId, occupiedUnits[3].Id,  tenants[1].Id, "34 MK 004", "Toyota",     "Yaris",    "Mavi",    2019),
            MakeVehicle(siteId, occupiedUnits[4].Id,  tenants[2].Id, "34 MK 005", "Honda",      "Jazz",     "Kırmızı", 2021),
            MakeVehicle(siteId, occupiedUnits[5].Id,  owners[5].Id,  "34 MK 006", "BMW",        "X3",       "Siyah",   2022),
            MakeVehicle(siteId, occupiedUnits[6].Id,  tenants[3].Id, "34 MK 007", "Hyundai",    "Tucson",   "Beyaz",   2023),
            MakeVehicle(siteId, occupiedUnits[7].Id,  owners[7].Id,  "34 MK 008", "Mercedes",   "E200",     "Gümüş",   2020),
            MakeVehicle(siteId, occupiedUnits[8].Id,  tenants[4].Id, "34 MK 009", "Peugeot",    "3008",     "Gri",     2021),
            MakeVehicle(siteId, occupiedUnits[9].Id,  owners[9].Id,  "34 MK 010", "Audi",       "Q3",       "Beyaz",   2022),
            MakeVehicle(siteId, occupiedUnits[10].Id, owners[10].Id, "34 MK 011", "Kia",        "Sportage", "Mavi",    2023),
            MakeVehicle(siteId, occupiedUnits[11].Id, tenants[5].Id, "34 MK 012", "Nissan",     "Qashqai",  "Siyah",   2020),
            MakeVehicle(siteId, occupiedUnits[12].Id, tenants[6].Id, "34 MK 013", "Skoda",      "Octavia",  "Gri",     2021),
            MakeVehicle(siteId, occupiedUnits[13].Id, owners[13].Id, "34 MK 014", "Volvo",      "V60",      "Beyaz",   2022),
            MakeVehicle(siteId, occupiedUnits[14].Id, tenants[7].Id, "34 MK 015", "Fiat",       "500",      "Sarı",    2023),
        };
        db.Vehicles.AddRange(vehicles);

        var cards = occupiedUnits.Select((u, i) =>
        {
            var userId = u.TenantUserId ?? u.OwnerUserId!.Value;
            return MakeAccessCard(siteId, userId, u.Id, $"MK-{i + 1:D4}");
        }).ToList();
        foreach (var u in occupiedUnits.Where(u => u.TenantUserId.HasValue))
            cards.Add(MakeAccessCard(siteId, u.OwnerUserId!.Value, u.Id, $"MK-{cards.Count + 1:D4}"));

        db.AccessCards.AddRange(cards);

        var payments = new List<Payment>();
        var now = DateTime.UtcNow;
        foreach (var u in occupiedUnits)
        {
            for (var m = -2; m <= 0; m++)
            {
                var due = new DateTime(now.Year, now.Month, 1).AddMonths(m);
                var isPaid = m < 0;
                payments.Add(new Payment
                {
                    SiteId = siteId,
                    UnitId = u.Id,
                    Amount = u.MonthlyFee ?? 2100m,
                    DueDate = due,
                    PaidDate = isPaid ? due.AddDays(7) : null,
                    Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Pending,
                    Description = $"{due:MMMM yyyy} Aidatı",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        db.Payments.AddRange(payments);
        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Muhasebe — cari hesaplar + parametreler
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedMuhasebeExtrasAsync(
        SharedTenantDbContext db, Guid siteId, User[] owners, User[] tenants)
    {
        // Ana hesapları bul
        var h100 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "100");
        var h102 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "102");
        var h120 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "120");
        var h600 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "600");
        var h642 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "642");

        // Cari hesap eklenince 120 artık yaprak hesap değil
        if (h120 is not null)
        {
            h120.FisKesilebilirMi = false;
            h120.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();

        // Kiracı cari hesapları (120.01.NNNN)
        var k = 1;
        foreach (var u in tenants)
        {
            db.HesapPlani.Add(new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = $"120.01.{k:D4}",
                HesapAdi = $"{u.FirstName} {u.LastName} (Kiracı)",
                HesapTipi = HesapTipi.Cari,
                HesapKategorisi = HesapKategorisi.Aktif,
                NormalBakiye = NormalBakiye.Borc,
                Seviye = 3,
                ParentId = h120?.Id,
                FisKesilebilirMi = true,
                CariTuru = CariTuru.Kiraci,
                PersonId = u.Id,
                AktifMi = true
            });
            k++;
        }

        // Ev sahibi cari hesapları (120.02.NNNN)
        var e = 1;
        foreach (var u in owners)
        {
            db.HesapPlani.Add(new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = $"120.02.{e:D4}",
                HesapAdi = $"{u.FirstName} {u.LastName} (Ev Sahibi)",
                HesapTipi = HesapTipi.Cari,
                HesapKategorisi = HesapKategorisi.Aktif,
                NormalBakiye = NormalBakiye.Borc,
                Seviye = 3,
                ParentId = h120?.Id,
                FisKesilebilirMi = true,
                CariTuru = CariTuru.EvSahibi,
                PersonId = u.Id,
                AktifMi = true
            });
            e++;
        }

        // MuhasebeParametre
        var parametreVar = await db.MuhasebeParametreler.IgnoreQueryFilters().AnyAsync(p => p.SiteId == siteId);
        if (!parametreVar)
        {
            db.MuhasebeParametreler.Add(new MuhasebeParametre
            {
                SiteId = siteId,
                VarsayilanKasaHesapId = h100?.Id,
                VarsayilanBankaHesapId = h102?.Id,
                AidatGelirHesapId = h600?.Id,
                GecikmeFaiziHesapId = h642?.Id,
                AlicilarAnaHesapKodu = "120",
                SaticilarAnaHesapKodu = "320",
                GiderAnaHesapKodu = "770",
                CariKodSablonu = "{ana}.{tur}.{sira:0000}",
                FisNoSablonu = "{yil}-{sira:0000000}",
                ParaBirimi = "TRY",
                KdvOrani = 20m,
                OtomatikTahsilFisi = false,
                OtomatikTediyeFisi = false
            });
        }

        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RolePermissions
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedRolePermissionsAsync(MasterDbContext masterDb, Guid roleTypeId)
    {
        var pageIds = await masterDb.Pages.Select(p => p.Id).ToListAsync();

        foreach (var pageId in pageIds)
        {
            var exists = await masterDb.RolePermissions
                .AnyAsync(rp => rp.RoleTypeId == roleTypeId && rp.PageId == pageId);
            if (exists) continue;

            masterDb.RolePermissions.Add(new RolePermission
            {
                RoleTypeId = roleTypeId,
                PageId = pageId,
                PermissionLevel = PermissionLevel.FullAccess,
                CreatedAt = DateTime.UtcNow
            });
        }

        await masterDb.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Yardımcı factory metodları
    // ─────────────────────────────────────────────────────────────────────────

    private static Site MakeSite(string name, string address, string district, string city,
        string postal, string phone, string email, string taxOffice, string taxNo) => new()
    {
        Name = name, Address = address, District = district, City = city,
        PostalCode = postal, Phone = phone, Email = email,
        TaxOffice = taxOffice, TaxNumber = taxNo,
        DbMode = DbMode.Shared, IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static User MakeUser(string first, string last, string email, string phone) => new()
    {
        FirstName = first, LastName = last, Email = email,
        PasswordHash = DemoPasswordHash, PhoneNumber = phone,
        IsActive = true, MustChangePassword = false, CreatedAt = DateTime.UtcNow
    };

    private static UserSite MakeUserSite(Guid userId, Guid siteId, UserType type, Guid? roleId = null) => new()
    {
        UserId = userId, SiteId = siteId, UserType = type,
        RoleTypeId = roleId, Status = UserSiteStatus.Approved, CreatedAt = DateTime.UtcNow
    };

    private static SiteSubscription MakeSub(Guid siteId, Guid planId) => new()
    {
        SiteId = siteId, SubscriptionPlanId = planId,
        StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(10),
        IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static UnitType MakeUnitType(Guid siteId, string name) => new()
    {
        SiteId = siteId, Name = name, IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static Building MakeBuilding(Guid siteId, string name, int floors) => new()
    {
        SiteId = siteId, Name = name, TotalFloors = floors, IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static Unit MakeUnit(
        Guid siteId, Guid buildingId, Guid unitTypeId,
        string doorNumber, string floor,
        decimal grossArea, decimal netArea, decimal monthlyFee,
        UnitStatus status, Guid? ownerUserId, Guid? tenantUserId) => new()
    {
        SiteId = siteId,
        BuildingId = buildingId,
        UnitTypeId = unitTypeId,
        DoorNumber = doorNumber,
        Floor = floor,
        GrossArea = grossArea,
        NetArea = netArea,
        MonthlyFee = monthlyFee,
        Status = status,
        OwnerUserId = ownerUserId,
        TenantUserId = tenantUserId,
        HasDask = status != UnitStatus.Bos,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static Vehicle MakeVehicle(
        Guid siteId, Guid unitId, Guid ownerUserId,
        string plate, string brand, string model, string color, int year) => new()
    {
        SiteId = siteId,
        UnitId = unitId,
        OwnerUserId = ownerUserId,
        Plate = plate, Brand = brand, Model = model, Color = color, Year = year,
        IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static AccessCard MakeAccessCard(Guid siteId, Guid userId, Guid unitId, string cardNumber) => new()
    {
        SiteId = siteId,
        UserId = userId,
        UnitId = unitId,
        CardNumber = cardNumber,
        IsActive = true,
        IssueDate = DateTime.UtcNow.AddDays(-30),
        ExpiryDate = DateTime.UtcNow.AddYears(2),
        Notes = "Demo verisi"
    };
}
