using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record UpdatePersonelKimlikCommand(Guid PersonelId, Guid SiteId, UpdatePersonelKimlikDto Dto) : IRequest<Result>;

public class UpdatePersonelKimlikCommandHandler : IRequestHandler<UpdatePersonelKimlikCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelKimlikCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelKimlikCommand request, CancellationToken cancellationToken)
    {
        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result.Failure("Personel bulunamadı.");

        var entity = await _db.PersonelKimlikBilgileri.FirstOrDefaultAsync(x => x.PersonelId == request.PersonelId, cancellationToken);
        if (entity is null)
        {
            entity = new PersonelKimlikBilgisi { SiteId = request.SiteId, PersonelId = request.PersonelId };
            _db.PersonelKimlikBilgileri.Add(entity);
        }

        var dto = request.Dto;
        entity.TcKimlikNo = dto.TcKimlikNo;
        entity.Seri = dto.Seri;
        entity.Sira = dto.Sira;
        entity.BabaAdi = dto.BabaAdi;
        entity.AnaAdi = dto.AnaAdi;
        entity.OncekiSoyad = dto.OncekiSoyad;
        entity.DogumYeri = dto.DogumYeri;
        entity.DogumTarihi = dto.DogumTarihi;
        entity.MedeniHali = dto.MedeniHali;
        entity.Il = dto.Il;
        entity.Ilce = dto.Ilce;
        entity.MahalleKoy = dto.MahalleKoy;
        entity.CiltNo = dto.CiltNo;
        entity.AileSiraNo = dto.AileSiraNo;
        entity.SiraNo = dto.SiraNo;
        entity.VerildigiYer = dto.VerildigiYer;
        entity.VerilisNedeni = dto.VerilisNedeni;
        entity.KayitNo = dto.KayitNo;
        entity.VerilisTarihi = dto.VerilisTarihi;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
