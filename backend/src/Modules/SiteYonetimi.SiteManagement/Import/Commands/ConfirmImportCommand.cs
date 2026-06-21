using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Import.DTOs;

namespace SiteYonetimi.SiteManagement.Import.Commands;

// ─── Buildings ───────────────────────────────────────────────────────────────

public record ConfirmBuildingsImportCommand(Guid SiteId, List<BuildingImportRowData> Rows) : IRequest<Result<ImportConfirmResult>>;

public class ConfirmBuildingsImportCommandHandler : IRequestHandler<ConfirmBuildingsImportCommand, Result<ImportConfirmResult>>
{
    private readonly SharedTenantDbContext _db;

    public ConfirmBuildingsImportCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<ImportConfirmResult>> Handle(ConfirmBuildingsImportCommand request, CancellationToken ct)
    {
        var existingNames = await _db.Buildings
            .Where(b => b.SiteId == request.SiteId)
            .Select(b => b.Name.ToLower())
            .ToListAsync(ct);

        var saved = 0;
        var skipped = 0;
        var errors = new List<string>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var row in request.Rows)
            {
                if (string.IsNullOrWhiteSpace(row.BuildingName) || row.TotalFloors is null or <= 0)
                {
                    skipped++;
                    continue;
                }

                if (existingNames.Contains(row.BuildingName.ToLower()) || !seenNames.Add(row.BuildingName))
                {
                    errors.Add($"'{row.BuildingName}' atlandı: zaten mevcut veya tekrar eden kayıt.");
                    skipped++;
                    continue;
                }

                _db.Buildings.Add(new Building
                {
                    SiteId = request.SiteId,
                    Name = row.BuildingName,
                    TotalFloors = row.TotalFloors,
                    Address = row.Address,
                    Description = row.Description,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                saved++;
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Result<ImportConfirmResult>.Failure($"Kayıt sırasında hata oluştu: {ex.Message}");
        }

        return Result<ImportConfirmResult>.Success(new ImportConfirmResult(saved, skipped, errors));
    }
}

// ─── Units ───────────────────────────────────────────────────────────────────

public record ConfirmUnitsImportCommand(Guid SiteId, List<UnitImportRowData> Rows) : IRequest<Result<ImportConfirmResult>>;

public class ConfirmUnitsImportCommandHandler : IRequestHandler<ConfirmUnitsImportCommand, Result<ImportConfirmResult>>
{
    private readonly SharedTenantDbContext _db;

    public ConfirmUnitsImportCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<ImportConfirmResult>> Handle(ConfirmUnitsImportCommand request, CancellationToken ct)
    {
        var buildings = await _db.Buildings
            .Where(b => b.SiteId == request.SiteId)
            .ToDictionaryAsync(b => b.Name.ToLower(), b => b, ct);

        var unitTypes = await _db.UnitTypes
            .Where(ut => ut.SiteId == request.SiteId)
            .ToDictionaryAsync(ut => ut.Name.ToLower(), ut => ut.Id, ct);

        var existingUnits = await _db.Units
            .Where(u => u.SiteId == request.SiteId)
            .Select(u => new { u.BuildingId, DoorLower = u.DoorNumber.ToLower() })
            .ToListAsync(ct);
        var existingSet = existingUnits.Select(u => (u.BuildingId, u.DoorLower)).ToHashSet();

        var saved = 0;
        var skipped = 0;
        var errors = new List<string>();
        var seenUnits = new HashSet<(Guid, string)>();

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var row in request.Rows)
            {
                if (string.IsNullOrWhiteSpace(row.BuildingName) || string.IsNullOrWhiteSpace(row.UnitNumber))
                {
                    skipped++;
                    continue;
                }

                if (!buildings.TryGetValue(row.BuildingName.ToLower(), out var building))
                {
                    errors.Add($"'{row.BuildingName}' binası bulunamadı, satır atlandı.");
                    skipped++;
                    continue;
                }

                var key = (building.Id, row.UnitNumber.ToLower());
                if (existingSet.Contains(key) || !seenUnits.Add(key))
                {
                    errors.Add($"'{row.UnitNumber}' ({row.BuildingName}) atlandı: zaten mevcut veya tekrar eden kayıt.");
                    skipped++;
                    continue;
                }

                Guid? unitTypeId = null;
                if (!string.IsNullOrWhiteSpace(row.UnitType))
                {
                    if (unitTypes.TryGetValue(row.UnitType.ToLower(), out var typeId))
                        unitTypeId = typeId;
                }

                _db.Units.Add(new Unit
                {
                    SiteId = request.SiteId,
                    BuildingId = building.Id,
                    UnitTypeId = unitTypeId,
                    DoorNumber = row.UnitNumber,
                    Code = row.UnitNumber,
                    Floor = row.FloorNumber?.ToString(),
                    GrossArea = row.SquareMeters,
                    Status = UnitStatus.Bos,
                    HasDask = false,
                    Description = row.Description,
                    CreatedAt = DateTime.UtcNow
                });
                saved++;
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Result<ImportConfirmResult>.Failure($"Kayıt sırasında hata oluştu: {ex.Message}");
        }

