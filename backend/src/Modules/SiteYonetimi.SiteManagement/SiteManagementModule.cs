using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SiteYonetimi.SiteManagement;

public static class SiteManagementModule
{
    public static IServiceCollection AddSiteManagementModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SiteManagementModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(SiteManagementModule).Assembly);
        return services;
    }
}
