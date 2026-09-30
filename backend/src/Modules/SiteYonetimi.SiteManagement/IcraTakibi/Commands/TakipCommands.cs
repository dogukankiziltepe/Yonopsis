using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;
using SiteYonetimi.SiteManagement.IcraTakibi.Services;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Commands;

// ── Takip Başlat ────────────────────────────────────────────────────────────
/// <summary>
/// Seçilen her (borçlu, daire) için bir takip açar. Takibe bağlanacak makbuzlar, listelemede
/// kullanılan aynı filtre ile sunucuda yeniden hesaplanır; dönen değer açılan takip sayısıdır.
/// </summary>
public record TakipBaslatCommand(Guid SiteId, TakipBaslatDto Dto) : IRequest<Result<int>>;

public class TakipBaslatCommandHandler : IRequestHandler<TakipBaslatCommand, Result<int>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public TakipBaslatCommandHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<int>> Handle(TakipBaslatCommand request, CancellationToken cancellationToken)
    {
        var secimler = request.Dto.Secimler.Distinct().ToList();
        if (secimler.Count == 0) return Result<int>.Failure("Takip başlatmak için en az bir kayıt seçilmelidir.");

        var userIds = secimler.Select(s => (Guid?)s.BorcluUserId).Distinct().ToList();
        var unitIds = secimler.Select(s => (Guid?)s.UnitId).Distinct().ToList();
        var makbuzlar = await IcraHesaplari.AcikMakbuzlar(_db, request.SiteId, request.Dto.Filtre)
            .Where(b => userIds.Contains(b.BorcluUserId) && unitIds.Contains(b.UnitId))
            .Select(b => new { b.Id, UserId = b.BorcluUserId!.Value, UnitId = b.UnitId!.Value, Kalan = b.Tutar + b.GecikmeTutari - b.OdenenTutar, b.BorcluAdiSnapshot })
            .ToListAsync(cancellationToken);

        var kisiler = await IcraHesaplari.IletisimAsync(_masterDb, request.SiteId, secimler.Select(s => s.BorcluUserId), cancellationToken);
        var bugun = DateTime.UtcNow.Date;
        var acilan = 0;

        foreach (var secim in secimler)
        {
            var grup = makbuzlar.Where(m => m.UserId == secim.BorcluUserId && m.UnitId == secim.UnitId).ToList();
            if (grup.Count == 0) continue; // bu arada ödenmiş ya da başka takibe alınmış olabilir

            var takip = new IcraTakip
            {
                SiteId = request.SiteId,
                TakipTarihi = bugun,
                BorcluUserId = secim.BorcluUserId,
                BorcluAdiSnapshot = kisiler.GetValueOrDefault(secim.BorcluUserId)?.Ad ?? grup[0].BorcluAdiSnapshot ?? string.Empty,
                UnitId = secim.UnitId,
                BaslangicTutari = grup.Sum(m => m.Kalan),
                Durum = TakipDurumu.Takipte
            };
            foreach (var m in grup) takip.Evraklar.Add(new IcraTakipEvrak { BorcMakbuzuId = m.Id });
            _db.IcraTakipleri.Add(takip);
            acilan++;
        }

        if (acilan == 0) return Result<int>.Failure("Seçilen kayıtlar için takibe alınabilecek açık borç bulunamadı.");
        await _db.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(acilan);
    }
}

// ── Takip güncelle (tarih + durum) ──────────────────────────────────────────
public record UpdateTakipCommand(Guid Id, Guid SiteId, UpdateTakipDto Dto) : IRequest<Result>;

public class UpdateTakipCommandHandler : IRequestHandler<UpdateTakipCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTakipCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateTakipCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var takip = await _db.IcraTakipleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (takip is null) return Result.Failure("Takip kaydı bulunamadı.");

        if (dto.Durum != takip.Durum)
        {
            if (dto.Durum == TakipDurumu.IcrayaVerildi)
                return Result.Failure("'İcraya Verildi' durumu yalnızca 'İcraya Ver' işlemiyle atanır.");
            if (takip.Durum == TakipDurumu.IcrayaVerildi)
                return Result.Failure("İcraya verilmiş takibin durumu değiştirilemez. Önce icra dosyasını silin.");
        }

        takip.TakipTarihi = dto.TakipTarihi.Date;
        takip.Durum = dto.Durum;
        takip.Aciklama = dto.Aciklama;
        takip.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── İcraya Ver ──────────────────────────────────────────────────────────────
/// <summary>Takipten icra dosyası açar; takipteki hâlâ kalanı olan makbuzlar "İcraya Verilen Evrak" olur.</summary>
public record IcrayaVerCommand(Guid TakipId, Guid SiteId, IcrayaVerDto Dto) : IRequest<Result<Guid>>;

public class IcrayaVerCommandHandler : IRequestHandler<IcrayaVerCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public IcrayaVerCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(IcrayaVerCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var takip = await _db.IcraTakipleri.FirstOrDefaultAsync(x => x.Id == request.TakipId && x.SiteId == request.SiteId, cancellationToken);
        if (takip is null) return Result<Guid>.Failure("Takip kaydı bulunamadı.");
        if (takip.Durum == TakipDurumu.IcrayaVerildi) return Result<Guid>.Failure("Bu takip zaten icraya verilmiş.");
        if (takip.Durum is TakipDurumu.Odendi or TakipDurumu.IptalEdildi)
            return Result<Guid>.Failure("Ödenmiş veya iptal edilmiş takip icraya verilemez.");

        var error = await IcraDosyasiValidation.ValidateAsync(_db, request.SiteId, null, dto.DosyaNo, dto.AvukatId, cancellationToken);
        if (error is not null) return Result<Guid>.Failure(error);

        var acikEvraklar = await _db.IcraTakipEvraklari.Where(e => e.TakipId == takip.Id)
            .Join(_db.BorcMakbuzlari, e => e.BorcMakbuzuId, b => b.Id, (e, b) => new { b.Id, Kalan = b.Tutar + b.GecikmeTutari - b.OdenenTutar })
            .Where(x => x.Kalan > 0)
            .ToListAsync(cancellationToken);
        if (acikEvraklar.Count == 0) return Result<Guid>.Failure("Takipte kalanı olan borç evrakı yok; icraya verilemez.");

        var dosya = new IcraDosyasi
        {
            SiteId = request.SiteId,
            TakipId = takip.Id,
            DosyaNo = dto.DosyaNo.Trim(),
            IcraTarihi = dto.IcraTarihi.Date,
            Durum = IcraDurumu.Icrada,
            BorcluUserId = takip.BorcluUserId,
            BorcluAdiSnapshot = takip.BorcluAdiSnapshot,
            UnitId = takip.UnitId,
            AvukatId = dto.AvukatId,
            DosyaTutari = acikEvraklar.Sum(x => x.Kalan),
            Aciklama = dto.Aciklama
        };
        foreach (var e in acikEvraklar) dosya.Evraklar.Add(new IcraDosyasiEvrak { BorcMakbuzuId = e.Id });
        _db.IcraDosyalari.Add(dosya);

        takip.Durum = TakipDurumu.IcrayaVerildi;
        takip.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(dosya.Id);
    }
}
