namespace SiteYonetimi.Auth.DTOs;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber);

// Login token yanıtı — site listesi içermez
public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpires,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    bool IsSuperAdmin,
    bool MustChangePassword);

// Site seçim listesi
public record UserSiteListDto(
    Guid SiteId,
    string SiteName,
    string? SiteLogo,
    List<string> UserTypes,
    bool IsSubscriptionActive,
    DateTime? SubscriptionEndDate,
    List<UserSiteApplicationDto> PendingApplications);

public record UserSiteApplicationDto(
    Guid UserSiteId,
    string UserType,
    string Status);

// Site seçim isteği
public record SelectSiteRequest(
    Guid SiteId,
    string UserType);

// Site token yanıtı
public record SelectSiteResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpires,
    Guid SiteId,
    string SiteName,
    string UserType,
    Guid? RoleTypeId,
    string? RoleName);

public record RefreshTokenRequest(string RefreshToken);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
