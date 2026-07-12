using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Bankalar.DTOs;

namespace SiteYonetimi.SiteManagement.Bankalar.Commands;

public record CreateBankaCommand(CreateBankaDto Dto) : IRequest<Result<Guid>>;

public class CreateBankaCommandHandler : IRequestHandler<CreateBankaCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;
    public CreateBankaCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateBankaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.Name))
            return Result<Guid>.Failure("Banka adı zorunludur.");

        var exists = await _db.Bankalar.AnyAsync(x => x.Name == request.Dto.Name.Trim(), cancellationToken);
        if (exists) return Result<Guid>.Failure("Bu banka zaten kayıtlı.");

        var entity = new Banka { Name = request.Dto.Name.Trim() };
        _db.Bankalar.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public record UpdateBankaCommand(Guid Id, UpdateBankaDto Dto) : IRequest<Result>;

public class UpdateBankaCommandHandler : IRequestHandler<UpdateBankaCommand, Result>
{
    private readonly MasterDbContext _db;
    public UpdateBankaCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateBankaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Bankalar.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return Result.Failure("Banka bulunamadı.");

        entity.Name = request.Dto.Name.Trim();
        entity.IsActive = request.Dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteBankaCommand(Guid Id) : IRequest<Result>;

public class DeleteBankaCommandHandler : IRequestHandler<DeleteBankaCommand, Result>
{
    private readonly MasterDbContext _db;
    public DeleteBankaCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteBankaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Bankalar.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null) return Result.Failure("Banka bulunamadı.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
