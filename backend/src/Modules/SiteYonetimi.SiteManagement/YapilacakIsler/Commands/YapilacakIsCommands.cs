using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.YapilacakIsler.DTOs;

namespace SiteYonetimi.SiteManagement.YapilacakIsler.Commands;

public record CreateYapilacakIsCommand(Guid SiteId, CreateYapilacakIsDto Dto) : IRequest<Result<Guid>>;
public class CreateYapilacakIsCommandHandler : IRequestHandler<CreateYapilacakIsCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateYapilacakIsCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateYapilacakIsCommand request, CancellationToken ct)
    {
        var e = new YapilacakIs { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, AtananKisi = request.Dto.AtananKisi, Oncelik = request.Dto.Oncelik, TamamlanmaTarihi = request.Dto.TamamlanmaTarihi };
        _db.YapilacakIsler.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateYapilacakIsDtoValidator : AbstractValidator<CreateYapilacakIsDto>
{
    public CreateYapilacakIsDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateYapilacakIsCommand(Guid Id, Guid SiteId, UpdateYapilacakIsDto Dto) : IRequest<Result<bool>>;
public class UpdateYapilacakIsCommandHandler : IRequestHandler<UpdateYapilacakIsCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateYapilacakIsCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateYapilacakIsCommand request, CancellationToken ct)
    {
        var entity = await _db.YapilacakIsler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Yapılacak iş bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.AtananKisi = request.Dto.AtananKisi;
        entity.Oncelik = request.Dto.Oncelik; entity.TamamlanmaTarihi = request.Dto.TamamlanmaTarihi; entity.Durum = request.Dto.Durum;
        entity.IsActive = request.Dto.IsActive; entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct); return Result<bool>.Success(true);
    }
}

public record DeleteYapilacakIsCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteYapilacakIsCommandHandler : IRequestHandler<DeleteYapilacakIsCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteYapilacakIsCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteYapilacakIsCommand request, CancellationToken ct)
    {
        var entity = await _db.YapilacakIsler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Yapılacak iş bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
