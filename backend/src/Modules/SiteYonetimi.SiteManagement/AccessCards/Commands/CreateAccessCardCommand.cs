using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.AccessCards.DTOs;

namespace SiteYonetimi.SiteManagement.AccessCards.Commands;

public record CreateAccessCardCommand(Guid SiteId, CreateAccessCardDto Dto) : IRequest<Result<Guid>>;

public class CreateAccessCardCommandHandler : IRequestHandler<CreateAccessCardCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAccessCardCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateAccessCardCommand request, CancellationToken cancellationToken)
    {
        var entity = new AccessCard
        {
            SiteId = request.SiteId,
            UserId = request.Dto.UserId,
            CardNumber = request.Dto.CardNumber,
            IssueDate = request.Dto.IssueDate,
            ExpiryDate = request.Dto.ExpiryDate,
            Notes = request.Dto.Notes
        };

        _db.AccessCards.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateAccessCardDtoValidator : AbstractValidator<CreateAccessCardDto>
{
    public CreateAccessCardDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CardNumber).NotEmpty().MaximumLength(50).WithMessage("Kart numarası zorunludur ve 50 karakteri geçemez.");
    }
}
