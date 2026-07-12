using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Toplantilar.DTOs;

namespace SiteYonetimi.SiteManagement.Toplantilar.Commands;

public record CreateToplamtiCommand(Guid SiteId, CreateToplamtiDto Dto) : IRequest<Result<Guid>>;
public class CreateToplamtiCommandHandler : IRequestHandler<CreateToplamtiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateToplamtiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateToplamtiCommand request, CancellationToken ct)
    {
        var e = new Toplanti { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, Gundem = request.Dto.Gundem, ToplamtiTarihi = request.Dto.ToplamtiTarihi, Konum = request.Dto.Konum, Katilimcilar = request.Dto.Katilimcilar };
        _db.Toplantilar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateToplamtiDtoValidator : AbstractValidator<CreateToplamtiDto>
{
    public CreateToplamtiDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateToplamtiCommand(Guid Id, Guid SiteId, UpdateToplamtiDto Dto) : IRequest<Result<bool>>;
public class UpdateToplamtiCommandHandler : IRequestHandler<UpdateToplamtiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateToplamtiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateToplamtiCommand request, CancellationToken ct)
    {
        var entity = await _db.Toplantilar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Toplantı bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.Gundem = request.Dto.Gundem;
        entity.ToplamtiTarihi = request.Dto.ToplamtiTarihi; entity.Konum = request.Dto.Konum; entity.Durum = request.Dto.Durum;
        entity.Katilimcilar = request.Dto.Katilimcilar; entity.Kararlar = request.Dto.Kararlar; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteToplamtiCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteToplamtiCommandHandler : IRequestHandler<DeleteToplamtiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteToplamtiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteToplamtiCommand request, CancellationToken ct)
    {
        var entity = await _db.Toplantilar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Toplantı bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
