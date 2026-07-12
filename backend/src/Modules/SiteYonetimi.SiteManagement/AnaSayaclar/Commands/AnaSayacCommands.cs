using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AnaSayaclar.DTOs;

namespace SiteYonetimi.SiteManagement.AnaSayaclar.Commands;

public record CreateAnaSayacCommand(Guid SiteId, CreateAnaSayacDto Dto) : IRequest<Result<Guid>>;
public class CreateAnaSayacCommandHandler : IRequestHandler<CreateAnaSayacCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAnaSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateAnaSayacCommand request, CancellationToken ct)
    {
        var e = new AnaSayac
        {
            SiteId = request.SiteId, Ad = request.Dto.Ad, Tip = request.Dto.Tip,
            SeriNo = request.Dto.SeriNo, Marka = request.Dto.Marka,
            TakimTarihi = request.Dto.TakimTarihi, Aciklama = request.Dto.Aciklama
        };
        _db.AnaSayaclar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateAnaSayacDtoValidator : AbstractValidator<CreateAnaSayacDto>
{
    public CreateAnaSayacDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(200); }
}

public record UpdateAnaSayacCommand(Guid Id, Guid SiteId, UpdateAnaSayacDto Dto) : IRequest<Result<bool>>;
public class UpdateAnaSayacCommandHandler : IRequestHandler<UpdateAnaSayacCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAnaSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateAnaSayacCommand request, CancellationToken ct)
    {
        var entity = await _db.AnaSayaclar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Ana sayaç bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Tip = request.Dto.Tip;
        entity.SeriNo = request.Dto.SeriNo; entity.Marka = request.Dto.Marka;
        entity.TakimTarihi = request.Dto.TakimTarihi; entity.Aciklama = request.Dto.Aciklama;
        entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteAnaSayacCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteAnaSayacCommandHandler : IRequestHandler<DeleteAnaSayacCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteAnaSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteAnaSayacCommand request, CancellationToken ct)
    {
        var entity = await _db.AnaSayaclar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Ana sayaç bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
