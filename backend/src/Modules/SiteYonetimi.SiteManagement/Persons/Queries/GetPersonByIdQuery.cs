using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;
using SiteYonetimi.SiteManagement.Units.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Queries;

public record GetPersonByIdQuery(Guid UserSiteId, Guid SiteId) : IRequest<Result<PersonDetailDto>>;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<PersonDetailDto>>
{
    private readonly MasterDbContext _masterDb;
    private readonly SharedTenantDbContext _db;

    public GetPersonByIdQueryHandler(MasterDbContext masterDb, SharedTenantDbContext db)
    {
        _masterDb = masterDb;
        _db = db;
    }

    public async Task<Result<PersonDetailDto>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var us = await _masterDb.UserSites
            .Include(x => x.User)
            .Include(x => x.RoleType)
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result<PersonDetailDto>.Failure("Kişi bulunamadı.");

        var units = await _db.Units
            .Include(x => x.Building)
            .Include(x => x.UnitType)
            .Where(x => x.SiteId == request.SiteId &&
                        (x.OwnerUserId == us.UserId || x.TenantUserId == us.UserId))
            .Select(x => new UnitSummaryDto(
                x.Id,
                x.BuildingId,
                x.Building.Name,
                x.UnitTypeId,
                x.UnitType != null ? x.UnitType.Name : null,
                x.DoorNumber,
                x.Code,
                x.Floor,
                x.Status,
                x.MonthlyFee,
                x.HasDask,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        var dto = new PersonDetailDto(
            us.UserId,
            us.Id,
            us.User.FirstName,
            us.User.LastName,
            us.User.Email,
            us.User.PhoneNumber,
            us.User.NationalId,
            us.User.Gender,
            us.UserType,
            us.RoleTypeId,
            us.RoleType?.Name,
            us.Status,
            us.User.IsActive,
            units);

        return Result<PersonDetailDto>.Success(dto);
    }
}
