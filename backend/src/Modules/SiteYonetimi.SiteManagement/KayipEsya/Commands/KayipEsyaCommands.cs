using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.KayipEsya.DTOs;
using Entity = SiteYonetimi.Infrastructure.Entities.Shared.KayipEsya;

namespace SiteYonetimi.SiteManagement.KayipEsya.Commands;

public record CreateKayipEsyaCommand(Guid SiteId, CreateKayipEsyaDto Dto) : IRequest<Result<Guid>>;

public class CreateKayipEsyaCommandHandler : IRequestHandler<CreateKayipEsyaCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateKayipEsyaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateKayipEsyaCommand request, CancellationToken ct)
    {
        var entity = new Entity
        {
            SiteId = request.SiteId,
            EsyaAdi = request.Dto.EsyaAdi,
            Aciklama = request.Dto.Aciklama,
            BulunanYer = request.Dto.BulunanYer,
            BulunanTarih = request.Dto.BulunanTarih,
            SahipAdi = request.Dto.SahipAdi,
            SahipIletisim = request.Dto.SahipIletisim,
            Durum = KayipEsyaDurum.Beklemede
        };
        _db.KayipEsyalar.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateKayipEsyaDtoValidator : AbstractValidator<CreateKayipEsyaDto>
{
    public CreateKayipEsyaDtoValidator()
    {
        RuleFor(x => x.EsyaAdi).NotEmpty().MaximumLength(200);
    }
}

public record UpdateKayipEsyaCommand(Guid Id, Guid SiteId, UpdateKayipEsyaDto Dto) : IRequest<Result<bool>>;

public class UpdateKayipEsyaCommandHandler : IRequestHandler<UpdateKayipEsyaCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateKayipEsyaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(UpdateKayipEsyaCommand request, CancellationToken ct)
    {
        var entity = await _db.KayipEsyalar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");

        entity.EsyaAdi = request.Dto.EsyaAdi;
        entity.Aciklama = request.Dto.Aciklama;
        entity.BulunanYer = request.Dto.BulunanYer;
        entity.BulunanTarih = request.Dto.BulunanTarih;
        entity.SahipAdi = request.Dto.SahipAdi;
        entity.SahipIletisim = request.Dto.SahipIletisim;
        entity.Durum = request.Dto.Durum;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteKayipEsyaCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;

public class DeleteKayipEsyaCommandHandler : IRequestHandler<DeleteKayipEsyaCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteKayipEsyaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<bool>> Handle(DeleteKayipEsyaCommand request, CancellationToken ct)
    {
        var entity = await _db.KayipEsyalar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
