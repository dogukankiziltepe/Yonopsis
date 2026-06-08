using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Import.DTOs;
using SiteYonetimi.SiteManagement.Import.Services;

namespace SiteYonetimi.SiteManagement.Import.Queries;

public record PreviewImportQuery(Guid SiteId, string ImportType, byte[] FileBytes) : IRequest<Result<ImportPreviewResult>>;

public class PreviewImportQueryHandler : IRequestHandler<PreviewImportQuery, Result<ImportPreviewResult>>
{
    private readonly SharedTenantDbContext _sharedDb;
    private readonly MasterDbContext _masterDb;
    private readonly ExcelTemplateService _templateService;

    public PreviewImportQueryHandler(SharedTenantDbContext sharedDb, MasterDbContext masterDb, ExcelTemplateService templateService)
    {
        _sharedDb = sharedDb;
        _masterDb = masterDb;
        _templateService = templateService;
    }

    public async Task<Result<ImportPreviewResult>> Handle(PreviewImportQuery request, CancellationToken ct)
    {
        return request.ImportType.ToLower() switch
        {
            "buildings" => await PreviewBuildings(request.SiteId, request.FileBytes, ct),
            "units"     => await PreviewUnits(request.SiteId, request.FileBytes, ct),
            "users"     => await PreviewUsers(request.SiteId, request.FileBytes, ct),
            _           => Result<ImportPreviewResult>.Failure("Geçersiz import tipi. Geçerli değerler: buildings, units, users")
        };
    }

    private async Task<Result<ImportPreviewResult>> PreviewBuildings(Guid siteId, byte[] fileBytes, CancellationToken ct)
    {
        List<(BuildingImportRowData Data, int RowIndex)> parsed;
        try { parsed = _templateService.ParseBuildingsFile(fileBytes); }
        catch { return Result<ImportPreviewResult>.Failure("Excel dosyası okunamadı. Lütfen geçerli bir şablon dosyası yükleyin."); }

        var existingNames = await _sharedDb.Buildings
            .Where(b => b.SiteId == siteId)
            .Select(b => b.Name.ToLower())
            .ToListAsync(ct);
        var existingNameSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);

        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rows = new List<ImportPreviewRow>();

