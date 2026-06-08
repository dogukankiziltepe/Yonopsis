using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SupportRequests.DTOs;

namespace SiteYonetimi.SiteManagement.SupportRequests.Commands;

public record CreateSupportRequestCommand(Guid SiteId, Guid UserId, CreateSupportRequestDto Dto) : IRequest<Result<Guid>>;

public class CreateSupportRequestCommandHandler : IRequestHandler<CreateSupportRequestCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateSupportRequestCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var unitExists = await _db.Units.FindAsync(new object[] { request.Dto.UnitId }, cancellationToken);
        if (unitExists == null || unitExists.SiteId != request.SiteId)
            return Result<Guid>.Failure("Daire bulunamadı.");

        var entity = new SupportRequest
        {
            SiteId = request.SiteId,
            UserId = request.UserId,
            UnitId = request.Dto.UnitId,
            Subject = request.Dto.Subject,
            Description = request.Dto.Description
        };

        _db.SupportRequests.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateSupportRequestDtoValidator : AbstractValidator<CreateSupportRequestDto>
{
    public CreateSupportRequestDtoValidator()
    {
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200).WithMessage("Konu zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
