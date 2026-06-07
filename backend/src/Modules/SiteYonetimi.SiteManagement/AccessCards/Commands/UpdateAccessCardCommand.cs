using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AccessCards.DTOs;

namespace SiteYonetimi.SiteManagement.AccessCards.Commands;

public record UpdateAccessCardCommand(Guid Id, Guid SiteId, UpdateAccessCardDto Dto) : IRequest<Result>;

public class UpdateAccessCardCommandHandler : IRequestHandler<UpdateAccessCardCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAccessCardCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAccessCardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.AccessCards
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);

        if (entity == null)
            return Result.Failure("Kart bulunamadı.");

        entity.CardNumber = request.Dto.CardNumber;
        entity.IsActive = request.Dto.IsActive;
        entity.IssueDate = request.Dto.IssueDate;
        entity.ExpiryDate = request.Dto.ExpiryDate;
        entity.Notes = request.Dto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
