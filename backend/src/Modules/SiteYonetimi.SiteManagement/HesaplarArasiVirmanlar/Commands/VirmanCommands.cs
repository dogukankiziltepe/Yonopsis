using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Services;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Commands;

internal static class VirmanFisSatirlari
{
    /// <summary>Fiş ekranından gelen satırları doğrular, hesap adlarını çözer. Denge (ΣBorç = ΣAlacak) bilinçli olarak kontrol edilmez.</summary>
    public static async Task<(Dictionary<(VirmanHesapTuru, Guid), string>? Adlar, string? Error)> DogrulaAsync(
        SharedTenantDbContext db, MasterDbContext masterDb, Guid siteId, List<VirmanSatiriInputDto> input, CancellationToken ct)
    {
        if (input is null || input.Count == 0) return (null, "En az bir satır girilmelidir.");

        for (var i = 0; i < input.Count; i++)
        {
            if (input[i].HesapId == Guid.Empty) return (null, $"{i + 1}. satırda hesap seçilmelidir.");
            var hata = VirmanSatirKurallari.TutarHatasi(input[i].BorcTutari, input[i].AlacakTutari, $"{i + 1}. satırda ");
            if (hata is not null) return (null, hata);
        }

        var daireKategoriHatasi = await VirmanSatirKurallari.DaireVeKategoriHatasiAsync(
            db, siteId, input.Select(s => s.UnitId), input.Select(s => s.GelirTanimiId), ct);
        if (daireKategoriHatasi is not null) return (null, daireKategoriHatasi);

        return await VirmanSatirKurallari.HesaplariCozAsync(db, masterDb, siteId, input.Select(s => (s.HesapTuru, s.HesapId)), ct);
    }

    public static void Uygula(VirmanSatiri satir, VirmanSatiriInputDto s, int siraNo, string hesapAdi)
    {
        satir.SiraNo = siraNo;
        satir.HesapTuru = s.HesapTuru;
        satir.HesapId = s.HesapId;
        satir.HesapAdiSnapshot = hesapAdi;
        satir.UnitId = s.UnitId;
        satir.GelirTanimiId = s.GelirTanimiId;
        satir.Aciklama = s.Aciklama;
        satir.BorcTutari = s.BorcTutari;
        satir.AlacakTutari = s.AlacakTutari;
    }
}

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateVirmanCommand(Guid SiteId, SaveVirmanDto Dto) : IRequest<Result<Guid>>;

public class CreateVirmanCommandHandler : IRequestHandler<CreateVirmanCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public CreateVirmanCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<Guid>> Handle(CreateVirmanCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var (adlar, error) = await VirmanFisSatirlari.DogrulaAsync(_db, _masterDb, request.SiteId, dto.Satirlar, cancellationToken);
        if (error is not null) return Result<Guid>.Failure(error);

        var count = await _db.HesaplarArasiVirmanlar.IgnoreQueryFilters()
            .CountAsync(x => x.SiteId == request.SiteId, cancellationToken);

        var virman = new HesaplarArasiVirman
        {
            SiteId = request.SiteId,
            EvrakNo = $"HV-{request.SiteId.ToString()[..8].ToUpper()}-{count + 1:D5}",
            IslemTarihi = DateTime.UtcNow,
            Tarih = dto.Tarih,
            BelgeTarihi = dto.BelgeTarihi,
            BelgeNo = dto.BelgeNo,
            Aciklama = dto.Aciklama
        };
        for (var i = 0; i < dto.Satirlar.Count; i++)
        {
            var s = dto.Satirlar[i];
            var satir = new VirmanSatiri { SiteId = request.SiteId, VirmanId = virman.Id };
            VirmanFisSatirlari.Uygula(satir, s, i + 1, adlar![(s.HesapTuru, s.HesapId)]);
            virman.Satirlar.Add(satir);
        }

        _db.HesaplarArasiVirmanlar.Add(virman);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(virman.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateVirmanCommand(Guid Id, Guid SiteId, SaveVirmanDto Dto) : IRequest<Result>;

public class UpdateVirmanCommandHandler : IRequestHandler<UpdateVirmanCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public UpdateVirmanCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(UpdateVirmanCommand request, CancellationToken cancellationToken)
    {
        var virman = await _db.HesaplarArasiVirmanlar
            .Include(x => x.Satirlar)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (virman is null) return Result.Failure("Virman fişi bulunamadı.");

        var dto = request.Dto;
        var (adlar, error) = await VirmanFisSatirlari.DogrulaAsync(_db, _masterDb, request.SiteId, dto.Satirlar, cancellationToken);
        if (error is not null) return Result.Failure(error);

        // Satır bazlı upsert: mevcut satırın tazminat/icra/dönem alanları (Virman Fişi Detayları ekranından girilen) korunur
        var gelenIdler = dto.Satirlar.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToHashSet();
        _db.VirmanSatirlari.RemoveRange(virman.Satirlar.Where(s => !gelenIdler.Contains(s.Id)));

        for (var i = 0; i < dto.Satirlar.Count; i++)
        {
            var s = dto.Satirlar[i];
            var satir = s.Id.HasValue ? virman.Satirlar.FirstOrDefault(x => x.Id == s.Id.Value) : null;
            if (satir is null)
            {
                satir = new VirmanSatiri { SiteId = request.SiteId, VirmanId = virman.Id };
                _db.VirmanSatirlari.Add(satir);
            }
            VirmanFisSatirlari.Uygula(satir, s, i + 1, adlar![(s.HesapTuru, s.HesapId)]);
        }

        virman.Tarih = dto.Tarih;
        virman.BelgeTarihi = dto.BelgeTarihi;
        virman.BelgeNo = dto.BelgeNo;
        virman.Aciklama = dto.Aciklama;
        virman.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteVirmanCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteVirmanCommandHandler : IRequestHandler<DeleteVirmanCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteVirmanCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteVirmanCommand request, CancellationToken cancellationToken)
    {
        var virman = await _db.HesaplarArasiVirmanlar
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (virman is null) return Result.Failure("Virman fişi bulunamadı.");

        virman.IsDeleted = true;
        virman.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
