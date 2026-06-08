using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;

namespace SiteYonetimi.SiteManagement.SupportRequests.Commands;

public record UpdateSupportRequestStatusCommand(Guid Id, Guid SiteId, UpdateSupportRequestStatusDto Dto) : IRequest<Result>;

public class UpdateSupportRequestStatusCommandHandler : IRequestHandler<UpdateSupportRequestStatusCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateSupportRequestStatusCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateSupportRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SupportRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Destek talebi bulunamadı.");

        entity.Status = request.Dto.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        if (request.Dto.Status == SupportRequestStatus.Resolved)
            entity.ResolvedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
