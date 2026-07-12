using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record AddPersonelIzinCommand(
    Guid PersonelId, Guid SiteId, DateOnly BaslangicTarihi, DateOnly BitisTarihi,
    PersonelIzinTuru IzinTuru, string? Aciklama) : IRequest<Result<Guid>>;

public class AddPersonelIzinCommandHandler : IRequestHandler<AddPersonelIzinCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public AddPersonelIzinCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(AddPersonelIzinCommand request, CancellationToken cancellationToken)
    {
        if (request.BitisTarihi < request.BaslangicTarihi)
            return Result<Guid>.Failure("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result<Guid>.Failure("Personel bulunamadı.");

        var entity = new PersonelIzin
        {
            SiteId = request.SiteId,
            PersonelId = request.PersonelId,
            BaslangicTarihi = request.BaslangicTarihi,
            BitisTarihi = request.BitisTarihi,
            IzinTuru = request.IzinTuru,
            Aciklama = request.Aciklama
        };
        _db.PersonelIzinleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdatePersonelIzinCommand(
    Guid Id, Guid SiteId, DateOnly BaslangicTarihi, DateOnly BitisTarihi,
    PersonelIzinTuru IzinTuru, string? Aciklama) : IRequest<Result>;

public class UpdatePersonelIzinCommandHandler : IRequestHandler<UpdatePersonelIzinCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelIzinCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelIzinCommand request, CancellationToken cancellationToken)
    {
        if (request.BitisTarihi < request.BaslangicTarihi)
            return Result.Failure("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var entity = await _db.PersonelIzinleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("İzin kaydı bulunamadı.");

        entity.BaslangicTarihi = request.BaslangicTarihi;
        entity.BitisTarihi = request.BitisTarihi;
        entity.IzinTuru = request.IzinTuru;
        entity.Aciklama = request.Aciklama;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeletePersonelIzinCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelIzinCommandHandler : IRequestHandler<DeletePersonelIzinCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelIzinCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelIzinCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelIzinleri.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("İzin kaydı bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
