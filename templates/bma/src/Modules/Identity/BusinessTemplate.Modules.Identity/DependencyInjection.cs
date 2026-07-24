using System.Text;
using BusinessTemplate.Modules.Identity.Domain;
using BusinessTemplate.Modules.Identity.Infrastructure.Auth;
using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BusinessTemplate.Modules.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("ConnectionStrings:Database is missing.");

        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IUserRepository, IdentityRepository>();
        services.AddScoped<Features.RegisterUser.Handler>();
        services.AddScoped<Features.GetUserById.Handler>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(IdentityPermissions.UsersRead, p => p.RequireClaim("permission", IdentityPermissions.UsersRead))
            .AddPolicy(IdentityPermissions.UsersWrite, p => p.RequireClaim("permission", IdentityPermissions.UsersWrite));

        services.AddHealthChecks().AddDbContextCheck<IdentityDbContext>(name: "identity-db");

        return services;
    }
}
