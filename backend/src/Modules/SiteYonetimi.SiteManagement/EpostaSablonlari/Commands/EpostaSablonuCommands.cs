using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.EpostaSablonlari.DTOs;

namespace SiteYonetimi.SiteManagement.EpostaSablonlari.Commands;

public record CreateEpostaSablonuCommand(Guid SiteId, CreateEpostaSablonuDto Dto) : IRequest<Result<Guid>>;
public class CreateEpostaSablonuCommandHandler : IRequestHandler<CreateEpostaSablonuCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateEpostaSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateEpostaSablonuCommand request, CancellationToken ct)
    {
        var e = new EpostaSablonu { SiteId = request.SiteId, Ad = request.Dto.Ad, Konu = request.Dto.Konu, IcerikHtml = request.Dto.IcerikHtml, IcerikText = request.Dto.IcerikText, Kategori = request.Dto.Kategori };
        _db.EpostaSablonlari.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateEpostaSablonuDtoValidator : AbstractValidator<CreateEpostaSablonuDto>
{
    public CreateEpostaSablonuDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(200); RuleFor(x => x.Konu).NotEmpty().MaximumLength(300); RuleFor(x => x.IcerikHtml).NotEmpty(); }
}

public record UpdateEpostaSablonuCommand(Guid Id, Guid SiteId, UpdateEpostaSablonuDto Dto) : IRequest<Result<bool>>;
public class UpdateEpostaSablonuCommandHandler : IRequestHandler<UpdateEpostaSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateEpostaSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateEpostaSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.EpostaSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("E-posta şablonu bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Konu = request.Dto.Konu; entity.IcerikHtml = request.Dto.IcerikHtml;
        entity.IcerikText = request.Dto.IcerikText; entity.Kategori = request.Dto.Kategori; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteEpostaSablonuCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteEpostaSablonuCommandHandler : IRequestHandler<DeleteEpostaSablonuCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteEpostaSablonuCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteEpostaSablonuCommand request, CancellationToken ct)
    {
        var entity = await _db.EpostaSablonlari.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("E-posta şablonu bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
