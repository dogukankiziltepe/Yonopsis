using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;

namespace SiteYonetimi.Muhasebe.Donemler.Commands;

public record CreateDonemCommand(Guid SiteId, int Yil, string? Ad) : IRequest<Result<Guid>>;

public class CreateDonemCommandHandler : IRequestHandler<CreateDonemCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;

    public CreateDonemCommandHandler(SharedTenantDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> Handle(CreateDonemCommand request, CancellationToken cancellationToken)
    {
        var varMi = await _db.MuhasebeDonemler
            .AnyAsync(d => d.SiteId == request.SiteId && d.Yil == request.Yil, cancellationToken);
        if (varMi)
            return Result<Guid>.Failure($"{request.Yil} yılı için dönem zaten mevcut.");

        var donem = new MuhasebeDonem
        {
            SiteId = request.SiteId,
            Yil = request.Yil,
            Ad = string.IsNullOrWhiteSpace(request.Ad) ? $"{request.Yil} Mali Yılı" : request.Ad.Trim(),
            BaslangicTarihi = new DateOnly(request.Yil, 1, 1),
            BitisTarihi = new DateOnly(request.Yil, 12, 31),
            Durum = DonemDurumu.Acik,
            SonYevmiyeNo = 0
        };

        _db.MuhasebeDonemler.Add(donem);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(donem.Id);
    }
}

public class CreateDonemDtoValidator : AbstractValidator<DTOs.CreateDonemDto>
{
    public CreateDonemDtoValidator()
    {
        RuleFor(x => x.Yil).InclusiveBetween(2000, 2100)
            .WithMessage("Geçerli bir mali yıl girilmelidir.");
        RuleFor(x => x.Ad).MaximumLength(100);
    }
}
