using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Olaylar.DTOs;

namespace SiteYonetimi.SiteManagement.Olaylar.Commands;

public record CreateOlayCommand(Guid SiteId, CreateOlayDto Dto) : IRequest<Result<Guid>>;

public class CreateOlayCommandHandler : IRequestHandler<CreateOlayCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateOlayCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateOlayCommand request, CancellationToken ct)
    {
        if (request.Dto.UnitId.HasValue)
        {
            var exists = await _db.Units.AnyAsync(u => u.Id == request.Dto.UnitId.Value && u.SiteId == request.SiteId, ct);
            if (!exists) return Result<Guid>.Failure("Daire bulunamadı.");
        }

        var entity = new Olay
        {
            SiteId = request.SiteId,
            Baslik = request.Dto.Baslik,
            Aciklama = request.Dto.Aciklama,
            OlayTarihi = request.Dto.OlayTarihi,
            Tip = request.Dto.Tip,
            Konum = request.Dto.Konum,
            UnitId = request.Dto.UnitId,
            Durum = OlayDurum.Acik
        };
        _db.Olaylar.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateOlayDtoValidator : AbstractValidator<CreateOlayDto>
{
    public CreateOlayDtoValidator()
    {
        RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Aciklama).NotEmpty().MaximumLength(3000);
    }
}

public record UpdateOlayCommand(Guid Id, Guid SiteId, UpdateOlayDto Dto) : IRequest<Result<bool>>;

public class UpdateOlayCommandHandler : IRequestHandler<UpdateOlayCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateOlayCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(UpdateOlayCommand request, CancellationToken ct)
    {
        var entity = await _db.Olaylar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");

        entity.Baslik = request.Dto.Baslik;
        entity.Aciklama = request.Dto.Aciklama;
        entity.OlayTarihi = request.Dto.OlayTarihi;
        entity.Tip = request.Dto.Tip;
        entity.Konum = request.Dto.Konum;
        entity.UnitId = request.Dto.UnitId;
        entity.Durum = request.Dto.Durum;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteOlayCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;

public class DeleteOlayCommandHandler : IRequestHandler<DeleteOlayCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteOlayCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(DeleteOlayCommand request, CancellationToken ct)
    {
        var entity = await _db.Olaylar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
