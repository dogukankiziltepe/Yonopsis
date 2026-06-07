using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;

namespace SiteYonetimi.SiteManagement.SupportRequests.Commands;

public record UpdateSupportRequestCommand(Guid Id, Guid SiteId, UpdateSupportRequestDto Dto) : IRequest<Result>;

public class UpdateSupportRequestCommandHandler : IRequestHandler<UpdateSupportRequestCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateSupportRequestCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.SupportRequests
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Destek talebi bulunamadı.");

        entity.Subject = request.Dto.Subject;
        entity.Description = request.Dto.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
