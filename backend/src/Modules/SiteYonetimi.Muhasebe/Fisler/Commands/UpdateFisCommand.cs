using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Commands;

/// <summary>Taslak fişi günceller (satırlar tamamen yeniden yazılır). Onaylı fiş düzenlenemez.</summary>
public record UpdateFisCommand(Guid SiteId, Guid Id, UpdateFisDto Dto) : IRequest<Result>;

public class UpdateFisCommandHandler : IRequestHandler<UpdateFisCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IFisService _fisService;

    public UpdateFisCommandHandler(SharedTenantDbContext db, IFisService fisService)
    {
        _db = db;
        _fisService = fisService;
    }

    public async Task<Result> Handle(UpdateFisCommand request, CancellationToken cancellationToken)
    {
        var fis = await _db.MuhasebeFisleri
            .Include(f => f.Detaylar)
            .FirstOrDefaultAsync(f => f.SiteId == request.SiteId && f.Id == request.Id, cancellationToken);
        if (fis is null)
            return Result.Failure("Fiş bulunamadı.");
        if (fis.Durum != FisDurumu.Taslak)
            return Result.Failure("Yalnızca taslak fişler düzenlenebilir.");

        // Dönem kapalı kontrolü (tarih değişebileceği için yeni tarihin dönemine bak).
        var donemResult = await _fisService.ResolveAcikDonemAsync(request.SiteId, request.Dto.FisTarihi.Year, cancellationToken);
        if (!donemResult.IsSuccess) return Result.Failure(donemResult.Error!);
        var donem = donemResult.Data!;

        var detaylarResult = await _fisService.BuildDetaylarAsync(request.SiteId, fis.Id, request.Dto.Detaylar, cancellationToken);
        if (!detaylarResult.IsSuccess) return Result.Failure(detaylarResult.Error!);
        var yeniDetaylar = detaylarResult.Data!;

        // Eski satırları sil, yenilerini ekle.
        _db.MuhasebeFisiDetaylari.RemoveRange(fis.Detaylar);
        _db.MuhasebeFisiDetaylari.AddRange(yeniDetaylar);

        fis.DonemId = donem.Id;
        fis.FisTuru = request.Dto.FisTuru;
        fis.FisTarihi = request.Dto.FisTarihi;
        fis.Aciklama = request.Dto.Aciklama?.Trim() ?? string.Empty;
        fis.ToplamBorc = yeniDetaylar.Sum(d => d.BorcTutar);
        fis.ToplamAlacak = yeniDetaylar.Sum(d => d.AlacakTutar);
        fis.SatirSayisi = yeniDetaylar.Count;
        fis.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UpdateFisDtoValidator : AbstractValidator<UpdateFisDto>
{
    public UpdateFisDtoValidator()
    {
        RuleFor(x => x.FisTarihi).NotEqual(default(DateOnly)).WithMessage("Fiş tarihi zorunludur.");
        RuleFor(x => x.Aciklama).MaximumLength(500);
        RuleForEach(x => x.Detaylar).ChildRules(d =>
        {
            d.RuleFor(s => s.Aciklama).MaximumLength(500);
            d.RuleFor(s => s.BelgeNo).MaximumLength(100);
        });
    }
}
