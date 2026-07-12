using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record AddPersonelEgitimCommand(
    Guid PersonelId, Guid SiteId, string EgitiminKonusu, string? Egitmen, string? EgitimYeri,
    DateOnly? BaslamaTarihi, DateOnly? BitisTarihi, decimal? ToplamSaat) : IRequest<Result<Guid>>;

public class AddPersonelEgitimCommandHandler : IRequestHandler<AddPersonelEgitimCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public AddPersonelEgitimCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(AddPersonelEgitimCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EgitiminKonusu))
            return Result<Guid>.Failure("Eğitimin konusu zorunludur.");

        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result<Guid>.Failure("Personel bulunamadı.");

        var entity = new PersonelEgitim
        {
            SiteId = request.SiteId,
            PersonelId = request.PersonelId,
            EgitiminKonusu = request.EgitiminKonusu.Trim(),
            Egitmen = request.Egitmen,
            EgitimYeri = request.EgitimYeri,
            BaslamaTarihi = request.BaslamaTarihi,
            BitisTarihi = request.BitisTarihi,
            ToplamSaat = request.ToplamSaat
        };
        _db.PersonelEgitimleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdatePersonelEgitimCommand(
    Guid Id, Guid SiteId, string EgitiminKonusu, string? Egitmen, string? EgitimYeri,
    DateOnly? BaslamaTarihi, DateOnly? BitisTarihi, decimal? ToplamSaat) : IRequest<Result>;

public class UpdatePersonelEgitimCommandHandler : IRequestHandler<UpdatePersonelEgitimCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelEgitimCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelEgitimCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelEgitimleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Eğitim kaydı bulunamadı.");

        entity.EgitiminKonusu = request.EgitiminKonusu.Trim();
        entity.Egitmen = request.Egitmen;
        entity.EgitimYeri = request.EgitimYeri;
        entity.BaslamaTarihi = request.BaslamaTarihi;
        entity.BitisTarihi = request.BitisTarihi;
        entity.ToplamSaat = request.ToplamSaat;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeletePersonelEgitimCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelEgitimCommandHandler : IRequestHandler<DeletePersonelEgitimCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelEgitimCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelEgitimCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelEgitimleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Eğitim kaydı bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
