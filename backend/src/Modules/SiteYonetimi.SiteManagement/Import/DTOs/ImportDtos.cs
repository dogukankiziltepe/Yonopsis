namespace SiteYonetimi.SiteManagement.Import.DTOs;

public enum ImportType
{
    Buildings,
    Units,
    Users
}

// ─── Row data records (parsed from Excel) ─────────────────────────────────────

public record BuildingImportRowData(
    string? BuildingName,
    int? TotalFloors,
    string? Address,
    string? Description
);

public record UnitImportRowData(
    string? BuildingName,
    int? FloorNumber,
    string? UnitNumber,
    string? UnitType,
    decimal? SquareMeters,
    string? Description
);

public record UserImportRowData(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? UnitNumber,
    string? BuildingName,
    string? Role
);

// ─── Preview result types ──────────────────────────────────────────────────────

public record ImportPreviewRow(
    int RowIndex,
    Dictionary<string, object?> Data,
    bool IsValid,
    List<string> Errors
);

public record ImportPreviewResult(
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    List<ImportPreviewRow> Rows
);

// ─── Confirm result ────────────────────────────────────────────────────────────

public record ImportConfirmResult(
    int Saved,
    int Skipped,
    List<string> Errors
);

// ─── Legacy DTOs (kept for controller compatibility) ──────────────────────────

/// <summary>Tek satırın parse + validation sonucu.</summary>
public class ImportRowResultDto
{
    public int RowIndex { get; set; }
    public Dictionary<string, string?> Data { get; set; } = new();
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>Önizleme yanıtı.</summary>
public class ImportPreviewDto
{
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public List<ImportRowResultDto> Rows { get; set; } = new();
}

/// <summary>Onay isteği — önizlemeden gelen satır verileri.</summary>
public class ConfirmImportDto
{
    public List<Dictionary<string, string?>> Rows { get; set; } = new();
}

/// <summary>Onay (kayıt) sonucu.</summary>
public class ImportResultDto
{
    public int SavedRows { get; set; }
    public int SkippedRows { get; set; }
    public List<string> Errors { get; set; } = new();
}
