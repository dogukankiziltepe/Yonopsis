using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.MobilBildirimSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.MobilBildirimSablonlari.Commands;

public record CreateMobilBildirimSablonuCommand(Guid SiteId, CreateMobilBildirimSablonuDto Dto) : IRequest<Result<Guid>>;
public class CreateMobilBildirimSablonuCommandHandler : IRequestHandler<CreateMobilBildirimSablonuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateMobilBildirimSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateMobilBildirimSablonuCommand request, CancellationToken ct)
    {
        var e = new MobilBildirimSablonu { SiteId = request.SiteId, Ad = request.Dto.Ad, Baslik = request.Dto.Baslik, Icerik = request.Dto.Icerik, Kategori = request.Dto.Kategori };
        _db.MobilBildirimSablonlari.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateMobilBildirimSablonuDtoValidator : AbstractValidator<CreateMobilBildirimSablonuDto>
{
    public CreateMobilBildirimSablonuDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(200); RuleFor(x => x.Baslik).NotEmpty().MaximumLength(200); RuleFor(x => x.Icerik).NotEmpty().MaximumLength(500); }
}

public record UpdateMobilBildirimSablonuCommand(Guid Id, Guid SiteId, UpdateMobilBildirimSablonuDto Dto) : IRequest<Result<bool>>;
public class UpdateMobilBildirimSablonuCommandHandler : IRequestHandler<UpdateMobilBildirimSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateMobilBildirimSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateMobilBildirimSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.MobilBildirimSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Mobil bildirim şablonu bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Baslik = request.Dto.Baslik; entity.Icerik = request.Dto.Icerik; entity.Kategori = request.Dto.Kategori; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteMobilBildirimSablonuCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteMobilBildirimSablonuCommandHandler : IRequestHandler<DeleteMobilBildirimSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteMobilBildirimSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteMobilBildirimSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.MobilBildirimSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Mobil bildirim şablonu bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
