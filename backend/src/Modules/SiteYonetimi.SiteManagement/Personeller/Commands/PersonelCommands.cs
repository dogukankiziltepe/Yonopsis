using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

// ── Create ──────────────────────────────────────────────────────────────────
public record CreatePersonelCommand(Guid SiteId, CreatePersonelDto Dto) : IRequest<Result<Guid>>;

public class CreatePersonelCommandHandler : IRequestHandler<CreatePersonelCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreatePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreatePersonelCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.Name))
            return Result<Guid>.Failure("Ad Soyad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(request.Dto.PersonelKodu))
            return Result<Guid>.Failure("Personel Kodu alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(request.Dto.Title))
            return Result<Guid>.Failure("Görevi alanı zorunludur.");

        var koduExists = await _db.Personeller.AnyAsync(
            x => x.SiteId == request.SiteId && x.PersonelKodu == request.Dto.PersonelKodu.Trim(), cancellationToken);
        if (koduExists)
            return Result<Guid>.Failure("Bu personel kodu zaten kullanılıyor.");

        var entity = new Personel
        {
            SiteId       = request.SiteId,
            PersonelKodu = request.Dto.PersonelKodu.Trim(),
            Name         = request.Dto.Name.Trim(),
            Firma        = request.Dto.Firma,
            Title        = request.Dto.Title.Trim(),
            Email        = request.Dto.Email,
            StartDate    = request.Dto.StartDate,
        };

        _db.Personeller.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

// ── Update ──────────────────────────────────────────────────────────────────
public record UpdatePersonelCommand(Guid Id, Guid SiteId, UpdatePersonelDto Dto) : IRequest<Result>;

public class UpdatePersonelCommandHandler : IRequestHandler<UpdatePersonelCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Personeller
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Personel bulunamadı.");

        if (string.IsNullOrWhiteSpace(request.Dto.Name))
            return Result.Failure("Ad Soyad alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(request.Dto.PersonelKodu))
            return Result.Failure("Personel Kodu alanı zorunludur.");
        if (string.IsNullOrWhiteSpace(request.Dto.Title))
            return Result.Failure("Görevi alanı zorunludur.");

        var koduCakisiyor = await _db.Personeller.AnyAsync(
            x => x.SiteId == request.SiteId && x.Id != request.Id && x.PersonelKodu == request.Dto.PersonelKodu.Trim(),
            cancellationToken);
        if (koduCakisiyor)
            return Result.Failure("Bu personel kodu zaten kullanılıyor.");

        entity.PersonelKodu = request.Dto.PersonelKodu.Trim();
        entity.Name = request.Dto.Name.Trim();
        entity.Firma = request.Dto.Firma;
        entity.Title = request.Dto.Title.Trim();
        entity.Cinsiyet = request.Dto.Cinsiyet;
        entity.YemekKarti = request.Dto.YemekKarti;
        entity.Aciklama = request.Dto.Aciklama;
        entity.Email = request.Dto.Email;
        entity.KanGrubu = request.Dto.KanGrubu;
        entity.OgrenimDurumu = request.Dto.OgrenimDurumu;
        entity.OkulKurum = request.Dto.OkulKurum;
        entity.Adres = request.Dto.Adres;
        entity.StartDate = request.Dto.StartDate;
        entity.CikisTarihi = request.Dto.CikisTarihi;
        entity.KidemTazminatiBaslamaTarihi = request.Dto.KidemTazminatiBaslamaTarihi;
        entity.IsActive = request.Dto.IsActive;
        entity.MuhasebeHesapKoduId = request.Dto.MuhasebeHesapKoduId;
        entity.BankaSubesiId = request.Dto.BankaSubesiId;
        entity.BankaHesapNo = request.Dto.BankaHesapNo;
        entity.BankaIBAN = request.Dto.BankaIBAN;
        entity.YillikIzinHakkiGun = request.Dto.YillikIzinHakkiGun;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// ── Delete ──────────────────────────────────────────────────────────────────
public record DeletePersonelCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelCommandHandler : IRequestHandler<DeletePersonelCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Personeller
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Personel bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
