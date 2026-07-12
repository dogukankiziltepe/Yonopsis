using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GiderGruplari.DTOs;

namespace SiteYonetimi.SiteManagement.GiderGruplari.Commands;

public record CreateGiderGrubuCommand(Guid SiteId, CreateGiderGrubuDto Dto) : IRequest<Result<Guid>>;

public class CreateGiderGrubuCommandHandler : IRequestHandler<CreateGiderGrubuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateGiderGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateGiderGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = new GiderGrubu
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Order = request.Dto.Order
        };
        _db.GiderGruplari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateGiderGrubuDtoValidator : AbstractValidator<CreateGiderGrubuDto>
{
    public CreateGiderGrubuDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public record UpdateGiderGrubuCommand(Guid Id, Guid SiteId, UpdateGiderGrubuDto Dto) : IRequest<Result>;

public class UpdateGiderGrubuCommandHandler : IRequestHandler<UpdateGiderGrubuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateGiderGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateGiderGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GiderGruplari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Expense group not found.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteGiderGrubuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteGiderGrubuCommandHandler : IRequestHandler<DeleteGiderGrubuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteGiderGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteGiderGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GiderGruplari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Expense group not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
