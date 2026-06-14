using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Fisler.Commands;
using SiteYonetimi.Muhasebe.Fisler.DTOs;
using SiteYonetimi.Muhasebe.Parametreler.Services;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Shared.Events;

namespace SiteYonetimi.Muhasebe.Integration;

/// <summary>
/// Ödeme "ödendi" olduğunda, parametre açıksa otomatik Tahsil fişi üretir:
/// Borç Kasa (100 / parametre), Alacak ilgili dairenin cari hesabı (120.xx).
/// Parametre kapalıysa veya gerekli hesaplar yoksa sessizce atlanır.
/// </summary>
public class PaymentTahsilFisiHandler : INotificationHandler<PaymentPaidDomainEvent>
{
    private readonly SharedTenantDbContext _db;
    private readonly IParametreService _parametreService;
    private readonly ISender _sender;
    private readonly ILogger<PaymentTahsilFisiHandler> _logger;

    public PaymentTahsilFisiHandler(
        SharedTenantDbContext db,
        IParametreService parametreService,
        ISender sender,
        ILogger<PaymentTahsilFisiHandler> logger)
    {
        _db = db;
        _parametreService = parametreService;
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(PaymentPaidDomainEvent n, CancellationToken cancellationToken)
    {
        try
        {
            var p = await _parametreService.GetOrCreateAsync(n.SiteId, cancellationToken);
            if (!p.OtomatikTahsilFisi) return;

            // Kasa hesabı (parametre veya kod 100).
            var kasaId = p.VarsayilanKasaHesapId;
            if (kasaId is null)
            {
                var kasa = await _db.HesapPlani.FirstOrDefaultAsync(
                    h => h.SiteId == n.SiteId && h.HesapKodu == "100", cancellationToken);
                kasaId = kasa?.Id;
            }
            if (kasaId is null)
            {
                _logger.LogWarning("Otomatik tahsil: kasa hesabı bulunamadı (SiteId={SiteId}).", n.SiteId);
                return;
            }

            // Dairenin sorumlusu (kiracı varsa kiracı, yoksa ev sahibi) → cari hesap.
            var unit = await _db.Units.FirstOrDefaultAsync(
                u => u.SiteId == n.SiteId && u.Id == n.UnitId, cancellationToken);
            var personId = unit?.TenantUserId ?? unit?.OwnerUserId;
            if (personId is null)
            {
                _logger.LogWarning("Otomatik tahsil: daireye bağlı kişi yok (UnitId={UnitId}).", n.UnitId);
                return;
            }

            var cari = await _db.HesapPlani.FirstOrDefaultAsync(
                h => h.SiteId == n.SiteId && h.PersonId == personId
                    && h.HesapTipi == HesapTipi.Cari && h.AktifMi, cancellationToken);
            if (cari is null)
            {
                _logger.LogWarning("Otomatik tahsil: cari hesap bulunamadı (PersonId={PersonId}).", personId);
                return;
            }

            var dto = new CreateFisDto
            {
                FisTuru = FisTuru.Tahsil,
                FisTarihi = n.PaidDate,
                Aciklama = $"Otomatik tahsil — Ödeme {n.PaymentId}",
                Detaylar = new()
                {
                    new FisDetaySatirDto { HesapId = kasaId.Value, BorcTutar = n.Amount },
                    new FisDetaySatirDto { HesapId = cari.Id, AlacakTutar = n.Amount }
                }
            };

            var created = await _sender.Send(new CreateFisCommand(n.SiteId, dto), cancellationToken);
            if (created.IsSuccess)
                await _sender.Send(new OnaylaFisCommand(n.SiteId, created.Data), cancellationToken);
            else
                _logger.LogWarning("Otomatik tahsil fişi oluşturulamadı: {Error}", created.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Otomatik tahsil fişi üretiminde hata (PaymentId={PaymentId}).", n.PaymentId);
        }
    }
}
