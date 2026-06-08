using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Units.Commands;

public record AssignTenantToUnitCommand(Guid UnitId, Guid UserId) : IRequest<Result>;

public class AssignTenantToUnitCommandHandler : IRequestHandler<AssignTenantToUnitCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public AssignTenantToUnitCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(AssignTenantToUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _db.Units.FirstOrDefaultAsync(x => x.Id == request.UnitId, cancellationToken);
        if (unit is null)
            return Result.Failure("Daire bulunamadı.");

        if (unit.TenantUserId.HasValue)
            return Result.Failure("Bu dairede zaten aktif bir kiracı bulunmaktadır.");

        var userExists = await _masterDb.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
            return Result.Failure("Kullanıcı bulunamadı.");

        unit.TenantUserId = request.UserId;
        unit.Status = SiteYonetimi.Shared.Enums.UnitStatus.Kiralik;
        unit.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class AssignTenantToUnitCommandValidator : AbstractValidator<AssignTenantToUnitCommand>
{
    public AssignTenantToUnitCommandValidator()
    {
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
