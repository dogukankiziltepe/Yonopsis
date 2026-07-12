using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AracGirisCikis.DTOs;
using Entity = SiteYonetimi.Infrastructure.Entities.Shared.AracGirisCikis;

namespace SiteYonetimi.SiteManagement.AracGirisCikis.Commands;

public record CreateAracGirisCikisCommand(Guid SiteId, CreateAracGirisCikisDto Dto) : IRequest<Result<Guid>>;

public class CreateAracGirisCikisCommandHandler : IRequestHandler<CreateAracGirisCikisCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAracGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateAracGirisCikisCommand request, CancellationToken ct)
    {
        if (request.Dto.UnitId.HasValue)
        {
            var exists = await _db.Units.AnyAsync(u => u.Id == request.Dto.UnitId.Value && u.SiteId == request.SiteId, ct);
            if (!exists) return Result<Guid>.Failure("Daire bulunamadı.");
        }

        var entity = new Entity
        {
            SiteId = request.SiteId,
            Plaka = request.Dto.Plaka.ToUpperInvariant().Trim(),
            SuruculAdi = request.Dto.SuruculAdi,
            UnitId = request.Dto.UnitId,
            AracTipi = request.Dto.AracTipi,
            GirisSaati = request.Dto.GirisSaati,
            Aciklama = request.Dto.Aciklama
        };
        _db.AracGirisCikislar.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateAracGirisCikisDtoValidator : AbstractValidator<CreateAracGirisCikisDto>
{
    public CreateAracGirisCikisDtoValidator()
    {
        RuleFor(x => x.Plaka).NotEmpty().MaximumLength(20);
        RuleFor(x => x.GirisSaati).NotEmpty();
    }
}

public record UpdateAracGirisCikisCommand(Guid Id, Guid SiteId, UpdateAracGirisCikisDto Dto) : IRequest<Result<bool>>;

public class UpdateAracGirisCikisCommandHandler : IRequestHandler<UpdateAracGirisCikisCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAracGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(UpdateAracGirisCikisCommand request, CancellationToken ct)
    {
        var entity = await _db.AracGirisCikislar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");

        entity.Plaka = request.Dto.Plaka.ToUpperInvariant().Trim();
        entity.SuruculAdi = request.Dto.SuruculAdi;
        entity.UnitId = request.Dto.UnitId;
        entity.AracTipi = request.Dto.AracTipi;
        entity.GirisSaati = request.Dto.GirisSaati;
        entity.CikisSaati = request.Dto.CikisSaati;
        entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteAracGirisCikisCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;

public class DeleteAracGirisCikisCommandHandler : IRequestHandler<DeleteAracGirisCikisCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAracGirisCikisCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(DeleteAracGirisCikisCommand request, CancellationToken ct)
    {
        var entity = await _db.AracGirisCikislar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
