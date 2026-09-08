using System.Globalization;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BorcMakbuzlari.TopluBorclandirma.DTOs;

namespace SiteYonetimi.SiteManagement.BorcMakbuzlari.TopluBorclandirma.Services;

public interface ITopluBorclandirmaService
{
    Task<List<GelirTanimiSecimDto>> GetAktifGelirTanimlariAsync(Guid siteId, CancellationToken ct);
    Task<byte[]> GenerateTemplateAsync(Guid siteId, TopluBorclandirmaTemplateRequestDto dto, CancellationToken ct);
    Task<TopluBorclandirmaPreviewDto> PreviewAsync(Guid siteId, Stream file, CancellationToken ct);
    Task<Result<TopluBorclandirmaSonucDto>> ConfirmAsync(Guid siteId, TopluBorclandirmaConfirmDto dto, CancellationToken ct);
}

/// <summary>
/// Toplu borçlandırma — kalem bazlı gerçek Excel (.xlsx) indir/yükle akışı. ImportService'ten
/// (Modules/SiteYonetimi.SiteManagement/Import) bağımsız yazılmıştır: bu akışın kolon sayısı seçilen
/// kalem sayısına göre dinamik olduğundan, ImportService'in sabit ImportType/Columns() deseni buraya uymaz.
/// </summary>
public class TopluBorclandirmaService : ITopluBorclandirmaService
{
    private const string MetaSheetName = "__meta";

    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public TopluBorclandirmaService(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<List<GelirTanimiSecimDto>> GetAktifGelirTanimlariAsync(Guid siteId, CancellationToken ct)
    {
        return await _db.GelirTanimlari
            .Where(x => x.SiteId == siteId && x.IsActive)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new GelirTanimiSecimDto(x.Id, x.Name))
            .ToListAsync(ct);
    }

    // ── Template üretimi ─────────────────────────────────────────────────────

    public async Task<byte[]> GenerateTemplateAsync(Guid siteId, TopluBorclandirmaTemplateRequestDto dto, CancellationToken ct)
    {
        var kalemler = await _db.GelirTanimlari
            .Where(x => x.SiteId == siteId && dto.GelirTanimiIds.Contains(x.Id))
            .ToListAsync(ct);
        // İstek sırasını koru
        kalemler = dto.GelirTanimiIds
            .Select(id => kalemler.FirstOrDefault(k => k.Id == id))
            .Where(k => k is not null).Select(k => k!).ToList();

        var rows = await GetUnitExportRowsAsync(siteId, dto.BorcluRolTercihi, ct);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Borçlandırma");

        const int refColCount = 6; // Blok, Daire, Tipi, Arsa Payı, Metrekare, Kişi
        var headers = new List<string> { "Blok", "Daire", "Tipi", "Arsa Payı", "Metrekare", "Kişi" };
        foreach (var k in kalemler)
        {
            headers.Add($"{k.Name} Tutar");
            headers.Add($"{k.Name} Açıklaması");
        }

        for (var i = 0; i < headers.Count; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            cell.Style.Protection.SetLocked(true);
        }

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            var excelRow = r + 2;
            ws.Cell(excelRow, 1).Value = row.BuildingName ?? "";
            ws.Cell(excelRow, 2).Value = row.DoorNumber;
            ws.Cell(excelRow, 3).Value = row.UnitTypeName ?? "";
            ws.Cell(excelRow, 4).Value = row.LandShare;
            ws.Cell(excelRow, 5).Value = row.GrossArea;
            ws.Cell(excelRow, 6).Value = row.PersonAdSoyad ?? "";
            for (var c = 1; c <= refColCount; c++)
                ws.Cell(excelRow, c).Style.Protection.SetLocked(true);

            for (var k = 0; k < kalemler.Count; k++)
            {
                var tutarCol = refColCount + 1 + k * 2;
                var aciklamaCol = tutarCol + 1;
                ws.Cell(excelRow, tutarCol).Value = 0;
                ws.Cell(excelRow, tutarCol).Style.Protection.SetLocked(false);
                ws.Cell(excelRow, aciklamaCol).Value = "";
                ws.Cell(excelRow, aciklamaCol).Style.Protection.SetLocked(false);
            }
        }

        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();
        ws.Protect();

        // ── Gizli meta sheet ─────────────────────────────────────────────────
        var meta = wb.Worksheets.Add(MetaSheetName);
        meta.Cell(1, 1).Value = "Donem";
        meta.Cell(1, 2).Value = dto.Donem ?? "";
        meta.Cell(2, 1).Value = "SonOdemeTarihi";
        meta.Cell(2, 2).Value = dto.SonOdemeTarihi?.ToString("O") ?? "";

