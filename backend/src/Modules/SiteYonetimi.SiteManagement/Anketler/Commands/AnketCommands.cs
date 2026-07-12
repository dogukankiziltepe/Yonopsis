using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Anketler.DTOs;

namespace SiteYonetimi.SiteManagement.Anketler.Commands;

public record CreateAnketCommand(Guid SiteId, CreateAnketDto Dto) : IRequest<Result<Guid>>;
public class CreateAnketCommandHandler : IRequestHandler<CreateAnketCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAnketCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateAnketCommand request, CancellationToken ct)
    {
        var e = new Anket { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, BaslangicTarihi = request.Dto.BaslangicTarihi, BitisTarihi = request.Dto.BitisTarihi };
        _db.Anketler.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateAnketDtoValidator : AbstractValidator<CreateAnketDto>
{
    public CreateAnketDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateAnketCommand(Guid Id, Guid SiteId, UpdateAnketDto Dto) : IRequest<Result<bool>>;
public class UpdateAnketCommandHandler : IRequestHandler<UpdateAnketCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAnketCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateAnketCommand request, CancellationToken ct)
    {
        var entity = await _db.Anketler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Anket bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.BaslangicTarihi = request.Dto.BaslangicTarihi;
        entity.BitisTarihi = request.Dto.BitisTarihi; entity.Durum = request.Dto.Durum; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteAnketCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteAnketCommandHandler : IRequestHandler<DeleteAnketCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAnketCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteAnketCommand request, CancellationToken ct)
    {
        var entity = await _db.Anketler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Anket bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
