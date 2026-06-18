using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Events;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record RemovePersonFromSiteCommand(Guid UserSiteId, Guid SiteId) : IRequest<Result>;

public class RemovePersonFromSiteCommandHandler : IRequestHandler<RemovePersonFromSiteCommand, Result>
{
    private readonly MasterDbContext _db;
    private readonly IPublisher _publisher;

    public RemovePersonFromSiteCommandHandler(MasterDbContext db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<Result> Handle(RemovePersonFromSiteCommand request, CancellationToken cancellationToken)
    {
        var us = await _db.UserSites
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result.Failure("Kişi bulunamadı.");

        us.IsDeleted = true;
        us.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        // İlgili cari hesabın pasife alınması (Muhasebe modülü dinler).
        await _publisher.Publish(
            new PersonRemovedFromSiteDomainEvent(request.SiteId, us.UserId, us.UserType),
            cancellationToken);

        return Result.Success();
    }
}
