using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Hesaplar.Queries;

public record GetHesapByIdQuery(Guid SiteId, Guid Id) : IRequest<Result<HesapDetailDto>>;

public class GetHesapByIdQueryHandler : IRequestHandler<GetHesapByIdQuery, Result<HesapDetailDto>>
{
    private readonly SharedTenantDbContext _db;

    public GetHesapByIdQueryHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<HesapDetailDto>> Handle(GetHesapByIdQuery request, CancellationToken cancellationToken)
    {
        var hesap = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == request.SiteId && h.Id == request.Id, cancellationToken);
        if (hesap is null)
            return Result<HesapDetailDto>.Failure("Hesap bulunamadı.");

        string? parentKodu = null;
        if (hesap.ParentId is not null)
        {
            parentKodu = await _db.HesapPlani
                .Where(h => h.Id == hesap.ParentId)
                .Select(h => h.HesapKodu)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var dto = new HesapDetailDto(
            hesap.Id, hesap.HesapKodu, hesap.HesapAdi, hesap.HesapTipi, hesap.HesapKategorisi,
            hesap.NormalBakiye, hesap.Seviye, hesap.ParentId, parentKodu, hesap.FisKesilebilirMi,
            hesap.AktifMi, hesap.CariTuru, hesap.PersonId, hesap.GiderTuruId, hesap.Aciklama);

        return Result<HesapDetailDto>.Success(dto);
    }
}
