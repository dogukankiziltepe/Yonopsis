using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record AddPersonelAcilDurumKisiCommand(Guid PersonelId, Guid SiteId, string AdSoyad, string? Yakinlik, string? Telefon) : IRequest<Result<Guid>>;

public class AddPersonelAcilDurumKisiCommandHandler : IRequestHandler<AddPersonelAcilDurumKisiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public AddPersonelAcilDurumKisiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(AddPersonelAcilDurumKisiCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AdSoyad))
            return Result<Guid>.Failure("Ad Soyad zorunludur.");

        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result<Guid>.Failure("Personel bulunamadı.");

        var entity = new PersonelAcilDurumKisi
        {
            SiteId = request.SiteId,
            PersonelId = request.PersonelId,
            AdSoyad = request.AdSoyad.Trim(),
            Yakinlik = request.Yakinlik,
            Telefon = request.Telefon
        };
        _db.PersonelAcilDurumKisileri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdatePersonelAcilDurumKisiCommand(Guid Id, Guid SiteId, string AdSoyad, string? Yakinlik, string? Telefon) : IRequest<Result>;

public class UpdatePersonelAcilDurumKisiCommandHandler : IRequestHandler<UpdatePersonelAcilDurumKisiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelAcilDurumKisiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelAcilDurumKisiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelAcilDurumKisileri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Acil durum kişisi bulunamadı.");

        entity.AdSoyad = request.AdSoyad.Trim();
        entity.Yakinlik = request.Yakinlik;
        entity.Telefon = request.Telefon;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeletePersonelAcilDurumKisiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelAcilDurumKisiCommandHandler : IRequestHandler<DeletePersonelAcilDurumKisiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelAcilDurumKisiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelAcilDurumKisiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelAcilDurumKisileri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Acil durum kişisi bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
