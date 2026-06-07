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
    }
}
