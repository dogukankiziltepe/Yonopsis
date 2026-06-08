using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;

namespace SiteYonetimi.SiteManagement.SupportRequests.Queries;

public record GetSupportRequestCommentsQuery(Guid RequestId, Guid SiteId) : IRequest<Result<List<SupportRequestCommentDto>>>;

public class GetSupportRequestCommentsQueryHandler : IRequestHandler<GetSupportRequestCommentsQuery, Result<List<SupportRequestCommentDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetSupportRequestCommentsQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<SupportRequestCommentDto>>> Handle(GetSupportRequestCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _db.SupportRequestComments
            .Where(c => c.RequestId == request.RequestId && c.SiteId == request.SiteId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new SupportRequestCommentDto(c.Id, c.UserId, c.AuthorName, c.Content, c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<SupportRequestCommentDto>>.Success(comments);
    }
}
