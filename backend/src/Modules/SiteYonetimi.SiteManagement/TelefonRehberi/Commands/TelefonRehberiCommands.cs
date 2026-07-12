using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.TelefonRehberi.DTOs;

namespace SiteYonetimi.SiteManagement.TelefonRehberi.Commands;

public record CreateTelefonRehberiCommand(Guid SiteId, CreateTelefonRehberiDto Dto) : IRequest<Result<Guid>>;
public class CreateTelefonRehberiCommandHandler : IRequestHandler<CreateTelefonRehberiCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateTelefonRehberiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateTelefonRehberiCommand request, CancellationToken ct)
    {
        var e = new Infrastructure.Entities.Shared.TelefonRehberi { SiteId = request.SiteId, Ad = request.Dto.Ad, Unvan = request.Dto.Unvan, Telefon = request.Dto.Telefon, Dahili = request.Dto.Dahili, Email = request.Dto.Email, Departman = request.Dto.Departman, Aciklama = request.Dto.Aciklama };
        _db.TelefonRehberi.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateTelefonRehberiDtoValidator : AbstractValidator<CreateTelefonRehberiDto>
{
    public CreateTelefonRehberiDtoValidator() { RuleFor(x => x.Ad).NotEmpty().MaximumLength(200); RuleFor(x => x.Telefon).NotEmpty().MaximumLength(50); }
}

public record UpdateTelefonRehberiCommand(Guid Id, Guid SiteId, UpdateTelefonRehberiDto Dto) : IRequest<Result<bool>>;
public class UpdateTelefonRehberiCommandHandler : IRequestHandler<UpdateTelefonRehberiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTelefonRehberiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateTelefonRehberiCommand request, CancellationToken ct)
    {
        var entity = await _db.TelefonRehberi.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.Ad = request.Dto.Ad; entity.Unvan = request.Dto.Unvan; entity.Telefon = request.Dto.Telefon; entity.Dahili = request.Dto.Dahili;
        entity.Email = request.Dto.Email; entity.Departman = request.Dto.Departman; entity.Aciklama = request.Dto.Aciklama; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteTelefonRehberiCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteTelefonRehberiCommandHandler : IRequestHandler<DeleteTelefonRehberiCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteTelefonRehberiCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteTelefonRehberiCommand request, CancellationToken ct)
    {
        var entity = await _db.TelefonRehberi.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Kayıt bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
