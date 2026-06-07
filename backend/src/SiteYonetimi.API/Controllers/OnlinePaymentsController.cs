using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteYonetimi.SiteManagement.Payments.Commands;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.API.Controllers;

[Route("api/online-payments")]
public class OnlinePaymentsController : BaseController
{
    [HttpPost("{id:guid}/checkout")]
    public async Task<IActionResult> CreateCheckout(Guid id, [FromBody] CreateCheckoutSessionDto dto)
    {
        var result = await Mediator.Send(new CreateCheckoutSessionCommand(id, CurrentSiteId, dto.SuccessUrl, dto.CancelUrl));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok(new { checkoutUrl = result.Data });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault() ?? string.Empty;

        var result = await Mediator.Send(new HandlePaymentWebhookCommand(payload, signature));
        if (!result.IsSuccess) return BadRequest(new { message = result.Error });
        return Ok();
    }
}
