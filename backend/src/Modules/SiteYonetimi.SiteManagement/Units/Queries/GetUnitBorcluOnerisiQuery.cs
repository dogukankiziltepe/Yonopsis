using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Units.Queries;

public record UnitBorcluOnerisiDto(
    Guid? OwnerUserId,
    string? OwnerAdSoyad,
    Guid? TenantUserId,
    string? TenantAdSoyad,
    Guid? OnerilenPersonId,
    string? OnerilenAdSoyad,
    UserType? OnerilenRol);

public record GetUnitBorcluOnerisiQuery(Guid UnitId, Guid SiteId) : IRequest<Result<UnitBorcluOnerisiDto>>;

/// <summary>
/// Borç Makbuzu formunda Daire seçilince Borçlu alanını otomatik doldurmak için hafif bir sorgu.
/// GetUnitFullDetailQuery'nin Muhasebe bakiye hesaplaması gibi form için gereksiz kısımları
/// olmadan sadece aktif Sahip/Kiracı bilgisini döner (her Daire seçiminde tetiklenir).
/// </summary>
public class GetUnitBorcluOnerisiQueryHandler : IRequestHandler<GetUnitBorcluOnerisiQuery, Result<UnitBorcluOnerisiDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetUnitBorcluOnerisiQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<UnitBorcluOnerisiDto>> Handle(GetUnitBorcluOnerisiQuery request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units
            .FirstOrDefaultAsync(x => x.Id == request.UnitId && x.SiteId == request.SiteId, cancellationToken);
        if (unit is null) return Result<UnitBorcluOnerisiDto>.Failure("Daire bulunamadı.");

        Guid? ownerUserId = unit.OwnerUserId;
        Guid? tenantUserId = unit.TenantUserId;

        var acikKayitlar = await _db.PersonUnitHistories
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId && x.ExitDate == null)
            .ToListAsync(cancellationToken);

        var ownerKayit = acikKayitlar.FirstOrDefault(x => x.Role == UserType.Owner);
        var tenantKayit = acikKayitlar.FirstOrDefault(x => x.Role == UserType.Renter);
        if (ownerKayit is not null) ownerUserId = ownerKayit.PersonUserId;
        if (tenantKayit is not null) tenantUserId = tenantKayit.PersonUserId;

        var ids = new List<Guid>();
        if (ownerUserId.HasValue) ids.Add(ownerUserId.Value);
        if (tenantUserId.HasValue) ids.Add(tenantUserId.Value);

        var users = await _masterDb.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        string? ownerAdSoyad = ownerUserId.HasValue && users.TryGetValue(ownerUserId.Value, out var ownerUser)
            ? $"{ownerUser.FirstName} {ownerUser.LastName}" : null;
        string? tenantAdSoyad = tenantUserId.HasValue && users.TryGetValue(tenantUserId.Value, out var tenantUser)
            ? $"{tenantUser.FirstName} {tenantUser.LastName}" : null;

        // Öneri kuralı: Kiracı varsa Kiracı, yoksa Sahip.
        Guid? onerilenPersonId = tenantUserId ?? ownerUserId;
        string? onerilenAdSoyad = tenantUserId.HasValue ? tenantAdSoyad : ownerAdSoyad;
        UserType? onerilenRol = tenantUserId.HasValue ? UserType.Renter : (ownerUserId.HasValue ? UserType.Owner : null);

        return Result<UnitBorcluOnerisiDto>.Success(new UnitBorcluOnerisiDto(
            ownerUserId, ownerAdSoyad, tenantUserId, tenantAdSoyad,
            onerilenPersonId, onerilenAdSoyad, onerilenRol));
    }
}
