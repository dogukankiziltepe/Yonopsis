using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record DeleteUnitCommand(Guid Id) : IRequest<Result>;

public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public DeleteUnitCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        unit.IsDeleted = true;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
