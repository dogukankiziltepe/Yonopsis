using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.GiderTanimlari.DTOs;

namespace SiteYonetimi.SiteManagement.GiderTanimlari.Commands;

public record CreateGiderTanimiCommand(Guid SiteId, CreateGiderTanimiDto Dto) : IRequest<Result<Guid>>;

public class CreateGiderTanimiCommandHandler : IRequestHandler<CreateGiderTanimiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateGiderTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateGiderTanimiCommand request, CancellationToken cancellationToken)
    {
        var koduExists = await _db.GiderTanimlari.AnyAsync(
            x => x.SiteId == request.SiteId && x.GiderKodu == request.Dto.GiderKodu, cancellationToken);
        if (koduExists) return Result<Guid>.Failure("Bu gider kodu zaten kullanılıyor.");

        var entity = new GiderTanimi
        {
            SiteId = request.SiteId,
            GiderKodu = request.Dto.GiderKodu,
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            GiderGrubuId = request.Dto.GiderGrubuId,
            DagitimSekli = request.Dto.DagitimSekli,
            BosDairelereDagit = request.Dto.BosDairelereDagit,
            Kdv = request.Dto.Kdv,
            BorclandirilacakKisi = request.Dto.BorclandirilacakKisi,
            MuhasebeKodu = request.Dto.MuhasebeKodu,
            Order = request.Dto.Order
        };
        _db.GiderTanimlari.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateGiderTanimiDtoValidator : AbstractValidator<CreateGiderTanimiDto>
{
    public CreateGiderTanimiDtoValidator()
    {
        RuleFor(x => x.GiderKodu).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public record UpdateGiderTanimiCommand(Guid Id, Guid SiteId, UpdateGiderTanimiDto Dto) : IRequest<Result>;

public class UpdateGiderTanimiCommandHandler : IRequestHandler<UpdateGiderTanimiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public UpdateGiderTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateGiderTanimiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GiderTanimlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Expense definition not found.");

        var koduCakisiyor = await _db.GiderTanimlari.AnyAsync(
            x => x.SiteId == request.SiteId && x.Id != request.Id && x.GiderKodu == request.Dto.GiderKodu, cancellationToken);
        if (koduCakisiyor) return Result.Failure("Bu gider kodu zaten kullanılıyor.");

        entity.GiderKodu = request.Dto.GiderKodu;
        entity.Name = request.Dto.Name;
        entity.Description = request.Dto.Description;
        entity.GiderGrubuId = request.Dto.GiderGrubuId;
        entity.DagitimSekli = request.Dto.DagitimSekli;
        entity.BosDairelereDagit = request.Dto.BosDairelereDagit;
        entity.Kdv = request.Dto.Kdv;
        entity.BorclandirilacakKisi = request.Dto.BorclandirilacakKisi;
        entity.MuhasebeKodu = request.Dto.MuhasebeKodu;
        entity.IsActive = request.Dto.IsActive;
        entity.Order = request.Dto.Order;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteGiderTanimiCommand(Guid Id, Guid SiteId) : IRequest<Result>;

public class DeleteGiderTanimiCommandHandler : IRequestHandler<DeleteGiderTanimiCommand, Result>
{
    private readonly SharedTenantDbContext _db;
    public DeleteGiderTanimiCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteGiderTanimiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.GiderTanimlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, cancellationToken);
        if (entity == null) return Result.Failure("Expense definition not found.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
