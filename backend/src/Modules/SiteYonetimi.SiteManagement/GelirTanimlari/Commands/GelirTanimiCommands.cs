using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GelirTanimlari.DTOs;

namespace SiteYonetimi.SiteManagement.GelirTanimlari.Commands;

public record CreateGelirTanimiCommand(Guid SiteId, CreateGelirTanimiDto Dto) : IRequest<Result<Guid>>;

public class CreateGelirTanimiCommandHandler : IRequestHandler<CreateGelirTanimiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateGelirTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateGelirTanimiCommand request, CancellationToken cancellationToken)
    {
        var entity = new GelirTanimi
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            GelirGrubuId = request.Dto.GelirGrubuId,
            Order = request.Dto.Order
        };
        _db.GelirTanimlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateGelirTanimiDtoValidator : AbstractValidator<CreateGelirTanimiDto>
{
    public CreateGelirTanimiDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public record UpdateGelirTanimiCommand(Guid Id, Guid SiteId, UpdateGelirTanimiDto Dto) : IRequest<Result>;

public class UpdateGelirTanimiCommandHandler : IRequestHandler<UpdateGelirTanimiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateGelirTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateGelirTanimiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GelirTanimlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Income definition not found.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.GelirGrubuId = request.Dto.GelirGrubuId;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteGelirTanimiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteGelirTanimiCommandHandler : IRequestHandler<DeleteGelirTanimiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteGelirTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteGelirTanimiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GelirTanimlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Income definition not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
