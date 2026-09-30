using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Queries;

public record GetVirmanSatirlariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<VirmanSatirListItemDto>>>;

public class GetVirmanSatirlariQueryHandler : IRequestHandler<GetVirmanSatirlariQuery, Result<PaginatedResult<VirmanSatirListItemDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVirmanSatirlariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<VirmanSatirListItemDto>>> Handle(GetVirmanSatirlariQuery request, CancellationToken cancellationToken)
    {
        var query = _db.VirmanSatirlari.Where(s => s.SiteId == request.SiteId && !s.Virman.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(s =>
                s.Virman.EvrakNo.ToLower().Contains(term) ||
                (s.HesapAdiSnapshot != null && s.HesapAdiSnapshot.ToLower().Contains(term)) ||
                (s.Unit != null && s.Unit.DoorNumber.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(s => s.Virman.Tarih).ThenBy(s => s.Virman.EvrakNo).ThenBy(s => s.SiraNo)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new VirmanSatirListItemDto(
                s.Id, s.VirmanId, s.Virman.EvrakNo, s.Virman.Tarih, s.Virman.BelgeTarihi, s.SonOdemeTarihi,
                s.Unit != null ? s.Unit.DoorNumber : null,
                s.HesapTuru, s.HesapAdiSnapshot, s.BorcTutari, s.AlacakTutari))
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<VirmanSatirListItemDto>>.Success(
            PaginatedResult<VirmanSatirListItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}

public record GetVirmanSatirByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<VirmanSatirDetayDto>>;

public class GetVirmanSatirByIdQueryHandler : IRequestHandler<GetVirmanSatirByIdQuery, Result<VirmanSatirDetayDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetVirmanSatirByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<VirmanSatirDetayDto>> Handle(GetVirmanSatirByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _db.VirmanSatirlari
            .Where(s => s.Id == request.Id && s.SiteId == request.SiteId && !s.Virman.IsDeleted)
            .Select(s => new VirmanSatirDetayDto(
                s.Id, s.VirmanId, s.Virman.EvrakNo, s.Virman.Tarih, s.Virman.BelgeTarihi, s.Virman.BelgeNo, s.Virman.Aciklama,
                s.BorcDonemi, s.HesapTuru, s.HesapId, s.HesapAdiSnapshot, s.UnitId, s.GelirTanimiId,
                s.GecikmeTazminatiUygula, s.TazminatBaslamaTarihi, s.SonOdemeTarihi, s.TazminatUygulamaSekli,
                s.AylikTazminatYuzdesi, s.TazminatHesapTarihi, s.Aciklama, s.BorcTutari, s.AlacakTutari,
                s.IcraTakibinde, s.IcrayaVerilmeTarihi, s.IcraDosyaNo))
            .FirstOrDefaultAsync(cancellationToken);

        return dto is null
            ? Result<VirmanSatirDetayDto>.Failure("Virman satırı bulunamadı.")
            : Result<VirmanSatirDetayDto>.Success(dto);
    }
}
