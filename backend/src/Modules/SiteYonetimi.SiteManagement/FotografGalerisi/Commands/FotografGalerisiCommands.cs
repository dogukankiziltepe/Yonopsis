using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.FotografGalerisi.DTOs;

namespace SiteYonetimi.SiteManagement.FotografGalerisi.Commands;

public record CreateFotografGalerisiCommand(Guid SiteId, CreateFotografGalerisiDto Dto) : IRequest<Result<Guid>>;
public class CreateFotografGalerisiCommandHandler : IRequestHandler<CreateFotografGalerisiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateFotografGalerisiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateFotografGalerisiCommand request, CancellationToken ct)
    {
        var e = new Infrastructure.Entities.Shared.FotografGalerisi { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, ImageUrl = request.Dto.ImageUrl, Sira = request.Dto.Sira };
        _db.FotografGalerisi.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateFotografGalerisiDtoValidator : AbstractValidator<CreateFotografGalerisiDto>
{
    public CreateFotografGalerisiDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(200); RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500); }
}

public record UpdateFotografGalerisiCommand(Guid Id, Guid SiteId, UpdateFotografGalerisiDto Dto) : IRequest<Result<bool>>;
public class UpdateFotografGalerisiCommandHandler : IRequestHandler<UpdateFotografGalerisiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateFotografGalerisiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateFotografGalerisiCommand request, CancellationToken ct)
    {
        var entity = await _db.FotografGalerisi.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Fotoğraf bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.ImageUrl = request.Dto.ImageUrl;
        entity.Sira = request.Dto.Sira; entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteFotografGalerisiCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteFotografGalerisiCommandHandler : IRequestHandler<DeleteFotografGalerisiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteFotografGalerisiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteFotografGalerisiCommand request, CancellationToken ct)
    {
        var entity = await _db.FotografGalerisi.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Fotoğraf bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
