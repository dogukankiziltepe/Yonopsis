using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Announcements.DTOs;

namespace SiteYonetimi.SiteManagement.Announcements.Queries;

public record GetAnnouncementsBySiteQuery(Guid SiteId) : IRequest<Result<List<AnnouncementSummaryDto>>>;
public record GetAnnouncementByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<AnnouncementDetailDto>>;

public class GetAnnouncementsBySiteQueryHandler : IRequestHandler<GetAnnouncementsBySiteQuery, Result<List<AnnouncementSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnnouncementsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<AnnouncementSummaryDto>>> Handle(GetAnnouncementsBySiteQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Announcements
            .Where(x => x.SiteId == request.SiteId)
            .OrderByDescending(x => x.IsPinned)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new AnnouncementSummaryDto(
                x.Id, x.SiteId, x.Title, x.IsPinned,
                x.PublishDate, x.ExpiryDate, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<AnnouncementSummaryDto>>.Success(items);
    }
}

public class GetAnnouncementByIdQueryHandler : IRequestHandler<GetAnnouncementByIdQuery, Result<AnnouncementDetailDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnnouncementByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<AnnouncementDetailDto>> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.Announcements
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (item == null)
            return Result<AnnouncementDetailDto>.Failure("Duyuru bulunamadı.");

        return Result<AnnouncementDetailDto>.Success(new AnnouncementDetailDto(
            item.Id, item.SiteId, item.CreatedByUserId,
            item.Title, item.Content, item.IsPinned,
            item.PublishDate, item.ExpiryDate, item.CreatedAt, item.UpdatedAt));
    }
}
