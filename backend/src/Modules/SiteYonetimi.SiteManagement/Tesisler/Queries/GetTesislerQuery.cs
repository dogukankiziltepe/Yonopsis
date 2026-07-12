using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Tesisler.DTOs;

namespace SiteYonetimi.SiteManagement.Tesisler.Queries;

public record GetTesislerQuery(Guid SiteId) : IRequest<Result<List<TesisDto>>>;

public class GetTesislerQueryHandler : IRequestHandler<GetTesislerQuery, Result<List<TesisDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetTesislerQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<List<TesisDto>>> Handle(GetTesislerQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.Tesisler
            .Where(x => x.SiteId == request.SiteId)
            .OrderBy(x => x.Order).ThenBy(x => x.Name)
            .Select(x => new TesisDto(x.Id, x.Name, x.Description, x.Kapasite, x.IsActive, x.Order, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<TesisDto>>.Success(items);
    }
}
