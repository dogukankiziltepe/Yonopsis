using MediatR;
using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.SiteManagement.Bankalar.DTOs;

namespace SiteYonetimi.SiteManagement.Bankalar.Queries;

public record GetBankalarQuery : IRequest<Result<List<BankaDto>>>;

public class GetBankalarQueryHandler : IRequestHandler<GetBankalarQuery, Result<List<BankaDto>>>
{
    private readonly MasterDbContext _db;
    public GetBankalarQueryHandler(MasterDbContext db) => _db = db;

    public async Task<Result<List<BankaDto>>> Handle(GetBankalarQuery request, CancellationToken cancellationToken)
    {
        var bankalar = await _db.Bankalar
            .Include(x => x.Subeler)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var dtos = bankalar.Select(b => new BankaDto(
            b.Id, b.Name, b.IsActive,
            b.Subeler.Where(s => !s.IsDeleted).OrderBy(s => s.SubeAdi)
                .Select(s => new BankaSubesiDto(s.Id, s.BankaId, s.SubeAdi, s.SubeKodu, s.IsActive))
                .ToList()
        )).ToList();

        return Result<List<BankaDto>>.Success(dtos);
    }
}
