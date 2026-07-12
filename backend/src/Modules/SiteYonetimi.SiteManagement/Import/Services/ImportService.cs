using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Import.DTOs;
using SiteYonetimi.SiteManagement.Units.Services;

namespace SiteYonetimi.SiteManagement.Import.Services;

public interface IImportService
{
    byte[] GenerateTemplate(ImportType type);
    Task<ImportPreviewDto> PreviewAsync(Guid siteId, ImportType type, Stream file, CancellationToken ct);
    Task<ImportResultDto> ConfirmAsync(Guid siteId, ImportType type, List<Dictionary<string, string?>> rows, CancellationToken ct);
}

public class ImportService : IImportService
{
    private readonly SharedTenantDbContext _shared;
    private readonly MasterDbContext _master;
    private readonly IPersonUnitHistoryService _historyService;

    public ImportService(SharedTenantDbContext shared, MasterDbContext master, IPersonUnitHistoryService historyService)
    {
        _shared = shared;
        _master = master;
        _historyService = historyService;
    }

    private static readonly string[] BuildingCols = { "BuildingName", "TotalFloors", "Address", "Description" };
    private static readonly string[] UnitCols = { "BuildingName", "FloorNumber", "UnitNumber", "UnitType", "SquareMeters", "Description" };
    private static readonly string[] UserCols = { "FirstName", "LastName", "Email", "Phone", "UnitNumber", "BuildingName", "Role" };
    private static readonly string[] Roles = { "Resident", "Owner", "Manager" };

    private static string[] Columns(ImportType type) => type switch
    {
        ImportType.Buildings => BuildingCols,
        ImportType.Units => UnitCols,
        ImportType.Users => UserCols,
        _ => Array.Empty<string>()
    };

    // ---- Template ----

