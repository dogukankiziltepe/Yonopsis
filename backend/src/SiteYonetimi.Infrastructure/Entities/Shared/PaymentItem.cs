namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class PaymentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public Payment Payment { get; set; } = null!;
}
