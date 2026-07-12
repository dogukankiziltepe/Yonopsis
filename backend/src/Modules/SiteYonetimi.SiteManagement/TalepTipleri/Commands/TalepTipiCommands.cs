using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TalepTipleri.DTOs;

namespace SiteYonetimi.SiteManagement.TalepTipleri.Commands;

public record CreateTalepTipiCommand(Guid SiteId, CreateTalepTipiDto Dto) : IRequest<Result<Guid>>;
public class CreateTalepTipiCommandHandler : IRequestHandler<CreateTalepTipiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateTalepTipiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateTalepTipiCommand request, CancellationToken ct)
    {
        var e = new TalepTipi { SiteId = request.SiteId, Ad = request.Dto.Ad, Aciklama = request.Dto.Aciklama };
        _db.TalepTipleri.Add(e); await _db.SaveChangesAsync(ct); return Result<Guid>.Success(e.Id);
    }
}
public class CreateTalepTipiDtoValidator : AbstractValidator<CreateTalepTipiDto>
{
    public CreateTalepTipiDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(100); }
}

public record UpdateTalepTipiCommand(Guid Id, Guid SiteId, UpdateTalepTipiDto Dto) : IRequest<Result<bool>>;
public class UpdateTalepTipiCommandHandler : IRequestHandler<UpdateTalepTipiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTalepTipiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateTalepTipiCommand request, CancellationToken ct)
    {
        var entity = await _db.TalepTipleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Talep tipi bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Aciklama = request.Dto.Aciklama; entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteTalepTipiCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteTalepTipiCommandHandler : IRequestHandler<DeleteTalepTipiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteTalepTipiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteTalepTipiCommand request, CancellationToken ct)
    {
        var entity = await _db.TalepTipleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Talep tipi bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
