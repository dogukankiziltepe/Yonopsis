using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.SupportRequests.Commands;

public record DeleteSupportRequestCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteSupportRequestCommandHandler : IRequestHandler<DeleteSupportRequestCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteSupportRequestCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SupportRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Destek talebi bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
