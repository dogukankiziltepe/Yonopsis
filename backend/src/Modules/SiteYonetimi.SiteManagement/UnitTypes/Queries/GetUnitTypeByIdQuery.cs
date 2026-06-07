using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.UnitTypes.DTOs;

namespace SiteYonetimi.SiteManagement.UnitTypes.Queries;

public record GetUnitTypeByIdQuery(Guid Id) : IRequest<Result<UnitTypeDto>>;

public class GetUnitTypeByIdQueryHandler : IRequestHandler<GetUnitTypeByIdQuery, Result<UnitTypeDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetUnitTypeByIdQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<UnitTypeDto>> Handle(GetUnitTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var unitType = await _db.UnitTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (unitType is null)
            return Result<UnitTypeDto>.Failure("Birim tipi bulunamadı.");

        return Result<UnitTypeDto>.Success(new UnitTypeDto(
            unitType.Id,
            unitType.SiteId,
            unitType.Name,
            unitType.IsActive,
            unitType.CreatedAt));
    }
}
