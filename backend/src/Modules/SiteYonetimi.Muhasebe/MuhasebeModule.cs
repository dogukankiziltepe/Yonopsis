using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Muhasebe.Hesaplar.Services;

namespace SiteYonetimi.Muhasebe;

public static class MuhasebeModule
{
    public static IServiceCollection AddMuhasebeModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MuhasebeModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(MuhasebeModule).Assembly);
        services.AddScoped<ICariHesapService, CariHesapService>();
        services.AddScoped<IFisService, FisService>();
        return services;
    }
}
