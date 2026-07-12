using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Faturalar.DTOs;

namespace SiteYonetimi.SiteManagement.Faturalar.Commands;

public record CreateFaturaCommand(Guid SiteId, CreateFaturaDto Dto) : IRequest<Result<Guid>>;

public class CreateFaturaCommandHandler : IRequestHandler<CreateFaturaCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateFaturaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateFaturaCommand request, CancellationToken cancellationToken)
    {
        if (request.Dto.ToplamTutar <= 0)
            return Result<Guid>.Failure("Tutar sıfırdan büyük olmalıdır.");
        if (string.IsNullOrWhiteSpace(request.Dto.CariAdi))
            return Result<Guid>.Failure("Cari adı zorunludur.");

        var prefix = request.Dto.Tip == SiteYonetimi.Shared.Enums.FaturaTipi.Gelir ? "GF" : "EF";
        var count = await _db.Faturalar.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId && x.Tip == request.Dto.Tip, cancellationToken);
        var evrakNo = $"{prefix}-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}";

        var entity = new Fatura
        {
            SiteId = request.SiteId,
            Tip = request.Dto.Tip,
            EvrakNo = evrakNo,
            IslemTarihi = DateTime.UtcNow,
            FaturaTarihi = request.Dto.FaturaTarihi,
            CariAdi = request.Dto.CariAdi,
            GelirTanimiId = request.Dto.GelirTanimiId,
            GiderTanimiId = request.Dto.GiderTanimiId,
            ToplamTutar = request.Dto.ToplamTutar,
            Aciklama = request.Dto.Aciklama,
            SonOdemeTarihi = request.Dto.SonOdemeTarihi,
            OdemeDurumu = request.Dto.Tip == SiteYonetimi.Shared.Enums.FaturaTipi.Gelir
                ? SiteYonetimi.Shared.Enums.FaturaOdemeDurumu.Odendi
                : SiteYonetimi.Shared.Enums.FaturaOdemeDurumu.Odenmedi
        };

        _db.Faturalar.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateFaturaCommand(Guid Id, Guid SiteId, UpdateFaturaDto Dto) : IRequest<Result>;

public class UpdateFaturaCommandHandler : IRequestHandler<UpdateFaturaCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateFaturaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateFaturaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Faturalar
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Fatura bulunamadı.");

        entity.FaturaTarihi = request.Dto.FaturaTarihi;
        entity.CariAdi = request.Dto.CariAdi;
        entity.GelirTanimiId = request.Dto.GelirTanimiId;
        entity.GiderTanimiId = request.Dto.GiderTanimiId;
        entity.ToplamTutar = request.Dto.ToplamTutar;
        entity.Aciklama = request.Dto.Aciklama;
        entity.SonOdemeTarihi = request.Dto.SonOdemeTarihi;
        if (request.Dto.OdemeDurumu.HasValue) entity.OdemeDurumu = request.Dto.OdemeDurumu.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteFaturaCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteFaturaCommandHandler : IRequestHandler<DeleteFaturaCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteFaturaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteFaturaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Faturalar
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Fatura bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
