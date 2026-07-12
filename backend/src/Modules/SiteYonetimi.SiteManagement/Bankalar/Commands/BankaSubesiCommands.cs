using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Bankalar.DTOs;

namespace SiteYonetimi.SiteManagement.Bankalar.Commands;

public record CreateBankaSubesiCommand(Guid BankaId, CreateBankaSubesiDto Dto) : IRequest<Result<Guid>>;

public class CreateBankaSubesiCommandHandler : IRequestHandler<CreateBankaSubesiCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;
    public CreateBankaSubesiCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateBankaSubesiCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.SubeAdi))
            return Result<Guid>.Failure("Şube adı zorunludur.");

        var bankaVar = await _db.Bankalar.AnyAsync(x => x.Id == request.BankaId, cancellationToken);
        if (!bankaVar) return Result<Guid>.Failure("Banka bulunamadı.");

        var entity = new BankaSubesi
        {
            BankaId = request.BankaId,
            SubeAdi = request.Dto.SubeAdi.Trim(),
            SubeKodu = request.Dto.SubeKodu
        };
        _db.BankaSubeleri.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateBankaSubesiCommand(Guid Id, UpdateBankaSubesiDto Dto) : IRequest<Result>;

public class UpdateBankaSubesiCommandHandler : IRequestHandler<UpdateBankaSubesiCommand, Result>
{
    private readonly MasterDbContext _db;
    public UpdateBankaSubesiCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateBankaSubesiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BankaSubeleri.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return Result.Failure("Şube bulunamadı.");

        entity.SubeAdi = request.Dto.SubeAdi.Trim();
        entity.SubeKodu = request.Dto.SubeKodu;
        entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteBankaSubesiCommand(Guid Id) : IRequest<Result>;

public class DeleteBankaSubesiCommandHandler : IRequestHandler<DeleteBankaSubesiCommand, Result>
{
    private readonly MasterDbContext _db;
    public DeleteBankaSubesiCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBankaSubesiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BankaSubeleri.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return Result.Failure("Şube bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
