namespace SiteYonetimi.SiteManagement.Import.DTOs;

public enum ImportType
{
    Buildings,
    Units,
    Users
}

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
