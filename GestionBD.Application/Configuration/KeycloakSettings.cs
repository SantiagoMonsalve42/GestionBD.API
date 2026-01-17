namespace GestionBD.Application.Configuration;

public sealed record KeycloakSettings
{
    public const string SectionName = "Keycloak";

    public string Authority { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string Realm { get; init; } = string.Empty;
    public string RequireHttpsMetadata { get; init; } = string.Empty;
    public string ValidateAudience { get; init; } = string.Empty;
    public string ValidateIssuer { get; init; } = string.Empty;
    public string ValidateLifetime { get; init; } = string.Empty;
}