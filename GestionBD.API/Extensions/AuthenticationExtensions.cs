using GestionBD.Application.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GestionBD.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var keycloakSettings = serviceProvider.GetRequiredService<IOptions<KeycloakSettings>>().Value;

            options.Authority = keycloakSettings.Authority;
            options.Audience = keycloakSettings.Audience;
            options.RequireHttpsMetadata = keycloakSettings.RequireHttpsMetadata == "true";
            options.MetadataAddress = $"{keycloakSettings.Authority}/.well-known/openid-configuration";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = keycloakSettings.ValidateIssuer == "true",
                ValidateAudience = keycloakSettings.ValidateAudience == "true",
                ValidateLifetime = keycloakSettings.ValidateLifetime == "true",
                ValidateIssuerSigningKey = true,
                ValidIssuer = keycloakSettings.Authority,
                ValidAudience = keycloakSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    
                    logger.LogError(context.Exception, 
                        "Error de autenticación: {Message}", context.Exception.Message);
                    
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    
                    logger.LogInformation(
                        "Token validado exitosamente para usuario: {User}", 
                        context.Principal?.Identity?.Name ?? "Unknown");
                    
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    
                    logger.LogWarning(
                        "Challenge emitido: {Error}, {ErrorDescription}", 
                        context.Error, context.ErrorDescription);
                    
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuthenticatedUser", policy =>
            {
                policy.RequireAuthenticatedUser();
            });

            // Políticas personalizadas basadas en roles de Keycloak
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("admin");
            });

            options.AddPolicy("UserOrAdmin", policy =>
            {
                policy.RequireRole("user", "admin");
            });
        });

        return services;
    }
}