        meta.Cell(4, 1).Value = "TutarKolonBasligi";
        meta.Cell(4, 2).Value = "GelirTanimiId";
        for (var k = 0; k < kalemler.Count; k++)
        {
            meta.Cell(5 + k, 1).Value = $"{kalemler[k].Name} Tutar";
            meta.Cell(5 + k, 2).Value = kalemler[k].Id.ToString();
        }

        var rowMapStartRow = 5 + kalemler.Count + 2;
        meta.Cell(rowMapStartRow, 1).Value = "SatirNo";
        meta.Cell(rowMapStartRow, 2).Value = "UnitId";
        meta.Cell(rowMapStartRow, 3).Value = "PersonUserId";
        meta.Cell(rowMapStartRow, 4).Value = "PersonRol";
        for (var r = 0; r < rows.Count; r++)
        {
            var metaRow = rowMapStartRow + 1 + r;
            meta.Cell(metaRow, 1).Value = r + 2; // ana sheet'teki excel satır no
            meta.Cell(metaRow, 2).Value = rows[r].UnitId.ToString();
            meta.Cell(metaRow, 3).Value = rows[r].PersonUserId?.ToString() ?? "";
            meta.Cell(metaRow, 4).Value = rows[r].PersonRol?.ToString() ?? "";
        }
        meta.Hide();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private record UnitExportRow(
        Guid UnitId, string DoorNumber, string? BuildingName, string? UnitTypeName,
        decimal? LandShare, decimal? GrossArea,
        Guid? PersonUserId, string? PersonAdSoyad, UserType? PersonRol);

    private async Task<List<UnitExportRow>> GetUnitExportRowsAsync(Guid siteId, UserType borcluRolTercihi, CancellationToken ct)
    {
        var units = await _db.Units
            .Include(u => u.Building)
            .Include(u => u.UnitType)
            .Where(u => u.SiteId == siteId)
            .OrderBy(u => u.Building.Name).ThenBy(u => u.DoorNumber)
            .ToListAsync(ct);

        var unitIds = units.Select(u => u.Id).ToList();
        var acikKayitlar = await _db.PersonUnitHistories
            .Where(x => x.SiteId == siteId && unitIds.Contains(x.UnitId) && x.ExitDate == null)
            .ToListAsync(ct);

        var ownerByUnit = acikKayitlar.Where(x => x.Role == UserType.Owner).ToDictionary(x => x.UnitId, x => x.PersonUserId);
        var tenantByUnit = acikKayitlar.Where(x => x.Role == UserType.Renter).ToDictionary(x => x.UnitId, x => x.PersonUserId);

        var personIds = new HashSet<Guid>();
        foreach (var u in units)
        {
            var ownerId = ownerByUnit.GetValueOrDefault(u.Id, u.OwnerUserId ?? Guid.Empty);
            var tenantId = tenantByUnit.GetValueOrDefault(u.Id, u.TenantUserId ?? Guid.Empty);
            if (ownerId != Guid.Empty) personIds.Add(ownerId);
            if (tenantId != Guid.Empty) personIds.Add(tenantId);
        }

        var users = await _masterDb.Users
            .Where(u => personIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, ct);

        var result = new List<UnitExportRow>();
        foreach (var u in units)
        {
            Guid? ownerId = ownerByUnit.TryGetValue(u.Id, out var oId) ? oId : u.OwnerUserId;
            Guid? tenantId = tenantByUnit.TryGetValue(u.Id, out var tId) ? tId : u.TenantUserId;

            Guid? personId;
            UserType? personRol;
            if (borcluRolTercihi == UserType.Renter)
            {
                personId = tenantId ?? ownerId;
                personRol = tenantId.HasValue ? UserType.Renter : (ownerId.HasValue ? UserType.Owner : null);
            }
            else
            {
                personId = ownerId ?? tenantId;
                personRol = ownerId.HasValue ? UserType.Owner : (tenantId.HasValue ? UserType.Renter : null);
            }

            string? personAdSoyad = personId.HasValue && users.TryGetValue(personId.Value, out var user)
                ? $"{user.FirstName} {user.LastName}" : null;

            result.Add(new UnitExportRow(
                u.Id, u.DoorNumber, u.Building?.Name, u.UnitType?.Name,
                u.LandShare, u.GrossArea, personId, personAdSoyad, personRol));
        }
        return result;
    }

