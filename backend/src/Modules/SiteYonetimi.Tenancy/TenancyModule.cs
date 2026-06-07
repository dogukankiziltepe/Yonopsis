using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SiteYonetimi.Tenancy;

public static class TenancyModule
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TenancyModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(TenancyModule).Assembly);
        return services;
    }
}
