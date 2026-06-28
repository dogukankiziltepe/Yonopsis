using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Tesisler.DTOs;

namespace SiteYonetimi.SiteManagement.Tesisler.Commands;

public record CreateTesisCommand(Guid SiteId, CreateTesisDto Dto) : IRequest<Result<Guid>>;

public class CreateTesisCommandHandler : IRequestHandler<CreateTesisCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateTesisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateTesisCommand request, CancellationToken cancellationToken)
    {
        var entity = new Tesis
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Kapasite = request.Dto.Kapasite,
            Order = request.Dto.Order
        };
        _db.Tesisler.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateTesisDtoValidator : AbstractValidator<CreateTesisDto>
{
    public CreateTesisDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Kapasite).GreaterThan(0).When(x => x.Kapasite.HasValue);
    }
}

public record UpdateTesisCommand(Guid Id, Guid SiteId, UpdateTesisDto Dto) : IRequest<Result>;

public class UpdateTesisCommandHandler : IRequestHandler<UpdateTesisCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTesisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTesisCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Tesisler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Facility not found.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.Kapasite = request.Dto.Kapasite;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteTesisCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteTesisCommandHandler : IRequestHandler<DeleteTesisCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteTesisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteTesisCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Tesisler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Facility not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
