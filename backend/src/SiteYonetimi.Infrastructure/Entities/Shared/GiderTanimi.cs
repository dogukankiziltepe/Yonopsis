using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class GiderTanimi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public string GiderKodu { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? GiderGrubuId { get; set; }
    public DagitimSekli? DagitimSekli { get; set; }
    public bool BosDairelereDagit { get; set; } = false;
    public int? Kdv { get; set; }
    public CariTuru? BorclandirilacakKisi { get; set; }
    public string? MuhasebeKodu { get; set; }
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public GiderGrubu? GiderGrubu { get; set; }
}
