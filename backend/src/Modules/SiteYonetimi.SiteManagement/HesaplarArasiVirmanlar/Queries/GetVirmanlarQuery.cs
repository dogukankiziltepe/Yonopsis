using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.DTOs;

namespace SiteYonetimi.SiteManagement.HesaplarArasiVirmanlar.Queries;

public record GetVirmanlarQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<VirmanListItemDto>>>;

public class GetVirmanlarQueryHandler : IRequestHandler<GetVirmanlarQuery, Result<PaginatedResult<VirmanListItemDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVirmanlarQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<VirmanListItemDto>>> Handle(GetVirmanlarQuery request, CancellationToken cancellationToken)
    {
        var query = _db.HesaplarArasiVirmanlar.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.EvrakNo.ToLower().Contains(term) ||
                (x.BelgeNo != null && x.BelgeNo.ToLower().Contains(term)) ||
                (x.Aciklama != null && x.Aciklama.ToLower().Contains(term)) ||
                x.Satirlar.Any(s => s.HesapAdiSnapshot != null && s.HesapAdiSnapshot.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.IslemTarihi)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.Id, x.EvrakNo, x.IslemTarihi, x.Tarih,
                SatirSayisi = x.Satirlar.Count,
                Borc = x.Satirlar.Sum(s => (decimal?)s.BorcTutari) ?? 0m,
                Alacak = x.Satirlar.Sum(s => (decimal?)s.AlacakTutari) ?? 0m
            })
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x => new VirmanListItemDto(
            x.Id, x.EvrakNo, x.IslemTarihi, x.Tarih, x.SatirSayisi, x.Borc, x.Alacak, x.Borc - x.Alacak)).ToList();

        return Result<PaginatedResult<VirmanListItemDto>>.Success(
            PaginatedResult<VirmanListItemDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}

public record GetVirmanSecimListesiQuery(Guid SiteId) : IRequest<Result<List<VirmanSecimDto>>>;

public class GetVirmanSecimListesiQueryHandler : IRequestHandler<GetVirmanSecimListesiQuery, Result<List<VirmanSecimDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetVirmanSecimListesiQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<VirmanSecimDto>>> Handle(GetVirmanSecimListesiQuery request, CancellationToken cancellationToken)
    {
        var list = await _db.HesaplarArasiVirmanlar
            .Where(x => x.SiteId == request.SiteId)
            .OrderByDescending(x => x.Tarih).ThenByDescending(x => x.IslemTarihi)
            .Take(500)
            .Select(x => new VirmanSecimDto(x.Id, x.EvrakNo, x.Tarih, x.BelgeNo))
            .ToListAsync(cancellationToken);
        return Result<List<VirmanSecimDto>>.Success(list);
    }
}

public record GetVirmanByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<VirmanDetayDto>>;

public class GetVirmanByIdQueryHandler : IRequestHandler<GetVirmanByIdQuery, Result<VirmanDetayDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetVirmanByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<VirmanDetayDto>> Handle(GetVirmanByIdQuery request, CancellationToken cancellationToken)
    {
        var virman = await _db.HesaplarArasiVirmanlar
            .Include(x => x.Satirlar).ThenInclude(s => s.Unit)
            .Include(x => x.Satirlar).ThenInclude(s => s.GelirTanimi)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (virman is null) return Result<VirmanDetayDto>.Failure("Virman fişi bulunamadı.");

        var satirlar = virman.Satirlar.OrderBy(s => s.SiraNo).Select(s => new VirmanSatiriDto(
            s.Id, s.HesapTuru, s.HesapId, s.HesapAdiSnapshot,
            s.UnitId, s.Unit?.DoorNumber,
            s.GelirTanimiId, s.GelirTanimi?.Name,
            s.Aciklama, s.BorcTutari, s.AlacakTutari)).ToList();

        return Result<VirmanDetayDto>.Success(new VirmanDetayDto(
            virman.Id, virman.EvrakNo, virman.IslemTarihi, virman.Tarih,
            virman.BelgeTarihi, virman.BelgeNo, virman.Aciklama, satirlar));
    }
}
