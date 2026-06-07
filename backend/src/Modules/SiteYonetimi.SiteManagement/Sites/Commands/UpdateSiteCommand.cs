using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Sites.DTOs;

namespace SiteYonetimi.SiteManagement.Sites.Commands;

public record UpdateSiteCommand(Guid Id, UpdateSiteDto Dto) : IRequest<Result>;

public class UpdateSiteCommandHandler : IRequestHandler<UpdateSiteCommand, Result>
{
    private readonly MasterDbContext _db;

    public UpdateSiteCommandHandler(MasterDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
    {
        var site = await _db.Sites.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (site is null)
            return Result.Failure("Site bulunamadı.");

        var dto = request.Dto;
        site.Name = dto.Name;
        site.Address = dto.Address;
        site.District = dto.District;
        site.City = dto.City;
        site.PostalCode = dto.PostalCode;
        site.Phone = dto.Phone;
        site.Email = dto.Email;
        site.TaxOffice = dto.TaxOffice;
        site.TaxNumber = dto.TaxNumber;
        site.IsActive = dto.IsActive;
        site.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class UpdateSiteDtoValidator : AbstractValidator<UpdateSiteDto>
{
    public UpdateSiteDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Site adı zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.PostalCode).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.PostalCode));
        RuleFor(x => x.Phone).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.TaxNumber).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.TaxNumber));
    }
}
