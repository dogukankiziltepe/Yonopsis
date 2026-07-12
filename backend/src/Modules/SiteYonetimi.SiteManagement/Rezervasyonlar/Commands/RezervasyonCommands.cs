using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Rezervasyonlar.DTOs;

namespace SiteYonetimi.SiteManagement.Rezervasyonlar.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateRezervasyonCommand(Guid SiteId, CreateRezervasyonDto Dto) : IRequest<Result<Guid>>;

public class CreateRezervasyonCommandHandler : IRequestHandler<CreateRezervasyonCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateRezervasyonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateRezervasyonCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.EndDate < request.Dto.StartDate)
            return Result<Guid>.Failure("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var entity = new Rezervasyon
        {
            SiteId    = request.SiteId,
            TesisId   = request.Dto.TesisId,
            PersonId  = request.Dto.PersonId,
            StartDate = request.Dto.StartDate,
            EndDate   = request.Dto.EndDate,
            Durum     = request.Dto.Durum,
            Notes     = request.Dto.Notes,
        };

        _db.Rezervasyonlar.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateRezervasyonCommand(Guid Id, Guid SiteId, UpdateRezervasyonDto Dto) : IRequest<Result>;

public class UpdateRezervasyonCommandHandler : IRequestHandler<UpdateRezervasyonCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateRezervasyonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateRezervasyonCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.EndDate < request.Dto.StartDate)
            return Result.Failure("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var entity = await _db.Rezervasyonlar
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Rezervasyon bulunamadı.");

        entity.TesisId   = request.Dto.TesisId;
        entity.PersonId  = request.Dto.PersonId;
        entity.StartDate = request.Dto.StartDate;
        entity.EndDate   = request.Dto.EndDate;
        entity.Durum     = request.Dto.Durum;
        entity.Notes     = request.Dto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteRezervasyonCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteRezervasyonCommandHandler : IRequestHandler<DeleteRezervasyonCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteRezervasyonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteRezervasyonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Rezervasyonlar
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Rezervasyon bulunamadı.");

        entity.IsDeleted  = true;
        entity.UpdatedAt  = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
