using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.IcraTakibi.DTOs;
using SiteYonetimi.SiteManagement.IcraTakibi.Services;

namespace SiteYonetimi.SiteManagement.IcraTakibi.Queries;

// ── Takibe Gönder: aday borçlular ───────────────────────────────────────────
public record GetTakipAdaylariQuery(Guid SiteId, TakipAdayFiltreDto Filtre) : IRequest<Result<List<TakipAdayiDto>>>;

public class GetTakipAdaylariQueryHandler : IRequestHandler<GetTakipAdaylariQuery, Result<List<TakipAdayiDto>>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetTakipAdaylariQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<List<TakipAdayiDto>>> Handle(GetTakipAdaylariQuery request, CancellationToken cancellationToken)
    {
        var gruplar = await IcraHesaplari.AcikMakbuzlar(_db, request.SiteId, request.Filtre)
            .GroupBy(b => new { b.BorcluUserId, b.UnitId })
            .Select(g => new
            {
                g.Key.BorcluUserId,
                g.Key.UnitId,
                Sayi = g.Count(),
                Borc = g.Sum(b => b.Tutar),
                Tazminat = g.Sum(b => b.GecikmeTutari),
                Odenen = g.Sum(b => b.OdenenTutar),
                Snapshot = g.Max(b => b.BorcluAdiSnapshot)
            })
            .ToListAsync(cancellationToken);

        var satirlar = gruplar.Where(g => g.Borc + g.Tazminat - g.Odenen >= request.Filtre.EnAzBakiye).ToList();

        var unitIds = satirlar.Select(g => g.UnitId!.Value).Distinct().ToList();
        var units = await _db.Units.Where(u => unitIds.Contains(u.Id))
            .Select(u => new { u.Id, u.DoorNumber, BlokAdi = u.Building.Name })
            .ToDictionaryAsync(u => u.Id, cancellationToken);
        var kisiler = await IcraHesaplari.IletisimAsync(_masterDb, request.SiteId, satirlar.Select(g => g.BorcluUserId!.Value), cancellationToken);

        var result = satirlar.Select(g =>
        {
            var userId = g.BorcluUserId!.Value;
            var unit = units.GetValueOrDefault(g.UnitId!.Value);
            return new TakipAdayiDto(
                userId, kisiler.GetValueOrDefault(userId)?.Ad ?? g.Snapshot ?? "—",
                g.UnitId.Value, unit?.BlokAdi, unit?.DoorNumber ?? "—",
                g.Sayi, g.Borc, g.Tazminat, g.Odenen, g.Borc + g.Tazminat - g.Odenen);
        })
        .OrderByDescending(x => x.Kalan)
        .ToList();

        return Result<List<TakipAdayiDto>>.Success(result);
    }
}

// ── Takip Listesi ───────────────────────────────────────────────────────────
public record GetTakiplerQuery(Guid SiteId, int Page = 1, int PageSize = 20, string? Search = null, TakipDurumu? Durum = null)
    : IRequest<Result<PaginatedResult<TakipListItemDto>>>;

public class GetTakiplerQueryHandler : IRequestHandler<GetTakiplerQuery, Result<PaginatedResult<TakipListItemDto>>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetTakiplerQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<PaginatedResult<TakipListItemDto>>> Handle(GetTakiplerQuery request, CancellationToken cancellationToken)
    {
        var query = _db.IcraTakipleri.Where(x => x.SiteId == request.SiteId);
        if (request.Durum.HasValue) query = query.Where(x => x.Durum == request.Durum);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x => x.BorcluAdiSnapshot.ToLower().Contains(term) || x.Unit.DoorNumber.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.TakipTarihi).ThenByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.Id, x.TakipTarihi, x.BorcluUserId, x.BorcluAdiSnapshot, x.UnitId,
                BlokAdi = x.Unit.Building.Name, x.Unit.DoorNumber, x.BaslangicTutari, x.Durum
            })
            .ToListAsync(cancellationToken);

        var ids = rows.Select(r => r.Id).ToList();
        var kalanlar = await IcraHesaplari.TakipKalanlariAsync(_db, ids, cancellationToken);
        var dosyalar = await _db.IcraDosyalari.Where(d => d.TakipId != null && ids.Contains(d.TakipId.Value))
            .Select(d => new { TakipId = d.TakipId!.Value, d.Id })
            .ToListAsync(cancellationToken);
        var kisiler = await IcraHesaplari.IletisimAsync(_masterDb, request.SiteId, rows.Select(r => r.BorcluUserId), cancellationToken);

        var items = rows.Select(r =>
        {
            var k = kisiler.GetValueOrDefault(r.BorcluUserId);
            return new TakipListItemDto(
                r.Id, r.TakipTarihi, r.BorcluUserId, k?.Ad ?? r.BorcluAdiSnapshot, r.UnitId, r.BlokAdi, r.DoorNumber,
                r.BaslangicTutari, kalanlar.GetValueOrDefault(r.Id), r.Durum,
                k?.Telefon, k?.Eposta, k?.Adres, dosyalar.FirstOrDefault(d => d.TakipId == r.Id)?.Id);
        }).ToList();

        return Result<PaginatedResult<TakipListItemDto>>.Success(
            PaginatedResult<TakipListItemDto>.Create(items, total, request.Page, request.PageSize));
    }
}

// ── Takip detayı ────────────────────────────────────────────────────────────
public record GetTakipByIdQuery(Guid Id, Guid SiteId) : IRequest<Result<TakipDetayDto>>;

public class GetTakipByIdQueryHandler : IRequestHandler<GetTakipByIdQuery, Result<TakipDetayDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetTakipByIdQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    public async Task<Result<TakipDetayDto>> Handle(GetTakipByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _db.IcraTakipleri.Where(x => x.Id == request.Id && x.SiteId == request.SiteId)
            .Select(x => new
            {
                x.Id, x.TakipTarihi, x.BorcluUserId, x.BorcluAdiSnapshot, x.UnitId,
                BlokAdi = x.Unit.Building.Name, x.Unit.DoorNumber, x.BaslangicTutari, x.Durum, x.Aciklama
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (t is null) return Result<TakipDetayDto>.Failure("Takip kaydı bulunamadı.");

        var evraklar = await IcraHesaplari.EvraklarAsync(_db,
            _db.IcraTakipEvraklari.Where(e => e.TakipId == t.Id).Select(e => e.BorcMakbuzuId), cancellationToken);
        var dosya = await _db.IcraDosyalari.Where(d => d.TakipId == t.Id)
            .Select(d => new { d.Id, d.DosyaNo }).FirstOrDefaultAsync(cancellationToken);
        var kisi = (await IcraHesaplari.IletisimAsync(_masterDb, request.SiteId, new[] { t.BorcluUserId }, cancellationToken))
            .GetValueOrDefault(t.BorcluUserId);

        return Result<TakipDetayDto>.Success(new TakipDetayDto(
            t.Id, t.TakipTarihi, t.BorcluUserId, kisi?.Ad ?? t.BorcluAdiSnapshot, t.UnitId, t.BlokAdi, t.DoorNumber,
            t.BaslangicTutari, evraklar.Sum(e => e.Kalan), t.Durum, t.Aciklama,
            dosya?.Id, dosya?.DosyaNo, evraklar));
    }
}
