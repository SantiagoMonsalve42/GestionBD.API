using GestionBD.Application.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

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
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role // Importante para el mapeo de roles
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

                    TransformKeycloakRoles(context);

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

    private static void TransformKeycloakRoles(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity)
            return;

        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim?.Value != null)
        {
            try
            {
                using var document = JsonDocument.Parse(realmAccessClaim.Value);
                if (document.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrWhiteSpace(roleValue))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing realm_access: {ex.Message}");
            }
        }

        var resourceAccessClaim = identity.FindFirst("resource_access");
        if (resourceAccessClaim?.Value != null)
        {
            try
            {
                using var document = JsonDocument.Parse(resourceAccessClaim.Value);
                foreach (var resource in document.RootElement.EnumerateObject())
                {
                    if (resource.Value.TryGetProperty("roles", out var resourceRoles))
                    {
                        foreach (var role in resourceRoles.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrWhiteSpace(roleValue))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, $"{resource.Name}:{roleValue}"));
                            }
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing resource_access: {ex.Message}");
            }
        }
    }
}