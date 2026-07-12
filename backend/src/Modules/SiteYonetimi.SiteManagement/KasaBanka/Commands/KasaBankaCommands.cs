using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.KasaBanka.DTOs;

namespace SiteYonetimi.SiteManagement.KasaBanka.Commands;

public record CreateKasaBankaCommand(Guid SiteId, CreateKasaBankaDto Dto) : IRequest<Result<Guid>>;

public class CreateKasaBankaCommandHandler : IRequestHandler<CreateKasaBankaCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateKasaBankaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateKasaBankaCommand request, CancellationToken cancellationToken)
    {
        var entity = new Infrastructure.Entities.Shared.KasaBanka
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            Tip = request.Dto.Tip,
            BankaAdi = request.Dto.BankaAdi,
            SubeAdi = request.Dto.SubeAdi,
            HesapNo = request.Dto.HesapNo,
            IBAN = request.Dto.IBAN
        };
        _db.KasaBanka.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateKasaBankaDtoValidator : AbstractValidator<CreateKasaBankaDto>
{
    public CreateKasaBankaDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public record UpdateKasaBankaCommand(Guid Id, Guid SiteId, UpdateKasaBankaDto Dto) : IRequest<Result>;

public class UpdateKasaBankaCommandHandler : IRequestHandler<UpdateKasaBankaCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateKasaBankaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateKasaBankaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.KasaBanka.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Cash/bank account not found.");

        entity.Name = request.Dto.Name;
        entity.Tip = request.Dto.Tip;
        entity.BankaAdi = request.Dto.BankaAdi;
        entity.SubeAdi = request.Dto.SubeAdi;
        entity.HesapNo = request.Dto.HesapNo;
        entity.IBAN = request.Dto.IBAN;
        entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteKasaBankaCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteKasaBankaCommandHandler : IRequestHandler<DeleteKasaBankaCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteKasaBankaCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteKasaBankaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.KasaBanka.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Cash/bank account not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
