using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Announcements.DTOs;

namespace SiteYonetimi.SiteManagement.Announcements.Queries;

public record GetAnnouncementsBySiteQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? SearchTerm = null)
    : IRequest<Result<PaginatedResult<AnnouncementSummaryDto>>>;
public record GetAnnouncementByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<AnnouncementDetailDto>>;

public class GetAnnouncementsBySiteQueryHandler : IRequestHandler<GetAnnouncementsBySiteQuery, Result<PaginatedResult<AnnouncementSummaryDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetAnnouncementsBySiteQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<AnnouncementSummaryDto>>> Handle(GetAnnouncementsBySiteQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Announcements.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.IsPinned)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AnnouncementSummaryDto(
                x.Id, x.SiteId, x.Title, x.IsPinned,
                x.PublishDate, x.ExpiryDate, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<AnnouncementSummaryDto>>.Success(
            PaginatedResult<AnnouncementSummaryDto>.Create(items, totalCount, request.Page, request.PageSize));
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
