using System.Globalization;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.BankaHareketleri.ExcelImport.DTOs;

namespace SiteYonetimi.SiteManagement.BankaHareketleri.ExcelImport.Services;

public interface IBankaHareketiExcelImportService
{
    byte[] GenerateTemplate();
    Task<BankaHareketiImportPreviewDto> PreviewAsync(Stream file, CancellationToken ct);
    Task<Result<BankaHareketiImportSonucDto>> ConfirmAsync(Guid siteId, Guid kasaBankaId, BankaHareketiImportConfirmDto dto, CancellationToken ct);
}

/// <summary>
/// Banka ekstresi Excel içe aktarma — TopluBorclandirmaService'in küçültülmüş hali
/// (sabit kolonlar, tek sheet, template/preview/confirm akışı).
/// </summary>
public class BankaHareketiExcelImportService : IBankaHareketiExcelImportService
{
    private static readonly string[] Columns = { "Tarih", "Açıklama", "Referans No", "Tutar" };

    private readonly SharedTenantDbContext _db;
    public BankaHareketiExcelImportService(SharedTenantDbContext db) => _db = db;

    public byte[] GenerateTemplate()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Banka Hareketleri");
        for (var i = 0; i < Columns.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = Columns[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        ws.SheetView.FreezeRows(1);
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public Task<BankaHareketiImportPreviewDto> PreviewAsync(Stream file, CancellationToken ct)
    {
        using var wb = new XLWorkbook(file);
        var ws = wb.Worksheets.First();
        var usedRows = ws.RowsUsed().ToList();

        var satirlar = new List<BankaHareketiImportRowDto>();
        if (usedRows.Count <= 1) return Task.FromResult(new BankaHareketiImportPreviewDto(0, 0, satirlar));

        var headerRow = usedRows[0];
        var colIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
            colIndex[cell.GetString().Trim()] = cell.Address.ColumnNumber;

        var rowNo = 1;
        foreach (var row in usedRows.Skip(1))
        {
            rowNo++;
            var hatalar = new List<string>();

            DateTime? tarih = null;
            if (colIndex.TryGetValue("Tarih", out var tarihCol))
            {
                var raw = row.Cell(tarihCol).GetString().Trim();
                if (string.IsNullOrEmpty(raw)) hatalar.Add("Tarih zorunludur.");
                else if (DateTime.TryParse(raw, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out var d)) tarih = d;
                else if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2)) tarih = d2;
                else hatalar.Add("Tarih formatı geçersiz.");
            }

            string? aciklama = colIndex.TryGetValue("Açıklama", out var acCol) ? row.Cell(acCol).GetString().Trim() : null;
            if (string.IsNullOrWhiteSpace(aciklama)) hatalar.Add("Açıklama zorunludur.");

            string? referansNo = colIndex.TryGetValue("Referans No", out var refCol) ? row.Cell(refCol).GetString().Trim() : null;
            if (string.IsNullOrEmpty(referansNo)) referansNo = null;

            decimal? tutar = null;
            if (colIndex.TryGetValue("Tutar", out var tutarCol))
            {
                var raw = row.Cell(tutarCol).GetString().Trim();
                if (string.IsNullOrEmpty(raw)) hatalar.Add("Tutar zorunludur.");
                else if (TryDecimal(raw, out var t)) tutar = t;
                else hatalar.Add("Tutar sayısal olmalı.");
            }

            var hasAnyValue = tarih.HasValue || !string.IsNullOrWhiteSpace(aciklama) || tutar.HasValue;
            if (!hasAnyValue) continue;

            satirlar.Add(new BankaHareketiImportRowDto(rowNo, tarih, aciklama, referansNo, tutar, hatalar.Count == 0, hatalar));
        }

        return Task.FromResult(new BankaHareketiImportPreviewDto(satirlar.Count, satirlar.Count(x => x.IsValid), satirlar));
    }

    public async Task<Result<BankaHareketiImportSonucDto>> ConfirmAsync(Guid siteId, Guid kasaBankaId, BankaHareketiImportConfirmDto dto, CancellationToken ct)
    {
        var kasaExists = await _db.KasaBanka.AnyAsync(k => k.Id == kasaBankaId && k.SiteId == siteId, ct);
        if (!kasaExists) return Result<BankaHareketiImportSonucDto>.Failure("Kasa/Banka bulunamadı.");

        var validItems = dto.Items.Where(x => x.Tutar != 0 && !string.IsNullOrWhiteSpace(x.Aciklama)).ToList();
        if (validItems.Count == 0)
            return Result<BankaHareketiImportSonucDto>.Failure("Aktarılacak geçerli satır bulunamadı.");

        var entities = validItems.Select(x => new BankaHareketi
        {
            SiteId = siteId,
            KasaBankaId = kasaBankaId,
            Tarih = x.Tarih,
            Aciklama = x.Aciklama,
            ReferansNo = x.ReferansNo,
            Tutar = x.Tutar,
            Durum = BankaHareketiDurum.Bekleyen
        }).ToList();

        _db.BankaHareketleri.AddRange(entities);
        await _db.SaveChangesAsync(ct);

        return Result<BankaHareketiImportSonucDto>.Success(new BankaHareketiImportSonucDto(entities.Count));
    }

    private static bool TryDecimal(string? s, out decimal v)
    {
        s = (s ?? "").Trim().Replace(',', '.');
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v);
    }
}
