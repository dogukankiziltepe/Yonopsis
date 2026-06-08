using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record ImportPaymentsCommand(
    Guid SiteId,
    Stream FileStream,
    DateTime DueDate,
    string? Description,
    bool DryRun = false) : IRequest<Result<ImportPaymentsResult>>;

public record ImportPreviewRow(
    string BuildingName,
    string DoorNumber,
    List<PaymentItemDto> Items,
    decimal Total);

public record ImportSkippedRow(
    string BuildingName,
    string DoorNumber,
    string Reason);

public record ImportPaymentsResult(
    int Created,
    int Skipped,
    List<string> Errors,
    List<ImportPreviewRow> Preview,
    List<ImportSkippedRow> SkippedRows);

public class ImportPaymentsCommandHandler : IRequestHandler<ImportPaymentsCommand, Result<ImportPaymentsResult>>
{
    private readonly SharedTenantDbContext _db;
    public ImportPaymentsCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<ImportPaymentsResult>> Handle(ImportPaymentsCommand request, CancellationToken cancellationToken)
    {
        List<string> errors = new();
        List<ImportPreviewRow> preview = new();
        List<ImportSkippedRow> skippedRows = new();

        XLWorkbook wb;
        try { wb = new XLWorkbook(request.FileStream); }
        catch { return Result<ImportPaymentsResult>.Failure("Geçersiz Excel dosyası."); }

        using (wb)
        {
            var ws = wb.Worksheets.FirstOrDefault();
            if (ws == null)
                return Result<ImportPaymentsResult>.Failure("Excel dosyasında sayfa bulunamadı.");

            var itemNames = new List<string>();
            int col = 3;
            while (true)
            {
                var cell = ws.Cell(1, col);
                if (cell.IsEmpty()) break;
                var name = cell.GetString().Trim();
                if (string.IsNullOrEmpty(name)) break;
                itemNames.Add(name);
                col++;
            }

            if (itemNames.Count == 0)
                return Result<ImportPaymentsResult>.Failure("Excel başlık satırında kalem bulunamadı.");

            var buildings = await _db.Buildings
                .Where(b => b.SiteId == request.SiteId)
                .ToDictionaryAsync(b => b.Name.Trim().ToLowerInvariant(), b => b.Id, cancellationToken);

            var unitMap = await _db.Units
                .Where(u => u.SiteId == request.SiteId)
                .ToListAsync(cancellationToken);

            var monthStart = new DateTime(request.DueDate.Year, request.DueDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            var existingUnitIds = await _db.Payments
                .Where(p => p.SiteId == request.SiteId && p.DueDate >= monthStart && p.DueDate < monthEnd)
                .Select(p => p.UnitId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var toCreate = new List<Payment>();
            int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int row = 2; row <= lastRow; row++)
            {
                var buildingName = ws.Cell(row, 1).GetString().Trim();
                var doorNumber = ws.Cell(row, 2).GetString().Trim();

                if (string.IsNullOrEmpty(buildingName) && string.IsNullOrEmpty(doorNumber)) continue;

                if (!buildings.TryGetValue(buildingName.ToLowerInvariant(), out var buildingId))
                {
                    errors.Add($"Satır {row}: '{buildingName}' adlı blok bulunamadı.");
                    continue;
                }

                var unit = unitMap.FirstOrDefault(u =>
                    u.BuildingId == buildingId &&
                    u.DoorNumber.Equals(doorNumber, StringComparison.OrdinalIgnoreCase));

                if (unit == null)
                {
                    errors.Add($"Satır {row}: '{buildingName}' / '{doorNumber}' daire bulunamadı.");
                    continue;
                }

                if (existingUnitIds.Contains(unit.Id))
                {
                    skippedRows.Add(new ImportSkippedRow(buildingName, doorNumber, "Bu ay için zaten aidat kaydı mevcut."));
                    continue;
                }

                var itemDtos = new List<PaymentItemDto>();
                for (int i = 0; i < itemNames.Count; i++)
                {
                    var cell = ws.Cell(row, 3 + i);
                    if (cell.IsEmpty()) continue;
                    decimal amount;
                    if (!decimal.TryParse(cell.GetString().Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out amount))
                    {
                        try { amount = (decimal)cell.GetDouble(); } catch { continue; }
                    }
                    if (amount <= 0) continue;
                    itemDtos.Add(new PaymentItemDto(itemNames[i], amount));
                }

                if (itemDtos.Count == 0)
                {
                    skippedRows.Add(new ImportSkippedRow(buildingName, doorNumber, "Tüm kalemler boş veya sıfır."));
                    continue;
                }

                var total = itemDtos.Sum(i => i.Amount);
                preview.Add(new ImportPreviewRow(buildingName, doorNumber, itemDtos, total));

                toCreate.Add(new Payment
                {
                    SiteId = request.SiteId,
                    UnitId = unit.Id,
                    Amount = total,
                    DueDate = request.DueDate,
                    Description = request.Description,
                    Items = itemDtos.Select(i => new PaymentItem { Name = i.Name, Amount = i.Amount }).ToList()
                });
            }

            if (!request.DryRun && toCreate.Count > 0)
            {
                _db.Payments.AddRange(toCreate);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return Result<ImportPaymentsResult>.Success(new ImportPaymentsResult(
                request.DryRun ? 0 : toCreate.Count,
                skippedRows.Count,
                errors,
                request.DryRun ? preview : new List<ImportPreviewRow>(),
                request.DryRun ? skippedRows : new List<ImportSkippedRow>()));
        }
    }
}
