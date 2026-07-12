using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreatePersonelCommand(Guid SiteId, CreatePersonelDto Dto) : IRequest<Result<Guid>>;

public class CreatePersonelCommandHandler : IRequestHandler<CreatePersonelCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreatePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreatePersonelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.Name))
            return Result<Guid>.Failure("Ad alanı zorunludur.");

        var entity = new Personel
        {
            SiteId     = request.SiteId,
            Name       = request.Dto.Name.Trim(),
            Title      = request.Dto.Title,
            Phone      = request.Dto.Phone,
            Email      = request.Dto.Email,
            Department = request.Dto.Department,
            StartDate  = request.Dto.StartDate,
        };

        _db.Personeller.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdatePersonelCommand(Guid Id, Guid SiteId, UpdatePersonelDto Dto) : IRequest<Result>;

public class UpdatePersonelCommandHandler : IRequestHandler<UpdatePersonelCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Personeller
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Personel bulunamadı.");

        entity.Name       = request.Dto.Name.Trim();
        entity.Title      = request.Dto.Title;
        entity.Phone      = request.Dto.Phone;
        entity.Email      = request.Dto.Email;
        entity.Department = request.Dto.Department;
        entity.StartDate  = request.Dto.StartDate;
        entity.IsActive   = request.Dto.IsActive;
        entity.UpdatedAt  = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeletePersonelCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelCommandHandler : IRequestHandler<DeletePersonelCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Personeller
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Personel bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
