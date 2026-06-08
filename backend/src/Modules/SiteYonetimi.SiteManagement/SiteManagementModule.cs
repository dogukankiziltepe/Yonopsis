using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SiteYonetimi.SiteManagement.Import.Services;

namespace SiteYonetimi.SiteManagement;

public static class SiteManagementModule
{
    public static IServiceCollection AddSiteManagementModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SiteManagementModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(SiteManagementModule).Assembly);
        services.AddScoped<ExcelTemplateService>();
        return services;
    }
}
