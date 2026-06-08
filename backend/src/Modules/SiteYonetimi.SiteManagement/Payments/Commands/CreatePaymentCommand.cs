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

        var totalAmount = request.Dto.Items.Sum(i => i.Amount);

        var entity = new Payment
        {
            SiteId = request.SiteId,
            UnitId = request.Dto.UnitId,
            Amount = totalAmount,
            DueDate = request.Dto.DueDate,
            Description = request.Dto.Description,
            Items = request.Dto.Items.Select(i => new PaymentItem
            {
                Name = i.Name,
                Amount = i.Amount
            }).ToList()
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
        RuleFor(x => x.Items).NotEmpty().WithMessage("En az bir aidat kalemi girilmelidir.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Name).NotEmpty().MaximumLength(100).WithMessage("Kalem adı zorunludur.");
            item.RuleFor(i => i.Amount).GreaterThan(0).WithMessage("Kalem tutarı sıfırdan büyük olmalıdır.");
        });
        RuleFor(x => x.DueDate).NotEmpty();
    }
}
