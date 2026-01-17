namespace GestionBD.Application.Configuration;

public sealed class OpenIASettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseURL { get; set; } = string.Empty;
    public string MaxTokens { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
}
