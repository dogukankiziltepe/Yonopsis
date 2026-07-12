using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.BirimFiyatlar.DTOs;

namespace SiteYonetimi.SiteManagement.BirimFiyatlar.Commands;

public record CreateBirimFiyatCommand(Guid SiteId, CreateBirimFiyatDto Dto) : IRequest<Result<Guid>>;
public class CreateBirimFiyatCommandHandler : IRequestHandler<CreateBirimFiyatCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateBirimFiyatCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateBirimFiyatCommand request, CancellationToken ct)
    {
        var e = new BirimFiyat
        {
            SiteId = request.SiteId, Tip = request.Dto.Tip, Fiyat = request.Dto.Fiyat,
            Birim = request.Dto.Birim, BaslangicTarihi = request.Dto.BaslangicTarihi,
            BitisTarihi = request.Dto.BitisTarihi, Aciklama = request.Dto.Aciklama
        };
        _db.BirimFiyatlar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateBirimFiyatDtoValidator : AbstractValidator<CreateBirimFiyatDto>
{
    public CreateBirimFiyatDtoValidator() { RuleFor(x => x.Fiyat).GreaterThan(0); }
}

public record UpdateBirimFiyatCommand(Guid Id, Guid SiteId, UpdateBirimFiyatDto Dto) : IRequest<Result<bool>>;
public class UpdateBirimFiyatCommandHandler : IRequestHandler<UpdateBirimFiyatCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateBirimFiyatCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateBirimFiyatCommand request, CancellationToken ct)
    {
        var entity = await _db.BirimFiyatlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Birim fiyat bulunamadı.");
        entity.Tip = request.Dto.Tip; entity.Fiyat = request.Dto.Fiyat; entity.Birim = request.Dto.Birim;
        entity.BaslangicTarihi = request.Dto.BaslangicTarihi; entity.BitisTarihi = request.Dto.BitisTarihi;
        entity.Aciklama = request.Dto.Aciklama; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteBirimFiyatCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteBirimFiyatCommandHandler : IRequestHandler<DeleteBirimFiyatCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteBirimFiyatCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteBirimFiyatCommand request, CancellationToken ct)
    {
        var entity = await _db.BirimFiyatlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Birim fiyat bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