    // ── Preview ──────────────────────────────────────────────────────────────

    public async Task<TopluBorclandirmaPreviewDto> PreviewAsync(Guid siteId, Stream file, CancellationToken ct)
    {
        using var wb = new XLWorkbook(file);
        var ws = wb.Worksheets.First(w => w.Name != MetaSheetName);
        var meta = wb.Worksheets.FirstOrDefault(w => w.Name == MetaSheetName);

        var satirHatalari = new List<string>();
        if (meta is null)
        {
            satirHatalari.Add("Dosyada beklenen meta bilgisi bulunamadı. Lütfen sistemden indirilen şablonu kullanın.");
            return new TopluBorclandirmaPreviewDto(0, 0, 0, null, null, new(), satirHatalari);
        }

        var donem = meta.Cell(1, 2).GetString().Trim();
        var sonOdemeTarihiStr = meta.Cell(2, 2).GetString().Trim();
        DateTime? sonOdemeTarihi = DateTime.TryParse(sonOdemeTarihiStr, CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var sot) ? sot : null;

        // Kolon başlığı → GelirTanimiId
        var colMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var r = 5;
        while (true)
        {
            var header = meta.Cell(r, 1).GetString().Trim();
            var idStr = meta.Cell(r, 2).GetString().Trim();
            if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(idStr)) break;
            if (Guid.TryParse(idStr, out var gid)) colMap[header] = gid;
            r++;
        }

        // Satır no → (UnitId, PersonUserId, PersonRol)
        var rowMapStart = r + 2; // header satırı sonra veriler
        var rowMap = new Dictionary<int, (Guid UnitId, Guid? PersonUserId, UserType? PersonRol)>();
        var mr = rowMapStart + 1;
        while (true)
        {
            var satirNoStr = meta.Cell(mr, 1).GetString().Trim();
            if (string.IsNullOrEmpty(satirNoStr)) break;
            if (int.TryParse(satirNoStr, out var satirNo) && Guid.TryParse(meta.Cell(mr, 2).GetString().Trim(), out var unitId))
            {
                var personUserIdStr = meta.Cell(mr, 3).GetString().Trim();
                var personRolStr = meta.Cell(mr, 4).GetString().Trim();
                Guid? personUserId = Guid.TryParse(personUserIdStr, out var pid) ? pid : null;
                UserType? personRol = Enum.TryParse<UserType>(personRolStr, out var rol) ? rol : null;
                rowMap[satirNo] = (unitId, personUserId, personRol);
            }
            mr++;
        }

        // Ana sheet başlık → kolon indeksi
        var headerRow = ws.Row(1);
        var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
            colIndex[cell.GetString().Trim()] = cell.Address.ColumnNumber;

        var gelirTanimiAdlari = colMap.Keys
            .Select(h => new { Header = h, Name = h.EndsWith(" Tutar") ? h[..^" Tutar".Length] : h })
            .ToDictionary(x => x.Header, x => x.Name);

        // Mükerrer kontrolü için mevcut kayıtları önden çek
        var existingSet = (await _db.BorcMakbuzlari
            .Where(x => x.SiteId == siteId && x.Donem == donem)
            .Select(x => new { x.UnitId, x.GelirTanimiId })
            .ToListAsync(ct))
            .Where(x => x.UnitId.HasValue && x.GelirTanimiId.HasValue)
            .Select(x => (x.UnitId!.Value, x.GelirTanimiId!.Value))
            .ToHashSet();

        var borcluIds = rowMap.Values.Where(v => v.PersonUserId.HasValue).Select(v => v.PersonUserId!.Value).Distinct().ToList();
        var users = await _masterDb.Users
            .Where(u => borcluIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, ct);

