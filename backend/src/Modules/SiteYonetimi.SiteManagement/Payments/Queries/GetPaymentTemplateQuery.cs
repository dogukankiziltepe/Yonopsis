using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Payments.Queries;

public record GetPaymentTemplateQuery(Guid SiteId, List<string> Items) : IRequest<Result<byte[]>>;

public class GetPaymentTemplateQueryHandler : IRequestHandler<GetPaymentTemplateQuery, Result<byte[]>>
{
    private readonly SharedTenantDbContext _db;
    public GetPaymentTemplateQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<byte[]>> Handle(GetPaymentTemplateQuery request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
            return Result<byte[]>.Failure("En az bir aidat kalemi belirtilmelidir.");

        var units = await _db.Units
            .Include(u => u.Building)
            .Where(u => u.SiteId == request.SiteId)
            .OrderBy(u => u.Building.Name)
            .ThenBy(u => u.DoorNumber)
            .Select(u => new { u.DoorNumber, BuildingName = u.Building.Name })
            .ToListAsync(cancellationToken);

        if (units.Count == 0)
            return Result<byte[]>.Failure("Sitede aktif daire bulunamadı.");

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Aidatlar");

        // Header satırı
        ws.Cell(1, 1).Value = "Blok";
        ws.Cell(1, 2).Value = "Kapı No";
        for (int i = 0; i < request.Items.Count; i++)
            ws.Cell(1, 3 + i).Value = request.Items[i];

        var headerRow = ws.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e3a5f");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Height = 18;

        // Daire satırları
        int row = 2;
        foreach (var unit in units)
        {
            ws.Cell(row, 1).Value = unit.BuildingName;
            ws.Cell(row, 2).Value = unit.DoorNumber;
            // Kalem sütunları boş bırakılır — kullanıcı doldurur
            for (int i = 0; i < request.Items.Count; i++)
            {
                var cell = ws.Cell(row, 3 + i);
                cell.Style.NumberFormat.Format = "#,##0.00";
            }
            if (row % 2 == 0)
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#f5f7fa");
            row++;
        }

        // Sütun genişlikleri
        ws.Column(1).Width = 22;
        ws.Column(2).Width = 12;
        for (int i = 0; i < request.Items.Count; i++)
            ws.Column(3 + i).Width = 16;

        // Bord korunması: Blok ve Kapı No değiştirilemesin
        ws.Protect("yonopsis");
        var dataRange = ws.Range(ws.Cell(2, 3), ws.Cell(row - 1, 2 + request.Items.Count));
        dataRange.Style.Protection.Locked = false;

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return Result<byte[]>.Success(ms.ToArray());
    }
}
