using SiteYonetimi.Shared.Entities;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Infrastructure.Entities;

public class UserSite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SiteId { get; set; }
    public UserType? UserType { get; set; }
    public Guid? RoleTypeId { get; set; } // Management için zorunlu, diğerleri için opsiyonel
    public UserSiteStatus Status { get; set; } = UserSiteStatus.Pending;

    // Genel Bilgiler
    public string? TaxOffice { get; set; }
    public string? SecondaryEmail { get; set; }
    public string? Address { get; set; }

    // Detay Bilgileri
    public string? Description { get; set; }
    public EducationStatus? EducationStatus { get; set; }
    public string? SchoolOrInstitution { get; set; }
    public string? Profession { get; set; }
    public bool HasPrivateInsurance { get; set; }
    public bool IsMartyrOrVeteranRelative { get; set; }
    public PetType? PetType { get; set; }
    public string? PetDetail { get; set; }

    // Kimlik Bilgileri
    public Nationality? Nationality { get; set; }
    public string? IdentitySeriNo { get; set; }
    public string? IdentitySiraNo { get; set; }
    public string? PassportNo { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? BirthPlace { get; set; }
    public DateTime? BirthDate { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    public string? RegisteredCity { get; set; }
    public string? RegisteredDistrict { get; set; }
    public string? RegisteredNeighborhood { get; set; }
    public string? FamilySiraNo { get; set; }
    public string? KayitSiraNo { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Site Site { get; set; } = null!;
    public RoleType? RoleType { get; set; }
    public ICollection<PersonPhone> Phones { get; set; } = new List<PersonPhone>();
}
