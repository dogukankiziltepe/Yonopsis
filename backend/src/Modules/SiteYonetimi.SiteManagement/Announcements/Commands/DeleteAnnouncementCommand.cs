using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Announcements.Commands;

public record DeleteAnnouncementCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteAnnouncementCommandHandler : IRequestHandler<DeleteAnnouncementCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAnnouncementCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Announcements
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Duyuru bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