        return Result<ImportConfirmResult>.Success(new ImportConfirmResult(saved, skipped, errors));
    }
}

// ─── Users ───────────────────────────────────────────────────────────────────

public record ConfirmUsersImportCommand(Guid SiteId, List<UserImportRowData> Rows) : IRequest<Result<ImportConfirmResult>>;

public class ConfirmUsersImportCommandHandler : IRequestHandler<ConfirmUsersImportCommand, Result<ImportConfirmResult>>
{
    private readonly MasterDbContext _masterDb;
    private readonly SharedTenantDbContext _sharedDb;

    public ConfirmUsersImportCommandHandler(MasterDbContext masterDb, SharedTenantDbContext sharedDb)
    {
        _masterDb = masterDb;
        _sharedDb = sharedDb;
    }

    public async Task<Result<ImportConfirmResult>> Handle(ConfirmUsersImportCommand request, CancellationToken ct)
    {
        var existingUserSiteEmails = await _masterDb.UserSites
            .Where(us => us.SiteId == request.SiteId)
            .Join(_masterDb.Users, us => us.UserId, u => u.Id, (us, u) => u.Email)
            .ToListAsync(ct);
        var siteEmailSet = new HashSet<string>(existingUserSiteEmails, StringComparer.OrdinalIgnoreCase);

        var unitLookup = await _sharedDb.Units
            .Where(u => u.SiteId == request.SiteId)
            .Join(_sharedDb.Buildings, u => u.BuildingId, b => b.Id, (u, b) => new
            {
                u.Id,
                DoorLower = u.DoorNumber.ToLower(),
                BuildingLower = b.Name.ToLower(),
                u.OwnerUserId,
                u.TenantUserId
            })
            .ToListAsync(ct);

        var defaultRoleType = await _masterDb.RoleTypes
            .Where(rt => rt.SiteId == request.SiteId && rt.IsDefault)
            .FirstOrDefaultAsync(ct);

        var saved = 0;
        var skipped = 0;
        var errors = new List<string>();
        var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        const string tempPassword = "Sifre1234";

        await using var tx = await _masterDb.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var row in request.Rows)
            {
                if (string.IsNullOrWhiteSpace(row.Email) || string.IsNullOrWhiteSpace(row.FirstName)
                    || string.IsNullOrWhiteSpace(row.LastName) || string.IsNullOrWhiteSpace(row.Role))
                {
                    skipped++;
                    continue;
                }

                if (siteEmailSet.Contains(row.Email) || !seenEmails.Add(row.Email))
                {
                    errors.Add($"'{row.Email}' atlandı: zaten siteye eklenmiş veya tekrar eden kayıt.");
                    skipped++;
                    continue;
                }

                var userType = MapRole(row.Role);
                if (userType is null)
                {
                    errors.Add($"'{row.Email}' atlandı: geçersiz rol '{row.Role}'.");
                    skipped++;
                    continue;
                }

                var user = await _masterDb.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == row.Email.ToLower().Trim(), ct);

                if (user is null)
                {
                    user = new User
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        Email = row.Email.ToLower().Trim(),
                        PhoneNumber = row.Phone,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                        MustChangePassword = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _masterDb.Users.Add(user);
                    await _masterDb.SaveChangesAsync(ct);
                }

                var roleTypeId = userType == UserType.Management ? defaultRoleType?.Id : null;

                _masterDb.UserSites.Add(new UserSite
                {
                    UserId = user.Id,
                    SiteId = request.SiteId,
                    UserType = userType.Value,
                    RoleTypeId = roleTypeId,
                    Status = UserSiteStatus.Approved,
                    CreatedAt = DateTime.UtcNow
                });
                await _masterDb.SaveChangesAsync(ct);

                // Assign to unit if provided
                if (!string.IsNullOrWhiteSpace(row.UnitNumber) && !string.IsNullOrWhiteSpace(row.BuildingName))
                {
                    var unitEntry = unitLookup.FirstOrDefault(u =>
                        u.DoorLower == row.UnitNumber.ToLower() &&
                        u.BuildingLower == row.BuildingName.ToLower());

                    if (unitEntry != null)
                    {
                        var unit = await _sharedDb.Units.FindAsync(new object[] { unitEntry.Id }, ct);
                        if (unit != null)
                        {
                            if (userType == UserType.Owner && unit.OwnerUserId is null)
                                unit.OwnerUserId = user.Id;
                            else if (userType == UserType.Renter && unit.TenantUserId is null)
                                unit.TenantUserId = user.Id;
                        }
                        await _sharedDb.SaveChangesAsync(ct);
                    }
                }

                saved++;
            }

            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Result<ImportConfirmResult>.Failure($"Kayıt sırasında hata oluştu: {ex.Message}");
        }

        return Result<ImportConfirmResult>.Success(new ImportConfirmResult(saved, skipped, errors));
    }

    private static UserType? MapRole(string role) => role.ToLower() switch
    {
        "owner"    => UserType.Owner,
        "resident" => UserType.Renter,
        "manager"  => UserType.Management,
        _          => null
    };
}
