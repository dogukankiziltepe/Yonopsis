using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Hesaplar.Commands;

public record UpdateHesapCommand(Guid SiteId, Guid Id, UpdateHesapDto Dto) : IRequest<Result>;

public class UpdateHesapCommandHandler : IRequestHandler<UpdateHesapCommand, Result>
{
    private readonly SharedTenantDbContext _db;

    public UpdateHesapCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateHesapCommand request, CancellationToken cancellationToken)
    {
        var hesap = await _db.HesapPlani
            .FirstOrDefaultAsync(h => h.SiteId == request.SiteId && h.Id == request.Id, cancellationToken);
        if (hesap is null)
            return Result.Failure("Hesap bulunamadı.");

        var dto = request.Dto;
        hesap.HesapAdi = dto.HesapAdi.Trim();
        hesap.HesapKategorisi = dto.HesapKategorisi;
        hesap.NormalBakiye = dto.NormalBakiye;
        hesap.FisKesilebilirMi = dto.FisKesilebilirMi;
        hesap.Aciklama = dto.Aciklama;
        hesap.VergiDairesi = dto.VergiDairesi;
        hesap.VergiNumarasi = dto.VergiNumarasi;
        hesap.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UpdateHesapDtoValidator : AbstractValidator<UpdateHesapDto>
{
    public UpdateHesapDtoValidator()
    {
        RuleFor(x => x.HesapAdi).NotEmpty().MaximumLength(200)
            .WithMessage("Hesap adı zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
