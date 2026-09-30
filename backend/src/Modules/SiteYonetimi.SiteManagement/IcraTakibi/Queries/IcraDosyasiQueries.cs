using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;
using SiteYonetimi.SiteManagement.IcraTakibi.Services;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Queries;

// ── İcra Listesi ────────────────────────────────────────────────────────────
public record GetIcraDosyalariQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PaginatedResult<IcraDosyasiListItemDto>>>;

public class GetIcraDosyalariQueryHandler : IRequestHandler<GetIcraDosyalariQuery, Result<PaginatedResult<IcraDosyasiListItemDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetIcraDosyalariQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<IcraDosyasiListItemDto>>> Handle(GetIcraDosyalariQuery request, CancellationToken cancellationToken)
    {
        var query = _db.IcraDosyalari.Where(x => x.SiteId == request.SiteId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x =>
                x.DosyaNo.ToLower().Contains(term) ||
                x.BorcluAdiSnapshot.ToLower().Contains(term) ||
                x.Unit.DoorNumber.ToLower().Contains(term) ||
                (x.Avukat != null && x.Avukat.AdSoyad.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.IcraTarihi).ThenByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.Id, x.IcraTarihi, x.DosyaNo, x.BorcluUserId, x.BorcluAdiSnapshot,
                BlokAdi = x.Unit.Building.Name, x.Unit.DoorNumber,
                AvukatAdi = x.Avukat != null ? x.Avukat.AdSoyad : null, x.Durum, x.DosyaTutari
            })
            .ToListAsync(cancellationToken);

        var kalanlar = await IcraHesaplari.DosyaKalanlariAsync(_db, rows.Select(r => r.Id).ToList(), cancellationToken);
        var items = rows.Select(r => new IcraDosyasiListItemDto(
            r.Id, r.IcraTarihi, r.DosyaNo, r.BorcluUserId, r.BorcluAdiSnapshot, r.BlokAdi, r.DoorNumber,
            r.AvukatAdi, r.Durum, r.DosyaTutari, kalanlar.GetValueOrDefault(r.Id).Kalan)).ToList();

        return Result<PaginatedResult<IcraDosyasiListItemDto>>.Success(
            PaginatedResult<IcraDosyasiListItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}

// ── İcra dosyası detayı ─────────────────────────────────────────────────────
public record GetIcraDosyasiByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<IcraDosyasiDetayDto>>;

public class GetIcraDosyasiByIdQueryHandler : IRequestHandler<GetIcraDosyasiByIdQuery, Result<IcraDosyasiDetayDto>>
{
    private readonly SharedTenantDbContext _db;
    public GetIcraDosyasiByIdQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<IcraDosyasiDetayDto>> Handle(GetIcraDosyasiByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _db.IcraDosyalari.Where(x => x.Id == request.Id && x.SiteId == request.SiteId)
            .Select(x => new
            {
                x.Id, x.DosyaNo, x.IcraTarihi, x.Durum, x.BorcluUserId, x.BorcluAdiSnapshot, x.UnitId,
                BlokAdi = x.Unit.Building.Name, x.Unit.DoorNumber,
                x.AvukatId, AvukatAdi = x.Avukat != null ? x.Avukat.AdSoyad : null,
                x.Aciklama, x.DosyaTutari, x.TakipId
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (d is null) return Result<IcraDosyasiDetayDto>.Failure("İcra dosyası bulunamadı.");

        var evraklar = await IcraHesaplari.EvraklarAsync(_db,
            _db.IcraDosyasiEvraklari.Where(e => e.DosyaId == d.Id).Select(e => e.BorcMakbuzuId), cancellationToken);

        return Result<IcraDosyasiDetayDto>.Success(new IcraDosyasiDetayDto(
            d.Id, d.DosyaNo, d.IcraTarihi, d.Durum, d.BorcluUserId, d.BorcluAdiSnapshot, d.UnitId, d.BlokAdi, d.DoorNumber,
            d.AvukatId, d.AvukatAdi, d.Aciklama, d.DosyaTutari, evraklar.Sum(e => e.Kalan), d.TakipId, evraklar));
    }
}

// ── İcra Raporu ─────────────────────────────────────────────────────────────
public record GetIcraRaporuQuery(Guid SiteId, IcraRaporFiltreDto Filtre) : IRequest<Result<List<IcraRaporSatiriDto>>>;

public class GetIcraRaporuQueryHandler : IRequestHandler<GetIcraRaporuQuery, Result<List<IcraRaporSatiriDto>>>
{
    private const int MaxSatir = 5000;
    private readonly SharedTenantDbContext _db;
    public GetIcraRaporuQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<IcraRaporSatiriDto>>> Handle(GetIcraRaporuQuery request, CancellationToken cancellationToken)
    {
        var f = request.Filtre;
        IQueryable<IcraDosyasi> query = _db.IcraDosyalari.Where(x => x.SiteId == request.SiteId);
        if (f.IlkTarih.HasValue) query = query.Where(x => x.IcraTarihi >= f.IlkTarih.Value.Date);
        if (f.SonTarih.HasValue) query = query.Where(x => x.IcraTarihi < f.SonTarih.Value.Date.AddDays(1));
        if (f.UnitId.HasValue) query = query.Where(x => x.UnitId == f.UnitId);
        else if (f.BuildingId.HasValue) query = query.Where(x => x.Unit.BuildingId == f.BuildingId);
        if (f.BorcluUserId.HasValue) query = query.Where(x => x.BorcluUserId == f.BorcluUserId);
        if (f.AvukatId.HasValue) query = query.Where(x => x.AvukatId == f.AvukatId);
        if (!string.IsNullOrWhiteSpace(f.DosyaNo))
        {
            var no = f.DosyaNo.Trim().ToLower();
            query = query.Where(x => x.DosyaNo.ToLower().Contains(no));
        }

        var rows = await query
            .OrderBy(x => x.IcraTarihi).ThenBy(x => x.DosyaNo)
            .Take(MaxSatir)
            .Select(x => new
            {
                x.Id, x.IcraTarihi, x.DosyaNo, x.BorcluAdiSnapshot,
                BlokAdi = x.Unit.Building.Name, x.Unit.DoorNumber,
                AvukatAdi = x.Avukat != null ? x.Avukat.AdSoyad : null, x.Durum, x.DosyaTutari
            })
            .ToListAsync(cancellationToken);

        var kalanlar = await IcraHesaplari.DosyaKalanlariAsync(_db, rows.Select(r => r.Id).ToList(), cancellationToken);
        var items = rows.Select(r =>
        {
            var (kalan, sayi) = kalanlar.GetValueOrDefault(r.Id);
            return new IcraRaporSatiriDto(
                r.Id, r.IcraTarihi, r.DosyaNo, r.BorcluAdiSnapshot, r.BlokAdi, r.DoorNumber, r.AvukatAdi, r.Durum,
                sayi, r.DosyaTutari, r.DosyaTutari - kalan, kalan);
        }).ToList();

        return Result<List<IcraRaporSatiriDto>>.Success(items);
    }
}
