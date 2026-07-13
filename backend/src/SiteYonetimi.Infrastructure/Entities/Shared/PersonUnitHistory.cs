using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PersonUnitHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    public Guid UnitId { get; set; }
    // Cross-context ref (MasterDb User.Id) — no navigation property.
    public Guid PersonUserId { get; set; }
    // Reuses UserType enum (Owner/Renter) instead of a new Malik/Kiracı enum.
    public UserType Role { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExitDate { get; set; }
    public string? ContactPerson { get; set; }
    public string? Notes { get; set; }
    public string? BankPaymentCode { get; set; }
    public decimal? SharePercentage { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Unit Unit { get; set; } = null!;
}
