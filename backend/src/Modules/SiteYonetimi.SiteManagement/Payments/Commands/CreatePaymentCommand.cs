using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Payments.DTOs;

namespace SiteYonetimi.SiteManagement.Payments.Commands;

public record CreatePaymentCommand(Guid SiteId, CreatePaymentDto Dto) : IRequest<Result<Guid>>;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreatePaymentCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var unitExists = await _db.Units.FindAsync(new object[] { request.Dto.UnitId }, cancellationToken);
        if (unitExists == null || unitExists.SiteId != request.SiteId)
            return Result<Guid>.Failure("Daire bulunamadı.");

        var entity = new Payment
        {
            SiteId = request.SiteId,
            UnitId = request.Dto.UnitId,
            Amount = request.Dto.Amount,
            DueDate = request.Dto.DueDate,
            Description = request.Dto.Description
        };

        _db.Payments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalıdır.");
        RuleFor(x => x.DueDate).NotEmpty();
    }
}
