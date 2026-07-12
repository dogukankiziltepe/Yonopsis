using MediatR;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Report.DTOs;
using SiteYonetimi.SiteManagement.Report.Services;

namespace SiteYonetimi.SiteManagement.Report.Queries;

public record GetReportSummaryQuery(Guid SiteId) : IRequest<Result<ReportSummaryDto>>;
public record GetKasalarQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<KasaBakiyeDto>>>;
public record GetIsTakibiQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<IsTakibiOgesiDto>>>;
public record GetAidatTahsilatQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<AidatTahsilatAyDto>>>;
public record GetFinansalDurumQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<FinansalDurumNoktaDto>>>;
public record GetGiderEvraklariQuery(Guid SiteId) : IRequest<Result<List<EvrakDto>>>;
public record GetGelirEvraklariQuery(Guid SiteId) : IRequest<Result<List<EvrakDto>>>;
public record GetOdenecekFaturalarQuery(Guid SiteId) : IRequest<Result<List<OdenecekFaturaDto>>>;
public record GetGiderDagilimiQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<DagilimDilimiDto>>>;
public record GetGelirDagilimiQuery(Guid SiteId, bool All, DateTime? From, DateTime? To) : IRequest<Result<List<DagilimDilimiDto>>>;
public record GetReportDuyurularQuery(Guid SiteId) : IRequest<Result<List<DuyuruOzetDto>>>;
public record GetReportBankaHesaplariQuery(Guid SiteId) : IRequest<Result<List<BankaHesabiDto>>>;

public class GetReportSummaryQueryHandler : IRequestHandler<GetReportSummaryQuery, Result<ReportSummaryDto>>
{
    private readonly IReportService _service;
    public GetReportSummaryQueryHandler(IReportService service) => _service = service;
    public async Task<Result<ReportSummaryDto>> Handle(GetReportSummaryQuery request, CancellationToken ct)
        => Result<ReportSummaryDto>.Success(await _service.GetSummaryAsync(request.SiteId, ct));
}

public class GetKasalarQueryHandler : IRequestHandler<GetKasalarQuery, Result<List<KasaBakiyeDto>>>
{
    private readonly IReportService _service;
    public GetKasalarQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<KasaBakiyeDto>>> Handle(GetKasalarQuery request, CancellationToken ct)
        => Result<List<KasaBakiyeDto>>.Success(await _service.GetKasalarAsync(request.SiteId, request.All, request.From, request.To, ct));
}

public class GetIsTakibiQueryHandler : IRequestHandler<GetIsTakibiQuery, Result<List<IsTakibiOgesiDto>>>
{
    private readonly IReportService _service;
    public GetIsTakibiQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<IsTakibiOgesiDto>>> Handle(GetIsTakibiQuery request, CancellationToken ct)
        => Result<List<IsTakibiOgesiDto>>.Success(await _service.GetIsTakibiAsync(request.SiteId, request.All, request.From, request.To, ct));
}

public class GetAidatTahsilatQueryHandler : IRequestHandler<GetAidatTahsilatQuery, Result<List<AidatTahsilatAyDto>>>
{
    private readonly IReportService _service;
    public GetAidatTahsilatQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<AidatTahsilatAyDto>>> Handle(GetAidatTahsilatQuery request, CancellationToken ct)
        => Result<List<AidatTahsilatAyDto>>.Success(await _service.GetAidatTahsilatAsync(request.SiteId, request.All, request.From, request.To, ct));
}

public class GetFinansalDurumQueryHandler : IRequestHandler<GetFinansalDurumQuery, Result<List<FinansalDurumNoktaDto>>>
{
    private readonly IReportService _service;
    public GetFinansalDurumQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<FinansalDurumNoktaDto>>> Handle(GetFinansalDurumQuery request, CancellationToken ct)
        => Result<List<FinansalDurumNoktaDto>>.Success(await _service.GetFinansalDurumAsync(request.SiteId, request.All, request.From, request.To, ct));
}

public class GetGiderEvraklariQueryHandler : IRequestHandler<GetGiderEvraklariQuery, Result<List<EvrakDto>>>
{
    private readonly IReportService _service;
    public GetGiderEvraklariQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<EvrakDto>>> Handle(GetGiderEvraklariQuery request, CancellationToken ct)
        => Result<List<EvrakDto>>.Success(await _service.GetEvraklarAsync(request.SiteId, FaturaTipi.Gider, ct));
}

public class GetGelirEvraklariQueryHandler : IRequestHandler<GetGelirEvraklariQuery, Result<List<EvrakDto>>>
{
    private readonly IReportService _service;
    public GetGelirEvraklariQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<EvrakDto>>> Handle(GetGelirEvraklariQuery request, CancellationToken ct)
        => Result<List<EvrakDto>>.Success(await _service.GetEvraklarAsync(request.SiteId, FaturaTipi.Gelir, ct));
}

public class GetOdenecekFaturalarQueryHandler : IRequestHandler<GetOdenecekFaturalarQuery, Result<List<OdenecekFaturaDto>>>
{
    private readonly IReportService _service;
    public GetOdenecekFaturalarQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<OdenecekFaturaDto>>> Handle(GetOdenecekFaturalarQuery request, CancellationToken ct)
        => Result<List<OdenecekFaturaDto>>.Success(await _service.GetOdenecekFaturalarAsync(request.SiteId, ct));
}

public class GetGiderDagilimiQueryHandler : IRequestHandler<GetGiderDagilimiQuery, Result<List<DagilimDilimiDto>>>
{
    private readonly IReportService _service;
    public GetGiderDagilimiQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<DagilimDilimiDto>>> Handle(GetGiderDagilimiQuery request, CancellationToken ct)
        => Result<List<DagilimDilimiDto>>.Success(await _service.GetDagilimAsync(request.SiteId, FaturaTipi.Gider, request.All, request.From, request.To, ct));
}

public class GetGelirDagilimiQueryHandler : IRequestHandler<GetGelirDagilimiQuery, Result<List<DagilimDilimiDto>>>
{
    private readonly IReportService _service;
    public GetGelirDagilimiQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<DagilimDilimiDto>>> Handle(GetGelirDagilimiQuery request, CancellationToken ct)
        => Result<List<DagilimDilimiDto>>.Success(await _service.GetDagilimAsync(request.SiteId, FaturaTipi.Gelir, request.All, request.From, request.To, ct));
}

public class GetReportDuyurularQueryHandler : IRequestHandler<GetReportDuyurularQuery, Result<List<DuyuruOzetDto>>>
{
    private readonly IReportService _service;
    public GetReportDuyurularQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<DuyuruOzetDto>>> Handle(GetReportDuyurularQuery request, CancellationToken ct)
        => Result<List<DuyuruOzetDto>>.Success(await _service.GetDuyurularAsync(request.SiteId, ct));
}

public class GetReportBankaHesaplariQueryHandler : IRequestHandler<GetReportBankaHesaplariQuery, Result<List<BankaHesabiDto>>>
{
    private readonly IReportService _service;
    public GetReportBankaHesaplariQueryHandler(IReportService service) => _service = service;
    public async Task<Result<List<BankaHesabiDto>>> Handle(GetReportBankaHesaplariQuery request, CancellationToken ct)
        => Result<List<BankaHesabiDto>>.Success(await _service.GetBankaHesaplariAsync(request.SiteId, ct));
}
