using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Announcements.DTOs;

namespace SiteYonetimi.SiteManagement.Announcements.Commands;

public record UpdateAnnouncementCommand(Guid Id, Guid SiteId, UpdateAnnouncementDto Dto) : IRequest<Result>;

public class UpdateAnnouncementCommandHandler : IRequestHandler<UpdateAnnouncementCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAnnouncementCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Announcements
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Duyuru bulunamadı.");

        entity.Title = request.Dto.Title;
        entity.Content = request.Dto.Content;
        entity.IsPinned = request.Dto.IsPinned;
        entity.PublishDate = request.Dto.PublishDate;
        entity.ExpiryDate = request.Dto.ExpiryDate;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
