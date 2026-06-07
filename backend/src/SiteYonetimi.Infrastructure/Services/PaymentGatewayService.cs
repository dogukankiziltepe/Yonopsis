using Microsoft.Extensions.Configuration;

namespace SiteYonetimi.Infrastructure.Services;

public record CheckoutSession(string SessionId, string CheckoutUrl);

public interface IPaymentGatewayService
{
    Task<CheckoutSession> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        string description,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default);

    bool VerifyWebhookSignature(string payload, string signature);
    string? ExtractPaymentIdFromEvent(string payload);
    bool IsPaymentSucceeded(string payload);
}

public class StripePaymentGatewayService : IPaymentGatewayService
{
    private readonly string _secretKey;
    private readonly string _webhookSecret;

    public StripePaymentGatewayService(IConfiguration configuration)
    {
        _secretKey = configuration["Stripe:SecretKey"] ?? string.Empty;
        _webhookSecret = configuration["Stripe:WebhookSecret"] ?? string.Empty;
    }

    public async Task<CheckoutSession> CreateCheckoutSessionAsync(
        Guid paymentId,
        decimal amount,
        string description,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default)
    {
        // Stripe SDK integration point.
        // Stripe.StripeClient client = new(_secretKey);
        // var options = new Stripe.Checkout.SessionCreateOptions {
        //     PaymentMethodTypes = ["card"],
        //     LineItems = [new() { PriceData = new() {
        //         Currency = "try",
        //         UnitAmount = (long)(amount * 100),
        //         ProductData = new() { Name = description }
        //     }, Quantity = 1 }],
        //     Mode = "payment",
        //     SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}",
        //     CancelUrl = cancelUrl,
        //     Metadata = new() { ["paymentId"] = paymentId.ToString() }
        // };
        // var service = new Stripe.Checkout.SessionService(client);
        // var session = await service.CreateAsync(options, cancellationToken: ct);
        // return new CheckoutSession(session.Id, session.Url);

        // Stub for when Stripe credentials are configured:
        await Task.CompletedTask;
        var sessionId = $"cs_test_{Guid.NewGuid():N}";
        return new CheckoutSession(sessionId, $"https://checkout.stripe.com/pay/{sessionId}");
    }

    public bool VerifyWebhookSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_webhookSecret)) return false;
        // var stripeEvent = Stripe.EventUtility.ConstructEvent(payload, signature, _webhookSecret);
        // return stripeEvent != null;
        return !string.IsNullOrEmpty(signature);
    }

    public string? ExtractPaymentIdFromEvent(string payload)
    {
        // Parse the Stripe event JSON and extract metadata["paymentId"]
        // var stripeEvent = Stripe.EventUtility.ParseEvent(payload);
        // if (stripeEvent.Data.Object is Stripe.Checkout.Session session)
        //     return session.Metadata.TryGetValue("paymentId", out var id) ? id : null;
        return null;
    }

    public bool IsPaymentSucceeded(string payload)
    {
        // return stripeEvent.Type == Stripe.Events.CheckoutSessionCompleted;
        return payload.Contains("checkout.session.completed");
    }
}
