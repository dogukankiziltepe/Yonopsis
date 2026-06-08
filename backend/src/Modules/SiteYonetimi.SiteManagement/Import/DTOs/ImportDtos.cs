namespace SiteYonetimi.SiteManagement.Import.DTOs;

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

public record ConfirmBuildingsImportDto(List<BuildingImportRowData> Rows);
public record ConfirmUnitsImportDto(List<UnitImportRowData> Rows);
public record ConfirmUsersImportDto(List<UserImportRowData> Rows);

public record ImportConfirmResult(int SavedCount, int SkippedCount, List<string> Errors);
