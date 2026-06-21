using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Commands;

/// <summary>Yeni fiş oluşturur (Taslak durumunda). Denge kontrolü onaylamada yapılır.</summary>
public record CreateFisCommand(Guid SiteId, CreateFisDto Dto) : IRequest<Result<Guid>>;

public class CreateFisCommandHandler : IRequestHandler<CreateFisCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    private readonly IFisService _fisService;

    public CreateFisCommandHandler(SharedTenantDbContext db, IFisService fisService)
    {
        _db = db;
        _fisService = fisService;
    }

    public async Task<Result<Guid>> Handle(CreateFisCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var donemResult = await _fisService.ResolveAcikDonemAsync(request.SiteId, dto.FisTarihi.Year, cancellationToken);
        if (!donemResult.IsSuccess) return Result<Guid>.Failure(donemResult.Error!);
        var donem = donemResult.Data!;

        var fisId = Guid.NewGuid();
        var detaylarResult = await _fisService.BuildDetaylarAsync(request.SiteId, fisId, dto.Detaylar, cancellationToken);
        if (!detaylarResult.IsSuccess) return Result<Guid>.Failure(detaylarResult.Error!);
        var detaylar = detaylarResult.Data!;

        var fisNo = await _fisService.GenerateFisNoAsync(request.SiteId, donem.Yil, cancellationToken);

        var fis = new MuhasebeFisi
        {
            Id = fisId,
            SiteId = request.SiteId,
            DonemId = donem.Id,
            YevmiyeNo = null,
            FisNo = fisNo,
            FisTuru = dto.FisTuru,
            FisTarihi = dto.FisTarihi,
            Aciklama = dto.Aciklama?.Trim() ?? string.Empty,
            Durum = FisDurumu.Taslak,
            ToplamBorc = detaylar.Sum(d => d.BorcTutar),
            ToplamAlacak = detaylar.Sum(d => d.AlacakTutar),
            SatirSayisi = detaylar.Count,
            Detaylar = detaylar
        };

        _db.MuhasebeFisleri.Add(fis);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(fis.Id);
    }
}

public class CreateFisDtoValidator : AbstractValidator<CreateFisDto>
{
    public CreateFisDtoValidator()
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
