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
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<AccessCard> AccessCards => Set<AccessCard>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

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
    }
}
