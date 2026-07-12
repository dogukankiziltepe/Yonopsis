using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.SmsSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.SmsSablonlari.Commands;

public record CreateSmsSablonuCommand(Guid SiteId, CreateSmsSablonuDto Dto) : IRequest<Result<Guid>>;
public class CreateSmsSablonuCommandHandler : IRequestHandler<CreateSmsSablonuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateSmsSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateSmsSablonuCommand request, CancellationToken ct)
    {
        var e = new SmsSablonu { SiteId = request.SiteId, Ad = request.Dto.Ad, Icerik = request.Dto.Icerik, Kategori = request.Dto.Kategori };
        _db.SmsSablonlari.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateSmsSablonuDtoValidator : AbstractValidator<CreateSmsSablonuDto>
{
    public CreateSmsSablonuDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(200); RuleFor(x => x.Icerik).NotEmpty().MaximumLength(500); }
}

public record UpdateSmsSablonuCommand(Guid Id, Guid SiteId, UpdateSmsSablonuDto Dto) : IRequest<Result<bool>>;
public class UpdateSmsSablonuCommandHandler : IRequestHandler<UpdateSmsSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateSmsSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateSmsSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.SmsSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("SMS şablonu bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Icerik = request.Dto.Icerik; entity.Kategori = request.Dto.Kategori; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteSmsSablonuCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteSmsSablonuCommandHandler : IRequestHandler<DeleteSmsSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteSmsSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteSmsSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.SmsSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("SMS şablonu bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
