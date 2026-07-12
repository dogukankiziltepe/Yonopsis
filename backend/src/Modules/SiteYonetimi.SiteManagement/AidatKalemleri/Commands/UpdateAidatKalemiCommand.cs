using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AidatKalemleri.DTOs;

namespace SiteYonetimi.SiteManagement.AidatKalemleri.Commands;

public record UpdateAidatKalemiCommand(Guid Id, Guid SiteId, UpdateAidatKalemiDto Dto) : IRequest<Result>;

public class UpdateAidatKalemiCommandHandler : IRequestHandler<UpdateAidatKalemiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateAidatKalemiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAidatKalemiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.AidatKalemleri.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null || entity.SiteId != request.SiteId)
            return Result.Failure("Aidat kalemi bulunamadı.");

        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class UpdateAidatKalemiDtoValidator : AbstractValidator<UpdateAidatKalemiDto>
{
    public UpdateAidatKalemiDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Kalem adı zorunludur ve 100 karakteri geçemez.");
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
