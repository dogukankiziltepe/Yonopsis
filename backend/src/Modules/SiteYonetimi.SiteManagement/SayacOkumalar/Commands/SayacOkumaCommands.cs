using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SayacOkumalar.DTOs;

namespace SiteYonetimi.SiteManagement.SayacOkumalar.Commands;

public record CreateSayacOkumaCommand(Guid SiteId, CreateSayacOkumaDto Dto) : IRequest<Result<Guid>>;
public class CreateSayacOkumaCommandHandler : IRequestHandler<CreateSayacOkumaCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateSayacOkumaCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateSayacOkumaCommand request, CancellationToken ct)
    {
        if (request.Dto.AnaSayacId is null && request.Dto.DaireSayacId is null)
            return Result<Guid>.Failure("Ana sayaç veya daire sayacı belirtilmelidir.");
        var e = new SayacOkuma
        {
            SiteId = request.SiteId, AnaSayacId = request.Dto.AnaSayacId, DaireSayacId = request.Dto.DaireSayacId,
            OkumaTarihi = request.Dto.OkumaTarihi, OncekiEndeks = request.Dto.OncekiEndeks,
            SonEndeks = request.Dto.SonEndeks, Aciklama = request.Dto.Aciklama
        };
        _db.SayacOkumalar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateSayacOkumaDtoValidator : AbstractValidator<CreateSayacOkumaDto>
{
    public CreateSayacOkumaDtoValidator() { RuleFor(x => x.SonEndeks).GreaterThanOrEqualTo(0); }
}

public record UpdateSayacOkumaCommand(Guid Id, Guid SiteId, UpdateSayacOkumaDto Dto) : IRequest<Result<bool>>;
public class UpdateSayacOkumaCommandHandler : IRequestHandler<UpdateSayacOkumaCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateSayacOkumaCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateSayacOkumaCommand request, CancellationToken ct)
    {
        var entity = await _db.SayacOkumalar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Sayaç okuması bulunamadı.");
        entity.OkumaTarihi = request.Dto.OkumaTarihi; entity.OncekiEndeks = request.Dto.OncekiEndeks;
        entity.SonEndeks = request.Dto.SonEndeks; entity.Aciklama = request.Dto.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteSayacOkumaCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteSayacOkumaCommandHandler : IRequestHandler<DeleteSayacOkumaCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteSayacOkumaCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteSayacOkumaCommand request, CancellationToken ct)
    {
        var entity = await _db.SayacOkumalar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Sayaç okuması bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
