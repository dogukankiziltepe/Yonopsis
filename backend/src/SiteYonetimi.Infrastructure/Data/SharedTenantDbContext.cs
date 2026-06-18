using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Entities.Shared;

namespace SiteYonetimi.Infrastructure.Data;

/// <summary>
/// DbMode = Shared olan sitelerin operasyonel verisi burada tutulur.
/// Tüm tablolarda SiteId kolonu ile tenant ayrımı yapılır.
/// </summary>
public class SharedTenantDbContext : DbContext
{
    public SharedTenantDbContext(DbContextOptions<SharedTenantDbContext> options) : base(options) { }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
    public DbSet<SupportRequestComment> SupportRequestComments => Set<SupportRequestComment>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<AccessCard> AccessCards => Set<AccessCard>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();

    // Muhasebe modülü
    public DbSet<HesapPlani> HesapPlani => Set<HesapPlani>();
    public DbSet<MuhasebeDonem> MuhasebeDonemler => Set<MuhasebeDonem>();
    public DbSet<MuhasebeFisi> MuhasebeFisleri => Set<MuhasebeFisi>();
    public DbSet<MuhasebeFisiDetay> MuhasebeFisiDetaylari => Set<MuhasebeFisiDetay>();
    public DbSet<MuhasebeParametre> MuhasebeParametreler => Set<MuhasebeParametre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Building>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasMany(x => x.Units).WithOne(x => x.Building).HasForeignKey(x => x.BuildingId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Unit>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.BuildingId).IsRequired();
            e.Property(x => x.DoorNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.Code).HasMaxLength(20);
            e.Property(x => x.Floor).HasMaxLength(50);
            e.Property(x => x.GrossArea).HasPrecision(18, 2);
            e.Property(x => x.NetArea).HasPrecision(18, 2);
            e.Property(x => x.LandShare).HasPrecision(18, 2);
            e.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            e.Property(x => x.Internet).HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasOne(x => x.UnitType).WithMany().HasForeignKey(x => x.UnitTypeId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<UnitType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SupportRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Comments).WithOne(x => x.Request).HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SupportRequestComment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Content).HasMaxLength(2000).IsRequired();
            e.Property(x => x.AuthorName).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Plate).HasMaxLength(20).IsRequired();
            e.Property(x => x.Brand).HasMaxLength(50);
            e.Property(x => x.Model).HasMaxLength(50);
            e.Property(x => x.Color).HasMaxLength(30);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AccessCard>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.CardNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(500);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Announcement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Content).HasMaxLength(5000).IsRequired();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<UploadedFile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UploadedByUserId).IsRequired();
            e.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.StoredFileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.Property(x => x.FileSize).IsRequired();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<HesapPlani>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.HesapKodu).HasMaxLength(50).IsRequired();
            e.Property(x => x.HesapAdi).HasMaxLength(200).IsRequired();
            e.Property(x => x.HesapTipi).HasConversion<int>();
            e.Property(x => x.HesapKategorisi).HasConversion<int>();
            e.Property(x => x.NormalBakiye).HasConversion<int>();
            e.Property(x => x.CariTuru).HasConversion<int>();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            // Hesap kodu tenant içinde unique (silinmemiş kayıtlar için)
            e.HasIndex(x => new { x.SiteId, x.HesapKodu }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasIndex(x => new { x.SiteId, x.ParentId });
            e.HasIndex(x => new { x.SiteId, x.PersonId });
            e.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeDonem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasIndex(x => new { x.SiteId, x.Yil }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeFisi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.DonemId).IsRequired();
            e.Property(x => x.FisNo).HasMaxLength(50).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.FisTuru).HasConversion<int>();
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.ToplamBorc).HasPrecision(18, 2);
            e.Property(x => x.ToplamAlacak).HasPrecision(18, 2);
            e.HasIndex(x => new { x.SiteId, x.FisNo }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasIndex(x => new { x.SiteId, x.DonemId, x.YevmiyeNo });
            e.HasIndex(x => new { x.SiteId, x.FisTarihi });
            e.HasOne(x => x.Donem).WithMany().HasForeignKey(x => x.DonemId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Detaylar).WithOne(x => x.Fis).HasForeignKey(x => x.FisId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeFisiDetay>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.FisId).IsRequired();
            e.Property(x => x.HesapId).IsRequired();
            e.Property(x => x.HesapKodu).HasMaxLength(50).IsRequired();
            e.Property(x => x.BorcTutar).HasPrecision(18, 2);
            e.Property(x => x.AlacakTutar).HasPrecision(18, 2);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.BelgeNo).HasMaxLength(100);
            e.HasIndex(x => new { x.SiteId, x.HesapId });
            e.HasIndex(x => x.FisId);
        });

        modelBuilder.Entity<MuhasebeParametre>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.AlicilarAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.SaticilarAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.GiderAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.CariKodSablonu).HasMaxLength(100);
            e.Property(x => x.FisNoSablonu).HasMaxLength(100);
            e.Property(x => x.ParaBirimi).HasMaxLength(10);
            e.Property(x => x.KdvOrani).HasPrecision(5, 2);
            e.HasIndex(x => x.SiteId).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
