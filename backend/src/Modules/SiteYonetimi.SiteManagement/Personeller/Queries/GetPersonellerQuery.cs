using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Personeller.DTOs;

namespace SiteYonetimi.SiteManagement.Personeller.Queries;

public record GetPersonellerQuery(
    Guid SiteId,
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedResult<PersonelDto>>>;

public class GetPersonellerQueryHandler : IRequestHandler<GetPersonellerQuery, Result<PaginatedResult<PersonelDto>>>
{
    private readonly SharedTenantDbContext _db;
    public GetPersonellerQueryHandler(SharedTenantDbContext db) => _db = db;

    public async Task<Result<PaginatedResult<PersonelDto>>> Handle(GetPersonellerQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Personeller.Where(x => x.SiteId == request.SiteId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            q = q.Where(x => x.Name.Contains(request.Search) ||
                              x.PersonelKodu.Contains(request.Search) ||
                              (x.Title != null && x.Title.Contains(request.Search)) ||
                              (x.Email != null && x.Email.Contains(request.Search)));
        if (request.IsActive.HasValue)
            q = q.Where(x => x.IsActive == request.IsActive);

        var total = await q.CountAsync(cancellationToken);
        var pageIds = await q
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var kimlikById = await _db.PersonelKimlikBilgileri
            .Where(x => pageIds.Contains(x.PersonelId))
            .ToDictionaryAsync(x => x.PersonelId, cancellationToken);

        var ilkTelefonById = await _db.PersonelTelefonlari
            .Where(x => pageIds.Contains(x.PersonelId))
            .OrderBy(x => x.CreatedAt)
            .GroupBy(x => x.PersonelId)
            .Select(g => new { PersonelId = g.Key, Phone = g.First().PhoneNumber })
            .ToDictionaryAsync(x => x.PersonelId, x => x.Phone, cancellationToken);

        var items = await q
            .Where(x => pageIds.Contains(x.Id))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(x =>
        {
            kimlikById.TryGetValue(x.Id, out var kimlik);
            ilkTelefonById.TryGetValue(x.Id, out var telefon);
            return new PersonelDto(
                x.Id, x.SiteId, x.PersonelKodu, x.Name, x.Firma, x.Title,
                kimlik?.TcKimlikNo, telefon ?? x.Phone, x.Email, kimlik?.DogumTarihi,
                x.Aciklama, x.StartDate, x.CikisTarihi, x.IsActive, x.CreatedAt, x.UpdatedAt);
        }).ToList();

        return Result<PaginatedResult<PersonelDto>>.Success(
            PaginatedResult<PersonelDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}
