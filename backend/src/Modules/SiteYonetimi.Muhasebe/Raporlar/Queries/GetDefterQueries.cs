using MediatR;
using SiteYonetimi.Muhasebe.Raporlar.DTOs;
using SiteYonetimi.Muhasebe.Raporlar.Services;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Raporlar.Queries;

/// <summary>Defter-i Kebir: bir hesabın (alt hesapları dahil) hareketleri + bakiye.</summary>
public record GetDefteriKebirQuery(Guid SiteId, Guid HesapId, DateOnly? From = null, DateOnly? To = null)
    : IRequest<Result<DefterDto>>;

public class GetDefteriKebirQueryHandler : IRequestHandler<GetDefteriKebirQuery, Result<DefterDto>>
{
    private readonly IRaporService _rapor;
    public GetDefteriKebirQueryHandler(IRaporService rapor) => _rapor = rapor;

    public Task<Result<DefterDto>> Handle(GetDefteriKebirQuery request, CancellationToken cancellationToken)
        => _rapor.HesapDefteriAsync(request.SiteId, request.HesapId, request.From, request.To, true, cancellationToken);
}

/// <summary>Muavin defter: tek hesabın detay hareketleri + bakiye.</summary>
public record GetMuavinDefterQuery(Guid SiteId, Guid HesapId, DateOnly? From = null, DateOnly? To = null)
    : IRequest<Result<DefterDto>>;

public class GetMuavinDefterQueryHandler : IRequestHandler<GetMuavinDefterQuery, Result<DefterDto>>
{
    private readonly IRaporService _rapor;
    public GetMuavinDefterQueryHandler(IRaporService rapor) => _rapor = rapor;

    public Task<Result<DefterDto>> Handle(GetMuavinDefterQuery request, CancellationToken cancellationToken)
        => _rapor.HesapDefteriAsync(request.SiteId, request.HesapId, request.From, request.To, false, cancellationToken);
}

/// <summary>Cari hesap ekstresi: tek cari hesabın hareket dökümü + bakiye.</summary>
public record GetCariEkstreQuery(Guid SiteId, Guid HesapId, DateOnly? From = null, DateOnly? To = null)
    : IRequest<Result<DefterDto>>;

public class GetCariEkstreQueryHandler : IRequestHandler<GetCariEkstreQuery, Result<DefterDto>>
{
    private readonly IRaporService _rapor;
    public GetCariEkstreQueryHandler(IRaporService rapor) => _rapor = rapor;

    public Task<Result<DefterDto>> Handle(GetCariEkstreQuery request, CancellationToken cancellationToken)
        => _rapor.HesapDefteriAsync(request.SiteId, request.HesapId, request.From, request.To, false, cancellationToken);
}
