using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OAuth2Api.Auth;
using OAuth2Api.Data;
using OAuth2Api.Services;

namespace OAuth2Api;

public static class ProgramConfig
{
    private static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ??
            throw new EnvVariableNotFoundException("JWT key is not configured.", "JWT_KEY");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        })
        .AddScheme<AuthenticationSchemeOptions, SecretAuthenticationHandler>("Secret", _ => { });

        return services;
    }

    private static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireSecret", policy =>
                policy.RequireClaim("ApiAccess", "true"));
        });

        return services;
    }

    public static IHostApplicationBuilder ConfigureEnvironment(this IHostApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName.ToLower();
        
        if (environment is "development" or "local")
            Env.Load();

        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return builder;
    }

    public static IHostApplicationBuilder ConfigureServices(this IHostApplicationBuilder builder)
    {
        // Database
        builder.Services.AddDbContext<UserContext>(options =>
            options.UseNpgsql(builder.Configuration["CONNECTION_STRING"]));

        // Application Services
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<TokenService>();

        // Authentication & Authorization
        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureAuthorization();

        // API Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        var environment = app.Environment.EnvironmentName.ToLower();

        if (environment is "development" or "local")
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
