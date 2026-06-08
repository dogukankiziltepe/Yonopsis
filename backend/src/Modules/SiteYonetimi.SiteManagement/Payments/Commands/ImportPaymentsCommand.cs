using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record ImportPaymentsCommand(
    Guid SiteId,
    Stream FileStream,
    DateTime DueDate,
    string? Description) : IRequest<Result<ImportPaymentsResult>>;

public record ImportPaymentsResult(int Created, int Skipped, List<string> Errors);

public class ImportPaymentsCommandHandler : IRequestHandler<ImportPaymentsCommand, Result<ImportPaymentsResult>>
{
    private readonly SharedTenantDbContext _db;
    public ImportPaymentsCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<ImportPaymentsResult>> Handle(ImportPaymentsCommand request, CancellationToken cancellationToken)
    {
        List<string> errors = new();

        XLWorkbook wb;
        try { wb = new XLWorkbook(request.FileStream); }
        catch { return Result<ImportPaymentsResult>.Failure("Geçersiz Excel dosyası."); }

        using (wb)
        {
            var ws = wb.Worksheets.FirstOrDefault();
            if (ws == null)
                return Result<ImportPaymentsResult>.Failure("Excel dosyasında sayfa bulunamadı.");

            // Header satırından kalem isimlerini oku (3. sütundan itibaren)
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

            // Mevcut bina/daire haritasını çek
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
            int skipped = 0;
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
                    errors.Add($"Satır {row}: '{buildingName}' bloğunda '{doorNumber}' kapı numaralı daire bulunamadı.");
                    continue;
                }

                if (existingUnitIds.Contains(unit.Id))
                {
                    skipped++;
                    continue;
                }

                var items = new List<PaymentItem>();
                for (int i = 0; i < itemNames.Count; i++)
                {
                    var cell = ws.Cell(row, 3 + i);
                    if (cell.IsEmpty()) continue;
                    if (!decimal.TryParse(cell.GetString().Replace(",", "."), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var amount))
                    {
                        // ClosedXML numeric value
                        try { amount = (decimal)cell.GetDouble(); } catch { continue; }
                    }
                    if (amount <= 0) continue;
                    items.Add(new PaymentItem { Name = itemNames[i], Amount = amount });
                }

                if (items.Count == 0)
                {
                    skipped++;
                    continue;
                }

                toCreate.Add(new Payment
                {
                    SiteId = request.SiteId,
                    UnitId = unit.Id,
                    Amount = items.Sum(i => i.Amount),
                    DueDate = request.DueDate,
                    Description = request.Description,
                    Items = items
                });
            }

            if (toCreate.Count > 0)
            {
                _db.Payments.AddRange(toCreate);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return Result<ImportPaymentsResult>.Success(
                new ImportPaymentsResult(toCreate.Count, skipped, errors));
        }
    }
}
