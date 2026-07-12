using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.DTOs;
using Entity = SiteYonetimi.Infrastructure.Entities.Shared.ZiyaretciGirisCikis;

namespace SiteYonetimi.SiteManagement.ZiyaretciGirisCikis.Commands;

// ── Create ─────────────────────────────────────────────────────────────
public record CreateZiyaretciGirisCikisCommand(Guid SiteId, CreateZiyaretciGirisCikisDto Dto) : IRequest<Result<Guid>>;

public class CreateZiyaretciGirisCikisCommandHandler : IRequestHandler<CreateZiyaretciGirisCikisCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateZiyaretciGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateZiyaretciGirisCikisCommand request, CancellationToken ct)
    {
        if (request.Dto.UnitId.HasValue)
        {
            var exists = await _db.Units.AnyAsync(u => u.Id == request.Dto.UnitId.Value && u.SiteId == request.SiteId, ct);
            if (!exists) return Result<Guid>.Failure("Daire bulunamadı.");
        }

        var entity = new Entity
        {
            SiteId = request.SiteId,
            GelensAdi = request.Dto.GelensAdi,
            GeldigiKisi = request.Dto.GeldigiKisi,
            UnitId = request.Dto.UnitId,
            ZiyaretAmaci = request.Dto.ZiyaretAmaci,
            GirisSaati = request.Dto.GirisSaati,
            Plaka = request.Dto.Plaka,
            Aciklama = request.Dto.Aciklama
        };
        _db.ZiyaretciGirisCikislar.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateZiyaretciGirisCikisDtoValidator : AbstractValidator<CreateZiyaretciGirisCikisDto>
{
    public CreateZiyaretciGirisCikisDtoValidator()
    {
        RuleFor(x => x.GelensAdi).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GirisSaati).NotEmpty();
    }
}

// ── Update ─────────────────────────────────────────────────────────────
public record UpdateZiyaretciGirisCikisCommand(Guid Id, Guid SiteId, UpdateZiyaretciGirisCikisDto Dto) : IRequest<Result<bool>>;

public class UpdateZiyaretciGirisCikisCommandHandler : IRequestHandler<UpdateZiyaretciGirisCikisCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateZiyaretciGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(UpdateZiyaretciGirisCikisCommand request, CancellationToken ct)
    {
        var entity = await _db.ZiyaretciGirisCikislar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");

        entity.GelensAdi = request.Dto.GelensAdi;
        entity.GeldigiKisi = request.Dto.GeldigiKisi;
        entity.UnitId = request.Dto.UnitId;
        entity.ZiyaretAmaci = request.Dto.ZiyaretAmaci;
        entity.GirisSaati = request.Dto.GirisSaati;
        entity.CikisSaati = request.Dto.CikisSaati;
        entity.Plaka = request.Dto.Plaka;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

// ── Delete ─────────────────────────────────────────────────────────────
public record DeleteZiyaretciGirisCikisCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;

public class DeleteZiyaretciGirisCikisCommandHandler : IRequestHandler<DeleteZiyaretciGirisCikisCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteZiyaretciGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(DeleteZiyaretciGirisCikisCommand request, CancellationToken ct)
    {
        var entity = await _db.ZiyaretciGirisCikislar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
