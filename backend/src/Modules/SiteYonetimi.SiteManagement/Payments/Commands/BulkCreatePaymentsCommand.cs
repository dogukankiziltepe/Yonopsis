using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record BulkCreatePaymentsCommand(Guid SiteId, BulkCreatePaymentsDto Dto) : IRequest<Result<int>>;

public class BulkCreatePaymentsCommandHandler : IRequestHandler<BulkCreatePaymentsCommand, Result<int>>
{
    private readonly SharedTenantDbContext _db;
    public BulkCreatePaymentsCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<int>> Handle(BulkCreatePaymentsCommand request, CancellationToken cancellationToken)
    {
        var unitsQuery = _db.Units.Where(u => u.SiteId == request.SiteId);

        if (request.Dto.BuildingId.HasValue)
            unitsQuery = unitsQuery.Where(u => u.BuildingId == request.Dto.BuildingId);

        var unitIds = await unitsQuery.Select(u => u.Id).ToListAsync(cancellationToken);

        if (unitIds.Count == 0)
            return Result<int>.Failure("Aktif daire bulunamadı.");

        var monthStart = new DateTime(request.Dto.DueDate.Year, request.Dto.DueDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var existingUnitIds = await _db.Payments
            .Where(p => p.SiteId == request.SiteId
                && p.DueDate >= monthStart
                && p.DueDate < monthEnd
                && unitIds.Contains(p.UnitId))
            .Select(p => p.UnitId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var newUnitIds = unitIds.Except(existingUnitIds).ToList();

        if (newUnitIds.Count == 0)
            return Result<int>.Failure("Seçilen ay için tüm dairelerde zaten aidat kaydı mevcut.");

        var payments = newUnitIds.Select(unitId => new Payment
        {
            SiteId = request.SiteId,
            UnitId = unitId,
            Amount = request.Dto.Amount,
            DueDate = request.Dto.DueDate,
            Description = request.Dto.Description
        }).ToList();

        _db.Payments.AddRange(payments);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(payments.Count);
    }
}
