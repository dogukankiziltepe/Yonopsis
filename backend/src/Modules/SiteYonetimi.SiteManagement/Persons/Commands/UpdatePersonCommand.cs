using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Persons.DTOs;

namespace SiteYonetimi.SiteManagement.Persons.Commands;

public record UpdatePersonCommand(Guid UserSiteId, Guid SiteId, UpdatePersonDto Dto) : IRequest<Result>;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdatePersonCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var us = await _db.UserSites
            .FirstOrDefaultAsync(x => x.Id == request.UserSiteId && x.SiteId == request.SiteId, cancellationToken);

        if (us is null)
            return Result.Failure("Kişi bulunamadı.");

        if (request.Dto.UserType.HasValue)
            us.UserType = request.Dto.UserType.Value;

        if (request.Dto.RoleTypeId.HasValue)
            us.RoleTypeId = request.Dto.RoleTypeId.Value;

        if (request.Dto.Status.HasValue)
            us.Status = request.Dto.Status.Value;

        us.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
