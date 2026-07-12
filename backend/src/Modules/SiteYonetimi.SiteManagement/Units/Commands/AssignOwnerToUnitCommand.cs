using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Units.Services;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record AssignOwnerToUnitCommand(Guid UnitId, Guid UserId) : IRequest<Result>;

public class AssignOwnerToUnitCommandHandler : IRequestHandler<AssignOwnerToUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    private readonly IPersonUnitHistoryService _historyService;

    public AssignOwnerToUnitCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb, IPersonUnitHistoryService historyService)
    {
        _db = db;
        _masterDb = masterDb;
        _historyService = historyService;
    }

    public async Task<Result> Handle(AssignOwnerToUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.UnitId, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        var userExists = await _masterDb.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
            return Result.Failure("Kullanıcı bulunamadı.");

        unit.OwnerUserId = request.UserId;
        unit.UpdatedAt = DateTime.UtcNow;

        await _historyService.RecordAssignmentAsync(unit.SiteId, unit.Id, request.UserId, UserType.Owner, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class AssignOwnerToUnitCommandValidator : AbstractValidator<AssignOwnerToUnitCommand>
{
    public AssignOwnerToUnitCommandValidator()
    {
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