        foreach (var (data, rowIndex) in parsed)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(data.BuildingName))
                errors.Add("BuildingName zorunludur.");
            else if (existingNameSet.Contains(data.BuildingName))
                errors.Add($"'{data.BuildingName}' adlı bina zaten mevcut.");
            else if (!seenNames.Add(data.BuildingName))
                errors.Add($"'{data.BuildingName}' bu dosyada tekrar kullanılmış.");

            if (data.TotalFloors is null)
                errors.Add("TotalFloors zorunludur.");
            else if (data.TotalFloors <= 0)
                errors.Add("TotalFloors pozitif bir tam sayı olmalıdır.");

            rows.Add(new ImportPreviewRow(
                RowIndex: rowIndex,
                Data: new Dictionary<string, object?>
                {
                    ["buildingName"] = data.BuildingName,
                    ["totalFloors"]  = data.TotalFloors,
                    ["address"]      = data.Address,
                    ["description"]  = data.Description,
                },
                IsValid: errors.Count == 0,
                Errors: errors
            ));
        }

        return Result<ImportPreviewResult>.Success(BuildResult(rows));
    }

    private async Task<Result<ImportPreviewResult>> PreviewUnits(Guid siteId, byte[] fileBytes, CancellationToken ct)
    {
        List<(UnitImportRowData Data, int RowIndex)> parsed;
        try { parsed = _templateService.ParseUnitsFile(fileBytes); }
        catch { return Result<ImportPreviewResult>.Failure("Excel dosyası okunamadı. Lütfen geçerli bir şablon dosyası yükleyin."); }

        var buildingMap = await _sharedDb.Buildings
            .Where(b => b.SiteId == siteId)
            .ToDictionaryAsync(b => b.Name.ToLower(), b => b, ct);

        var unitTypeMap = await _sharedDb.UnitTypes
            .Where(ut => ut.SiteId == siteId)
            .ToDictionaryAsync(ut => ut.Name.ToLower(), ut => ut.Id, ct);

        var existingUnits = await _sharedDb.Units
            .Where(u => u.SiteId == siteId)
            .Select(u => new { u.BuildingId, DoorLower = u.DoorNumber.ToLower() })
            .ToListAsync(ct);

        var seenUnits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rows = new List<ImportPreviewRow>();

        foreach (var (data, rowIndex) in parsed)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(data.BuildingName))
            {
                errors.Add("BuildingName zorunludur.");
            }
            else if (!buildingMap.TryGetValue(data.BuildingName.ToLower(), out var building))
            {
                errors.Add($"'{data.BuildingName}' adlı bina bulunamadı.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(data.UnitNumber))
                {
                    errors.Add("UnitNumber zorunludur.");
                }
                else
                {
                    var existsInDb = existingUnits.Any(u =>
                        u.BuildingId == building.Id &&
                        u.DoorLower == data.UnitNumber.ToLower());

                    var batchKey = $"{building.Id}|{data.UnitNumber.ToLower()}";

                    if (existsInDb)
                        errors.Add($"'{data.UnitNumber}' numaralı daire bu binada zaten mevcut.");
                    else if (!seenUnits.Add(batchKey))
                        errors.Add($"'{data.UnitNumber}' bu dosyada aynı bina için tekrar kullanılmış.");
                }

                if (data.FloorNumber is null)
                {
                    errors.Add("FloorNumber zorunludur.");
                }
                else if (building.TotalFloors.HasValue && data.FloorNumber > building.TotalFloors)
                {
                    errors.Add($"FloorNumber ({data.FloorNumber}) binanın toplam katını ({building.TotalFloors}) aşıyor.");
                }
            }

            if (!string.IsNullOrWhiteSpace(data.UnitType) && !unitTypeMap.ContainsKey(data.UnitType.ToLower()))
                errors.Add($"'{data.UnitType}' daire tipi bulunamadı.");

            rows.Add(new ImportPreviewRow(
                RowIndex: rowIndex,
                Data: new Dictionary<string, object?>
                {
                    ["buildingName"] = data.BuildingName,
                    ["floorNumber"]  = data.FloorNumber,
                    ["unitNumber"]   = data.UnitNumber,
                    ["unitType"]     = data.UnitType,
                    ["squareMeters"] = data.SquareMeters,
                    ["description"]  = data.Description,
                },
                IsValid: errors.Count == 0,
                Errors: errors
            ));
        }

        return Result<ImportPreviewResult>.Success(BuildResult(rows));
    }

    private async Task<Result<ImportPreviewResult>> PreviewUsers(Guid siteId, byte[] fileBytes, CancellationToken ct)
    {
        List<(UserImportRowData Data, int RowIndex)> parsed;
        try { parsed = _templateService.ParseUsersFile(fileBytes); }
        catch { return Result<ImportPreviewResult>.Failure("Excel dosyası okunamadı. Lütfen geçerli bir şablon dosyası yükleyin."); }

        var siteUserEmails = await _masterDb.UserSites
            .Where(us => us.SiteId == siteId)
            .Join(_masterDb.Users, us => us.UserId, u => u.Id, (us, u) => u.Email)
            .ToListAsync(ct);
        var siteEmailSet = new HashSet<string>(siteUserEmails, StringComparer.OrdinalIgnoreCase);

        var unitKeys = await _sharedDb.Units
            .Where(u => u.SiteId == siteId)
            .Join(_sharedDb.Buildings, u => u.BuildingId, b => b.Id, (u, b) => new
            {
                DoorLower     = u.DoorNumber.ToLower(),
                BuildingLower = b.Name.ToLower()
            })
            .ToListAsync(ct);
        var unitKeySet = unitKeys.Select(u => $"{u.DoorLower}|{u.BuildingLower}").ToHashSet();

        var validRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Resident", "Owner", "Manager" };
        var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rows = new List<ImportPreviewRow>();

        foreach (var (data, rowIndex) in parsed)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(data.FirstName)) errors.Add("FirstName zorunludur.");
            if (string.IsNullOrWhiteSpace(data.LastName))  errors.Add("LastName zorunludur.");

            if (string.IsNullOrWhiteSpace(data.Email))
            {
                errors.Add("Email zorunludur.");
            }
            else if (!IsValidEmail(data.Email))
            {
                errors.Add("Email formatı geçersiz.");
            }
            else if (siteEmailSet.Contains(data.Email))
            {
                errors.Add($"'{data.Email}' bu siteye zaten eklenmiş.");
            }
            else if (!seenEmails.Add(data.Email))
            {
                errors.Add($"'{data.Email}' bu dosyada tekrar kullanılmış.");
            }

            if (!string.IsNullOrWhiteSpace(data.Phone) && !IsValidTurkishPhone(data.Phone))
                errors.Add("Phone Türkiye formatında olmalıdır (10 hane, 5xx ile başlamalı).");

            if (string.IsNullOrWhiteSpace(data.Role))
                errors.Add("Role zorunludur.");
            else if (!validRoles.Contains(data.Role))
                errors.Add("Role geçersiz. Geçerli değerler: Resident, Owner, Manager");

            if (!string.IsNullOrWhiteSpace(data.UnitNumber) && !string.IsNullOrWhiteSpace(data.BuildingName))
            {
                var key = $"{data.UnitNumber.ToLower()}|{data.BuildingName.ToLower()}";
                if (!unitKeySet.Contains(key))
                    errors.Add($"'{data.UnitNumber}' - '{data.BuildingName}' kombinasyonu bulunamadı.");
            }

            rows.Add(new ImportPreviewRow(
                RowIndex: rowIndex,
                Data: new Dictionary<string, object?>
                {
                    ["firstName"]   = data.FirstName,
                    ["lastName"]    = data.LastName,
                    ["email"]       = data.Email,
                    ["phone"]       = data.Phone,
                    ["unitNumber"]  = data.UnitNumber,
                    ["buildingName"]= data.BuildingName,
                    ["role"]        = data.Role,
                },
                IsValid: errors.Count == 0,
                Errors: errors
            ));
        }

        return Result<ImportPreviewResult>.Success(BuildResult(rows));
    }

    private static ImportPreviewResult BuildResult(List<ImportPreviewRow> rows)
    {
        int valid = rows.Count(r => r.IsValid);
        return new ImportPreviewResult(rows.Count, valid, rows.Count - valid, rows);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch { return false; }
    }

    private static bool IsValidTurkishPhone(string phone)
    {
        var cleaned = phone.Replace(" ", "").Replace("-", "");
        return cleaned.Length == 10 && cleaned[0] == '5' && cleaned.All(char.IsDigit);
    }
}
