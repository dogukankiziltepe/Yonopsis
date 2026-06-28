using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirGruplari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirGruplari.Commands;

// --- Create ---
public record CreateGelirGrubuCommand(Guid SiteId, CreateGelirGrubuDto Dto) : IRequest<Result<Guid>>;

public class CreateGelirGrubuCommandHandler : IRequestHandler<CreateGelirGrubuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateGelirGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateGelirGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = new GelirGrubu
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Order = request.Dto.Order
        };
        _db.GelirGruplari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateGelirGrubuDtoValidator : AbstractValidator<CreateGelirGrubuDto>
{
    public CreateGelirGrubuDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

// --- Update ---
public record UpdateGelirGrubuCommand(Guid Id, Guid SiteId, UpdateGelirGrubuDto Dto) : IRequest<Result>;

public class UpdateGelirGrubuCommandHandler : IRequestHandler<UpdateGelirGrubuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateGelirGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateGelirGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GelirGruplari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Income group not found.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// --- Delete ---
public record DeleteGelirGrubuCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteGelirGrubuCommandHandler : IRequestHandler<DeleteGelirGrubuCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteGelirGrubuCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteGelirGrubuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GelirGruplari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Income group not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
