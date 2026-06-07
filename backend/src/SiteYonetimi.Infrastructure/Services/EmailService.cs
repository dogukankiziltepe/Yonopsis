using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SiteYonetimi.Infrastructure.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public class SendGridEmailService : IEmailService
{
    private readonly string _apiKey;

    public SendGridEmailService(IConfiguration configuration)
    {
        _apiKey = configuration["SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid:ApiKey configuration is missing.");
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress("goktug.turkan5@gmail.com", "SiteYonetimi");
        var toAddress = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);
        await client.SendEmailAsync(msg);
    }
}
