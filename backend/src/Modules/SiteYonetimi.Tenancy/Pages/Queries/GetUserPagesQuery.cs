using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Shared.Common;
using SiteYonetimi.Shared.Enums;
using SiteYonetimi.Tenancy.Pages.DTOs;

namespace SiteYonetimi.Tenancy.Pages.Queries;

public record GetUserPagesQuery(Guid UserId, Guid SiteId, Guid? RoleTypeId, bool IsSuperAdmin) : IRequest<Result<List<PageDto>>>;

public class GetUserPagesQueryHandler : IRequestHandler<GetUserPagesQuery, Result<List<PageDto>>>
{
    private readonly MasterDbContext _db;
    private readonly IMemoryCache _cache;

    public GetUserPagesQueryHandler(MasterDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result<List<PageDto>>> Handle(GetUserPagesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"pages_{request.UserId}_{request.SiteId}";
        if (_cache.TryGetValue(cacheKey, out List<PageDto>? cached) && cached != null)
            return Result<List<PageDto>>.Success(cached);

        // Check if the role is the default admin role (bypass subscription + give FullAccess)
        bool isDefaultRole = false;
        if (!request.IsSuperAdmin && request.RoleTypeId.HasValue)
        {
            isDefaultRole = await _db.RoleTypes
                .AnyAsync(rt => rt.Id == request.RoleTypeId.Value && rt.IsDefault, cancellationToken);
        }

        // Get accessible module IDs from active subscription
        List<Guid>? accessibleModuleIds = null;

        if (!request.IsSuperAdmin && !isDefaultRole)
        {
            var subscription = await _db.SiteSubscriptions
                .Where(s => s.SiteId == request.SiteId && s.IsActive && s.EndDate >= DateTime.UtcNow)
                .Select(s => s.SubscriptionPlanId)
                .FirstOrDefaultAsync(cancellationToken);

            if (subscription == Guid.Empty)
                return Result<List<PageDto>>.Success(new List<PageDto>());

            accessibleModuleIds = await _db.SubscriptionPlanModules
                .Where(spm => spm.SubscriptionPlanId == subscription)
                .Select(spm => spm.ModuleId)
                .ToListAsync(cancellationToken);

            if (accessibleModuleIds.Count == 0)
                return Result<List<PageDto>>.Success(new List<PageDto>());
        }

        // Get all active pages (query filter already applies !IsDeleted && IsActive)
        var pagesQuery = _db.Pages
            .Include(p => p.Module)
            .AsQueryable();

        if (accessibleModuleIds != null)
            pagesQuery = pagesQuery.Where(p => accessibleModuleIds.Contains(p.ModuleId));

        var pages = await pagesQuery.ToListAsync(cancellationToken);

        // Get permission levels for the role
        Dictionary<Guid, PermissionLevel> permissionMap = new();

        if (!request.IsSuperAdmin && !isDefaultRole && request.RoleTypeId.HasValue)
        {
            var pageIds = pages.Select(p => p.Id).ToList();
            var permissions = await _db.RolePermissions
                .Where(rp => rp.RoleTypeId == request.RoleTypeId.Value && pageIds.Contains(rp.PageId))
                .ToListAsync(cancellationToken);

            permissionMap = permissions.ToDictionary(rp => rp.PageId, rp => rp.PermissionLevel);
        }

        var result = new List<PageDto>();

        foreach (var page in pages)
        {
            PermissionLevel userPermission;

            if (request.IsSuperAdmin || isDefaultRole)
            {
                userPermission = PermissionLevel.FullAccess;
            }
            else if (!request.RoleTypeId.HasValue)
            {
                // Owner/Renter without a role → ReadOnly
                userPermission = PermissionLevel.ReadOnly;
            }
            else if (permissionMap.TryGetValue(page.Id, out var level))
            {
                userPermission = level;
            }
            else
            {
                userPermission = PermissionLevel.Unauthorized;
            }

            if (!request.IsSuperAdmin && !isDefaultRole && userPermission < PermissionLevel.ReadOnly)
                continue;

            result.Add(new PageDto
            {
                Id = page.Id,
                Name = page.Name,
                Label = page.Label,
                Icon = page.Icon,
                Route = page.Route,
                Order = page.OrderIndex,
                ParentId = page.ParentPageId,
                UserPermission = userPermission
            });
        }

        result = result.OrderBy(p => p.Order).ToList();

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(2));

        return Result<List<PageDto>>.Success(result);
    }
}
