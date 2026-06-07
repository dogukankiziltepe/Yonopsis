using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record HandlePaymentWebhookCommand(string Payload, string Signature) : IRequest<Result>;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    private readonly IPaymentGatewayService _gateway;

    public HandlePaymentWebhookCommandHandler(SharedTenantDbContext db, IPaymentGatewayService gateway)
    {
        _db = db;
        _gateway = gateway;
    }

    public async Task<Result> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        if (!_gateway.VerifyWebhookSignature(request.Payload, request.Signature))
            return Result.Failure("Geçersiz webhook imzası.");

        if (!_gateway.IsPaymentSucceeded(request.Payload))
            return Result.Success();

        var paymentIdStr = _gateway.ExtractPaymentIdFromEvent(request.Payload);
        if (!Guid.TryParse(paymentIdStr, out var paymentId))
            return Result.Failure("Ödeme ID alınamadı.");

        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        if (payment is null) return Result.Failure("Ödeme bulunamadı.");

        payment.Status = PaymentStatus.Paid;
        payment.PaidDate = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
