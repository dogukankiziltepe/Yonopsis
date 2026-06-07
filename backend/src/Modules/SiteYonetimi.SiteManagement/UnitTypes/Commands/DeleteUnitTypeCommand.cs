using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.UnitTypes.Commands;

public record DeleteUnitTypeCommand(Guid Id) : IRequest<Result>;

public class DeleteUnitTypeCommandHandler : IRequestHandler<DeleteUnitTypeCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public DeleteUnitTypeCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var unitType = await _db.UnitTypes
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (unitType is null)
            return Result.Failure("Birim tipi bulunamadı.");

        unitType.IsDeleted = true;
        unitType.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
