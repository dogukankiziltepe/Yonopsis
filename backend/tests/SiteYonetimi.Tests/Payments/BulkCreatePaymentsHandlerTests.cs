using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.SiteManagement.Payments.Commands;
using SiteYonetimi.SiteManagement.Payments.DTOs;
using SiteYonetimi.Tests.Helpers;
using Xunit;

namespace SiteYonetimi.Tests.Payments;

public class BulkCreatePaymentsHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPaymentsForAllUnits()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var dueDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        db.Units.Add(new Unit { SiteId = siteId, BuildingId = buildingId, DoorNumber = "1" });
        db.Units.Add(new Unit { SiteId = siteId, BuildingId = buildingId, DoorNumber = "2" });
        await db.SaveChangesAsync();

        var dto = new BulkCreatePaymentsDto(null, 500m, dueDate, null);
        var handler = new BulkCreatePaymentsCommandHandler(db);
        var result = await handler.Handle(new BulkCreatePaymentsCommand(siteId, dto), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data);
        Assert.Equal(2, db.Payments.Count());
    }

    [Fact]
    public async Task Handle_SkipsDuplicatesForSameMonth()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var buildingId = Guid.NewGuid();
        var unit = new Unit { SiteId = siteId, BuildingId = buildingId, DoorNumber = "1" };
        db.Units.Add(unit);
        var dueDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc);
        db.Payments.Add(new Payment { SiteId = siteId, UnitId = unit.Id, Amount = 500, DueDate = dueDate });
        await db.SaveChangesAsync();

        var dto = new BulkCreatePaymentsDto(null, 500m, new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), null);
        var handler = new BulkCreatePaymentsCommandHandler(db);
        var result = await handler.Handle(new BulkCreatePaymentsCommand(siteId, dto), default);

        Assert.False(result.IsSuccess);
        Assert.Contains("mevcut", result.Error);
    }

    [Fact]
    public async Task Handle_FiltersByBuildingId()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var building1 = Guid.NewGuid();
        var building2 = Guid.NewGuid();
        var dueDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);

        db.Units.Add(new Unit { SiteId = siteId, BuildingId = building1, DoorNumber = "A1" });
        db.Units.Add(new Unit { SiteId = siteId, BuildingId = building2, DoorNumber = "B1" });
        await db.SaveChangesAsync();

        var dto = new BulkCreatePaymentsDto(building1, 400m, dueDate, null);
        var handler = new BulkCreatePaymentsCommandHandler(db);
        var result = await handler.Handle(new BulkCreatePaymentsCommand(siteId, dto), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Data);
    }

    [Fact]
    public async Task Handle_NoUnits_ReturnsFailure()
    {
        var db = InMemoryDbHelper.CreateSharedDb();
        var siteId = Guid.NewGuid();
        var dto = new BulkCreatePaymentsDto(null, 300m, new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc), null);

        var handler = new BulkCreatePaymentsCommandHandler(db);
        var result = await handler.Handle(new BulkCreatePaymentsCommand(siteId, dto), default);

        Assert.False(result.IsSuccess);
        Assert.Contains("daire", result.Error);
    }
}
