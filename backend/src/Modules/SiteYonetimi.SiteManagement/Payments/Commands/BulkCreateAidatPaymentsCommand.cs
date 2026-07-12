using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record AidatPaymentItem(Guid UnitId, Guid AidatKalemId, decimal Amount);

public record BulkCreateAidatPaymentsDto(DateTime DueDate, List<AidatPaymentItem> Items);

public record BulkCreateAidatPaymentsCommand(Guid SiteId, BulkCreateAidatPaymentsDto Dto) : IRequest<Result<int>>;

public class BulkCreateAidatPaymentsCommandHandler : IRequestHandler<BulkCreateAidatPaymentsCommand, Result<int>>
{
    private readonly SharedTenantDbContext _db;
    public BulkCreateAidatPaymentsCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<int>> Handle(BulkCreateAidatPaymentsCommand request, CancellationToken cancellationToken)
    {
        var items = request.Dto.Items.Where(x => x.Amount > 0).ToList();
        if (items.Count == 0)
            return Result<int>.Failure("Oluşturulacak aidat kalemi bulunamadı.");

        var unitIds = items.Select(x => x.UnitId).Distinct().ToList();
        var validUnitIds = (await _db.Units
            .Where(u => u.SiteId == request.SiteId && unitIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken)).ToHashSet();

        var kalemIds = items.Select(x => x.AidatKalemId).Distinct().ToList();
        var validKalemIds = (await _db.AidatKalemleri
            .Where(k => k.SiteId == request.SiteId && kalemIds.Contains(k.Id))
            .Select(k => k.Id)
            .ToListAsync(cancellationToken)).ToHashSet();

        var monthStart = new DateTime(request.Dto.DueDate.Year, request.Dto.DueDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var existing = await _db.Payments
            .Where(p => p.SiteId == request.SiteId
                && p.DueDate >= monthStart
                && p.DueDate < monthEnd
                && p.AidatKalemId != null
                && unitIds.Contains(p.UnitId))
            .Select(p => new { p.UnitId, p.AidatKalemId })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(x => (x.UnitId, x.AidatKalemId!.Value))
            .ToHashSet();

        var payments = new List<Payment>();
        foreach (var item in items)
        {
            if (!validUnitIds.Contains(item.UnitId)) continue;
            if (!validKalemIds.Contains(item.AidatKalemId)) continue;
            if (existingSet.Contains((item.UnitId, item.AidatKalemId))) continue;

            payments.Add(new Payment
            {
                SiteId = request.SiteId,
                UnitId = item.UnitId,
                AidatKalemId = item.AidatKalemId,
                Amount = item.Amount,
                DueDate = request.Dto.DueDate
            });
        }

        if (payments.Count == 0)
            return Result<int>.Failure("Seçilen ay için tüm dairelerde bu kalemler zaten mevcut.");

        _db.Payments.AddRange(payments);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(payments.Count);
    }
}

public class BulkCreateAidatPaymentsDtoValidator : AbstractValidator<BulkCreateAidatPaymentsDto>
{
    public BulkCreateAidatPaymentsDtoValidator()
    {
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("En az bir aidat kalemi gereklidir.");
    }
}
