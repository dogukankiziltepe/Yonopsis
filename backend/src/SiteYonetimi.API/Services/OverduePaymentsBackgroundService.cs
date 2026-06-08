using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.API.Services;

public class OverduePaymentsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverduePaymentsBackgroundService> _logger;
    private readonly string _sharedConnStr;

    public OverduePaymentsBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OverduePaymentsBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _sharedConnStr = configuration.GetConnectionString("SharedTenantDb")!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunAsync(stoppingToken);

            var now = DateTime.UtcNow;
            var delay = now.Date.AddDays(1) - now;
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        try
        {
            await MarkOverdueAsync(_sharedConnStr, ct);

            using var scope = _scopeFactory.CreateScope();
            var masterDb = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            var dedicatedConnStrings = await masterDb.Sites
                .Where(s => !s.IsDeleted && s.IsActive && s.DbMode == DbMode.Dedicated && s.ConnectionString != null)
                .Select(s => s.ConnectionString!)
                .ToListAsync(ct);

            foreach (var connStr in dedicatedConnStrings)
                await MarkOverdueAsync(connStr, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gecikmiş aidat job hatası.");
        }
    }

    private async Task MarkOverdueAsync(string connStr, CancellationToken ct)
    {
        var options = new DbContextOptionsBuilder<SharedTenantDbContext>()
            .UseSqlServer(connStr)
            .Options;
        await using var db = new SharedTenantDbContext(options);

        var today = DateTime.UtcNow.Date;
        var overdueList = await db.Payments
            .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Pending && p.DueDate < today)
            .ToListAsync(ct);

        foreach (var p in overdueList)
        {
            p.Status = PaymentStatus.Overdue;
            p.UpdatedAt = DateTime.UtcNow;
        }

        if (overdueList.Count > 0)
        {
            await db.SaveChangesAsync(ct);
            _logger.LogInformation("{Count} aidat gecikmiş olarak işaretlendi.", overdueList.Count);
        }
    }
}
