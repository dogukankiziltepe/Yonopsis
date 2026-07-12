using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.DaireSayaclar.DTOs;

namespace SiteYonetimi.SiteManagement.DaireSayaclar.Commands;

public record CreateDaireSayacCommand(Guid SiteId, CreateDaireSayacDto Dto) : IRequest<Result<Guid>>;
public class CreateDaireSayacCommandHandler : IRequestHandler<CreateDaireSayacCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateDaireSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateDaireSayacCommand request, CancellationToken ct)
    {
        var e = new DaireSayac
        {
            SiteId = request.SiteId, UnitId = request.Dto.UnitId, AnaSayacId = request.Dto.AnaSayacId,
            Tip = request.Dto.Tip, SeriNo = request.Dto.SeriNo, Marka = request.Dto.Marka,
            TakimTarihi = request.Dto.TakimTarihi, Aciklama = request.Dto.Aciklama
        };
        _db.DaireSayaclar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateDaireSayacDtoValidator : AbstractValidator<CreateDaireSayacDto>
{
    public CreateDaireSayacDtoValidator() { RuleFor(x => x.UnitId).NotEmpty(); RuleFor(x => x.AnaSayacId).NotEmpty(); }
}

public record UpdateDaireSayacCommand(Guid Id, Guid SiteId, UpdateDaireSayacDto Dto) : IRequest<Result<bool>>;
public class UpdateDaireSayacCommandHandler : IRequestHandler<UpdateDaireSayacCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateDaireSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateDaireSayacCommand request, CancellationToken ct)
    {
        var entity = await _db.DaireSayaclar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Daire sayacı bulunamadı.");
        entity.UnitId = request.Dto.UnitId; entity.AnaSayacId = request.Dto.AnaSayacId;
        entity.Tip = request.Dto.Tip; entity.SeriNo = request.Dto.SeriNo; entity.Marka = request.Dto.Marka;
        entity.TakimTarihi = request.Dto.TakimTarihi; entity.Aciklama = request.Dto.Aciklama;
        entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteDaireSayacCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteDaireSayacCommandHandler : IRequestHandler<DeleteDaireSayacCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteDaireSayacCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteDaireSayacCommand request, CancellationToken ct)
    {
        var entity = await _db.DaireSayaclar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Daire sayacı bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
