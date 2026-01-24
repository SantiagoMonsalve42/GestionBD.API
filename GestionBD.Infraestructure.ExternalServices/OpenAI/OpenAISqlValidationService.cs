using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestionBD.Application.Configuration;
using GestionBD.Application.DTO.OpenAI;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GestionBD.Infraestructure.ExternalServices.OpenAI;

public sealed class OpenAISqlValidationService : ISqlValidationService
{
    private readonly HttpClient _httpClient;
    private readonly OpenIASettings _settings;
    private readonly ILogger<OpenAISqlValidationService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public OpenAISqlValidationService(
        HttpClient httpClient,
        IOptions<OpenIASettings> settings,
        ILogger<OpenAISqlValidationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ConfigureHttpClient();
    }

    public async Task<SqlValidation> ValidateScriptAsync(
        bool isSecuencial,
        string sqlScript,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sqlScript))
            throw new ArgumentException("El script SQL no puede estar vacío", nameof(sqlScript));

        try
        {
            var request = BuildValidationRequest(isSecuencial,sqlScript);
            
            _logger.LogInformation(
                "Enviando script SQL a OpenAI para validación. Longitud: {Length} caracteres", 
                sqlScript.Length);

         var response = await _httpClient.PostAsJsonAsync(
                "/v1/responses", 
                request, 
                JsonOptions, 
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var openAIResponse = await response.Content.ReadFromJsonAsync<OpenAIValidationResponse>(
                JsonOptions, 
                cancellationToken);

            if (openAIResponse == null)
                throw new InvalidOperationException("La respuesta de OpenAI es nula");

            var result = ParseToSqlValidation(openAIResponse);

            _logger.LogInformation(
                "Validación completada. IsValid: {IsValid}, Errores: {Errors}, Warnings: {Warnings}, Sugerencias: {Suggestions}",
                result.IsValid, result.Errors.Count, result.Warnings.Count, result.Suggestions.Count);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con OpenAI API");
            throw new InvalidOperationException("Error al comunicarse con OpenAI", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar la respuesta de OpenAI");
            throw new InvalidOperationException("Error al procesar la respuesta de OpenAI", ex);
        }
    }

    private void ConfigureHttpClient()
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseURL))
            throw new InvalidOperationException("OpenAI BaseURL no está configurada");

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new InvalidOperationException("OpenAI ApiKey no está configurada");

        if (string.IsNullOrWhiteSpace(_settings.Model))
            throw new InvalidOperationException("OpenAI Model no está configurada");

        _httpClient.BaseAddress = new Uri(_settings.BaseURL);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "responses=v1");
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    private OpenAIValidationRequest BuildValidationRequest(bool isSecuencial,string sqlScript)
    {
        var model = _settings.Model;
        var maxTokens = int.TryParse(_settings.MaxTokens, out var tokens) ? tokens : 600;

        var systemPrompt = "Eres un motor automático de validación SQL. No expliques razonamientos internos. Responde únicamente un objeto JSON válido.";
        
        var userPrompt = (!isSecuencial)? $@"CASO_USO: VALIDACION_SQL

                            REGLAS:
                            - No usar DROP sin IF EXISTS
                            - No usar SELECT *
                            - Detectar operaciones destructivas (DROP, TRUNCATE, DELETE sin WHERE)
                            - Transacciones deben usar BEGIN / COMMIT
                            - No usar GO dentro de SP o funciones
                            - Sugerir mejoras solo si son críticas
                            - Validar idempotencia

                            SCRIPT_SQL:
                            {sqlScript}

                            RESPONDE EXCLUSIVAMENTE CON ESTE OBJETO JSON, code y message siempre en español:
                            {{
                              ""isValid"": boolean,
                              ""errors"": [{{ ""code"": string, ""message"": string }}],
                              ""warnings"": [{{ ""code"": string, ""message"": string }}],
                              ""suggestions"": [{{ ""code"": string, ""message"": string }}]
                            }}"
                            : 
                            $@"CASO_USO: VALIDAR QUE EN CASO DE EJECUTAR LOS SCRIPT EN EL ORDEN ESTABLECIDO, TODO FUNCIONE

                            REGLAS:
                            - Validar que no haya conflictos entre scripts
                            - Validar que no haya dependencias en el orden establecido
                            - Validar idempotencia

                            SCRIPT_SQL:
                            {sqlScript}

                            RESPONDE EXCLUSIVAMENTE CON ESTE OBJETO JSON, code y message siempre en español:
                            {{
                              ""isValid"": boolean,
                              ""errors"": [{{ ""code"": string, ""message"": string }}],
                              ""warnings"": [{{ ""code"": string, ""message"": string }}],
                              ""suggestions"": [{{ ""code"": string, ""message"": string }}]
                            }}";

        return new OpenAIValidationRequest(
            Model: model,
            Input:
            [
                new OpenAIMessage(
                    Role: "system",
                    Content:
                    [
                        new OpenAIContent(Type: "input_text", Text: systemPrompt)
                    ]
                ),
                new OpenAIMessage(
                    Role: "user",
                    Content:
                    [
                        new OpenAIContent(Type: "input_text", Text: userPrompt)
                    ]
                )
            ],
            Text: new OpenAITextFormat(
                Format: new OpenAIFormatType(Type: "json_object")
            ),
            Reasoning: new OpenAIReasoning(Effort: "minimal"),
            MaxOutputTokens: maxTokens
        );
    }

    private static SqlValidation ParseToSqlValidation(OpenAIValidationResponse response)
    {
        var messageOutput = response.Output
            .FirstOrDefault(o => o.Type == "message" && o.Content != null);

        if (messageOutput?.Content == null || messageOutput.Content.Count == 0)
        {
            throw new InvalidOperationException(
                "No se encontró contenido de mensaje en la respuesta de OpenAI");
        }

        var textContent = messageOutput.Content.First().Text;
        
        var validationDto = JsonSerializer.Deserialize<ValidationDto>(
            textContent, 
            new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

        if (validationDto == null)
            throw new InvalidOperationException("No se pudo deserializar el resultado de validación");

        return SqlValidation.Create(
            validationDto.IsValid,
            validationDto.Errors?.Select(e => new ValidationError(e.Code, e.Message)) ?? [],
            validationDto.Warnings?.Select(w => new ValidationWarning(w.Code, w.Message)) ?? [],
            validationDto.Suggestions?.Select(s => new ValidationSuggestion(s.Code, s.Message)) ?? []
        );
    }

    // DTO interno solo para deserialización
    private sealed record ValidationDto(
        bool IsValid,
        List<ErrorDto>? Errors,
        List<WarningDto>? Warnings,
        List<SuggestionDto>? Suggestions
    );

    private sealed record ErrorDto(string Code, string Message);
    private sealed record WarningDto(string Code, string Message);
    private sealed record SuggestionDto(string Code, string Message);
}