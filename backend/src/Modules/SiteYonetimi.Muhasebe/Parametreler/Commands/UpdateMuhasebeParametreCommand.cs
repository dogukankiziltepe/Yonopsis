using MediatR;
using SiteYonetimi.Muhasebe.Parametreler.DTOs;
using SiteYonetimi.Muhasebe.Parametreler.Services;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Parametreler.Commands;

public record UpdateMuhasebeParametreCommand(Guid SiteId, UpdateMuhasebeParametreDto Dto) : IRequest<Result>;

public class UpdateMuhasebeParametreCommandHandler : IRequestHandler<UpdateMuhasebeParametreCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IParametreService _parametreService;

    public UpdateMuhasebeParametreCommandHandler(SharedTenantDbContext db, IParametreService parametreService)
    {
        _db = db;
        _parametreService = parametreService;
    }

    public async Task<Result> Handle(UpdateMuhasebeParametreCommand request, CancellationToken cancellationToken)
    {
        var p = await _parametreService.GetOrCreateAsync(request.SiteId, cancellationToken);
        var dto = request.Dto;

        p.VarsayilanKasaHesapId = dto.VarsayilanKasaHesapId;
        p.VarsayilanBankaHesapId = dto.VarsayilanBankaHesapId;
        p.AidatGelirHesapId = dto.AidatGelirHesapId;
        p.GecikmeFaiziHesapId = dto.GecikmeFaiziHesapId;
        p.AlicilarAnaHesapKodu = dto.AlicilarAnaHesapKodu?.Trim() ?? "120";
        p.SaticilarAnaHesapKodu = dto.SaticilarAnaHesapKodu?.Trim() ?? "320";
        p.GiderAnaHesapKodu = dto.GiderAnaHesapKodu?.Trim() ?? "770";
        p.CariKodSablonu = dto.CariKodSablonu?.Trim() ?? p.CariKodSablonu;
        p.FisNoSablonu = dto.FisNoSablonu?.Trim() ?? p.FisNoSablonu;
        p.ParaBirimi = string.IsNullOrWhiteSpace(dto.ParaBirimi) ? "TRY" : dto.ParaBirimi.Trim();
        p.KdvOrani = dto.KdvOrani;
        p.OtomatikTahsilFisi = dto.OtomatikTahsilFisi;
        p.OtomatikTediyeFisi = dto.OtomatikTediyeFisi;
        p.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
