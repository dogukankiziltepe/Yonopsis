using BCrypt.Net;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Seed;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Sites.DTOs;

namespace SiteYonetimi.SiteManagement.Sites.Commands;

public record CreateSiteCommand(CreateSiteDto Dto) : IRequest<Result<Guid>>;

public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public CreateSiteCommandHandler(MasterDbContext db, IEmailService emailService, IConfiguration configuration)
    {
        _db = db;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<Result<Guid>> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var site = new Site
        {
            Name = dto.Name,
            Address = dto.Address,
            District = dto.District,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Phone = dto.Phone,
            Email = dto.Email,
            TaxOffice = dto.TaxOffice,
            TaxNumber = dto.TaxNumber,
            DbMode = dto.DbMode,
            IsActive = true
        };

        _db.Sites.Add(site);

        var siteAdminRole = new RoleType
        {
            SiteId = site.Id,
            Name = "SiteAdmin",
            IsDefault = true
        };

        _db.RoleTypes.Add(siteAdminRole);

        var tempPassword = Guid.NewGuid().ToString("N")[..8];
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);

        var adminUser = new User
        {
            FirstName = dto.AdminFirstName,
            LastName = dto.AdminLastName,
            Email = dto.AdminEmail,
            PasswordHash = passwordHash,
            PhoneNumber = dto.AdminPhone,
            IsActive = true,
            MustChangePassword = true
        };

        _db.Users.Add(adminUser);

        var userSite = new UserSite
        {
            UserId = adminUser.Id,
            SiteId = site.Id,
            UserType = UserType.Management,
            RoleTypeId = siteAdminRole.Id,
            Status = UserSiteStatus.Approved
        };

        _db.UserSites.Add(userSite);

        // Auto-create Temel plan subscription for new sites
        var temelPlan = await _db.SubscriptionPlans
            .FirstOrDefaultAsync(sp => sp.Name == "Temel" && sp.IsActive, cancellationToken);
        if (temelPlan != null)
        {
            _db.SiteSubscriptions.Add(new SiteSubscription
            {
                SiteId = site.Id,
                SubscriptionPlanId = temelPlan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddYears(10),
                IsActive = true
            });
        }

        if (dto.DbMode == DbMode.Dedicated)
        {
            var masterConnStr = _configuration.GetConnectionString("MasterDb")!;
            var dbName = $"SiteYonetimi_{site.Id:N}";

            // Ayrı SqlConnection ile CREATE DATABASE — transaction dışında olmalı
            using var rawConn = new SqlConnection(masterConnStr);
            await rawConn.OpenAsync(cancellationToken);
            using var cmd = new SqlCommand(
                $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}') CREATE DATABASE [{dbName}]",
                rawConn);
            await cmd.ExecuteNonQueryAsync(cancellationToken);

            // Connection string türet
            var builder = new SqlConnectionStringBuilder(masterConnStr);
            builder.InitialCatalog = dbName;
            var dedicatedConnStr = builder.ConnectionString;

            // Migration çalıştır
            var options = new DbContextOptionsBuilder<SharedTenantDbContext>()
                .UseSqlServer(dedicatedConnStr)
                .Options;
            await using var dedicatedDb = new SharedTenantDbContext(options);
            await dedicatedDb.Database.MigrateAsync(cancellationToken);

            site.ConnectionString = dedicatedConnStr;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // Yeni site için varsayılan muhasebe hesap planı + açık dönem seed et (idempotent).
        try
        {
            var tenantConnStr = site.DbMode == DbMode.Dedicated
                ? site.ConnectionString!
                : _configuration.GetConnectionString("SharedTenantDb")!;
            var tenantOptions = new DbContextOptionsBuilder<SharedTenantDbContext>()
                .UseSqlServer(tenantConnStr)
                .Options;
            await using var tenantDb = new SharedTenantDbContext(tenantOptions);
            await MuhasebeSeeder.SeedForSiteAsync(tenantDb, site.Id);
        }
        catch { /* Seed başarısız olsa da site oluşturma başarılı sayılır; sonradan tekrar denenebilir */ }

        await _emailService.SendAsync(
            dto.AdminEmail,
            "SiteYonetimi - Hesabiniz Olusturuldu",
            $"Merhaba {dto.AdminFirstName} {dto.AdminLastName},\n\n" +
            $"SiteYonetimi sistemine hoş geldiniz. \"{site.Name}\" sitesi için yönetici hesabınız oluşturulmuştur.\n\n" +
            $"Geçici şifreniz: {tempPassword}\n\n" +
            "İlk girişinizde şifrenizi değiştirmeniz gerekmektedir.\n\n" +
            "SiteYonetimi Ekibi");

        return Result<Guid>.Success(site.Id);
    }
}

public class CreateSiteDtoValidator : AbstractValidator<CreateSiteDto>
{
    public CreateSiteDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Site adı zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.DbMode).IsInEnum().WithMessage("Geçerli bir DbMode değeri giriniz (Shared=1, Dedicated=2).");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.PostalCode).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.PostalCode));
        RuleFor(x => x.Phone).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.TaxNumber).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.TaxNumber));
        RuleFor(x => x.AdminFirstName).NotEmpty().MaximumLength(100).WithMessage("Yönetici adı zorunludur.");
        RuleFor(x => x.AdminLastName).NotEmpty().MaximumLength(100).WithMessage("Yönetici soyadı zorunludur.");
        RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress().WithMessage("Geçerli bir yönetici e-posta adresi zorunludur.");
        RuleFor(x => x.AdminPhone).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.AdminPhone));
    }
}
