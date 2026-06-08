using FluentValidation;
using MediatR;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Entities.Shared;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Announcements.DTOs;

namespace SiteYonetimi.SiteManagement.Announcements.Commands;

public record CreateAnnouncementCommand(Guid SiteId, Guid UserId, CreateAnnouncementDto Dto) : IRequest<Result<Guid>>;

public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, Result<Guid>>
{
    private readonly SharedTenantDbContext _db;
    public CreateAnnouncementCommandHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var entity = new Announcement
        {
            SiteId = request.SiteId,
            CreatedByUserId = request.UserId,
            Title = request.Dto.Title,
            Content = request.Dto.Content,
            IsPinned = request.Dto.IsPinned,
            PublishDate = request.Dto.PublishDate,
            ExpiryDate = request.Dto.ExpiryDate
        };

        _db.Announcements.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}

public class CreateAnnouncementDtoValidator : AbstractValidator<CreateAnnouncementDto>
{
    public CreateAnnouncementDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200).WithMessage("Başlık zorunludur ve 200 karakteri geçemez.");
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000).WithMessage("İçerik zorunludur ve 5000 karakteri geçemez.");
    }
}
