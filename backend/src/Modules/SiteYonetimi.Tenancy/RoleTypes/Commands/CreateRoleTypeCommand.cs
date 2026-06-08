using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Tenancy.RoleTypes.DTOs;

namespace SiteYonetimi.Tenancy.RoleTypes.Commands;

public record CreateRoleTypeCommand(Guid SiteId, CreateRoleTypeDto Dto) : IRequest<Result<Guid>>;

public class CreateRoleTypeCommandHandler : IRequestHandler<CreateRoleTypeCommand, Result<Guid>>
{
    private readonly MasterDbContext _db;

    public CreateRoleTypeCommandHandler(MasterDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateRoleTypeCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.RoleTypes
            .AnyAsync(rt => rt.SiteId == request.SiteId && rt.Name == request.Dto.Name, cancellationToken);

        if (exists)
            return Result<Guid>.Failure("Bu isimde bir rol tipi zaten mevcut.");

        var role = new RoleType
        {
            SiteId = request.SiteId,
            Name = request.Dto.Name,
            IsDefault = false
        };

        _db.RoleTypes.Add(role);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(role.Id);
    }
}

public class CreateRoleTypeCommandValidator : AbstractValidator<CreateRoleTypeCommand>
{
    public CreateRoleTypeCommandValidator()
    {
        RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(100).WithMessage("Rol adı zorunludur ve 100 karakteri geçemez.");
    }
}
