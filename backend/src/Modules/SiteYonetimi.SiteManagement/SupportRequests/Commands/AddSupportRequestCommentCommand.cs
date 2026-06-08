using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.SupportRequests.Commands;

public record AddSupportRequestCommentCommand(
    Guid RequestId,
    Guid SiteId,
    Guid UserId,
    string AuthorName,
    string Content) : IRequest<Result<Guid>>;

public class AddSupportRequestCommentCommandHandler : IRequestHandler<AddSupportRequestCommentCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public AddSupportRequestCommentCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(AddSupportRequestCommentCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return Result<Guid>.Failure("Yorum içeriği boş olamaz.");

        var exists = await _db.SupportRequests
            .AnyAsync(r => r.Id == request.RequestId && r.SiteId == request.SiteId, cancellationToken);

        if (!exists)
            return Result<Guid>.Failure("Destek talebi bulunamadı.");

        var comment = new SupportRequestComment
        {
            SiteId = request.SiteId,
            RequestId = request.RequestId,
            UserId = request.UserId,
            AuthorName = request.AuthorName,
            Content = request.Content.Trim()
        };

        _db.SupportRequestComments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(comment.Id);
    }
}
