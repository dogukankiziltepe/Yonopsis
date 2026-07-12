using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Seed;

/// <summary>
/// Demo/test data seeder.
/// Contains 2 sites, 3 blocks each, 21 units each, tenant/owner assignments,
/// vehicles, access cards and accounting definitions.
/// Idempotent: skipped if "Güneş Sitesi" already exists.
/// </summary>
public static class DemoDataSeeder
{
    private static readonly string DemoPasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo1234");

    public static async Task SeedAsync(MasterDbContext masterDb, SharedTenantDbContext sharedDb)
    {
        var existingSite1 = await masterDb.Sites.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Name == "Güneş Sitesi");
        if (existingSite1 is not null)
        {
            var existingSite2 = await masterDb.Sites.IgnoreQueryFilters().FirstAsync(s => s.Name == "Mavi Köy Sitesi");
            await SeedOzetDashboardDataAsync(masterDb, sharedDb, existingSite1.Id);
            await SeedOzetDashboardDataAsync(masterDb, sharedDb, existingSite2.Id);
            return;
        }

        var premiumPlan = await masterDb.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == "Premium");

        // ── Sites ────────────────────────────────────────────────────────────
        var site1 = MakeSite("Güneş Sitesi", "Moda Cad. No:45", "Kadıköy", "İstanbul",
            "34710", "0216 123 45 67", "info@gunessitesi.com", "Kadıköy V.D.", "1234567890");
        var site2 = MakeSite("Mavi Köy Sitesi", "Barbaros Blv. No:120", "Beşiktaş", "İstanbul",
            "34349", "0212 456 78 90", "info@mavikoysitesi.com", "Beşiktaş V.D.", "9876543210");
        masterDb.Sites.AddRange(site1, site2);

        // ── SiteAdmin Roles ───────────────────────────────────────────────────
        var role1 = new RoleType { SiteId = site1.Id, Name = "SiteAdmin", IsDefault = true, CreatedAt = DateTime.UtcNow };
        var role2 = new RoleType { SiteId = site2.Id, Name = "SiteAdmin", IsDefault = true, CreatedAt = DateTime.UtcNow };
        masterDb.RoleTypes.AddRange(role1, role2);

        // ── Managers ─────────────────────────────────────────────────────────
        var mgmt1 = MakeUser("Ahmet", "Yıldız", "admin@gunessitesi.com", "0555 111 00 01");
        var mgmt2 = MakeUser("Zeynep", "Çelik", "admin@mavikoysitesi.com", "0555 222 00 01");
        masterDb.Users.AddRange(mgmt1, mgmt2);
        masterDb.UserSites.Add(MakeUserSite(mgmt1.Id, site1.Id, UserType.Management, role1.Id));
        masterDb.UserSites.Add(MakeUserSite(mgmt2.Id, site2.Id, UserType.Management, role2.Id));

        // ── Subscriptions ────────────────────────────────────────────────────
        if (premiumPlan is not null)
        {
            masterDb.SiteSubscriptions.AddRange(
                MakeSub(site1.Id, premiumPlan.Id),
                MakeSub(site2.Id, premiumPlan.Id));
        }

        // ── People — Site 1 ──────────────────────────────────────────────────
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
        var s1OwnerSites = s1Owners.Select(u => MakeUserSite(u.Id, site1.Id, UserType.Owner)).ToList();
        var s1TenantSites = s1Tenants.Select(u => MakeUserSite(u.Id, site1.Id, UserType.Renter)).ToList();
        masterDb.UserSites.AddRange(s1OwnerSites);
        masterDb.UserSites.AddRange(s1TenantSites);
        EnrichPersonDetails(s1Owners[0], s1OwnerSites[0], Gender.Male, EducationStatus.Lisans, "Marmara Üniversitesi", "Mühendis",
            Nationality.TC, "İstanbul", new DateTime(1985, 4, 12), MaritalStatus.Evli, "Ahmet", "Sevgi",
            "Kadıköy V.D.", "Moda Cad. No:12 Kadıköy/İstanbul");
        EnrichPersonDetails(s1Tenants[0], s1TenantSites[0], Gender.Female, EducationStatus.YuksekLisans, "Boğaziçi Üniversitesi", "Öğretmen",
            Nationality.TC, "Ankara", new DateTime(1990, 8, 23), MaritalStatus.Bekar, "Hasan", "Nur",
            "Kadıköy V.D.", "Bahariye Cad. No:7 Kadıköy/İstanbul");
        masterDb.PersonPhones.AddRange(
            new PersonPhone { UserSiteId = s1OwnerSites[0].Id, PhoneNumber = "0532 101 0001", Label = "Cep" },
            new PersonPhone { UserSiteId = s1OwnerSites[0].Id, PhoneNumber = "0216 555 44 33", Label = "İş" },
            new PersonPhone { UserSiteId = s1TenantSites[0].Id, PhoneNumber = "0543 201 0001", Label = "Cep" });

        // ── People — Site 2 ──────────────────────────────────────────────────
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
        var s2OwnerSites = s2Owners.Select(u => MakeUserSite(u.Id, site2.Id, UserType.Owner)).ToList();
        var s2TenantSites = s2Tenants.Select(u => MakeUserSite(u.Id, site2.Id, UserType.Renter)).ToList();
        masterDb.UserSites.AddRange(s2OwnerSites);
        masterDb.UserSites.AddRange(s2TenantSites);
        EnrichPersonDetails(s2Owners[0], s2OwnerSites[0], Gender.Male, EducationStatus.Lise, null, "Esnaf",
            Nationality.TC, "Trabzon", new DateTime(1978, 11, 2), MaritalStatus.Evli, "İsmail", "Fatma",
            "Beşiktaş V.D.", "Barbaros Blv. No:8 Beşiktaş/İstanbul");
        EnrichPersonDetails(s2Tenants[0], s2TenantSites[0], Gender.Female, EducationStatus.OnLisans, "Anadolu Üniversitesi", "Muhasebeci",
            Nationality.Yabanci, "Baku", new DateTime(1993, 2, 17), MaritalStatus.Bekar, "Elvin", "Gunay",
            "Beşiktaş V.D.", "Levent Cad. No:3 Beşiktaş/İstanbul");
        masterDb.PersonPhones.AddRange(
            new PersonPhone { UserSiteId = s2OwnerSites[0].Id, PhoneNumber = "0532 202 0001", Label = "Cep" },
            new PersonPhone { UserSiteId = s2TenantSites[0].Id, PhoneNumber = "0543 302 0001", Label = "Cep" });

        await masterDb.SaveChangesAsync();

        // ── RolePermissions — SiteAdmin roles get FullAccess to all pages ────
        // GetUserPagesQuery reads RolePermissions directly for the sidebar;
        // PermissionService's IsDefault shortcut is only for endpoint filtering.
        await SeedRolePermissionsAsync(masterDb, role1.Id);
        await SeedRolePermissionsAsync(masterDb, role2.Id);

        // ── SharedDB data ────────────────────────────────────────────────────
        await SeedSite1SharedAsync(sharedDb, site1.Id, s1Owners, s1Tenants);
        await SeedSite2SharedAsync(sharedDb, site2.Id, s2Owners, s2Tenants);

        // ── İletişim Geçmişi tabı — demo log kayıtları ───────────────────────
        await SeedContactHistoryAsync(sharedDb, site1.Id, s1Owners[0], s1Tenants[0]);
        await SeedContactHistoryAsync(sharedDb, site2.Id, s2Owners[0], s2Tenants[0]);

        // ── Accounting ───────────────────────────────────────────────────────
        await MuhasebeSeeder.SeedForSiteAsync(sharedDb, site1.Id);
        await MuhasebeSeeder.SeedForSiteAsync(sharedDb, site2.Id);

        await SeedMuhasebeExtrasAsync(sharedDb, site1.Id, s1Owners, s1Tenants);
        await SeedMuhasebeExtrasAsync(sharedDb, site2.Id, s2Owners, s2Tenants);

        // ── Özet dashboard demo data ────────────────────────────────────────
        await SeedOzetDashboardDataAsync(masterDb, sharedDb, site1.Id);
        await SeedOzetDashboardDataAsync(masterDb, sharedDb, site2.Id);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Site 1 — Güneş Sitesi (Sun Site)
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedSite1SharedAsync(
        SharedTenantDbContext db, Guid siteId, User[] owners, User[] tenants)
    {
        // Unit types
        var ut1p1 = MakeUnitType(siteId, "1+1");
        var ut2p1 = MakeUnitType(siteId, "2+1");
        var ut3p1 = MakeUnitType(siteId, "3+1");
        db.UnitTypes.AddRange(ut1p1, ut2p1, ut3p1);
        await db.SaveChangesAsync();

        // Blocks
        var blokA = MakeBuilding(siteId, "A Blok", 7);
        var blokB = MakeBuilding(siteId, "B Blok", 7);
        var blokC = MakeBuilding(siteId, "C Blok", 7);
        db.Buildings.AddRange(blokA, blokB, blokC);
        await db.SaveChangesAsync();

        // Units — Block A
        // owners: 0=Mehmet(Occupied) 1=Ayşe(Rented) 2=Fatma(Rented) 3=Ali(Occupied) 4=Zeynep(Rented)
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

            // Block B
            // owners: 5=Hasan(Occupied) 6=Elif(Rented) 7=Mustafa(Occupied) 8=Selin(Rented) 9=Kemal(Occupied)
            // tenants: 3=Deniz 4=Ece
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "1",  "1", 116m, 69m,  2200m, UnitStatus.Dolu,    owners[5].Id, null),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "2",  "1", 118m, 70m,  2200m, UnitStatus.Kiralik,  owners[6].Id, tenants[3].Id),
            MakeUnit(siteId, blokB.Id, ut3p1.Id, "3",  "2", 148m, 90m,  2900m, UnitStatus.Dolu,    owners[7].Id, null),
            MakeUnit(siteId, blokB.Id, ut1p1.Id, "4",  "2",  76m, 56m,  1500m, UnitStatus.Kiralik,  owners[8].Id, tenants[4].Id),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "5",  "3", 114m, 67m,  2200m, UnitStatus.Dolu,    owners[9].Id, null),
            MakeUnit(siteId, blokB.Id, ut1p1.Id, "6",  "3",  73m, 53m,  1500m, UnitStatus.Bos,     null,        null),
            MakeUnit(siteId, blokB.Id, ut2p1.Id, "7",  "4", 112m, 65m,  2200m, UnitStatus.Bos,     null,        null),

            // Block C
            // owners: 10=Burak(Occupied) 11=Merve(Rented) 12=Serkan(Rented) 13=Pınar(Rented) 14=Volkan(Occupied)
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

        // Daireler tabı — kişi/daire geçmişi (güncel atamalar için açık kayıt)
        db.PersonUnitHistories.AddRange(MakeUnitHistoryRows(siteId, units));
        // Örnek kapanmış geçmiş kaydı — daha önce başka bir kiracı yaşamış (Daireler tabı çeşitliliği)
        db.PersonUnitHistories.Add(new PersonUnitHistory
        {
            SiteId = siteId,
            UnitId = units[1].Id,
            PersonUserId = tenants[3].Id,
            Role = UserType.Renter,
            EntryDate = DateTime.UtcNow.AddYears(-2),
            ExitDate = DateTime.UtcNow.AddMonths(-7),
            ContactPerson = "Yönetim Ofisi",
            Notes = "Kira sözleşmesi sona erdi.",
            BankPaymentCode = "BPK-GS-0002"
        });

        // Vehicles — one per occupied/rented unit
        var occupiedUnits = units.Where(u => u.Status != UnitStatus.Bos).ToList();
        var vehicles = new[]
        {
            MakeVehicle(siteId, occupiedUnits[0].Id,  owners[0].Id,  "34 ABC 123", "Toyota",     "Corolla",  "White",  2020),
            MakeVehicle(siteId, occupiedUnits[1].Id,  tenants[0].Id, "34 DEF 456", "Honda",      "Civic",    "Gray",   2021),
            MakeVehicle(siteId, occupiedUnits[2].Id,  tenants[1].Id, "34 GHI 789", "Volkswagen", "Polo",     "Black",  2019),
            MakeVehicle(siteId, occupiedUnits[3].Id,  owners[3].Id,  "34 JKL 012", "Renault",    "Clio",     "Blue",   2022),
            MakeVehicle(siteId, occupiedUnits[4].Id,  tenants[2].Id, "34 MNO 345", "Ford",       "Focus",    "Red",    2018),
            MakeVehicle(siteId, occupiedUnits[5].Id,  owners[5].Id,  "34 PQR 678", "Hyundai",    "i20",      "White",  2023),
            MakeVehicle(siteId, occupiedUnits[6].Id,  tenants[3].Id, "34 STU 901", "Kia",        "Picanto",  "Silver", 2020),
            MakeVehicle(siteId, occupiedUnits[7].Id,  owners[7].Id,  "34 VWX 234", "BMW",        "3 Series", "Black",  2021),
            MakeVehicle(siteId, occupiedUnits[8].Id,  tenants[4].Id, "34 YZA 567", "Mercedes",   "C200",     "White",  2022),
            MakeVehicle(siteId, occupiedUnits[9].Id,  owners[9].Id,  "34 BCD 890", "Fiat",       "Egea",     "Gray",   2019),
            MakeVehicle(siteId, occupiedUnits[10].Id, owners[10].Id, "34 EFG 123", "Audi",       "A3",       "Blue",   2023),
            MakeVehicle(siteId, occupiedUnits[11].Id, tenants[5].Id, "34 HIJ 456", "Peugeot",    "208",      "Red",    2020),
            MakeVehicle(siteId, occupiedUnits[12].Id, tenants[6].Id, "34 KLM 789", "Nissan",     "Micra",    "Yellow", 2021),
            MakeVehicle(siteId, occupiedUnits[13].Id, tenants[7].Id, "34 NOP 012", "Skoda",      "Fabia",    "Silver", 2022),
            MakeVehicle(siteId, occupiedUnits[14].Id, owners[14].Id, "34 QRS 345", "Volvo",      "XC40",     "White",  2023),
        };
        db.Vehicles.AddRange(vehicles);

        // Access Cards
        var allResidents = occupiedUnits.Select((u, i) =>
        {
            var userId = u.TenantUserId ?? u.OwnerUserId!.Value;
            return MakeAccessCard(siteId, userId, u.Id, $"GS-{i + 1:D4}");
        }).ToList();
        // Extra card — for owners of rented units
        foreach (var u in occupiedUnits.Where(u => u.TenantUserId.HasValue))
            allResidents.Add(MakeAccessCard(siteId, u.OwnerUserId!.Value, u.Id, $"GS-{allResidents.Count + 1:D4}"));

        db.AccessCards.AddRange(allResidents);

        // Payments — dues for the last 3 months
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
                    Description = $"{due:MMMM yyyy} Dues",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        db.Payments.AddRange(payments);
        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Site 2 — Mavi Köy Sitesi (Blue Village Site)
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

        // Units
        // owners: 0=Tamer(Occupied) 1=Nalan(Rented) 2=Rıfat(Occupied) 3=Dilek(Rented) 4=Okan(Rented)
        //         5=Bahar(Occupied) 6=Gökhan(Rented) 7=İpek(Occupied) 8=Mert(Rented) 9=Tuğba(Occupied)
        //         10=Cenk(Occupied) 11=Aylin(Rented) 12=Erdem(Rented) 13=Seda(Occupied) 14=Ufuk(Rented)
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

        // Daireler tabı — kişi/daire geçmişi (güncel atamalar için açık kayıt)
        db.PersonUnitHistories.AddRange(MakeUnitHistoryRows(siteId, units));
        db.PersonUnitHistories.Add(new PersonUnitHistory
        {
            SiteId = siteId,
            UnitId = units[1].Id,
            PersonUserId = tenants[3].Id,
            Role = UserType.Renter,
            EntryDate = DateTime.UtcNow.AddYears(-2),
            ExitDate = DateTime.UtcNow.AddMonths(-9),
            ContactPerson = "Yönetim Ofisi",
            Notes = "Kira sözleşmesi sona erdi.",
            BankPaymentCode = "BPK-MK-0002"
        });

        var occupiedUnits = units.Where(u => u.Status != UnitStatus.Bos).ToList();

        var vehicles = new[]
        {
            MakeVehicle(siteId, occupiedUnits[0].Id,  owners[0].Id,  "34 MK 001", "Volkswagen", "Passat",   "Silver", 2021),
            MakeVehicle(siteId, occupiedUnits[1].Id,  tenants[0].Id, "34 MK 002", "Renault",    "Megane",   "White",  2020),
            MakeVehicle(siteId, occupiedUnits[2].Id,  owners[2].Id,  "34 MK 003", "Ford",       "Mondeo",   "Gray",   2022),
            MakeVehicle(siteId, occupiedUnits[3].Id,  tenants[1].Id, "34 MK 004", "Toyota",     "Yaris",    "Blue",   2019),
            MakeVehicle(siteId, occupiedUnits[4].Id,  tenants[2].Id, "34 MK 005", "Honda",      "Jazz",     "Red",    2021),
            MakeVehicle(siteId, occupiedUnits[5].Id,  owners[5].Id,  "34 MK 006", "BMW",        "X3",       "Black",  2022),
            MakeVehicle(siteId, occupiedUnits[6].Id,  tenants[3].Id, "34 MK 007", "Hyundai",    "Tucson",   "White",  2023),
            MakeVehicle(siteId, occupiedUnits[7].Id,  owners[7].Id,  "34 MK 008", "Mercedes",   "E200",     "Silver", 2020),
            MakeVehicle(siteId, occupiedUnits[8].Id,  tenants[4].Id, "34 MK 009", "Peugeot",    "3008",     "Gray",   2021),
            MakeVehicle(siteId, occupiedUnits[9].Id,  owners[9].Id,  "34 MK 010", "Audi",       "Q3",       "White",  2022),
            MakeVehicle(siteId, occupiedUnits[10].Id, owners[10].Id, "34 MK 011", "Kia",        "Sportage", "Blue",   2023),
            MakeVehicle(siteId, occupiedUnits[11].Id, tenants[5].Id, "34 MK 012", "Nissan",     "Qashqai",  "Black",  2020),
            MakeVehicle(siteId, occupiedUnits[12].Id, tenants[6].Id, "34 MK 013", "Skoda",      "Octavia",  "Gray",   2021),
            MakeVehicle(siteId, occupiedUnits[13].Id, owners[13].Id, "34 MK 014", "Volvo",      "V60",      "White",  2022),
            MakeVehicle(siteId, occupiedUnits[14].Id, tenants[7].Id, "34 MK 015", "Fiat",       "500",      "Yellow", 2023),
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
                    Description = $"{due:MMMM yyyy} Dues",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        db.Payments.AddRange(payments);
        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // İletişim Geçmişi — demo Email/SMS/WhatsApp/Mobil Bildirim log kayıtları
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedContactHistoryAsync(SharedTenantDbContext db, Guid siteId, params User[] persons)
    {
        var now = DateTime.UtcNow;
        foreach (var person in persons)
        {
            var fullName = $"{person.FirstName} {person.LastName}";

            db.EmailLogs.AddRange(
                new EmailLog
                {
                    SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-2), DeliveredAt = now.AddDays(-2).AddMinutes(1),
                    ReadAt = now.AddDays(-2).AddHours(3), RecipientEmail = person.Email, Subject = "Aidat Hatırlatması",
                    Body = $"Sayın {fullName}, bu ayki aidat ödemenizin son tarihi yaklaşmaktadır.", Status = "Okundu"
                },
                new EmailLog
                {
                    SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-10), DeliveredAt = now.AddDays(-10).AddMinutes(2),
                    ReadAt = null, RecipientEmail = person.Email, Subject = "Su Kesintisi Bildirimi",
                    Body = "Yarın 09:00-13:00 arası planlı su kesintisi yapılacaktır.", Status = "İletildi"
                });

            db.SmsLogs.AddRange(
                new SmsLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-2), PhoneNumber = person.PhoneNumber ?? "-", Message = "Aidat ödemenizin son tarihi yaklaşıyor.", Status = "Teslim Edildi" },
                new SmsLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-15), PhoneNumber = person.PhoneNumber ?? "-", Message = "Genel kurul toplantımız önümüzdeki hafta yapılacaktır.", Status = "Teslim Edildi" });

            db.WhatsappLogs.AddRange(
                new WhatsappLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-5), PhoneNumber = person.PhoneNumber ?? "-", Message = "Asansör bakımı nedeniyle kısa süreli kesinti olacaktır.", Status = "Okundu" },
                new WhatsappLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-20), PhoneNumber = person.PhoneNumber ?? "-", Message = "Bahçe düzenleme çalışmaları başlıyor.", Status = "Teslim Edildi" });

            db.MobilBildirimLogs.AddRange(
                new MobilBildirimLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-1), Message = "Yeni duyuru: Ortak alan kullanım kuralları güncellendi.", Status = "Gönderildi" },
                new MobilBildirimLog { SiteId = siteId, UserId = person.Id, SentAt = now.AddDays(-8), Message = "Aidat ödemeniz alınmıştır, teşekkür ederiz.", Status = "Gönderildi" });
        }

        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Accounting — receivable accounts + parameters
    // ─────────────────────────────────────────────────────────────────────────

    private static async Task SeedMuhasebeExtrasAsync(
        SharedTenantDbContext db, Guid siteId, User[] owners, User[] tenants)
    {
        // Find master accounts
        var h100 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "100");
        var h102 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "102");
        var h120 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "120");
        var h600 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "600");
        var h642 = await db.HesapPlani.IgnoreQueryFilters().FirstOrDefaultAsync(h => h.SiteId == siteId && h.HesapKodu == "642");

        // Account 120 is no longer a leaf account once sub-accounts are added
        if (h120 is not null)
        {
            h120.FisKesilebilirMi = false;
            h120.UpdatedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();

        // Tenant receivable accounts (120.01.NNNN)
        var k = 1;
        foreach (var u in tenants)
        {
            db.HesapPlani.Add(new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = $"120.01.{k:D4}",
                HesapAdi = $"{u.FirstName} {u.LastName} (Tenant)",
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

        // Owner receivable accounts (120.02.NNNN)
        var e = 1;
        foreach (var u in owners)
        {
            db.HesapPlani.Add(new HesapPlani
            {
                SiteId = siteId,
                HesapKodu = $"120.02.{e:D4}",
                HesapAdi = $"{u.FirstName} {u.LastName} (Owner)",
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

        // Accounting parameters
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
    // Özet dashboard — Kasalar, İş Takibi, Aidat, Finansal Durum, Evraklar,
    // Ödenecek Faturalar, Gelir/Gider Dağılımı, Duyurular
    // ─────────────────────────────────────────────────────────────────────────

    private const string OzetDemoMarker = "Demo verisi (Özet)";

    private static async Task SeedOzetDashboardDataAsync(MasterDbContext masterDb, SharedTenantDbContext db, Guid siteId)
    {
        if (await db.KasaBanka.IgnoreQueryFilters().AnyAsync(k => k.SiteId == siteId && k.Name == "Ana Kasa"))
            return;

        var units = await db.Units.IgnoreQueryFilters()
            .Where(u => u.SiteId == siteId && u.Status != UnitStatus.Bos)
            .ToListAsync();
        if (units.Count == 0) return;

        var managerUserId = await masterDb.UserSites
            .Where(us => us.SiteId == siteId && us.UserType == UserType.Management)
            .Select(us => us.UserId)
            .FirstOrDefaultAsync();

        var now = DateTime.UtcNow;

        // ── Gelir/Gider tanımları ────────────────────────────────────────────
        var aidatGrubu = MakeGelirGrubu(siteId, "Aidat Gelirleri", 1);
        var digerGelirGrubu = MakeGelirGrubu(siteId, "Diğer Gelirler", 2);
        db.GelirGruplari.AddRange(aidatGrubu, digerGelirGrubu);

        var personelGrubu = MakeGiderGrubu(siteId, "Personel Giderleri", 1);
        var bakimGrubu = MakeGiderGrubu(siteId, "Bakım Onarım", 2);
        var faturaGrubu = MakeGiderGrubu(siteId, "Faturalar", 3);
        db.GiderGruplari.AddRange(personelGrubu, bakimGrubu, faturaGrubu);
        await db.SaveChangesAsync();

        var aidatTanimi = MakeGelirTanimi(siteId, "Aidat", aidatGrubu.Id, 1);
        var gecikmeTanimi = MakeGelirTanimi(siteId, "Gecikme Faizi", aidatGrubu.Id, 2);
        var kiraGeliriTanimi = MakeGelirTanimi(siteId, "Kira Geliri", digerGelirGrubu.Id, 1);
        var ortakAlanGeliriTanimi = MakeGelirTanimi(siteId, "Ortak Alan Geliri", digerGelirGrubu.Id, 2);
        db.GelirTanimlari.AddRange(aidatTanimi, gecikmeTanimi, kiraGeliriTanimi, ortakAlanGeliriTanimi);

        var maasTanimi = MakeGiderTanimi(siteId, "Maaş", personelGrubu.Id, 1);
        var sgkTanimi = MakeGiderTanimi(siteId, "SGK Primi", personelGrubu.Id, 2);
        var asansorTanimi = MakeGiderTanimi(siteId, "Asansör Bakımı", bakimGrubu.Id, 1);
        var temizlikTanimi = MakeGiderTanimi(siteId, "Temizlik", bakimGrubu.Id, 2);
        var peyzajTanimi = MakeGiderTanimi(siteId, "Peyzaj", bakimGrubu.Id, 3);
        var elektrikTanimi = MakeGiderTanimi(siteId, "Elektrik", faturaGrubu.Id, 1);
        var suTanimi = MakeGiderTanimi(siteId, "Su", faturaGrubu.Id, 2);
        var dogalgazTanimi = MakeGiderTanimi(siteId, "Doğalgaz", faturaGrubu.Id, 3);
        db.GiderTanimlari.AddRange(maasTanimi, sgkTanimi, asansorTanimi, temizlikTanimi, peyzajTanimi, elektrikTanimi, suTanimi, dogalgazTanimi);
        await db.SaveChangesAsync();

        // ── Kasa/Banka ────────────────────────────────────────────────────────
        var anaKasa = MakeKasaBanka(siteId, "Ana Kasa", KasaBankaTipi.Kasa, null, null, null, null);
        var akbank = MakeKasaBanka(siteId, "Akbank Vadesiz Hesap", KasaBankaTipi.BankHesabi, "Akbank", "Kadıköy Şubesi", "1234567", "TR120004600123456789012345");
        var garanti = MakeKasaBanka(siteId, "Garanti BBVA Hesap", KasaBankaTipi.BankHesabi, "Garanti BBVA", "Beşiktaş Şubesi", "7654321", "TR330006200123456789012346");
        db.KasaBanka.AddRange(anaKasa, akbank, garanti);
        await db.SaveChangesAsync();
        var kasalar = new[] { anaKasa, akbank, garanti };

        // ── Banka/Kasa hareketleri — son 4 ay ───────────────────────────────────
        var hareketler = new List<BankaHareketi>();
        for (var m = -3; m <= 0; m++)
        {
            var ayBasi = new DateTime(now.Year, now.Month, 1).AddMonths(m);
            for (var i = 0; i < kasalar.Length; i++)
            {
                var kasa = kasalar[i];
                hareketler.Add(MakeBankaHareketi(siteId, kasa.Id, ayBasi.AddDays(3 + i), "Aidat tahsilatı", 15000m + i * 1000m));
                hareketler.Add(MakeBankaHareketi(siteId, kasa.Id, ayBasi.AddDays(10 + i), "Personel maaş ödemesi", -8000m - i * 500m));
                hareketler.Add(MakeBankaHareketi(siteId, kasa.Id, ayBasi.AddDays(15 + i), "Bakım onarım ödemesi", -2500m - i * 200m));
                hareketler.Add(MakeBankaHareketi(siteId, kasa.Id, ayBasi.AddDays(20 + i), "Fatura ödemesi", -1800m - i * 150m));
            }
        }
        db.BankaHareketleri.AddRange(hareketler);
        await db.SaveChangesAsync();

        // ── Aidat borç makbuzları — son 12 ay ───────────────────────────────────
        var borclar = new List<BorcMakbuzu>();
        var bEvrakNo = 1;
        var siteIdPrefix = siteId.ToString()[..4];
        for (var ui = 0; ui < units.Count; ui++)
        {
            var unit = units[ui];
            var tutar = unit.MonthlyFee ?? 2000m;
            for (var m = -11; m <= 0; m++)
            {
                var donemTarihi = new DateTime(now.Year, now.Month, 1).AddMonths(m);
                decimal odenen;
                if (m <= -2) odenen = tutar;
                else if (m == -1) odenen = (ui % 4 == 0) ? tutar * 0.5m : tutar;
                else odenen = (ui % 3 == 0) ? tutar : (ui % 3 == 1 ? tutar * 0.4m : 0m);

                borclar.Add(new BorcMakbuzu
                {
                    SiteId = siteId,
                    EvrakNo = $"OZT-BM-{siteIdPrefix}-{bEvrakNo:D5}",
                    IslemTarihi = donemTarihi,
                    Donem = donemTarihi.ToString("yyyy-MM"),
                    SonOdemeTarihi = donemTarihi.AddDays(10),
                    UnitId = unit.Id,
                    GelirTanimiId = aidatTanimi.Id,
                    Tutar = tutar,
                    GecikmeTutari = 0m,
                    OdenenTutar = odenen,
                    Aciklama = OzetDemoMarker,
                    CreatedAt = DateTime.UtcNow
                });
                bEvrakNo++;
            }
        }
        db.BorcMakbuzlari.AddRange(borclar);
        await db.SaveChangesAsync();

        // ── Tahsilat makbuzları ──────────────────────────────────────────────
        var tahsilatlar = new List<TahsilatMakbuzu>();
        var tEvrakNo = 1;
        for (var i = 0; i < borclar.Count; i++)
        {
            var borc = borclar[i];
            if (borc.OdenenTutar <= 0) continue;
            var kasa = (i % 2 == 0) ? akbank : garanti;
            tahsilatlar.Add(new TahsilatMakbuzu
            {
                SiteId = siteId,
                EvrakNo = $"OZT-TM-{siteIdPrefix}-{tEvrakNo:D5}",
                IslemTarihi = borc.IslemTarihi.AddDays(2),
                KasaBankaId = kasa.Id,
                BorcMakbuzuId = borc.Id,
                OdemeTutari = borc.OdenenTutar,
                OdemeTipi = (i % 2 == 0) ? OdemeTipi.HavaleEFT : OdemeTipi.Nakit,
                Aciklama = OzetDemoMarker,
                CreatedAt = DateTime.UtcNow
            });
            tEvrakNo++;
        }
        db.TahsilatMakbuzlari.AddRange(tahsilatlar);
        await db.SaveChangesAsync();

        // ── Faturalar — son 3 ay ─────────────────────────────────────────────
        var faturalar = new List<Fatura>();
        var gEvrakNo = 1;
        var giderTanimlari = new[] { maasTanimi, sgkTanimi, asansorTanimi, temizlikTanimi, peyzajTanimi, elektrikTanimi, suTanimi, dogalgazTanimi };
        for (var m = -2; m <= 0; m++)
        {
            var ayBasi = new DateTime(now.Year, now.Month, 1).AddMonths(m);

            faturalar.Add(MakeFatura(siteId, $"OZT-GF-{gEvrakNo++:D5}", FaturaTipi.Gelir, ayBasi.AddDays(5), "Ortak Alan Kiracısı", null, kiraGeliriTanimi.Id, 4500m, null, FaturaOdemeDurumu.Odendi));
            faturalar.Add(MakeFatura(siteId, $"OZT-GF-{gEvrakNo++:D5}", FaturaTipi.Gelir, ayBasi.AddDays(18), "Sosyal Tesis Geliri", null, ortakAlanGeliriTanimi.Id, 1200m, null, FaturaOdemeDurumu.Odendi));

            for (var gi = 0; gi < giderTanimlari.Length; gi++)
            {
                var tanim = giderTanimlari[gi];
                var isCurrentMonth = m == 0;
                var isUnpaidTail = isCurrentMonth && gi >= giderTanimlari.Length - 3;
                faturalar.Add(MakeFatura(
                    siteId, $"OZT-EF-{gEvrakNo++:D5}", FaturaTipi.Gider, ayBasi.AddDays(7 + gi),
                    $"{tanim.Name} Tedarikçisi", tanim.Id, null, 1200m + gi * 150m,
                    isUnpaidTail ? ayBasi.AddDays(25 + gi) : ayBasi.AddDays(15),
                    isUnpaidTail ? FaturaOdemeDurumu.Odenmedi : FaturaOdemeDurumu.Odendi));
            }
        }
        db.Faturalar.AddRange(faturalar);
        await db.SaveChangesAsync();

        // ── İş Emirleri ───────────────────────────────────────────────────────
        var personeller = new[] { "Ahmet Usta", "Mehmet Kapıcı", "Ayşe Temizlik" };
        var isEmirleri = new List<IsEmri>
        {
            MakeIsEmri(siteId, "Asansör arızası", "A Blok asansörü sesli çalışıyor.", IsEmriOncelik.Yuksek, IsEmriDurum.Devam, personeller[0]),
            MakeIsEmri(siteId, "B Blok su sızıntısı", "Bodrum katta su sızıntısı tespit edildi.", IsEmriOncelik.Kritik, IsEmriDurum.Atandi, personeller[0]),
            MakeIsEmri(siteId, "Ortak alan aydınlatma arızası", "Otopark aydınlatması yanmıyor.", IsEmriOncelik.Normal, IsEmriDurum.YeniTalep, null),
            MakeIsEmri(siteId, "Bahçe sulama sistemi bakımı", "Otomatik sulama vanası arızalı.", IsEmriOncelik.Dusuk, IsEmriDurum.YeniTalep, null),
            MakeIsEmri(siteId, "Kapı otomasyonu ayarı", "Giriş kapısı otomatiği geç kapanıyor.", IsEmriOncelik.Normal, IsEmriDurum.Devam, personeller[1]),
            MakeIsEmri(siteId, "Yangın merdiveni boyası", "Yangın merdiveni korkulukları paslanmış.", IsEmriOncelik.Dusuk, IsEmriDurum.Atandi, personeller[1]),
            MakeIsEmri(siteId, "Havuz filtre değişimi", "Havuz filtresi periyodik bakımı yapıldı.", IsEmriOncelik.Normal, IsEmriDurum.Tamamlandi, personeller[2]),
            MakeIsEmri(siteId, "Çatı izolasyon kontrolü", "İptal edilen talep — tekrar planlanacak.", IsEmriOncelik.Dusuk, IsEmriDurum.Iptal, null),
        };
        db.IsEmirleri.AddRange(isEmirleri);

        // ── Yapılacak İşler ───────────────────────────────────────────────────
        var yapilacakIsler = new List<YapilacakIs>
        {
            MakeYapilacakIs(siteId, "Yıllık yangın tatbikatı planlama", "Tüm blok sakinleri için tatbikat organize edilecek.", YapilacakIsOncelik.Yuksek, YapilacakIsDurum.Beklemede),
            MakeYapilacakIs(siteId, "Havuz bakım sözleşmesi yenileme", "Mevcut sözleşme ay sonunda bitiyor.", YapilacakIsOncelik.Normal, YapilacakIsDurum.Devam),
            MakeYapilacakIs(siteId, "Site yönetim planı güncelleme", "Yeni yönetmelik maddeleri eklenecek.", YapilacakIsOncelik.Normal, YapilacakIsDurum.Beklemede),
            MakeYapilacakIs(siteId, "Ortak alan kamera sistemi teklifi", "3 firmadan teklif alınacak.", YapilacakIsOncelik.Dusuk, YapilacakIsDurum.Beklemede),
            MakeYapilacakIs(siteId, "Genel kurul toplantısı davetiyeleri", "Davetiyeler tüm sakinlere gönderildi.", YapilacakIsOncelik.Yuksek, YapilacakIsDurum.Tamamlandi),
            MakeYapilacakIs(siteId, "Bütçe raporu hazırlığı", "Yıl sonu bütçe raporu tamamlandı.", YapilacakIsOncelik.Normal, YapilacakIsDurum.Tamamlandi),
        };
        db.YapilacakIsler.AddRange(yapilacakIsler);

        // ── Duyurular ─────────────────────────────────────────────────────────
        var duyurular = new List<Announcement>
        {
            MakeAnnouncement(siteId, managerUserId, "Genel Kurul Toplantısı Duyurusu", "Yıllık olağan genel kurul toplantımız önümüzdeki ay yapılacaktır.", true, now.AddDays(-2)),
            MakeAnnouncement(siteId, managerUserId, "Asansör Bakım Çalışması", "A Blok asansöründe bakım çalışması nedeniyle kısa süreli kesinti yaşanacaktır.", true, now.AddDays(-5)),
            MakeAnnouncement(siteId, managerUserId, "Aidat Ödeme Hatırlatması", "Bu ayki aidat son ödeme tarihi yaklaşmaktadır.", false, now.AddDays(-8)),
            MakeAnnouncement(siteId, managerUserId, "Ortak Alan Kullanım Kuralları", "Ortak alanların kullanımına ilişkin güncellenmiş kurallar yayınlandı.", false, now.AddDays(-14)),
            MakeAnnouncement(siteId, managerUserId, "Bahçe Düzenleme Çalışmaları", "Peyzaj ekibi önümüzdeki hafta bahçe düzenlemesi yapacaktır.", false, now.AddDays(-20)),
            MakeAnnouncement(siteId, managerUserId, "Yeni Güvenlik Personeli", "Site güvenlik ekibimize yeni bir arkadaşımız katıldı.", false, now.AddDays(-27)),
        };
        db.Announcements.AddRange(duyurular);

        await db.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Role Permissions
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
    // Helper factory methods
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

    /// <summary>Fills in the Kişi Detay tabs (Genel/Detay/Kimlik) for a handful of demo persons
    /// so every tab has non-empty data when manually testing the person detail page.</summary>
    private static void EnrichPersonDetails(
        User user, UserSite us, Gender gender, EducationStatus education, string? school, string profession,
        Nationality nationality, string birthPlace, DateTime birthDate, MaritalStatus maritalStatus,
        string fatherName, string motherName, string taxOffice, string address)
    {
        user.Gender = gender;

        us.TaxOffice = taxOffice;
        us.SecondaryEmail = $"ikincil.{user.FirstName.ToLowerInvariant()}@mail.com";
        us.Address = address;

        us.Description = "Demo verisi — kişi detay sayfası test kaydı.";
        us.EducationStatus = education;
        us.SchoolOrInstitution = school;
        us.Profession = profession;
        us.HasPrivateInsurance = true;
        us.IsMartyrOrVeteranRelative = false;
        us.PetType = PetType.Kedi;
        us.PetDetail = "Tekir, 3 yaşında";

        us.Nationality = nationality;
        us.IdentitySeriNo = "A12";
        us.IdentitySiraNo = "345678";
        us.PassportNo = nationality == Nationality.Yabanci ? "U1234567" : null;
        us.FatherName = fatherName;
        us.MotherName = motherName;
        us.BirthPlace = birthPlace;
        us.BirthDate = birthDate;
        us.MaritalStatus = maritalStatus;
        us.RegisteredCity = birthPlace;
        us.RegisteredDistrict = "Merkez";
        us.RegisteredNeighborhood = "Cumhuriyet Mah.";
        us.FamilySiraNo = "12";
        us.KayitSiraNo = "3";
    }

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
        HgsNo = $"HGS-{plate.Replace(" ", "")}",
        IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static List<PersonUnitHistory> MakeUnitHistoryRows(Guid siteId, List<Unit> units)
    {
        var rows = new List<PersonUnitHistory>();
        foreach (var u in units)
        {
            if (u.OwnerUserId.HasValue)
            {
                rows.Add(new PersonUnitHistory
                {
                    SiteId = siteId,
                    UnitId = u.Id,
                    PersonUserId = u.OwnerUserId.Value,
                    Role = UserType.Owner,
                    EntryDate = DateTime.UtcNow.AddYears(-2),
                    ExitDate = null,
                    ContactPerson = null,
                    Notes = "Demo verisi",
                    BankPaymentCode = $"BPK-{u.DoorNumber}-OWN"
                });
            }

            if (u.TenantUserId.HasValue)
            {
                rows.Add(new PersonUnitHistory
                {
                    SiteId = siteId,
                    UnitId = u.Id,
                    PersonUserId = u.TenantUserId.Value,
                    Role = UserType.Renter,
                    EntryDate = DateTime.UtcNow.AddMonths(-6),
                    ExitDate = null,
                    ContactPerson = null,
                    Notes = "Demo verisi",
                    BankPaymentCode = $"BPK-{u.DoorNumber}-TEN"
                });
            }
        }
        return rows;
    }

    private static AccessCard MakeAccessCard(Guid siteId, Guid userId, Guid unitId, string cardNumber) => new()
    {
        SiteId = siteId,
        UserId = userId,
        UnitId = unitId,
        CardNumber = cardNumber,
        IsActive = true,
        IssueDate = DateTime.UtcNow.AddDays(-30),
        ExpiryDate = DateTime.UtcNow.AddYears(2),
        Notes = "Demo data"
    };

    private static GelirGrubu MakeGelirGrubu(Guid siteId, string name, int order) => new()
    {
        SiteId = siteId, Name = name, IsActive = true, Order = order, CreatedAt = DateTime.UtcNow
    };

    private static GelirTanimi MakeGelirTanimi(Guid siteId, string name, Guid grubuId, int order) => new()
    {
        SiteId = siteId, Name = name, GelirGrubuId = grubuId, IsActive = true, Order = order, CreatedAt = DateTime.UtcNow
    };

    private static GiderGrubu MakeGiderGrubu(Guid siteId, string name, int order) => new()
    {
        SiteId = siteId, Name = name, IsActive = true, Order = order, CreatedAt = DateTime.UtcNow
    };

    private static GiderTanimi MakeGiderTanimi(Guid siteId, string name, Guid grubuId, int order) => new()
    {
        SiteId = siteId, Name = name, GiderGrubuId = grubuId, IsActive = true, Order = order, CreatedAt = DateTime.UtcNow
    };

    private static KasaBanka MakeKasaBanka(
        Guid siteId, string name, KasaBankaTipi tip,
        string? bankaAdi, string? subeAdi, string? hesapNo, string? iban) => new()
    {
        SiteId = siteId, Name = name, Tip = tip,
        BankaAdi = bankaAdi, SubeAdi = subeAdi, HesapNo = hesapNo, IBAN = iban,
        IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static BankaHareketi MakeBankaHareketi(Guid siteId, Guid kasaBankaId, DateTime tarih, string aciklama, decimal tutar) => new()
    {
        SiteId = siteId, KasaBankaId = kasaBankaId, Tarih = tarih, Aciklama = aciklama, Tutar = tutar,
        Durum = BankaHareketiDurum.Tamamlandi, CreatedAt = DateTime.UtcNow
    };

    private static Fatura MakeFatura(
        Guid siteId, string evrakNo, FaturaTipi tip, DateTime faturaTarihi, string cariAdi,
        Guid? giderTanimiId, Guid? gelirTanimiId, decimal tutar,
        DateTime? sonOdemeTarihi, FaturaOdemeDurumu odemeDurumu) => new()
    {
        SiteId = siteId, Tip = tip, EvrakNo = evrakNo, IslemTarihi = faturaTarihi, FaturaTarihi = faturaTarihi,
        CariAdi = cariAdi, GiderTanimiId = giderTanimiId, GelirTanimiId = gelirTanimiId, ToplamTutar = tutar,
        SonOdemeTarihi = sonOdemeTarihi, OdemeDurumu = odemeDurumu,
        Aciklama = OzetDemoMarker, CreatedAt = DateTime.UtcNow
    };

    private static IsEmri MakeIsEmri(
        Guid siteId, string baslik, string aciklama, IsEmriOncelik oncelik, IsEmriDurum durum, string? atananKisiAdi) => new()
    {
        SiteId = siteId, Baslik = baslik, Aciklama = aciklama, Oncelik = oncelik, Durum = durum,
        AtananKisiAdi = atananKisiAdi, CreatedAt = DateTime.UtcNow
    };

    private static YapilacakIs MakeYapilacakIs(
        Guid siteId, string baslik, string aciklama, YapilacakIsOncelik oncelik, YapilacakIsDurum durum) => new()
    {
        SiteId = siteId, Baslik = baslik, Aciklama = aciklama, Oncelik = oncelik, Durum = durum,
        IsActive = true, CreatedAt = DateTime.UtcNow
    };

    private static Announcement MakeAnnouncement(
        Guid siteId, Guid createdByUserId, string title, string content, bool isPinned, DateTime publishDate) => new()
    {
        SiteId = siteId, CreatedByUserId = createdByUserId, Title = title, Content = content,
        IsPinned = isPinned, PublishDate = publishDate, CreatedAt = DateTime.UtcNow
    };
}
