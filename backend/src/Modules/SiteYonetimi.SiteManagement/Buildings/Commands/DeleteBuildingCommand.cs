using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Buildings.Commands;

public record DeleteBuildingCommand(Guid Id) : IRequest<Result>;

public class DeleteBuildingCommandHandler : IRequestHandler<DeleteBuildingCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public DeleteBuildingCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = await _db.Buildings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (building is null)
            return Result.Failure("Bina bulunamadı.");

        building.IsDeleted = true;
        building.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
