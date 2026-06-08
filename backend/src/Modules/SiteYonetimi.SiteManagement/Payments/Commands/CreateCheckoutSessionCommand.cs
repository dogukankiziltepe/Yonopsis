using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record CreateCheckoutSessionCommand(Guid PaymentId, Guid SiteId, string SuccessUrl, string CancelUrl) : IRequest<Result<string>>;

public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, Result<string>>
{
    private readonly SharedTenantDbContext _db;
    private readonly IPaymentGatewayService _gateway;

    public CreateCheckoutSessionCommandHandler(SharedTenantDbContext db, IPaymentGatewayService gateway)
    {
        _db = db;
        _gateway = gateway;
    }

    public async Task<Result<string>> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId && p.SiteId == request.SiteId, cancellationToken);

        if (payment is null)
            return Result<string>.Failure("Ödeme kaydı bulunamadı.");

        if (payment.Status == Shared.Enums.PaymentStatus.Paid)
            return Result<string>.Failure("Bu ödeme zaten tamamlanmış.");

        var description = payment.Description ?? $"Aidat Ödemesi — {payment.DueDate:MM/yyyy}";
        var session = await _gateway.CreateCheckoutSessionAsync(
            payment.Id, payment.Amount, description,
            request.SuccessUrl, request.CancelUrl, cancellationToken);

        return Result<string>.Success(session.CheckoutUrl);
    }
}