        var items = new List<TopluBorclandirmaPreviewItemDto>();
        var usedRows = ws.RowsUsed().Skip(1).ToList();
        foreach (var row in usedRows)
        {
            var excelRowNo = row.RowNumber();
            if (!rowMap.TryGetValue(excelRowNo, out var mapInfo))
            {
                satirHatalari.Add($"Satır {excelRowNo}: meta bilgisiyle eşleşmedi, atlandı (satır silinmiş/eklenmiş olabilir).");
                continue;
            }

            var unit = await _db.Units.Include(u => u.Building)
                .FirstOrDefaultAsync(u => u.Id == mapInfo.UnitId && u.SiteId == siteId, ct);
            if (unit is null)
            {
                satirHatalari.Add($"Satır {excelRowNo}: Daire bulunamadı.");
                continue;
            }

            foreach (var (header, gelirTanimiId) in colMap)
            {
                if (!colIndex.TryGetValue(header, out var tutarColIdx)) continue;
                var tutarStr = row.Cell(tutarColIdx).GetString().Trim();
                if (string.IsNullOrEmpty(tutarStr)) continue;
                if (!TryDecimal(tutarStr, out var tutar) || tutar <= 0) continue;

                var aciklamaHeader = $"{gelirTanimiAdlari[header]} Açıklaması";
                string? aciklama = null;
                if (colIndex.TryGetValue(aciklamaHeader, out var aciklamaColIdx))
                {
                    var v = row.Cell(aciklamaColIdx).GetString().Trim();
                    aciklama = string.IsNullOrEmpty(v) ? null : v;
                }

                var uyarilar = new List<string>();
                var mukerrer = existingSet.Contains((mapInfo.UnitId, gelirTanimiId));
                if (mukerrer) uyarilar.Add("Bu daire için aynı kalem ve dönemde zaten bir Borç Makbuzu var.");

                string? borcluAdSoyad = mapInfo.PersonUserId.HasValue && users.TryGetValue(mapInfo.PersonUserId.Value, out var u)
                    ? $"{u.FirstName} {u.LastName}" : null;
                if (mapInfo.PersonUserId is null)
                    uyarilar.Add("Bu daire için borçlu kişi bulunamadı (sahip/kiracı atanmamış).");

                items.Add(new TopluBorclandirmaPreviewItemDto(
                    mapInfo.UnitId, unit.DoorNumber, unit.Building?.Name,
                    gelirTanimiId, gelirTanimiAdlari[header],
                    tutar, aciklama,
                    mapInfo.PersonUserId, borcluAdSoyad, mapInfo.PersonRol,
                    mukerrer, uyarilar));
            }
        }

        return new TopluBorclandirmaPreviewDto(
            items.Count,
            items.Sum(x => x.Tutar),
            items.Count(x => x.Mukerrer),
            string.IsNullOrEmpty(donem) ? null : donem,
            sonOdemeTarihi,
            items,
            satirHatalari);
    }

    // ── Confirm ──────────────────────────────────────────────────────────────

    public async Task<Result<TopluBorclandirmaSonucDto>> ConfirmAsync(Guid siteId, TopluBorclandirmaConfirmDto dto, CancellationToken ct)
    {
        var validItems = dto.Items.Where(x => x.Tutar > 0).ToList();
        if (validItems.Count == 0)
            return Result<TopluBorclandirmaSonucDto>.Failure("Oluşturulacak geçerli bir borçlandırma kombinasyonu bulunamadı.");

        var borcluIds = validItems.Where(x => x.BorcluUserId.HasValue).Select(x => x.BorcluUserId!.Value).Distinct().ToList();
        var users = await _masterDb.Users
            .Where(u => borcluIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, ct);

        var baseCount = await _db.BorcMakbuzlari.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == siteId, ct);

        var batchId = Guid.NewGuid();
        var entities = new List<BorcMakbuzu>();
        for (var i = 0; i < validItems.Count; i++)
        {
            var item = validItems[i];
            string? borcluAdiSnapshot = item.BorcluUserId.HasValue && users.TryGetValue(item.BorcluUserId.Value, out var u)
                ? $"{u.FirstName} {u.LastName}" : null;

            entities.Add(new BorcMakbuzu
            {
                SiteId = siteId,
                EvrakNo = $"BM-{siteId.ToString()[..8].ToUpper()}-{baseCount + i + 1:D5}",
                IslemTarihi = DateTime.UtcNow,
                Donem = dto.Donem,
                SonOdemeTarihi = dto.SonOdemeTarihi,
                UnitId = item.UnitId,
                BorcluUserId = item.BorcluUserId,
                BorcluRol = item.BorcluRol,
                BorcluAdiSnapshot = borcluAdiSnapshot,
                GelirTanimiId = item.GelirTanimiId,
                Tutar = item.Tutar,
                Aciklama = item.Aciklama,
                TopluBorclandirmaBatchId = batchId
            });
        }

        _db.BorcMakbuzlari.AddRange(entities);
        await _db.SaveChangesAsync(ct);

        return Result<TopluBorclandirmaSonucDto>.Success(new TopluBorclandirmaSonucDto(batchId, entities.Count));
    }

    private static bool TryDecimal(string? s, out decimal v)
    {
        s = (s ?? "").Trim().Replace(',', '.');
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }
}
