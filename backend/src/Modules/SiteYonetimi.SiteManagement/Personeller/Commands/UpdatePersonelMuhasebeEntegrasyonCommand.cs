using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record UpdatePersonelMuhasebeEntegrasyonCommand(Guid PersonelId, Guid SiteId, UpdatePersonelMuhasebeEntegrasyonDto Dto) : IRequest<Result>;

public class UpdatePersonelMuhasebeEntegrasyonCommandHandler : IRequestHandler<UpdatePersonelMuhasebeEntegrasyonCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelMuhasebeEntegrasyonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelMuhasebeEntegrasyonCommand request, CancellationToken cancellationToken)
    {
        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result.Failure("Personel bulunamadı.");

        var entity = await _db.PersonelMuhasebeEntegrasyonlari.FirstOrDefaultAsync(x => x.PersonelId == request.PersonelId, cancellationToken);
        if (entity is null)
        {
            entity = new PersonelMuhasebeEntegrasyon { SiteId = request.SiteId, PersonelId = request.PersonelId };
            _db.PersonelMuhasebeEntegrasyonlari.Add(entity);
        }

        var dto = request.Dto;
        entity.BrutUcretlerGiderTanimiId = dto.BrutUcretlerGiderTanimiId;
        entity.HuzurHakkiBrutUcretlerGiderTanimiId = dto.HuzurHakkiBrutUcretlerGiderTanimiId;
        entity.SgkIsverenPayiGiderTanimiId = dto.SgkIsverenPayiGiderTanimiId;
        entity.IssizlikSigortasiIsverenPayiGiderTanimiId = dto.IssizlikSigortasiIsverenPayiGiderTanimiId;
        entity.PrimVeIkramiyelerGiderTanimiId = dto.PrimVeIkramiyelerGiderTanimiId;
        entity.FazlaMesaiGiderTanimiId = dto.FazlaMesaiGiderTanimiId;
        entity.KidemTazminatlariGiderTanimiId = dto.KidemTazminatlariGiderTanimiId;
        entity.IhbarTazminatlariGiderTanimiId = dto.IhbarTazminatlariGiderTanimiId;
        entity.YolYardimiGiderTanimiId = dto.YolYardimiGiderTanimiId;
        entity.YemekYardimiGiderTanimiId = dto.YemekYardimiGiderTanimiId;
        entity.PersonelGelirVergisiHesapId = dto.PersonelGelirVergisiHesapId;
        entity.PersonelDamgaVergisiHesapId = dto.PersonelDamgaVergisiHesapId;
        entity.OdenecekSgkHesapId = dto.OdenecekSgkHesapId;
        entity.AsgariGecimIndirimiHesapId = dto.AsgariGecimIndirimiHesapId;
        entity.IcraKesintisiHesapId = dto.IcraKesintisiHesapId;
        entity.DigerKesintilerHesapId = dto.DigerKesintilerHesapId;
        entity.BesHesapId = dto.BesHesapId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