    public byte[] GenerateTemplate(ImportType type)
    {
        var cols = Columns(type);
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add(type.ToString());

        for (var i = 0; i < cols.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = cols[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ---- Parse ----

    private static List<Dictionary<string, string?>> Parse(Stream file, ImportType type)
    {
        var cols = Columns(type);
        var result = new List<Dictionary<string, string?>>();

        using var wb = new XLWorkbook(file);
        var ws = wb.Worksheets.First();
        var usedRows = ws.RowsUsed().ToList();
        if (usedRows.Count <= 1) return result;

        // Başlık satırından kolon adı → sütun indeksi eşlemesi (büyük/küçük harf duyarsız)
        var headerRow = usedRows[0];
        var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
            colIndex[cell.GetString().Trim()] = cell.Address.ColumnNumber;

        foreach (var row in usedRows.Skip(1))
        {
            var dict = new Dictionary<string, string?>();
            var hasValue = false;
            foreach (var col in cols)
            {
                string? val = null;
                if (colIndex.TryGetValue(col, out var ci))
                {
                    val = row.Cell(ci).GetString().Trim();
                    if (string.IsNullOrEmpty(val)) val = null;
                    else hasValue = true;
                }
                dict[col] = val;
            }
            if (hasValue) result.Add(dict);
        }
        return result;
    }

    // ---- Preview ----

    public async Task<ImportPreviewDto> PreviewAsync(Guid siteId, ImportType type, Stream file, CancellationToken ct)
    {
        var rows = Parse(file, type);
        var results = await ValidateAsync(siteId, type, rows, ct);

        return new ImportPreviewDto
        {
            TotalRows = results.Count,
            ValidRows = results.Count(r => r.IsValid),
            InvalidRows = results.Count(r => !r.IsValid),
            Rows = results
        };
    }

    // ---- Validation ----

    private async Task<List<ImportRowResultDto>> ValidateAsync(
        Guid siteId, ImportType type, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        return type switch
        {
            ImportType.Buildings => await ValidateBuildingsAsync(siteId, rows, ct),
            ImportType.Units => await ValidateUnitsAsync(siteId, rows, ct),
            ImportType.Users => await ValidateUsersAsync(siteId, rows, ct),
            _ => new List<ImportRowResultDto>()
        };
    }

    private async Task<List<ImportRowResultDto>> ValidateBuildingsAsync(
        Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var existing = (await _shared.Buildings.Where(b => b.SiteId == siteId)
            .Select(b => b.Name).ToListAsync(ct))
            .Select(n => n.ToLowerInvariant()).ToHashSet();
        var seen = new HashSet<string>();

        var results = new List<ImportRowResultDto>();
        var i = 1;
        foreach (var row in rows)
        {
            var r = NewRow(i++, row);
            var name = row.GetValueOrDefault("BuildingName");
            if (string.IsNullOrWhiteSpace(name))
                r.Errors.Add("BuildingName zorunludur.");
            else
            {
                var key = name.Trim().ToLowerInvariant();
                if (existing.Contains(key)) r.Errors.Add("Bina zaten mevcut.");
                if (!seen.Add(key)) r.Errors.Add("Dosyada mükerrer bina adı.");
            }
            var floors = row.GetValueOrDefault("TotalFloors");
            if (!string.IsNullOrWhiteSpace(floors) && (!TryInt(floors, out var f) || f <= 0))
                r.Errors.Add("TotalFloors pozitif tam sayı olmalı.");

            r.IsValid = r.Errors.Count == 0;
            results.Add(r);
        }
        return results;
    }

    private async Task<List<ImportRowResultDto>> ValidateUnitsAsync(
        Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var buildings = await _shared.Buildings.Where(b => b.SiteId == siteId)
            .ToDictionaryAsync(b => b.Name.ToLowerInvariant(), b => b.Id, ct);
        var existingUnits = (await _shared.Units.Where(u => u.SiteId == siteId)
            .Select(u => new { u.BuildingId, u.DoorNumber }).ToListAsync(ct))
            .Select(u => $"{u.BuildingId}|{u.DoorNumber.ToLowerInvariant()}").ToHashSet();
        var seen = new HashSet<string>();

        var results = new List<ImportRowResultDto>();
        var i = 1;
        foreach (var row in rows)
        {
            var r = NewRow(i++, row);
            var bName = row.GetValueOrDefault("BuildingName");
            var unitNo = row.GetValueOrDefault("UnitNumber");
            Guid? buildingId = null;

            if (string.IsNullOrWhiteSpace(bName))
                r.Errors.Add("BuildingName zorunludur.");
            else if (!buildings.TryGetValue(bName.Trim().ToLowerInvariant(), out var bid))
                r.Errors.Add("BuildingName bulunamadı.");
            else buildingId = bid;

            if (string.IsNullOrWhiteSpace(unitNo))
                r.Errors.Add("UnitNumber zorunludur.");

            if (buildingId is not null && !string.IsNullOrWhiteSpace(unitNo))
            {
                var key = $"{buildingId}|{unitNo.Trim().ToLowerInvariant()}";
                if (existingUnits.Contains(key)) r.Errors.Add("Bu bina içinde daire no zaten mevcut.");
                if (!seen.Add(key)) r.Errors.Add("Dosyada mükerrer daire (bina + no).");
            }

            var floor = row.GetValueOrDefault("FloorNumber");
            if (!string.IsNullOrWhiteSpace(floor) && !TryInt(floor, out _))
                r.Errors.Add("FloorNumber sayı olmalı.");
            var m2 = row.GetValueOrDefault("SquareMeters");
            if (!string.IsNullOrWhiteSpace(m2) && !TryDecimal(m2, out _))
                r.Errors.Add("SquareMeters sayı olmalı.");

            r.IsValid = r.Errors.Count == 0;
            results.Add(r);
        }
        return results;
    }

    private async Task<List<ImportRowResultDto>> ValidateUsersAsync(
        Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var existingEmails = (await _master.Users.Select(u => u.Email).ToListAsync(ct))
            .Select(e => e.ToLowerInvariant()).ToHashSet();
        var buildings = await _shared.Buildings.Where(b => b.SiteId == siteId)
            .ToDictionaryAsync(b => b.Name.ToLowerInvariant(), b => b.Id, ct);
        var units = (await _shared.Units.Where(u => u.SiteId == siteId)
            .Select(u => new { u.BuildingId, u.DoorNumber }).ToListAsync(ct))
            .Select(u => $"{u.BuildingId}|{u.DoorNumber.ToLowerInvariant()}").ToHashSet();
        var seenEmails = new HashSet<string>();

        var results = new List<ImportRowResultDto>();
        var i = 1;
        foreach (var row in rows)
        {
            var r = NewRow(i++, row);
            if (string.IsNullOrWhiteSpace(row.GetValueOrDefault("FirstName")))
                r.Errors.Add("FirstName zorunludur.");
            if (string.IsNullOrWhiteSpace(row.GetValueOrDefault("LastName")))
                r.Errors.Add("LastName zorunludur.");

            var email = row.GetValueOrDefault("Email");
            if (string.IsNullOrWhiteSpace(email))
                r.Errors.Add("Email zorunludur.");
            else
            {
                var key = email.Trim().ToLowerInvariant();
                if (!IsEmail(key)) r.Errors.Add("Email formatı geçersiz.");
                if (existingEmails.Contains(key)) r.Errors.Add("Email zaten kayıtlı.");
                if (!seenEmails.Add(key)) r.Errors.Add("Dosyada mükerrer email.");
            }

            var phone = row.GetValueOrDefault("Phone");
            if (!string.IsNullOrWhiteSpace(phone) && !IsTrPhone(phone))
                r.Errors.Add("Phone Türkiye formatında olmalı (5xx, 10 hane).");

            var role = row.GetValueOrDefault("Role");
            if (string.IsNullOrWhiteSpace(role) || !Roles.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase))
                r.Errors.Add("Role: Resident, Owner veya Manager olmalı.");

            // Resident/Owner için daire eşleşmesi zorunlu
            if (!string.IsNullOrWhiteSpace(role) &&
                (role.Trim().Equals("Resident", StringComparison.OrdinalIgnoreCase) ||
                 role.Trim().Equals("Owner", StringComparison.OrdinalIgnoreCase)))
            {
                var bName = row.GetValueOrDefault("BuildingName");
                var unitNo = row.GetValueOrDefault("UnitNumber");
                if (string.IsNullOrWhiteSpace(bName) || string.IsNullOrWhiteSpace(unitNo))
                    r.Errors.Add("Resident/Owner için BuildingName ve UnitNumber zorunludur.");
                else if (!buildings.TryGetValue(bName.Trim().ToLowerInvariant(), out var bid)
                         || !units.Contains($"{bid}|{unitNo.Trim().ToLowerInvariant()}"))
                    r.Errors.Add("UnitNumber + BuildingName eşleşen daire bulunamadı.");
            }

            r.IsValid = r.Errors.Count == 0;
            results.Add(r);
        }
        return results;
    }

    // ---- Confirm (persist) ----

    public async Task<ImportResultDto> ConfirmAsync(
        Guid siteId, ImportType type, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var validated = await ValidateAsync(siteId, type, rows, ct);
        var valid = validated.Where(r => r.IsValid).Select(r => r.Data).ToList();
        var skipped = validated.Count - valid.Count;

        if (valid.Count == 0)
            return new ImportResultDto { SavedRows = 0, SkippedRows = skipped, Errors = { "Kaydedilecek geçerli satır yok." } };

        var saved = type switch
        {
            ImportType.Buildings => await PersistBuildingsAsync(siteId, valid, ct),
            ImportType.Units => await PersistUnitsAsync(siteId, valid, ct),
            ImportType.Users => await PersistUsersAsync(siteId, valid, ct),
            _ => 0
        };

        return new ImportResultDto { SavedRows = saved, SkippedRows = skipped };
    }

    private async Task<int> PersistBuildingsAsync(Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        await using var tx = await _shared.Database.BeginTransactionAsync(ct);
        foreach (var row in rows)
        {
            _shared.Buildings.Add(new Building
            {
                SiteId = siteId,
                Name = row["BuildingName"]!.Trim(),
                IsActive = true
            });
        }
        await _shared.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return rows.Count;
    }

    private async Task<int> PersistUnitsAsync(Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var buildings = await _shared.Buildings.Where(b => b.SiteId == siteId)
            .ToDictionaryAsync(b => b.Name.ToLowerInvariant(), b => b.Id, ct);
        var unitTypes = await _shared.UnitTypes.Where(t => t.SiteId == siteId)
            .ToDictionaryAsync(t => t.Name.ToLowerInvariant(), t => t, ct);

        await using var tx = await _shared.Database.BeginTransactionAsync(ct);
        foreach (var row in rows)
        {
            var bId = buildings[row["BuildingName"]!.Trim().ToLowerInvariant()];

            Guid? unitTypeId = null;
            var typeName = row.GetValueOrDefault("UnitType");
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                var tkey = typeName.Trim().ToLowerInvariant();
                if (!unitTypes.TryGetValue(tkey, out var ut))
                {
                    ut = new UnitType { SiteId = siteId, Name = typeName.Trim(), IsActive = true };
                    _shared.UnitTypes.Add(ut);
                    unitTypes[tkey] = ut;
                }
                unitTypeId = ut.Id;
            }

            decimal? m2 = TryDecimal(row.GetValueOrDefault("SquareMeters"), out var mv) ? mv : null;

            _shared.Units.Add(new Unit
            {
                SiteId = siteId,
                BuildingId = bId,
                DoorNumber = row["UnitNumber"]!.Trim(),
                Floor = row.GetValueOrDefault("FloorNumber"),
                UnitTypeId = unitTypeId,
                GrossArea = m2,
                Description = row.GetValueOrDefault("Description")
            });
        }
        await _shared.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return rows.Count;
    }

    private async Task<int> PersistUsersAsync(Guid siteId, List<Dictionary<string, string?>> rows, CancellationToken ct)
    {
        var defaultRole = await _master.RoleTypes
            .FirstOrDefaultAsync(rt => rt.SiteId == siteId && rt.IsDefault, ct);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Sifre1234");

        await using var masterTx = await _master.Database.BeginTransactionAsync(ct);

        // (email → userId) ileride daire eşlemesi için
        var emailToUser = new Dictionary<string, (Guid UserId, string Role, string? Building, string? Unit)>();

        foreach (var row in rows)
        {
            var email = row["Email"]!.Trim().ToLowerInvariant();
            var role = row["Role"]!.Trim();
            var userType = role.Equals("Owner", StringComparison.OrdinalIgnoreCase) ? UserType.Owner
                : role.Equals("Manager", StringComparison.OrdinalIgnoreCase) ? UserType.Management
                : UserType.Renter;

            var user = await _master.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null)
            {
                user = new User
                {
                    FirstName = row["FirstName"]!.Trim(),
                    LastName = row["LastName"]!.Trim(),
                    Email = email,
                    PhoneNumber = row.GetValueOrDefault("Phone"),
                    PasswordHash = passwordHash,
                    IsActive = true,
                    MustChangePassword = true
                };
                _master.Users.Add(user);
                await _master.SaveChangesAsync(ct);
            }

            var exists = await _master.UserSites.AnyAsync(
                us => us.UserId == user.Id && us.SiteId == siteId && us.UserType == userType, ct);
            if (!exists)
            {
                _master.UserSites.Add(new UserSite
                {
                    UserId = user.Id,
                    SiteId = siteId,
                    UserType = userType,
                    RoleTypeId = userType == UserType.Management ? defaultRole?.Id : null,
                    Status = UserSiteStatus.Approved
                });
            }

            emailToUser[email] = (user.Id, role, row.GetValueOrDefault("BuildingName"), row.GetValueOrDefault("UnitNumber"));
        }

        await _master.SaveChangesAsync(ct);
        await masterTx.CommitAsync(ct);

        // Daire sahibi/kiracı işaretlemesi (SharedTenantDb)
        var buildings = await _shared.Buildings.Where(b => b.SiteId == siteId)
            .ToDictionaryAsync(b => b.Name.ToLowerInvariant(), b => b.Id, ct);
        foreach (var (_, info) in emailToUser)
        {
            if (string.IsNullOrWhiteSpace(info.Building) || string.IsNullOrWhiteSpace(info.Unit)) continue;
            if (!buildings.TryGetValue(info.Building.Trim().ToLowerInvariant(), out var bId)) continue;

            var unit = await _shared.Units.FirstOrDefaultAsync(
                u => u.SiteId == siteId && u.BuildingId == bId && u.DoorNumber == info.Unit.Trim(), ct);
            if (unit is null) continue;

            if (info.Role.Equals("Owner", StringComparison.OrdinalIgnoreCase))
            {
                unit.OwnerUserId = info.UserId;
                await _historyService.RecordAssignmentAsync(siteId, unit.Id, info.UserId, UserType.Owner, ct);
            }
            else if (info.Role.Equals("Resident", StringComparison.OrdinalIgnoreCase))
            {
                unit.TenantUserId = info.UserId;
                await _historyService.RecordAssignmentAsync(siteId, unit.Id, info.UserId, UserType.Renter, ct);
            }
            unit.UpdatedAt = DateTime.UtcNow;
        }
        await _shared.SaveChangesAsync(ct);

        return rows.Count;
    }

    // ---- Helpers ----

    private static ImportRowResultDto NewRow(int index, Dictionary<string, string?> data)
        => new() { RowIndex = index, Data = data };

    private static bool TryInt(string? s, out int v)
        => int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

    private static bool TryDecimal(string? s, out decimal v)
    {
        s = (s ?? "").Trim().Replace(',', '.');
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }

    private static bool IsEmail(string s) => Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private static bool IsTrPhone(string s)
    {
        var digits = Regex.Replace(s, @"\D", "");
        if (digits.Length == 12 && digits.StartsWith("90")) digits = digits[2..];
        if (digits.Length == 11 && digits.StartsWith("0")) digits = digits[1..];
        return digits.Length == 10 && digits.StartsWith("5");
    }
}
