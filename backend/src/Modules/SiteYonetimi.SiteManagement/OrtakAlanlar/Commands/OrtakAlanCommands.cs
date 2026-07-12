using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.OrtakAlanlar.DTOs;

namespace SiteYonetimi.SiteManagement.OrtakAlanlar.Commands;

public record CreateOrtakAlanCommand(Guid SiteId, CreateOrtakAlanDto Dto) : IRequest<Result<Guid>>;
public class CreateOrtakAlanCommandHandler : IRequestHandler<CreateOrtakAlanCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateOrtakAlanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateOrtakAlanCommand request, CancellationToken ct)
    {
        var e = new OrtakAlan { SiteId = request.SiteId, Ad = request.Dto.Ad, Aciklama = request.Dto.Aciklama, Konum = request.Dto.Konum };
        _db.OrtakAlanlar.Add(e); await _db.SaveChangesAsync(ct); return Result<Guid>.Success(e.Id);
    }
}
public class CreateOrtakAlanDtoValidator : AbstractValidator<CreateOrtakAlanDto>
{
    public CreateOrtakAlanDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(100); }
}

public record UpdateOrtakAlanCommand(Guid Id, Guid SiteId, UpdateOrtakAlanDto Dto) : IRequest<Result<bool>>;
public class UpdateOrtakAlanCommandHandler : IRequestHandler<UpdateOrtakAlanCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateOrtakAlanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateOrtakAlanCommand request, CancellationToken ct)
    {
        var entity = await _db.OrtakAlanlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Ortak alan bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Aciklama = request.Dto.Aciklama; entity.Konum = request.Dto.Konum;
        entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteOrtakAlanCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteOrtakAlanCommandHandler : IRequestHandler<DeleteOrtakAlanCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteOrtakAlanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteOrtakAlanCommand request, CancellationToken ct)
    {
        var entity = await _db.OrtakAlanlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Ortak alan bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
