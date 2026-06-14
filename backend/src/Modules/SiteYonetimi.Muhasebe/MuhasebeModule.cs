using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SiteYonetimi.Muhasebe.Donemler.Services;
using SiteYonetimi.Muhasebe.Fisler.Services;
using SiteYonetimi.Muhasebe.Hesaplar.Services;
using SiteYonetimi.Muhasebe.Parametreler.Services;
using SiteYonetimi.Muhasebe.Raporlar.Services;

namespace SiteYonetimi.Muhasebe;

public static class MuhasebeModule
{
    public static IServiceCollection AddMuhasebeModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MuhasebeModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(MuhasebeModule).Assembly);
        services.AddScoped<ICariHesapService, CariHesapService>();
        services.AddScoped<IFisService, FisService>();
        services.AddScoped<IRaporService, RaporService>();
        services.AddScoped<IParametreService, ParametreService>();
        services.AddScoped<IDonemSonuService, DonemSonuService>();
        return services;
    }
}
