using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SiteYonetimi.Auth.Services;

namespace SiteYonetimi.Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(AuthModule).Assembly);
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}
