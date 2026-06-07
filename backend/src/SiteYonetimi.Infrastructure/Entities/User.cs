using SiteYonetimi.Shared.Entities;

namespace SiteYonetimi.Infrastructure.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; } = false;
    public bool MustChangePassword { get; set; } = false;

    // Navigation
    public ICollection<UserSite> UserSites { get; set; } = new List<UserSite>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
