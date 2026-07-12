using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IsEmirleri.DTOs;

namespace SiteYonetimi.SiteManagement.IsEmirleri.Commands;

public record CreateIsEmriCommand(Guid SiteId, CreateIsEmriDto Dto) : IRequest<Result<Guid>>;
public class CreateIsEmriCommandHandler : IRequestHandler<CreateIsEmriCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateIsEmriCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateIsEmriCommand request, CancellationToken ct)
    {
        var entity = new IsEmri
        {
            SiteId = request.SiteId,
            Baslik = request.Dto.Baslik,
            Aciklama = request.Dto.Aciklama,
            TalepTipiId = request.Dto.TalepTipiId,
            DepartmanId = request.Dto.DepartmanId,
            OrtakAlanId = request.Dto.OrtakAlanId,
            UnitId = request.Dto.UnitId,
            Oncelik = request.Dto.Oncelik,
            Durum = IsEmriDurum.YeniTalep,
            AtananKisiAdi = request.Dto.AtananKisiAdi,
            IslemBaslangic = request.Dto.IslemBaslangic,
            Notlar = request.Dto.Notlar
        };
        _db.IsEmirleri.Add(entity); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}
public class CreateIsEmriDtoValidator : AbstractValidator<CreateIsEmriDto>
{
    public CreateIsEmriDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateIsEmriCommand(Guid Id, Guid SiteId, UpdateIsEmriDto Dto) : IRequest<Result<bool>>;
public class UpdateIsEmriCommandHandler : IRequestHandler<UpdateIsEmriCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateIsEmriCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateIsEmriCommand request, CancellationToken ct)
    {
        var entity = await _db.IsEmirleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("İş emri bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama;
        entity.TalepTipiId = request.Dto.TalepTipiId; entity.DepartmanId = request.Dto.DepartmanId;
        entity.OrtakAlanId = request.Dto.OrtakAlanId; entity.UnitId = request.Dto.UnitId;
        entity.Oncelik = request.Dto.Oncelik; entity.Durum = request.Dto.Durum;
        entity.AtananKisiAdi = request.Dto.AtananKisiAdi;
        entity.IslemBaslangic = request.Dto.IslemBaslangic; entity.IslemBitis = request.Dto.IslemBitis;
        entity.Notlar = request.Dto.Notlar; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record UpdateIsEmriDurumCommand(Guid Id, Guid SiteId, IsEmriDurum Durum) : IRequest<Result<bool>>;
public class UpdateIsEmriDurumCommandHandler : IRequestHandler<UpdateIsEmriDurumCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateIsEmriDurumCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateIsEmriDurumCommand request, CancellationToken ct)
    {
        var entity = await _db.IsEmirleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("İş emri bulunamadı.");
        entity.Durum = request.Durum;
        if (request.Durum == IsEmriDurum.Devam && entity.IslemBaslangic == null) entity.IslemBaslangic = DateTime.UtcNow;
        if (request.Durum == IsEmriDurum.Tamamlandi) entity.IslemBitis = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteIsEmriCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteIsEmriCommandHandler : IRequestHandler<DeleteIsEmriCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteIsEmriCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteIsEmriCommand request, CancellationToken ct)
    {
        var entity = await _db.IsEmirleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("İş emri bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
