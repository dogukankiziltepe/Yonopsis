using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.AidatKalemleri.Commands;

public record DeleteAidatKalemiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteAidatKalemiCommandHandler : IRequestHandler<DeleteAidatKalemiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAidatKalemiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteAidatKalemiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.AidatKalemleri.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null || entity.SiteId != request.SiteId)
            return Result.Failure("Aidat kalemi bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
