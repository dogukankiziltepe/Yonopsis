using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using SiteYonetimi.API.Filters;
using SiteYonetimi.API.Middleware;
using SiteYonetimi.API.Services;
using SiteYonetimi.Auth;
using SiteYonetimi.Infrastructure.Data;
using SiteYonetimi.Infrastructure.Seed;
using SiteYonetimi.Infrastructure.Services;
using SiteYonetimi.Muhasebe;
using SiteYonetimi.SiteManagement;
using SiteYonetimi.Tenancy;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // DbContexts
    builder.Services.AddDbContext<MasterDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("MasterDb")));

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IDbConnectionResolver, DbConnectionResolver>();
    builder.Services.AddScoped<SharedTenantDbContext>(sp =>
    {
        var resolver = sp.GetRequiredService<IDbConnectionResolver>();
        var connStr = resolver.GetConnectionStringAsync().GetAwaiter().GetResult();
        var options = new DbContextOptionsBuilder<SharedTenantDbContext>()
            .UseSqlServer(connStr)
            .Options;
        return new SharedTenantDbContext(options);
    });

    // Modules
    builder.Services.AddAuthModule();
    builder.Services.AddTenancyModule();
    builder.Services.AddSiteManagementModule();
    builder.Services.AddMuhasebeModule();

    // JWT Authentication
    var jwtKey = builder.Configuration["Jwt:SecretKey"]!;
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    // CORS
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
    builder.Services.AddScoped<IPaymentGatewayService, StripePaymentGatewayService>();
    builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o => o.MultipartBodyLengthLimit = 10 * 1024 * 1024);
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("login", opt =>
        {
            opt.PermitLimit = 5;
            opt.Window = TimeSpan.FromMinutes(1);
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        });
        options.RejectionStatusCode = 429;
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(
                "{\"message\":\"Çok fazla deneme yaptınız. 1 dakika sonra tekrar deneyin.\"}",
                token);
        };
    });

    builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("MasterDb")!, name: "master-db")
        .AddSqlServer(builder.Configuration.GetConnectionString("SharedTenantDb")!, name: "shared-db");

    builder.Services.AddAuthorization();
    builder.Services.AddScoped<IPermissionService, PermissionService>();
    builder.Services.AddScoped<IEmailService, SendGridEmailService>();
    builder.Services.AddHostedService<OverduePaymentsBackgroundService>();
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<PermissionFilter>();
        options.Filters.Add<AuditLogFilter>();
    });

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Site Yonetimi API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    app.UseMiddleware<ValidationExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<MustChangePasswordMiddleware>();
    app.UseMiddleware<SubscriptionMiddleware>();
    app.MapControllers();
    app.MapHealthChecks("/health");

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        await db.Database.MigrateAsync();
        await DataSeeder.SeedAsync(db);

        var sharedDb = scope.ServiceProvider.GetRequiredService<SharedTenantDbContext>();
        await sharedDb.Database.MigrateAsync();
    }

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Uygulama baslatilirken hata olustu.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
