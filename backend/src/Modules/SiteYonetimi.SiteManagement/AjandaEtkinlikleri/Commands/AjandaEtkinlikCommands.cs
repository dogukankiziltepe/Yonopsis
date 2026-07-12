using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AjandaEtkinlikleri.DTOs;

namespace SiteYonetimi.SiteManagement.AjandaEtkinlikleri.Commands;

public record CreateAjandaEtkinlikCommand(Guid SiteId, CreateAjandaEtkinlikDto Dto) : IRequest<Result<Guid>>;
public class CreateAjandaEtkinlikCommandHandler : IRequestHandler<CreateAjandaEtkinlikCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAjandaEtkinlikCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateAjandaEtkinlikCommand request, CancellationToken ct)
    {
        var e = new AjandaEtkinlik { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, BaslangicTarihi = request.Dto.BaslangicTarihi, BitisTarihi = request.Dto.BitisTarihi, Konum = request.Dto.Konum, Renk = request.Dto.Renk, TumGun = request.Dto.TumGun };
        _db.AjandaEtkinlikleri.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateAjandaEtkinlikDtoValidator : AbstractValidator<CreateAjandaEtkinlikDto>
{
    public CreateAjandaEtkinlikDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateAjandaEtkinlikCommand(Guid Id, Guid SiteId, UpdateAjandaEtkinlikDto Dto) : IRequest<Result<bool>>;
public class UpdateAjandaEtkinlikCommandHandler : IRequestHandler<UpdateAjandaEtkinlikCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAjandaEtkinlikCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateAjandaEtkinlikCommand request, CancellationToken ct)
    {
        var entity = await _db.AjandaEtkinlikleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Etkinlik bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.BaslangicTarihi = request.Dto.BaslangicTarihi;
        entity.BitisTarihi = request.Dto.BitisTarihi; entity.Konum = request.Dto.Konum; entity.Renk = request.Dto.Renk;
        entity.TumGun = request.Dto.TumGun; entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteAjandaEtkinlikCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteAjandaEtkinlikCommandHandler : IRequestHandler<DeleteAjandaEtkinlikCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAjandaEtkinlikCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteAjandaEtkinlikCommand request, CancellationToken ct)
    {
        var entity = await _db.AjandaEtkinlikleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Etkinlik bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
