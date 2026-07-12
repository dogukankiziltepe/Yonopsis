using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Teklifler.DTOs;

namespace SiteYonetimi.SiteManagement.Teklifler.Commands;

public record CreateTeklifCommand(Guid SiteId, CreateTeklifDto Dto) : IRequest<Result<Guid>>;
public class CreateTeklifCommandHandler : IRequestHandler<CreateTeklifCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateTeklifCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<Guid>> Handle(CreateTeklifCommand request, CancellationToken ct)
    {
        var e = new Teklif { SiteId = request.SiteId, Baslik = request.Dto.Baslik, Aciklama = request.Dto.Aciklama, TedarikciAdi = request.Dto.TedarikciAdi, Tutar = request.Dto.Tutar, TeklifTarihi = request.Dto.TeklifTarihi, GecerlilikTarihi = request.Dto.GecerlilikTarihi, Notlar = request.Dto.Notlar };
        _db.Teklifler.Add(e); await _db.SaveChangesAsync(ct);
        return Result<Guid>.Success(e.Id);
    }
}
public class CreateTeklifDtoValidator : AbstractValidator<CreateTeklifDto>
{
    public CreateTeklifDtoValidator() { RuleFor(x => x.Baslik).NotEmpty().MaximumLength(300); }
}

public record UpdateTeklifCommand(Guid Id, Guid SiteId, UpdateTeklifDto Dto) : IRequest<Result<bool>>;
public class UpdateTeklifCommandHandler : IRequestHandler<UpdateTeklifCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public UpdateTeklifCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(UpdateTeklifCommand request, CancellationToken ct)
    {
        var entity = await _db.Teklifler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Teklif bulunamadı.");
        entity.Baslik = request.Dto.Baslik; entity.Aciklama = request.Dto.Aciklama; entity.TedarikciAdi = request.Dto.TedarikciAdi;
        entity.Tutar = request.Dto.Tutar; entity.TeklifTarihi = request.Dto.TeklifTarihi; entity.GecerlilikTarihi = request.Dto.GecerlilikTarihi;
        entity.Durum = request.Dto.Durum; entity.Notlar = request.Dto.Notlar; entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public record DeleteTeklifCommand(Guid Id, Guid SiteId) : IRequest<Result<bool>>;
public class DeleteTeklifCommandHandler : IRequestHandler<DeleteTeklifCommand, Result<bool>>
{
    private readonly SharedTenantDbContext _db;
    public DeleteTeklifCommandHandler(SharedTenantDbContext db) => _db = db;
    public async Task<Result<bool>> Handle(DeleteTeklifCommand request, CancellationToken ct)
    {
        var entity = await _db.Teklifler.FirstOrDefaultAsync(x => x.Id == request.Id && x.SiteId == request.SiteId, ct);
        if (entity is null) return Result<bool>.Failure("Teklif bulunamadı.");
        entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
