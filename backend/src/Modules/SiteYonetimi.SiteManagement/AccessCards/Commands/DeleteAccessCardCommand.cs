using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.AccessCards.Commands;

public record DeleteAccessCardCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteAccessCardCommandHandler : IRequestHandler<DeleteAccessCardCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAccessCardCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteAccessCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.AccessCards
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Kart bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
