using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Fisler.Commands;

/// <summary>Taslak fişi siler (soft delete). Onaylı fiş silinemez (storno gerekir).</summary>
public record DeleteFisCommand(Guid SiteId, Guid Id) : IRequest<Result>;

public class DeleteFisCommandHandler : IRequestHandler<DeleteFisCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public DeleteFisCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteFisCommand request, CancellationToken cancellationToken)
    {
        var fis = await _db.MuhasebeFisleri
            .FirstOrDefaultAsync(f => f.SiteId == request.SiteId && f.Id == request.Id, cancellationToken);
        if (fis is null)
            return Result.Failure("Fiş bulunamadı.");
        if (fis.Durum != FisDurumu.Taslak)
            return Result.Failure("Yalnızca taslak fişler silinebilir. Onaylı fiş için ters kayıt (iptal) kullanın.");

        fis.IsDeleted = true;
        fis.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
