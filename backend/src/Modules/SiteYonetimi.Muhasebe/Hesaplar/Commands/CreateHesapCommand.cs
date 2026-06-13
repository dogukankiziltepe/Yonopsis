using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Hesaplar.Commands;

public record CreateHesapCommand(Guid SiteId, CreateHesapDto Dto) : IRequest<Result<Guid>>;

public class CreateHesapCommandHandler : IRequestHandler<CreateHesapCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;

    public CreateHesapCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateHesapCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var kod = dto.HesapKodu.Trim();

        var kodVar = await _db.HesapPlani
            .AnyAsync(h => h.SiteId == request.SiteId && h.HesapKodu == kod, cancellationToken);
        if (kodVar)
            return Result<Guid>.Failure($"'{kod}' hesap kodu zaten kullanılıyor.");

        HesapPlani? parent = null;
        var seviye = 1;
        if (dto.ParentId is not null)
        {
            parent = await _db.HesapPlani
                .FirstOrDefaultAsync(h => h.SiteId == request.SiteId && h.Id == dto.ParentId, cancellationToken);
            if (parent is null)
                return Result<Guid>.Failure("Üst hesap bulunamadı.");
            seviye = parent.Seviye + 1;
        }

        var hesap = new HesapPlani
        {
            SiteId = request.SiteId,
            HesapKodu = kod,
            HesapAdi = dto.HesapAdi.Trim(),
            HesapTipi = HesapTipi.Tekduzen,
            HesapKategorisi = dto.HesapKategorisi,
            NormalBakiye = dto.NormalBakiye,
            Seviye = seviye,
            ParentId = parent?.Id,
            FisKesilebilirMi = dto.FisKesilebilirMi,
            Aciklama = dto.Aciklama,
            AktifMi = true
        };

        _db.HesapPlani.Add(hesap);

        // Üst hesap artık yaprak değil → fiş kesilemez hale getir.
        if (parent is not null && parent.FisKesilebilirMi)
        {
            parent.FisKesilebilirMi = false;
            parent.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(hesap.Id);
    }
}

public class CreateHesapDtoValidator : AbstractValidator<CreateHesapDto>
{
    public CreateHesapDtoValidator()
    {
        RuleFor(x => x.HesapKodu).NotEmpty().MaximumLength(50)
            .WithMessage("Hesap kodu zorunludur ve 50 karakteri geçemez.");
        RuleFor(x => x.HesapAdi).NotEmpty().MaximumLength(200)
            .WithMessage("Hesap adı zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
