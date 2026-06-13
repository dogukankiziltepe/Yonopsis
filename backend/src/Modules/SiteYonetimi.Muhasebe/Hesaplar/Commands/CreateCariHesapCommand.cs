using FluentValidation;
using MediatR;
using SiteYonetimi.Muhasebe.Hesaplar.DTOs;
using SiteYonetimi.Muhasebe.Hesaplar.Services;
using SiteYonetimi.Shared.Common;

namespace SiteYonetimi.Muhasebe.Hesaplar.Commands;

/// <summary>
/// Manuel cari hesap ekleme (kiracı/ev sahibi/tedarikçi/personel). Kod
/// otomatik üretilir; PersonId verilirse idempotenttir.
/// </summary>
public record CreateCariHesapCommand(Guid SiteId, CreateCariHesapDto Dto) : IRequest<Result<Guid>>;

public class CreateCariHesapCommandHandler : IRequestHandler<CreateCariHesapCommand, Result<Guid>>
{
    private readonly ICariHesapService _cariHesapService;

    public CreateCariHesapCommandHandler(ICariHesapService cariHesapService)
    {
        _cariHesapService = cariHesapService;
    }

    public Task<Result<Guid>> Handle(CreateCariHesapCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        return _cariHesapService.EnsureCariHesapAsync(
            request.SiteId, dto.CariTuru, dto.HesapAdi, dto.PersonId, dto.Aciklama, cancellationToken);
    }
}

public class CreateCariHesapDtoValidator : AbstractValidator<CreateCariHesapDto>
{
    public CreateCariHesapDtoValidator()
    {
        RuleFor(x => x.HesapAdi).NotEmpty().MaximumLength(200)
            .WithMessage("Cari hesap adı zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.CariTuru).IsInEnum().WithMessage("Geçerli bir cari türü seçilmelidir.");
        RuleFor(x => x.Aciklama).MaximumLength(500);
    }
}
