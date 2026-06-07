using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Sites.Commands;

public record DeleteSiteCommand(Guid Id) : IRequest<Result>;

public class DeleteSiteCommandHandler : IRequestHandler<DeleteSiteCommand, Result>
{
    private readonly MasterDbContext _db;

    public DeleteSiteCommandHandler(MasterDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _db.Sites.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (site is null)
            return Result.Failure("Site bulunamadı.");

        site.IsDeleted = true;
        site.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
