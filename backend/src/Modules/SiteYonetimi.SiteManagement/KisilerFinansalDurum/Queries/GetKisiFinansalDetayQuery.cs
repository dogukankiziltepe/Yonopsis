using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.KisilerFinansalDurum.DTOs;

namespace SiteYonetimi.SiteManagement.KisilerFinansalDurum.Queries;

public record GetKisiFinansalDetayQuery(Guid SiteId, Guid PersonUserId) : IRequest<Result<KisiFinansalDetayDto>>;

/// <summary>
/// Kişilere Göre Finansal Durum ekranındaki detay modalı için: kişi kartı +
/// Daireler → GelirGrubu kategorisi → evrak bazlı hareket hiyerarşisi.
/// </summary>
public class GetKisiFinansalDetayQueryHandler : IRequestHandler<GetKisiFinansalDetayQuery, Result<KisiFinansalDetayDto>>
{
    private readonly SharedTenantDbContext _db;
    private readonly MasterDbContext _masterDb;

    public GetKisiFinansalDetayQueryHandler(SharedTenantDbContext db, MasterDbContext masterDb)
    {
        _db = db;
        _masterDb = masterDb;
    }

    private record Hareket(Guid Id, Guid? UnitId, string GrupAdi, string Kaynak, DateTime Tarih, DateTime? SonOdeme, string? Aciklama, decimal Borc, decimal Tazminat, decimal Alacak);

    public async Task<Result<KisiFinansalDetayDto>> Handle(GetKisiFinansalDetayQuery request, CancellationToken cancellationToken)
    {
        var siteId = request.SiteId;
        var personId = request.PersonUserId;

        var user = await _masterDb.Users.Where(u => u.Id == personId)
            .Select(u => new { u.FirstName, u.LastName, u.Email, u.PhoneNumber })
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) return Result<KisiFinansalDetayDto>.Failure("Kişi bulunamadı.");

        var kisi = new KisiOzetDto(personId, $"{user.FirstName} {user.LastName}", user.Email, user.PhoneNumber);

        // Kişinin daireleri: PersonUnitHistory + Unit.OwnerUserId/TenantUserId fallback
        var historyUnitIds = await _db.PersonUnitHistories
            .Where(x => x.SiteId == siteId && x.PersonUserId == personId)
            .Select(x => x.UnitId).Distinct().ToListAsync(cancellationToken);
        var directUnitIds = await _db.Units
            .Where(u => u.SiteId == siteId && (u.OwnerUserId == personId || u.TenantUserId == personId))
            .Select(u => u.Id).ToListAsync(cancellationToken);
        var unitIds = historyUnitIds.Union(directUnitIds).Distinct().ToList();

        var units = await _db.Units
            .Where(u => u.SiteId == siteId && unitIds.Contains(u.Id))
            .Select(u => new { u.Id, u.DoorNumber })
            .ToListAsync(cancellationToken);

        var borclar = await _db.BorcMakbuzlari
            .Include(b => b.GelirTanimi!).ThenInclude(g => g.GelirGrubu)
            .Where(b => b.SiteId == siteId && b.BorcluUserId == personId)
            .ToListAsync(cancellationToken);

        var tahsilatlar = await _db.TahsilatMakbuzlari
            .Include(t => t.BorcMakbuzu!).ThenInclude(b => b.GelirTanimi!).ThenInclude(g => g.GelirGrubu)
            .Where(t => t.SiteId == siteId && t.BorcluUserId == personId)
            .ToListAsync(cancellationToken);

        var devirler = await _db.DevirBakiyeleri
            .Where(d => d.SiteId == siteId && d.BorcluUserId == personId)
            .ToListAsync(cancellationToken);

        var hareketler = new List<Hareket>();
        foreach (var b in borclar)
        {
            var grupAdi = b.GelirTanimi?.GelirGrubu?.Name ?? "Diğer Gelirler";
            hareketler.Add(new Hareket(b.Id, b.UnitId, grupAdi, "Borc", b.IslemTarihi, b.SonOdemeTarihi, b.Aciklama, b.Tutar, b.GecikmeTutari, 0m));
        }
        foreach (var t in tahsilatlar)
        {
            var grupAdi = t.BorcMakbuzu?.GelirTanimi?.GelirGrubu?.Name ?? "Diğer Gelirler";
            hareketler.Add(new Hareket(t.Id, t.BorcMakbuzu?.UnitId, grupAdi, "Tahsilat", t.IslemTarihi, null, t.Aciklama, 0m, 0m, t.OdemeTutari));
        }
        foreach (var d in devirler)
        {
            hareketler.Add(new Hareket(d.Id, d.UnitId, "Devir", "Devir", d.Tarih, null, d.Aciklama, d.Tutar, 0m, 0m));
        }

        var daireler = new List<DaireFinansalDto>();
        foreach (var grup in hareketler.GroupBy(h => h.UnitId))
        {
            var unitInfo = units.FirstOrDefault(u => u.Id == grup.Key);
            var doorNumber = unitInfo?.DoorNumber ?? "Diğer";

            var kategoriler = new List<GelirGrubuFinansalDto>();
            foreach (var kategoriGrup in grup.GroupBy(h => h.GrupAdi))
            {
                var siraliHareketler = kategoriGrup.OrderBy(h => h.Tarih).ToList();
                var yurudakiBakiye = 0m;
                var hareketDtolar = new List<FinansalHareketDto>();
                foreach (var h in siraliHareketler)
                {
                    yurudakiBakiye += h.Borc + h.Tazminat - h.Alacak;
                    hareketDtolar.Add(new FinansalHareketDto(h.Id, h.Kaynak, h.Tarih, h.SonOdeme, h.Aciklama, h.Borc, h.Tazminat, h.Alacak, yurudakiBakiye));
                }
                // En yeni evrak en üstte gösterilecek şekilde ters çevir (yürüyen bakiye hesaplaması tarihe göre yapıldıktan sonra)
                hareketDtolar.Reverse();

                var kBorc = kategoriGrup.Sum(h => h.Borc);
                var kTazminat = kategoriGrup.Sum(h => h.Tazminat);
                var kAlacakToplam = kategoriGrup.Sum(h => h.Alacak);
                kategoriler.Add(new GelirGrubuFinansalDto(null, kategoriGrup.Key, kBorc, kTazminat, kAlacakToplam, kBorc + kTazminat - kAlacakToplam, hareketDtolar));
            }

            var dBorc = kategoriler.Sum(k => k.Borc);
            var dTazminat = kategoriler.Sum(k => k.Tazminat);
            var dAlacak = kategoriler.Sum(k => k.Alacak);
            daireler.Add(new DaireFinansalDto(grup.Key, doorNumber, dBorc, dTazminat, dAlacak, dBorc + dTazminat - dAlacak, kategoriler));
        }

        var toplamBorcHam = daireler.Sum(d => d.Borc + d.Tazminat - d.Alacak);
        var toplamBorc = Math.Max(0, toplamBorcHam);
        var toplamAlacak = Math.Max(0, -toplamBorcHam);

        var dto = new KisiFinansalDetayDto(kisi, toplamBorc, toplamAlacak, toplamBorc - toplamAlacak, daireler);
        return Result<KisiFinansalDetayDto>.Success(dto);
    }
}
