using ClosedXML.Excel;
using SiteYonetimi.SiteManagement.Import.DTOs;

namespace SiteYonetimi.SiteManagement.Import.Services;

public class ExcelTemplateService
{
    public byte[] CreateBuildingsTemplate()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Binalar");

        var headers = new[] { "BuildingName", "TotalFloors", "Address", "Description" };
        SetHeaders(ws, headers);

        ws.Cell(2, 1).Value = "A Blok";
        ws.Cell(2, 2).Value = 5;
        ws.Cell(2, 3).Value = "Örnek Sokak No:1, İstanbul";
        ws.Cell(2, 4).Value = "Açıklama (opsiyonel)";

        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] CreateUnitsTemplate()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Daireler");

        var headers = new[] { "BuildingName", "FloorNumber", "UnitNumber", "UnitType", "SquareMeters", "Description" };
        SetHeaders(ws, headers);

        ws.Cell(2, 1).Value = "A Blok";
        ws.Cell(2, 2).Value = 3;
        ws.Cell(2, 3).Value = "A-301";
        ws.Cell(2, 4).Value = "3+1";
        ws.Cell(2, 5).Value = 120.5;
        ws.Cell(2, 6).Value = "Açıklama (opsiyonel)";

        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public byte[] CreateUsersTemplate()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Kullanıcılar");

        var headers = new[] { "FirstName", "LastName", "Email", "Phone", "UnitNumber", "BuildingName", "Role" };
        SetHeaders(ws, headers);

        ws.Cell(2, 1).Value = "Ahmet";
        ws.Cell(2, 2).Value = "Yılmaz";
        ws.Cell(2, 3).Value = "ahmet@example.com";
        ws.Cell(2, 4).Value = "5001234567";
        ws.Cell(2, 5).Value = "A-301";
        ws.Cell(2, 6).Value = "A Blok";
        ws.Cell(2, 7).Value = "Owner";

        var noteCell = ws.Cell(3, 7);
        noteCell.Value = "Role değerleri: Resident, Owner, Manager";
        noteCell.Style.Font.Italic = true;
        noteCell.Style.Font.FontColor = XLColor.Gray;

        ws.Columns().AdjustToContents();
        return ToBytes(wb);
    }

    public List<(BuildingImportRowData Data, int RowIndex)> ParseBuildingsFile(byte[] fileBytes)
    {
        var result = new List<(BuildingImportRowData, int)>();
        using var ms = new MemoryStream(fileBytes);
        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var row = new BuildingImportRowData(
                BuildingName: GetString(ws, r, 1),
                TotalFloors: GetInt(ws, r, 2),
                Address: GetString(ws, r, 3),
                Description: GetString(ws, r, 4)
            );
            result.Add((row, r - 1));
        }

        return result;
    }

    public List<(UnitImportRowData Data, int RowIndex)> ParseUnitsFile(byte[] fileBytes)
    {
        var result = new List<(UnitImportRowData, int)>();
        using var ms = new MemoryStream(fileBytes);
        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var row = new UnitImportRowData(
                BuildingName: GetString(ws, r, 1),
                FloorNumber: GetInt(ws, r, 2),
                UnitNumber: GetString(ws, r, 3),
                UnitType: GetString(ws, r, 4),
                SquareMeters: GetDecimal(ws, r, 5),
                Description: GetString(ws, r, 6)
            );
            result.Add((row, r - 1));
        }

        return result;
    }

    public List<(UserImportRowData Data, int RowIndex)> ParseUsersFile(byte[] fileBytes)
    {
        var result = new List<(UserImportRowData, int)>();
        using var ms = new MemoryStream(fileBytes);
        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        int lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var row = new UserImportRowData(
                FirstName: GetString(ws, r, 1),
                LastName: GetString(ws, r, 2),
                Email: GetString(ws, r, 3),
                Phone: GetString(ws, r, 4),
                UnitNumber: GetString(ws, r, 5),
                BuildingName: GetString(ws, r, 6),
                Role: GetString(ws, r, 7)
            );
            result.Add((row, r - 1));
        }

        return result;
    }

    private static void SetHeaders(IXLWorksheet ws, string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3B82F6");
            cell.Style.Font.FontColor = XLColor.White;
        }
    }

    private static byte[] ToBytes(XLWorkbook wb)
    {
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private static string? GetString(IXLWorksheet ws, int row, int col)
    {
        var cell = ws.Cell(row, col);
        if (cell.IsEmpty()) return null;
        var val = cell.GetString();
        return string.IsNullOrWhiteSpace(val) ? null : val.Trim();
    }

    private static int? GetInt(IXLWorksheet ws, int row, int col)
    {
        var cell = ws.Cell(row, col);
        if (cell.IsEmpty()) return null;
        try { return (int)cell.GetDouble(); }
        catch { return null; }
    }

    private static decimal? GetDecimal(IXLWorksheet ws, int row, int col)
    {
        var cell = ws.Cell(row, col);
        if (cell.IsEmpty()) return null;
        try { return (decimal)cell.GetDouble(); }
        catch { return null; }
    }
}
