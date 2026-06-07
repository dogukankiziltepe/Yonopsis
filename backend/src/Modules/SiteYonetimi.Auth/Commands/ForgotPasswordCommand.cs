using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly MasterDbContext _db;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ForgotPasswordCommandHandler(MasterDbContext db, IEmailService emailService, IConfiguration configuration)
    {
        _db = db;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim(), cancellationToken);

        // Güvenlik: kullanıcı bulunamasa da başarılı dön
        if (user is null)
            return Result.Success();

        // Önceki kullanılmamış token'ları iptal et
        var existingTokens = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var t in existingTokens)
            t.IsUsed = true;

        var token = Guid.NewGuid().ToString("N");
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
        };
        _db.PasswordResetTokens.Add(resetToken);
        await _db.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/reset-password?token={token}";

        try
        {
            await _emailService.SendAsync(
                user.Email,
                "SiteYönetimi — Şifre Sıfırlama",
                $"Merhaba {user.FirstName} {user.LastName},\n\n" +
                $"Şifre sıfırlama talebinde bulundunuz.\n\n" +
                $"Aşağıdaki bağlantıya tıklayarak yeni şifrenizi belirleyebilirsiniz:\n" +
                $"{resetLink}\n\n" +
                "Bu bağlantı 1 saat geçerlidir. Talebi siz yapmadıysanız bu e-postayı dikkate almayınız.\n\n" +
                "SiteYönetimi Ekibi");
        }
        catch { /* E-posta gönderilse de gönderilmese de işlem başarılı sayılır */ }

        return Result.Success();
    }
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
    }
}
