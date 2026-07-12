using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Departmanlar.DTOs;

namespace SiteYonetimi.SiteManagement.Departmanlar.Commands;

public record CreateDepartmanCommand(Guid SiteId, CreateDepartmanDto Dto) : IRequest<Result<Guid>>;
public class CreateDepartmanCommandHandler : IRequestHandler<CreateDepartmanCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateDepartmanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateDepartmanCommand request, CancellationToken ct)
    {
        var dup = await _db.Departmanlar.AnyAsync(x => x.SiteId == request.SiteId && x.Ad == request.Dto.Ad, ct);
        if (dup) return Result<Guid>.Failure("Bu isimde bir departman zaten mevcut.");
        var e = new Departman { SiteId = request.SiteId, Ad = request.Dto.Ad, Aciklama = request.Dto.Aciklama };
        _db.Departmanlar.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateDepartmanDtoValidator : AbstractValidator<CreateDepartmanDto>
{
    public CreateDepartmanDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(100); }
}

public record UpdateDepartmanCommand(Guid Id, Guid SiteId, UpdateDepartmanDto Dto) : IRequest<Result<bool>>;
public class UpdateDepartmanCommandHandler : IRequestHandler<UpdateDepartmanCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateDepartmanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateDepartmanCommand request, CancellationToken ct)
    {
        var entity = await _db.Departmanlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Departman bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Aciklama = request.Dto.Aciklama; entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteDepartmanCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteDepartmanCommandHandler : IRequestHandler<DeleteDepartmanCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteDepartmanCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteDepartmanCommand request, CancellationToken ct)
    {
        var entity = await _db.Departmanlar.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Departman bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}
