using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Services;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Commands;

internal static class VirmanSatirKaydi
{
    public static async Task<(string? HesapAdi, string? Error)> DogrulaAsync(
        SharedTenantDbContext db, MasterDbContext masterDb, Guid siteId, SaveVirmanSatirDto dto, CancellationToken ct)
    {
        var fisVar = await db.HesaplarArasiVirmanlar.AnyAsync(v => v.Id == dto.VirmanId && v.SiteId == siteId, ct);
        if (!fisVar) return (null, "Virman fişi bulunamadı.");

        var tutarHatasi = VirmanSatirKurallari.TutarHatasi(dto.BorcTutari, dto.AlacakTutari);
        if (tutarHatasi is not null) return (null, tutarHatasi);

        if (dto.AylikTazminatYuzdesi is < 0 or > 100) return (null, "Tazminat yüzdesi 0 ile 100 arasında olmalıdır.");
        if (dto.BorcDonemi is { Length: > 0 } && !System.Text.RegularExpressions.Regex.IsMatch(dto.BorcDonemi, @"^\d{4}-\d{2}$"))
            return (null, "Borç dönemi YYYY-AA formatında olmalıdır.");

        var daireHatasi = await VirmanSatirKurallari.DaireVeKategoriHatasiAsync(db, siteId, new[] { dto.UnitId }, new[] { dto.GelirTanimiId }, ct);
        if (daireHatasi is not null) return (null, daireHatasi);

        var (adlar, hesapHatasi) = await VirmanSatirKurallari.HesaplariCozAsync(db, masterDb, siteId, new[] { (dto.HesapTuru, dto.HesapId) }, ct);
        return hesapHatasi is not null ? (null, hesapHatasi) : (adlar![(dto.HesapTuru, dto.HesapId)], null);
    }

    public static void Uygula(VirmanSatiri s, SaveVirmanSatirDto dto, string hesapAdi)
    {
        s.VirmanId = dto.VirmanId;
        s.BorcDonemi = string.IsNullOrWhiteSpace(dto.BorcDonemi) ? null : dto.BorcDonemi;
        s.HesapTuru = dto.HesapTuru;
        s.HesapId = dto.HesapId;
        s.HesapAdiSnapshot = hesapAdi;
        s.UnitId = dto.UnitId;
        s.GelirTanimiId = dto.GelirTanimiId;
        s.GecikmeTazminatiUygula = dto.GecikmeTazminatiUygula;
        s.TazminatBaslamaTarihi = dto.TazminatBaslamaTarihi;
        s.SonOdemeTarihi = dto.SonOdemeTarihi;
        s.TazminatUygulamaSekli = dto.TazminatUygulamaSekli;
        s.AylikTazminatYuzdesi = dto.AylikTazminatYuzdesi;
        s.TazminatHesapTarihi = dto.TazminatHesapTarihi;
        s.Aciklama = dto.Aciklama;
        s.BorcTutari = dto.BorcTutari;
        s.AlacakTutari = dto.AlacakTutari;
        s.IcraTakibinde = dto.IcraTakibinde;
        s.IcrayaVerilmeTarihi = dto.IcraTakibinde ? dto.IcrayaVerilmeTarihi : null;
        s.IcraDosyaNo = dto.IcraTakibinde ? dto.IcraDosyaNo : null;
    }
}

// ── Create ──────────────────────────────────────────────────────────────────
public record CreateVirmanSatirCommand(Guid SiteId, SaveVirmanSatirDto Dto) : IRequest<Result<Guid>>;

public class CreateVirmanSatirCommandHandler : IRequestHandler<CreateVirmanSatirCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public CreateVirmanSatirCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<Guid>> Handle(CreateVirmanSatirCommand request, CancellationToken cancellationToken)
    {
        var (hesapAdi, error) = await VirmanSatirKaydi.DogrulaAsync(_db, _masterDb, request.SiteId, request.Dto, cancellationToken);
        if (error is not null) return Result<Guid>.Failure(error);

        var sonSira = await _db.VirmanSatirlari
            .Where(s => s.VirmanId == request.Dto.VirmanId)
            .MaxAsync(s => (int?)s.SiraNo, cancellationToken) ?? 0;

        var satir = new VirmanSatiri { SiteId = request.SiteId, SiraNo = sonSira + 1 };
        VirmanSatirKaydi.Uygula(satir, request.Dto, hesapAdi!);

        _db.VirmanSatirlari.Add(satir);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(satir.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdateVirmanSatirCommand(Guid Id, Guid SiteId, SaveVirmanSatirDto Dto) : IRequest<Result>;

public class UpdateVirmanSatirCommandHandler : IRequestHandler<UpdateVirmanSatirCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;
    public UpdateVirmanSatirCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result> Handle(UpdateVirmanSatirCommand request, CancellationToken cancellationToken)
    {
        var satir = await _db.VirmanSatirlari
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.SiteId == request.SiteId && !s.Virman.IsDeleted, cancellationToken);
        if (satir is null) return Result.Failure("Virman satırı bulunamadı.");

        var (hesapAdi, error) = await VirmanSatirKaydi.DogrulaAsync(_db, _masterDb, request.SiteId, request.Dto, cancellationToken);
        if (error is not null) return Result.Failure(error);

        // Başka fişe taşındıysa o fişin sonuna eklenir
        if (satir.VirmanId != request.Dto.VirmanId)
        {
            satir.SiraNo = (await _db.VirmanSatirlari
                .Where(s => s.VirmanId == request.Dto.VirmanId)
                .MaxAsync(s => (int?)s.SiraNo, cancellationToken) ?? 0) + 1;
        }

        VirmanSatirKaydi.Uygula(satir, request.Dto, hesapAdi!);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeleteVirmanSatirCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteVirmanSatirCommandHandler : IRequestHandler<DeleteVirmanSatirCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteVirmanSatirCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteVirmanSatirCommand request, CancellationToken cancellationToken)
    {
        var satir = await _db.VirmanSatirlari
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.SiteId == request.SiteId && !s.Virman.IsDeleted, cancellationToken);
        if (satir is null) return Result.Failure("Virman satırı bulunamadı.");

        // Satırlarda soft-delete yok; fiş ekranı da satırları fiziksel olarak siler
        _db.VirmanSatirlari.Remove(satir);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
