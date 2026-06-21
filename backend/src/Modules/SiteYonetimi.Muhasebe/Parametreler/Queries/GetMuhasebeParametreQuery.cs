using MediatR;
using SiteYonetimi.Muhasebe.Parametreler.DTOs;
using SiteYonetimi.Muhasebe.Parametreler.Services;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Parametreler.Queries;

public record GetMuhasebeParametreQuery(Guid SiteId) : IRequest<Result<MuhasebeParametreDto>>;

public class GetMuhasebeParametreQueryHandler : IRequestHandler<GetMuhasebeParametreQuery, Result<MuhasebeParametreDto>>
{
    private readonly IParametreService _parametreService;

    public GetMuhasebeParametreQueryHandler(IParametreService parametreService)
    {
        _parametreService = parametreService;
    }

    public async Task<Result<MuhasebeParametreDto>> Handle(GetMuhasebeParametreQuery request, CancellationToken cancellationToken)
    {
        var p = await _parametreService.GetOrCreateAsync(request.SiteId, cancellationToken);
        var dto = new MuhasebeParametreDto(
            p.Id, p.VarsayilanKasaHesapId, p.VarsayilanBankaHesapId, p.AidatGelirHesapId, p.GecikmeFaiziHesapId,
            p.AlicilarAnaHesapKodu, p.SaticilarAnaHesapKodu, p.GiderAnaHesapKodu,
            p.CariKodSablonu, p.FisNoSablonu, p.ParaBirimi, p.KdvOrani,
            p.OtomatikTahsilFisi, p.OtomatikTediyeFisi);
        return Result<MuhasebeParametreDto>.Success(dto);
    }
}
