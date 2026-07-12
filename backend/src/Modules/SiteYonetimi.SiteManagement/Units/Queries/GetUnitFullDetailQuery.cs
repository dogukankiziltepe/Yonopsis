using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Units.Queries;

public record GetUnitFullDetailQuery(Guid UnitId, Guid SiteId) : IRequest<Result<UnitFullDetailDto>>;

public class GetUnitFullDetailQueryHandler : IRequestHandler<GetUnitFullDetailQuery, Result<UnitFullDetailDto>>
{
    private readonly MasterDbContext _masterDb;
    private readonly SharedTenantDbContext _db;

    public GetUnitFullDetailQueryHandler(MasterDbContext masterDb, SharedTenantDbContext db)
    {
        _masterDb = masterDb;
        _db = db;
    }

    public async Task<Result<UnitFullDetailDto>> Handle(GetUnitFullDetailQuery request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .FirstOrDefaultAsync(x => x.Id == request.UnitId && x.SiteId == request.SiteId, cancellationToken);

        if (unit is null)
            return Result<UnitFullDetailDto>.Failure("Daire bulunamadı.");

        var core = new UnitCoreDto(
            unit.Id,
            unit.DoorNumber,
            unit.Code,
            unit.BuildingId,
            unit.Building?.Name,
            unit.UnitTypeId,
            unit.UnitType?.Name,
            unit.GrossArea,
            unit.NetArea,
            unit.LandShare,
            unit.Floor,
            unit.Status,
            unit.MonthlyFee,
            unit.ParkingCount,
            unit.Direction,
            unit.Internet,
            unit.HasDask);

        var detail = new UnitDetailInfoDto(unit.Code2, unit.Code3, unit.DeliveryDate, unit.Description);

        var histories = await _db.PersonUnitHistories
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);

        var personIds = histories.Select(h => h.PersonUserId).Distinct().ToList();

        var users = await _masterDb.Users
            .Where(u => personIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var userSites = await _masterDb.UserSites
            .Where(us => us.SiteId == request.SiteId && personIds.Contains(us.UserId))
            .Select(us => new { us.UserId, us.PetType, us.PetDetail })
            .ToDictionaryAsync(us => us.UserId, cancellationToken);

        // Bakiye: GetBorcAlacakDurumuQueryHandler ile aynı mantık (Muhasebe modülüne
        // referans vermeden, modül sınırlarını korumak için burada tekrarlanır).
        var cariHesaplar = await _db.HesapPlani
            .Where(h => h.SiteId == request.SiteId && h.PersonId != null && personIds.Contains(h.PersonId!.Value))
            .Select(h => new { h.Id, PersonId = h.PersonId!.Value })
            .ToListAsync(cancellationToken);

        var cariIdler = cariHesaplar.Select(c => c.Id).ToList();

        var bakiyeToplamlari = await (
            from d in _db.MuhasebeFisiDetaylari
            join f in _db.MuhasebeFisleri on d.FisId equals f.Id
            where d.SiteId == request.SiteId && f.Durum == FisDurumu.Onaylandi && cariIdler.Contains(d.HesapId)
            group d by d.HesapId into g
            select new { HesapId = g.Key, Borc = g.Sum(x => x.BorcTutar), Alacak = g.Sum(x => x.AlacakTutar) }
        ).ToDictionaryAsync(x => x.HesapId, cancellationToken);

        var bakiyeByPerson = cariHesaplar.ToDictionary(
            c => c.PersonId,
            c =>
            {
                var t = bakiyeToplamlari.GetValueOrDefault(c.Id);
                return (t?.Borc ?? 0) - (t?.Alacak ?? 0);
            });

        var relatedPersons = histories.Select(h =>
        {
            var user = users.GetValueOrDefault(h.PersonUserId);
            var us = userSites.GetValueOrDefault(h.PersonUserId);
            var fullName = user is not null ? $"{user.FirstName} {user.LastName}" : "-";
            return new UnitRelatedPersonDto(
                h.Id,
                h.PersonUserId,
                h.Role,
                fullName,
                h.SharePercentage,
                us?.PetType,
                us?.PetDetail,
                h.EntryDate,
                h.ExitDate,
                h.ContactPerson,
                h.Notes,
                bakiyeByPerson.GetValueOrDefault(h.PersonUserId));
        }).ToList();

        var vehicleEntities = await _db.Vehicles
            .Where(x => x.SiteId == request.SiteId && x.UnitId == request.UnitId)
            .ToListAsync(cancellationToken);

        var ownerIds = vehicleEntities.Where(v => v.OwnerUserId.HasValue).Select(v => v.OwnerUserId!.Value).Distinct().ToList();
        var owners = await _masterDb.Users
            .Where(u => ownerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var vehicles = vehicleEntities.Select(v =>
        {
            var owner = v.OwnerUserId.HasValue ? owners.GetValueOrDefault(v.OwnerUserId.Value) : null;
            return new UnitVehicleDto(
                v.Id,
                v.Plate,
                owner is not null ? $"{owner.FirstName} {owner.LastName}" : null,
                v.Brand,
                v.Model,
                v.Color,
                v.IsActive);
        }).ToList();

        var dto = new UnitFullDetailDto(core, detail, relatedPersons, vehicles);

        return Result<UnitFullDetailDto>.Success(dto);
    }
}
