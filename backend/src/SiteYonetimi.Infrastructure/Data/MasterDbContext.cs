using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Entities;

namespace SiteYonetimi.Infrastructure.Data;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<UserSite> UserSites => Set<UserSite>();
    public DbSet<RoleType> RoleTypes => Set<RoleType>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<SubscriptionPlanModule> SubscriptionPlanModules => Set<SubscriptionPlanModule>();
    public DbSet<SiteSubscription> SiteSubscriptions => Set<SiteSubscription>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<PersonPhone> PersonPhones => Set<PersonPhone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.NationalId).HasMaxLength(20);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Site>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.District).HasMaxLength(100);
            e.Property(x => x.City).HasMaxLength(100);
            e.Property(x => x.PostalCode).HasMaxLength(20);
            e.Property(x => x.Phone).HasMaxLength(50);
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.TaxOffice).HasMaxLength(200);
            e.Property(x => x.TaxNumber).HasMaxLength(50);
            e.Property(x => x.ConnectionString).HasMaxLength(1000);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<UserSite>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.SiteId, x.UserType }).IsUnique();
            e.Property(x => x.Status).IsRequired();
            e.HasOne(x => x.User).WithMany(x => x.UserSites).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Site).WithMany(x => x.UserSites).HasForeignKey(x => x.SiteId);
            e.HasOne(x => x.RoleType).WithMany(x => x.UserSites).HasForeignKey(x => x.RoleTypeId).IsRequired(false);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<RoleType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Site).WithMany(x => x.RoleTypes).HasForeignKey(x => x.SiteId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Module>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Page>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Label).HasMaxLength(100).IsRequired();
            e.Property(x => x.Icon).HasMaxLength(50);
            e.Property(x => x.Route).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Module).WithMany(x => x.Pages).HasForeignKey(x => x.ModuleId);
            e.HasOne(x => x.ParentPage).WithMany(x => x.SubPages).HasForeignKey(x => x.ParentPageId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted && x.IsActive);
        });

        modelBuilder.Entity<RolePermission>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.RoleTypeId, x.PageId }).IsUnique();
            e.HasOne(x => x.RoleType).WithMany(x => x.Permissions).HasForeignKey(x => x.RoleTypeId);
            e.HasOne(x => x.Page).WithMany(x => x.Permissions).HasForeignKey(x => x.PageId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(500).IsRequired();
            e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SubscriptionPlan>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SubscriptionPlanModule>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.SubscriptionPlanId, x.ModuleId }).IsUnique();
            e.HasOne(x => x.SubscriptionPlan).WithMany(x => x.PlanModules).HasForeignKey(x => x.SubscriptionPlanId);
            e.HasOne(x => x.Module).WithMany().HasForeignKey(x => x.ModuleId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SiteSubscription>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Site).WithMany(x => x.SiteSubscriptions).HasForeignKey(x => x.SiteId);
            e.HasOne(x => x.SubscriptionPlan).WithMany(x => x.SiteSubscriptions).HasForeignKey(x => x.SubscriptionPlanId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<PasswordResetToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<PersonPhone>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Label).HasMaxLength(50);
            e.HasOne(x => x.UserSite).WithMany(x => x.Phones).HasForeignKey(x => x.UserSiteId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
