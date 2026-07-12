using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Personeller.Commands;

public record AddPersonelTelefonCommand(Guid PersonelId, Guid SiteId, string PhoneNumber, string? Label) : IRequest<Result<Guid>>;

public class AddPersonelTelefonCommandHandler : IRequestHandler<AddPersonelTelefonCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public AddPersonelTelefonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(AddPersonelTelefonCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            return Result<Guid>.Failure("Telefon numarası zorunludur.");

        var personelVar = await _db.Personeller.AnyAsync(x => x.Id == request.PersonelId && x.SiteId == request.SiteId, cancellationToken);
        if (!personelVar) return Result<Guid>.Failure("Personel bulunamadı.");

        var entity = new PersonelTelefon
        {
            SiteId = request.SiteId,
            PersonelId = request.PersonelId,
            PhoneNumber = request.PhoneNumber.Trim(),
            Label = request.Label
        };
        _db.PersonelTelefonlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdatePersonelTelefonCommand(Guid Id, Guid SiteId, string PhoneNumber, string? Label) : IRequest<Result>;

public class UpdatePersonelTelefonCommandHandler : IRequestHandler<UpdatePersonelTelefonCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdatePersonelTelefonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdatePersonelTelefonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelTelefonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Telefon kaydı bulunamadı.");

        entity.PhoneNumber = request.PhoneNumber.Trim();
        entity.Label = request.Label;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeletePersonelTelefonCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeletePersonelTelefonCommandHandler : IRequestHandler<DeletePersonelTelefonCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeletePersonelTelefonCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePersonelTelefonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.PersonelTelefonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity is null) return Result.Failure("Telefon kaydı bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
