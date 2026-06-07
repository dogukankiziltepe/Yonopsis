using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.SiteManagement.Payments.Commands;
using SiteYonetimi.Tests.Helpers;

namespace SiteYonetimi.Tests.Payments;

public class MarkOverduePaymentsHandlerTests
{
    [Fact]
    public async Task Handle_MarksPendingOverduePayments()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        db.Payments.Add(new Payment { SiteId = siteId, UnitId = unitId, Amount = 500, DueDate = yesterday, Status = PaymentStatus.Pending });
        db.Payments.Add(new Payment { SiteId = siteId, UnitId = unitId, Amount = 500, DueDate = yesterday, Status = PaymentStatus.Paid });
        await db.SaveChangesAsync();

        var handler = new MarkOverduePaymentsCommandHandler(db);
        var result = await handler.Handle(new MarkOverduePaymentsCommand(siteId), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data);
        var pending = db.Payments.Where(p => p.Status == PaymentStatus.Pending).ToList();
        Assert.Empty(pending);
    }

    [Fact]
    public async Task Handle_DoesNotMarkFuturePayments()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        db.Payments.Add(new Payment { SiteId = siteId, UnitId = Guid.NewGuid(), Amount = 500, DueDate = tomorrow, Status = PaymentStatus.Pending });
        await db.SaveChangesAsync();

        var handler = new MarkOverduePaymentsCommandHandler(db);
        var result = await handler.Handle(new MarkOverduePaymentsCommand(siteId), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data);
    }

    [Fact]
    public async Task Handle_DoesNotMarkOtherSites()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteA = Guid.NewGuid();
        var siteB = Guid.NewGuid();
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        db.Payments.Add(new Payment { SiteId = siteB, UnitId = Guid.NewGuid(), Amount = 100, DueDate = yesterday, Status = PaymentStatus.Pending });
        await db.SaveChangesAsync();

        var handler = new MarkOverduePaymentsCommandHandler(db);
        var result = await handler.Handle(new MarkOverduePaymentsCommand(siteA), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data);
        Assert.Equal(PaymentStatus.Pending, db.Payments.First().Status);
    }
}
