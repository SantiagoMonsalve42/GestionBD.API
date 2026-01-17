namespace GestionBD.API.Configuration;

public sealed record CorsSettings
{
    public const string SectionName = "Cors";

    public string PolicyName { get; init; } = string.Empty;
    public string[] AllowedOrigins { get; init; } = [];
    public string[] AllowedMethods { get; init; } = [];
    public string[] AllowedHeaders { get; init; } = [];
    public bool AllowCredentials { get; init; }
    public int MaxAge { get; init; }
}